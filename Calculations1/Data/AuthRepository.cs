using Calculations1.Models;
using Jose;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Calculations1.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IConfiguration _configuration;
        private static List<User> userList = new List<User>();

        public AuthRepository(IConfiguration configuration)
        {
            
            _configuration = configuration;
            
            
        }

        public ServiceResponse<string> Login(string username, string password)
        {

            var response = new ServiceResponse<string>();
            
            var user = GetUserList().FirstOrDefault(x => x.Username.ToLower().Equals(username.ToLower()));
            

            if (user == null)
            {
                response.Succes = false;
                response.Message = "User not found";
            }
            else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                response.Succes = false;
                response.Message = "Wrong password";
            }
            else
            {
                response.Data = CreateToken(user);
                response.Message = "Successful login";

            }
            return response;
        }

        public ServiceResponse<int> Register(string username, string password)
        {
            User user = new User();
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            var random = new Random();
            user.Id = random.Next(1, 1000000);
            user.Username = username;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            AddUser(user);
            ServiceResponse<int> response = new ServiceResponse<int>();

            response.Data = user.Id;
            response.Message = "New user added";
            return response;

        }




       

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false;
                }

                return true;
            }
        }

        private string CreateToken (User user)
        {
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            
            var tokenDecsriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = System.DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };
            //return " ";

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDecsriptor);

            return tokenHandler.WriteToken(token);
        }

        private List<User> GetUserList()
        {
            return userList;
        }

        private void AddUser(User user)
        {
            userList.Add(user);
        }


    }
}
