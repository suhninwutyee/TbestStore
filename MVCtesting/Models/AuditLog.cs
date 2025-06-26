namespace MVCtesting.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string UserId { get; set; }         // ID of the user who performed the action
        public string UserName { get; set; }       // Username or email
        public string Action { get; set; }         // Action performed (e.g. "Create Product")
        public string Controller { get; set; }     // Controller name
        public string IpAddress { get; set; }      // Optional: IP address of the request
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
