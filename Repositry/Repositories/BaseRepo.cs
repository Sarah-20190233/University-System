using Core.Models;
using Microsoft.EntityFrameworkCore;
using Core.InterfacesOfRepo;



namespace Infrastructure.Repositories
{
    public class BaseRepo<T> : IBaseRepo<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _context;

        public BaseRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<T>> GetAll(PaginationParams paginationParams)
        {
            return await _context.Set<T>()
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToListAsync();
        }

        //public async Task<List<T>> GetAll()
        //{
        //    return await _context.Set<T>()

        //        .ToListAsync();
        //}

        public async Task<T> GetById(string id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<bool> Add(T entity)
        {
            try
            {
                await _context.Set<T>().AddAsync(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Update( T updatedEntity)
        {
          
           // _context.Entry(entity).CurrentValues.SetValues(updatedEntity);
           _context.Set<T>().Update(updatedEntity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(string id)
        {
            var entity = await GetById(id);
            if (entity == null)
                return false;

            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }


}
