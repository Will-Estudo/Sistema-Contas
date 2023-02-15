using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using OfficeOpenXml;
using SistemaContas.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaContas.Reports.Services
{
    public class CategoriasReportService
    {
        /// <summary>
        /// Método para refornar um relatório de categorias em formato Excel
        /// </summary>
        /// <param name="categorias">Lista de categorias</param>
        /// <returns>Arquivo em memória (bytes[])</returns>
        public byte[] GerarRelatorioExcel(List<Categoria> categorias)
        {
            //define o tipo de licença do EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            //criando conteúdo do arquivo
            using (var excelPackage = new ExcelPackage())
            {
                //nome da planilha
                var sheet = excelPackage.Workbook.Worksheets.Add("Categorias");

                //escrevendo nas celulas
                sheet.Cells["A1"].Value = "Relatório de categorias";

                sheet.Cells["A3"].Value = "ID";
                sheet.Cells["B3"].Value = "Nome da categoria";

                //imprimindo as categorias
                var linha = 4;

                foreach (var item in categorias)
                {
                    sheet.Cells[$"A{linha}"].Value = item.Id.ToString();
                    sheet.Cells[$"B{linha}"].Value = item.Nome;
                    linha++;
                }

                //formatando as celulas
                sheet.Cells["A:B"].AutoFitColumns();

                //retorna o arquivo em memória
                return excelPackage.GetAsByteArray();
            }
        }

        /// <summary>
        /// Método para refornar um relatório de categorias em formato PDF
        /// </summary>
        /// <param name="categorias">Lista de categorias</param>
        /// <returns>Arquivo em memória (bytes[])</returns>
        public byte[] GerarRelatorioPdf(List<Categoria> categorias)
        {
            var memoryStream = new MemoryStream();
            var pdf = new PdfDocument(new PdfWriter(memoryStream));

            using (var document = new Document(pdf))
            {
                document.Add(new Paragraph("Relatório de Categorias\n"));
                document.Add(new Paragraph($"Data e hora: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}"));

                var table = new Table(2);
                table.SetWidth(UnitValue.CreatePercentValue(100));
                table.AddHeaderCell("ID");
                table.AddHeaderCell("Nome da categoria");

                foreach (var item in categorias)
                {
                    table.AddCell(item.Id.ToString());
                    table.AddCell(item.Nome);
                }
                document.Add(table);
            }

            return memoryStream.ToArray();
        }
    }
}
