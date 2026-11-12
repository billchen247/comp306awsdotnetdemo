using Example9WeekCityInfoLibrary.Data;
using Example9WeekCityInfoLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace Example9WeekCityInfoAPI.Repositories
{
    /// <summary>
    /// Repository implementation for CityInfo entity operations
    /// </summary>
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoDbContext _context;

        public CityInfoRepository(CityInfoDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region City Operations

        public async Task<IEnumerable<CityInfo>> GetAllCitiesAsync()
        {
            return await _context.Cities
                .OrderBy(c => c.CityName)
                .ToListAsync();
        }

        public async Task<CityInfo?> GetCityByIdAsync(int cityId)
        {
            return await _context.Cities
                .FirstOrDefaultAsync(c => c.CityId == cityId);
        }

        public async Task<CityInfo?> GetCityWithPointsOfInterestAsync(int cityId)
        {
            return await _context.Cities
                .Include(c => c.PointsOfInterest)
                .FirstOrDefaultAsync(c => c.CityId == cityId);
        }

        public async Task<bool> CityExistsAsync(int cityId)
        {
            return await _context.Cities
                .AnyAsync(c => c.CityId == cityId);
        }

        public async Task AddCityAsync(CityInfo city)
        {
            if (city == null)
                throw new ArgumentNullException(nameof(city));

            await _context.Cities.AddAsync(city);
        }

        public async Task UpdateCityAsync(CityInfo city)
        {
            if (city == null)
                throw new ArgumentNullException(nameof(city));

            _context.Cities.Update(city);
            await Task.CompletedTask;
        }

        public async Task DeleteCityAsync(CityInfo city)
        {
            if (city == null)
                throw new ArgumentNullException(nameof(city));

            _context.Cities.Remove(city);
            await Task.CompletedTask;
        }

        #endregion

        #region PointsOfInterest Operations

        public async Task<IEnumerable<PointsOfInterest>> GetPointsOfInterestByCityAsync(int cityId)
        {
            return await _context.PointsOfInterest
                .Where(p => p.CityId == cityId)
                .OrderBy(p => p.NameofPOI)
                .ToListAsync();
        }

        public async Task<PointsOfInterest?> GetPointOfInterestByIdAsync(int cityId, int poiId)
        {
            return await _context.PointsOfInterest
                .FirstOrDefaultAsync(p => p.CityId == cityId && p.PointsOfInterestId == poiId);
        }

        public async Task<bool> PointOfInterestExistsAsync(int cityId, int poiId)
        {
            return await _context.PointsOfInterest
                .AnyAsync(p => p.CityId == cityId && p.PointsOfInterestId == poiId);
        }

        public async Task AddPointOfInterestAsync(PointsOfInterest poi)
        {
            if (poi == null)
                throw new ArgumentNullException(nameof(poi));

            await _context.PointsOfInterest.AddAsync(poi);
        }

        public async Task UpdatePointOfInterestAsync(PointsOfInterest poi)
        {
            if (poi == null)
                throw new ArgumentNullException(nameof(poi));

            _context.PointsOfInterest.Update(poi);
            await Task.CompletedTask;
        }

        public async Task DeletePointOfInterestAsync(PointsOfInterest poi)
        {
            if (poi == null)
                throw new ArgumentNullException(nameof(poi));

            _context.PointsOfInterest.Remove(poi);
            await Task.CompletedTask;
        }

        #endregion

        #region Save Changes

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        #endregion
    }
}
