using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaContas.Data.Entities;
using SistemaContas.Data.Repositories;
using SistemaContas.Presentation.Models;

namespace SistemaContas.Presentation.Controllers
{
    [Authorize]
    public class CategoriasController : Controller
    {
        public IActionResult Cadastro()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Cadastro(CategoriasCadastroViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var categoria = new Categoria();
                    categoria.Id = Guid.NewGuid();
                    categoria.Nome = model.Nome;
                    categoria.IdUsuario = UsuarioAutenticado.Id;

                    var categoriaRepository = new CategoriaRepository();
                    categoriaRepository.Add(categoria);

                    TempData["MensagemSucesso"] = $"Categoria {categoria.Nome} cadastrada com sucesso.";
                    ModelState.Clear();
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = $"Falha ao cadastrar categoria: {e.Message}";
                }
            }
            else
            {
                TempData["MensagemAlerta"] = "Ocorreram dados de validação no preenchimento do formulário.";
            }

            return View();
        }

        public IActionResult Consulta()
        {
            var model = new List<CategoriasConsultaViewModel>();

            try
            {
                var categoriaRepository = new CategoriaRepository();
                foreach (var item in categoriaRepository.GetByUsario(UsuarioAutenticado.Id))
                {
                    model.Add(new CategoriasConsultaViewModel { Id = item.Id, Nome = item.Nome });

                    /* // Ou fazendo passo a passo
                    var categoriaConsultaViewModel = new CategoriasConsultaViewModel();
                    categoriaConsultaViewModel.Id = item.Id;
                    categoriaConsultaViewModel.Nome = item.Nome;
                    model.Add(categoriaConsultaViewModel);*/
                }
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = $"Falha ao consultar categorias: {e.Message}";
            }

            return View(model);
        }

        public IActionResult Edicao(Guid id)
        {
            var model = new CategoriasEdicaoViewModel();

            try
            {
                var categoriaRepository = new CategoriaRepository();
                var categoria = categoriaRepository.GetById(id);

                if (categoria != null && categoria.IdUsuario == UsuarioAutenticado.Id)
                {
                    model.Id = categoria.Id;
                    model.Nome = categoria.Nome;
                }
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = $"Falha ao excluir categoria: {e.Message}";
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Edicao(CategoriasEdicaoViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var categoriaRepository = new CategoriaRepository();
                    var categoria = categoriaRepository.GetById(model.Id);

                    if (categoria != null && categoria.IdUsuario == UsuarioAutenticado.Id)
                    {
                        categoria.Nome = model.Nome;
                        categoriaRepository.Update(categoria);
                        TempData["MensagemSucesso"] = $"Categoria {categoria.Nome} atualizada com sucesso.";
                        return RedirectToAction("Consulta");
                    }
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = $"Falha ao atualizar categoria: {e.Message}";
                }
            }
            else
            {
                TempData["MensagemAlerta"] = "Ocorreram dados de validação no preenchimento do formulário.";
            }

            return View(model);
        }

        public IActionResult Exclusao(Guid id)
        {
            try
            {
                var categoriaRepository = new CategoriaRepository();
                var categoria = categoriaRepository.GetById(id);

                if (categoria != null && categoria.IdUsuario == UsuarioAutenticado.Id)
                {
                    var qtdContas = categoriaRepository.CountContasByIdCategoria(id);
                    if (qtdContas == 0)
                    {
                        categoriaRepository.Delete(categoria);
                        TempData["MensagemSucesso"] = $"Categoria {categoria.Nome} excluída com sucesso.";
                    }
                    else
                    {
                        TempData["MensagemAlerta"] = $"Não é possível excluir, pois existem {qtdContas} contas cadastradas para esta categoria.";
                    }
                }
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = $"Falha ao excluir categoria: {e.Message}";
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
    }
}
