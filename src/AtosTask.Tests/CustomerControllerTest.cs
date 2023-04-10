using AtosTask.Controllers;
using AtosTask.Model;
using AtosTask.Repository;
using Bogus;
using Castle.Core.Resource;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Collections.Generic;

namespace AtosTask.Tests
{
    public class CustomerControllerTest
    {
        [Fact]        
        public void GetCustomer_WithCustomerId10_ReturnsCustomer()
        {
            // Arrange
            List<Customer> list = new List<Customer>();
            list.Add(new Customer() { FirstName = "A", Surname = "B", Id = 10 });            
            var mockRepository = new MockCustomerRepository(list);
            NullLoggerFactory factory = new NullLoggerFactory();
            var logger = factory.CreateLogger<CustomersController>();
            var controller = new CustomersController(mockRepository, logger);


            // Act
            var result = controller.GetCustomerById(10).Result;


            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<Customer>(okResult.Value);
            Assert.Equal(10, returnValue.Id);
        }

        [Fact]
        public void GetCustomer_WithCustomerId20_ReturnsNotFound()
        {
            // Arrange
            var mockRepository = new MockCustomerRepository(new List<Customer>());
            NullLoggerFactory factory = new NullLoggerFactory();
            var logger = factory.CreateLogger<CustomersController>();
            var controller = new CustomersController(mockRepository, logger);


            // Act
            var result = controller.GetCustomerById(20).Result;


            // Assert
            Assert.IsType<NotFoundResult>(result.Result);                        
        }

        [Fact]
        public void GetCustomer_WithCustomerIdMinus1_ReturnsBadRequest()
        {
            // Arrange
            var mockRepository = new MockCustomerRepository(new List<Customer>());
            NullLoggerFactory factory = new NullLoggerFactory();
            var logger = factory.CreateLogger<CustomersController>();
            var controller = new CustomersController(mockRepository, logger);


            // Act
            var result = controller.GetCustomerById(-1).Result;


            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Customer id must be > 0", ((ObjectResult)result.Result).Value);         
        }

        [Fact]
        public void DeleteCustomer_WithCustomerId20_ReturnsNotFound()
        {
            // Arrange
            var mockRepository = new MockCustomerRepository(new List<Customer>());
            NullLoggerFactory factory = new NullLoggerFactory();
            var logger = factory.CreateLogger<CustomersController>();
            var controller = new CustomersController(mockRepository, logger);


            // Act
            var result = controller.DeleteCustomerById(20).Result;


            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeleteCustomer_WithCustomerId10_ReturnsNotContent()
        {
            // Arrange
            List<Customer> list = new List<Customer>();
            list.Add(new Customer() { FirstName = "A", Surname = "B", Id = 10 });
            var mockRepository = new MockCustomerRepository(list);
            NullLoggerFactory factory = new NullLoggerFactory();
            var logger = factory.CreateLogger<CustomersController>();
            var controller = new CustomersController(mockRepository, logger);


            // Act
            var result = controller.DeleteCustomerById(10).Result;


            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.False(mockRepository.ExistCustomer(10).Result,"The customer was not deleted");
        }

        [Fact]
        public void PostCustomer_WithValidData_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var mockRepository = new MockCustomerRepository(new List<Customer>());
            NullLoggerFactory factory = new NullLoggerFactory();
            var logger = factory.CreateLogger<CustomersController>();
            var controller = new CustomersController(mockRepository, logger);
            var newCustomer = new CreateCustomerCommand { FirstName = "Asia", Surname = "Iksinska" };

            // Act
            var result = controller.Post(newCustomer).Result;


            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
            var createdResult = (CreatedAtActionResult)result;            
            Assert.NotNull(createdResult.RouteValues);
            Assert.Equal(1, ((object[])createdResult.RouteValues.Values)[0]);
            Assert.Equal("GetCustomerById", createdResult.ActionName);
            Assert.True(mockRepository.ExistCustomer(1).Result, "The customer was not added");
        }

        [Fact]
        public void GetCustomers_WithPageSizeMinus1_Returns10Customers()
        {
            // Arrange
            Randomizer.Seed = new Random(8675309);
            var userIds = 1;
            var testUsers = new Faker<Customer>()
                .CustomInstantiator(f => new Customer() { Id = userIds++ })
                .RuleFor(u => u.FirstName, (f, u) => f.Name.FirstName())
                .RuleFor(u => u.Surname, (f, u) => f.Name.LastName());
            List<Customer> list = testUsers.Generate(15);
            var mockRepository = new MockCustomerRepository(list);
            NullLoggerFactory factory = new NullLoggerFactory();
            var logger = factory.CreateLogger<CustomersController>();
            var controller = new CustomersController(mockRepository, logger);
            CustomerQuery customerQuery = new CustomerQuery();
            customerQuery.PageSize = -1;

            // Act
            var result = controller.GetCustomerList(customerQuery).Result;


            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var resultList = (((ObjectResult)result.Result).Value) as IEnumerable<Customer>;
            Assert.NotNull(resultList);
            Assert.Equal(10, resultList.ToList().Count);
        }

        [Fact]  
        public void GetCustomers_WithPageSize50_Returns10Customers()
        {
            // Arrange
            Randomizer.Seed = new Random(8675309);
            var userIds = 1;
            var testUsers = new Faker<Customer>()
                .CustomInstantiator(f => new Customer() { Id = userIds++ })
                .RuleFor(u => u.FirstName, (f, u) => f.Name.FirstName())
                .RuleFor(u => u.Surname, (f, u) => f.Name.LastName());
            List<Customer> list = testUsers.Generate(15);
            var mockRepository = new MockCustomerRepository(list);
            NullLoggerFactory factory = new NullLoggerFactory();
            var logger = factory.CreateLogger<CustomersController>();
            var controller = new CustomersController(mockRepository, logger);
            CustomerQuery customerQuery = new CustomerQuery();
            customerQuery.PageSize = 50;

            // Act
            var result = controller.GetCustomerList(customerQuery).Result;


            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var resultList = (((ObjectResult)result.Result).Value) as IEnumerable<Customer>;
            Assert.NotNull(resultList);
            Assert.Equal(10, resultList.ToList().Count);
        }

        [Fact]
        public void GetCustomers_WithPageSize10AndPageNumber3_Returns5Customers()
        {
            // Arrange
            Randomizer.Seed = new Random(8675309);
            var userIds = 1;
            var testUsers = new Faker<Customer>()
                .CustomInstantiator(f => new Customer() { Id = userIds++ })
                .RuleFor(u => u.FirstName, (f, u) => f.Name.FirstName())
                .RuleFor(u => u.Surname, (f, u) => f.Name.LastName());
            List<Customer> list = testUsers.Generate(25);
            var mockRepository = new MockCustomerRepository(list);
            NullLoggerFactory factory = new NullLoggerFactory();
            var logger = factory.CreateLogger<CustomersController>();
            var controller = new CustomersController(mockRepository, logger);
            CustomerQuery customerQuery = new CustomerQuery();
            customerQuery.PageNumber = 3;

            // Act
            var result = controller.GetCustomerList(customerQuery).Result;


            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var resultList = (((ObjectResult)result.Result).Value) as IEnumerable<Customer>;
            Assert.NotNull(resultList);
            Assert.Equal(5, resultList.ToList().Count);
        }

        [Fact]
        public void GetCustomers_WithPageSize10AndPageNumber1Ascending_Returns5CustomersFirstCoralie()
        {
            // Arrange
            Randomizer.Seed = new Random(8675309);
            var userIds = 1;
            var testUsers = new Faker<Customer>()
                .CustomInstantiator(f => new Customer() { Id = userIds++ })
                .RuleFor(u => u.FirstName, (f, u) => f.Name.FirstName())
                .RuleFor(u => u.Surname, (f, u) => f.Name.LastName());
            List<Customer> list = testUsers.Generate(5);
            var mockRepository = new MockCustomerRepository(list);
            NullLoggerFactory factory = new NullLoggerFactory();
            var logger = factory.CreateLogger<CustomersController>();
            var controller = new CustomersController(mockRepository, logger);
            CustomerQuery customerQuery = new CustomerQuery();
            customerQuery.OrderBy = CustomerOrderBy.FirstName;
            customerQuery.Direction = OrderByDirection.Ascending;

            // Act
            var result = controller.GetCustomerList(customerQuery).Result;


            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var resultList = (((ObjectResult)result.Result).Value) as IEnumerable<Customer>;
            Assert.NotNull(resultList);
            Assert.Equal("Coralie", resultList.ToList()[0].FirstName);
        }
        [Fact]
        public void GetCustomers_WithPageSize10AndPageNumber1Descending_Returns5CustomersLastCoralie()
        {
            // Arrange
            Randomizer.Seed = new Random(8675309);
            var userIds = 1;
            var testUsers = new Faker<Customer>()
                .CustomInstantiator(f => new Customer() { Id = userIds++ })
                .RuleFor(u => u.FirstName, (f, u) => f.Name.FirstName())
                .RuleFor(u => u.Surname, (f, u) => f.Name.LastName());
            List<Customer> list = testUsers.Generate(5);
            var mockRepository = new MockCustomerRepository(list);
            NullLoggerFactory factory = new NullLoggerFactory();
            var logger = factory.CreateLogger<CustomersController>();
            var controller = new CustomersController(mockRepository, logger);
            CustomerQuery customerQuery = new CustomerQuery();
            customerQuery.OrderBy = CustomerOrderBy.FirstName;
            customerQuery.Direction = OrderByDirection.Descending;

            // Act
            var result = controller.GetCustomerList(customerQuery).Result;


            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var resultList = (((ObjectResult)result.Result).Value) as IEnumerable<Customer>;
            Assert.NotNull(resultList);
            Assert.Equal("Coralie", resultList.ToList()[4].FirstName);
        }
        [Fact]
        public void GetCustomers_WithPageSize10AndPageNumber1Ascending_Returns5CustomersFirstSurnameCoralie()
        {
            // Arrange
            Randomizer.Seed = new Random(8675309);
            var userIds = 1;
            var testUsers = new Faker<Customer>()
                .CustomInstantiator(f => new Customer() { Id = userIds++ })
                .RuleFor(u => u.FirstName, (f, u) => f.Name.FirstName())
                .RuleFor(u => u.Surname, (f, u) => f.Name.LastName());
            List<Customer> list = testUsers.Generate(5);
            var mockRepository = new MockCustomerRepository(list);
            NullLoggerFactory factory = new NullLoggerFactory();
            var logger = factory.CreateLogger<CustomersController>();
            var controller = new CustomersController(mockRepository, logger);
            CustomerQuery customerQuery = new CustomerQuery();
            customerQuery.OrderBy = CustomerOrderBy.Surname;
            customerQuery.Direction = OrderByDirection.Ascending;

            // Act
            var result = controller.GetCustomerList(customerQuery).Result;


            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var resultList = (((ObjectResult)result.Result).Value) as IEnumerable<Customer>;
            Assert.NotNull(resultList);
            Assert.Equal("Bergstrom", resultList.ToList()[0].Surname);
        }

    }   
}