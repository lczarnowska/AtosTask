using AtosTask.Model;
using AtosTask.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtosTask.Tests
{
    public class CreateCustomerCommandTests
    {
        [Fact]
        public void Name_Empty_ReturnsError()
        {
            //Arrange
            CreateCustomerCommand command = new CreateCustomerCommand();
            command.Surname = "BB";
            command.FirstName = "";
            var checkPropertyValidation = new CheckPropertyValidation();

            //Act
            var result = checkPropertyValidation.ValidateObject(command);

            //Arrert
            Assert.Equal(1, result.Count);
            Assert.Contains("FirstName",result[0].MemberNames);
        }

        [Fact]
        public void Surname_Empty_ReturnsError()
        {
            //Arrange
            CreateCustomerCommand command = new CreateCustomerCommand();
            command.Surname = "";
            command.FirstName = "BB";
            var checkPropertyValidation = new CheckPropertyValidation();

            //Act
            var result = checkPropertyValidation.ValidateObject(command);

            //Arrert
            Assert.Equal(1, result.Count);
            Assert.Contains("Surname", result[0].MemberNames);
        }

        [Fact]
        public void SurnameAndName_MoreThen1Letter_ReturnsNoError()
        {
            //Arrange
            CreateCustomerCommand command = new CreateCustomerCommand();
            command.Surname = "AA";
            command.FirstName = "BB";
            var checkPropertyValidation = new CheckPropertyValidation();

            //Act
            var result = checkPropertyValidation.ValidateObject(command);

            //Arrert
            Assert.Equal(0, result.Count);            
        }
    }
}
