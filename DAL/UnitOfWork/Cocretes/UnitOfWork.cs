using Microsoft.EntityFrameworkCore;
using WebApiAdvance.DAL.EfCore;
using WebApiAdvance.DAL.Repositories.Abstracts;
using WebApiAdvance.DAL.Repositories.Concretes.EfCore;
using WebApiAdvance.DAL.UnitOfWork.Abstracts;

namespace WebApiAdvance.DAL.UnitOfWork.Cocretes
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly AppDbContext _appDbContext;
		private IProductRepository _productRepository;
		private IBrandRepository _brandRepository;

		public UnitOfWork(AppDbContext appDbContext)
		{
			_appDbContext = appDbContext;
		}

		// 2 sual isaresi null olub olmadigini yoxlayir
		public IProductRepository ProductRepository =>_productRepository=_productRepository??new EfProductRepository(_appDbContext) ;

		public IBrandRepository BrandRepository => _brandRepository=_brandRepository?? new EfBrandRepository(_appDbContext);

		public async Task SaveAsync()
		{
			await _appDbContext.SaveChangesAsync();
		}
	}
}
