using ClosedXML.Excel;

namespace CashFlow.Application.UseCases.Expenses.Reports.Excel
{
    public class GenerateExpenseReportExcelUseCase : IGenerateExpenseReportExcelUseCase
    {
        public async Task<byte[]> Execute(DateOnly month)
        {
            var workbook = new XLWorkbook();

            workbook.Author = "thay damazio";
            workbook.Style.Font.FontSize = 12;
            workbook.Style.Font.FontName = "Times New Roman";     
           
            var worksheet = workbook.Worksheets.Add(month.ToString("Y"));
        }
    }
}
