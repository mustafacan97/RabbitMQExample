using ConsumerService.Abstractions;
using ConsumerService.Concretes;
using ConsumerService.IntegrationEvents.EventHandlers;
using ConsumerService.IntegrationEvents.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection(RabbitMQSettings.SectionName));
builder.Services.AddTransient<OrderCreatedSuccessIntegrationEvent>();
builder.Services.AddTransient<OrderCreatedFailedIntegrationEvent>();
builder.Services.AddScoped<IEventBus, RabbitMQEventBus>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{ 
    IEventBus eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
    eventBus.Subscribe<OrderCreatedSuccessIntegrationEvent, OrderCreatedSuccessIntegartionEventHandler>();
    eventBus.Subscribe<OrderCreatedFailedIntegrationEvent, OrderCreatedFailedIntegrationEventHandler>();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
