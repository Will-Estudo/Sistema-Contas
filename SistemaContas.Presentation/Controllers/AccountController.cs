using Bogus;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaContas.Data.Entities;
using SistemaContas.Data.Helpers;
using SistemaContas.Data.Repositories;
using SistemaContas.Messages.Services;
using SistemaContas.Presentation.Models;
using System.Security.Claims;

namespace SistemaContas.Presentation.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }             

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var usuarioRepository = new UsuarioRepository();
                    var usuario = usuarioRepository.GetByEmailAndSenha(model.Email, MD5Helper.Encrypt(model.Senha));
                    if (usuario != null)
                    {
                        var identityViewModel = new IdentityViewModel();
                        identityViewModel.Id = usuario.Id;
                        identityViewModel.Nome = usuario.Nome;
                        identityViewModel.Email = usuario.Email;
                        identityViewModel.DataHoraAcesso = DateTime.Now;
                                                
                        // serializando dados do usuário autenticado para Json
                        var claimsIdentity = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, JsonConvert.SerializeObject(identityViewModel)),
                        }, CookieAuthenticationDefaults.AuthenticationScheme);
                        //gravando cookie de autenticação
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                        return RedirectToAction("Index","Home");
                    }
                    else
                    {
                        TempData["MensagemAlerta"] = "Acesso negado, usuário não encontrado.";
                    }
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = $"Falha ao autenticar usuário: {e.Message}";
                }
            }
            else
            {
                TempData["MensagemAlerta"] = "Ocorreram erros de validação no preenchimento do formulário";
            }

            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var usuarioRepository = new UsuarioRepository();

                    if (usuarioRepository.GetByEmail(model.Email) != null)
                    {
                        TempData["MensagemAlerta"] = "O e-mail informado já está cadastrado no sistema, tente outro.";
                    }
                    else
                    {
                        var usuario = new Usuario();
                        usuario.Id = Guid.NewGuid();
                        usuario.Nome = model.Nome;
                        usuario.Email = model.Email;
                        usuario.Senha = MD5Helper.Encrypt(model.Senha);

                        usuarioRepository.Add(usuario);

                        TempData["MensagemSucesso"] = "Parabéns, sua conta foi cadastrada com sucescco.";
                        ModelState.Clear();
                    }
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = $"Falha ao cadastrar usuário: {e.Message}";
                }
            }
            else
            {
                TempData["MensagemAlerta"] = "Ocorreram erros de validação no preenchimento do formulário";
            }
            return View();
        }

        public IActionResult PasswordRecover()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PasswordRecover(PasswordRecoverViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var usuarioRepository = new UsuarioRepository();
                    var usuario = usuarioRepository.GetByEmail(model.Email);

                    if (usuario != null) 
                    {
                        var novaSenha = new Faker().Internet.Password();

                        var emailDest = usuario.Email;
                        var assunto = "Recuperação de senha - Sistema Contas";
                        var mensagem = $@"
                            <h3>Olá, {usuario.Nome}</h3>
                            <p>Uma nova senha foi gerada para o seu usuário</p>
                            <p>Acesse o sistema com a senha: {novaSenha}</p>
                        ";

                        EmailService.EnviarMensagem(emailDest, assunto, mensagem);

                        usuarioRepository.Update(usuario.Id, MD5Helper.Encrypt(novaSenha));

                        TempData["MensagemSucesso"] = "Nova senha enviada para o e-mail cadastrado.";
                    }
                    else
                    {
                        TempData["MensagemAlerta"] = "Usuário não encontrado, verifique o e-mail informado.";
                    }
                }
                catch (Exception e)
                {
                    TempData["MensagemErro"] = $"Falha ao recuperar senha: {e.Message}";
                }
            }
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Account");
        }

    }
}
