using Calculations1.Data;
using Calculations1.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calculations1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController: ControllerBase
    {
        private readonly IAuthRepository _authRepository;

        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpPost("Login")]
        public ActionResult<ServiceResponse<string>> Login(UserLogin request)
        {
            var response =  _authRepository.Login(request.Username, request.Password);

            if (!response.Succes)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("Register")]
        public ActionResult<ServiceResponse<string>> Register(UserLogin request)
        {
            var response =  _authRepository.Register(request.Username, request.Password);

            if (!response.Succes)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }


        
    }
}
