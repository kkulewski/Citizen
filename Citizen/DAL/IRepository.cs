﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Citizen.DAL
{
    public interface IRepository
    {
        IApplicationUserRepo ApplicationUserRepo();
    }
}