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
            if (customerQuery.PageSize > 10 || customerQuery.PageSize < 1)
            {
                customerQuery.PageSize = 10;
            }
            var result = await _customerRepository.GetAll(customerQuery);            
            return Ok(result);
        }


        [HttpGet("{customerId:int}", Name = "GetCustomerById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async Task<ActionResult<Customer>> GetCustomerById(int customerId)
        {
            if(customerId <= 0)
            {
                return BadRequest($"Customer id must be > 0");
            }            
            var result = await _customerRepository.GetById(customerId);
            if(result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult> Post([FromBody] CreateCustomerCommand customer)
        {
            var id = await _customerRepository.Add(customer);
            await _customerRepository.SaveChangesAsync();
            var addedCustomer = await _customerRepository.GetById(id);
            return CreatedAtAction(nameof(GetCustomerById), new { CustomerId = id }, addedCustomer);
        }

        [HttpDelete("{customerId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCustomerById(int customerId)
        {
            var result = await _customerRepository.GetById(customerId);
            if(result == null)
            {
                return NotFound();
            }
            await _customerRepository.Delete(customerId);
            await _customerRepository.SaveChangesAsync();
            return NoContent();
        }
    }
}
