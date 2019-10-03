using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;
        }

        // We can use UserForRegisterDto here because [ApiController]/ASP.net Core infers
        // the json object given back and retrieves the data and puts it into the properties
        // in userForRegisterDto. If we don't use ApiController, we have to use [FromBody]
        // behind the parameter to manually tell the DTO where to infer the data from.
        // We also have to do the validation manually
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            // Do validation manually
            /* 
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            */

            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if (await _repo.UserExists(userForRegisterDto.Username))
                return BadRequest("Username already exists");

            User userToCreate = new User
            {
                Username = userForRegisterDto.Username
            };

            User createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            // placeholder: status of createdAtRoute. Use this for now until we can
            // get user and the route to which the client can fetch it
            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            // check if the user exists in the database and if the password matches
            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            if (userFromRepo == null)
                return Unauthorized();

            //JSON Web Token (JWT) claims are pieces of information asserted about a subject. 
            //For example, an ID Token (which is always a JWT) may contain a claim called name 
            //that asserts that the name of the user authenticating is "John Doe"
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

            // To make sure the token is a valid token when it comes back,
            // the server needs to sign the token.
            // Use the key as the signing credentials. Hash it.
            // Use the token from AppSettings which is a .json class in the project.
            // The token usually should be long and random, but in this case, just for testing purposes,
            // the token will be just long enough. There is a length requirement (12 characters).
            // Also, the reason appSettings is used here because it is not very big and it is easier
            // and faster to retrieve from appsettings which already exists on the server so no call
            // to the database is needed.
            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            // hash the signing credentials
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // describe the token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            // Use this to actually make the token
            var tokenHandler = new JwtSecurityTokenHandler();
            
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // return Ok with the token as an objects
            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }
    }
}