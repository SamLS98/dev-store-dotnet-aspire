using DevStore.Core.Data;
using DevStore.Customers.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevStore.Customers.API.Data.Repository
{
    public class CustomerRepository(CustomerContext context) : ICustomerRepository
    {
        public IUnitOfWork UnitOfWork => context;

        public async Task<IEnumerable<Customer>> GetAll()
        {
            return await context.Customers.AsNoTracking().ToListAsync();
        }

        public Task<Customer> GetBySocialNumber(string ssn)
        {
            return context.Customers.FirstOrDefaultAsync(c => c.SocialNumber == ssn);
        }

        public void Add(Customer customer)
        {
            context.Customers.Add(customer);
        }

        public async Task<Address> GetAddressById(Guid id)
        {
            return await context.Addresses.FirstOrDefaultAsync(e => e.CustomerId == id);
        }

        public void AddAddress(Address address)
        {
            context.Addresses.Add(address);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            context.Dispose();
        }
    }
}