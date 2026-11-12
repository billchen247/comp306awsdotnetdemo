using AutoMapper;
using Example9WeekCityInfoAPI.DTOs;
using Example9WeekCityInfoAPI.Repositories;
using Example9WeekCityInfoLibrary.Models;
using Microsoft.AspNetCore.Mvc;

namespace Example9WeekCityInfoAPI.Controllers
{
    /// <summary>
    /// API controller for managing cities
    /// </summary>
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityInfoRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<CitiesController> _logger;

        public CitiesController(
            ICityInfoRepository repository,
            IMapper mapper,
            ILogger<CitiesController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all cities
        /// </summary>
        /// <returns>List of all cities</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CityInfoDto>>> GetCities()
        {
            _logger.LogInformation("Getting all cities");
            var cities = await _repository.GetAllCitiesAsync();
            return Ok(_mapper.Map<IEnumerable<CityInfoDto>>(cities));
        }

        /// <summary>
        /// Get a specific city by ID
        /// </summary>
        /// <param name="cityId">The city ID</param>
        /// <returns>City details with its points of interest</returns>
        [HttpGet("{cityId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CityInfoDto>> GetCity(int cityId)
        {
            _logger.LogInformation($"Getting city with ID {cityId}");
            var city = await _repository.GetCityWithPointsOfInterestAsync(cityId);
            
            if (city == null)
            {
                _logger.LogWarning($"City with ID {cityId} not found");
                return NotFound();
            }

            return Ok(_mapper.Map<CityInfoDto>(city));
        }

        /// <summary>
        /// Create a new city
        /// </summary>
        /// <param name="createDto">City creation data</param>
        /// <returns>Created city</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CityInfoDto>> CreateCity([FromBody] CityInfoCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation($"Creating new city: {createDto.CityName}");
            
            var city = _mapper.Map<CityInfo>(createDto);
            await _repository.AddCityAsync(city);
            await _repository.SaveChangesAsync();

            var result = _mapper.Map<CityInfoDto>(city);
            return CreatedAtAction(nameof(GetCity), new { cityId = result.CityId }, result);
        }

        /// <summary>
        /// Update a city completely (PUT)
        /// </summary>
        /// <param name="cityId">The city ID</param>
        /// <param name="updateDto">Updated city data</param>
        /// <returns>No content</returns>
        [HttpPut("{cityId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCity(int cityId, [FromBody] CityInfoUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var city = await _repository.GetCityByIdAsync(cityId);
            if (city == null)
            {
                _logger.LogWarning($"City with ID {cityId} not found for update");
                return NotFound();
            }

            _logger.LogInformation($"Updating city with ID {cityId}");
            _mapper.Map(updateDto, city);
            await _repository.UpdateCityAsync(city);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Partially update a city (PATCH)
        /// </summary>
        /// <param name="cityId">The city ID</param>
        /// <param name="patchDocument">JSON Patch document</param>
        /// <returns>No content</returns>
        [HttpPatch("{cityId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PartiallyUpdateCity(
            int cityId,
            [FromBody] Microsoft.AspNetCore.JsonPatch.JsonPatchDocument<CityInfoUpdateDto> patchDocument)
        {
            var city = await _repository.GetCityByIdAsync(cityId);
            if (city == null)
            {
                _logger.LogWarning($"City with ID {cityId} not found for patch");
                return NotFound();
            }

            _logger.LogInformation($"Partially updating city with ID {cityId}");
            
            var cityToPatch = _mapper.Map<CityInfoUpdateDto>(city);
            patchDocument.ApplyTo(cityToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(cityToPatch, city);
            await _repository.UpdateCityAsync(city);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Delete a city
        /// </summary>
        /// <param name="cityId">The city ID</param>
        /// <returns>No content</returns>
        [HttpDelete("{cityId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCity(int cityId)
        {
            var city = await _repository.GetCityByIdAsync(cityId);
            if (city == null)
            {
                _logger.LogWarning($"City with ID {cityId} not found for deletion");
                return NotFound();
            }

            _logger.LogInformation($"Deleting city with ID {cityId}");
            await _repository.DeleteCityAsync(city);
            await _repository.SaveChangesAsync();

            return NoContent();
        }
    }
}
