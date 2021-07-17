﻿using ALMS_DAL;
using Business_Model.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;



namespace BuildMyUnicorn_Supplier.Business_Layer
{
    public class MasterManager
    {
        public IEnumerable<Option> GetOptionMasterList(int Type)
        {
            DataLayer obj = new DataLayer(ConfigurationManager.ConnectionStrings["ConnectionBuildMyUnicorn"].ConnectionString, Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeOut"]));
            List<ParametersCollection> parameters = new List<ParametersCollection>() {
            new ParametersCollection { ParamterName = "@Type", ParamterValue = Type, ParamterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }   };
            return obj.GetList<Option>(CommandType.StoredProcedure, "sp_get_option_master_by_type", parameters);
        }
        public IEnumerable<Modules> GetModuleList()
        {
            DataLayer obj = new DataLayer(ConfigurationManager.ConnectionStrings["ConnectionBuildMyUnicorn"].ConnectionString, Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeOut"]));
            return obj.GetList<Modules>(CommandType.StoredProcedure, "sp_get_all_module", null);

        }

        public _EmailTemplates GetEmailTemplate(string EmailTemplateCode)
        {
            var query = $@"select * from tbl_email_templates where EmailTemplateCode = '{EmailTemplateCode}' and IsActive = 1";
            return SharedManager.GetSingle<_EmailTemplates>(query);
        }

    }
}