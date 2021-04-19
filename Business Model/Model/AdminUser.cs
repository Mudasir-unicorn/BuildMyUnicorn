﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Business_Model.Model
{
    public class AdminUser :Common
    {
        public Guid AdminUserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public DateTime CurrentLogin { get; set; }
        public DateTime LastLogin { get; set; }
    }
}