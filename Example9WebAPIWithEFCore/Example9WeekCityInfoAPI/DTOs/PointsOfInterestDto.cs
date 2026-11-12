namespace Example9WeekCityInfoAPI.DTOs
{
    public class PointsOfInterestDto
    {
        public int PointsOfInterestId { get; set; }
        public string NameofPOI { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CityId { get; set; }
    }
}
