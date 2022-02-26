using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Tax.Entity.Common;

namespace Tax.Rules.Engine
{
    public class TaxRuleEngine
    {

        /// <summary>
        /// This method is to read the external Rule set File
        /// and create list of Tax rule specific to City 
        /// </summary>
        /// <returns></returns>
        public CityTaxRules RunTaxRuleEngine()
        {
            var cityTaxRule = new CityTaxRules()
            {
                CityTaxRuleList = new List<TaxRule>()
            };

            string fileName = "C:\\TaxCalculator\\Documentation\\City Tax Rules.xlsx";

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var excelPackage = new ExcelPackage(new FileInfo(fileName));
            var worksheet = excelPackage.Workbook.Worksheets["TaxRule"];

            for (int rw = 2; rw <= worksheet.Dimension.End.Row; rw++)
            {
                if (worksheet.Cells[rw, 1].Value != null)
                {
                    DateTime startTime = (DateTime)worksheet.Cells[rw, 1].Value;
                    DateTime endTime = (DateTime)worksheet.Cells[rw, 2].Value;
                    int amount = Convert.ToInt32(Regex.Match(worksheet.Cells[rw, 3].Value.ToString(), @"\d+").Value); ;

                    var taxRule = new TaxRule()
                    {
                        StartTime = startTime.TimeOfDay,
                        EndTime = endTime.TimeOfDay,
                        Amount = amount

                    };

                    cityTaxRule.CityTaxRuleList.Add(taxRule);
                }
            }

            return cityTaxRule;
        }
    }
}
