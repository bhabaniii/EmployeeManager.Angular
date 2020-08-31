using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using EmployeeManager.Angular.Models;
using EmployeeManager.Angular.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeManager.Angular.Controllers
{
    [Route("api/[controller]")]
    //[EnableCors("CorsPolicy")]
   
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly AppDbContext _appDbContext = null;
        private readonly IConfiguration _config = null;

        public SecurityController(IConfiguration config, AppDbContext appDbContext)
        {
           
            this._config = config;
            this._appDbContext = appDbContext;

        }
        [AllowAnonymous]
        [HttpPost]
        [Route("[action]")]
        public IActionResult SignIn([FromBody] SignIn loginDetails)
        {

            var qry = _appDbContext.users.Where(u => u.UserName == loginDetails.UserName &&
                        u.Password == loginDetails.Password);

            if (qry.Count() > 0)
            {

                var tokenString = GenerateJwt(loginDetails.UserName);
                return Ok(new { token = tokenString });
            }

            else
            {

                return Unauthorized();
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("[action]")]
        public IActionResult Register([FromBody] Register userDetails)
        {
            var qry = _appDbContext.users.Where(u => u.UserName == userDetails.UserName);

            if (qry.Count() <= 0)
            {
                var user = new User();
                user.UserName = userDetails.UserName;
                user.Password = userDetails.Password;
                user.Email = userDetails.Email;
                user.FullName = userDetails.FullName;
                user.BirthDate = userDetails.BirthDate;
                user.Role = "Manager";

                _appDbContext.users.Add(user);
                _appDbContext.SaveChanges();

                return Ok("User Created Successfully !!");

            }

            else
            {
                return BadRequest("User Already exist !");

            }




        }


        private string GenerateJwt(string userName)
        {

        //    var usr = (from u in _appDbContext.users
        //               where u.UserName == userName
        //               select u).SingleOrDefault();


            var usr = _appDbContext.users.Where(u => u.UserName == userName).SingleOrDefault();

            var claim = new List<Claim>();
            claim.Add(new Claim(ClaimTypes.Name, usr.UserName));
            claim.Add(new Claim(ClaimTypes.Role, usr.Role));

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

          
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                expires: DateTime.Now.AddHours(12),
                signingCredentials: credentials,
                claims: claim);

            var tokeHandler = new JwtSecurityTokenHandler();
            var stringToken = tokeHandler.WriteToken(token);

            return stringToken;

        }

    }
}
