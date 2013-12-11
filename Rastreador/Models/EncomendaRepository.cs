using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Rastreador;

namespace Rastreador.Models
{ 
    public class EncomendaRepository : IEncomendaRepository
    {
        RastreadorContext context = new RastreadorContext();

        public IQueryable<Encomenda> All
        {
            get { return context.Encomenda; }
        }

        public IQueryable<Encomenda> AllIncluding(params Expression<Func<Encomenda, object>>[] includeProperties)
        {
            IQueryable<Encomenda> query = context.Encomenda;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Encomenda Find(int id)
        {
            return context.Encomenda.Find(id);
        }

        public void InsertOrUpdate(Encomenda encomenda)
        {
            if (encomenda.Id == default(int)) {
                // New entity
                context.Encomenda.Add(encomenda);
            } else {
                // Existing entity
                context.Entry(encomenda).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var encomenda = context.Encomenda.Find(id);
            context.Encomenda.Remove(encomenda);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Dispose() 
        {
            context.Dispose();
        }
    }

    public interface IEncomendaRepository : IDisposable
    {
        IQueryable<Encomenda> All { get; }
        IQueryable<Encomenda> AllIncluding(params Expression<Func<Encomenda, object>>[] includeProperties);
        Encomenda Find(int id);
        void InsertOrUpdate(Encomenda encomenda);
        void Delete(int id);
        void Save();
    }
}