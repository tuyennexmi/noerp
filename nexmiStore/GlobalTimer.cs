using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;

public class GlobalTimer : IDisposable
{
    private static Timer timer;
    private static int interval = 8*60*60*1000;

    public static void StartGlobalTimer()
    {   
        if (null == timer)
        {
            SessionChecker.CreateSessionDictionary();
            timer = new Timer(new TimerCallback(EndSession), HttpContext.Current, 0, interval);
        }
    }

    private static void EndSession(object sender)
    {
        HttpContext context = (HttpContext)sender;
        var dict = HttpRuntime.Cache["UsersCountSession"] as Dictionary<String, DateTime>;
        if (dict != null)
        {
            if (dict.Count > 0)
            {
                try
                {
                    var temp = dict;
                    foreach (var Item in temp)
                    {
                        if ((DateTime.Now.Subtract(Item.Value)).TotalSeconds > 90)
                        {
                            dict.Remove(Item.Key);
                            //NEXMI.NMTimeKeepingBL TKWS = new NEXMI.NMTimeKeepingBL();
                            //NEXMI.NMTimeKeepingWSI TKWSI;
                            //TKWSI = new NEXMI.NMTimeKeepingWSI();
                            //TKWSI.Mode = "SAV_OBJ";
                            //TKWSI.UserId = Item.Key;
                            //TKWSI.EndTime = DateTime.Now;
                            //TKWSI = TKWS.callBussinessLogic(TKWSI);
                        }
                    }
                    HttpRuntime.Cache["UsersCountSession"] = dict;
                }
                catch (Exception ex)
                {
                    EventLog.WriteEntry("Application", "exception at DropUsers: " + ex.Message, EventLogEntryType.Error);
                }
            }
        }
        else
            SessionChecker.CreateSessionDictionary(); // for some resone if the cache is not filled.
    }

    #region IDisposable Members

    public void Dispose()
    {
        timer = null;
    }

    #endregion
}