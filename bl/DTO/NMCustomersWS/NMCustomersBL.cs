
// NMCustomersBL.cs

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
using System.Data.SqlTypes;

namespace NEXMI
{
    public class NMCustomersBL
    {
        private readonly ISession Session = SessionFactory.GetNewSession();

        public NMCustomersBL()
        {
            
        }

        public NMCustomersWSI callSingleBL(NMCustomersWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SAV_OBJ":
                    return SaveObject(wsi);
                case "SEL_OBJ":
                    return SelectObject(wsi);
                case "DEL_OBJ":
                    return DeleteObject(wsi);
                default:
                    return wsi;
            }
        }

        public List<NMCustomersWSI> callListBL(NMCustomersWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMCustomersWSI>();
            }
        }

        public NMCustomersWSI SelectObject(NMCustomersWSI wsi)
        {
            CustomersAccesser Accesser = new CustomersAccesser(Session);
            StocksAccesser StockAccesser = new StocksAccesser(Session);
            Customers obj;
            obj = Accesser.GetAllCustomersByID(wsi.Customer.CustomerId, true);
            if (obj != null)
            {
                wsi.Customer = obj;
                wsi.ManagedBy = Accesser.GetAllCustomersByID(obj.ManagedBy, true);
                //wsi.Details = obj.CustomerdetailsList.Cast<NMCustomerDetails>().ToList();
                wsi.ContactPersons = Accesser.GetAllCustomersByParentId(obj.CustomerId, true);
                NMAreasBL AreaBL = new NMAreasBL();
                wsi.AreaWSI = new NMAreasWSI();
                wsi.AreaWSI.Mode = "SEL_OBJ";
                wsi.AreaWSI.Area = new Areas();
                wsi.AreaWSI.Area.Id = obj.AreaId;
                wsi.AreaWSI = AreaBL.callSingleBL(wsi.AreaWSI);
                NMSalesInvoicesBL SIBL = new NMSalesInvoicesBL();
                NMSalesInvoicesWSI SIWSI = new NMSalesInvoicesWSI();
                SIWSI.Mode = "SRC_OBJ";
                SIWSI.Invoice = new SalesInvoices();
                SIWSI.Invoice.CustomerId = obj.CustomerId;
                wsi.SalesInvoicesWSI = SIBL.callListBL(SIWSI);
                NMPurchaseInvoicesBL PIBL = new NMPurchaseInvoicesBL();
                NMPurchaseInvoicesWSI PIWSI = new NMPurchaseInvoicesWSI();
                PIWSI.Mode = "SRC_OBJ";
                PIWSI.Invoice = new PurchaseInvoices();
                PIWSI.Invoice.SupplierId = obj.CustomerId;
                wsi.PurchaseInvoicesWSI = PIBL.callListBL(PIWSI);
                NMStocksBL StockBL = new NMStocksBL();
                NMStocksWSI StockWSI = new NMStocksWSI();
                StockWSI.Mode = "SEL_OBJ";
                StockWSI.Id = obj.StockId;
                wsi.DefaultStock = StockBL.callSingleBL(StockWSI);
                wsi.WsiError = "";
            }
            else
            {
                wsi.Customer = null;
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public NMCustomersWSI SaveObject(NMCustomersWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                CustomersAccesser Accesser = new CustomersAccesser(Session);
                IList<Customers> ContactPersonsOld = null;
                Customers obj = Accesser.GetAllCustomersByID(wsi.Customer.CustomerId, true);
                if (obj != null)
                {
                    ContactPersonsOld = Accesser.GetAllCustomersByParentId(wsi.Customer.CustomerId, true);
                    //Session.Evict(obj);
                    if (wsi.Customer.Avatar == null)
                    {
                        wsi.Customer.Avatar = obj.Avatar;
                    }
                    wsi.Customer.CreatedBy = obj.CreatedBy;
                    wsi.Customer.CreatedDate = obj.CreatedDate;
                    wsi.Customer.ModifiedBy = wsi.ActionBy;
                    wsi.Customer.ModifiedDate = DateTime.Now;
                    Accesser.UpdateCustomers(wsi.Customer);
                    String rs = wsi.CompareTo(obj);
                    if (rs != "")
                        NMMessagesBL.SaveMessage(Session, wsi.Customer.CustomerId, "cập nhật thông tin", rs, wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
                }
                else
                {
                    string typeName = "";
                    if (wsi.Customer.GroupId == NMConstant.CustomerGroups.Customer)
                        typeName = "Customers";
                    else if (wsi.Customer.GroupId == NMConstant.CustomerGroups.Supplier)
                        typeName = "Suppliers";
                    else if (wsi.Customer.GroupId == NMConstant.CustomerGroups.Manufacturer)
                        typeName = "Manufacturers";
                    else if (wsi.Customer.GroupId == NMConstant.CustomerGroups.User)
                        typeName = "Users";

                    wsi.Customer.CustomerId = AutomaticValueAccesser.AutoGenerateId(typeName);
                    wsi.Customer.CreatedBy = wsi.ActionBy;
                    wsi.Customer.CreatedDate = DateTime.Now;
                    wsi.Customer.ModifiedBy = wsi.ActionBy;
                    wsi.Customer.ModifiedDate = DateTime.Now;
                    Accesser.InsertCustomers(wsi.Customer);
                    NMMessagesBL.SaveMessage(Session, wsi.Customer.CustomerId, "khởi tạo đối tượng", "", wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
                }

                #region Nguoi lien he
                
                if (wsi.ContactPersons != null)
                {
                    foreach (Customers Item in wsi.ContactPersons)
                    {
                        obj = Accesser.GetAllCustomersByID(Item.CustomerId, false);
                        if (obj != null)
                        {
                            Session.Evict(obj);
                            Item.ParentId = wsi.Customer.CustomerId;
                            Item.GroupId = wsi.Customer.GroupId;
                            Item.CustomerTypeId = NMConstant.CustomerTypes.Individual;
                            Item.ManagedBy = wsi.Customer.ManagedBy;

                            Item.CreatedBy = obj.CreatedBy;
                            Item.CreatedDate = obj.CreatedDate;
                            Item.ModifiedBy = wsi.ActionBy;
                            Item.ModifiedDate = DateTime.Now;

                            Accesser.UpdateCustomers(Item);
                        }
                        else
                        {
                            Item.CustomerId = AutomaticValueAccesser.AutoGenerateId("Contacts");
                            Item.ParentId = wsi.Customer.CustomerId;
                            Item.GroupId = wsi.Customer.GroupId;
                            Item.CustomerTypeId = NMConstant.CustomerTypes.Individual;
                            Item.ManagedBy = wsi.Customer.ManagedBy;

                            Item.CreatedBy = wsi.ActionBy;
                            Item.CreatedDate = DateTime.Now;
                            Item.ModifiedBy = wsi.ActionBy;
                            Item.ModifiedDate = DateTime.Now;

                            Accesser.InsertCustomers(Item);
                        }
                    }
                }
                bool flag = true;
                if (ContactPersonsOld != null)
                {
                    foreach (Customers Old in ContactPersonsOld)
                    {
                        flag = true;
                        foreach (Customers New in wsi.ContactPersons)
                        {
                            if (Old.CustomerId == New.CustomerId)
                            {
                                flag = false;
                            }
                        }
                        if (flag)
                        {
                            Accesser.DeleteCustomers(Old);
                        }
                    }
                }
                #endregion

                tx.Commit();

                //neu la User duoc tao moi => set quyen theo group user
                if (wsi.Customer.GroupId == NMConstant.CustomerGroups.User && obj == null)
                {
                    if (!string.IsNullOrEmpty(wsi.Customer.DeparmentId))
                    {
                        NMPermissionsBL PermissionBL = new NMPermissionsBL();
                        NMPermissionsWSI PermissionWSI = new NMPermissionsWSI();
                        PermissionWSI.Mode = "SRC_OBJ";
                        PermissionWSI.UserGroupId = wsi.Customer.DeparmentId;
                        List<NMPermissionsWSI> permList = PermissionBL.callListBL(PermissionWSI);

                        // By default, Set all permission of Department for User
                        foreach (NMPermissionsWSI per in permList)
                        {
                            PermissionWSI = per;
                            PermissionWSI.Mode = "SAV_OBJ";

                            PermissionWSI.Id = "";
                            PermissionWSI.UserGroupId = null;
                            PermissionWSI.UserId = wsi.Customer.CustomerId;
                            PermissionWSI.CreatedBy = wsi.ActionBy;

                            PermissionWSI = PermissionBL.callSingleBL(PermissionWSI);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return wsi;
        }

        public NMCustomersWSI DeleteObject(NMCustomersWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                CustomersAccesser Accesser = new CustomersAccesser(Session);
                Customers obj = Accesser.GetAllCustomersByID(wsi.Customer.CustomerId, true);
                if (obj == null)
                {
                    wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
                }
                else
                {
                    try
                    {
                        PermissionsAccesser PermissionsAccesser = new NEXMI.PermissionsAccesser(Session);
                        PermissionsAccesser.DeletePermissionsByUserId(obj.CustomerId);

                        Accesser.DeleteCustomers(obj);
                        tx.Commit();
                    }
                    catch
                    {
                        wsi.WsiError = "Không được xóa.";
                        tx.Rollback();
                    }
                }
            }
            catch (Exception ex)
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return wsi;
        }

        public List<NMCustomersWSI> SearchObject(NMCustomersWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (wsi.Customer != null)
            {
                if (!string.IsNullOrEmpty(wsi.Customer.ParentId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.ParentId = :ParentId";
                    ListCriteria.Add("ParentId", wsi.Customer.ParentId);
                }
                if (!string.IsNullOrEmpty(wsi.Customer.CustomerTypeId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.CustomerTypeId = :CustomerTypeId";
                    ListCriteria.Add("CustomerTypeId", wsi.Customer.CustomerTypeId);
                }
                if (!string.IsNullOrEmpty(wsi.Customer.AreaId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.AreaId = :AreaId";
                    ListCriteria.Add("AreaId", wsi.Customer.AreaId);
                }
                if (!string.IsNullOrEmpty(wsi.Customer.GroupId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.GroupId = :GroupId";
                    ListCriteria.Add("GroupId", wsi.Customer.GroupId);
                }

                if (!string.IsNullOrEmpty(wsi.Customer.DeparmentId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.DeparmentId = :DeparmentId";
                    ListCriteria.Add("DeparmentId", wsi.Customer.DeparmentId);
                }

                if (!string.IsNullOrEmpty(wsi.Customer.ManagedBy))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.ManagedBy = :ManagedBy";
                    ListCriteria.Add("ManagedBy", wsi.Customer.ManagedBy);
                }
                if (!string.IsNullOrEmpty(wsi.Customer.KindOfIndustryId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.KindOfIndustryId = :KindOfIndustryId";
                    ListCriteria.Add("KindOfIndustryId", wsi.Customer.KindOfIndustryId);
                }
                if (!string.IsNullOrEmpty(wsi.Customer.Code))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.Code = :Code";
                    ListCriteria.Add("Code", wsi.Customer.Code);
                }
                if (!string.IsNullOrEmpty(wsi.Customer.EmailAddress))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.EmailAddress = :EmailAddress";
                    ListCriteria.Add("EmailAddress", wsi.Customer.EmailAddress);
                }
                if (!string.IsNullOrEmpty(wsi.Customer.Password))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.Password = :Password";
                    ListCriteria.Add("Password", wsi.Customer.Password);
                }
                if (wsi.Customer.DateOfBirth != null)
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.DateOfBirth = :DateOfBirth";
                    ListCriteria.Add("DateOfBirth", wsi.Customer.DateOfBirth);
                }
                if (wsi.Customer.Gender != null)
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.Gender = :Gender";
                    ListCriteria.Add("Gender", wsi.Customer.Gender);
                }
                if (NMCommon.GetSetting(NMConstant.Settings.SHOW_COMPANY_ONLY))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.IsContact = :IsContact";
                    ListCriteria.Add("IsContact", false);
                }                
            }
            
            //strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
            //strCriteria += " O.CustomerId != :CustomerId";
            //ListCriteria.Add("CustomerId", "COMPANY");

            if (!string.IsNullOrEmpty(wsi.FromDate))
            {
                if (string.IsNullOrEmpty(wsi.ToDate))
                {
                    wsi.ToDate = DateTime.Parse(wsi.FromDate).AddDays(1).ToString();
                }
                else
                {
                    wsi.ToDate = DateTime.Parse(wsi.ToDate).AddDays(1).ToString();
                }
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.CreatedDate >= :FromDate AND O.CreatedDate < :ToDate";
                ListCriteria.Add("FromDate", DateTime.Parse(wsi.FromDate));
                ListCriteria.Add("ToDate", DateTime.Parse(wsi.ToDate));
            }
            if (!string.IsNullOrEmpty(wsi.ActionBy))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.ManagedBy = :ActionBy";
                ListCriteria.Add("ActionBy", wsi.ActionBy);
            }
            if (!string.IsNullOrEmpty(wsi.Keyword))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " (O.CustomerId LIKE :Keyword OR O.Code LIKE :Keyword OR O.CompanyNameInVietnamese LIKE :Keyword OR O.Address LIKE :Keyword OR O.Telephone LIKE :Keyword OR O.Cellphone LIKE :Keyword OR O.Website LIKE :Keyword OR O.TaxCode LIKE :Keyword OR O.EmailAddress LIKE :Keyword)";
            }
            string strCmdCounter = "SELECT COUNT(O.CustomerId) FROM Customers AS O" + strCriteria;
            IQuery queryCounter = Session.CreateSQLQuery(strCmdCounter);
            if (wsi.SortField != null && wsi.SortOrder != "")
            {
                strCriteria += " ORDER BY O." + wsi.SortField + " " + wsi.SortOrder;
            }
            else
            {
                strCriteria += " ORDER BY O.CustomerId ASC, O.CompanyNameInVietnamese ASC";
            }
            string strCmd = "SELECT O FROM Customers AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                queryCounter.SetParameter(Item.Key, Item.Value);
                query.SetParameter(Item.Key, Item.Value);
            }
            if (!string.IsNullOrEmpty(wsi.Keyword))
            {
                queryCounter.SetString("Keyword", "%" + wsi.Keyword + "%");
                query.SetString("Keyword", "%" + wsi.Keyword + "%");
            }
            List<NMCustomersWSI> ListWSI = new List<NMCustomersWSI>();
            CustomersAccesser Accesser = new CustomersAccesser(Session);
            StocksAccesser StockAccesser = new StocksAccesser(Session);
            IList<Customers> objs;
            int totalRows = Accesser.Counter(queryCounter);
            if (wsi.PageSize != 0)
            {
                objs = Accesser.GetCustomersByQuery(query, wsi.PageSize, wsi.PageNum, false);
            }
            else
            {
                objs = Accesser.GetCustomersByQuery(query, false);
            }
            if (wsi.Limit != 0)
            {
                objs = objs.Take(wsi.Limit).ToList();
            }
            foreach (Customers obj in objs)
            {
                wsi = new NMCustomersWSI();
                wsi.Customer = obj;
                //wsi.ManagedBy = Accesser.GetAllCustomersByID(obj.ManagedBy, true);
                //wsi.DefaultStock = StockAccesser.GetAllStocksByID(obj.StockId, true);
                //NMAreasBL AreaBL = new NMAreasBL();
                //wsi.AreaWSI = new NMAreasWSI();
                //wsi.AreaWSI.Mode = "SEL_OBJ";
                //wsi.AreaWSI.Area = new Areas();
                //wsi.AreaWSI.Area.Id = obj.AreaId;
                //wsi.AreaWSI = AreaBL.callSingleBL(wsi.AreaWSI);
                NMStocksBL StockBL = new NMStocksBL();
                NMStocksWSI StockWSI = new NMStocksWSI();
                StockWSI.Mode = "SEL_OBJ";
                StockWSI.Id = obj.StockId;
                wsi.DefaultStock = StockBL.callSingleBL(StockWSI);
                wsi.WsiError = "";
                wsi.TotalRows = totalRows;
                ListWSI.Add(wsi);
            }
            return ListWSI;
        }
    }
}
