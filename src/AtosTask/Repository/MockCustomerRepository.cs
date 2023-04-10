using AtosTask.Model;
using Bogus;

namespace AtosTask.Repository
{
    public class MockCustomerRepository : ICustomerRepository
    {
        private readonly List<Customer> _customers;
        public MockCustomerRepository()
        {
            Randomizer.Seed = new Random(8675309);
            var userIds = 1;
            var testUsers = new Faker<Customer>()
                .CustomInstantiator(f => new Customer() { Id = userIds++ })
                .RuleFor(u => u.FirstName, (f, u) => f.Name.FirstName())
                .RuleFor(u => u.Surname, (f, u) => f.Name.LastName());
            _customers = testUsers.Generate(15);
        }

        public MockCustomerRepository(List<Customer> customers)
        {
            _customers = customers;
        }
        public async Task<int> Add(CreateCustomerCommand createCustomerCommand)
        {
            if (createCustomerCommand is null)
            {
                throw new ArgumentNullException(nameof(createCustomerCommand));
            }

            var newId = 1;
            if (_customers.Any())
            {
                newId = _customers.Max(x => x.Id) + 1;
            }
            _customers.Add(new Customer { Id = newId, FirstName = createCustomerCommand.FirstName, Surname = createCustomerCommand.Surname });
            return await Task.FromResult(newId);
        }

        public async Task Delete(int id)
        {
            if (id <= 0) throw new ArgumentException(nameof(id));
            var customer = _customers.FirstOrDefault(c => c.Id == id);            
            if(customer == null)
            {
                throw new InvalidDataException($"Customer not found with id = {id}");
            }
            await Task.FromResult(_customers.Remove(customer));
        }

        public async Task<bool> ExistCustomer(int id)
        {
            if(id <= 0) throw new ArgumentException(nameof(id));
            var isCustomer = _customers.Any(c => c.Id == id);
            return await Task.FromResult(isCustomer);
        }

        public async Task<IEnumerable<Customer>> GetAll(CustomerQuery customerQuery)
        {
            IOrderedEnumerable<Customer>? sortedCustomers = null;
            if(customerQuery.OrderBy == CustomerOrderBy.Id && customerQuery.Direction == OrderByDirection.Ascending)
            {
                sortedCustomers = _customers.OrderBy(l => l.Id);                
            }
            else if (customerQuery.OrderBy == CustomerOrderBy.FirstName && customerQuery.Direction == OrderByDirection.Ascending)
            {
                sortedCustomers = _customers.OrderBy(l => l.FirstName);
            }
            else if (customerQuery.OrderBy == CustomerOrderBy.Surname && customerQuery.Direction == OrderByDirection.Ascending)
            {
                sortedCustomers = _customers.OrderBy(l => l.Surname);
            }
            else if (customerQuery.OrderBy == CustomerOrderBy.Id && customerQuery.Direction == OrderByDirection.Descending)
            {
                sortedCustomers = _customers.OrderByDescending(l => l.Id);
            }
            else if (customerQuery.OrderBy == CustomerOrderBy.FirstName && customerQuery.Direction == OrderByDirection.Descending)
            {
                sortedCustomers = _customers.OrderByDescending(l => l.FirstName);
            }
            else
            {
                sortedCustomers = _customers.OrderByDescending(l => l.Surname);
            }
            var result = sortedCustomers.Skip(customerQuery.PageNumber * customerQuery.PageSize - customerQuery.PageSize)
            .Take(customerQuery.PageSize)
            .ToList();
            return await Task.FromResult(result);
        }

        public async Task<Customer?> GetById(int id)
        {
            if (id <= 0) throw new ArgumentException(nameof(id));
            var customer = _customers.SingleOrDefault(c => c.Id == id);
            return await Task.FromResult(customer);
        }

        public Task SaveChangesAsync()
        {
            return Task.CompletedTask;
        }
    }
}
