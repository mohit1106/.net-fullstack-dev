namespace modelFirstMVC.Models
{
    public class Student
    {
        public int Id { get; set; }   // Primary Key
        public required string Name { get; set; }
        public float Age { get; set; }
        public string? Department { get; set; }
        public string? City { get; set; }

        //public Hostel Hostel { get; set; }
    }
}
