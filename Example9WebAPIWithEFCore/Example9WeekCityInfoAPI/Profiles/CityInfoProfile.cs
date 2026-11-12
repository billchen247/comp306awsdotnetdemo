using AutoMapper;
using Example9WeekCityInfoAPI.DTOs;
using Example9WeekCityInfoLibrary.Models;

namespace Example9WeekCityInfoAPI.Profiles
{
    /// <summary>
    /// AutoMapper profile for mapping between entities and DTOs
    /// </summary>
    public class CityInfoProfile : Profile
    {
        public CityInfoProfile()
        {
            // CityInfo mappings
            CreateMap<CityInfo, CityInfoDto>();
            CreateMap<CityInfoCreateDto, CityInfo>();
            CreateMap<CityInfoUpdateDto, CityInfo>();
            CreateMap<CityInfo, CityInfoUpdateDto>();

            // PointsOfInterest mappings
            CreateMap<PointsOfInterest, PointsOfInterestDto>();
            CreateMap<PointsOfInterestCreateDto, PointsOfInterest>();
            CreateMap<PointsOfInterestUpdateDto, PointsOfInterest>();
            CreateMap<PointsOfInterest, PointsOfInterestUpdateDto>();
        }
    }
}
