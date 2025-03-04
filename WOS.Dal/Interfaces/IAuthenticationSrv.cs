﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WOS.Model;

namespace WOS.Dal.Interfaces
{
    public interface IAuthenticationSrv
    {
        public Task<ClaimsPrincipal> LoginAccountClient(Client client);

        public Task<ClaimsPrincipal> LoginAccountAdmin(Admin admin);
    }
}
