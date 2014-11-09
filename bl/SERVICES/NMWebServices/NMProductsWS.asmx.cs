using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace NEXMI
{
    /// <summary>
    /// Summary description for NMProductWS
    /// </summary>
    [WebService(Namespace = "http://nexmi.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class NMProductsWS : System.Web.Services.WebService
    {
        [WebMethod]
        public NMProductsWSI callSingleWS(NMProductsWSI wsi)
        {
            NMProductsBL BL = new NMProductsBL();
            return BL.callSingleBL(wsi);
        }
        [WebMethod]
        public List<NMProductsWSI> callListWS(NMProductsWSI wsi)
        {
            NMProductsBL BL = new NMProductsBL();
            return BL.callListBL(wsi);
        }
    }
}
