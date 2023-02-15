using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SistemaContas.Data.Entities;
using SistemaContas.Data.Enums;
using SistemaContas.Data.Repositories;
using SistemaContas.Presentation.Models;
using SistemaContas.Reports.Services;

namespace SistemaContas.Presentation.Controllers
{
    [Authorize]
    public class ContasController : Controller
    {
        public IActionResult Cadastro()
        {
            var model = new ContasCadastroViewModel();
            model.Categorias = ObterCategorias();

            return View(model);
        }

        [HttpPost]
        public IActionResult Cadastro(ContasCadastroViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var conta = new Conta();
                    conta.Id = Guid.NewGuid();
                    conta.Nome = model.Nome;
                    conta.Valor = model.Valor.Value;
                    conta.Data = model.Data.Value;
                    conta.Tipo = model.Tipo == 1 ? TipoConta.Receber : TipoConta.Pagar;
                    conta.Observacoes = model.Observacoes;
                    conta.IdCategoria = model.IdCategoria.Value;
                    conta.IdUsuario = UsuarioAutenticado.Id;

                    var contaRepository = new ContaRepository();
                    contaRepository.Add(conta);

                    TempData["MensagemSucesso"] = $"Conta {conta.Nome} cadastrada com sucesso.";
                    model = new ContasCadastroViewModel();
                    ModelState.Clear();
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = $"Falha ao cadastrar conta: {e.Message}";
                }
            }
            else
            {
                TempData["MensagemAlerta"] = "Ocorreram dados de validação no preenchimento do formulário.";
            }


            model.Categorias = ObterCategorias();
            return View(model);
        }

        public IActionResult Consulta()
        {
            var model = new ContasConsultaViewModel();
            try
            {
                var dataAtual = DateTime.Now;
                var dataIni = new DateTime(dataAtual.Year, dataAtual.Month, 1);
                var dataFim = dataIni.AddMonths(1).AddDays(-1);

                model.DataIni = dataIni.ToString("yyyy-MM-dd");
                model.DataFim = dataFim.ToString("yyyy-MM-dd");

                ObterContas(model);
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = $"Falha ao consultar contas: {e.Message}";
            }            

            return View(model);
        }

        [HttpPost]
        public IActionResult Consulta(ContasConsultaViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ObterContas(model);
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = $"Falha ao consultar contas: {e.Message}";
                }
            }
            /*else
            {
                TempData["MensagemAlerta"] = "Ocorreram dados de validação no preenchimento do formulário.";
            }*/

            return View(model);
        }

        public IActionResult Edicao(Guid id)
        {
            var model = new ContasEdicaoViewModel();

            try
            {
                var contaRepository = new ContaRepository();
                var conta = contaRepository.GetById(id);

                if(conta != null && conta.IdUsuario == UsuarioAutenticado.Id)
                {
                    model.Id = conta.Id;
                    model.Nome = conta.Nome;
                    model.Valor = conta.Valor;
                    model.Data = conta.Data.ToString("yyyy-MM-dd");
                    model.Tipo = conta.Tipo == TipoConta.Receber ? 1 : 2;
                    model.Observacoes = conta.Observacoes;
                    model.IdCategoria = conta.IdCategoria;
                    model.Categorias = ObterCategorias();
                }
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = $"Falha ao consultar contas: {e.Message}";
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Edicao(ContasEdicaoViewModel model)
        {
            if (ModelState.IsValid) 
            {
                try
                {
                    var contaRepository = new ContaRepository();
                    var conta = contaRepository.GetById(model.Id);
                    if(conta != null && conta.IdUsuario == UsuarioAutenticado.Id)
                    {
                        conta.Nome = model.Nome;
                        conta.Data = DateTime.Parse(model.Data);
                        conta.Valor = model.Valor.Value;
                        conta.Tipo = model.Tipo == 1 ? TipoConta.Receber : TipoConta.Pagar;
                        conta.IdCategoria = model.IdCategoria.Value;
                        conta.Observacoes = model.Observacoes;

                        contaRepository.Update(conta);
                        TempData["MensagemSucesso"] = $"Conta {conta.Nome} atualizada com sucesso.";
                        return RedirectToAction("Consulta");
                    }
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = $"Falha ao atualizar conta: {e.Message}";
                }
            }
            model.Categorias = ObterCategorias();
            return View(model);
        }

        public IActionResult Exclusao(Guid Id)
        {
            try
            {
                var contaRepository = new ContaRepository();
                var conta = contaRepository.GetById(Id);

                if (conta != null && conta.IdUsuario == UsuarioAutenticado.Id)
                {
                    contaRepository.Delete(conta);
                    TempData["MensagemSucesso"] = $"Conta {conta.Nome} excluída com sucesso.";                    
                }
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = $"Falha ao excluir contas: {e.Message}";
            }

            return RedirectToAction("Consulta");
        }

        /// <summary>
        /// Método para retornar uma lista de seleção contendo categorias
        /// </summary>        
        private List<SelectListItem> ObterCategorias()
        {
            var categoriaRepository = new CategoriaRepository();
            var categorias = categoriaRepository.GetByUsario(UsuarioAutenticado.Id);

            var lista = new List<SelectListItem>();
            foreach (var item in categorias)
            {
                var selectListItem = new SelectListItem();
                selectListItem.Value = item.Id.ToString();
                selectListItem.Text = item.Nome;
                lista.Add(selectListItem);
            }

            /*foreach (var item in new CategoriaRepository().GetByUsario(UsuarioAutenticado.Id))
            {
                lista.Add(
                    new SelectListItem
                    {
                        Value = item.Id.ToString(),
                        Text = item.Nome                           
                    }
                );
            }*/

            return lista;
        }

        public IActionResult RelatorioExcel()
        {
            try
            {
                var contaRepository = new ContaRepository();
                var contas = contaRepository.GetByUsario(UsuarioAutenticado.Id);

                var relatorio = new ContasReportService().GerarRelatorioExcel(contas);
                return File(relatorio, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Relatorio_contas.xlsx");
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = $"Falha ao gerar relatório: {e.Message}";
            }
            return RedirectToAction("Consulta");
        }

        public IActionResult RelatorioPdf()
        {
            try
            {
                var contaRepository = new ContaRepository();
                var contas = contaRepository.GetByUsario(UsuarioAutenticado.Id);

                var relatorio = new ContasReportService().GerarRelatorioPdf(contas);
                return File(relatorio, "application/pdf", "Relatorio_contas.pdf");
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = $"Falha ao gerar relatório: {e.Message}";
            }
            return RedirectToAction("Consulta");
        }

        /// <summary>
        /// Retorna dados do usuário autenticado
        /// </summary>
        private IdentityViewModel UsuarioAutenticado
        {
            get
            {
                // obtendo dados do usuário autenticado (cookie)
                var data = User.Identity.Name;
                return JsonConvert.DeserializeObject<IdentityViewModel>(data);
            }
        }

        private void ObterContas(ContasConsultaViewModel model)
        {
            var contaRepository = new ContaRepository();
            var contas = contaRepository.GetByUsarioAndDatas(UsuarioAutenticado.Id, DateTime.Parse(model.DataIni), DateTime.Parse(model.DataFim));

            model.Resultado = new List<ContasConsultaResultadoViewModel>();
            foreach (var item in contas)
            {
                model.Resultado.Add(
                    new ContasConsultaResultadoViewModel
                    {
                        Id = item.Id,
                        Nome = item.Nome,
                        Data = item.Data.ToString("ddd, dd/MM/yyyy"),
                        Valor = item.Valor.ToString("c"),
                        Tipo = item.Tipo.ToString(),
                        Categoria = item.Categoria.Nome
                    }
                );
            }            
        }
    }
}
