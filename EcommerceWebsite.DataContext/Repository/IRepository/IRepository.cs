﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceWebsite.DataContext.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(string includeProperties);        
        T Get(Expression<Func<T, bool>> filter);
        void Add(T entity);
        void Remove(T entity);        
    }
}
