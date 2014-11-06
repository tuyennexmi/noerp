using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using System.Linq;

public class SessionChecker
{
    private static Dictionary<String, DateTime> dictSession;

    public static Dictionary<String, DateTime> CreateSessionDictionary()
    {
        var objToLock = new Object();
        var dictControl = HttpRuntime.Cache["UsersCountSession"] as Dictionary<String, DateTime>;
        try
        {
            if (dictControl == null)
            {
                lock (objToLock)
                {
                    dictSession = new Dictionary<String, DateTime>();
                    HttpRuntime.Cache.Insert("UsersCountSession", dictSession, null, DateTime.Now.AddMinutes(10100),
                    System.Web.Caching.Cache.NoSlidingExpiration,
                    System.Web.Caching.CacheItemPriority.NotRemovable, null);

                }
            }
        }
        catch (Exception ex)
        {
            EventLog.WriteEntry("Application", "exception at CreatSessionDictionary: " + ex.Message, EventLogEntryType.Error);
        }
        return HttpRuntime.Cache["UsersCountSession"] as Dictionary<String, DateTime>;
    }

    public static bool CheckExist(String pUserId)
    {
        bool flag = false;
        var objElseLock = new object();
        var dic = HttpRuntime.Cache["UsersCountSession"] as Dictionary<String, DateTime>;
        if (dic != null)
        {
            if (dic.ContainsKey(pUserId))
            {
                double a = DateTime.Now.Subtract(dic[pUserId]).TotalMinutes;
                if (DateTime.Now.Subtract(dic[pUserId]).TotalSeconds < 90)
                {
                    flag = true;
                }
            }
        }
        return flag;
    }

    public static void UpdateUsers(String pUserId)
    {
        try
        {
            var objElseLock = new object();
            var dic = HttpRuntime.Cache["UsersCountSession"] as Dictionary<String, DateTime>;
            if (dic != null)
            {
                if (dic.ContainsKey(pUserId))
                {
                    dic[pUserId] = DateTime.Now.AddMinutes(1);
                }
                else
                {
                    lock (objElseLock)
                    {
                        dic.Add(pUserId, DateTime.Now.AddMinutes(1));
                        HttpRuntime.Cache["UsersCountSession"] = dic;
                    }
                }
            }
            else
                CreateSessionDictionary();
        }
        catch (Exception ex)
        {
            EventLog.WriteEntry("Application", "exception at UpadteInsertSessionDictionary: " + ex.Message, EventLogEntryType.Error);
        }
    }

    public static void RemoveUser(String pUserId)
    {
        try
        {
            var objElseLock = new object();
            var dic = HttpRuntime.Cache["UsersCountSession"] as Dictionary<String, DateTime>;
            if (dic != null)
            {
                if (dic.ContainsKey(pUserId))
                {
                    dic.Remove(pUserId);
                    HttpRuntime.Cache["UsersCountSession"] = dic;
                }
            }
        }
        catch (Exception ex)
        {
            EventLog.WriteEntry("Application", "exception at UpadteInsertSessionDictionary: " + ex.Message, EventLogEntryType.Error);
        }
    }

    public static int GetOnlineUsersCount()
    {
        if (null != HttpRuntime.Cache["UsersCountSession"])
        {
            return (HttpRuntime.Cache["UsersCountSession"] as Dictionary<String, DateTime>).Count;
        }
        return 0;
    }

    public static Dictionary<String, DateTime> GetOnlineUsers()
    {
        return HttpRuntime.Cache["UsersCountSession"] as Dictionary<String, DateTime>;
    }
}

public class GlobalValues
{
    public static void InitGlobalValues()
    {
        //new NEXMI.Constants();

        SetTypes(); //Types

        //SetStocks(); //Stocks

        //SetGroups(); //Groups

        //SetAreas(); //Areas

        SetStatuses(); //Statuses
        SetAccTypes();
        
        //SetSettings();
        //SetInterface();
    }

    //public static void SetInterface()
    //{
    //    var control = HttpRuntime.Cache["Interface"] as List<NEXMI.Interface>;
    //    if (control != null)
    //    {
    //        HttpRuntime.Cache.Remove("Interface");
    //    }
        
    //    NEXMI.InterfaceAccesser acc = new NEXMI.InterfaceAccesser();

    //    HttpRuntime.Cache.Insert("Interface", acc.GetAllInterface(true), null, System.Web.Caching.Cache.NoAbsoluteExpiration,
    //        System.Web.Caching.Cache.NoSlidingExpiration,
    //                System.Web.Caching.CacheItemPriority.AboveNormal, null);
    //}

    //public static NEXMI.Interface GetInterface(string owner, string lang)
    //{
    //    NEXMI.Interface WSI = null;
    //    var WSIs = (List<NEXMI.Interface>)HttpRuntime.Cache["Interface"];
    //    if (WSIs != null)
    //        WSI = WSIs.Select(i => i).Where(i => i.OwnerId == owner & i.LanguageId == lang).FirstOrDefault();
    //    if (WSI == null)
    //        WSI = new NEXMI.Interface();
    //    return WSI;
    //}

    public static void SetTypes()
    {
        var control = HttpRuntime.Cache["Types"] as List<NEXMI.NMTypesWSI>;
        if (control != null)
        {
            HttpRuntime.Cache.Remove("Types");
        }
        NEXMI.NMTypesBL TypeBL = new NEXMI.NMTypesBL();
        NEXMI.NMTypesWSI TypeWSI = new NEXMI.NMTypesWSI();
        TypeWSI.Mode = "SRC_OBJ";
        HttpRuntime.Cache.Insert("Types", TypeBL.callListBL(TypeWSI), null, System.Web.Caching.Cache.NoAbsoluteExpiration,
            System.Web.Caching.Cache.NoSlidingExpiration,
                    System.Web.Caching.CacheItemPriority.AboveNormal, null);
    }

    public static string GetType(string id)
    {
        NEXMI.NMTypesWSI WSI = null;
        var WSIs = (List<NEXMI.NMTypesWSI>)HttpRuntime.Cache["Types"];
        if (WSIs != null)
            WSI = WSIs.Select(i => i).Where(i => i.Id == id).FirstOrDefault();
        if (WSI == null)
            WSI = new NEXMI.NMTypesWSI();
        return WSI.Name;
    }

    public static void SetAccTypes()
    {
        var control = HttpRuntime.Cache["ACCTypes"] as List<NEXMI.NMTypesWSI>;
        if (control != null)
        {
            HttpRuntime.Cache.Remove("ACCTypes");
        }
        NEXMI.NMAccountNumbersBL BL = new NEXMI.NMAccountNumbersBL();
        NEXMI.NMAccountNumbersWSI WSI = new NEXMI.NMAccountNumbersWSI();
        WSI.Mode = "SRC_OBJ";
        //WSI.AccountNumber.ForReceipt = objectName;
        //WSI.AccountNumber.IsDiscontinued = false;
        HttpRuntime.Cache.Insert("ACCTypes", BL.callListBL(WSI), null, System.Web.Caching.Cache.NoAbsoluteExpiration,
            System.Web.Caching.Cache.NoSlidingExpiration,
                    System.Web.Caching.CacheItemPriority.AboveNormal, null);
    }

    public static NEXMI.NMAccountNumbersWSI GetACCType(string id)
    {
        NEXMI.NMAccountNumbersWSI WSI = null;
        var WSIs = (List<NEXMI.NMAccountNumbersWSI>)HttpRuntime.Cache["ACCTypes"];
        if (WSIs != null)
            WSI = WSIs.Select(i => i).Where(i => i.AccountNumber.Id == id).FirstOrDefault();
        if (WSI == null)
            WSI = new NEXMI.NMAccountNumbersWSI();
        return WSI;
    }

    //public static void SetStocks()
    //{
    //    var control = HttpRuntime.Cache["Stocks"] as List<NEXMI.NMStocksWSI>;
    //    if (control != null)
    //    {
    //        HttpRuntime.Cache.Remove("Stocks");
    //    }
    //    NEXMI.NMStocksBL StockBL = new NEXMI.NMStocksBL();
    //    NEXMI.NMStocksWSI StockWSI = new NEXMI.NMStocksWSI();
    //    StockWSI.Mode = "SRC_OBJ";
    //    HttpRuntime.Cache.Insert("Stocks", StockBL.callListBL(StockWSI), null, System.Web.Caching.Cache.NoAbsoluteExpiration,
    //        System.Web.Caching.Cache.NoSlidingExpiration,
    //                System.Web.Caching.CacheItemPriority.AboveNormal, null);
    //}

    //public static void SetGroups()
    //{
    //    var control = HttpRuntime.Cache["Groups"] as List<NEXMI.NMGroupsWSI>;
    //    if (control != null)
    //    {
    //        HttpRuntime.Cache.Remove("Groups");
    //    }
    //    NEXMI.NMGroupsBL GroupBL = new NEXMI.NMGroupsBL();
    //    NEXMI.NMGroupsWSI GroupWSI = new NEXMI.NMGroupsWSI();
    //    GroupWSI.Mode = "SRC_OBJ";
    //    HttpRuntime.Cache.Insert("Groups", GroupBL.callListBL(GroupWSI), null, System.Web.Caching.Cache.NoAbsoluteExpiration,
    //        System.Web.Caching.Cache.NoSlidingExpiration,
    //                System.Web.Caching.CacheItemPriority.AboveNormal, null);
    //}

    //public static void SetAreas()
    //{
    //    var control = HttpRuntime.Cache["Areas"] as List<NEXMI.NMAreasWSI>;
    //    if (control != null)
    //    {
    //        HttpRuntime.Cache.Remove("Areas");
    //    }
    //    NEXMI.NMAreasBL AreaBL = new NEXMI.NMAreasBL();
    //    NEXMI.NMAreasWSI AreaWSI = new NEXMI.NMAreasWSI();
    //    AreaWSI.Mode = "SRC_OBJ";
    //    HttpRuntime.Cache.Insert("Areas", AreaBL.callListBL(AreaWSI), null, System.Web.Caching.Cache.NoAbsoluteExpiration,
    //    System.Web.Caching.Cache.NoSlidingExpiration,
    //            System.Web.Caching.CacheItemPriority.AboveNormal, null);
    //}

    public static void SetStatuses()
    {
        var control = HttpRuntime.Cache["Statuses"] as List<NEXMI.NMStatusesWSI>;
        if (control != null)
        {
            HttpRuntime.Cache.Remove("Statuses");
        }
        NEXMI.NMStatusesBL StatusBL = new NEXMI.NMStatusesBL();
        NEXMI.NMStatusesWSI StatusWSI = new NEXMI.NMStatusesWSI();
        StatusWSI.Mode = "SRC_OBJ";
        HttpRuntime.Cache.Insert("Statuses", StatusBL.callListBL(StatusWSI), null, System.Web.Caching.Cache.NoAbsoluteExpiration,
        System.Web.Caching.Cache.NoSlidingExpiration,
                System.Web.Caching.CacheItemPriority.AboveNormal, null);
    }


    public static NEXMI.NMStatusesWSI GetStatus(string id)
    {
        NEXMI.NMStatusesWSI WSI = null;
        var WSIs = (List<NEXMI.NMStatusesWSI>)HttpRuntime.Cache["Statuses"];
        if (WSIs != null)
            WSI = WSIs.Select(i => i).Where(i => i.Id == id).FirstOrDefault();
        if (WSI == null)
            WSI = new NEXMI.NMStatusesWSI();
        return WSI;
    }

    public static List<NEXMI.NMStatusesWSI> GetStatusByObjectName(string objectName)
    {
        List<NEXMI.NMStatusesWSI> WSI = null;
        var WSIs = (List<NEXMI.NMStatusesWSI>)HttpRuntime.Cache["Statuses"];
        if (WSIs != null)
            WSI = WSIs.Select(i => i).Where(i => i.Status.ObjectName == objectName).ToList();
        if (WSI == null)
            WSI = new List<NEXMI.NMStatusesWSI>();
        return WSI;
    }

    //public static void SetSettings()
    //{
    //    var control = HttpRuntime.Cache["Settings"] as List<NEXMI.Settings>;
    //    if (control != null)
    //    {
    //        HttpRuntime.Cache.Remove("Settings");
    //    }
    //    HttpRuntime.Cache.Insert("Settings", NEXMI.NMCommon.GetSettings(), null, System.Web.Caching.Cache.NoAbsoluteExpiration,
    //        System.Web.Caching.Cache.NoSlidingExpiration,
    //                System.Web.Caching.CacheItemPriority.AboveNormal, null);
    //}

    

    //public static NEXMI.NMStocksWSI GetStock(string id)
    //{
    //    NEXMI.NMStocksWSI WSI = null;
    //    var WSIs = (List<NEXMI.NMStocksWSI>)HttpRuntime.Cache["Stocks"];
    //    if (WSIs != null)
    //        WSI = WSIs.Select(i => i).Where(i => i.Id == id).FirstOrDefault();
    //    if (WSI == null)
    //        WSI = new NEXMI.NMStocksWSI();
    //    return WSI;
    //}

    //public static NEXMI.NMGroupsWSI GetGroups(string id)
    //{
    //    NEXMI.NMGroupsWSI WSI = null;
    //    var WSIs = (List<NEXMI.NMGroupsWSI>)HttpRuntime.Cache["Groups"];
    //    if (WSIs != null)
    //        WSI = WSIs.Select(i => i).Where(i => i.Id == id).FirstOrDefault();
    //    if (WSI == null)
    //        WSI = new NEXMI.NMGroupsWSI();
    //    return WSI;
    //}

    //public static NEXMI.NMAreasWSI GetAreas(string id)
    //{
    //    NEXMI.NMAreasWSI WSI = null;
    //    var WSIs = (List<NEXMI.NMAreasWSI>)HttpRuntime.Cache["Areas"];
    //    if (WSIs != null)
    //        WSI = WSIs.Select(i => i).Where(i => i.Area.Id == id).FirstOrDefault();
    //    if (WSI == null)
    //        WSI = new NEXMI.NMAreasWSI();
    //    return WSI;
    //}


    //public static bool GetSettings(string id)
    //{
    //    NEXMI.Settings obj = null;
    //    var objs = (List<NEXMI.Settings>)HttpRuntime.Cache["Settings"];
    //    if (objs != null)
    //        obj = objs.Select(i => i).Where(i => i.Id == id).FirstOrDefault();
    //    if (obj != null)
    //        return obj.Value;
    //    else
    //        return false;
    //}
}