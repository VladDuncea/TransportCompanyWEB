﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransportCompany.Models
{
    public class UserViewModel
    {
        //Utilizatorul
        public ApplicationUser User { get; set; }
        //Rolul sau
        public string RoleName { get; set; }
    }
}