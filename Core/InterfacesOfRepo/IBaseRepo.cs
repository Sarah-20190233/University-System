using Core.Models.DTOs;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.InterfacesOfRepo
{
    public interface IBaseRepo<T> where T : BaseEntity
    {
        Task<List<T>> GetAll(PaginationParams paginationParams);

        //Task<List<T>> GetAll();
        Task<T> GetById(string id);
        Task<bool> Add(T entity);
        Task<bool> Update(T updatedEntity);
        Task<bool> Delete(string id);
    }



}
