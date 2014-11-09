using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace NEXMI
{
    /// <summary>
    /// Summary description for NMOrderProductWS
    /// </summary>
    [WebService(Namespace = "http://nexmi.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class NMRequirementTypesWS : System.Web.Services.WebService
    {
        [WebMethod]
        public NMRequirementTypesWSI callSingleWS(NMRequirementTypesWSI wsi)
        {
            NMRequirementTypesBL BL = new NMRequirementTypesBL();
            return BL.callSingleBL(wsi);
        }
        [WebMethod]
        public List<NMRequirementTypesWSI> callListWS(NMRequirementTypesWSI wsi)
        {
            NMRequirementTypesBL BL = new NMRequirementTypesBL();
            return BL.callListBL(wsi);
        }
    }
}
