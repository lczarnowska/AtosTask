using System.ComponentModel.DataAnnotations;

namespace AtosTask.Model
{
    public class CreateCustomerCommand
    {
        [Required(ErrorMessage = "First name is required")]
        [MinLength(1, ErrorMessage = "First name should be at least 1 characters long")]
        public string FirstName { get; set; } = null!;
        [Required(ErrorMessage = "Surname is required")]
        [MinLength(1, ErrorMessage = "Surname should be at least 1 characters long")]
        public string Surname { get; set; } = null!;
    }
}
