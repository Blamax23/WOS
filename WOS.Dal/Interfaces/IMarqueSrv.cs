﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOS.Model;

namespace WOS.Dal.Interfaces
{
    public interface IMarqueSrv
    {
        List<Marque> GetMarquesByHome();

        Marque GetMarqueById(int id);

        void AddMarque(Marque marque);

        void UpdateHomeMarque(int id, bool tendance);

        void DeleteMarque(int id);
    }
}
