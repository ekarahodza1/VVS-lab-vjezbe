using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Calculations1.Models
{
    public class Calculation
    {
        [Key]
        public int ID { get; set; }
        public int ParentID { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? ArchitectDate { get; set; } = null;
        public DateTime DateCalculated { get; set; }
        public int ProjectNumber { get; set; }
        public string ProjectLeadName { get; set; }
        public int years { get; set; }
        public int months { get; set; }
        public Category Category { get; set; } 
        public ConstructionType Type { get; set; }
        public string Location { get; set; }
        public bool UrbanArea { get; set; }
        public int ApartmentNumber { get; set; }
        public int BathNumber { get; set; }
        public int ToiletNumber { get; set; }
        public double Loa { get; set; }
        public double Boa { get; set; }
        public double darkBta { get; set; }
        public double lightBta { get; set; }
        public int InternalStandard { get; set; }
        public int ExternalStandard { get; set; }

        public double ProductionInclVat { get; set; }
        public double ProductionExclVat { get; set; }
        public double ClientInclVat { get; set; }
        public double ClientExclVat { get; set; }
        public double TotalInclVat { get; set; }
        public double TotalExclVat { get; set; }

        public double boaContract { get; set; }
        public double loaContract { get; set; }
        public double ApartmentNumberContract { get; set; }
        public double BathNumberContract { get; set; }
        public double ToiletNumberContract { get; set; }
        public double darkBtaContract { get; set; }
        public double lightBtaContract { get; set; }
        public double Contractor { get; set; }
        public double ContractInclVat { get; set; }
        public double ContractExclVat { get; set; }

        public double ProductionBoaLoaExclVat { get; set; }
        public double ProductionBoaLoaInclVat { get; set; }
        public double ProductionLightBtaExclVat { get; set; }
        public double ProductionLightBtaInclVat { get; set; }
        public double ProductionTotalBtaExclVat { get; set; }
        public double ProductionTotalBtaInclVat { get; set; }
        public double ProductionPerIghExclVat { get; set; }
        public double ProductionPerIghInclVat { get; set; }

        public double ClientBoaLoaExclVat { get; set; }
        public double ClientBoaLoaInclVat { get; set; }
        public double ClientLightBtaExclVat { get; set; }
        public double ClientLightBtaInclVat { get; set; }
        public double ClientTotalBtaExclVat { get; set; }
        public double ClientTotalBtaInclVat { get; set; }
        public double ClientPerIghExclVat { get; set; }
        public double ClientPerIghInclVat { get; set; }

        public double TotalBoaLoaExclVat { get; set; }
        public double TotalBoaLoaInclVat { get; set; }
        public double TotalLightBtaExclVat { get; set; }
        public double TotalLightBtaInclVat { get; set; }
        public double TotalTotalBtaExclVat { get; set; }
        public double TotalTotalBtaInclVat { get; set; }
        public double TotalPerIghExclVat { get; set; }
        public double TotalPerIghInclVat { get; set; }



        public User User { get; set; }

        public Calculation() { }

        public Calculation(int id, string name, DateTime date, string category, string type, string area)
        {
            ID = id;
            Name = name;
            
        }

    }
}
