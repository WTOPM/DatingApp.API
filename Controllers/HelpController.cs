using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace DatingApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class HelpController : ControllerBase
    {
        private readonly DataContext _context;
        public HelpController(DataContext context)
        {
            _context = context;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetHelps()
        {
            var helps = await _context.Helps.ToListAsync();
            
            return Ok(helps);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHelp(int id)
        {
            var helps = await _context.Helps.FirstOrDefaultAsync(x => x.Id == id);

            return Ok(helps);
        }
    }
}
