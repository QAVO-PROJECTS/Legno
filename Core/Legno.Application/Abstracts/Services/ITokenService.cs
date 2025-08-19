﻿using Legno.Application.Dtos.Account;
using Legno.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Absrtacts.Services
{
    public interface ITokenService
    {
        TokenResponseDto CreateToken(Admin admin, string role, int expireDate = 1440);
    }
}
