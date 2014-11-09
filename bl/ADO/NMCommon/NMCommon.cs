
// NMCommon.cs

// * http://nexmi.com
// * NoErp Project - Nexmi Open ERP
// * Copyright (C) 2012, Nguyễn Quang Tuyến (tuyen.nq@nexmi.com), AUTHOR.txt (http://nexmi.com/about)
// * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; 
//   either version 2 of the License, or (at your option) any later version. This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
//   without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details. 
//   You should have received a copy of the GNU General Public License along with this library; if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// *

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using System.Globalization;
using System.Data;
using System.Text.RegularExpressions;

namespace NEXMI
{
    public class NMCommon
    {
        public static String AddOperatorForQuery(String strCriteria)
        {
            if (strCriteria == "")
            {
                strCriteria += " WHERE";
            }
            else
            {
                strCriteria += " AND";
            }
            return strCriteria;
        }

        public static bool CheckExistInDatabase(string id, string col, string tableName)
        {
            ISession session = SessionFactory.GetNewSession();
            IQuery query = session.CreateSQLQuery("SELECT COUNT(O." + col + ") FROM " + tableName + " AS O WHERE O." + col + "= :x");
            query.SetString("x", id);
            int count = 0;
            try
            {
                count = (int)query.UniqueResult();
            }
            catch { }
            if (count != 0)
                return true;
            return false;
        }

        public static int Counter(IQuery query)
        {
            int count = 0;
            try
            {
                count = (int)query.UniqueResult();
            }
            catch { }
            return count;
        }

        public static string PreviousId(string currentId, string columName, string tableName, string customString)
        {
            ISession session = SessionFactory.GetNewSession();
            IQuery query = session.CreateSQLQuery(";WITH cte AS (SELECT " + columName + ", ROW_NUMBER() OVER (ORDER BY CreatedDate) AS RowIndex FROM " + tableName + " " + customString + ") SELECT A." + columName + " FROM cte AS A where A.RowIndex = (SELECT B.RowIndex FROM cte AS B where B." + columName + " = :currentId) - 1");
            query.SetString("currentId", currentId);
            string previousId = "";
            try
            {
                previousId = query.UniqueResult().ToString();
            }
            catch { }
            return previousId;
        }

        public static string NextId(string currentId, string columName, string tableName, string customString)
        {
            ISession session = SessionFactory.GetNewSession();
            IQuery query = session.CreateSQLQuery(";WITH cte AS (SELECT " + columName + ", ROW_NUMBER() OVER (ORDER BY CreatedDate) AS RowIndex FROM " + tableName + " " + customString + ") SELECT A." + columName + " FROM cte AS A where A.RowIndex = (SELECT B.RowIndex FROM cte AS B where B." + columName + " = :currentId) + 1");
            query.SetString("currentId", currentId);
            string nextId = "";
            try
            {
                nextId = query.UniqueResult().ToString();
            }
            catch { }
            return nextId;
        }

        public static string UpdateObjectStatus(string objectName, string columnIdName, string id, string columnStatusName, string status, string columnTypeName, string typeId)
        {
            string error = "";
            try
            {
                ISession session = SessionFactory.GetNewSession();
                string strCmd = "";
                strCmd += "update " + objectName + " set";
                strCmd += " " + columnStatusName + " = :x";
                if (!string.IsNullOrEmpty(typeId))
                {
                    strCmd += ", " + columnTypeName + " = :y";
                }
                strCmd += " where " + columnIdName + " = :z";
                IQuery query = session.CreateSQLQuery(strCmd);
                query.SetString("x", status);
                if (!string.IsNullOrEmpty(typeId))
                {
                    query.SetString("y", typeId);
                }
                query.SetString("z", id);
                query.ExecuteUpdate();
            }
            catch (Exception ex)
            {
                error = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return error;
        }

        public static string UpdateObjectStatus(string objectName, string id, string status, string typeId, string ActionBy, string description)
        {
            string error = "";
            ISession session = SessionFactory.GetNewSession();
            ITransaction tx = session.BeginTransaction();

            try
            {
                string strCmd = "";
                strCmd += "update " + objectName + " set";
                switch (objectName)
                {
                    case "SalesOrders":
                        strCmd += " OrderStatus = :x";
                        if (!string.IsNullOrEmpty(typeId))
                        {
                            strCmd += ", OrderTypeId = :y";
                        }
                        strCmd += " where OrderId = :z";
                        break;
                    case "PurchaseOrders":
                        strCmd += " OrderStatus = :x";
                        if (!string.IsNullOrEmpty(typeId))
                        {
                            strCmd += ", OrderTypeId = :y";
                        }
                        strCmd += " where Id = :z";
                        break;
                    case "Exports":
                        strCmd += " ExportStatus = :x";
                        if (!string.IsNullOrEmpty(typeId))
                        {
                            strCmd += ", ExportTypeId = :y";
                        }
                        strCmd += " where ExportId = :z";
                        break;
                    case "SalesInvoices":
                        strCmd += " InvoiceStatus = :x";
                        if (!string.IsNullOrEmpty(typeId))
                        {
                            strCmd += ", InvoiceTypeId = :y";
                        }
                        strCmd += " where InvoiceId = :z";
                        break;
                    case "Imports":
                        strCmd += " ImportStatus = :x";
                        if (!string.IsNullOrEmpty(typeId))
                        {
                            strCmd += ", ImportTypeId = :y";
                        }
                        strCmd += " where ImportId = :z";
                        break;
                    case "Projects":
                        strCmd += " StatusId = :x";
                        strCmd += " where ProjectId = :z";
                        typeId = "";
                        break;
                    case "Tasks":
                        strCmd += " StatusId = :x";
                        strCmd += " where TaskId = :z";
                        typeId = "";
                        break;
                    case "Messages":
                        strCmd += " IsRead = :x";
                        strCmd += " where MessageId = :z";
                        typeId = "";
                        break;
                    default: break;
                }
                IQuery query = session.CreateSQLQuery(strCmd);
                query.SetString("x", status);
                if (!string.IsNullOrEmpty(typeId))
                {
                    query.SetString("y", typeId);
                }
                query.SetString("z", id);
                query.ExecuteUpdate();

                // tạo log
                if (string.IsNullOrEmpty(description))
                    description = "";

                string msgType = NMConstant.MessageTypes.SysLog;
                if (status == NMConstant.SOStatuses.Sent)
                    msgType = NMConstant.MessageTypes.Message;

                SaveMessage(session, id, "thay đổi trạng thái", description, ActionBy, null, null, msgType);
                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                error = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return error;
        }

        public static string UpdateObjectStatus(ISession session, string objectName, string id, string status, string typeId, string ActionBy, string description)
        {
            string error = "";
            try
            {
                string strCmd = "";
                strCmd += "update " + objectName + " set";
                switch (objectName)
                {
                    case "SalesOrders":
                        strCmd += " OrderStatus = :x";
                        if (!string.IsNullOrEmpty(typeId))
                        {
                            strCmd += ", OrderTypeId = :y";
                        }
                        strCmd += " where OrderId = :z";
                        break;
                    case "PurchaseOrders":
                        strCmd += " OrderStatus = :x";
                        if (!string.IsNullOrEmpty(typeId))
                        {
                            strCmd += ", OrderTypeId = :y";
                        }
                        strCmd += " where Id = :z";
                        break;
                    case "Exports":
                        strCmd += " ExportStatus = :x";
                        if (!string.IsNullOrEmpty(typeId))
                        {
                            strCmd += ", ExportTypeId = :y";
                        }
                        strCmd += " where ExportId = :z";
                        break;
                    case "SalesInvoices":
                        strCmd += " InvoiceStatus = :x";
                        if (!string.IsNullOrEmpty(typeId))
                        {
                            strCmd += ", InvoiceTypeId = :y";
                        }
                        strCmd += " where InvoiceId = :z";
                        break;
                    case "PurchaseInvoices":
                        strCmd += " InvoiceStatus = :x";
                        if (!string.IsNullOrEmpty(typeId))
                        {
                            strCmd += ", InvoiceTypeId = :y";
                        }
                        strCmd += " where InvoiceId = :z";
                        break;
                    case "Imports":
                        strCmd += " ImportStatus = :x";
                        if (!string.IsNullOrEmpty(typeId))
                        {
                            strCmd += ", ImportTypeId = :y";
                        }
                        strCmd += " where ImportId = :z";
                        break;
                    case "Projects":
                        strCmd += " StatusId = :x";
                        strCmd += " where ProjectId = :z";
                        typeId = "";
                        break;
                    case "Tasks":
                        strCmd += " StatusId = :x";
                        strCmd += " where TaskId = :z";
                        typeId = "";
                        break;
                    default: break;
                }
                IQuery query = session.CreateSQLQuery(strCmd);
                query.SetString("x", status);
                if (!string.IsNullOrEmpty(typeId))
                {
                    query.SetString("y", typeId);
                }
                query.SetString("z", id);
                query.ExecuteUpdate();

                if (!string.IsNullOrEmpty(description))
                    SaveMessage(session, id, "thay đổi trạng thái", description, ActionBy, null, null, NMConstant.MessageTypes.SysLog);
            }
            catch (Exception ex)
            {
                error = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return error;
        }

        //public static string UpdateObjectStatus(string objectName, string id, string status, string typeId, string ActionBy)
        //{
        //    string error = "";
        //    ISession session = SessionFactory.GetNewSession();
        //    ITransaction tx = session.BeginTransaction();

        //    try
        //    {
        //        string strCmd = "";
        //        strCmd += "update " + objectName + " set";
        //        switch (objectName)
        //        {
        //            case "SalesOrders":
        //                strCmd += " OrderStatus = :x";
        //                if (!string.IsNullOrEmpty(typeId))
        //                {
        //                    strCmd += ", OrderTypeId = :y";
        //                }
        //                strCmd += " where OrderId = :z";
        //                break;
        //            case "PurchaseOrders":
        //                strCmd += " OrderStatus = :x";
        //                if (!string.IsNullOrEmpty(typeId))
        //                {
        //                    strCmd += ", OrderTypeId = :y";
        //                }
        //                strCmd += " where Id = :z";
        //                break;
        //            case "Exports":
        //                strCmd += " ExportStatus = :x";
        //                if (!string.IsNullOrEmpty(typeId))
        //                {
        //                    strCmd += ", ExportTypeId = :y";
        //                }
        //                strCmd += " where ExportId = :z";
        //                break;
        //            case "SalesInvoices":
        //                strCmd += " InvoiceStatus = :x";
        //                if (!string.IsNullOrEmpty(typeId))
        //                {
        //                    strCmd += ", InvoiceTypeId = :y";
        //                }
        //                strCmd += " where InvoiceId = :z";
        //                break;
        //            case "Imports":
        //                strCmd += " ImportStatus = :x";
        //                if (!string.IsNullOrEmpty(typeId))
        //                {
        //                    strCmd += ", ImportTypeId = :y";
        //                }
        //                strCmd += " where ImportId = :z";
        //                break;
        //            case "Projects":
        //                strCmd += " StatusId = :x";
        //                strCmd += " where ProjectId = :z";
        //                typeId = "";
        //                break;
        //            case "Tasks":
        //                strCmd += " StatusId = :x";
        //                strCmd += " where TaskId = :z";
        //                typeId = "";
        //                break;
        //            default: break;
        //        }
        //        IQuery query = session.CreateSQLQuery(strCmd);
        //        query.SetString("x", status);
        //        if (!string.IsNullOrEmpty(typeId))
        //        {
        //            query.SetString("y", typeId);
        //        }
        //        query.SetString("z", id);
        //        query.ExecuteUpdate();

        //        // tạo log
        //        //NMStatusesBL SBL = new NMStatusesBL();
        //        //NMStatusesWSI SWSI = new NMStatusesWSI();
        //        //SWSI.Mode = "SEL_OBJ";
        //        //SWSI.Id = status;
        //        //SBL.callSingleBL(SWSI);

        //        //NMMessagesBL.SaveMessage(session, id, "thay đổi trạng thái", SWSI.Status.Description, ActionBy, null, null, NMConstant.MessageTypes.SysLog);
        //        tx.Commit();
        //    }
        //    catch (Exception ex)
        //    {
        //        tx.Rollback();
        //        error = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
        //    }
        //    return error;
        //}

        public static double GetQuantityOfDetails(string masterName, string masterId, string detailName, string masterIdInDetail, string columnQuantity, string id)
        {
            ISession session = SessionFactory.GetNewSession();
            string strCmd = "";
            strCmd += "select sum(D." + columnQuantity + ") from " + detailName + " D, " + masterName + " M where M." + masterId + " = D." + masterIdInDetail + " and M." + masterId + " = :x";
            IQuery query = session.CreateSQLQuery(strCmd);
            query.SetString("x", id);
            return (double)query.UniqueResult();
        }

        public static double GetQuantityOfDetails(string objectName, string id)
        {
            double value = 0;
            ISession session = SessionFactory.GetNewSession();
            string strCmd = "";
            switch (objectName)
            {
                case "SalesOrders":
                    strCmd += "select sum(D.Quantity) from SalesOrderDetails D, SalesOrders M where M.OrderId = D.OrderId and M.OrderId = :x";
                    break;
                case "Exports":
                    strCmd += "select sum(D.Quantity) from ExportDetails D, Exports M where M.ExportId = D.ExportId and M.ExportId = :x";
                    break;
                default: value = 0; break;
            }
            IQuery query = session.CreateSQLQuery(strCmd);
            query.SetString("x", id);
            try
            {
                string a = query.UniqueResult().ToString();
                value = double.Parse(a);
            }
            catch { }
            return value;
        }

        public static string GetName(string id, string langId)
        {
            ISession session = SessionFactory.GetNewSession();
            IQuery query = session.CreateSQLQuery("select T.Name from Translations as T where T.OwnerId = :id and T.LanguageId = '" + langId + "'");
            query.SetString("id", id);
            string name = "";
            try
            {
                name = query.UniqueResult().ToString();
            }
            catch { }
            return name;
        }

        public static string GetTypeName(string id)
        {
            ISession session = SessionFactory.GetNewSession();
            IQuery query = session.CreateSQLQuery("select T.Name from Types as T where T.Id = :id");
            query.SetString("id", id);
            string name = "";
            try
            {
                name = query.UniqueResult().ToString();
            }
            catch { }
            return name;
        }

        public static string GetGroupName(string id)
        {
            ISession session = SessionFactory.GetNewSession();
            IQuery query = session.CreateSQLQuery("select T.Name from Groups as T where T.Id = :id");
            query.SetString("id", id);
            string name = "";
            try
            {
                name = query.UniqueResult().ToString();
            }
            catch { }
            return name;
        }

        public static string GetCustomerName(string id)
        {
            ISession session = SessionFactory.GetNewSession();
            IQuery query = session.CreateSQLQuery("select O.CompanyNameInVietnamese from Customers as O where O.CustomerId = :id");
            query.SetString("id", id);
            string name = "";
            try
            {
                name = query.UniqueResult().ToString();
            }
            catch { }
            return name;
        }

        public static string GetProductUnitName(string productId)
        {
            ISession session = SessionFactory.GetNewSession();
            IQuery query = session.CreateSQLQuery("select O.Name from ProductUnits as O where O.Id = (select P.ProductUnit from Products P where P.ProductId = :id)");
            query.SetString("id", productId);
            string name = "";
            try
            {
                name = query.UniqueResult().ToString();
            }
            catch { }
            return name;
        }

        public static string GetUnitNameById(string unitId)
        {
            ISession session = SessionFactory.GetNewSession();
            ProductUnitsAccesser Accesser = new ProductUnitsAccesser(session);
            ProductUnits unit = Accesser.GetAllProductUnitsByID(unitId, true);

            string name = "";
            try
            {
                name = unit.Name;
            }
            catch { }
            return name;
        }

        public static string GetProductType(string productId)
        {
            ISession session = SessionFactory.GetNewSession();
            IQuery query = session.CreateSQLQuery("select P.TypeId from Products as P where P.ProductId = :id");
            query.SetString("id", productId);
            string name = "";
            try
            {
                name = query.UniqueResult().ToString();
            }
            catch { }
            return name;
        }

        public static string GetProductCode(string productId)
        {
            ISession session = SessionFactory.GetNewSession();
            IQuery query = session.CreateSQLQuery("select P.ProductCode from Products as P where P.ProductId = :id");
            query.SetString("id", productId);
            string name = "";
            try
            {
                name = query.UniqueResult().ToString();
            }
            catch { }
            return name;
        }

        public static string GetProductUnit(string productId)
        {
            ISession session = SessionFactory.GetNewSession();
            IQuery query = session.CreateSQLQuery("select P.ProductUnit from Products as P where P.ProductId = :id");
            query.SetString("id", productId);
            string name = "";
            try
            {
                name = query.UniqueResult().ToString();
            }
            catch { }
            return name;
        }

        public static NMCustomersWSI GetCompany()
        {
            NMCustomersBL BL = new NMCustomersBL();
            NMCustomersWSI WSI = new NMCustomersWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Customer = new Customers();
            WSI.Customer.CustomerId = "COMPANY";
            WSI = BL.callSingleBL(WSI);
            return WSI;            
        }

        public static int PageSize()
        {
            return NMCommon.GetSettingValue("PAGE_SIZE");
        }

        public static string GetParameterName(string id)
        {
            ISession session = SessionFactory.GetNewSession();
            IQuery query = session.CreateSQLQuery("select O.Name from Parameters as O where O.Id = :id");
            query.SetString("id", id);
            string name = "";
            try
            {
                name = query.UniqueResult().ToString();
            }
            catch { }
            return name;
        }

        public static string DeleteObject(string tableName, string columnName, string id)
        {
            string error = "";
            try
            {
                ISession session = SessionFactory.GetNewSession();
                session.CreateSQLQuery("delete " + tableName + " where " + columnName + " = " + id).ExecuteUpdate();
            }
            catch (Exception ex)
            {
                error = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return error;
        }

        public static string RandomString(int length, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < length; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        public static string ToRelativeDate(DateTime input)
        {
            TimeSpan oSpan = DateTime.Now.Subtract(input);
            double TotalMinutes = oSpan.TotalMinutes;
            string Suffix = " trước";

            if (TotalMinutes < 0.0)
            {
                TotalMinutes = Math.Abs(TotalMinutes);
                Suffix = " lúc này";
            }

            var aValue = new SortedList<double, Func<string>>();
            aValue.Add(0.75, () => "ít hơn 1 phút");
            aValue.Add(1.5, () => "1 phút");
            aValue.Add(45, () => string.Format("{0} phút", Math.Round(TotalMinutes)));
            aValue.Add(90, () => "1 giờ");
            aValue.Add(1440, () => string.Format("{0} giờ", Math.Round(Math.Abs(oSpan.TotalHours)))); // 60 * 24
            aValue.Add(2880, () => "1 ngày"); // 60 * 48
            aValue.Add(43200, () => string.Format("{0} ngày", Math.Floor(Math.Abs(oSpan.TotalDays)))); // 60 * 24 * 30
            aValue.Add(86400, () => "1 tháng"); // 60 * 24 * 60
            aValue.Add(525600, () => string.Format("{0} tháng", Math.Floor(Math.Abs(oSpan.TotalDays / 30)))); // 60 * 24 * 365 
            aValue.Add(1051200, () => "1 năm"); // 60 * 24 * 365 * 2
            aValue.Add(double.MaxValue, () => string.Format("{0} năm", Math.Floor(Math.Abs(oSpan.TotalDays / 365))));

            return aValue.First(n => TotalMinutes < n.Key).Value.Invoke() + Suffix;
        }

        public static string ToRelativeDate(DateTime input, string languageId)
        {
            string beforeText = " trước", nowText = " lúc này", lessText = "ít hơn ",
                minText = " phút", minText1 = "phút",
                hourText = " giờ", hourText1 = " giờ",
                dayText = " ngày", dayText1 = " ngày",
                monthText = " tháng", monthText1 = " tháng",
                yearText = " năm", yearText1 = " năm";
            switch (languageId)
            {
                case "en":
                    beforeText = " ago";
                    nowText = " now";
                    lessText = "less than ";
                    minText = " minute"; minText1 = " minutes";
                    hourText = " hour"; hourText1 = " hours";
                    dayText = " day"; dayText1 = " days";
                    monthText = " month"; monthText1 = " months";
                    yearText = " year"; yearText1 = " years";
                    break;
            }
            TimeSpan oSpan = DateTime.Now.Subtract(input);
            double TotalMinutes = oSpan.TotalMinutes;
            string Suffix = beforeText;

            if (TotalMinutes < 0.0)
            {
                TotalMinutes = Math.Abs(TotalMinutes);
                Suffix = nowText;
            }

            var aValue = new SortedList<double, Func<string>>();
            aValue.Add(0.75, () => lessText + "1" + minText);
            aValue.Add(1.5, () => "1" + minText);
            aValue.Add(45, () => string.Format("{0}" + minText1, Math.Round(TotalMinutes)));
            aValue.Add(90, () => "1" + hourText);
            aValue.Add(1440, () => string.Format("{0}" + hourText1, Math.Round(Math.Abs(oSpan.TotalHours)))); // 60 * 24
            aValue.Add(2880, () => "1" + dayText); // 60 * 48
            aValue.Add(43200, () => string.Format("{0}" + dayText1, Math.Floor(Math.Abs(oSpan.TotalDays)))); // 60 * 24 * 30
            aValue.Add(86400, () => "1" + monthText); // 60 * 24 * 60
            aValue.Add(525600, () => string.Format("{0}" + monthText1, Math.Floor(Math.Abs(oSpan.TotalDays / 30)))); // 60 * 24 * 365 
            aValue.Add(1051200, () => "1" + yearText); // 60 * 24 * 365 * 2
            aValue.Add(double.MaxValue, () => string.Format("{0}" + yearText1, Math.Floor(Math.Abs(oSpan.TotalDays / 365))));

            return aValue.First(n => TotalMinutes < n.Key).Value.Invoke() + Suffix;
        }

        public static DateTime convertDate(string strDate)
        {
            DateTime date;
            try
            {
                string convertedDate = String.Empty;
                if (DateTime.TryParseExact(strDate, "dd/MM/yyyy", null, DateTimeStyles.None, out date))
                {
                    convertedDate = date.ToString("MM/dd/yyyy");
                }
                date = DateTime.Parse(convertedDate);
            }
            catch
            {
                date = DateTime.Parse(strDate);
            }
            return date;
        }

        //public static bool CheckPermission(string userId, string functionId, string action)
        //{
        //    NEXMI.NMPermissionsBL PermissionBL = new NEXMI.NMPermissionsBL();
        //    NEXMI.NMPermissionsWSI PermissionWSI;
        //    Boolean isOK = false;
        //    if (userId == "U0001")
        //    {
        //        isOK = true;
        //    }
        //    else
        //    {
        //        PermissionWSI = new NEXMI.NMPermissionsWSI();
        //        PermissionWSI.Mode = "SEL_OBJ";
        //        PermissionWSI.UserId = userId;
        //        PermissionWSI.FunctionId = functionId;
        //        PermissionWSI = PermissionBL.callSingleBL(PermissionWSI);
        //        if (action == "")
        //        {
        //            if (PermissionWSI.PSelect == "Y" || PermissionWSI.PInsert == "Y" || PermissionWSI.PUpdate == "Y" || PermissionWSI.PDelete == "Y"
        //                || PermissionWSI.Approval == "Y" || PermissionWSI.ViewAll == "Y" || PermissionWSI.Calculate == "Y" || PermissionWSI.History == "Y")
        //            {
        //                isOK = true;
        //            }
        //        }
        //        else
        //        {
        //            switch (action)
        //            {
        //                case "Select": if (PermissionWSI.PSelect == "Y") isOK = true; break;
        //                case "Insert": if (PermissionWSI.PInsert == "Y") isOK = true; break;
        //                case "Update": if (PermissionWSI.PUpdate == "Y") isOK = true; break;
        //                case "Delete": if (PermissionWSI.PDelete == "Y") isOK = true; break;
        //                case "Approval": if (PermissionWSI.Approval == "Y") isOK = true; break;
        //                case "ViewAll": if (PermissionWSI.ViewAll == "Y") isOK = true; break;
        //                case "Calculate": if (PermissionWSI.Calculate == "Y") isOK = true; break;
        //                case "History": if (PermissionWSI.History == "Y") isOK = true; break;
        //            }
        //        }
        //    }
        //    return isOK;
        //}

        //public static NMPermissionsWSI CheckPermission(string userId, string functionId)
        //{
        //    NMPermissionsBL PermissionBL = new NMPermissionsBL();
        //    NMPermissionsWSI PermissionWSI = new NMPermissionsWSI();
        //    PermissionWSI.Mode = "SEL_OBJ";
        //    PermissionWSI.UserId = userId;
        //    PermissionWSI.FunctionId = functionId;
        //    PermissionWSI = PermissionBL.callSingleBL(PermissionWSI);
        //    return PermissionWSI;
        //}

        public static string GetAllLanguageId()
        {
            ISession session = SessionFactory.GetNewSession();
            IQuery query = session.CreateSQLQuery("select O.Id + ';' from Types O where O.ObjectName = 'Languages' FOR XML Path('')");
            string data = "";
            try
            {
                data = query.UniqueResult().ToString();
            }
            catch { }
            return data;
        }

        public static string GetLanguageDefault()
        {
            ISession session = SessionFactory.GetNewSession();
            IQuery query = session.CreateSQLQuery("select O.Id from Types O where O.ObjectName = 'Languages' and O.IsDefault = 'true'");
            string data = "";
            try
            {
                data = query.UniqueResult().ToString();
            }
            catch { }
            return data;
        }

        public static Translations GetTranslation(string ownerId, string languageId)
        {
            ISession session = SessionFactory.GetNewSession();
            TranslationsAccesser Accesser = new TranslationsAccesser(session);
            return Accesser.GetAllTranslationsByID(languageId, ownerId, true);
        }

        public static string SetXmlValue(string path, string key, string value)
        {
            string error = "";
            try
            {
                System.Data.DataSet tmpDs = new System.Data.DataSet();
                tmpDs.ReadXml(path);
                try
                {
                    string temp = tmpDs.Tables[0].Rows[0][key].ToString();
                }
                catch
                {
                    DataColumn col = new DataColumn(key);
                    tmpDs.Tables[0].Columns.Add(col);
                }

                if (value == null)
                {
                    int oldValue = 0;
                    try { oldValue = Int32.Parse(tmpDs.Tables[0].Rows[0][key].ToString()); }
                    catch { }
                    value = (oldValue + 1).ToString();
                }

                tmpDs.Tables[0].Rows[0][key] = value;

                tmpDs.WriteXml(path);
            }
            catch (Exception ex)
            {
                error = "Lỗi!\n" + ex.Message;
            }
            return error;
        }

        public static string GetXmlValue(string path, string key)
        {
            string value = "";
            try
            {
                System.Data.DataSet tmpDs = new System.Data.DataSet();
                tmpDs.ReadXml(path);
                value = tmpDs.Tables[0].Rows[0][key].ToString();
            }
            catch
            {
                //Response.Write("<script>alert('This is Alert');</script>");
            }
            return value;
        }

        public static double GetProductQuantity(string productId, string stockId)
        {
            ProductInStocksAccesser Accesser = new ProductInStocksAccesser(SessionFactory.GetNewSession());
            ProductInStocks obj = Accesser.GetAllProductinstocksByStockIdAndProductId(stockId, productId, true);
            return (obj.BeginQuantity + obj.ImportQuantity - obj.ExportQuantity - obj.BadProductInStock);
        }

        public static bool CheckExistCustomer(string id)
        {
            CustomersAccesser Accesser = new CustomersAccesser(SessionFactory.GetNewSession());
            Customers obj = Accesser.GetAllCustomersByID(id, true);
            if (obj != null)
                return true;
            return false;
        }

        public static string GetStatusName(string id, string lang)
        {
            ISession session = SessionFactory.GetNewSession();
            IQuery query;
            
            if(lang == "vi")
                query = session.CreateSQLQuery("select O.Name from Statuses as O where O.Id = :id");
            else if (lang == "kr")
                query = session.CreateSQLQuery("select O.Korea from Statuses as O where O.Id = :id");
            else
                query = session.CreateSQLQuery("select O.English from Statuses as O where O.Id = :id");
                
            query.SetString("id", id);
            string name = "";
            try
            {
                name = query.UniqueResult().ToString();
            }
            catch { }
            return name;
        }

        public static void SaveMessage(ISession session, string owner, string name, string content, string createdBy, string categoryId, string groupId, string typeId)
        {
            if (NMCommon.GetSetting("LOG_INFO_CHANGE"))
            {
                AutomaticValuesAccesser AutoIDAccesser = new AutomaticValuesAccesser(session);
                MessagesAccesser Accesser = new MessagesAccesser(session);
                Messages Message = new Messages();
                Message.MessageId = AutoIDAccesser.AutoGenerateId("Messages");
                Message.MessageName = name;
                Message.Owner = owner;
                Message.Description = content;
                Message.CreatedDate = DateTime.Now;
                if (!String.IsNullOrEmpty(createdBy))
                    Message.CreatedBy = createdBy;
                if (!String.IsNullOrEmpty(categoryId))
                    Message.CategoryId = categoryId;
                if (!String.IsNullOrEmpty(groupId))
                    Message.GroupId = groupId;
                if (!String.IsNullOrEmpty(typeId))
                    Message.TypeId = typeId;
                Accesser.InsertMessage(Message);
            }
        }

        public static string SaveMessage(String ownerId, String name, String msgContent, string createdBy)
        {
            string err = "";
            if (NMCommon.GetSetting("LOG_INFO_CHANGE"))
            {
                NMMessagesBL BL = new NMMessagesBL();
                NMMessagesWSI WSI = new NMMessagesWSI();
                WSI.Message = new Messages();

                WSI.Mode = "SAV_OBJ";
                WSI.Message.MessageName = name;
                WSI.Message.Owner = ownerId;
                WSI.Message.Description = msgContent;
                WSI.Message.TypeId = NMConstant.MessageTypes.SysLog;
                WSI.ActionBy = createdBy;
                err = BL.callSingleBL(WSI).WsiError;
            }
            return err;
        }

        public static int GetNumberOfSI(string reference)
        {
            ISession session = SessionFactory.GetNewSession();
            IQuery query = session.CreateSQLQuery("select COUNT(O.InvoiceId) from SalesInvoices O where O.Reference = :reference");
            query.SetString("reference", reference);
            int rs = 0;
            try
            {
                rs = (int)query.UniqueResult();
            }
            catch { }
            return rs;
        }

        public static int GetNumberOfSIDetails(string reference)
        {
            ISession session = SessionFactory.GetNewSession();
            IQuery query = session.CreateSQLQuery("select COUNT(SIDetail.OrdinalNumber) from SalesInvoiceDetails SIDetail, SalesInvoices SI where SI.InvoiceId = SIDetail.InvoiceId and SI.Reference = :reference");
            query.SetString("reference", reference);
            int rs = 0;
            try
            {
                rs = (int)query.UniqueResult();
            }
            catch { }
            return rs;
        }

        public static List<Settings> GetSettings()
        {
            ISession session = SessionFactory.GetNewSession();
            SettingsAccesser Accesser = new SettingsAccesser(session);
            return Accesser.GetAllSettings(true).ToList();
        }

        public static int GetSettingValue(string id)
        {
            ISession session = SessionFactory.GetNewSession();
            SettingsAccesser Accesser = new SettingsAccesser(session);
            Settings obj = Accesser.GetAllSettingsByID(id, true);

            //List<Settings> objs = GetSettings();
            //NEXMI.Settings obj = null;
            //if (objs != null)
            //    obj = objs.Select(i => i).Where(i => i.Id == id).FirstOrDefault();
            
            if (obj != null)
                return obj.Value;
            else
                return 0;
        }

        public static bool GetSetting(string id)
        {
            ISession session = SessionFactory.GetNewSession();
            SettingsAccesser Accesser = new SettingsAccesser(session);
            Settings obj = Accesser.GetAllSettingsByID(id, true);

            //List<Settings> objs = GetSettings();
            //NEXMI.Settings obj = null;
            //if (objs != null)
            //    obj = objs.Select(i => i).Where(i => i.Id == id).FirstOrDefault();

            if (obj != null)
                return (obj.Value > 0)? true: false;
            else
                return false;
        }

        public static int GetStartIndex(string columnName, string tableName, string sortField, string sortOrder, string customerQuery, string currentId)
        {
            ISession session = SessionFactory.GetNewSession();
            IQuery query = session.CreateSQLQuery(";WITH cte AS (SELECT " + columnName + ", ROW_NUMBER() OVER (ORDER BY " + sortField + " " + sortOrder + ") AS RowIndex FROM " + tableName + " " + customerQuery + ") SELECT A.RowIndex FROM cte AS A where A." + columnName + " = :currentId");
            query.SetString("currentId", currentId);
            int nextId = 1;
            try
            {
                nextId = (int)query.UniqueResult();
            }
            catch { }
            return nextId;
        }

        public static void UpdateStock(ISession Session, ExportDetails oldObj, ExportDetails newObj, string typeId)
        {
            ProductInStocksAccesser PISAccesser = new ProductInStocksAccesser(Session);
            ProductInStocks PIS;
            double beginQty = 0;
            //trường hợp nhập mới
            if ((oldObj == null) && (newObj != null))
            {
                PIS = PISAccesser.GetAllProductinstocksByStockIdAndProductId(newObj.StockId, newObj.ProductId, true);
                if (PIS != null)
                {
                    beginQty = PIS.BeginQuantity;
                    PIS.ExportQuantity += newObj.Quantity;
                    //PIS.GoodProductInStock = PIS.GoodProductInStock - newObj.Quantity;
                    PISAccesser.UpdateProductinstocks(PIS);
                }
                else
                {
                    PIS = new ProductInStocks();
                    PIS.StockId = newObj.StockId;
                    PIS.ProductId = newObj.ProductId;
                    PIS.ExportQuantity = newObj.Quantity;
                    //PIS.GoodProductInStock -= newObj.Quantity;
                    PIS.CreatedDate = DateTime.Now;
                    PIS.CreatedBy = newObj.CreatedBy;
                    PISAccesser.InsertProductinstocks(PIS);
                }
            }
            ////trường hợp điều chỉnh
            //else if ((oldObj != null) && (newObj != null))
            //{
            //    PIS = PISAccesser.GetAllProductinstocksByStockIdAndProductId(newObj.StockId, newObj.ProductId, true);
            //    if (PIS != null)
            //    {
            //        PIS.GoodProductInStock = PIS.GoodProductInStock - newObj.Quantity + oldObj.Quantity;
            //        //PIS.BadProductInStock = PIS.BadProductInStock - newObj.b
            //        PISAccesser.UpdateProductinstocks(PIS);
            //    }
            //}
            ////truong hop xoa item p/x, tang lai so luong ton kho
            //else if ((oldObj != null) && (newObj == null))
            //{
            //    PIS = PISAccesser.GetAllProductinstocksByStockIdAndProductId(oldObj.StockId, oldObj.ProductId, true);
            //    if (PIS != null)
            //    {
            //        PIS.GoodProductInStock = PIS.GoodProductInStock + oldObj.Quantity;
            //        //PIS.BadProductInStock = PIS.BadProductInStock - newObj.b
            //        PISAccesser.UpdateProductinstocks(PIS);
            //    }
            //}
            if (newObj != null)
            {
                MonthlyInventoryControlAccesser MICAccesser = new MonthlyInventoryControlAccesser(Session);
                MonthlyInventoryControl MIC = MICAccesser.GetAllMonthlyinventorycontrolByStockIdAndProductId(newObj.StockId, newObj.ProductId, false);
                if (MIC != null)
                {
                    if (typeId == NMConstant.ExportType.Destroy)
                        MIC.OutQuantity += newObj.Quantity;
                    else if (typeId == NMConstant.ExportType.Transfer)
                        MIC.ExportQuantity += newObj.Quantity;
                    else
                        MIC.SalesQuantity += newObj.Quantity;
                    MICAccesser.UpdateMonthlyinventorycontrol(MIC);
                }
                else
                {
                    MIC = new MonthlyInventoryControl();
                    MIC.StockId = newObj.StockId;
                    MIC.ProductId = newObj.ProductId;
                    MIC.BeginQuantity = beginQty;
                    if (typeId == NMConstant.ExportType.Destroy)
                        MIC.OutQuantity += newObj.Quantity;
                    else if (typeId == NMConstant.ExportType.Transfer)
                        MIC.ExportQuantity += newObj.Quantity;
                    else
                        MIC.SalesQuantity += newObj.Quantity;
                    MIC.CreatedDate = DateTime.Now;
                    //MIC.CreatedBy = PIS.CreatedBy;
                    MICAccesser.InsertMonthlyinventorycontrol(MIC);
                }
            }
        }

        public static void UpdateStock(ISession Session, ImportDetails oldObj, ImportDetails newObj)
        {
            ProductInStocksAccesser PISAccesser = new ProductInStocksAccesser(Session);
            ProductInStocks PIS;
            double beginQty = 0;
            // nhập sản phẩm mới
            if ((oldObj == null) && (newObj != null))
            {
                PIS = PISAccesser.GetAllProductinstocksByStockIdAndProductId(newObj.StockId, newObj.ProductId, true);
                if (PIS != null)
                {
                    beginQty = PIS.BeginQuantity;
                    PIS.ImportQuantity += newObj.GoodQuantity;
                    //PIS.GoodProductInStock = PIS.GoodProductInStock + newObj.GoodQuantity;
                    PIS.BadProductInStock = PIS.BadProductInStock + newObj.BadQuantity;
                    PISAccesser.UpdateProductinstocks(PIS);
                }
                else
                {
                    PIS = new ProductInStocks();
                    PIS.StockId = newObj.StockId;
                    PIS.ProductId = newObj.ProductId;
                    try { PIS.ImportQuantity = newObj.GoodQuantity; }
                    catch { }
                    try { PIS.BadProductInStock = newObj.BadQuantity; }
                    catch { }
                    PIS.CreatedDate = DateTime.Now;
                    PIS.CreatedBy = newObj.CreatedBy;
                    PIS.ModifiedDate = DateTime.Now;
                    PIS.ModifiedBy = newObj.CreatedBy;
                    PISAccesser.InsertProductinstocks(PIS);
                }
            }
            //else if ((oldObj != null) && (newObj != null))  //sửa phiếu nhập => điều chỉnh số lượng
            //{
            //    PIS = PISAccesser.GetAllProductinstocksByStockIdAndProductId(oldObj.StockId, oldObj.ProductId, true);
            //    if (PIS != null)
            //    {
            //        PIS.GoodProductInStock = PIS.GoodProductInStock + newObj.GoodQuantity - oldObj.GoodQuantity;
            //        PIS.BadProductInStock = PIS.BadProductInStock + newObj.BadQuantity - oldObj.BadQuantity;
            //        PISAccesser.UpdateProductinstocks(PIS);
            //    }
            //}
            //else if ((oldObj != null) && (newObj == null))  //xóa item của phiếu nhập => giảm số lượng
            //{
            //    PIS = PISAccesser.GetAllProductinstocksByStockIdAndProductId(oldObj.StockId, oldObj.ProductId, true);
            //    if (PIS != null)
            //    {
            //        PIS.GoodProductInStock = PIS.GoodProductInStock - oldObj.GoodQuantity;
            //        PIS.BadProductInStock = PIS.BadProductInStock - oldObj.BadQuantity;
            //        PISAccesser.UpdateProductinstocks(PIS);
            //    }
            //}
            if (newObj != null)
            {
                MonthlyInventoryControlAccesser MICAccesser = new MonthlyInventoryControlAccesser(Session);
                MonthlyInventoryControl MIC = MICAccesser.GetAllMonthlyinventorycontrolByStockIdAndProductId(newObj.StockId, newObj.ProductId, false);
                if (MIC != null)
                {
                    MIC.ImportQuantity += newObj.GoodQuantity;
                    MICAccesser.UpdateMonthlyinventorycontrol(MIC);
                }
                else
                {
                    MIC = new MonthlyInventoryControl();
                    MIC.StockId = newObj.StockId;
                    MIC.ProductId = newObj.ProductId;
                    MIC.BeginQuantity = beginQty;
                    MIC.ImportQuantity = newObj.GoodQuantity;
                    MIC.CreatedDate = DateTime.Now;
                    //MIC.CreatedBy = PIS.CreatedBy;
                    MICAccesser.InsertMonthlyinventorycontrol(MIC);
                }
            }
        }

        public static void UpdateInvoiceStatus(ISession Session, string invoiceId, string actionBy)
        {
            SalesInvoicesAccesser Accesser = new SalesInvoicesAccesser(Session);
            SalesInvoices Invoice = Accesser.GetAllSalesinvoicesByID(invoiceId, true);
            if (Invoice != null && Invoice.InvoiceStatus != NMConstant.SIStatuses.Paid)
            {
                if (Invoice.DetailsList.Sum(i=>i.TotalAmount) <= NMCommon.GetAmountOfSI(Invoice.InvoiceId))
                {
                    Invoice.InvoiceStatus = NMConstant.SIStatuses.Paid;
                    Accesser.UpdateSalesinvoices(Invoice);
                    SaveMessage(Session, invoiceId, "thay đổi trạng thái", "Trạng thái: Đã thanh toán", actionBy, null, null, NMConstant.MessageTypes.SysLog);
                }
            }
        }

        public static bool UpdateLeaseOrderMaintainDate(string orderId, string maintainDate, string actionBy)
        {
            ISession Session = SessionFactory.GetNewSession();
            ITransaction tx = Session.BeginTransaction();
            SalesOrdersAccesser Accesser = new SalesOrdersAccesser(Session);
            SalesOrders Order = Accesser.GetAllOrdersByID(orderId, true);
            if (Order != null )
            {
                try
                {
                    DateTime newD = DateTime.Parse(maintainDate);
                    if (Order.MaintainDate != newD)
                    {
                        Order.MaintainDate = newD;
                        Accesser.UpdateOrders(Order);
                        SaveMessage(Session, orderId, "thay đổi ngày vệ sinh", "", actionBy, null, null, NMConstant.MessageTypes.SysLog);
                        tx.Commit();
                    }
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public static double GetAmountOfSI(string id)
        {
            try
            {
                ISession session = SessionFactory.GetNewSession();
                IQuery query = session.CreateSQLQuery("select SUM(R.ReceiptAmount) from Receipts R, SalesInvoices SI where SI.InvoiceId = R.InvoiceId and SI.InvoiceId = :x");
                query.SetString("x", id);
                IQuery query1 = session.CreateSQLQuery("select SUM(P.PaymentAmount) from Payments P, SalesInvoices SI where SI.InvoiceId = P.InvoiceId and SI.InvoiceId = :x");
                query1.SetString("x", id);
                double value = 0, value1 = 0;
                try
                {
                    value = double.Parse(query.UniqueResult().ToString());
                }
                catch { }
                try
                {
                    value1 = double.Parse(query1.UniqueResult().ToString());
                }
                catch { }
                return value - value1;
            }
            catch
            {
                return 0;
            }
        }
        
        public static string ReadNum(double number)
        {
            string s = number.ToString("#");
            string[] so = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] hang = new string[] { "", "nghìn", "triệu", "tỷ" };
            int i, j, donvi, chuc, tram;
            string str = " ";
            bool booAm = false;
            decimal decS = 0;
            //Tung addnew
            try
            {
                decS = Convert.ToDecimal(s.ToString());
            }
            catch
            {

            }
            if (decS < 0)
            {
                decS = -decS;
                s = decS.ToString();
                booAm = true;
            }
            i = s.Length;
            if (i == 0)
                str = so[0] + str;
            else
            {
                j = 0;
                while (i > 0)
                {
                    donvi = Convert.ToInt32(s.Substring(i - 1, 1));
                    i--;
                    if (i > 0)
                        chuc = Convert.ToInt32(s.Substring(i - 1, 1));
                    else
                        chuc = -1;
                    i--;
                    if (i > 0)
                        tram = Convert.ToInt32(s.Substring(i - 1, 1));
                    else
                        tram = -1;
                    i--;
                    if ((donvi > 0) || (chuc > 0) || (tram > 0) || (j == 3))
                        str = hang[j] + " " + str;
                    j++;
                    if (j > 3) j = 1;
                    if ((donvi == 1) && (chuc > 1))
                        str = "một " + str;
                    else
                    {
                        if ((donvi == 5) && (chuc > 0))
                            str = "lăm " + str;
                        else if (donvi > 0)
                            str = so[donvi] + " " + str;
                    }
                    if (chuc < 0)
                        break;
                    else
                    {
                        if ((chuc == 0) && (donvi > 0)) str = "lẻ " + str;
                        if (chuc == 1) str = "mười " + str;
                        if (chuc > 1) str = so[chuc] + " mươi " + str;
                    }
                    if (tram < 0) break;
                    else
                    {
                        if ((tram > 0) || (chuc > 0) || (donvi > 0)) str = so[tram] + " trăm " + str;
                    }
                    str = "" + str;
                }
            }
            if (booAm) str = "Âm " + str;
            return str + "đồng";
        }

        public static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public static string GetInterface(string owner, string lang)
        {
            ISession session = SessionFactory.GetNewSession();
            NEXMI.InterfaceAccesser Accesser = new NEXMI.InterfaceAccesser(session);

            Interface Interface = Accesser.GetAllInterfaceByID(owner, true);

            string result = "";
            if(lang == "vi")
                result = Interface.Vietnamese;
            else if (lang == "en")
                result = Interface.English;
            else if (lang == "kr")
                result = Interface.Korea;

            return result;
        }

        // lay danh sach Class của Document
        public static List<string> GetDocumentClass()
        {
            ISession session = SessionFactory.GetNewSession();
            DocumentsAccesser Accesser = new DocumentsAccesser(session);
            //IQuery query = session.CreateSQLQuery("select d.Class from Documents d group by Class");
            IQuery query = session.CreateSQLQuery("select distinct(d.Class) from Documents d");
            
            List<string> result = query.List<string>().ToList();

            return result;
        }

        // lay danh sach Entity của Document
        public static List<string> GetDocumentEntity()
        {
            ISession session = SessionFactory.GetNewSession();
            DocumentsAccesser Accesser = new DocumentsAccesser(session);
            //IQuery query = session.CreateSQLQuery("select d.Class from Documents d group by Class");
            IQuery query = session.CreateSQLQuery("select d.Entity from Documents d group by Entity");

            List<string> result = query.List<string>().ToList();

            return result;
        }

        // tao Payment cho phieu thu tien thuong cua NCC
        public static string CreatePaymentFromReceipt(NMReceiptsWSI WSI)
        {
            string error = "";
            if (WSI.Receipt.ReceiptStatus == NMConstant.ReceiptStatuses.Done)
            {
                if (WSI.Receipt.ReceiptTypeId == "7111" | WSI.Receipt.ReceiptTypeId == "7112")
                {
                    NMPaymentsBL pBL = new NMPaymentsBL();
                    NMPaymentsWSI pWSI = new NMPaymentsWSI();
                    pWSI.Mode = "SRC_OBJ";
                    pWSI.Payment.Reference = WSI.Receipt.ReceiptId;
                    List<NMPaymentsWSI> lst = pBL.callListBL(pWSI);
                    if (lst.Count > 0)
                        pWSI.Payment.PaymentId = lst[0].Payment.PaymentId;

                    pWSI.Mode = "SAV_OBJ";

                    pWSI.Payment.PaymentDate = WSI.Receipt.ReceiptDate;
                    pWSI.Payment.SupplierId = WSI.Receipt.CustomerId;
                    pWSI.Payment.Reference = WSI.Receipt.ReceiptId;

                    pWSI.Payment.Amount = WSI.Receipt.Amount;
                    pWSI.Payment.CurrencyId = "VND";
                    pWSI.Payment.ExchangeRate = 1;

                    pWSI.Payment.DescriptionInVietnamese = WSI.Receipt.DescriptionInVietnamese + " (cấn trừ công nợ)";
                    pWSI.Payment.PaymentTypeId = "331";
                    pWSI.Payment.PaymentMethodId = WSI.Receipt.PaymentMethodId;
                    pWSI.Payment.PaymentBankAccount = WSI.Receipt.ReceivedBankAccount;

                    pWSI.Payment.PaymentStatus = NMConstant.PaymentStatuses.Done;
                    pWSI.ActionBy = WSI.ActionBy;
                    pWSI.Payment.StockId = WSI.Receipt.StockId;

                    pWSI = pBL.callSingleBL(pWSI);
                    error = pWSI.WsiError;
                }
            }
            return error;
        }

        // tao phieu thu cho phieu chi tien thuong cho khach hang
        public static string CreateReceiptFromPayment(NMPaymentsWSI wsi)
        {
            string error = "";
            if (wsi.Payment.PaymentStatus == NMConstant.PaymentStatuses.Done)
            {
                if (wsi.Payment.PaymentTypeId == "8111" | wsi.Payment.PaymentTypeId == "8112")
                {
                    NMReceiptsBL rBL = new NMReceiptsBL();
                    NMReceiptsWSI rWSI = new NMReceiptsWSI();
                    rWSI.Mode = "SRC_OBJ";
                    rWSI.Receipt.Reference = wsi.Payment.PaymentId;
                    List<NMReceiptsWSI> lst = rBL.callListBL(rWSI);

                    if (lst.Count > 0)
                        rWSI.Receipt.ReceiptId = lst[0].Receipt.ReceiptId;

                    rWSI.Mode = "SAV_OBJ";

                    rWSI.Receipt.ReceiptDate = wsi.Payment.PaymentDate;
                    rWSI.Receipt.CustomerId = wsi.Payment.SupplierId;

                    rWSI.Receipt.Amount = wsi.Payment.Amount;
                    rWSI.Receipt.CurrencyId = "VND";
                    rWSI.Receipt.ExchangeRate = 1;

                    rWSI.Receipt.DescriptionInVietnamese = wsi.Payment.DescriptionInVietnamese + " (cấn trừ công nợ)";
                    rWSI.Receipt.Reference = wsi.Payment.PaymentId;
                    rWSI.Receipt.ReceiptTypeId = "131";
                    rWSI.Receipt.PaymentMethodId = wsi.Payment.PaymentMethodId;
                    rWSI.Receipt.ReceivedBankAccount = wsi.Payment.PaymentBankAccount;

                    rWSI.Receipt.ReceiptStatus = NMConstant.ReceiptStatuses.Done;
                    rWSI.ActionBy = wsi.ActionBy;
                    rWSI.Receipt.StockId = wsi.Payment.StockId;

                    rWSI = rBL.callSingleBL(rWSI);
                }
            }
            return error;
        }

        //  chuyen doi chuoi có dấu thành chuoi khong dau
        public static string ConvertToUnSign(string text)
        {
            for (int i = 33; i < 48; i++)
            {
                text = text.Replace(((char)i).ToString(), "");
            }

            for (int i = 58; i < 65; i++)
            {
                text = text.Replace(((char)i).ToString(), "");
            }

            for (int i = 91; i < 97; i++)
            {
                text = text.Replace(((char)i).ToString(), "");
            }
            for (int i = 123; i < 127; i++)
            {
                text = text.Replace(((char)i).ToString(), "");
            }
            text = text.Replace(" ", "-");
            Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
            string strFormD = text.Normalize(System.Text.NormalizationForm.FormD);
            return regex.Replace(strFormD, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
    }
    
    public class MoneyToString
    {
        public static string SetValue(Decimal number)
        {
            string s = number.ToString("#");
            string[] so = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] hang = new string[] { "", "nghìn", "triệu", "tỷ" };
            int i, j, donvi, chuc, tram;
            string str = " ";
            bool booAm = false;
            decimal decS = 0;
            //Tung addnew
            try
            {
                decS = Convert.ToDecimal(s.ToString());
            }
            catch
            {

            }
            if (decS < 0)
            {
                decS = -decS;
                s = decS.ToString();
                booAm = true;
            }
            i = s.Length;
            if (i == 0)
                str = so[0] + str;
            else
            {
                j = 0;
                while (i > 0)
                {
                    donvi = Convert.ToInt32(s.Substring(i - 1, 1));
                    i--;
                    if (i > 0)
                        chuc = Convert.ToInt32(s.Substring(i - 1, 1));
                    else
                        chuc = -1;
                    i--;
                    if (i > 0)
                        tram = Convert.ToInt32(s.Substring(i - 1, 1));
                    else
                        tram = -1;
                    i--;
                    if ((donvi > 0) || (chuc > 0) || (tram > 0) || (j == 3))
                        str = hang[j] + " " + str;
                    j++;
                    if (j > 3) j = 1;
                    if ((donvi == 1) && (chuc > 1))
                        str = "một " + str;
                    else
                    {
                        if ((donvi == 5) && (chuc > 0))
                            str = "lăm " + str;
                        else if (donvi > 0)
                            str = so[donvi] + " " + str;
                    }
                    if (chuc < 0)
                        break;
                    else
                    {
                        if ((chuc == 0) && (donvi > 0)) str = "lẻ " + str;
                        if (chuc == 1) str = "mười " + str;
                        if (chuc > 1) str = so[chuc] + " mươi " + str;
                    }
                    if (tram < 0) break;
                    else
                    {
                        if ((tram > 0) || (chuc > 0) || (donvi > 0)) str = so[tram] + " trăm " + str;
                    }
                    str = "" + str;
                }
            }
            if (booAm) str = "Âm " + str;
            return str + "đồng";
        }

        public static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

    }

}
