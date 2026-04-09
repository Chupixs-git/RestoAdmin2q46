using System.Linq;
using RestoAdmin.Database;
using RestoAdmin.Models;

namespace RestoAdmin.Services
{
    public class CustomerService
    {
        private readonly AppDbContext _context;

        public CustomerService(AppDbContext context)
        {
            _context = context;
        }

        public Customer GetOrCreateCustomer(string name, string phone, string? email)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Phone == phone);

            if (customer == null)
            {
                customer = new Customer
                {
                    Name = name,
                    Phone = phone,
                    Email = email
                };
                _context.Customers.Add(customer);
                _context.SaveChanges();
            }
            else
            {
                customer.Name = name;
                customer.Email = email;
                _context.SaveChanges();
            }

            return customer;
        }
    }
}