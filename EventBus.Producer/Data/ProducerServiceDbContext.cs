using Microsoft.EntityFrameworkCore;
using ProducerService.Entities;

namespace ProducerService.Data;

public class ProducerServiceDbContext : DbContext
{
	public ProducerServiceDbContext(DbContextOptions<ProducerServiceDbContext> options) : base(options)
	{
	}

	public DbSet<OrderEntity> Order { get; set; }
}
