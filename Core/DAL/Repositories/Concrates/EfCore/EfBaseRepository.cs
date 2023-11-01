using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebApiAdvance.Core.DAL.Repositories.Abstracts;
using WebApiAdvance.DAL.EfCore;
using WebApiAdvance.Entities;

namespace WebApiAdvance.Core.DAL.Repositories.Concrates.EfCore
{
	//obyekt yaranmasin sadece miras verilsin deye abstract yaziriq
	public abstract class EfBaseRepository<TEntity,TContext> :IRepository<TEntity>
		where TEntity : class,new()
		where TContext:DbContext
	{
		private readonly AppDbContext _context;

		private readonly DbSet<TEntity> _dbSet;

		protected EfBaseRepository(AppDbContext context)
		{
			_context = context;
			_dbSet = _context.Set<TEntity>();
		}
		public async Task AddAsync(TEntity entity)
		{
			await _dbSet.AddAsync(entity);
		}
		public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter, params string[] includes)
		{
			IQueryable<TEntity> query = GetQuery(includes);
			return await query.Where(filter).FirstOrDefaultAsync();
		}


		public Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter = null, params string[] includes)
		{
			IQueryable<TEntity> query = GetQuery(includes);
			return filter == null
				? query.ToListAsync()
				: query.Where(filter).ToListAsync();

		}

		public Task<List<TEntity>> GetAllPaginatedAsync(int page, int size, Expression<Func<TEntity, bool>> filter, params string[] includes)
		{

			IQueryable<TEntity> query = GetQuery(includes);
			return filter == null
				? query.Skip((page - 1) * size).Take(size).ToListAsync()
				: query.Where(filter).Skip((page - 1) * size).Take(size).ToListAsync();
		}

		public async Task<bool> IsExistsAsync(Expression<Func<TEntity, bool>> filter)
		{
			return await _dbSet.AnyAsync(filter);
		}



		public void Delete(TEntity product)
		{
			_dbSet.Remove(product);

		}

		public void Update(TEntity product)
		{
			_dbSet.Update(product);
		}
		private IQueryable<TEntity> GetQuery(string[] includes)
		{
			IQueryable<TEntity> query = _dbSet;
			foreach (var item in includes)
			{
				query=query.Include(item);
			}

			return query;
		}

	}
}
