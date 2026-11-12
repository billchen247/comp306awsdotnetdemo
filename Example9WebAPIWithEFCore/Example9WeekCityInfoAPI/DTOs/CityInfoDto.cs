namespace Example9WeekCityInfoAPI.DTOs
{
    public class CityInfoDto
    {
        public int CityId { get; set; }
        public string CityName { get; set; }
        public string? Description { get; set; }
        public int PointsOfInterestCount 
        { 
            get { return PointsOfInterest?.Count ?? 0; } 
        }
        public ICollection<PointsOfInterestDto> PointsOfInterest { get; set; } = new List<PointsOfInterestDto>();
    }
}
