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
    public class ContasReportService
    {
        /// <summary>
        /// Método para refornar um relatório de contas em formato Excel
        /// </summary>
        /// <param name="contas">Lista de contas</param>
        /// <returns>Arquivo em memória (bytes[])</returns>
        public byte[] GerarRelatorioExcel(List<Conta> contas)
        {
            //define o tipo de licença do EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            //criando conteúdo do arquivo
            using (var excelPackage = new ExcelPackage())
            {
                //nome da planilha
                var sheet = excelPackage.Workbook.Worksheets.Add("Contas");

                //escrevendo nas celulas
                sheet.Cells["A1"].Value = "Relatório de contas";

                sheet.Cells["A3"].Value = "ID";
                sheet.Cells["B3"].Value = "Nome da conta";
                sheet.Cells["C3"].Value = "Data";
                sheet.Cells["D3"].Value = "Valor";
                sheet.Cells["E3"].Value = "Tipo";
                sheet.Cells["F3"].Value = "Categoria";
                sheet.Cells["G3"].Value = "Observações";

                //imprimindo as categorias
                var linha = 4;

                foreach (var item in contas)
                {
                    sheet.Cells[$"A{linha}"].Value = item.Id.ToString();
                    sheet.Cells[$"B{linha}"].Value = item.Nome;
                    sheet.Cells[$"C{linha}"].Value = item.Data.ToString("dd/MM/yyyy");
                    sheet.Cells[$"D{linha}"].Value = item.Valor.ToString("c");
                    sheet.Cells[$"E{linha}"].Value = item.Tipo.ToString();
                    sheet.Cells[$"F{linha}"].Value = item.Categoria.Nome;
                    sheet.Cells[$"G{linha}"].Value = item.Observacoes;                    
                    linha++;
                }

                //formatando as celulas
                sheet.Cells["A:G"].AutoFitColumns();

                //retorna o arquivo em memória
                return excelPackage.GetAsByteArray();
            }
        }

        /// <summary>
        /// Método para refornar um relatório de contas em formato Excel
        /// </summary>
        /// <param name="contas">Lista de contas</param>
        /// <returns>Arquivo em memória (bytes[])</returns>
        public byte[] GerarRelatorioPdf(List<Conta> contas)
        {
            var memoryStream = new MemoryStream();
            var pdf = new PdfDocument(new PdfWriter(memoryStream));

            using (var document = new Document(pdf))
            {
                document.Add(new Paragraph("Relatório de contas\n"));
                document.Add(new Paragraph($"Data e hora: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}"));
                document.Add(new Paragraph("\n\n"));

                foreach (var item in contas)
                {
                    document.Add(new Paragraph($"ID da conta: {item.Id.ToString()}"));
                    document.Add(new Paragraph($"Nome: {item.Nome}"));
                    document.Add(new Paragraph($"Data: {item.Data.ToString("dd/MM/yyyy")}"));
                    document.Add(new Paragraph($"Valor: {item.Valor.ToString("c")}"));
                    document.Add(new Paragraph($"Tipo: {item.Tipo.ToString()}"));
                    document.Add(new Paragraph($"Categoria: {item.Categoria.Nome}"));
                    document.Add(new Paragraph($"Observações: {item.Observacoes}"));
                    document.Add(new Paragraph("\n\n"));
                }                
            }

            return memoryStream.ToArray();
        }
    }
}
