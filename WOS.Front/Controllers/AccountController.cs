﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using WOS.Model;
using System.Security.Cryptography;
using WOS.Dal.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;

namespace WOS.Front.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly IClientSrv _clientSrv;
        private readonly IAdminSrv _adminSrv;
        private readonly IConfiguration _configuration;
        private readonly IAuthenticationSrv _authenticationSrv;
        private readonly IProduitSrv _produitSrv;
        private readonly ICommandeSrv _commandeSrv;

        public AccountController(IClientSrv clientSrv, IAdminSrv adminSrv, IConfiguration configuration, IAuthenticationSrv authenticationSrv, IProduitSrv produitSrv, ICommandeSrv commandeSrv)
        {
            _clientSrv = clientSrv;
            _adminSrv = adminSrv;
            _configuration = configuration;
            _authenticationSrv = authenticationSrv;
            _produitSrv = produitSrv;
            _commandeSrv = commandeSrv;
        }

        #region Account
        [Route("")]
        [Authorize]
        public async Task<ActionResult> Index()
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            AccountViewModel accountViewModel = new AccountViewModel();
            Admin admin = null;

            Client client = _clientSrv.GetClientByEmail(userEmail);

            if (client == null)
                admin = _adminSrv.GetAdminByEmail(userEmail);

            if (client == null && admin == null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("LogIn");
            }

            HttpContext.Session.SetString("ReturnUrl", HttpContext.Request.Path);

            if (User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role) != null)
            {
                if (User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value.ToString().Equals("Admin"))
                {
                    accountViewModel.User = _adminSrv.GetAdminByEmail(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value.ToString());
                    accountViewModel.Produits = _produitSrv.GetProduits();
                    accountViewModel.Commandes = _commandeSrv.GetCommandes();
                }
                else
                {
                    Client clientView = _clientSrv.GetClientByEmail(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value.ToString());
                    accountViewModel.User = clientView;
                    accountViewModel.Commandes = _commandeSrv.GetCommandesByClientId(clientView.Id);
                }
            }

            return View(accountViewModel);
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> LogOut()
        {
            // On se déconnecte totalement
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var returnUrl = HttpContext.Session.GetString("ReturnUrl");

            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region SignIn
        [Route("signin")]
        [HttpGet]
        public IActionResult SignIn(SignInViewModel model = null)
        {
            if (model == null)
                model = new SignInViewModel();
            if (model.ErrorMessage != null)
                ViewBag.ErrorMessage = model.ErrorMessage;
            if (model.Client == null)
                model.Client = new Client();
            return View(model);
        }

        [Route("signin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignInPost(string nom, string prenom, string email, string password)
        {
            bool clientWithSameEmail = _clientSrv.ClientExists(email);

            Client client = new Client
            {
                Nom = nom,
                Prenom = prenom,
                Email = email,
                MotDePasse = password,
                AncienMotDePasse = password
            };

            SignInViewModel signInViewModel = new SignInViewModel
            {
                Client = client
            };

            if (clientWithSameEmail)
            {
                signInViewModel.ErrorMessage = "Un compte existe déjà avec cet e-mail.";
                return RedirectToAction("SignIn", signInViewModel);
            }

            // On controle les champs
            if (nom == null || prenom == null || email == null || password == null)
            {
                if (string.IsNullOrEmpty(nom))
                {
                    ViewData["ErrorLastName"] = "Le nom est obligatoire.";
                }
                else if (!IsNameValid(nom))
                {
                    ViewData["ErrorLastName"] = "Le nom est obligatoire.";
                }

                if (string.IsNullOrEmpty(prenom))
                {
                    ViewData["ErrorFirstName"] = "Le prénom est obligatoire.";
                }
                else if (!IsFirstNameValid(prenom))
                {
                    ViewData["ErrorFirstName"] = "Le prénom est obligatoire.";
                }

                if (string.IsNullOrEmpty(email))
                {
                    ViewData["ErrorEmail"] = "L'email est obligatoire.";
                }
                else if (!IsEmailValid(email))
                {
                    ViewData["ErrorEmail"] = "Veuillez renseigner un email valide.";
                }

                if (string.IsNullOrEmpty(password))
                {
                    ViewData["ErrorPassword"] = "Le mot de passe est obligatoire.";
                }
                else if (!IsPasswordValid(password))
                {
                    ViewData["ErrorPassword"] = "Le mot de passe doit contenir au minimum 8 caractères";
                }

                return View("SignIn", signInViewModel);
            }


            client.MotDePasse = HashPassword(password);

            _clientSrv.AddClient(client);

            // On envoie le mail de vérification d'email
            string token = GenerateSecureToken();
            string subject = "Vérification de votre adresse e-mail";
            // On récupère la value du domaine dans le appsettings.json
            string domain = _configuration.GetSection("Site")["Domain"];
            string body = $"Veuillez cliquer sur le lien suivant pour vérifier votre adresse e-mail : <a href='{domain}account/verifyemail?email={email}&token={token}'>Vérifier mon adresse e-mail</a>";
            _authenticationSrv.SendEmail(email, subject, body);

            // On stocke le token dans la base de données
            client.VerificationToken = token;
            client.TokenExpiryDate = DateTime.Now.AddHours(24);
            _clientSrv.UpdateClient(client);

            _authenticationSrv.LoginAccountClient(client);

            // On redirige vers la méthode post de connexion pour se connecter
            return RedirectToAction("SignInSuccess");
        }

        [Route("signinsuccess")]
        [HttpGet]
        public IActionResult SignInSuccess()
        {
            return View();
        }

        [Route("verifyemail")]
        [HttpGet]
        public IActionResult VerifyEmail(string email, string token)
        {
            // On vérifie le token
            if (token == null || token.Length != 43)
            {
                return View("ErrorVerifyEmail", new SignInViewModel { ErrorMessage = "Token invalide" });
            }

            // On vérifie l'email
            if (email == null || !IsEmailValid(email))
            {
                return View("ErrorVerifyEmail", new SignInViewModel { ErrorMessage = "Email invalide" });
            }

            // On vérifie si le client existe
            Client client = _clientSrv.GetClientByEmail(email);

            if (client == null)
            {
                return View("ErrorVerifyEmail", new SignInViewModel { ErrorMessage = "Client introuvable" });
            }

            // On vérifie si le token est le bon
            if (client.VerificationToken != token)
            {
                return View("ErrorVerifyEmail", new SignInViewModel { ErrorMessage = "Token invalide" });
            }

            // On vérifie si le token est expiré
            if (client.TokenExpiryDate < DateTime.Now)
            {
                return View("ErrorVerifyEmail", new SignInViewModel { ErrorMessage = "Token expiré" });
            }

            // On vérifie si l'email est déjà vérifié
            if (client.IsEmailVerified.Value)
            {
                return View("ErrorVerifyEmail", new SignInViewModel { ErrorMessage = "Email déjà vérifié" });
            }

            // On valide l'email
            client.IsEmailVerified = true;
            client.VerificationToken = null;
            _clientSrv.UpdateClient(client);

            return View("VerifyEmail", client);
        }

        [Route("errorverifyemail")]
        [HttpGet]
        public IActionResult ErrorVerifyEmail(SignInViewModel model)
        {
            return View(model.ErrorMessage);
        }
        #endregion

        #region LogIn
        [Route("login")]
        [HttpGet]
        public IActionResult LogIn(LogInViewModel logInViewModel = null)
        {
            // On vérifie si l'utilisateur est déjà connecté
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Account");
            }

            return View(logInViewModel);
        }

        [Route("login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogInPost(string email, string password)
        {
            LogInViewModel logInViewModel = new LogInViewModel
            {
                Email = email,
                MotDePasse = password
            };

            if (email == null || password == null || !IsEmailValid(email) || !IsPasswordValid(password))
            {
                if (string.IsNullOrEmpty(email))
                {
                    ViewData["ErrorEmail"] = "L'email est obligatoire.";
                }
                else if (!IsEmailValid(email))
                {
                    ViewData["ErrorEmail"] = WhichPartIsMissingEmail(email);
                }

                if (string.IsNullOrEmpty(password))
                {
                    ViewData["ErrorPassword"] = "Le mot de passe est obligatoire.";
                }
                else if (!IsPasswordValid(password))
                {
                    ViewData["ErrorPassword"] = "Le mot de passe doit contenir au moins 8 caractères.";
                }

                return View("LogIn", logInViewModel);
            }

            // On hashe le password
            string hashedPassword = HashPassword(password).ToUpper();

            // On récupère le client
            Client client = _clientSrv.GetClient(email, hashedPassword);

            if (client == null)
            {
                Admin admin = _adminSrv.GetAdmin(email, hashedPassword);
                if (admin != null)
                {
                    HttpContext.Session.SetString("User", JsonSerializer.Serialize(admin));
                    return RedirectToAction("AuthAdmin", "Account");
                }
                logInViewModel.ErrorMessage = "L'email ou le mot de passe est incorrect";
                return View("LogIn", logInViewModel);
            }

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, client.Nom),
                    new Claim(ClaimTypes.Email, client.Email),
                    new Claim(ClaimTypes.Role, "Client")
                };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true // Pour rendre l'authentification persistante
            };

            // SignInAsync va créer le cookie d'authentification
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            var returnUrl = HttpContext.Session.GetString("ReturnUrl");

            if (returnUrl != null && !returnUrl.Equals(HttpContext.Request.Path))
                return Redirect(returnUrl);
            else
                return View("Index", client);


        }
        #endregion

        #region Admin
        [Route("admin")]
        [HttpGet]
        public async Task<IActionResult> AuthAdmin()
        {
            Admin admin = new Admin();
            var userJson = HttpContext.Session.GetString("User");
            if (!string.IsNullOrEmpty(userJson))
            {
                admin = JsonSerializer.Deserialize<Admin>(userJson);
                // Traiter les données de l'utilisateur
            }

            Random random = new Random();
            int code = random.Next(100000, 1000000);

            admin.Code = code;
            admin.CodeExpirationDate = DateTime.Now.AddMinutes(5);

            _adminSrv.UpdateAdmin(admin);

            // On envoie le mail
            string subject = "Code de connexion";
            string body = $"Votre code de connexion est : {code}";
            _authenticationSrv.SendEmail(admin.Email, subject, body);

            return View(admin);
        }

        //[Authorize(Roles = "Admin")]
        [Route("verifyadmin")]
        [HttpPost]
        public async Task<IActionResult> VerifyCodeAdmin(string code, int id)
        {
            Admin adminBdd = _adminSrv.GetAdminById(id);

            if (!int.TryParse(code, out int result) || adminBdd.Code != result)
            {
                return View("LogIn", new LogInViewModel { ErrorMessage = "Le code est incorrect" });
            }

            if (adminBdd.CodeExpirationDate < DateTime.Now)
            {
                return View("LogIn", new LogInViewModel { ErrorMessage = "Le code a expiré" });
            }

            // Création des revendications pour l'utilisateur
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, adminBdd.Nom),
                    new Claim(ClaimTypes.Email, adminBdd.Email),
                    new Claim(ClaimTypes.Role, "Admin") // Rôle pour autorisation
                };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Authentification persistante
                AllowRefresh = true // Permet le renouvellement du cookie
            };

            // Création du cookie d'authentification
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                authProperties
            );

            // À ce stade, le HttpContext.User sera mis à jour dans les requêtes suivantes
            return RedirectToAction("Index", "Account");
        }

        #endregion

        public static string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static bool IsEmailValid(string email)
        {
            if (WhichPartIsMissingEmail(email) == null)
                return true;
            else
                return false;
        }

        public static string WhichPartIsMissingEmail(string email)
        {
            if (!email.Contains("@"))
            {
                return "L'email doit contenir un \"@\"";
            }
            else if (string.IsNullOrEmpty(email.Split("@")[1]))
            {
                return "Veuillez renseigner la partie après le \"@\".";
            }
            else if (!email.Split("@")[1].Contains("."))
            {
                return "La deuxième partie d'email doit contenir un \".\"";
            }
            if (string.IsNullOrEmpty(email.Split("@")[1].Split(".")[1]))
            {
                return "Veuillez renseigner la partie après le \".\" .";
            }
            return null;
        }

        public static bool IsPasswordValid(string password)
        {
            if (password.Length < 8)
            {
                return false;
            }
            return true;
        }

        public static bool IsNameValid(string name)
        {
            if (name == null)
            {
                return false;
            }
            return true;
        }

        public static bool IsFirstNameValid(string firstName)
        {
            if (firstName == null)
            {
                return false;
            }
            return true;
        }

        public static string GenerateSecureToken()
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            return token.Replace("+", "").Replace("/", "").Replace("=", ""); // Nettoyage pour l'URL
        }
    }
}
