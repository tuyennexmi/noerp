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
    public class NMCurrenciesWS : System.Web.Services.WebService
    {
        [WebMethod]
        public NMCurrenciesWSI callSingleWS(NMCurrenciesWSI wsi)
        {
            NMCurrenciesBL BL = new NMCurrenciesBL();
            return BL.callSingleBL(wsi);
        }
        [WebMethod]
        public List<NMCurrenciesWSI> callListWS(NMCurrenciesWSI wsi)
        {
            NMCurrenciesBL BL = new NMCurrenciesBL();
            return BL.callListBL(wsi);
        }
    }
}
