using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using WOS.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using WOS.Dal.Interfaces;
using System.Net.Mime;
using System.Security.Policy;

namespace WOS.Front.Services
{
    public class MailSrv : IMailSrv
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private MailConfiguration _mailConfiguration;
        private readonly IGlobalDataSrv _globalDataSrv;

        public MailSrv(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IGlobalDataSrv globalDataSrv)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _mailConfiguration = new MailConfiguration()
            {
                SenderMail = _configuration.GetSection("EmailSettings")["EmailSender"],
                SenderPassword = _configuration.GetSection("EmailSettings")["Password"],
                ReceiverMail = _configuration.GetSection("EmailSettings")["EmailReceiver"],
                SmtpServer = _configuration.GetSection("EmailSettings")["Host"]
            };
            _globalDataSrv = globalDataSrv;
        }
        public void SendEmail(string email, string subject, string body)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(_mailConfiguration.SenderMail);
            mail.To.Add(new MailAddress(email));
            mail.Subject = subject;
            mail.Body = $"<html><body>{body}</body></html>";
            mail.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient(_mailConfiguration.SmtpServer)
            {
                Port = 587,
                Credentials = new NetworkCredential(_mailConfiguration.SenderMail, _mailConfiguration.SenderPassword),
                EnableSsl = true
            };

            smtp.Send(mail);
        }

        public void SendEmailPurchasedConfirmed(Commande commande)
        {
            // On récupère le client
            //Client client = _globalDataSrv.Clients.FirstOrDefault(c => c.Id == commande.ClientId);
            Admin client = _globalDataSrv.Admins.FirstOrDefault(c => c.Id == commande.ClientId);

            using (MemoryStream ms = new MemoryStream(commande.BinaryFacture))
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(_mailConfiguration.SenderMail);
                mail.To.Add(new MailAddress(client.Email));
                mail.Subject = $"Confirmation de votre commande n°{commande.NumeroCommande}";
                mail.Body = $"<table role=\"presentation\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\" style=\"background-color:#f4f4f4;\">\r\n        <tr>\r\n            <td align=\"center\">\r\n                <table role=\"presentation\" width=\"600\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\" style=\"background-color:#ffffff; padding:20px; font-family:Arial, sans-serif;\">\r\n\r\n                    <!-- HEADER -->\r\n                    <tr>\r\n                        <td align=\"center\" style=\"padding-bottom:20px;\">\r\n                            <img src=\"https://wossneakers.fr/src/WosLogos/logoWosBlack.png\" width=\"200\" alt=\"Logo\">\r\n                        </td>\r\n                    </tr>\r\n\r\n                    <!-- TITRE -->\r\n                    <tr>\r\n                        <td align=\"center\" style=\"font-size:24px; font-weight:bold; padding-bottom:50px;\">\r\n                            Merci pour votre commande !\r\n                        </td>\r\n                    </tr>\r\n\r\n                    <!-- TEXTE -->\r\n                    <tr>\r\n                        <td align=\"left\" style=\"font-size:16px; line-height:24px; color:#333;\">\r\n                            Suite à l'achat effectué sur notre site, le {commande.DateCommande.Date} à {commande.DateCommande.Hour}h{commande.DateCommande.Minute}, vous trouverez ci-joint la facture de votre commande n°{commande.NumeroCommande}.<br /><br />\r\n                            Cliquez sur ce lien si vous souhaitez afficher les détails de votre commande.\r\n                        </td>\r\n                    </tr>\r\n\r\n                    <!-- BOUTON -->\r\n                    <tr>\r\n                        <td align=\"center\" style=\"padding-top:40px;\">\r\n                            <a href=\"https://wossneakers.fr/Account?section=info-commandes\" style=\"background-color:black; color:#ffffff; text-decoration:none; padding:10px 20px; font-size:16px; border-radius:5px;\">\r\n                                Voir ma commande\r\n                            </a>\r\n                        </td>\r\n                    </tr>\r\n\r\n                    <!-- FOOTER -->\r\n                    <tr>\r\n                        <td align=\"center\" style=\"font-size:12px; color:#777; padding-top:20px;\">\r\n                            <strong>L'équipe WOS Sneakers.</strong> <br />\r\n                            <a href=\"https://wossneakers.fr\">WOS Sneakers</a> -\r\n                            <a href=\"https://wossneakers.fr/Account\">Mon compte</a>\r\n                            Mentions légales.\r\n                        </td>\r\n                    </tr>\r\n\r\n                </table>\r\n            </td>\r\n        </tr>\r\n    </table>";
                mail.IsBodyHtml = true;

                // Création de la pièce jointe
                string pdfFileName = $"Facture n°{commande.NumeroCommande}.pdf";
                Attachment attachment = new Attachment(ms, pdfFileName, MediaTypeNames.Application.Pdf);
                mail.Attachments.Add(attachment);

                // Configuration du SMTP
                SmtpClient smtp = new SmtpClient(_mailConfiguration.SmtpServer)
                {
                    Port = 587,
                    Credentials = new NetworkCredential(_mailConfiguration.SenderMail, _mailConfiguration.SenderPassword),
                    EnableSsl = true
                };

                smtp.Send(mail);
            }
        }

        public void SendEmailSuccessfulRegistration(Client client)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(_mailConfiguration.SenderMail);
            mail.To.Add(new MailAddress(client.Email));
            mail.Subject = $"✅ Votre inscription est confirmée !";
            mail.Body = $"<table role=\"presentation\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\" style=\"background-color:#f4f4f4;\">\r\n        <tr>\r\n            <td align=\"center\">\r\n                <table role=\"presentation\" width=\"600\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\" style=\"background-color:#ffffff; padding:20px; font-family:Arial, sans-serif;\">\r\n\r\n                    <!-- HEADER -->\r\n                    <tr>\r\n                        <td align=\"center\" style=\"padding-bottom:20px;\">\r\n                            <img src=\"https://wossneakers.fr/src/WosLogos/logoWosBlack.png\" width=\"200\" alt=\"Logo\">\r\n                        </td>\r\n                    </tr>\r\n\r\n                    <!-- TITRE -->\r\n                    <tr>\r\n                        <td align=\"center\" style=\"font-size:24px; font-weight:bold; padding-bottom:50px;\">\r\n                            ✅ Votre inscription est confirmée !\r\n                        </td>\r\n                    </tr>\r\n\r\n                    <!-- TEXTE -->\r\n                    <tr>\r\n                        <td align=\"left\" style=\"font-size:16px; line-height:24px; color:#333;\">\r\n                            Bonjour {client.Prenom}, <br /><br />\r\n                            <strong>Bienvenue chez WOS Sneakers !</strong> Votre inscription a bien été prise en compte. Nous sommes ravis de vous compter parmi nous. Découvrez nos produits dès à présen.\r\n                        </td>\r\n                    </tr>\r\n\r\n                    <!-- BOUTON -->\r\n                    <tr>\r\n                        <td align=\"center\" style=\"padding-top:40px;\">\r\n                            <a href=\"https://wossneakers.fr/Product\" style=\"background-color:black; color:#ffffff; text-decoration:none; padding:10px 20px; font-size:16px; border-radius:5px;\">\r\n                                Catalogue\r\n                            </a>\r\n                        </td>\r\n                    </tr>\r\n\r\n                    <!-- FOOTER -->\r\n                    <tr>\r\n                        <td align=\"center\" style=\"font-size:12px; color:#777; padding-top:25px;\">\r\n                            @* Vous recevez cet email parce que vous êtes abonné. <br>\r\n                            <a href=\"#\" style=\"color:#007bff; text-decoration:none;\">Se désinscrire</a> *@\r\n                            <strong>L'équipe WOS Sneakers.</strong> <br />\r\n                            <a href=\"https://wossneakers.fr\">WOS Sneakers</a> -\r\n                            <a href=\"https://wossneakers.fr/Account\">Mon compte</a><br />\r\n                            Mentions légales.\r\n                        </td>\r\n                    </tr>\r\n\r\n                </table>\r\n            </td>\r\n        </tr>\r\n    </table>";
            mail.IsBodyHtml = true;

            // Configuration du SMTP
            SmtpClient smtp = new SmtpClient(_mailConfiguration.SmtpServer)
            {
                Port = 587,
                Credentials = new NetworkCredential(_mailConfiguration.SenderMail, _mailConfiguration.SenderPassword),
                EnableSsl = true
            };

            smtp.Send(mail);
        }

        public void SendEmailVerification(Client client, string token)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(_mailConfiguration.SenderMail);
            mail.To.Add(new MailAddress(client.Email));
            mail.Subject = $"Veuillez confirmer votre adresse email !";
            mail.Body = $"<table role=\"presentation\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\" style=\"background-color:#f4f4f4;\">\r\n        <tr>\r\n            <td align=\"center\">\r\n                <table role=\"presentation\" width=\"600\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\" style=\"background-color:#ffffff; padding:20px; font-family:Arial, sans-serif;\">\r\n\r\n                    <!-- HEADER -->\r\n                    <tr>\r\n                        <td align=\"center\" style=\"padding-bottom:20px;\">\r\n                            <img src=\"https://wossneakers.fr/src/WosLogos/logoWosBlack.png\" width=\"200\" alt=\"Logo\">\r\n                        </td>\r\n                    </tr>\r\n\r\n                    <!-- TITRE -->\r\n                    <tr>\r\n                        <td align=\"center\" style=\"font-size:24px; font-weight:bold; padding-bottom:50px;\">\r\n                            Il ne vous reste plus qu'une étape !\r\n                        </td>\r\n                    </tr>\r\n\r\n                    <!-- TEXTE -->\r\n                    <tr>\r\n                        <td align=\"left\" style=\"font-size:16px; line-height:24px; color:#333;\">\r\n                            Pour compléter la création de votre profil, veuillez confirmer votre adresse mail.<br /><br />\r\n                            Si ce message ne vous est pas adressé, merci de l'ignorer.\r\n                        </td>\r\n                    </tr>\r\n\r\n                    <!-- BOUTON -->\r\n                    <tr>\r\n                        <td align=\"center\" style=\"padding-top:40px;\">\r\n                            <a href=\"https://wossneakers.fr/account/verifyemail?email={client.Email}&token={token}\" style=\"background-color:black; color:#ffffff; text-decoration:none; padding:10px 20px; font-size:16px; border-radius:5px;\">\r\n                                Vérifier\r\n                            </a>\r\n                        </td>\r\n                    </tr>\r\n\r\n                    <!-- FOOTER -->\r\n                    <tr>\r\n                        <td align=\"center\" style=\"font-size:12px; color:#777; padding-top:25px;\">\r\n                            @* Vous recevez cet email parce que vous êtes abonné. <br>\r\n                            <a href=\"#\" style=\"color:#007bff; text-decoration:none;\">Se désinscrire</a> *@\r\n                            <strong>L'équipe WOS Sneakers.</strong> <br />\r\n                            <a href=\"https://wossneakers.fr\">WOS Sneakers</a> -\r\n                            <a href=\"https://wossneakers.fr/Account\">Mon compte</a><br />\r\n                            Mentions légales.\r\n                        </td>\r\n                    </tr>\r\n\r\n                </table>\r\n            </td>\r\n        </tr>\r\n    </table>";
            mail.IsBodyHtml = true;

            // Configuration du SMTP
            SmtpClient smtp = new SmtpClient(_mailConfiguration.SmtpServer)
            {
                Port = 587,
                Credentials = new NetworkCredential(_mailConfiguration.SenderMail, _mailConfiguration.SenderPassword),
                EnableSsl = true
            };

            smtp.Send(mail);
        }

        public void SendCodeAuthentication(Admin admin, int code)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(_mailConfiguration.SenderMail);
            mail.To.Add(new MailAddress(admin.Email));
            mail.Subject = $"Code de vérification : {code}";
            mail.Body = $"<table role=\"presentation\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\" style=\"background-color:#f4f4f4;\">\r\n        <tr>\r\n            <td align=\"center\">\r\n                <table role=\"presentation\" width=\"600\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\" style=\"background-color:#ffffff; padding:20px; font-family:Arial, sans-serif;\">\r\n\r\n                    <!-- HEADER -->\r\n                    <tr>\r\n                        <td align=\"center\" style=\"padding-bottom:20px;\">\r\n                            <img src=\"https://wossneakers.fr/src/WosLogos/logoWosBlack.png\" width=\"200\" alt=\"Logo\">\r\n                        </td>\r\n                    </tr>\r\n\r\n                    <!-- TITRE -->\r\n                    <tr>\r\n                        <td align=\"center\" style=\"font-size:24px; font-weight:bold; padding-bottom:50px;\">\r\n                            Code de vérification\r\n                        </td>\r\n                    </tr>\r\n\r\n                    <!-- TEXTE -->\r\n                    <tr>\r\n                        <td align=\"left\" style=\"font-size:16px; line-height:24px; color:#333;\">\r\n                            Voici votre code de vérification pour accéder à WOS Sneakers : <strong>{code}</strong>\r\n                        </td>\r\n                    </tr>\r\n\r\n                    <!-- FOOTER -->\r\n                    <tr>\r\n                        <td align=\"center\" style=\"font-size:12px; color:#777; padding-top:25px;\">\r\n                            @* Vous recevez cet email parce que vous êtes abonné. <br>\r\n                            <a href=\"#\" style=\"color:#007bff; text-decoration:none;\">Se désinscrire</a> *@\r\n                            <strong>L'équipe WOS Sneakers.</strong> <br />\r\n                            <a href=\"https://wossneakers.fr\">WOS Sneakers</a> -\r\n                            <a href=\"https://wossneakers.fr/Account\">Mon compte</a><br />\r\n                            Mentions légales.\r\n                        </td>\r\n                    </tr>\r\n\r\n                </table>\r\n            </td>\r\n        </tr>\r\n    </table>";
            mail.IsBodyHtml = true;

            // Configuration du SMTP
            SmtpClient smtp = new SmtpClient(_mailConfiguration.SmtpServer)
            {
                Port = 587,
                Credentials = new NetworkCredential(_mailConfiguration.SenderMail, _mailConfiguration.SenderPassword),
                EnableSsl = true
            };

            smtp.Send(mail);
        }

        public void SendDeliveryConfirmation(Commande commande)
        {
            // On récupère le client
            Client client = _globalDataSrv.Clients.FirstOrDefault(c => c.Id == commande.ClientId);

            var listProduits = "";
            foreach (LigneCommande ligneCommande in commande.LignesCommande)
            {
                Produit produit = _globalDataSrv.Produits.FirstOrDefault(p => p.Id == ligneCommande.ProduitId);
                string absolutePath = "wwwroot" + produit.ProduitImages.First().Url.TrimStart('~', '\\').Replace("\\", "/");
                
                byte[] imageBytes = File.ReadAllBytes(absolutePath);
                string base64String = Convert.ToBase64String(imageBytes);
                string imageSrc = $"data:image/png;base64,{base64String}";
                listProduits += $"<div style=\"display: flex; flex-direction: row;\">\r\n    <img src=\"{imageSrc}\" style=\"width: 30%; border-radius: 15px;\" alt=\"{produit.Nom}\" />\r\n    <div style=\"margin: 0 1vw;\">\r\n        <h3 style=\"margin: 0 0 .5em 0;\">{produit.Nom}</h3>\r\n        <div style=\"display: flex; flex-direction: row;\">\r\n            <div style=\"display: flex; flex-direction: column; margin-right: 25px;\">\r\n                <span><strong>Taille :</strong></span>\r\n                <span style=\"margin: 0;\">{produit.ProduitTailles.FirstOrDefault(pt => pt.Id == ligneCommande.ProduitTailleId).Taille}</span>\r\n            </div>\r\n            <div style=\"display: flex; flex-direction: column;\">\r\n                <span><strong>Quantité :</strong></span>\r\n                <span>{ligneCommande.Quantite}</span>\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>";
            }

            // On récupère l'adresse de la livraison
            Adresse adresse = _globalDataSrv.Adresses.FirstOrDefault(a => a.Id == commande.AdresseLivraisonId);

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(_mailConfiguration.SenderMail);
            mail.To.Add(new MailAddress(client.Email));
            mail.Subject = $"Votre commande est arrivée !";
            mail.Body = $"<table role=\"presentation\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\" style=\"background-color:#f4f4f4;\">\r\n        <tr>\r\n            <td align=\"center\">\r\n                <table role=\"presentation\" width=\"600\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\" style=\"background-color:#ffffff; padding:20px; font-family:Arial, sans-serif;\">\r\n\r\n                    <!-- HEADER -->\r\n                    <tr>\r\n                        <td align=\"center\" style=\"padding-bottom:20px;\">\r\n                            <img src=\"https://wossneakers.fr/src/WosLogos/logoWosBlack.png\" width=\"200\" alt=\"Logo\">\r\n                        </td>\r\n                    </tr>\r\n\r\n                    <!-- TITRE -->\r\n                    <tr>\r\n                        <td align=\"center\" style=\"font-size:24px; font-weight:bold; padding-bottom:50px;\">\r\n                            Votre commande a été livrée\r\n                        </td>\r\n                    </tr>\r\n\r\n                    <!-- TEXTE -->\r\n                    <tr>\r\n                        <td align=\"left\" style=\"font-size:16px; line-height:24px; color:#333;\">\r\n                            Bonjour {client.Prenom},<br>\r\n                            <br>\r\n                            Bonne nouvelle ! Nous vous informons que votre commande a été livrée à l'adresse suivante : <br>\r\n                            <br>\r\n                            <strong style=\"display: block; text-align: center;\">{adresse.Rue + " " + adresse.CodePostal + " " + adresse.Ville}</strong><br>\r\n                            <div style=\"display: flex; flex-direction: column; border: 3px solid black; border-radius: 15px; padding: .5em; margin-bottom: 2vh;\">\r\n                                {listProduits}\r\n                            </div>\r\n                            Si le service vous a plus, n'hésitez pas à nous laisser votre avis !\r\n                        </td>\r\n\r\n                    <!-- BOUTON -->\r\n\r\n                    <tr>\r\n                        <td align=\"center\" style=\"padding-top:50px;\">\r\n                            <a href=\"https://wossneakers.fr/Account\" style=\"background-color:black; color:#ffffff; padding:10px 20px; text-decoration:none; border-radius:5px; font-size:16px;\">Mettre un avis</a>\r\n                        </td>\r\n                    </tr>\r\n\r\n                    <!-- FOOTER -->\r\n                    <tr>\r\n                        <td align=\"center\" style=\"font-size:12px; color:#777; padding-top:25px;\">\r\n                            <strong>L'équipe WOS Sneakers.</strong> <br />\r\n                            <a href=\"https://wossneakers.fr\">WOS Sneakers</a> -\r\n                            <a href=\"https://wossneakers.fr/Account\">Mon compte</a><br />\r\n                            Mentions légales.\r\n                        </td>\r\n                    </tr>\r\n\r\n                </table>\r\n            </td>\r\n        </tr>\r\n    </table>";
            mail.IsBodyHtml = true;

            // Configuration du SMTP
            SmtpClient smtp = new SmtpClient(_mailConfiguration.SmtpServer)
            {
                Port = 587,
                Credentials = new NetworkCredential(_mailConfiguration.SenderMail, _mailConfiguration.SenderPassword),
                EnableSsl = true
            };

            smtp.Send(mail);
        }
    }
}
