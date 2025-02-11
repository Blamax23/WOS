using Microsoft.AspNetCore.Http;
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
using Microsoft.AspNetCore.Http.HttpResults;
using WOS.Back.Services;

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
        private readonly ICategorieSrv _categorieSrv;
        private readonly IMarqueSrv _marqueSrv;
        private readonly IModeLivraisonSrv _modeLivraisonSrv;
        private readonly IGlobalDataSrv _globalDataSrv;
        private readonly ICookiesSrv _cookiesSrv;
        private readonly IMailSrv _mailSrv;

        public AccountController(IClientSrv clientSrv, IAdminSrv adminSrv, IConfiguration configuration, IAuthenticationSrv authenticationSrv, IProduitSrv produitSrv, ICommandeSrv commandeSrv, ICategorieSrv categorieSrv, IMarqueSrv marqueSrv, IModeLivraisonSrv modeLivraisonSrv, IGlobalDataSrv globalDataSrv, ICookiesSrv cookiesSrv, IMailSrv mailSrv)
        {
            _clientSrv = clientSrv;
            _adminSrv = adminSrv;
            _configuration = configuration;
            _authenticationSrv = authenticationSrv;
            _produitSrv = produitSrv;
            _commandeSrv = commandeSrv;
            _categorieSrv = categorieSrv;
            _marqueSrv = marqueSrv;
            _modeLivraisonSrv = modeLivraisonSrv;
            _globalDataSrv = globalDataSrv;
            _cookiesSrv = cookiesSrv;
            _mailSrv = mailSrv;
        }

        #region Account
        [Route("")]
        [Authorize]
        public async Task<ActionResult> Index(string section = null)
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
                    accountViewModel.Produits = _globalDataSrv.Produits;
                    accountViewModel.Commandes = _globalDataSrv.Commandes;
                    accountViewModel.Marques = _globalDataSrv.Marques;
                    accountViewModel.Categories = _globalDataSrv.Categories;
                    accountViewModel.ModeLivraisons = _globalDataSrv.ModeLivraisons;
                    accountViewModel.StatutCommandes = _globalDataSrv.StatutsCommande;
                    accountViewModel.CodePromo = _globalDataSrv.CodePromos;
                }
                else
                {
                    Client clientView = _clientSrv.GetClientByEmail(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value.ToString());
                    accountViewModel.User = clientView;
                    accountViewModel.Commandes = _commandeSrv.GetCommandesByClientId(clientView.Id);
                }
            }

            // On modifie le cookie activeSection pour afficher la bonne section
            if(section != null)
                HttpContext.Response.Cookies.Append("activeSection", section);

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

        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> UpdateProfile(string nom, string prenom, string email, string password, string newPassword)
        {
            // On modifie le profil si les informations ont changé
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            Client client = _clientSrv.GetClientByEmail(userEmail);
            if (client == null)
            {
                Admin admin = _adminSrv.GetAdminByEmail(userEmail);
                // On effectue le contrôle avec les focntions statiques sur chaque champ s'il n'est pas null
                if (admin == null)
                {
                    return Json(new { errorMessage = "Nous n'avons trouvé aucun compte vous correspondant." });
                }
                if (!IsEmailValid(email))
                {
                    var result = WhichPartIsMissingEmail(email);
                    return Json(new { errorMessage = result }); ;
                }
                else
                {
                    if (email != admin.Email)
                        admin.Email = email;
                }

                if (!IsFirstNameValid(prenom))
                {
                    return Json(new { errorMessage = "Le prénom est obligatoire" });
                }
                else
                {
                    if (prenom != admin.Prenom)
                        admin.Prenom = prenom;
                }

                if (!IsNameValid(nom))
                {
                    return Json(new { errorMessage = "Le nom est obligatoire" });
                }
                else
                {
                    if (nom != admin.Nom)
                        admin.Nom = nom;
                }

                if (password != null)
                {
                    if (HashPassword(password) != admin.MotDePasse)
                    {
                        return Json(new { errorMessage = "Le mot de passe actuel est incorrect" });
                    }
                    else
                    {
                        if (newPassword != null)
                        {
                            if (!IsPasswordValid(newPassword))
                            {
                                return Json(new { errorMessage = "Le nouveau mot de passe doit contenir au moins 8 caractères" });
                            }
                            else
                            {
                                admin.AncienMotDePasse = admin.MotDePasse;
                                admin.MotDePasse = HashPassword(newPassword);
                            }
                        }

                        _adminSrv.UpdateAdmin(admin);
                    }
                }
            }
            else
            {
                if (client.Nom != nom)
                    client.Nom = nom;
                if (client.Prenom != prenom)
                    client.Prenom = prenom;
                if (client.Email != email)
                    client.Email = email;
                if (client.MotDePasse != HashPassword(password))
                {
                    client.AncienMotDePasse = client.MotDePasse;
                    client.MotDePasse = HashPassword(password);
                }
                _clientSrv.UpdateClient(client);
            }
            return Json(new { errorMessage = "" });
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
            _mailSrv.SendEmail(email, subject, body);

            _mailSrv.SendEmailVerification(client, token);

            // On stocke le token dans la base de données
            client.VerificationToken = token;
            client.TokenExpiryDate = DateTime.Now.AddHours(24);
            _clientSrv.UpdateClient(client);

            _authenticationSrv.LoginAccountClient(client);

            _mailSrv.SendEmailSuccessfulRegistration(client);

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
            if (token == null || token.Length != 44)
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
                    new Claim(ClaimTypes.Sid, client.Id.ToString()),
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

            _mailSrv.SendCodeAuthentication(admin, code);

            var returnUrl = HttpContext.Session.GetString("ReturnUrl");
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

        #region Cookies

        //[HttpPost]
        //[Route("SaveConsent")]
        //public ActionResult SaveConsent([FromBody] string consentGiven)
        //{
        //    if(consentGiven == null) return BadRequest();

        //    bool.TryParse(consentGiven, out bool result);

        //    string userId = Request.Cookies["userId"] ?? Guid.NewGuid().ToString();
        //    if (Request.Cookies["userId"] == null)
        //    {
        //        Response.Cookies.Append("userId", userId, new CookieOptions { Expires = DateTime.UtcNow.AddYears(1) });
        //    }

        //    var userIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        //    var userAgent = Request.Headers["User-Agent"].ToString();

        //    var consent = new UserCookies
        //    {
        //        UserId = userId,
        //        ConsentGiven = result,
        //        UserIp = userIp,
        //        UserAgent = userAgent
        //    };

        //    _cookiesSrv.SaveCookies(consent);

        //    return Ok();
        //}

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
                return builder.ToString().ToUpper();
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
            var length = token.Length;
            // On récupère une lettre au hasard
            var random = new Random();
            var letter1 = (char)random.Next(65, 90);
            var letter2 = (char)random.Next(65, 90);
            var letter3 = (char)random.Next(65, 90);

            return token.Replace('+', letter1).Replace('/', letter2).Replace('=', letter3); // Nettoyage pour l'URL
        }
    }
}
