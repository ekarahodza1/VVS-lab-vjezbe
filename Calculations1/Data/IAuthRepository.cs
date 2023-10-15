using Calculations1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calculations1.Data
{
    public interface IAuthRepository
    {
        ServiceResponse<int> Register(string username, string password);

        ServiceResponse<string> Login(string username, string password);

       

    }
}
