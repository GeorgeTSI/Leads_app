namespace Leads_app.Entities
{
    public class Leads
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public string? DeliveredFrom { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? GeoLocation { get; set; }
        public string? Session { get; set; }
        public string? Source { get; set; }
        public string? Lander { get; set; }
        public string? AdditionalInfo { get; set; }
        public string? IPAddress { get; set; }
        public required int StatusId { get; set; }
        public DateTime StatusDateChange { get; set; }
    }
}
