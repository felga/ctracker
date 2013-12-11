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
    public class TemplateRepository : ITemplateRepository
    {
        RastreadorContext context = new RastreadorContext();

        public IQueryable<Template> All
        {
            get { return context.Template; }
        }

        public IQueryable<Template> AllIncluding(params Expression<Func<Template, object>>[] includeProperties)
        {
            IQueryable<Template> query = context.Template;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Template Find(int id)
        {
            return context.Template.Find(id);
        }

        public void InsertOrUpdate(Template template)
        {
            if (template.Id == default(int)) {
                // New entity
                context.Template.Add(template);
            } else {
                // Existing entity
                context.Entry(template).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var template = context.Template.Find(id);
            context.Template.Remove(template);
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

    public interface ITemplateRepository : IDisposable
    {
        IQueryable<Template> All { get; }
        IQueryable<Template> AllIncluding(params Expression<Func<Template, object>>[] includeProperties);
        Template Find(int id);
        void InsertOrUpdate(Template template);
        void Delete(int id);
        void Save();
    }
}