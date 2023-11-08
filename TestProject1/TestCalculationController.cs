using Calculations1.Controllers;
using Calculations1.Models;
using Calculations1.Services.CalculationsService;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Xml;

namespace TestProject1
{
    [TestClass]
    public class TestCalculationController
    {
        static IHttpContextAccessor httpContext;
        static CalculationsService calculationsService;
        static CalculationsController calculationsController;

        public TestContext testContext { get; set; }

        [ClassInitialize]
        public static void Init(TestContext testContext)
        {
            httpContext = new HttpContextAccessor();
            calculationsService = new CalculationsService(httpContext);
            calculationsController = new CalculationsController(calculationsService);

        }


        [TestMethod]
        public void TestCalculationInitialization()
        {
            Calculation calculation = new Calculation();
            calculation.Name = "name";
        }

        [TestMethod]
        public void TestControllerGet()
        {
            calculationsController.Get();
            

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestControllerAdd()
        {
            CalculationDto calculationDto = new CalculationDto();
            calculationDto.Name = "Name";
            calculationDto.BathNumber = 3;

            calculationsController.AddCalculation(calculationDto);

        }

        public static IEnumerable<object[]> CalculationDto1
        {
            get
            {
                return new[]
                {
            new object[] {null, 0, 0, 0, 0, 0, 0, 0},
             new object[] {"name", 1, 1, 0, 0, 0, 0, 0},
             new object[] {"name", 1, 2, 2, 0, 0, 0, 0},
             new object[] {"name", 2, 1, 3, 1, 0, 0, 0},
             new object[] {"name", 2, 4, 4, 2, 1, 0, 0},
             new object[] {"name", 2, 4, 4, 2, 1, 2, 0},
             new object[] {"name", 2, 4, 4, 2, 1, 0, 0}
        };
            }
        }//name, years, months, category, type, boa, loa, standard

        [TestMethod]
        [DynamicData("CalculationDto1")]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInitExcepitons(string name, int years, int months, int category, int type, int boa, int loa, int standard)
        {
            CalculationDto calculationDto = new CalculationDto();
            calculationDto.Name = name;
            calculationDto.years = years;
            calculationDto.months = months;
            calculationDto.Category = (Category)category;
            calculationDto.Type = (ConstructionType)type;
            calculationDto.Boa = boa;
            calculationDto.Loa = loa;
            calculationDto.InternalStandard = standard;

            calculationsController.AddCalculation(calculationDto);
        }

        static IEnumerable<object[]> CalculationDtos2
        {
            get
            {
                return new[]
                {    //name, years, months, category, type, boa, loa, standard, location
             new object[] {"name", 2, 0, 4, 2, 1, 0, 1, "Gothenburg"},
             new object[] {"name lastname", 1, 1, 14, 2, 3, 2, 2, "Malmo"},
             new object[] {"name 123", 1, 2, 22, 1, 2, 7, 1, "Orter"},
             new object[] {"name_mon", 2, 1, 29, 1, 2, 32, 2, "Orter"},
             new object[] {"name,.", 2, 4, 14, 2, 1, 8, 1, "Gothenburg"},
             new object[] {"name2", 2, 4, 19, 1, 1, 2, 1, "Malmo"},
             new object[] {"name  ", 2, 4, 4, 2, 1, 0, 1, "Malmo"}

         };
            }
        }

        [TestMethod]
        [DynamicData("CalculationDtos2")]
        public void TestInit(string name, int years, int months, int category, int type, int boa, int loa, int standard, string location)
        {
            CalculationDto calculationDto = new CalculationDto();
            calculationDto.Name = name;
            calculationDto.years = years;
            calculationDto.months = months;
            calculationDto.Category = (Category)category;
            calculationDto.Type = (ConstructionType)type;
            calculationDto.Boa = boa;
            calculationDto.Loa = loa;
            calculationDto.InternalStandard = standard;
            calculationDto.ExternalStandard = standard;
            calculationDto.Location = location;

            calculationsController.AddCalculation(calculationDto);

            var returnObject = calculationsController.Get();

            var actual = (returnObject.Result as OkObjectResult)?.Value as ServiceResponse<List<Calculation>>;
            Assert.IsTrue(actual.Data[0].TotalInclVat > 10000);



        }


        public static IEnumerable<object[]> ReadCSV()
        {
            using (var reader = new StreamReader("dataCSV.txt"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var rows = csv.GetRecords<dynamic>();
                foreach (var row in rows)
                {
                    yield return new object[] { row.name, Int32.Parse(row.years),
              Int32.Parse(row.months), Int32.Parse(row.category),
                Int32.Parse(row.type),
              Int32.Parse(row.boa), Int32.Parse(row.loa),
              Int32.Parse(row.standard), row.location};
                }
            }
        }

        static IEnumerable<object[]> CalculationsCSV
        {
            get
            {
                return ReadCSV();
            }
        }

        

        public static IEnumerable<object[]> ReadXML()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("dataXML.xml");
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                List<string> elements = new List<string>();
                foreach (XmlNode innerNode in node)
                {
                    elements.Add(innerNode.InnerText);
                }
                yield return new object[] { elements[0], Int32.Parse(elements[1]),
            Int32.Parse(elements[2]), Int32.Parse(elements[3]),
            Int32.Parse(elements[4]), Int32.Parse(elements[5]), Int32.Parse(elements[6]),
            Int32.Parse(elements[7]), elements[8]
        };
            }
        }

        static IEnumerable<object[]> CalculationsXML
        {
            get
            {
                return ReadXML();
            }
        }


        [TestMethod]
        [DynamicData("CalculationsXML")]
        public void TestInitXML(string name, int years, int months, int category, int type, int boa, int loa, int standard, string location)
        {
            CalculationDto calculationDto = new CalculationDto();
            calculationDto.Name = name;
            calculationDto.years = years;
            calculationDto.months = months;
            calculationDto.Category = (Category)category;
            calculationDto.Type = (ConstructionType)type;
            calculationDto.Boa = boa;
            calculationDto.Loa = loa;
            calculationDto.InternalStandard = standard;
            calculationDto.ExternalStandard = standard;
            calculationDto.Location = location;

            calculationsController.AddCalculation(calculationDto);

            var returnObject = calculationsController.Get();

            var actual = (returnObject.Result as OkObjectResult)?.Value as ServiceResponse<List<Calculation>>;
            Assert.IsTrue(actual.Data[0].TotalInclVat > 10000);


        }

        [TestMethod]
        public void TestWithMock()
        {
            var mockRepository = new Mock<ICalculationsService>();
            var expectedResult = new ServiceResponse<List<Calculation>> { Succes = true, Data = new List<Calculation>() };
            mockRepository.Setup(repo => repo.GetAllCalculations()).Returns(expectedResult);
            var calculationsController = new CalculationsController(mockRepository.Object);

            var returnObject = calculationsController.Get();

            var actual = (returnObject.Result as OkObjectResult)?.Value as ServiceResponse<List<Calculation>>;
            Assert.AreEqual(actual.Data.Count, 0);





        }
    }
}
