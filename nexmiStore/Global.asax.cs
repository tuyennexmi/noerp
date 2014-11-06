using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NEXMI;
using System.IO;
using System.Timers;
using System.Net;
using System.Net.Mail;

namespace nexmiStore
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                //new { controller = "Account", action = "LogOn", id = UrlParameter.Optional } // Parameter defaults
                new { controller = "Account", action = "LogOn", id = UrlParameter.Optional } // Parameter defaults
            );

            routes.Add(
                new Route("{controller}.mvc/{action}/{id}",
                new RouteValueDictionary(new { controller = "Account", action = "LogOn", id = (string)null }),
                new MvcRouteHandler()
                )
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);

            GlobalTimer.StartGlobalTimer();

            GlobalValues.InitGlobalValues();
             // Khai báo đếm số người truy cập
            Application["So_luot_truy_cap"] = 0;
            Application["So_nguoi_online"] = 0;

            int miliseconds = 24 * 60 * 60 * 1000;
            Timer timer = new Timer();
            timer.Interval = miliseconds;
            timer.Enabled = true;
            timer.Elapsed += new ElapsedEventHandler(Auto_SendMail);

        }

        private static void Auto_SendMail(object source, ElapsedEventArgs e)
        {
            MailMessage oMail = new MailMessage();
            SmtpClient oSmtp = new SmtpClient();
            //string host = email.Split('@')[1];
            oSmtp.Host = "smtp.gmail.com"; //"smtp." + host;
            oMail.IsBodyHtml = true;
            //oSmtp.UseDefaultCredentials = true;
            oSmtp.Credentials = new NetworkCredential(NEXMI.NMCommon.GetCompany().Customer.EmailAddress, NEXMI.NMCommon.GetCompany().Customer.EmailPassword);
            oSmtp.EnableSsl = true;
            oSmtp.Port = 587;
            oMail.From = new MailAddress(NEXMI.NMCommon.GetCompany().Customer.EmailAddress, NEXMI.NMCommon.GetCompany().Customer.CompanyNameInVietnamese, System.Text.Encoding.UTF8);
            
            //string[] nameTemps = Regex.Split(attachmentNames, "_NEXMI_");
            //string[] dataTemps = Regex.Split(attachmentDatas, "_NEXMI_");
            //MemoryStream ms;
            //for (int i = 0; i < dataTemps.Length - 1; i++)
            //{
            //    byte[] data = Convert.FromBase64String(dataTemps[i]);
            //    ms = new MemoryStream(data, 0, data.Length);
            //    oMail.Attachments.Add(new Attachment(ms, nameTemps[i]));
            //}

            //  gửi thông báo, lễ
            NMSystemParamsBL SPBL = new NMSystemParamsBL();
            NMSystemParamsWSI SPWSI = new NMSystemParamsWSI();
            SPWSI.Mode = "SRC_OBJ";            
            SPWSI.SystemParam.ActionDate = DateTime.Today;
            List<NEXMI.SystemParams> list = new List<SystemParams>();
            list = SPBL.callListBL(SPWSI);

            foreach (SystemParams sp in list)
            {
                if (sp.Value == 1)       // nếu SysParams enable...
                {
                    // gán nội dung lấy từ Subject của bảng SystemParams
                    oMail.Subject = sp.Subject; 
                    // gán lại nội dung lấy từ Descrition của bảng SystemParams
                    oMail.Body = sp.Contents + "<br /><br /><div>Sent by <a href=\"http://www.nexmi.com\" target=\"_blank\" style=\"text-decoration: none; color: #f7941d\">NEXMI Solutions</a></div>"; 

                    NMCustomersBL CBL = new NMCustomersBL();
                    NMCustomersWSI CWSI = new NMCustomersWSI();
                    CWSI.Mode = "SRC_OBJ";
                    if (sp.Email == "F")
                    {
                        CWSI.Customer = new Customers();
                        CWSI.Customer.Gender = 0;
                    }
                    List<NMCustomersWSI> CWSIs = CBL.callListBL(CWSI);
                    foreach (NMCustomersWSI customer in CWSIs)
                    {
                        if (!String.IsNullOrEmpty(customer.Customer.EmailAddress))
                        {
                            oMail.To.Clear();
                            oMail.To.Add(customer.Customer.EmailAddress);
                            oSmtp.Send(oMail);
                        }
                    }
                }
            }

            //  chúc mừng sinh nhật khách hàng            
            SPWSI.Mode = "SRC_OBJ";
            SPWSI.SystemParam = new SystemParams();
            SPWSI.SystemParam.Type = "BIRTH";
            list = SPBL.callListBL(SPWSI);
            if (list.Count > 0 & list[0].Value == 1)       // nếu SysParams enable...
            {
                // gán nội dung lấy từ Subject của bảng SystemParams
                oMail.Subject = list[0].Subject;
                // gán lại nội dung lấy từ Descrition của bảng SystemParams
                oMail.Body = list[0].Contents + "<br /><br /><div>Sent by <a href=\"http://www.nexmi.com\" target=\"_blank\" style=\"text-decoration: none; color: #f7941d\">NEXMI Solutions</a></div>";

                NMCustomersBL CBL = new NMCustomersBL();
                NMCustomersWSI CWSI = new NMCustomersWSI();
                CWSI.Customer = new Customers();
                CWSI.Mode = "SRC_OBJ";
                CWSI.Customer.DateOfBirth = DateTime.Today;
                List<NMCustomersWSI> CWSIs = CBL.callListBL(CWSI);
                foreach (NMCustomersWSI customer in CWSIs)
                {
                    if (!String.IsNullOrEmpty(customer.Customer.EmailAddress))
                    {
                        oMail.To.Clear();
                        oMail.To.Add(customer.Customer.EmailAddress);
                        oSmtp.Send(oMail);
                    }
                }
            }
        }
        
        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown
        }

        void Application_BeginRequest(object sender, EventArgs e)
        {
            //if ((Request.Headers["X-Requested-With"] != null && Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            //    || (Request["X-Requested-With"] != null && Request["X-Requested-With"] == "XMLHttpRequest"))
            //{
            //    HttpContext context = HttpContext.Current;
            //    if (context.Session["UserInfo"] != null)
            //    {
            //        //Response.Clear();
            //        //Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
            //        ////Response.Flush();
            //        Response.Redirect("Account/LogOn");
            //        //Response.End();
            //        //Response.Write("aaaaaaaaaaa");
            //    }
            //}
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
             //Prevent Ajax requests from being returned the login form when not authenticated
             //(eg. after authentication timeout).
            //if ((Request.Headers["X-Requested-With"] != null && Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            //    || (Request["X-Requested-With"] != null && Request["X-Requested-With"] == "XMLHttpRequest"))
            //{
            //    if (!Request.IsAuthenticated && Session["UserInfo"] != null)
            //    {
            //        //Response.Clear();
            //        //Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
            //        ////Response.Flush();
            //        //Response.Redirect("Account/LogOn");
            //        //Response.End();
            //        Response.Write("aaaaaaaaaaa");
            //    }
            //}            
        }

        void Session_Start(object sender, EventArgs e)
        {
            // Tăng giá trị biến Application
            //Application["So_luot_truy_cap"] = (int)Application["So_luot_truy_cap"] + 1;
            //Application["So_nguoi_online"] = (int)Application["So_nguoi_online"] + 1;
            //Application["Thoi_gian_bat_dau"] = DateTime.Now;
        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.
            // <%= NEXMI.NMCommon.GetInterface("DISCOUNT", Session["Lang"].ToString()).Name %> trị biến Application
            //double n = (DateTime.Now - (DateTime)Application["Thoi_gian_bat_dau"]).TotalSeconds;
            //Application["So_nguoi_online"] = (int)Application["So_nguoi_online"] - 1;
        }
    }
}