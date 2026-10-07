namespace FlexTravel.Domain
{
    public class ScheduledJobRun
    {
        public int ScheduledJobRunId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }

        public string StartedBy { get; set; }
        public int ExpiredReservations { get; set; }
        public int PaymentReminders { get; set; }
        public int ReviewInvitations { get; set; }
        public int PriceAlerts { get; set; }
        public int ClosedWaitlists { get; set; }
        public string Error { get; set; }
    }
}
