namespace AtosTask.Model
{
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string Surname { get; set; } = null!;

        public override string ToString()
        {
            return $"{Id} {FirstName} {Surname}";
        }
    }
}
