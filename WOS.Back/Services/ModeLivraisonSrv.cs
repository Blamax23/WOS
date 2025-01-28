using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOS.Dal.Context;
using WOS.Dal.Interfaces;
using WOS.Model;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Http;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Metadata;
using System.Drawing;
using System.Drawing.Imaging;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using Newtonsoft.Json.Linq;

namespace WOS.Back.Services
{
    public class ModeLivraisonSrv : IModeLivraisonSrv
    {
        private readonly WOSDbContext _context;
        private readonly IGlobalDataSrv _globalDataSrv;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ModeLivraisonSrv(WOSDbContext context, IGlobalDataSrv globalDataSrv, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _globalDataSrv = globalDataSrv;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public ModeLivraison GetModeLivraisonById(int id)
        {
            return _globalDataSrv.ModeLivraisons.FirstOrDefault(ml => ml.Id == id);
        }

        public ModeLivraison GetModeLivraisonByName(string name)
        {
            return _globalDataSrv.ModeLivraisons.FirstOrDefault(ml => ml.Nom == name);
        }

        public void UpdatePriceLivraison(int id, float newPrice, int deliveryTime)
        {
            ModeLivraison modeLivraison = _context.ModeLivraisons.FirstOrDefault(ml => ml.Id == id);

            modeLivraison.PrixLivraison = newPrice;
            modeLivraison.JoursLivraisonMini = deliveryTime;

            _context.SaveChanges();
            _globalDataSrv.RefreshCacheAsync(typeof(ModeLivraison));
        }

        public async Task<string> GetAuthUPS()
        {
            // On récupère le client secret et le client id depuis le fichier de configuration
            var clientId = _configuration.GetSection("DeliveryCredentials:UPS:ClientId").Value;
            var clientSecret = _configuration.GetSection("DeliveryCredentials:UPS:ClientSecret").Value;

            // Encode ClientID:ClientSecret en Base64
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

            using (var client = new HttpClient())
            {
                // Ajouter le header Authorization
                client.DefaultRequestHeaders.Add("Authorization", $"Basic {credentials}");

                // Créer le contenu avec le bon Content-Type
                var content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

                // Appeler l'endpoint OAuth
                var response = await client.PostAsync("https://wwwcie.ups.com/security/v1/oauth/token", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // On déserialize la réponse pour récupérer le token
                    var json = JsonDocument.Parse(responseContent).RootElement;
                    var token = json.GetProperty("access_token").GetString();

                    return token;
                }
                else
                {
                    Console.WriteLine("Erreur:");
                    return responseContent;
                }
            }
        }

        public async Task SetWebhookUPS(string trackingNumber, string destinationUrl, string token)
        {
            try
            {
                // On récupère le domain dans le fichier de configuration
                var domain = _configuration.GetSection("DeliveryCredentials:UPS:Domain").Value;
                var version = _configuration.GetSection("DeliveryCredentials:UPS:Version").Value;

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("transId", Guid.NewGuid().ToString());
                client.DefaultRequestHeaders.Add("transactionSrc", "myApp");
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                JObject json = JObject.Parse(@"{
                    locale: 'en_US',
                    countryCode: 'US',
                    trackingNumberList: [],
                    eventPreference: ['string'],
                    destination: {
                      url: '" + destinationUrl + @"',
                      credentialType: 'Bearer',
                      credential: '" + token + @"'
                    }
                }");

                // Ajout dynamique du numéro de suivi
                json["trackingNumberList"] = new JArray(trackingNumber);

                var postData = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                var request = await client.PostAsync($"https://wwwcie.ups.com/api/track/{version}/subscription/standard/package", postData);
                var response = await request.Content.ReadAsStringAsync();

                Console.WriteLine();
            }
            catch (Exception ex)
            {
                   throw new Exception("Erreur lors de la configuration du webhook UPS", ex);
            }
            

            
        }

        public async Task<(byte[], string)> GetEtiquetteFromUPS(string token = "")
        {
            var shipmentRequest = new
            {
                ShipmentRequest = new
                {
                    Request = new
                    {
                        SubVersion = "1801",
                        RequestOption = "validate"
                    },
                    Shipment = new
                    {
                        Description = "Ship WS test",
                        Shipper = new
                        {
                            Name = "Maxime Blanc",
                            TaxIdentificationNumber = "123456",
                            Phone = new
                            {
                                Number = "0647526322",
                                Extension = "33"
                            },
                            ShipperNumber = "V702J7",
                            Address = new
                            {
                                AddressLine = new[] { "166 Route de Thoissey" },
                                City = "Châtillon sur Chalaronne",
                                PostalCode = "01400",
                                CountryCode = "FR"
                            }
                        },
                        ShipTo = new
                        {
                            Name = "Imrane Abdoul",
                            Phone = new
                            {
                                Number = "0645236897"
                            },
                            Address = new
                            {
                                AddressLine = new[] { "44 Rue du Sergent Michel Berthet" },
                                City = "Lyon",
                                PostalCode = "69009",
                                CountryCode = "FR"
                            },
                            Residential = "true"
                        },
                        PaymentInformation = new
                        {
                            ShipmentCharge = new[]
                {
                    new
                    {
                        Type = "01",
                        BillShipper = new
                        {
                            AccountNumber = "V702J7"
                        }
                    }
                }
                        },
                        Service = new
                        {
                            Code = "11"
                        },
                        Package = new[]
            {
                new
                {
                    Packaging = new
                    {
                        Code = "02",
                        Description = "Nails"
                    },
                    Dimensions = new
                    {
                        UnitOfMeasurement = new
                        {
                            Code = "CM",
                            Description = "Centimeters"
                        },
                        Length = "10",
                        Width = "30",
                        Height = "45"
                    },
                    PackageWeight = new
                    {
                        UnitOfMeasurement = new
                        {
                            Code = "KGS",
                            Description = "Kilogramms"
                        },
                        Weight = "5"
                    }
                }
            }
                    },
                    LabelSpecification = new
                    {
                        LabelImageFormat = new
                        {
                            Code = "GIF",
                            Description = "GIF"
                        },
                        HTTPUserAgent = "Mozilla/4.5"
                    }
                }
            };
            try
            {

                // Sérialisation avec System.Text.Json
                var json = JsonSerializer.Serialize(shipmentRequest);

                // Ensuite tu peux l'utiliser comme suit pour faire ta requête HTTP, par exemple :
                var postData = new StringContent(json, Encoding.UTF8, "application/json");

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("transactionSrc", "testing");
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var Version = "v2409"; // Remplace par la version appropriée
                var request = await client.PostAsync("https://wwwcie.ups.com/api/shipments/" + Version + "/ship", postData);
                var response = await request.Content.ReadAsStringAsync();
                byte[] base64Image;
                string trackingNumber;

                // On déserialize la réponse pour récupérer l'image
                using (JsonDocument doc = JsonDocument.Parse(response))
                {
                    // Accès au tableau PackageResults
                    var packageResults = doc.RootElement
                        .GetProperty("ShipmentResponse")
                        .GetProperty("ShipmentResults")
                        .GetProperty("PackageResults");

                    var firstPackage = packageResults[0];

                    // Extraire le champ "GraphicImage"
                    base64Image = Convert.FromBase64String(firstPackage
                                          .GetProperty("ShippingLabel")
                                          .GetProperty("GraphicImage")
                                          .GetString());

                    // Extraire le champ "TrackingNumber"
                    trackingNumber = firstPackage
                                          .GetProperty("TrackingNumber")
                                          .GetString();
                }

                return (base64Image, trackingNumber);
            }
            catch (Exception e)
            {
                throw new Exception("Erreur lors de la récupération de l'étiquette UPS", e);
            }

        }

        public async Task<string> GetEtiquetteFromMondialRelay()
        {
            string soapRequest = @"<?xml version=""1.0"" encoding=""utf-8""?>
        <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
          <soap:Body>
            <WSI2_CreationEtiquette xmlns=""http://www.mondialrelay.fr/webservice/"">
              <Enseigne>VOTRE_ENSEIGNE</Enseigne>
              <ModeCol>REL</ModeCol>
              <ModeLiv>24R</ModeLiv>
              <NDossier></NDossier>
              <NClient></NClient>
              <Expe_Langage>FR</Expe_Langage>
              <Expe_Ad1>Adresse expéditeur</Expe_Ad1>
              <Expe_Ad2>Complément</Expe_Ad2>
              <Expe_Ad3></Expe_Ad3>
              <Expe_Ad4></Expe_Ad4>
              <Expe_Ville>Paris</Expe_Ville>
              <Expe_CP>75001</Expe_CP>
              <Expe_Pays>FR</Expe_Pays>
              <Expe_Tel1>0123456789</Expe_Tel1>
              <Expe_Tel2></Expe_Tel2>
              <Expe_Mail>expediteur@email.com</Expe_Mail>
              <Dest_Langage>FR</Dest_Langage>
              <Dest_Ad1>Adresse destinataire</Dest_Ad1>
              <Dest_Ad2>Complément</Dest_Ad2>
              <Dest_Ad3></Dest_Ad3>
              <Dest_Ad4></Dest_Ad4>
              <Dest_Ville>Lyon</Dest_Ville>
              <Dest_CP>69001</Dest_CP>
              <Dest_Pays>FR</Dest_Pays>
              <Dest_Tel1>0987654321</Dest_Tel1>
              <Dest_Tel2></Dest_Tel2>
              <Dest_Mail>destinataire@email.com</Dest_Mail>
              <Poids>2000</Poids>
              <Longueur></Longueur>
              <Taille></Taille>
              <NbColis>1</NbColis>
              <CRT_Valeur>0</CRT_Valeur>
              <CRT_Devise></CRT_Devise>
              <Exp_Valeur>0</Exp_Valeur>
              <Exp_Devise></Exp_Devise>
              <COL_Rel_Pays>FR</COL_Rel_Pays>
              <COL_Rel>123456</COL_Rel>
              <LIV_Rel_Pays>FR</LIV_Rel_Pays>
              <LIV_Rel>654321</LIV_Rel>
              <TAvisage>0</TAvisage>
              <TReprise>0</TReprise>
              <Montage>0</Montage>
              <TRDV>0</TRDV>
              <Assurance>0</Assurance>
              <Instructions></Instructions>
              <Security>VOTRE_CLE_SECURITE</Security>
              <Texte></Texte>
            </WSI2_CreationEtiquette>
          </soap:Body>
        </soap:Envelope>";

            string url = "https://www.mondialrelay.fr/webservice/WebService.asmx";
            using (HttpClient client = new HttpClient())
            {
                var httpContent = new StringContent(soapRequest, Encoding.UTF8, "text/xml");

                // Ajouter le header SOAPAction
                httpContent.Headers.Add("SOAPAction", "http://www.mondialrelay.fr/webservice/WSI2_CreationEtiquette");

                try
                {
                    // Envoyer la requête
                    HttpResponseMessage response = await client.PostAsync(url, httpContent);
                    string responseContent = await response.Content.ReadAsStringAsync();

                    // Afficher la réponse
                    return responseContent;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erreur lors de la récupération de l'étiquette Mondial Relay", ex);
                }
            }
        }

        private string GenerateCodeVerifier()
        {
            return Guid.NewGuid().ToString();
        }

        private string GenerateCodeChallenge(string codeVerifier)
        {
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                return Convert.ToBase64String(hash)
                    .Replace("=", "") // Retirer les padding '='
                    .Replace("+", "-") // Convertir en URL-safe
                    .Replace("/", "_"); // Convertir en URL-safe
            }
        }

        public static bool IsValidGifBase64(string base64Image)
        {
            try
            {
                // Décoder le base64 en bytes
                byte[] imageBytes = Convert.FromBase64String(base64Image);

                // Vérifier l'en-tête GIF (GIF87a ou GIF89a)
                string header = Encoding.ASCII.GetString(imageBytes, 0, 6); // Lire les 6 premiers octets

                return header == "GIF87a" || header == "GIF89a";
            }
            catch (FormatException)
            {
                // Si le base64 est invalide
                return false;
            }
            catch (Exception)
            {
                // Gestion des autres erreurs
                return false;
            }
        }
    }
}
