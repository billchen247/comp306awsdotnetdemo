using AutoMapper;
using Example9WeekCityInfoAPI.DTOs;
using Example9WeekCityInfoAPI.Repositories;
using Example9WeekCityInfoLibrary.Models;
using Microsoft.AspNetCore.Mvc;

namespace Example9WeekCityInfoAPI.Controllers
{
    /// <summary>
    /// API controller for managing points of interest within cities
    /// </summary>
    [ApiController]
    [Route("api/cities/{cityId}/pointsofinterest")]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ICityInfoRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<PointsOfInterestController> _logger;

        public PointsOfInterestController(
            ICityInfoRepository repository,
            IMapper mapper,
            ILogger<PointsOfInterestController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all points of interest for a specific city
        /// </summary>
        /// <param name="cityId">The city ID</param>
        /// <returns>List of points of interest</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PointsOfInterestDto>>> GetPointsOfInterest(int cityId)
        {
            var cityExists = await _repository.CityExistsAsync(cityId);
            if (!cityExists)
            {
                _logger.LogWarning($"City with ID {cityId} not found");
                return NotFound();
            }

            _logger.LogInformation($"Getting points of interest for city {cityId}");
            var pointsOfInterest = await _repository.GetPointsOfInterestByCityAsync(cityId);
            return Ok(_mapper.Map<IEnumerable<PointsOfInterestDto>>(pointsOfInterest));
        }

        /// <summary>
        /// Get a specific point of interest
        /// </summary>
        /// <param name="cityId">The city ID</param>
        /// <param name="poiId">The point of interest ID</param>
        /// <returns>Point of interest details</returns>
        [HttpGet("{poiId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PointsOfInterestDto>> GetPointOfInterest(int cityId, int poiId)
        {
            var cityExists = await _repository.CityExistsAsync(cityId);
            if (!cityExists)
            {
                _logger.LogWarning($"City with ID {cityId} not found");
                return NotFound();
            }

            _logger.LogInformation($"Getting point of interest {poiId} for city {cityId}");
            var poi = await _repository.GetPointOfInterestByIdAsync(cityId, poiId);

            if (poi == null)
            {
                _logger.LogWarning($"Point of interest with ID {poiId} not found");
                return NotFound();
            }

            return Ok(_mapper.Map<PointsOfInterestDto>(poi));
        }

        /// <summary>
        /// Create a new point of interest for a city
        /// </summary>
        /// <param name="cityId">The city ID</param>
        /// <param name="createDto">Point of interest creation data</param>
        /// <returns>Created point of interest</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PointsOfInterestDto>> CreatePointOfInterest(
            int cityId,
            [FromBody] PointsOfInterestCreateDto createDto)
        {
            var cityExists = await _repository.CityExistsAsync(cityId);
            if (!cityExists)
            {
                _logger.LogWarning($"City with ID {cityId} not found");
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation($"Creating new point of interest for city {cityId}");
            
            var poi = _mapper.Map<PointsOfInterest>(createDto);
            poi.CityId = cityId;
            
            await _repository.AddPointOfInterestAsync(poi);
            await _repository.SaveChangesAsync();

            var result = _mapper.Map<PointsOfInterestDto>(poi);
            return CreatedAtAction(
                nameof(GetPointOfInterest),
                new { cityId, poiId = result.PointsOfInterestId },
                result);
        }

        /// <summary>
        /// Update a point of interest completely (PUT)
        /// </summary>
        /// <param name="cityId">The city ID</param>
        /// <param name="poiId">The point of interest ID</param>
        /// <param name="updateDto">Updated point of interest data</param>
        /// <returns>No content</returns>
        [HttpPut("{poiId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePointOfInterest(
            int cityId,
            int poiId,
            [FromBody] PointsOfInterestUpdateDto updateDto)
        {
            var cityExists = await _repository.CityExistsAsync(cityId);
            if (!cityExists)
            {
                _logger.LogWarning($"City with ID {cityId} not found");
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var poi = await _repository.GetPointOfInterestByIdAsync(cityId, poiId);
            if (poi == null)
            {
                _logger.LogWarning($"Point of interest with ID {poiId} not found");
                return NotFound();
            }

            _logger.LogInformation($"Updating point of interest {poiId} for city {cityId}");
            _mapper.Map(updateDto, poi);
            await _repository.UpdatePointOfInterestAsync(poi);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Partially update a point of interest (PATCH)
        /// </summary>
        /// <param name="cityId">The city ID</param>
        /// <param name="poiId">The point of interest ID</param>
        /// <param name="patchDocument">JSON Patch document</param>
        /// <returns>No content</returns>
        [HttpPatch("{poiId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PartiallyUpdatePointOfInterest(
            int cityId,
            int poiId,
            [FromBody] Microsoft.AspNetCore.JsonPatch.JsonPatchDocument<PointsOfInterestUpdateDto> patchDocument)
        {
            var cityExists = await _repository.CityExistsAsync(cityId);
            if (!cityExists)
            {
                _logger.LogWarning($"City with ID {cityId} not found");
                return NotFound();
            }

            var poi = await _repository.GetPointOfInterestByIdAsync(cityId, poiId);
            if (poi == null)
            {
                _logger.LogWarning($"Point of interest with ID {poiId} not found");
                return NotFound();
            }

            _logger.LogInformation($"Partially updating point of interest {poiId} for city {cityId}");
            
            var poiToPatch = _mapper.Map<PointsOfInterestUpdateDto>(poi);
            patchDocument.ApplyTo(poiToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(poiToPatch, poi);
            await _repository.UpdatePointOfInterestAsync(poi);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Delete a point of interest
        /// </summary>
        /// <param name="cityId">The city ID</param>
        /// <param name="poiId">The point of interest ID</param>
        /// <returns>No content</returns>
        [HttpDelete("{poiId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePointOfInterest(int cityId, int poiId)
        {
            var cityExists = await _repository.CityExistsAsync(cityId);
            if (!cityExists)
            {
                _logger.LogWarning($"City with ID {cityId} not found");
                return NotFound();
            }

            var poi = await _repository.GetPointOfInterestByIdAsync(cityId, poiId);
            if (poi == null)
            {
                _logger.LogWarning($"Point of interest with ID {poiId} not found");
                return NotFound();
            }

            _logger.LogInformation($"Deleting point of interest {poiId} for city {cityId}");
            await _repository.DeletePointOfInterestAsync(poi);
            await _repository.SaveChangesAsync();

            return NoContent();
        }
    }
}
