using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace NEXMI
{
    /// <summary>
    /// Summary description for NMImportProductWS
    /// </summary>
    [WebService(Namespace = "http://nexmi.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class NMImportTypesWS : System.Web.Services.WebService
    {
        [WebMethod]
        public NMImportTypesWSI callSingleWS(NMImportTypesWSI wsi)
        {
            NMImportTypesBL BL = new NMImportTypesBL();
            return BL.callSingleBL(wsi);
        }
        [WebMethod]
        public List<NMImportTypesWSI> callListWS(NMImportTypesWSI wsi)
        {
            NMImportTypesBL BL = new NMImportTypesBL();
            return BL.callListBL(wsi);
        }
    }
}
