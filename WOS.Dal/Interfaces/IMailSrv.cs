using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WOS.Model;

namespace WOS.Dal.Interfaces
{
    public interface IMailSrv
    {

        public void SendEmail(string email, string subject, string body);

        public void SendEmailPurchasedConfirmed(Commande commande);

        void SendEmailSuccessfulRegistration(Client client);

        void SendEmailVerification(Client client, string token);

        void SendCodeAuthentication(Admin admin, int code);

        void SendDeliveryConfirmation(Commande commande);
    }
}
