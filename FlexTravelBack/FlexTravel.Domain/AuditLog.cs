namespace FlexTravel.Domain
{
    public class AuditLog
    {
        public int AuditLogId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public DateTime ExecutionDate { get; set; }
        public string UseCaseName { get; set; }
        public string Data { get; set; }
        public bool IsAuthorized { get; set; }
    }
}
