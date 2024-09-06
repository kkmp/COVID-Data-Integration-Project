using Projekt.Data.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Projekt.Data.Repositories
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> Get(Func<T, bool> condition);
        Task Add(T element);
    }
}
