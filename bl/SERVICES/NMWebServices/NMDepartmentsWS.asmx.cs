﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace NEXMI
{
    /// <summary>
    /// Summary description for NMUserWS
    /// </summary>
    [WebService(Namespace = "http://nexmi.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class NMDepartmentsWS : System.Web.Services.WebService
    {
        [WebMethod]
        public NMDepartmentsWSI callSingleWS(NMDepartmentsWSI wsi)
        {
            NMDepartmentsBL BL = new NMDepartmentsBL();
            return BL.callSingleBL(wsi);
        }
        [WebMethod]
        public List<NMDepartmentsWSI> callListWS(NMDepartmentsWSI wsi)
        {
            NMDepartmentsBL BL = new NMDepartmentsBL();
            return BL.callListBL(wsi);
        }
    }
}
