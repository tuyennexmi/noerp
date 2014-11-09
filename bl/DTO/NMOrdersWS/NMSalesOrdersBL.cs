// NMSalesOrdersBL.cs

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
using Iesi.Collections;

namespace NEXMI
{
    public class NMSalesOrdersBL
    {
        ISession Session;

        public NMSalesOrdersWSI WSI = new NMSalesOrdersWSI();

        public NMSalesOrdersBL() 
        {
            this.Session = SessionFactory.GetNewSession();
        }

        public NMSalesOrdersBL(ISession ses)
        {
            this.Session = ses;
        }

        public NMSalesOrdersWSI callSingleBL(NMSalesOrdersWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SAV_OBJ":
                    return SaveObject(wsi);
                case "SEL_OBJ":
                    return SelectObject(wsi);
                case "CONFIRM":
                    return ConfirmObject(wsi);
                case "APP_OBJ":
                    return Approval(wsi);
                case "DEL_OBJ":
                    return DeleteObject(wsi);
                default:
                    return wsi;
            }
        }

        public List<NMSalesOrdersWSI> callListBL(NMSalesOrdersWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMSalesOrdersWSI>();
            }
        }

        private NMSalesOrdersWSI SelectObject(NMSalesOrdersWSI wsi)
        {
            try
            {
                SalesOrdersAccesser Acceser = new SalesOrdersAccesser(Session);
                CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
                ParametersAccesser ParameterAccesser = new ParametersAccesser(Session);
                StatusesAccesser StatusAccesser = new StatusesAccesser(Session);
                SalesInvoicesAccesser SIAccesser = new SalesInvoicesAccesser(Session);
                ExportsAccesser ExAcc = new ExportsAccesser(this.Session);
                SalesOrders obj;
                obj = Acceser.GetAllOrdersByID(wsi.Order.OrderId, false);
                if (obj != null)
                {
                    SalesOrderDetailsAccesser detailAcceser = new SalesOrderDetailsAccesser(Session);
                    wsi.Order = obj;
                    wsi.Details = detailAcceser.GetAllOrderdetailsBySOID(obj.OrderId, true).ToList();    //.OrderDetailsList.Cast<SalesOrderDetails>().ToList();
                    wsi.Invoices = SIAccesser.GetSalesinvoicesByQuery(Session.CreateQuery("select O from SalesInvoices O where O.Reference = '" + wsi.Order.OrderId + "'"), true);
                    wsi.Exports = ExAcc.GetExportsByQuery(Session.CreateQuery("select O from Exports O where O.Reference = '" + wsi.Order.OrderId + "'"), true);
                    wsi.Customer = CustomerAccesser.GetAllCustomersByID(obj.CustomerId, true);
                    wsi.SalesPerson = CustomerAccesser.GetAllCustomersByID(obj.SalesPersonId, true);
                    //wsi.Receipts = obj.ReceiptList.Cast<NMReceipts>().ToList();
                    wsi.CreatedBy = CustomerAccesser.GetAllCustomersByID(obj.CreatedBy, true);
                    //wsi.Status = StatusAccesser.GetAllStatusesByID(obj.OrderStatus, true);
                    wsi.Incoterm = ParameterAccesser.GetAllParametersByID(obj.Incoterm, true);
                    if (wsi.Incoterm == null) wsi.Incoterm = new Parameters();
                    wsi.ShippingPolicy = ParameterAccesser.GetAllParametersByID(obj.ShippingPolicy, true);
                    if (wsi.ShippingPolicy == null) wsi.ShippingPolicy = new Parameters();
                    wsi.CreateInvoice = ParameterAccesser.GetAllParametersByID(obj.CreateInvoice, true);
                    if (wsi.CreateInvoice == null) wsi.CreateInvoice = new Parameters();
                    wsi.WsiError = "";
                    Session.Evict(obj);
                }
                else
                {
                    wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
                }
            }
            catch (Exception ex)
            {
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return wsi;
        }

        private NMSalesOrdersWSI SaveObject(NMSalesOrdersWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                SalesOrdersAccesser Accesser = new SalesOrdersAccesser(Session);
                SalesOrderDetailsAccesser DetailAccesser = new SalesOrderDetailsAccesser(Session);
                List<SalesOrderDetails> OldDetails = null;
                SalesOrders obj = Accesser.GetAllOrdersByID(wsi.Order.OrderId, false);
                SalesOrderDetails detail;

                #region Master

                wsi.Order.Discount = wsi.Details.Sum(i => i.DiscountAmount);
                wsi.Order.Tax = wsi.Details.Sum(i => i.TaxAmount);
                wsi.Order.Amount = wsi.Details.Sum(i => i.Amount);
                wsi.Order.TotalAmount = wsi.Details.Sum(i => i.TotalAmount);
                bool IsUpdate = false;
                
                if (obj != null)    // cập nhật
                {
                    OldDetails = obj.OrderDetailsList.Cast<SalesOrderDetails>().ToList();
                    Session.Evict(obj);                    
                    wsi.Order.CreatedDate = obj.CreatedDate;
                    wsi.Order.CreatedBy = obj.CreatedBy;
                    wsi.Order.ModifiedDate = DateTime.Now;
                    wsi.Order.ModifiedBy = wsi.ActionBy;
                    if (wsi.Order.OrderStatus != obj.OrderStatus)
                    {
                        if (NMCommon.GetSetting("UPDATE_SORDER_DATE_BY_ACTION"))
                            wsi.Order.OrderDate = DateTime.Now;
                    }
                    //wsi.Order.OrderDetailsList = wsi.Details;
                    String rs = wsi.CompareTo(obj);
                    if (rs != "")
                        NMMessagesBL.SaveMessage(Session, wsi.Order.OrderId, "cập nhật thông tin", rs, wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
                    //wsi.Order.OrderDetailsList = null;
                    Accesser.UpdateOrders(wsi.Order);
                    IsUpdate = true;
                }
                else    //tạo mới
                {
                    AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                    wsi.Order.OrderId = AutomaticValueAccesser.AutoGenerateId("SalesOrders");
                    wsi.Order.CreatedDate = DateTime.Now;
                    wsi.Order.CreatedBy = wsi.ActionBy;
                    wsi.Order.ModifiedDate = DateTime.Now;
                    wsi.Order.ModifiedBy = wsi.ActionBy;
                    Accesser.InsertOrders(wsi.Order);
                    NMMessagesBL.SaveMessage(Session, wsi.Order.OrderId, "khởi tạo Đơn đặt hàng", "", wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
                }
                #endregion
                #region Details
                foreach (SalesOrderDetails Item in wsi.Details)
                {
                    detail = DetailAccesser.GetAllOrderdetailsByID(Item.Id.ToString(), true);
                    if (detail != null)
                    {
                        Item.OrderId = wsi.Order.OrderId;
                        
                        Item.CreatedBy = detail.CreatedBy;
                        Item.CreatedDate = detail.CreatedDate;
                        Item.ModifiedBy = wsi.ActionBy;
                        Item.ModifiedDate = DateTime.Now;

                        DetailAccesser.UpdateOrderdetails(Item);
                    }
                    else
                    {
                        Item.OrderId = wsi.Order.OrderId;
                        
                        Item.CreatedBy = wsi.ActionBy;
                        Item.CreatedDate = DateTime.Now;
                        Item.ModifiedBy = wsi.ActionBy;
                        Item.ModifiedDate = DateTime.Now;

                        DetailAccesser.InsertOrderdetails(Item);
                    }
                }
                if (OldDetails != null)
                {
                    bool flag = true;
                    foreach (SalesOrderDetails Old in OldDetails)
                    {
                        flag = true;
                        foreach (SalesOrderDetails New in wsi.Details)
                        {
                            if (Old.Id == New.Id)
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (flag)
                        {
                            DetailAccesser.DeleteOrderdetails(Old);
                        }
                    }
                }
                #endregion

                tx.Commit();

                //tự động tạo các chứng từ liên quan
                if (wsi.Order.OrderStatus == NMConstant.SOStatuses.Order | wsi.Order.OrderStatus == NMConstant.SOStatuses.Invoice)
                {
                    //tạo phiếu xuất kho
                    if (NMCommon.GetSetting(NMConstant.Settings.EXPORT_FROM_ORDER))
                    {
                        this.ExportFromOrder(wsi);
                    }
                    //tạo hóa đơn
                    if (NMCommon.GetSetting(NMConstant.Settings.INVOICE_FROM_ORDER))
                    {
                        this.InvoiceFromOrder(wsi);
                    }
                }
                
                // cap nhat hoa don, phieu xuat kho
                if (IsUpdate)
                {
                    ReconcileInvoiceFromOrder(wsi);
                    ReconcileExportFromOrder(wsi);
                }
            }
            catch (Exception ex)
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return wsi;
        }

        private NMSalesOrdersWSI ConfirmObject(NMSalesOrdersWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                SalesOrdersAccesser Accesser = new SalesOrdersAccesser(Session);
                SalesOrderDetailsAccesser DetailAccesser = new SalesOrderDetailsAccesser(Session);                
                SalesOrders obj = Accesser.GetAllOrdersByID(wsi.Order.OrderId, true);

                if (obj != null)
                {
                    //obj.OrderStatus = wsi.Order.OrderStatus;
                    wsi.Order.ModifiedDate = DateTime.Now;
                    wsi.Order.ModifiedBy = wsi.ActionBy;
                    if (wsi.Order.OrderStatus != obj.OrderStatus)
                    {
                        if (NMCommon.GetSetting("UPDATE_SORDER_DATE_BY_ACTION"))
                            wsi.Order.OrderDate = DateTime.Now;
                    }
                    NMMessagesBL.SaveMessage(Session, wsi.Order.OrderId, "thay đổi tình trạng", NMCommon.GetStatusName(obj.OrderStatus, "vi") + " => " + NMCommon.GetStatusName(wsi.Order.OrderStatus, "vi"), wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
                    
                    Accesser.UpdateOrders(wsi.Order);

                    tx.Commit();

                    //tự động tạo các chứng từ liên quan
                    if (wsi.Order.OrderStatus == NMConstant.SOStatuses.Order | wsi.Order.OrderStatus == NMConstant.SOStatuses.Invoice)
                    {
                        //tạo phiếu xuất kho
                        if (NMCommon.GetSetting(NMConstant.Settings.EXPORT_FROM_ORDER))
                        {
                            this.ExportFromOrder(wsi);
                        }
                        //tạo hóa đơn
                        if (NMCommon.GetSetting(NMConstant.Settings.INVOICE_FROM_ORDER))
                        {
                            this.InvoiceFromOrder(wsi);
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

        private NMSalesOrdersWSI Approval(NMSalesOrdersWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                SalesOrdersAccesser Accesser = new SalesOrdersAccesser(Session);
                SalesOrderDetailsAccesser DetailAccesser = new SalesOrderDetailsAccesser(Session);
                SalesOrders obj = Accesser.GetAllOrdersByID(wsi.Order.OrderId, true);

                if (obj != null)
                {   
                    wsi.Order.ModifiedDate = DateTime.Now;
                    wsi.Order.ModifiedBy = wsi.ActionBy;

                    NMMessagesBL BL = new NMMessagesBL();
                    NMMessagesWSI MSG = new NMMessagesWSI();
                    MSG.Mode = "SAV_OBJ";
                    
                    MSG.Message = new Messages();
                    MSG.Message.MessageName = "Duyệt báo giá";
                    MSG.Message.Owner = wsi.Order.OrderId;
                    MSG.Message.SendTo = obj.CreatedBy;
                    MSG.Message.Description = NMCommon.GetStatusName(obj.OrderStatus, "vi") + " => " + NMCommon.GetStatusName(wsi.Order.OrderStatus, "vi");
                    MSG.Message.TypeId = NMConstant.MessageTypes.SysLog;
                    MSG.ActionBy = wsi.ActionBy;
                    MSG = BL.callSingleBL(MSG);

                    //NMMessagesBL.SaveMessage(Session, wsi.Order.OrderId, "", , wsi.ActionBy, null, null, );
                    Accesser.UpdateOrders(wsi.Order);

                    tx.Commit();
                }
                
            }
            catch (Exception ex)
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return wsi;
        }

        private NMSalesOrdersWSI DeleteObject(NMSalesOrdersWSI wsi)
        {
            try
            {
                ITransaction tx = Session.BeginTransaction();
                SalesOrdersAccesser Accesser = new SalesOrdersAccesser(Session);
                SalesOrders obj = Accesser.GetAllOrdersByID(wsi.Order.OrderId, true);
                if (obj != null)
                {
                    Accesser.DeleteOrders(obj);
                    try
                    {
                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        wsi.WsiError = "Không được xóa hóa đơn này.";
                    }
                }
                else
                {
                    wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
                }
            }
            catch (Exception ex)
            {
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return wsi;
        }

        private List<NMSalesOrdersWSI> SearchObject(NMSalesOrdersWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (wsi.Order != null)
            {
                if (!string.IsNullOrEmpty(wsi.Order.CustomerId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.CustomerId = :CustomerId";
                    ListCriteria.Add("CustomerId", wsi.Order.CustomerId);
                }
                if (!string.IsNullOrEmpty(wsi.Order.OrderTypeId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.OrderTypeId = :OrderTypeId";
                    ListCriteria.Add("OrderTypeId", wsi.Order.OrderTypeId);
                }
                if (!string.IsNullOrEmpty(wsi.Order.OrderGroup))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.OrderGroup = :OrderGroup";
                    ListCriteria.Add("OrderGroup", wsi.Order.OrderGroup);
                }
                if (!string.IsNullOrEmpty(wsi.Order.OrderStatus))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.OrderStatus = :OrderStatus";
                    ListCriteria.Add("OrderStatus", wsi.Order.OrderStatus);
                }
                if (!string.IsNullOrEmpty(wsi.Order.Description))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.Description LIKE :Description";
                    ListCriteria.Add("Description", wsi.Order.Description);
                }
            }
            // neu la ban
            if (wsi.Order.OrderGroup == NMConstant.SOrderGroups.Sale)
            {
                if (!string.IsNullOrEmpty(wsi.FromDate))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.OrderDate >= :FromDate ";
                    ListCriteria.Add("FromDate", DateTime.Parse(wsi.FromDate));
                }
                if (!string.IsNullOrEmpty(wsi.ToDate))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.OrderDate <= :ToDate";
                    ListCriteria.Add("ToDate", DateTime.Parse(wsi.ToDate));
                }
            }
            else  // cho thuê
            {
                if (!string.IsNullOrEmpty(wsi.FromDate) & !string.IsNullOrEmpty(wsi.ToDate))
                {
                    int from = DateTime.Parse(wsi.FromDate).Day;
                    int to = DateTime.Parse(wsi.ToDate).Day;
                    if (from < to)
                    {
                        strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                        strCriteria += " DAY(O.MaintainDate) >= :FromDate AND DAY(O.MaintainDate) <= :ToDate";
                        ListCriteria.Add("FromDate", from);
                        ListCriteria.Add("ToDate", to);
                    }
                    else
                    {
                        strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                        strCriteria += " DAY(O.MaintainDate) >= :FromDate AND DAY(O.MaintainDate) <= :ToDate";
                        ListCriteria.Add("FromDate", from);
                        ListCriteria.Add("ToDate", to);
                    }
                }
            }

            if (!string.IsNullOrEmpty(wsi.ActionBy))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.CreatedBy LIKE :ActionBy";
                ListCriteria.Add("ActionBy", wsi.ActionBy);
            }
            if (!string.IsNullOrEmpty(wsi.Keyword))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " (O.Description LIKE :Keyword OR O.OrderId LIKE :Keyword OR O.Reference LIKE :Keyword OR O.Transportation LIKE :Keyword OR O.CustomerId in (SELECT C.CustomerId FROM Customers AS C WHERE C.CustomerId LIKE :Keyword OR C.CompanyNameInVietnamese LIKE :Keyword OR C.TaxCode LIKE :Keyword OR C.Cellphone LIKE :Keyword OR C.Telephone LIKE :Keyword OR C.Code LIKE :Keyword))";
                ListCriteria.Add("Keyword", "%" + wsi.Keyword + "%");
            }
            string strCmdCounter = "SELECT COUNT(O.OrderId) FROM SalesOrders AS O" + strCriteria;
            IQuery queryCounter = Session.CreateSQLQuery(strCmdCounter);
            if (wsi.SortField != null && wsi.SortOrder != "")
            {
                strCriteria += " ORDER BY O." + wsi.SortField + " " + wsi.SortOrder;
            }
            else
            {
                strCriteria += " ORDER BY O.OrderDate DESC, O.OrderId DESC";
            }
            string strCmd = "SELECT O FROM SalesOrders AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                queryCounter.SetParameter(Item.Key, Item.Value);
                query.SetParameter(Item.Key, Item.Value);
            }
            List<NMSalesOrdersWSI> ListWSI = new List<NMSalesOrdersWSI>();
            SalesOrdersAccesser Accesser = new SalesOrdersAccesser(Session);
            CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
            ParametersAccesser ParameterAccesser = new ParametersAccesser(Session);
            StatusesAccesser StatusAccesser = new StatusesAccesser(Session);
            SalesInvoicesAccesser SIAccesser = new SalesInvoicesAccesser(Session);
            IList<SalesOrders> objs;
            int totalRows = NMCommon.Counter(queryCounter);
            if (wsi.PageSize != 0)
                objs = Accesser.GetOrdersByQuery(query, wsi.PageSize, wsi.PageNum, false);
            else
                objs = Accesser.GetOrdersByQuery(query, false);

            bool get_Invoice = NMCommon.GetSetting("GET_SO_INVOICE_ON_SRC_MODE");
            bool get_Details = NMCommon.GetSetting("GET_SO_DETAILS_ON_SRC_MODE");
                
            foreach (SalesOrders obj in objs)
            {
                wsi = new NMSalesOrdersWSI();
                wsi.Order = obj;
                if (get_Details)
                    wsi.Details = obj.OrderDetailsList.Cast<SalesOrderDetails>().ToList();

                if (get_Invoice)
                    wsi.Invoices = SIAccesser.GetSalesinvoicesByQuery(Session.CreateQuery("select O from SalesInvoices O where O.Reference = '" + wsi.Order.OrderId + "'"), true);

                wsi.Customer = CustomerAccesser.GetAllCustomersByID(obj.CustomerId, true);
                wsi.CreatedBy = CustomerAccesser.GetAllCustomersByID(obj.CreatedBy, true);
                //wsi.Status = StatusAccesser.GetAllStatusesByID(obj.OrderStatus, true);
                //wsi.TotalRows = totalRows;
                wsi.WsiError = "";
                ListWSI.Add(wsi);
            }

            if (objs.Count > 0)
                ListWSI[0].TotalRows = totalRows;

            return ListWSI;
        }

        // tao phieu xuat kho
        #region Export

        public NMExportsWSI CreateExport()
        {
            NMExportsWSI exWsi = new NMExportsWSI();
            exWsi.Export.CopyFromSO(this.WSI.Order);

            if (NMCommon.GetSetting("USE_SODATE_FOR_EXPORT_DATE"))
                exWsi.Export.ExportDate = this.WSI.Order.OrderDate;

            exWsi.Details = new List<ExportDetails>();
            ExportDetails Detail;
            foreach (SalesOrderDetails Item in this.WSI.Details)
            {
                Detail = new ExportDetails();
                Detail.CopyFromSODetail(Item);

                exWsi.Details.Add(Detail);
            }

            // tra ve Customer
            exWsi.Customer = this.WSI.Customer;

            return exWsi;
        }

        public NMExportsWSI CreateExport(string soId)
        {
            NMSalesOrdersBL SOBL = new NMSalesOrdersBL();
            NMSalesOrdersWSI SOWSI = new NMSalesOrdersWSI();
            SOWSI.Mode = "SEL_OBJ";
            SOWSI.Order.OrderId = soId;
            SOWSI = SOBL.callSingleBL(SOWSI);

            NMExportsWSI exWsi = new NMExportsWSI();
            if (SOWSI.WsiError == string.Empty)
            {
                this.WSI = SOWSI;
                exWsi = this.CreateExport();
            }
            else
                exWsi.WsiError = SOWSI.WsiError;
            
            return exWsi;
        }
        
        public string ExportFromOrder(NMSalesOrdersWSI wsi)
        {
            string error = "";
            try
            {
                //kiểm tra đã xuất kho cho đơn hàng hay chưa
                NMExportsBL ExportBL = new NMExportsBL();
                NMExportsWSI ExportWSI = new NMExportsWSI();                
                ExportWSI.Mode = "SRC_OBJ";
                ExportWSI.Export.Reference = wsi.Order.OrderId;
                List<NMExportsWSI> expList = ExportBL.callListBL(ExportWSI);
                if (expList.Count <= 0)
                {
                    this.WSI = wsi;
                    ExportWSI = this.CreateExport();

                    ExportWSI.Mode = "SAV_OBJ";
                    ExportWSI.ActionBy = wsi.ActionBy;

                    //  luu
                    ExportWSI = ExportBL.callSingleBL(ExportWSI);
                    
                    error = ExportWSI.WsiError;
                }
            }
            catch (Exception ex)
            {
                error = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return error;
        }

        public string ReconcileExportFromOrder(NMSalesOrdersWSI soWsi)
        {
            string error = "";
            try
            {
                //kiểm tra đã xuất kho cho đơn hàng hay chưa
                NMExportsBL ExportBL = new NMExportsBL();
                NMExportsWSI ExportWSI = new NMExportsWSI();
                ExportWSI.Mode = "SRC_OBJ";
                ExportWSI.Export.Reference = soWsi.Order.OrderId;
                List<NMExportsWSI> expList = ExportBL.callListBL(ExportWSI);
                if (expList.Count == 1)
                {
                    NMExportsBL BL = new NMExportsBL();
                    NMExportsWSI WSI = new NMExportsWSI();
                    WSI.Mode = "SAV_OBJ";
                    WSI.Export = new Exports();
                    WSI.Export.ExportId = expList[0].Export.ExportId;

                    if (NMCommon.GetSetting("USE_SODATE_FOR_EXPORT_DATE"))
                        WSI.Export.ExportDate = soWsi.Order.OrderDate;
                    else
                        WSI.Export.ExportDate = expList[0].Export.ExportDate;
                    
                    if (!string.IsNullOrEmpty(soWsi.Order.CustomerId))
                        WSI.Export.CustomerId = soWsi.Order.CustomerId;
                    WSI.Export.Reference = soWsi.Order.OrderId;
                    WSI.Export.DescriptionInVietnamese = expList[0].Export.DescriptionInVietnamese;
                    WSI.Export.ExportTypeId = expList[0].Export.ExportTypeId;
                    WSI.Export.ExportStatus = expList[0].Export.ExportStatus;
                    WSI.Export.StockId = soWsi.Order.StockId;
                    WSI.Export.DeliveryMethod = expList[0].Export.DeliveryMethod;
                    if (!string.IsNullOrEmpty(expList[0].Export.CarrierId))
                        WSI.Export.CarrierId = expList[0].Export.CarrierId;
                    WSI.Export.Transport = expList[0].Export.Transport;
                    WSI.ActionBy = soWsi.ActionBy;

                    WSI.Details = new List<ExportDetails>();
                    ExportDetails detail;
                    foreach (SalesOrderDetails Item in soWsi.Details)
                    {
                        detail = expList[0].Details.Find(e => e.ProductId == Item.ProductId);
                        if (detail == null)
                        {
                            detail = new ExportDetails();
                        }
                        detail.CopyFromSODetail(Item);
                        WSI.Details.Add(detail);
                    }

                    WSI = BL.callSingleBL(WSI);
                    error = WSI.WsiError;
                }
                else
                {
                    error = "Có quá nhiều Phiếu xuất, do đó bạn phải tự hiệu chỉnh phiếu bạn muốn!";
                }
            }
            catch (Exception ex)
            {
                error = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return error;
        }

        #endregion

        // tao hoa don
        #region Invoice

        // tao ma khong luu
        public NMSalesInvoicesWSI CreateSaleInvoice()
        {
            NMSalesInvoicesWSI siWsi = new NMSalesInvoicesWSI();
            siWsi.Invoice.CopyFromSO(this.WSI.Order);

            if (NMCommon.GetSetting("USE_SODATE_FOR_SI_DATE"))
                siWsi.Invoice.InvoiceDate = this.WSI.Order.OrderDate;

            siWsi.Details = new List<SalesInvoiceDetails>();
            SalesInvoiceDetails SIDetail;
            foreach (SalesOrderDetails Item in this.WSI.Details)
            {
                SIDetail = new SalesInvoiceDetails();
                SIDetail.CopyFromSODetail(Item);

                siWsi.Details.Add(SIDetail);
            }

            siWsi.Invoice.Amount = WSI.Details.Sum(i => i.Amount);
            siWsi.Invoice.Discount = WSI.Details.Sum(i => i.DiscountAmount);
            siWsi.Invoice.Tax = WSI.Details.Sum(i => i.TaxAmount);
            siWsi.Invoice.TotalAmount = WSI.Details.Sum(i => i.TotalAmount);

            // gan luon du lieu di kem
            siWsi.Customer = this.WSI.Customer;

            return siWsi;
        }

        // tao nhung chua luu tu SO Id
        public NMSalesInvoicesWSI CreateSaleInvoice(string soId)
        {   
            NMSalesOrdersWSI SOWSI = new NMSalesOrdersWSI();
            SOWSI.Mode = "SEL_OBJ";
            SOWSI.Order.OrderId = soId;
            SOWSI = this.callSingleBL(SOWSI);

            NMSalesInvoicesWSI siWsi = new NMSalesInvoicesWSI();
            if (SOWSI.WsiError == string.Empty)
            {
                this.WSI = SOWSI;
                siWsi = this.CreateSaleInvoice();
            }
            else
                siWsi.WsiError = SOWSI.WsiError;

            return siWsi;
        }

        // tao SI tu SO va luu lai
        private string InvoiceFromOrder(NMSalesOrdersWSI SOWSI)
        {
            string err="";
            try
            {
                NMSalesInvoicesBL SIBL = new NMSalesInvoicesBL(this.Session);
                NMSalesInvoicesWSI SIWSI = new NMSalesInvoicesWSI();
                SIWSI.Mode = "SRC_OBJ";
                SIWSI.Invoice.Reference = SOWSI.Order.OrderId;
                List<NMSalesInvoicesWSI> SIs = SIBL.callListBL(SIWSI);

                if (SIs.Count <= 0)
                {
                    this.WSI = SOWSI;
                    SIWSI = this.CreateSaleInvoice();

                    SIWSI.Mode = "SAV_OBJ";
                    SIWSI.ActionBy = SOWSI.ActionBy;

                    // luu
                    SIWSI = SIBL.callSingleBL(SIWSI);
                    //SOWSI.Invoices.Add(SIWSI.Invoice);
                    err = SIWSI.WsiError;
                }
            }
            catch (Exception ex)
            {
                err = "Có lỗi xảy ra: " + ex.Message + "\n" + ex.InnerException;
            }
            return err;
        }

        private string ReconcileInvoiceFromOrder(NMSalesOrdersWSI SOWSI)
        {
            string error = "";
            try
            {
                NMSalesInvoicesBL SIBL = new NMSalesInvoicesBL();
                NMSalesInvoicesWSI SIWSI = new NMSalesInvoicesWSI();
                SIWSI.Mode = "SRC_OBJ";
                SIWSI.GetDetails = true;
                SIWSI.Invoice.Reference = SOWSI.Order.OrderId;
                List<NMSalesInvoicesWSI> SIs = SIBL.callListBL(SIWSI);

                if (SIs.Count == 1)
                {
                    SIWSI.Mode = "SAV_OBJ";
                    SIWSI.Invoice = new SalesInvoices();
                    SIWSI.Invoice.InvoiceId = SIs[0].Invoice.InvoiceId;
                    SIWSI.Invoice.InvoiceStatus = SIs[0].Invoice.InvoiceStatus;

                    if (NMCommon.GetSetting("USE_SODATE_FOR_SI_DATE"))
                        SIWSI.Invoice.InvoiceDate = SOWSI.Order.OrderDate;
                    else
                        SIWSI.Invoice.InvoiceDate = SIs[0].Invoice.InvoiceDate;

                    SIWSI.Invoice.CustomerId = SOWSI.Order.CustomerId;
                    SIWSI.Invoice.CurrencyId = "VND";
                    SIWSI.Invoice.ExchangeRate = 1;
                    SIWSI.Invoice.DescriptionInVietnamese = SOWSI.Order.Description;
                    SIWSI.Invoice.InvoiceTypeId = SOWSI.Order.InvoiceTypeId;
                    SIWSI.Invoice.SalesPersonId = SOWSI.Order.SalesPersonId;
                    SIWSI.Invoice.Reference = SOWSI.Order.OrderId;
                    SIWSI.Invoice.SourceDocument = SOWSI.Order.Reference + "; " + SOWSI.Order.Transportation;
                    SIWSI.Invoice.StockId = SOWSI.Order.StockId;

                    SIWSI.Details = new List<SalesInvoiceDetails>();
                    SalesInvoiceDetails SIDetail;
                    foreach (SalesOrderDetails Item in SOWSI.Details)
                    {
                        SIDetail = SIs[0].Details.Find(d=>d.ProductId == Item.ProductId);
                        if (SIDetail == null)
                        {
                            SIDetail = new SalesInvoiceDetails();
                        }
                        SIDetail.CopyFromSODetail(Item);
                        SIWSI.Details.Add(SIDetail);
                    }
                    SIWSI.ActionBy = SOWSI.ActionBy;
                    SIWSI = SIBL.callSingleBL(SIWSI);
                    error = SIWSI.WsiError;
                }
                else
                {
                    error = "Có quá nhiều Hóa đơn!";
                }
            }
            catch (Exception ex)
            {
                error = "Có lỗi xảy ra: " + ex.Message + "\n" + ex.InnerException;
            }
            return error;
        }

        #endregion

        // thu tien tam ung

        public void AdvancesPayment(NMSalesOrdersWSI soWSI)
        {
            double adv = double.Parse(soWSI.Order.Advances);
            if (adv != 0)
            {
                NMReceiptsBL rcpBL = new NMReceiptsBL();
                NMReceiptsWSI rcpWSI = new NMReceiptsWSI();
                rcpWSI.Mode = "SAV_OBJ";
                
                rcpWSI.Receipt.ReceiptId = "";
                rcpWSI.Receipt.ReceiptDate = soWSI.Order.OrderDate;
                rcpWSI.Receipt.CustomerId = soWSI.Order.CustomerId;
                rcpWSI.Receipt.InvoiceId = soWSI.Order.OrderId;
                rcpWSI.Receipt.Amount = adv;
                rcpWSI.Receipt.CurrencyId = "VND";
                rcpWSI.Receipt.ExchangeRate = 1;
                rcpWSI.Receipt.ReceiptAmount = adv / rcpWSI.Receipt.ExchangeRate;
                rcpWSI.Receipt.DescriptionInVietnamese = "Thu tiền tạm ứng cho đơn hàng: " + soWSI.Order.OrderId;
                rcpWSI.Receipt.ReceiptTypeId = NEXMI.NMConstant.ReceiptType.Deposit;
                rcpWSI.Receipt.ReceiptStatus = NMConstant.ReceiptStatuses.Done;
                rcpWSI.ActionBy = soWSI.ActionBy;

                rcpWSI = rcpBL.callSingleBL(rcpWSI);
            }
        }
    }
}
