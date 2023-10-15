﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calculations1.Models
{
    public class CalculationDto
    {
        public string Name { get; set; }
        public int ProjectNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ArchitectDate { get; set; }
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
    }
}
