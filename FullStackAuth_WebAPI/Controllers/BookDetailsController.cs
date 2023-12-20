using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FullStackAuth_WebAPI.Data;
using FullStackAuth_WebAPI.DataTransferObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FullStackAuth_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookDetailsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BookDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET api/BookDetails/5
        [HttpGet("{bookId}")]
        public IActionResult Get(string bookId)
        {
            try
            {
                // Retrieve the reviews with the specified ID, including the owner object
                var currentUserId = User.Identity.Name;
                var isFavorited = _context.Favorites.Any(f => f.UserId == currentUserId && f.BookId == bookId);
                var reviews = _context.Reviews.Include(r => r.User).Where(r => r.BookId == bookId);

                double average = reviews.Select(r => r.Rating).Average();

                // If the review does not exist, return a 404 not found response
                if (reviews == null)
                {
                    return NotFound();
                }

                

                var bookDetailsDto = new BookDetailsDto
                {
                    BookId = bookId,
                    Average = average,
                    IsFavorited = isFavorited,
                    Reviews = reviews.Select(r => new ReviewWithUserDto
                    {
                        Id = r.Id,
                        Text = r.Text,
                        Rating = r.Rating,
                        User = new UserForDisplayDto
                        {
                            Id = r.User.Id,
                            FirstName = r.User.FirstName,
                            LastName = r.User.LastName,
                            UserName = r.User.UserName,

                        }

                    }).ToList()
                };
               

                //Return the review as a 200 OK response
                return StatusCode(200, bookDetailsDto);
            }
            catch (Exception ex)
            {
                // If an error occurs, return a 500 internal server error with the error message
                return StatusCode(500, ex.Message);
            }
        }
    }
}
