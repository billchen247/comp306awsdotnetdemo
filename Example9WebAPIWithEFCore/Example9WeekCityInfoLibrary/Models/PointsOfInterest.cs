namespace Example9WeekCityInfoLibrary.Models
{
    public class PointsOfInterest
    {
        public int PointsOfInterestId { get; set; }
        public string NameofPOI { get; set; }
        public string? Description { get; set; }
        public int CityId { get; set; }

        // Navigation property for Entity Framework
        public CityInfo City { get; set; }
    }
}
