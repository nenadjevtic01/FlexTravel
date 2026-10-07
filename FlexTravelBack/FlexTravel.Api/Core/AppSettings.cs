namespace FlexTravel.Api.Core
{
    public class AppSettings
    {
        public string ConnectionString { get; set; }
        public JwtSettings JwtSettings { get; set; }
        public ScheduledJobsSettings ScheduledJobs { get; set; } = new ScheduledJobsSettings();

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                throw new InvalidOperationException("Missing 'ConnectionString' in configuration.");
            }

            if (JwtSettings == null || string.IsNullOrWhiteSpace(JwtSettings.Issuer) || string.IsNullOrWhiteSpace(JwtSettings.SecretKey))
            {
                throw new InvalidOperationException("Missing 'JwtSettings' (Issuer, SecretKey, Minutes) in configuration.");
            }

            if (JwtSettings.SecretKey.Length < 32)
            {
                throw new InvalidOperationException("'JwtSettings:SecretKey' must be at least 32 characters for HMAC-SHA256.");
            }

            if (JwtSettings.Minutes <= 0)
            {
                throw new InvalidOperationException("'JwtSettings:Minutes' must be a positive number of minutes.");
            }
        }
    }

    public class ScheduledJobsSettings
    {
        public int IntervalMinutes { get; set; } = 15;
        public int StartDelaySeconds { get; set; } = 20;
    }
}
