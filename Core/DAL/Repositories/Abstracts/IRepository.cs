using System.Linq.Expressions;
using WebApiAdvance.Entities;

namespace WebApiAdvance.Core.DAL.Repositories.Abstracts
{
	public interface IRepository<T> where T : class ,new()
	{
		Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, params string[] includes);
		Task<T> GetAsync(Expression<Func<T, bool>> filter, params string[] includes);
		Task<List<T>> GetAllPaginatedAsync(int page, int size, Expression<Func<T, bool>> filter = null, params string[] includes);
		Task<bool> IsExistsAsync(Expression<Func<T, bool>> filter);

		Task AddAsync(T entity);
		void Update(T entity);
		void Delete(T entity);
		Task SaveAsync();
	}
}
