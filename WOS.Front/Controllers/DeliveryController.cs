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
using Microsoft.AspNetCore.Http.HttpResults;
using WOS.Back.Services;

namespace WOS.Front.Controllers
{
    [Route("[controller]")]
    public class DeliveryController : Controller
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

        public DeliveryController(IClientSrv clientSrv, IAdminSrv adminSrv, IConfiguration configuration, IAuthenticationSrv authenticationSrv, IProduitSrv produitSrv, ICommandeSrv commandeSrv, ICategorieSrv categorieSrv, IMarqueSrv marqueSrv, IModeLivraisonSrv modeLivraisonSrv, IGlobalDataSrv globalDataSrv)
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
        }

        [HttpPost]
        [Route("webhook/ups")]
        public IActionResult GetWebhookUPS([FromBody] WebhookPayloadUPS payload)
        {
            // 2. Traitement des données reçues
            if (payload == null || payload.TrackingNumber == null)
            {
                return BadRequest("Invalid payload.");
            }

            Commande commande = _commandeSrv.GetCommandeByNumberOrderDelivery(payload.TrackingNumber);

            if (commande == null)
            {
                return BadRequest("Order not found.");
            }

            if(payload.ActivityStatus.Type == "D")
            {
                commande.StatutId = 5;
            }

            _commandeSrv.UpdateCommande(commande);

            // 3. Réponse à UPS
            return Ok(new { status = "Received" });
        }
    }
}
