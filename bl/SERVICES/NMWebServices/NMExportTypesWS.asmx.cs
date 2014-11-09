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
    public class NMExportTypesWS : System.Web.Services.WebService
    {
        [WebMethod]
        public NMExportTypesWSI callSingleWS(NMExportTypesWSI wsi)
        {
            NMExportTypesBL BL = new NMExportTypesBL();
            return BL.callSingleBL(wsi);
        }
        [WebMethod]
        public List<NMExportTypesWSI> callListWS(NMExportTypesWSI wsi)
        {
            NMExportTypesBL BL = new NMExportTypesBL();
            return BL.callListBL(wsi);
        }
    }
}
