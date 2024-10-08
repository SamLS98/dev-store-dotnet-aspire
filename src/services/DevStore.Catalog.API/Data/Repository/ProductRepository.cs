using DevStore.Catalog.API.Models;
using DevStore.Core.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevStore.Catalog.API.Data.Repository
{
    public class ProductRepository(CatalogContext context) : IProductRepository
    {
        public IUnitOfWork UnitOfWork => context;

        public async Task<PagedResult<Product>> GetAll(int pageSize, int pageIndex, string query = null)
        {
            var catalogQuery = context.Products.AsQueryable();

            var catalog = await catalogQuery.AsNoTrackingWithIdentityResolution()
                                            .Where(x => EF.Functions.Like(x.Name, $"%{query}%"))
                                            .OrderBy(x => x.Name)
                                            .Skip(pageSize * (pageIndex - 1))
                                            .Take(pageSize).ToListAsync();

            var total = await catalogQuery.AsNoTrackingWithIdentityResolution()
                                          .Where(x => EF.Functions.Like(x.Name, $"%{query}%"))
                                          .CountAsync();


            return new PagedResult<Product>()
            {
                List = catalog,
                TotalResults = total,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Query = query
            };
        }

        public async Task<Product> GetById(Guid id)
        {
            return await context.Products.FindAsync(id);
        }

        public async Task<List<Product>> GetProductsById(string ids)
        {
            var idsGuid = ids.Split(',')
                .Select(id => (Ok: Guid.TryParse(id, out var x), Value: x));

            if (!idsGuid.All(nid => nid.Ok)) return [];

            var idsValue = idsGuid.Select(id => id.Value);

            return await context.Products.AsNoTracking()
                .Where(p => idsValue.Contains(p.Id) && p.Active).ToListAsync();
        }

        public void Add(Product product)
        {
            context.Products.Add(product);
        }

        public void Update(Product product)
        {
            context.Products.Update(product);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            context?.Dispose();
        }
    }
}