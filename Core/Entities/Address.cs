namespace Core.Entities
{
    public class Address : BaseEntity
    {
        public required string Line1 { get; set; }
        public string? Line2 { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string Postal_code { get; set; }
        public required string Country { get; set; }





        
    }
}