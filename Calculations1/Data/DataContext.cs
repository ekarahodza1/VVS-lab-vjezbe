using Calculations1.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calculations1.Data
{
    public class DataContext 
    {
        private readonly DataContext _dataContext;
        private List<Calculation> calculationList;
        private List<User> userList;
        public DataContext(DataContext dataContext) 
        {
            _dataContext = dataContext;
            calculationList = new List<Calculation>();
            userList = new List<User>();


        }

        

        public void DeleteCalculation(int id)
        {
            calculationList.RemoveAll(c => c.ID == id);

        }

        public void AddCalculation (Calculation calculation)
        {
            calculationList.Add(calculation);
        }

        public Calculation GetCalculation(int id)
        {
            return calculationList[id];
        }

        public void UpdateCalculation(int id, Calculation calculation)
        {
            calculationList[id] = calculation;
        }


        public List<Calculation> GetCalculationList() 
        {  
            return calculationList; 
        }

        public List<User> GetUserList()
        {
            return userList;
        }

        public void AddUser(User user)
        {
            userList.Add(user);
        }

    }
}
