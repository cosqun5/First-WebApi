﻿namespace WebApiAdvance.Entities.Dtos
{
	public class UpdateProductDto
	{
        public int Id { get; set; }
        public string Name { get; set; }
		public double Price { get; set; }
		public int BrandId { get; set; }
		public string Description { get; set; }
	}

}
