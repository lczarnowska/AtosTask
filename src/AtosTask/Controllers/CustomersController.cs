using AtosTask.Model;
using Microsoft.AspNetCore.Mvc;

namespace AtosTask.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger _logger;
        public CustomersController(ICustomerRepository customerRepository,
                    ILogger<CustomersController> logger)
        {
            _customerRepository = customerRepository;
            _logger = logger;
        }


        [HttpGet(Name = "GetCustomerList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomerList([FromQuery] CustomerQuery customerQuery)
        {
            _logger.LogTrace("Getting customers with {customerQuery}", customerQuery);
            if (customerQuery.PageSize > 10 || customerQuery.PageSize < 1)
            {
                _logger.LogInformation("Page size was invalid {customerQuery.PageSize} - corrected to 10", customerQuery.PageSize);
                customerQuery.PageSize = 10;
            }
            var result = await _customerRepository.GetAll(customerQuery);
            _logger.LogTrace("Result for query is {result}", result);
            return Ok(result);
        }


        [HttpGet("{customerId:int}", Name = "GetCustomerById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async Task<ActionResult<Customer>> GetCustomerById(int customerId)
        {
            _logger.LogTrace("Getting customer by id {customerId}", customerId);
            if (customerId <= 0)
            {
                _logger.LogInformation("Customer id was less then 0 = {customerId}", customerId);
                return BadRequest($"Customer id must be > 0");
            }            
            var result = await _customerRepository.GetById(customerId);
            if(result == null)
            {
                _logger.LogInformation("Customer was not found with id = {customerId}", customerId);
                return NotFound();
            }
            _logger.LogTrace("Customer was found {result}", result);
            return Ok(result);
        }

        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult> Post([FromBody] CreateCustomerCommand customer)
        {
            _logger.LogTrace("Adding customer {customer}", customer);
            var id = await _customerRepository.Add(customer);
            await _customerRepository.SaveChangesAsync();
            var addedCustomer = await _customerRepository.GetById(id);
            _logger.LogTrace("Added customer with id = {id}", id);
            return CreatedAtAction(nameof(GetCustomerById), new { CustomerId = id }, addedCustomer);
        }

        [HttpDelete("{customerId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCustomerById(int customerId)
        {
            _logger.LogTrace("Deleting customer with id = {customerId}", customerId);
            var result = await _customerRepository.GetById(customerId);
            if(result == null)
            {
                _logger.LogInformation("Did not found customer with id = {customerId}", customerId);
                return NotFound();
            }
            await _customerRepository.Delete(customerId);
            await _customerRepository.SaveChangesAsync();
            _logger.LogTrace("Successfuly deleted customer with id = {customerId}", customerId);
            return NoContent();
        }
    }
}
