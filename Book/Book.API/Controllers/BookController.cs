using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class BookController : Controller
    {
        [HttpGet]
        public IEnumerable<Models.Book> Get()
        {
            return new Models.Book[]
            {
                new Models.Book()
                {
                    Id = 1,
                    Name = "Clean Code",
                    Author = "Uncle Bob",
                    Year = 2008
                },
                new Models.Book()
                {
                    Id = 2,
                    Name = "Clean Architecture",
                    Author = "Uncle Bob",
                    Year = 2017
                },
            };
        }
    }
}
