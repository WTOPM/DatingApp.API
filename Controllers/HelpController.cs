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
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class HelpController : ControllerBase
    {
            private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public HelpController(IDatingRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }
                public async Task<IActionResult> GetHelp(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var helpFromRepo = await _repo.GetHelp(id);

            if (helpFromRepo == null)
                return NotFound();

            return Ok(helpFromRepo);
        }
        [HttpGet]
        public async Task<IActionResult> GetHelpsForUser(int userId, 
            [FromQuery]HelpParams helpParams)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            helpParams.UserId = userId;

            var helpsFromRepo = await _repo.GetHelpsForUser(helpParams);

            var helps = _mapper.Map<IEnumerable<MessageToReturnDto>>(helpsFromRepo);

            Response.AddPagination(helpsFromRepo.CurrentPage, helpsFromRepo.PageSize, 
                helpsFromRepo.TotalCount, helpsFromRepo.TotalPages);
            
            return Ok(helps);
        }
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetHelpProduct(int userId, int productId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var helpsFromRepo = await _repo.GetHelpProduct(userId, productId);

            var helpsProduct = _mapper.Map<IEnumerable<HelpToReturnDto>>(helpsFromRepo);

            return Ok(helpsProduct);
        }
        [HttpPost]
        public async Task<IActionResult> CreateHelp(int userId, HelpForCreationDto helpForCreationDto)
        {
            var sender = await _repo.GetHelp(userId);

            if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            helpForCreationDto.ProductId = userId;

            var recipient = await _repo.GetHelp(helpForCreationDto.RecipientId);

            if (recipient == null)
                return BadRequest("Could not find user");
            
            var help = _mapper.Map<Message>(helpForCreationDto);

            _repo.Add(help);

            if (await _repo.SaveAll())
            {
                var helpToReturn = _mapper.Map<MessageToReturnDto>(help);
                return CreatedAtRoute(nameof(GetHelp), new {userId, id = help.Id}, helpToReturn);
            }

            throw new Exception("Creating the message failed on save");
        }

    }
}
