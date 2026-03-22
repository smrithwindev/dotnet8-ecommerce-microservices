using BuildingBlocks.Core.Responses;
using BuildingBlocks.Web.Logging;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using System;
using System.Linq.Expressions;

namespace ProductApi.Infrastructure.Repositories
{
    public class ProductRepository(ProductDbContext context) : IProductRepository
    {
        public async Task<Response> CreateAsync(Product entity)
        {
            try
            {
                // checking if the product already exists
                var getProduct = await GetByAsync(p => p.Name!.Equals(entity.Name));
                if (getProduct is not null && !string.IsNullOrEmpty(getProduct.Name))
                {
                    return new Response(false, $"{entity.Name} already added");
                }
                // here wer are checking the database if the product with the same name exists
                var currentEntity = context.Products.Add(entity).Entity;
                await context.SaveChangesAsync();

                if (currentEntity is not null && currentEntity.Id > 0)
                {
                    return new Response(true, "Product added to database successfully");
                }
                else
                {
                    return new Response(false, "Failed to add product to database");
                }
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogException.LogExceptions(ex);

                //display scary-free message to the client
                return new Response(false, "An error occurred while adding the product");
            }
        }

        public async Task<Response> DeleteAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);  // FindByIdAsync implementation has to be given in the below method
                if (product is null)
                {
                    return new Response(false, $"{entity.Name} not found");
                }

                context.Products.Remove(product);
                await context.SaveChangesAsync();
                return new Response(true, "Product deleted successfully");
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogException.LogExceptions(ex);

                //display scary-free message to the client
                return new Response(false, "An error occurred while deleting the product");
            }
        }

        public async Task<Product> FindByIdAsync(int id)
        {
            try
            {
                var product = await context.Products.FindAsync(id);
                return product is not null ? product : null!;
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogException.LogExceptions(ex);

                //display scary-free message to the client
                throw new Exception("An error occurred while retreiving the product");
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                var products = await context.Products.AsNoTracking().ToListAsync(); //ToListAsync() method is coming from EF Core
                return products is not null ? products : null!;
            }
            catch(Exception ex)
            {
                //Log the original exception
                LogException.LogExceptions(ex);

                //display scary-free message to the client
                throw new Exception("An error occurred while retreiving the product");
            }
        }

        public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
        {
            try
            {
                var product = await context.Products.Where(predicate).FirstOrDefaultAsync()!;
                return product is not null ? product : null!;
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogException.LogExceptions(ex);

                //display scary-free message to the client
                throw new Exception("An error occurred while retreiving the product");
            }
        }

        public async Task<Response> UpdateAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);
                if(product is null)
                {
                    return new Response(false, $"{entity.Name} not found");
                }

                context.Entry(product).State = EntityState.Detached;
                context.Products.Update(entity);
                await context.SaveChangesAsync();
                return new Response(true, $"{entity.Name} is updated successfully");
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogException.LogExceptions(ex);

                //display scary-free message to the client
                return new Response(false,"An error occurred while updating existing product");
            }
        }
    }
}
