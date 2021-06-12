﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectApi.Data;
using ProjectApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotesController1 : ControllerBase
    {
        private QuotesDbContext _quotesDbContext;

        public QuotesController1(QuotesDbContext quotesDbContext)
        {
            _quotesDbContext = quotesDbContext;
        }

        // GET: api/<QuotesController1>
        //[HttpGet]
        //public IEnumerable<Quote> Get()
        //{
        //    return _quotesDbContext.Quotes;
        //}

        // GET: api/<QuotesController1>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_quotesDbContext.Quotes);
        }

        // GET api/<QuotesController1>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var quote = _quotesDbContext.Quotes.Find(id);
            if(quote == null)
            {
                return NotFound("Kayıt bulunamadı!..");
            }
            return Ok(quote);
        }

        // POST api/<QuotesController1>
        [HttpPost]
        public IActionResult Post([FromBody] Quote quote)
        {
            _quotesDbContext.Quotes.Add(quote);
            _quotesDbContext.SaveChanges();
            // Http status code
            return StatusCode(StatusCodes.Status201Created);
        }

        // PUT api/<QuotesController1>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Quote quote)
        {
            var entity = _quotesDbContext.Quotes.Find(id);
            // if id couldnt found
            if(entity == null)
            {
                return NotFound("Bu id ile kayıt bulunamadı!..");
            }
            else
            {
                entity.Title = quote.Title;
                entity.Author = quote.Author;
                entity.Description = quote.Description;
                entity.Type = quote.Type;
                _quotesDbContext.SaveChanges();
                return Ok("Kayıt güncelleme başarılı..");
            }
            
        }

        // DELETE api/<QuotesController1>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var quote = _quotesDbContext.Quotes.Find(id);
            if(quote == null)
            {
                return NotFound("Bu id ile kayıt bulunamadı!..");
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
