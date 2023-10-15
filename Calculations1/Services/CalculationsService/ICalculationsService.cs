using Calculations1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#nullable enable

namespace Calculations1.Services.CalculationsService
{
    public interface ICalculationsService
    {
        ServiceResponse<List<Calculation>> GetAllCalculations();
        ServiceResponse<List<Calculation>> GetLastSix();
        ServiceResponse<List<Calculation>> GetCalculationById(int id);
        ServiceResponse<Calculation> GetCalculationByName(string name);
        ServiceResponse<List<Calculation>> GetCalculationsFilter(string? name, int Number = 1, Category? category = null, ConstructionType? type = null);
        ServiceResponse<List<Calculation>> DeleteCalculation(int id);
        ServiceResponse<List<Calculation>> AddCalculation(Calculation calculation);
    }
}
