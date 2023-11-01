using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApiAdvance.Entities;

namespace WebApiAdvance.DAL.Configurations
{
	public class ProductConfigurations : IEntityTypeConfiguration<Product>
	{
		public void Configure(EntityTypeBuilder<Product> builder)
		{
			builder.Property(p => p.Name)
				.IsRequired().
				HasMaxLength(100);


			//builder.HasOne(p => p.Brand)
			//	.WithMany(b => b.Products)
			//	.HasForeignKey(p => p.BrandId);
				
		}
	}
}
