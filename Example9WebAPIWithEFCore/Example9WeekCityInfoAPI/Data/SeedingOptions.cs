namespace Example9WeekCityInfoAPI.Data
{
    /// <summary>
    /// Configuration options for database seeding
    /// </summary>
    public class SeedingOptions
    {
        /// <summary>
        /// Whether to enable automatic seeding on startup
        /// </summary>
        public bool EnableAutoSeeding { get; set; } = true;

        /// <summary>
        /// Whether to clear existing data before seeding (use with caution!)
        /// </summary>
        public bool ClearExistingData { get; set; } = false;

        /// <summary>
        /// Number of retry attempts if seeding fails
        /// </summary>
        public int RetryAttempts { get; set; } = 3;

        /// <summary>
        /// Delay between retry attempts in milliseconds
        /// </summary>
        public int RetryDelayMilliseconds { get; set; } = 1000;
    }
}
