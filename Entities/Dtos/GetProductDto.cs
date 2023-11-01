namespace WebApiAdvance.Entities.Dtos
{
	public class GetProductDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public double Price { get; set; }
		public int BrandId { get; set; }
		public Brand? Brand { get; set; }
		public string Description { get; set; }
	}

}
