using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FullStackAuth_WebAPI.Data;
using FullStackAuth_WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FullStackAuth_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FavoritesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/favorites/myFavorites
        [HttpGet("myFavorites"), Authorize]
        public IActionResult GetUsersFavorites()
        {
            try
            {
                // Retrieve the authenticated user's ID from the JWT token
                string userId = User.FindFirstValue("id");

                // Retrieve all cars that belong to the authenticated user, including the owner object
                var favorites = _context.Favorites.Where(c => c.UserId.Equals(userId));

                // Return the list of cars as a 200 OK response
                return StatusCode(200, favorites);
            }
            catch (Exception ex)
            {
                // If an error occurs, return a 500 internal server error with the error message
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/Favorites
        [HttpPost, Authorize]
        public IActionResult Post([FromBody] Favorite data)
        {
            try
            {
                // Retrieve the authenticated user's ID from the JWT token
                string userId = User.FindFirstValue("id");

                // If the user ID is null or empty, the user is not authenticated, so return a 401 unauthorized response
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                // Set the Reviews's owner ID  the authenticated user's ID we found earlier
                data.UserId = userId;

                // Add the reviews to the database and save changes
                _context.Favorites.Add(data);
                if (!ModelState.IsValid)
                {
                    // If the review model state is invalid, return a 400 bad request response with the model state errors
                    return BadRequest(ModelState);
                }
                _context.SaveChanges();

                // Return the newly created review as a 201 created response
                return StatusCode(201, data);
            }
            catch (Exception ex)
            {
                // If an error occurs, return a 500 internal server error with the error message
                return StatusCode(500, ex.Message);
            }
        }
    }
}
