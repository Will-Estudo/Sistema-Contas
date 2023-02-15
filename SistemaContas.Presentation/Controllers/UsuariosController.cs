using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaContas.Data.Entities;
using SistemaContas.Data.Helpers;
using SistemaContas.Data.Repositories;
using SistemaContas.Presentation.Models;

namespace SistemaContas.Presentation.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        public IActionResult MinhaConta()
        {
            return View();
        }

        [HttpPost]
        public IActionResult MinhaConta(AlterarSenhaViewModel model)
        {
            if(ModelState.IsValid) 
            {
                try
                {
                    var data = User.Identity.Name;
                    var identityViewModel = JsonConvert.DeserializeObject<IdentityViewModel>(data);

                    var usuarioRepository = new UsuarioRepository();
                    usuarioRepository.Update(identityViewModel.Id, MD5Helper.Encrypt(model.NovaSenha));
                    TempData["MensagemSucesso"] = $"Conta atualizada com sucesso.";
                    ModelState.Clear();
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = $"Falha ao atualizar senha do usuário: {e.Message}";
                }
            }
            else
            {
                TempData["MensagemAlerta"] = "Ocorreram dados de validação no preenchimento do formulário.";
            }

            return View();
        }
    }
}
