using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAdvance.DAL.EfCore;
using WebApiAdvance.DAL.Repositories.Abstracts;
using WebApiAdvance.Entities;
using WebApiAdvance.Entities.Dtos;

namespace WebApiAdvance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

		public ProductsController(IProductRepository repository, IMapper mapper)
		{
            _repository = repository;
			_mapper = mapper;
		}

		// GET: api/Products
		[HttpGet]
        public async Task<ActionResult<IEnumerable<GetProductDto>>> GetProducts()
        {
          if (await _repository.GetAllAsync() == null)
          {
              return NotFound();
          }
          var result = await _repository.GetAllAsync();
            List<GetProductDto> getProductDtos = _mapper.Map<List<GetProductDto>>(result);
            return getProductDtos;
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetProductDto>> GetProduct(int id)
        {
          if (await _repository.GetAllAsync() == null)
          {
              return NotFound();
          }
            var product = await _repository.GetAsync(p=>p.Id==id);

            if (product == null)
            {
                return NotFound();
            }
			GetProductDto result = _mapper.Map<GetProductDto>(product);

			return result;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, UpdateProductDto productdto)
        {
            if (!await ProductExists(id))
            {
                return NotFound();

            }
          Product product = _mapper.Map<Product>(productdto);
             _repository.Update(product);
            await _repository.SaveAsync();
            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Product>> PostProduct([FromBody]CreateProductDto productdto)
        {
          Product product = _mapper.Map<Product>(productdto);
            product.Created = DateTime.UtcNow;
            await _repository.AddAsync(product);
            await _repository.SaveAsync();
            return NoContent();
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (await _repository.GetAllAsync() == null)
            {
                return NotFound();
            }
            var product = await _repository.GetAsync(p=>p.Id==id);
            if (product == null)
            {
                return NotFound();
            }

            _repository.Delete(product);
            await _repository.SaveAsync();

            return NoContent();
        }

        private async Task< bool> ProductExists(int id)
        {
            return await _repository.IsExistsAsync(p=>p.Id==id);
        }
    }
}
