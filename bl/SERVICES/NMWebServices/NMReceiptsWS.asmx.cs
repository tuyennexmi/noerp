using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace NEXMI
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://nexmi.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class NMReceiptsWS : System.Web.Services.WebService
    {
        [WebMethod]
        public NMReceiptsWSI callSingleWS(NMReceiptsWSI wsi)
        {
            NMReceiptsBL BL = new NMReceiptsBL();
            return BL.callSingleBL(wsi);
        }
        [WebMethod]
        public List<NMReceiptsWSI> callListWS(NMReceiptsWSI wsi)
        {
            NMReceiptsBL BL = new NMReceiptsBL();
            return BL.callListBL(wsi);
        }
    }
}
