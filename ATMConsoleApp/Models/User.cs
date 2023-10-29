namespace ATMConsoleApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public long CardNumber { get; set; }
        public int CardPin { get; set; }
        public long AccountNumber { get; set; }
        public string FullName { get; set; }
        public decimal AccountBalance { get; set; }
    }
}
