using Calculations1.Data;
using Calculations1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace Calculations1.Services.CalculationsService
{
    public class CalculationsService : ICalculationsService
    {

        
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static List<Calculation> calculationList = new List<Calculation>();
        

        public CalculationsService(IHttpContextAccessor httpContextAccessor)
        {
           
            _httpContextAccessor = httpContextAccessor;
            
            
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

       public ServiceResponse<List<Calculation>> DeleteCalculation(int id)
        {
            var serviceResponse = new ServiceResponse<List<Calculation>>();
            var dbCalculations = GetList().Where(c => c.User.Id == GetUserId());
            Calculation calculation1 = dbCalculations.FirstOrDefault(c => c.ID == id);
            Delete(calculation1.ID);
            Calculation calculation2 = dbCalculations.FirstOrDefault(c => c.ParentID == id);
            Delete(calculation2.ID);
            
            serviceResponse.Data = GetList();
            return serviceResponse;
        }

        public ServiceResponse<List<Calculation>> GetCalculationsFilter(string name, int number = 1, Category? category = null, ConstructionType? type = null)
        {
            var serviceResponse = new ServiceResponse<List<Calculation>>();
            var dbCalculations = GetList().Where(c => c.User.Id == GetUserId() && c.ParentID == 0).ToList();

            if (!(name.Equals("ššđšđ") || name.Equals("*"))) dbCalculations = dbCalculations.Select(sublist => sublist).Where(c => c.Name.ToLower().Contains(name.ToLower())).ToList();

            if (category != 0) dbCalculations = dbCalculations.Select(sublist => sublist).Where(c => c.Category == (category)).ToList();
            if (type != 0) dbCalculations = dbCalculations.Select(sublist => sublist).Where(c => c.Type == type).ToList();
            
            if (dbCalculations.Count() < (number - 1) * 10) serviceResponse.Data = new List<Calculation>();
            else if (dbCalculations.Count() - number * 10 < 0) serviceResponse.Data = dbCalculations.GetRange((number - 1) * 10, dbCalculations.Count % 10);    
            else serviceResponse.Data = dbCalculations.GetRange((number - 1) * 10, 10);

            return serviceResponse;
        }

        public ServiceResponse<List<Calculation>> GetCalculationById(int id)
        {
            var serviceResponse = new ServiceResponse<List<Calculation>>();
            var dbCalculations = GetList().Where(c => c.User.Id == GetUserId()).ToList();
            serviceResponse.Data = dbCalculations.Select(sublist => sublist).Where(c => c.ID == id || c.ParentID == id).ToList();
            return serviceResponse;
        }

        public ServiceResponse<Calculation> GetCalculationByName(string name)
        {
            var serviceResponse = new ServiceResponse<Calculation>();
            var dbCalculations = GetList().Where(c => c.User.Id == GetUserId() && c.ParentID == 0).ToList();
            serviceResponse.Data = dbCalculations.FirstOrDefault(c => c.Name.ToLower().Contains(name.ToLower()));
            return serviceResponse;
        }


        public ServiceResponse<List<Calculation>> GetAllCalculations()
        {
            var serviceResponse = new ServiceResponse<List<Calculation>>();
            var dbCalculations = GetList();//.Where(c => c.User.Id == GetUserId() && c.ParentID == 0).ToList();
            serviceResponse.Data = dbCalculations;
            return serviceResponse;
        }

        public ServiceResponse<List<Calculation>> GetLastSix()
        {
            var serviceResponse = new ServiceResponse<List<Calculation>>();
            var dbCalculations = GetList().Where(c => c.User.Id == GetUserId() && c.ParentID == 0).ToList();

            dbCalculations = dbCalculations.OrderByDescending(d => d.DateCalculated).ToList();
            serviceResponse.Data = dbCalculations;

            if (dbCalculations.Count > 6) serviceResponse.Data = dbCalculations.GetRange(0, 6);

            return serviceResponse;
        }

        public ServiceResponse<List<Calculation>> AddCalculation(Calculation calculation)
        {
            var serviceResponse = new ServiceResponse<List<Calculation>>();
            List<Calculation> list = new List<Calculation>();

            Calculation calculationParent = CalculateNew(calculation, true);
            //calculationParent.User = GetList()//.FirstOrDefault(u => u.Id == GetUserId());
            Add(calculationParent);
            
            list.Add(calculationParent);

            Calculation calculationChild = CalculateNew(calculation, false);
            calculationChild.User = calculationParent.User;
            calculationChild.ParentID = calculationParent.ID;
            calculation.ID = 0;
            Add(calculationChild);
            
            list.Add(calculationChild);

            var dbCalculations = GetList();//.Where(c => c.User.Id == GetUserId()).ToList();
            dbCalculations = dbCalculations.OrderByDescending(d => d.DateCalculated).ToList();
            serviceResponse.Data = dbCalculations.GetRange(0, 2);
            //serviceResponse.Data = list;
            serviceResponse.Message = "New calculation added";
            return serviceResponse;
        }

        private Calculation CalculateNew (Calculation calculation, bool contract)
        {
            int index = (int)calculation.Category;
            var data = File.ReadLines("formulas.txt").Select(x => x.Split(',')).ToArray();

            //dio za produkciju 
            double _boa = calculation.Boa * Convert.ToDouble(data[index][1]);
            double _loa = calculation.Loa * Convert.ToDouble(data[index][2]);
            int _apartments = calculation.ApartmentNumber * Convert.ToInt32(data[index][3]);
            int _bathNumber = calculation.BathNumber * Convert.ToInt32(data[index][4]);
            int _toiletNumber = calculation.ToiletNumber * Convert.ToInt32(data[index][5]);
            double _darkBTA = calculation.darkBta * Convert.ToDouble(data[index][6]);
            double _lightBTA = calculation.lightBta * Convert.ToDouble(data[index][7]);

            List<double> totalList = new List<double> { _boa, _loa, _apartments, _bathNumber, _toiletNumber, _darkBTA, _lightBTA };

            var constructionCost = totalList.ToList();
            if (contract) constructionCost = constructionCost.Select(b => b * 1.375).ToList();
            else constructionCost = constructionCost.Select(b => b * 1.25).ToList();

            double contractor = constructionCost.Sum() * 0.06;
            double constractorVat;

            if (contract) constractorVat = contractor * 1.35;
            else constractorVat = contractor * 1.25;

            //dio za klijenta
            double totalBoaLoa = calculation.Boa + calculation.Loa;

            double landPreparation = totalBoaLoa * Convert.ToDouble(data[index][8]);
            double streetExploration = totalBoaLoa * Convert.ToDouble(data[index][9]);
            double clientCost = totalBoaLoa * Convert.ToDouble(data[index][10]);
            double connectionFees = totalBoaLoa * Convert.ToDouble(data[index][11]);
            double interest = totalBoaLoa * Convert.ToDouble(data[index][12]);
            double contractorClient = contractor;
            if (calculation.Type == ConstructionType.NewProduction) contractorClient *= 0.7;

            landPreparation *= 1.25;
            contractorClient *= 1.25;
            
            interest = interest + interest * 0.2;

            List<double> clientList = new List<double> { landPreparation, streetExploration, clientCost, connectionFees, contractorClient, interest };

            //racunanje lokacije
            if (new[] { "Sundsvall", "Malmo", "Gothenburg", "Norrkoping" }.Contains(calculation.Location))
            {
                constructionCost = constructionCost.Select(b => b * 0.95).ToList();
                constractorVat *= 0.95;
                clientList = clientList.Select(b => b * 0.95).ToList();
            }
            else if (calculation.Location.Equals("Orter"))
            {
                constructionCost = constructionCost.Select(b => b * 0.9).ToList();
                constractorVat *= 0.9;
                clientList = clientList.Select(b => b * 0.9).ToList();
            }

            if (calculation.UrbanArea)
            {
                constructionCost = constructionCost.Select(b => b * 1.065).ToList();
                constractorVat *= 1.065;
                clientList = clientList.Select(b => b * 1.065).ToList();
            }

            //racunanje standarda
            List<double> standardsList = new List<double> { 0.95, 0.975, 1, 1.025, 1.05 };
            if ((calculation.Type == ConstructionType.Rebuildings)) standardsList = new List<double> { 0.9, 0.95, 1, 1.05, 1.1 };

            if (calculation.Type == ConstructionType.NewProduction) constructionCost = constructionCost.Select(b => b * standardsList[calculation.ExternalStandard - 1]).ToList();
            constructionCost = constructionCost.Select(b => b * standardsList[calculation.InternalStandard - 1]).ToList();

            if (calculation.Type == ConstructionType.NewProduction) constractorVat *= standardsList[calculation.ExternalStandard - 1];
            constractorVat *= standardsList[calculation.InternalStandard - 1];

            if (calculation.Type == ConstructionType.NewProduction) clientList = clientList.Select(b => b * standardsList[calculation.ExternalStandard - 1]).ToList();
            clientList = clientList.Select(b => b * standardsList[calculation.InternalStandard - 1]).ToList();

            var year_num = calculation.years;

            for (int i = 0; i < year_num; i++)
            {
                if (i == 0 && calculation.StartDate.Year == 2022)
                {
                    constructionCost = constructionCost.Select(b => b * 1.03).ToList();
                    constractorVat *= 1.03;
                    clientList = clientList.Select(b => b * 1.03).ToList();
                }

                else if (i == year_num - 1 && calculation.months < 5) continue;

                else
                {
                    constructionCost = constructionCost.Select(b => b * 1.02).ToList();
                    constractorVat *= 1.02;
                    clientList = clientList.Select(b => b * 1.02).ToList();
                }
            }

            //prve sume - production, client i total
            calculation.ProductionInclVat = Math.Round(constructionCost.Sum() + constractorVat);
            calculation.ProductionExclVat = Math.Round(calculation.ProductionInclVat * 0.8);

            calculation.ClientInclVat = Math.Round(clientList.Sum());
            calculation.ClientExclVat = Math.Round(clientList[0] * 0.8 + clientList[1] + clientList[2] * 0.9 + clientList[3] + clientList[4] * 0.8 + clientList[5] * 0.835);

            calculation.TotalInclVat = calculation.ProductionInclVat + calculation.ClientInclVat;
            calculation.TotalExclVat = calculation.ProductionExclVat + calculation.ClientExclVat;

            calculation.ContractExclVat = Math.Round(calculation.ProductionExclVat * 0.092);
            calculation.ContractInclVat = Math.Round(calculation.ContractExclVat * 1.25);

            //contract total
            constructionCost = constructionCost.Select(b => b * 0.8).ToList();

            calculation.boaContract = Math.Round(constructionCost[0] * 0.092);
            calculation.loaContract = Math.Round(constructionCost[1] * 0.092);
            calculation.ApartmentNumberContract = Math.Round(constructionCost[2] * 0.092);
            calculation.BathNumberContract = Math.Round(constructionCost[3] * 0.092);
            calculation.ToiletNumberContract = Math.Round(constructionCost[4] * 0.092);
            calculation.darkBtaContract = Math.Round(constructionCost[5] * 0.092);
            calculation.lightBtaContract = Math.Round(constructionCost[6] * 0.092);
            calculation.Contractor = Math.Round((constractorVat * 0.8) * 0.092);

            //po kvadratnom metru
            calculation.ProductionBoaLoaExclVat = Math.Round(calculation.ProductionExclVat / (calculation.Boa + calculation.Loa));
            calculation.ProductionBoaLoaInclVat = Math.Round(calculation.ProductionInclVat / (calculation.Boa + calculation.Loa));

            calculation.ProductionLightBtaExclVat = Math.Round(calculation.ProductionExclVat / calculation.lightBta);
            calculation.ProductionLightBtaInclVat = Math.Round(calculation.ProductionInclVat / calculation.lightBta);

            calculation.ProductionTotalBtaExclVat = Math.Round(calculation.ProductionExclVat / (calculation.lightBta + calculation.darkBta));
            calculation.ProductionTotalBtaInclVat = Math.Round(calculation.ProductionInclVat / (calculation.lightBta + calculation.darkBta));

            //
            calculation.ClientBoaLoaExclVat = Math.Round(calculation.ClientExclVat / (calculation.Boa + calculation.Loa));
            calculation.ClientBoaLoaInclVat = Math.Round(calculation.ClientInclVat / (calculation.Boa + calculation.Loa));

            calculation.ClientLightBtaExclVat = Math.Round(calculation.ClientExclVat / calculation.lightBta);
            calculation.ClientLightBtaInclVat = Math.Round(calculation.ClientInclVat / calculation.lightBta);

            calculation.ClientTotalBtaExclVat = Math.Round(calculation.ClientExclVat / (calculation.lightBta + calculation.darkBta));
            calculation.ClientTotalBtaInclVat = Math.Round(calculation.ClientInclVat / (calculation.lightBta + calculation.darkBta));

            //
            calculation.TotalBoaLoaExclVat = calculation.ProductionBoaLoaExclVat + calculation.ClientBoaLoaExclVat;
            calculation.TotalBoaLoaInclVat = calculation.ProductionBoaLoaInclVat + calculation.ClientBoaLoaInclVat;

            calculation.TotalLightBtaExclVat = calculation.ProductionLightBtaExclVat + calculation.ClientLightBtaExclVat;
            calculation.TotalLightBtaInclVat = calculation.ProductionLightBtaInclVat + calculation.ClientLightBtaInclVat;

            calculation.TotalTotalBtaExclVat = calculation.ProductionTotalBtaExclVat + calculation.ClientTotalBtaExclVat;
            calculation.TotalTotalBtaInclVat = calculation.ProductionTotalBtaInclVat + calculation.ClientTotalBtaInclVat;

            //po unit-u
            List<int> displayInfo = new List<int> { 1, 2, 3, 4, 5, 6, 17, 18, 19, 20 };

            if (displayInfo.Contains(index))
            {

                calculation.ProductionPerIghExclVat = Math.Round(calculation.ProductionExclVat / calculation.ApartmentNumber);
                calculation.ProductionPerIghInclVat = Math.Round(calculation.ProductionInclVat / calculation.ApartmentNumber);
                calculation.ClientPerIghExclVat = Math.Round(calculation.ClientExclVat / calculation.ApartmentNumber);
                calculation.ClientPerIghInclVat = Math.Round(calculation.ClientInclVat / calculation.ApartmentNumber);
                calculation.TotalPerIghExclVat = Math.Round((calculation.ProductionExclVat + calculation.ClientExclVat) / calculation.ApartmentNumber);
                calculation.TotalPerIghInclVat = Math.Round((calculation.ProductionInclVat + calculation.ClientInclVat) / calculation.ApartmentNumber);
            }

            return calculation;
        }

        private void Delete(int id)
        {
            calculationList.RemoveAll(c => c.ID == id);

        }

        private void Add(Calculation calculation)
        {
            calculationList.Add(calculation);
        }

        private Calculation Get(int id)
        {
            return calculationList[id];
        }

        private void Update(int id, Calculation calculation)
        {
            calculationList[id] = calculation;
        }


        public List<Calculation> GetList()
        {
            return calculationList;
        }

    }
}
