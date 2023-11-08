using Calculations1.Models;
using Calculations1.Services.CalculationsService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace Calculations1.Controllers 
{
    //[Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("[controller]")]
    public class CalculationsController : ControllerBase
    {

        private readonly ICalculationsService _calculationsService;

        public CalculationsController(ICalculationsService calculationsService)
        {
            _calculationsService = calculationsService;
        }

        
        [HttpGet("GetAll")]
        public ActionResult<ServiceResponse<List<Calculation>>> Get()
        {
            
            return Ok( _calculationsService.GetAllCalculations());

        }

        [HttpGet("GetLast")]
        public ActionResult<ServiceResponse<List<Calculation>>> GetLast()
        {
           
            return Ok( _calculationsService.GetLastSix());

        }

        [HttpGet("GetID/{id}")]
        public ActionResult<ServiceResponse<List<Calculation>>> GetSingle(int id)
        {
            return Ok( _calculationsService.GetCalculationById(id));
        }

        [HttpGet("GetName/{name}")]
        public ActionResult<ServiceResponse<Calculation>> GetSingleName(string name)
        {
            return Ok( _calculationsService.GetCalculationByName(name));
        }

        [HttpGet("GetCalculations/filter/{name?}/{number?}/{category?}/{type?}")]
        public ActionResult<ServiceResponse<Calculation>> GetAllCategory(string name = "*", int number = 1,  Category? category = null, ConstructionType? type = null)
        {
            return Ok( _calculationsService.GetCalculationsFilter(name, number, category, type));
        }


        [HttpDelete("{id}")]
        public ActionResult<ServiceResponse<Calculation>> Delete(int id)
        {
             _calculationsService.DeleteCalculation(id);
            return Ok();
        }

        [HttpPost]
        public ActionResult<ServiceResponse<List<Calculation>>> AddCalculation(CalculationDto newCalculation)
        {
            try
            {
                Calculation calculation = new Calculation();
                if (String.IsNullOrEmpty(newCalculation.Name)) throw new ArgumentException("Project must have a name!");
                calculation.Name = newCalculation.Name;
                if (!String.IsNullOrEmpty(newCalculation.ProjectLeadName)) calculation.ProjectLeadName = newCalculation.ProjectLeadName;
                calculation.ProjectNumber = newCalculation.ProjectNumber;

                calculation.ArchitectDate = newCalculation.ArchitectDate;
                calculation.StartDate = newCalculation.StartDate;

                if (newCalculation.years == 0 && newCalculation.months == 0) throw new ArgumentException("Project must have a duration time!");
                calculation.years = newCalculation.years;
                calculation.months = newCalculation.months;

                if ((int)newCalculation.Category < 1 || (int)newCalculation.Category > 30) throw new ArgumentException("Category must have a value between 1 and 30!");
                if ((int)newCalculation.Type < 1 || (int)newCalculation.Type > 2) throw new ArgumentException("Type must have a value 1 or 2!");
                calculation.Category = newCalculation.Category;
                calculation.Type = newCalculation.Type;
                calculation.Location = newCalculation.Location;
                calculation.UrbanArea = newCalculation.UrbanArea;

                calculation.ApartmentNumber = newCalculation.ApartmentNumber;
                calculation.BathNumber = newCalculation.BathNumber;
                calculation.ToiletNumber = newCalculation.ToiletNumber;

                if (newCalculation.Loa == 0 && newCalculation.Boa == 0) throw new ArgumentException("Boa and loa can't both be zero!");
                calculation.Loa = newCalculation.Loa;
                calculation.Boa = newCalculation.Boa;

                calculation.darkBta = newCalculation.darkBta;
                calculation.lightBta = newCalculation.lightBta;

                if (newCalculation.InternalStandard < 1 || newCalculation.InternalStandard > 5 || 
                    (newCalculation.ExternalStandard == 0 && newCalculation.Type == ConstructionType.NewProduction)) throw new ArgumentException("Standard must be in range 1-5!");
                calculation.ExternalStandard = newCalculation.ExternalStandard;
                calculation.InternalStandard = newCalculation.InternalStandard;
                calculation.DateCalculated = DateTime.Now;

                var response =  _calculationsService.AddCalculation(calculation);

                return Ok(response);
            }
            catch (ArgumentException e)
            {
                //return StatusCode(403, e.Message);
                throw e;
            }
             
        }
    }
}
