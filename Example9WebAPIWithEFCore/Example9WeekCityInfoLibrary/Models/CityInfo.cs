namespace Example9WeekCityInfoLibrary.Models
{
    public class CityInfo
    {
        public int CityId { get; set; }
        public string CityName { get; set; }
        public string? Description { get; set; }

        // Navigation property for Entity Framework
        public ICollection<PointsOfInterest> PointsOfInterest { get; set; } = new List<PointsOfInterest>();
    }
}
