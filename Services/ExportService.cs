using OfficeOpenXml;
using ShiftManagement.Models;
using System;
using System.Text;

namespace ShiftManagement.Services
{
    public class ExportService
    {
        static ExportService()
        {
            OfficeOpenXml.ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public byte[] ExportReportToCsv(Report report)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Отчет о рабочем времени");
            sb.AppendLine($"Сотрудник: {report.Employee?.FirstName} {report.Employee?.LastName}");
            sb.AppendLine($"Период: {report.PeriodStart:dd.MM.yyyy} - {report.PeriodEnd:dd.MM.yyyy}");
            sb.AppendLine($"Дата создания: {report.CreatedAt:dd.MM.yyyy HH:mm}");
            sb.AppendLine("");
            sb.AppendLine($"Всего часов отработано: {report.TotalWorkHours}");
            sb.AppendLine($"Количество смен: {report.ShiftCount}");
            sb.AppendLine($"Примечания: {report.Notes ?? "Нет"}");

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public byte[] ExportReportToExcel(Report report)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Отчет");

                worksheet.Cells[1, 1].Value = "Отчет о рабочем времени";
                worksheet.Cells[2, 1].Value = "Сотрудник:";
                worksheet.Cells[2, 2].Value = $"{report.Employee?.FirstName} {report.Employee?.LastName}";
                worksheet.Cells[3, 1].Value = "Период:";
                worksheet.Cells[3, 2].Value = $"{report.PeriodStart:dd.MM.yyyy} - {report.PeriodEnd:dd.MM.yyyy}";
                worksheet.Cells[4, 1].Value = "Всего часов:";
                worksheet.Cells[4, 2].Value = report.TotalWorkHours;
                worksheet.Cells[5, 1].Value = "Количество смен:";
                worksheet.Cells[5, 2].Value = report.ShiftCount;

                worksheet.Cells.AutoFitColumns();

                return package.GetAsByteArray();
            }
        }
    }
}
