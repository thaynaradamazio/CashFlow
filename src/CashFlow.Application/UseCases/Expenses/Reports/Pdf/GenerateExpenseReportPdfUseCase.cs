using CashFlow.Application.UseCases.Expenses.Reports.Pdf.Colors;
using CashFlow.Application.UseCases.Expenses.Reports.Pdf.Fonts;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Reports;
using CashFlow.Domain.Repositories.Expenses;
using DocumentFormat.OpenXml.Bibliography;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Fonts;
using System.Reflection;
using Font = MigraDoc.DocumentObjectModel.Font;

namespace CashFlow.Application.UseCases.Expenses.Reports.Pdf
{
    public class GenerateExpenseReportPdfUseCase : IGenerateExpenseReportPdfUseCase
    {
        private const string CURRENCY_SYMBOL = "$";
        private readonly IExpensesReadOnlyRepository _repository;

        public GenerateExpenseReportPdfUseCase(IExpensesReadOnlyRepository repository)
        {
            _repository = repository;

            GlobalFontSettings.FontResolver = new ExpensesReportFontResolver();
        }

        public async Task<byte[]> Execute(DateOnly month)
        {
            var expenses = await _repository.FilterByMonth(month);
            if(expenses.Count == 0)
            {
                return [];
            }

            var document = CreateDocument(month);
            var page = CreatePage(document);

            CreateHeaderWithProfilePhotoAndName(page);

            var totalExpenses = expenses.Sum(expense => expense.Amount);
            CreateTotalSpentSection(page, month, totalExpenses);

            foreach (var expense in expenses)
            {
                var table = CreateExpenseTable(page);

                var row = table.AddRow();
                row.Height = 25;

                row.Cells[0].AddParagraph(expense.Title);
                row.Cells[0].Format.Font = new Font { Name = FontHelper.RALEWAY_BLACK, Size = 14, Color = ColorsHelper.BLACK };
                row.Cells[0].Shading.Color = ColorsHelper.RED_LIGHT;
                row.Cells[0].VerticalAlignment = VerticalAlignment.Center;
                row.Cells[0].MergeRight = 2;

                row.Cells[3].AddParagraph(ResourceReportGenerationMessages.AMOUNT);
                row.Cells[3].Format.Font = new Font { Name = FontHelper.RALEWAY_BLACK, Size = 14, Color = ColorsHelper.WHITE };
                row.Cells[3].Shading.Color = ColorsHelper.RED_DARK;
                row.Cells[3].VerticalAlignment = VerticalAlignment.Center;
            }

            return RenderDocument(document);
        }

        private Document CreateDocument(DateOnly month)
        {
            var document = new Document();

            document.Info.Title = $"{ResourceReportGenerationMessages.EXPENSES_FOR} {month:Y}";
            document.Info.Author = "Thaynara Damazio";

            var style = document.Styles["Normal"];
            style!.Font.Name = FontHelper.RALEWAY_REGULAR;

            return document;
        }

        private Section CreatePage(Document document)
        {
            var section = document.AddSection();
            section.PageSetup = document.DefaultPageSetup.Clone();

            section.PageSetup.PageFormat = PageFormat.A4;
            section.PageSetup.LeftMargin = 40;
            section.PageSetup.RightMargin = 40;
            section.PageSetup.TopMargin = 80;
            section.PageSetup.BottomMargin = 80;

            return section;
        }

        private void CreateHeaderWithProfilePhotoAndName(Section page)
        {
            var table = page.AddTable();
            table.AddColumn();
            table.AddColumn("300");

            var row = table.AddRow();

            var assembly = Assembly.GetExecutingAssembly();
            var directoryName = Path.GetDirectoryName(assembly.Location);
            var pathFile = Path.Combine(directoryName!, "Logo", "machos_red_black2.jpg");

            row.Cells[0].AddImage(pathFile);

            row.Cells[1].AddParagraph("Olá, Thaynara Damazio");
            row.Cells[1].Format.Font = new Font { Name = FontHelper.RALEWAY_BLACK, Size = 16 };
            row.Cells[1].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
        }

        private void CreateTotalSpentSection(Section page, DateOnly month, decimal totalExpenses)
        {
            var paragraph = page.AddParagraph();
            paragraph.Format.SpaceBefore = "40";
            paragraph.Format.SpaceAfter = "40";

            var title = string.Format(ResourceReportGenerationMessages.TOTAL_SPENT_IN, month.ToString("Y"));

            paragraph.AddFormattedText(title, new Font { Name = FontHelper.RALEWAY_REGULAR, Size = 15 });

            paragraph.AddLineBreak();

            paragraph.AddFormattedText($"{totalExpenses} {CURRENCY_SYMBOL}", new Font { Name = FontHelper.WORKSANS_BLACK, Size = 50 });
        }

        private Table CreateExpenseTable(Section page)
        {
            var table = page.AddTable();

            table.AddColumn("195").Format.Alignment = ParagraphAlignment.Left;
            table.AddColumn("80").Format.Alignment = ParagraphAlignment.Center;
            table.AddColumn("120").Format.Alignment = ParagraphAlignment.Center;
            table.AddColumn("120").Format.Alignment = ParagraphAlignment.Right;

            return table;
        }

        private byte[] RenderDocument(Document document)
        {
            var renderer = new PdfDocumentRenderer
            {
                Document = document,
            };

            renderer.RenderDocument();

            using var file = new MemoryStream();
            renderer.PdfDocument.Save(file);

            return file.ToArray();
        }
    }
}
