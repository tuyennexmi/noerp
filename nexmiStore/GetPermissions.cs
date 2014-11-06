using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NEXMI;

public class GetPermissions
{
    //public static List<NMPermissionsWSI> Permissions = new List<NMPermissionsWSI>();

    public static bool Get(List<NMPermissionsWSI> Permissions, string functionId, string action)
    {
        bool isOK = false;

        if (Permissions != null)
        {
            var Permission = Permissions.Select(i => i).Where(i => i.FunctionId == functionId).FirstOrDefault();
            if (Permission != null)
            {
                if (action == "")
                {
                    if (Permission.PSelect == "Y" || Permission.PInsert == "Y" || Permission.PUpdate == "Y" || Permission.PDelete == "Y"
                        || Permission.Approval == "Y" || Permission.ViewAll == "Y" || Permission.Calculate == "Y" || Permission.History == "Y")
                    {
                        isOK = true;
                    }
                }
                else
                {
                    switch (action)
                    {
                        case "Select": if (Permission.PSelect == "Y") isOK = true; break;
                        case "Insert": if (Permission.PInsert == "Y") isOK = true; break;
                        case "Update": if (Permission.PUpdate == "Y") isOK = true; break;
                        case "Reconcile": if (Permission.Reconcile == "Y") isOK = true; break;
                        case "Delete": if (Permission.PDelete == "Y") isOK = true; break;
                        case "Approval": if (Permission.Approval == "Y") isOK = true; break;
                        case "ViewAll": if (Permission.ViewAll == "Y") isOK = true; break;
                        case "Calculate": if (Permission.Calculate == "Y") isOK = true; break;
                        case "History": if (Permission.History == "Y") isOK = true; break;

                        case "ViewPrice": if (Permission.ViewPrice == "Y") isOK = true; break;
                        case "Export": if (Permission.Export == "Y") isOK = true; break;
                        case "PPrint": if (Permission.PPrint == "Y") isOK = true; break;
                        case "Duplicate": if (Permission.Duplicate == "Y") isOK = true; break;
                    }
                }
            }
        }
        return isOK;
    }

    public static bool GetSelect(List<NMPermissionsWSI> Permissions, string functionId)
    {
        return Get(Permissions, functionId, "Select");
    }

    public static bool GetInsert(List<NMPermissionsWSI> Permissions, string functionId)
    {
        return Get(Permissions, functionId, "Insert");
    }

    public static bool GetUpdate(List<NMPermissionsWSI> Permissions, string functionId)
    {
        return Get(Permissions, functionId, "Update");
    }

    public static bool GetReconcile(List<NMPermissionsWSI> Permissions, string functionId)
    {
        return Get(Permissions, functionId, "Reconcile");
    }

    public static bool GetDelete(List<NMPermissionsWSI> Permissions, string functionId)
    {
        return Get(Permissions, functionId, "Delete");
    }

    public static bool GetApproval(List<NMPermissionsWSI> Permissions, string functionId)
    {
        return Get(Permissions, functionId, "Approval");
    }

    public static bool GetViewAll(List<NMPermissionsWSI> Permissions, string functionId)
    {
        return Get(Permissions, functionId, "ViewAll");
    }

    public static bool GetCalculate(List<NMPermissionsWSI> Permissions, string functionId)
    {
        return Get(Permissions, functionId, "Calculate");
    }

    public static bool GetHistory(List<NMPermissionsWSI> Permissions, string functionId)
    {
        return Get(Permissions, functionId, "History");
    }

    public static bool GetViewPrice(List<NMPermissionsWSI> Permissions, string functionId)
    {
        return Get(Permissions, functionId, "ViewPrice");
    }

    public static bool GetExport(List<NMPermissionsWSI> Permissions, string functionId)
    {
        return Get(Permissions, functionId, "Export");
    }

    public static bool GetPPrint(List<NMPermissionsWSI> Permissions, string functionId)
    {
        return Get(Permissions, functionId, "PPrint");
    }

    public static bool GetDuplicate(List<NMPermissionsWSI> Permissions, string functionId)
    {
        return Get(Permissions, functionId, "Duplicate");
    }
}

//public class UserInfoCache
//{
//    public static NMCustomersWSI User = new NMCustomersWSI();
//}