using Example9WeekCityInfoLibrary.Models;

namespace Example9WeekCityInfoAPI.Repositories
{
    /// <summary>
    /// Repository interface for CityInfo entity operations
    /// </summary>
    public interface ICityInfoRepository
    {
        // City operations
        Task<IEnumerable<CityInfo>> GetAllCitiesAsync();
        Task<CityInfo?> GetCityByIdAsync(int cityId);
        Task<CityInfo?> GetCityWithPointsOfInterestAsync(int cityId);
        Task<bool> CityExistsAsync(int cityId);
        Task AddCityAsync(CityInfo city);
        Task UpdateCityAsync(CityInfo city);
        Task DeleteCityAsync(CityInfo city);

        // PointsOfInterest operations
        Task<IEnumerable<PointsOfInterest>> GetPointsOfInterestByCityAsync(int cityId);
        Task<PointsOfInterest?> GetPointOfInterestByIdAsync(int cityId, int poiId);
        Task<bool> PointOfInterestExistsAsync(int cityId, int poiId);
        Task AddPointOfInterestAsync(PointsOfInterest poi);
        Task UpdatePointOfInterestAsync(PointsOfInterest poi);
        Task DeletePointOfInterestAsync(PointsOfInterest poi);

        // Save changes
        Task<bool> SaveChangesAsync();
    }
}
