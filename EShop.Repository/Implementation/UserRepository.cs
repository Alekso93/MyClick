using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eshop.Web.Data;
using EShop.Domain.Identity;
using EShop.Repository.Interface;
using Microsoft.EntityFrameworkCore;


namespace EShop.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext context;
        private DbSet<EShopApplicationUser> entities;
        string errorMessage = string.Empty;

        public UserRepository(Eshop.Web.Data.ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<EShopApplicationUser>();
        }

        public IEnumerable<EShopApplicationUser> GetAll()
        {
            return entities.AsEnumerable();
        }
        public EShopApplicationUser Get(string id)
        {
            return entities
                .Include(z => z.UserCart)
                .Include("UserCart.ProductinShoppingCart")
                .Include("UserCart.ProductinShoppingCart.Product")
                .SingleOrDefault(s => s.Id == id);
        }
        public void Insert(EShopApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Add(entity);
            context.SaveChanges();
        }
        public void Delete(EShopApplicationUser entity)
        {

            entities.Remove(entity);
            context.SaveChanges();
        }
        public void Remove(EShopApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Remove(entity);
            context.SaveChanges();

        }
        public void SaveChanges()
        {
            context.SaveChanges();
        }
        public void Update(EShopApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            context.Update(entity);
            context.SaveChanges();

        }
    }
}
