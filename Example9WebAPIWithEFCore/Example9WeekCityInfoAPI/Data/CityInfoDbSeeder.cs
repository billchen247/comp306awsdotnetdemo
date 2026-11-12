using Example9WeekCityInfoLibrary.Data;
using Example9WeekCityInfoLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace Example9WeekCityInfoAPI.Data
{
    /// <summary>
    /// Seeder class for initializing the database with sample data
    /// </summary>
    public class CityInfoDbSeeder
    {
        private readonly CityInfoDbContext _context;
        private readonly ILogger<CityInfoDbSeeder> _logger;

        public CityInfoDbSeeder(CityInfoDbContext context, ILogger<CityInfoDbSeeder> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Seeds the database with initial data if it doesn't already exist
        /// </summary>
        public async Task SeedAsync()
        {
            try
            {
                // Ensure database is created
                await _context.Database.EnsureCreatedAsync();

                // Check if data already exists
                if (await _context.Cities.AnyAsync())
                {
                    _logger.LogInformation("Database already contains data. Skipping seed.");
                    return;
                }

                _logger.LogInformation("Starting database seeding...");

                var cities = new List<CityInfo>
                {
                    new CityInfo
                    {
                        CityName = "New York",
                        Description = "The Big Apple - A vibrant metropolis known for its iconic skyline, world-class museums, and Broadway shows.",
                        PointsOfInterest = new List<PointsOfInterest>
                        {
                            new PointsOfInterest
                            {
                                NameofPOI = "Statue of Liberty",
                                Description = "A colossal neoclassical sculpture located on Liberty Island in New York Harbor. It's a universal symbol of freedom and democracy."
                            },
                            new PointsOfInterest
                            {
                                NameofPOI = "Central Park",
                                Description = "An urban park in Manhattan spanning 843 acres. It's a peaceful oasis in the heart of the city with lakes, gardens, and recreational facilities."
                            },
                            new PointsOfInterest
                            {
                                NameofPOI = "Empire State Building",
                                Description = "An iconic Art Deco skyscraper completed in 1931. The building offers panoramic views of the city from its observation deck."
                            },
                            new PointsOfInterest
                            {
                                NameofPOI = "Times Square",
                                Description = "A major commercial intersection and popular tourist attraction known for its bright LED billboards and New Year's Eve celebration."
                            },
                            new PointsOfInterest
                            {
                                NameofPOI = "Brooklyn Bridge",
                                Description = "A hybrid cable-stayed bridge completed in 1883, spanning the East River and connecting Manhattan and Brooklyn."
                            }
                        }
                    },
                    new CityInfo
                    {
                        CityName = "Los Angeles",
                        Description = "The City of Angels - Known for its entertainment industry, beautiful beaches, and diverse culture.",
                        PointsOfInterest = new List<PointsOfInterest>
                        {
                            new PointsOfInterest
                            {
                                NameofPOI = "Hollywood Walk of Fame",
                                Description = "A famous Hollywood landmark that honors achievements in entertainment on five categories: motion pictures, television, radio, recording, and theatre/performance."
                            },
                            new PointsOfInterest
                            {
                                NameofPOI = "Griffith Observatory",
                                Description = "An arts and science facility featuring exhibits and a planetarium. It offers views of the city skyline and is a popular spot for stargazing."
                            },
                            new PointsOfInterest
                            {
                                NameofPOI = "Venice Beach",
                                Description = "A popular beach in the coastal district of Venice, known for its broad sandy beaches, oceanside promenade, and bohemian culture."
                            },
                            new PointsOfInterest
                            {
                                NameofPOI = "Getty Center",
                                Description = "A world-renowned art museum and cultural center featuring European paintings, sculptures, and photographs."
                            }
                        }
                    },
                    new CityInfo
                    {
                        CityName = "San Francisco",
                        Description = "City by the Bay - Famous for its steep rolling hills, iconic suspension bridge, and progressive culture.",
                        PointsOfInterest = new List<PointsOfInterest>
                        {
                            new PointsOfInterest
                            {
                                NameofPOI = "Golden Gate Bridge",
                                Description = "An iconic suspension bridge spanning the Golden Gate strait. The bridge is famous for its distinctive international orange color and Art Deco design."
                            },
                            new PointsOfInterest
                            {
                                NameofPOI = "Alcatraz Island",
                                Description = "A former federal penitentiary turned historical landmark and museum. It's accessible by ferry and offers guided tours of its notorious history."
                            },
                            new PointsOfInterest
                            {
                                NameofPOI = "Cable Cars",
                                Description = "Historic streetcars that climb San Francisco's steep hills. They're an iconic symbol of the city and a fun tourist attraction."
                            },
                            new PointsOfInterest
                            {
                                NameofPOI = "Fisherman's Wharf",
                                Description = "A working waterfront area at the northern end of the San Francisco Peninsula, famous for seafood restaurants and sea lion colonies."
                            }
                        }
                    },
                    new CityInfo
                    {
                        CityName = "Chicago",
                        Description = "The Windy City - A major cultural and architectural hub with world-class museums and soaring skyscrapers.",
                        PointsOfInterest = new List<PointsOfInterest>
                        {
                            new PointsOfInterest
                            {
                                NameofPOI = "Willis Tower",
                                Description = "A 110-story skyscraper that was the world's tallest building from 1973 to 1998. It features a glass Skydeck on the 103rd floor."
                            },
                            new PointsOfInterest
                            {
                                NameofPOI = "Millennium Park",
                                Description = "A 24.5-acre public park featuring iconic public art, including the famous Cloud Gate (The Bean) sculpture by Anish Kapoor."
                            },
                            new PointsOfInterest
                            {
                                NameofPOI = "Art Institute of Chicago",
                                Description = "One of the oldest and largest art museums in the United States, featuring an impressive collection of artwork from around the world."
                            },
                            new PointsOfInterest
                            {
                                NameofPOI = "Navy Pier",
                                Description = "A 3,300-foot pier that projects into Lake Michigan, featuring restaurants, shops, attractions, and beautiful views of the city skyline."
                            }
                        }
                    },
                    new CityInfo
                    {
                        CityName = "Boston",
                        Description = "The Hub - A historic city with significant Revolutionary War and American history sites.",
                        PointsOfInterest = new List<PointsOfInterest>
                        {
                            new PointsOfInterest
                            {
                                NameofPOI = "Freedom Trail",
                                Description = "A 2.5-mile brick walking path that connects historic sites related to American independence and the American Revolution."
                            },
                            new PointsOfInterest
                            {
                                NameofPOI = "Boston Common",
                                Description = "A public park and the oldest city park in the United States, established in 1634. It's a popular gathering place and recreational area."
                            },
                            new PointsOfInterest
                            {
                                NameofPOI = "Museum of Fine Arts",
                                Description = "A major art museum housing Egyptian mummies, Asian art, and American paintings among its extensive collection."
                            },
                            new PointsOfInterest
                            {
                                NameofPOI = "Faneuil Hall and Quincy Market",
                                Description = "Historic public market and meeting hall, now a shopping and dining destination, with street performers and vendor stalls."
                            }
                        }
                    }
                };

                await _context.Cities.AddRangeAsync(cities);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Successfully seeded database with {cities.Count} cities and their points of interest.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }
    }
}
