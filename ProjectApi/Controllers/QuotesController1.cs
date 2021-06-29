using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectApi.Data;
using ProjectApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuotesController1 : ControllerBase
    {
        private QuotesDbContext _quotesDbContext;

        public QuotesController1(QuotesDbContext quotesDbContext)
        {
            _quotesDbContext = quotesDbContext;
        }

        
        // GET: api/<QuotesController1>
        [HttpGet]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]  // caching for 60 second in clientside
        public IActionResult Get(string sort)
        {
            // more efficient for sorting according to IEnumerable<>
            IQueryable<Quote> quotes;
            switch(sort)
            {
                case "desc":
                    quotes = _quotesDbContext.Quotes.OrderByDescending(q => q.CreateAt);
                    break;
                case "asc":
                    quotes = _quotesDbContext.Quotes.OrderBy(q => q.CreateAt);
                    break;
                default:
                    quotes = _quotesDbContext.Quotes;
                    break;
            }
            return Ok(quotes);
        }

        // pagination with url
        [HttpGet("[action]")]
        // ? mean is nullable
        public IActionResult PagingQuote(int? pageNumber, int? pageSize)
        {
            var quotes = _quotesDbContext.Quotes;

            //default pagination values
            var currentPageNumber = pageNumber ?? 1;
            var currentPageSize = pageSize ?? 5;

            return Ok(quotes.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize));
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult SearchQuote(string type)
        {
            var quotes = _quotesDbContext.Quotes.Where(q => q.Type.StartsWith(type));
            return Ok(quotes);
        }

        [HttpGet("[action]")]
        public IActionResult MyQuote()
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var quotes = _quotesDbContext.Quotes.Where(q=>q.UserId==userId);
            return Ok(quotes);
        }

        // GET api/<QuotesController1>/5
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            var quote = _quotesDbContext.Quotes.Find(id);
            if(quote == null)
            {
                return NotFound("Kayıt bulunamadı!..");
            }
            return Ok(quote);
        }

        //// api/<QuotesController1/Test/1
        //[HttpGet("[action]/{id}")]
        //public int Test(int id)
        //{
        //    return id;
        //}

        // POST api/<QuotesController1>
        [HttpPost]
        public IActionResult Post([FromBody] Quote quote)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            quote.UserId = userId;
            _quotesDbContext.Quotes.Add(quote);
            _quotesDbContext.SaveChanges();
            // Http status code
            return StatusCode(StatusCodes.Status201Created);
        }

        // PUT api/<QuotesController1>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Quote quote)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            
            var entity = _quotesDbContext.Quotes.Find(id);
            // if id couldnt found
            if(entity == null)
            {
                return NotFound("Bu id ile kayıt bulunamadı!..");
            }
            
            if(userId != entity.UserId)
            {
                return BadRequest("Kayıt güncelleyemedin moruk!..");
            }
            
            else
            {
                entity.Title = quote.Title;
                entity.Author = quote.Author;
                entity.Description = quote.Description;
                entity.Type = quote.Type;
                entity.CreateAt = quote.CreateAt;
                _quotesDbContext.SaveChanges();
                return Ok("Kayıt güncelleme başarılı..");
            }
            
        }

        // DELETE api/<QuotesController1>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var quote = _quotesDbContext.Quotes.Find(id);
            if(quote == null)
            {
                return NotFound("Bu id ile kayıt bulunamadı!..");
            }

            if(userId != quote.UserId)
            {
                return BadRequest("Kayıt silemezsin hafz!..");
            }

            else
            {
                _quotesDbContext.Quotes.Remove(quote);
                _quotesDbContext.SaveChanges();
                return Ok("Kayıt silindi!..");
            }
            
        }
    }
}
