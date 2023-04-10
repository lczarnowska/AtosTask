namespace AtosTask.Model
{
    public interface ICustomerRepository
    {
        Task<int> Add(CreateCustomerCommand customer);
        Task Delete(int id);
        Task<IEnumerable<Customer>> GetAll(CustomerQuery customerQuery);
        Task<bool> ExistCustomer(int id);
        Task<Customer?> GetById(int id);
        Task SaveChangesAsync();
    }
}
