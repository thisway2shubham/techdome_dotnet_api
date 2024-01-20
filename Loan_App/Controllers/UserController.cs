using Loan_App.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Loan_App.Context;

namespace Loan_App.Controllers
{
       
        [Route("api/[controller]")]
        [ApiController]
        public class UserController : ControllerBase
        {
            private readonly LoanContext _context;

            public UserController(LoanContext context)
            {
                _context = context;
            }

            [HttpPost("register")]
            public async Task<IActionResult> Register([FromBody] user user)
            {
                try
                {
                    if (string.IsNullOrEmpty(user.Name) || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
                    {
                        return BadRequest(new { success = false, message = "Please enter name, email, and password!" });
                    }

                    var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email && u.IsAdmin == user.IsAdmin);
                    if (existingUser != null)
                    {
                        return BadRequest(new { success = false, message = "User already exists!" });
                    }

                    using var hmac = new HMACSHA512();
                    user.Password = Encoding.UTF8.GetString(hmac.ComputeHash(Encoding.UTF8.GetBytes(user.Password)));

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    return StatusCode(201, new { success = true, message = "Signup successful" });
                }
                catch (Exception)
                {
                    return StatusCode(500, new { success = false, message = "Internal server Error" });
                }
            }

            [HttpPost("login")]
            public async Task<IActionResult> Login([FromBody] user user)
            {
                try
                {
                    if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
                    {
                        return BadRequest(new { success = false, message = "Please enter email and password!" });
                    }

                    var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email && u.IsAdmin == user.IsAdmin);
                    if (existingUser == null)
                    {
                        return BadRequest(new { success = false, message = "Invalid email or password!" });
                    }

                    using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(existingUser.Password));
                    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(user.Password));

                    for (int i = 0; i < computedHash.Length; i++)
                    {
                        if (computedHash[i] != existingUser.Password[i])
                        {
                            return Unauthorized(new { success = false, message = "Invalid email or password!" });
                        }
                    }

                    var claims = new[]
                    {
                    new Claim(ClaimTypes.NameIdentifier, existingUser.Id.ToString()),
                    new Claim(ClaimTypes.Name, existingUser.Name),
                    new Claim(ClaimTypes.Email, existingUser.Email),
                    new Claim(ClaimTypes.Role, existingUser.IsAdmin ? "Admin" : "User")
                };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("yourSecretKey"));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(claims),
                        Expires = DateTime.Now.AddDays(1),
                        SigningCredentials = creds
                    };

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var token = tokenHandler.CreateToken(tokenDescriptor);

                    return Ok(new
                    {
                        success = true,
                        message = "User Successfully logged in!",
                        token = tokenHandler.WriteToken(token),
                        user = existingUser
                    });
                }
                catch (Exception)
                {
                    return StatusCode(500, new { success = false, message = "Internal server error!" });
                }
            }
        }
    }


