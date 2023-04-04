using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProducerService.Entities;

namespace ProducerService.Data.Builders;

public class OrderBuilder : IEntityTypeConfiguration<OrderEntity>
{
    #region Public Methods

    public void Configure(EntityTypeBuilder<OrderEntity> builder)
    {
        ConfigureOrderTable(builder);
    }

    #endregion

    #region Methods

    private void ConfigureOrderTable(EntityTypeBuilder<OrderEntity> builder)
    {
        builder.ToTable("order");
    }

    #endregion
}
