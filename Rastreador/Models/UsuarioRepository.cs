using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Security;

namespace Rastreador.Models
{ 
    public class UsuarioRepository : IUsuarioRepository
    {
        RastreadorContext context = new RastreadorContext();

        public IQueryable<Usuario> All
        {
            get { return context.Usuario; }
        }

        public IQueryable<Usuario> AllIncluding(params Expression<Func<Usuario, object>>[] includeProperties)
        {
            IQueryable<Usuario> query = context.Usuario;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Usuario Find(System.Guid id)
        {
            return context.Usuario.Find(id);
        }

        public Usuario FindLogin(string login)
        {
            return context.Usuario.Where(x => x.Login == login).FirstOrDefault();
        }

        public void InsertOrUpdate(Usuario usuario)
        {
            if (context.Usuario.Where(x => x.Id == usuario.Id).Count() == 0)
            {
                // New entity
                context.Usuario.Add(usuario);
            }
            else
            {
                // Existing entity
                context.Entry(usuario).State = EntityState.Modified;
            }
        }

        public void Delete(System.Guid id)
        {
            var usuario = context.Usuario.Find(id);
            context.Usuario.Remove(usuario);
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

    public interface IUsuarioRepository : IDisposable
    {
        IQueryable<Usuario> All { get; }
        IQueryable<Usuario> AllIncluding(params Expression<Func<Usuario, object>>[] includeProperties);
        Usuario Find(System.Guid id);
        Usuario FindLogin(string login);
        void InsertOrUpdate(Usuario usuario);
        void Delete(System.Guid id);
        void Save();
    }
}