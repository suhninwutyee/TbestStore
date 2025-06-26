namespace MVCtesting.Services
{
    public interface IAuditLogService
    {
        Task LogAsync(string userId, string userName, string action, string controller, string ipAddress);
    }

}
