using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WOS.Model;
using WOS.Dal.Interfaces;

namespace WOS.Back.Services
{
    public class MondialRelaySrv : IMondialRelaySrv
    {
        private const string SOAP_ENDPOINT = "https://api.mondialrelay.com/Web_Services.asmx";
        private const string SOAP_ACTION = "http://www.mondialrelay.fr/webservice/WSI2_RecherchePointRelais";

        // Votre identifiant enseigne Mondial Relay
        private const string ENSEIGNE = "BDTEST13";

        // Votre clé privée 
        private const string CLE_PRIVEE = "PrivateK";

        public async Task<List<PointRelais>> RechercherPointsRelais(
            string pays,
            string ville,
            string codePostal,
            string taille = "",
            string poids = "",
            string action = "")
        {
            // Génération de la signature
            string signature = GenererSignature(pays, ville, codePostal, taille, poids, action);

            // Construction du corps SOAP
            string soapBody = ConstruireSoapRequest(
                pays, ville, codePostal, taille, poids, action, signature);

            // Envoi de la requête
            using (var client = new HttpClient())
            {
                var content = new StringContent(soapBody, Encoding.UTF8, "text/xml");
                content.Headers.Add("SOAPAction", SOAP_ACTION);

                var response = await client.PostAsync(SOAP_ENDPOINT, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return ParseReponse(responseBody);
                }

                throw new Exception("Erreur lors de la requête SOAP");
            }
        }

        private string GenererSignature(
            string pays,
            string ville,
            string codePostal,
            string taille,
            string poids,
            string action)
        {
            // Concaténation des paramètres
            string chaineConcatenee =
                ENSEIGNE +
                pays +
                ville +
                codePostal +
                taille +
                poids +
                action +
                CLE_PRIVEE;

            // Calcul MD5
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(chaineConcatenee);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Conversion en chaîne hexadécimale en majuscules
                return BitConverter.ToString(hashBytes)
                    .Replace("-", "")
                    .ToUpper();
            }
        }

        private string ConstruireSoapRequest(
            string pays,
            string ville,
            string codePostal,
            string taille,
            string poids,
            string action,
            string signature)
        {
            return $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
               xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
               xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <WSI2_RecherchePointRelais xmlns=""http://www.mondialrelay.fr/webservice/"">
      <Enseigne>{ENSEIGNE}</Enseigne>
      <Pays>{pays}</Pays>
      <Ville>{ville}</Ville>
      <CP>{codePostal}</CP>
      <Taille>{taille}</Taille>
      <Poids>{poids}</Poids>
      <Action>{action}</Action>
      <Security>{signature}</Security>
    </WSI2_RecherchePointRelais>
  </soap:Body>
</soap:Envelope>";
        }

        private List<PointRelais> ParseReponse(string reponseXml)
        {
            var points = new List<PointRelais>();

            // Parsing XML de la réponse SOAP
            var xdoc = XDocument.Parse(reponseXml);

            // Utilisez le bon namespace et le bon chemin pour extraire les points relais
            var pointsElements = xdoc.Descendants(
                XName.Get("PointRelais", "http://www.mondialrelay.fr/webservice/")
            );

            foreach (var pointElement in pointsElements)
            {
                points.Add(new PointRelais
                {
                    Numero = pointElement.Element(XName.Get("Num", "http://www.mondialrelay.fr/webservice/"))?.Value,
                    Adresse = pointElement.Element(XName.Get("Adresse", "http://www.mondialrelay.fr/webservice/"))?.Value,
                    CodePostal = pointElement.Element(XName.Get("CP", "http://www.mondialrelay.fr/webservice/"))?.Value,
                    Ville = pointElement.Element(XName.Get("Ville", "http://www.mondialrelay.fr/webservice/"))?.Value,
                    Latitude = pointElement.Element(XName.Get("Latitude", "http://www.mondialrelay.fr/webservice/"))?.Value,
                    Longitude = pointElement.Element(XName.Get("Longitude", "http://www.mondialrelay.fr/webservice/"))?.Value
                });
            }

            return points;
        }

        // Exemple d'utilisation
        public async Task ExempleRecherche()
        {
            try
            {
                var pointsRelais = await RechercherPointsRelais(
                    pays: "FR",
                    ville: "Paris",
                    codePostal: "75001"
                );

                foreach (var point in pointsRelais)
                {
                    Console.WriteLine($"Point Relais {point.Numero} : " +
                        $"{point.Adresse}, {point.CodePostal} {point.Ville}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur : {ex.Message}");
            }
        }
    }
}
