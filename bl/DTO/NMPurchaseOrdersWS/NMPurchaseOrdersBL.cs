// NMPurchaseOrdersBL.cs

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
    public class NMPurchaseOrdersBL
    {
        ISession Session ;

        public NMPurchaseOrdersWSI WSI = new NMPurchaseOrdersWSI();

        public NMPurchaseOrdersBL() 
        {
             this.Session = SessionFactory.GetNewSession();
        }

        public NMPurchaseOrdersBL(ISession ses)
        {
            this.Session = ses;
        }

        public NMPurchaseOrdersWSI callSingleBL(NMPurchaseOrdersWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SAV_OBJ":
                    return SaveObject(wsi);
                case "SEL_OBJ":
                    return SelectObject(wsi);
                case "DEL_OBJ":
                    return DeleteObject(wsi);
                case "CRT_IMP":
                    return ImportFromPOrder(wsi);
                default:
                    return wsi;
            }
        }

        public List<NMPurchaseOrdersWSI> callListBL(NMPurchaseOrdersWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMPurchaseOrdersWSI>();
            }
        }

        public NMPurchaseOrdersWSI SelectObject(NMPurchaseOrdersWSI wsi)
        {
            try
            {
                PurchaseOrdersAccesser Accesser = new PurchaseOrdersAccesser(Session);
                CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
                ParametersAccesser ParameterAccesser = new ParametersAccesser(Session);
                StatusesAccesser StatusAccesser = new StatusesAccesser(Session);
                PurchaseInvoicesAccesser PIAcc = new PurchaseInvoicesAccesser(this.Session);
                ImportsAccesser ImAcc = new ImportsAccesser(this.Session);

                PurchaseOrders obj;
                obj = Accesser.GetAllOrdersByID(wsi.Order.Id, false);
                if (obj != null)
                {
                    PurchaseOrderDetailsAccesser detailAccesser = new PurchaseOrderDetailsAccesser(Session);

                    wsi.Order = obj;
                    wsi.Details = detailAccesser.GetAllOrderdetailsByPOID(obj.Id, true).ToList();    // .OrderDetailsList.Cast<PurchaseOrderDetails>().ToList();
                    wsi.Invoices = PIAcc.GetPurchaseinvoicesByQuery(Session.CreateQuery("select O from PurchaseInvoices O where O.Reference = '" + wsi.Order.Id + "'"), true);
                    wsi.Imports = ImAcc.GetImportsByQuery(Session.CreateQuery("select O from Imports O where O.Reference = '" + wsi.Order.Id + "'"), true);
                    wsi.Supplier = CustomerAccesser.GetAllCustomersByID(obj.SupplierId, true);
                    wsi.CreatedUser = CustomerAccesser.GetAllCustomersByID(obj.CreatedBy, true);
                    //wsi.Status = StatusAccesser.GetAllStatusesByID(obj.OrderStatus, true);
                    // thong tin kho
                    NMStocksBL StockBL = new NMStocksBL();
                    NMStocksWSI StockWSI = new NMStocksWSI();
                    StockWSI.Mode = "SEL_OBJ";
                    StockWSI.Id = obj.ImportStockId;
                    wsi.Stock = StockBL.callSingleBL(StockWSI);

                    // t.t nguoi mua
                    wsi.Buyer = CustomerAccesser.GetAllCustomersByID(obj.BuyerId, true);
                    wsi.ApprovalBy = CustomerAccesser.GetAllCustomersByID(obj.ApproveBy, true);

                    wsi.InvoiceControl = ParameterAccesser.GetAllParametersByID(obj.InvoiceControl, true);

                    Session.Evict(obj);

                    wsi.WsiError = "";
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

        public NMPurchaseOrdersWSI SaveObject(NMPurchaseOrdersWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                PurchaseOrdersAccesser Accesser = new PurchaseOrdersAccesser(Session);
                PurchaseOrderDetailsAccesser DetailAccesser = new PurchaseOrderDetailsAccesser(Session);
                List<PurchaseOrderDetails> OldDetails = null;
                PurchaseOrders obj = Accesser.GetAllOrdersByID(wsi.Order.Id, false);
                PurchaseOrderDetails detail;
                bool IsUpdate = false;

                #region Master

                wsi.Order.Discount = wsi.Details.Sum(i => i.DiscountAmount);
                wsi.Order.Tax = wsi.Details.Sum(i => i.TaxAmount);
                wsi.Order.Amount = wsi.Details.Sum(i => i.Amount);
                wsi.Order.TotalAmount = wsi.Details.Sum(i => i.TotalAmount);
                
                if (obj != null)    // cập nhật
                {
                    OldDetails = obj.OrderDetailsList.Cast<PurchaseOrderDetails>().ToList();
                    Session.Evict(obj);

                    wsi.Order.CreatedDate = obj.CreatedDate;
                    wsi.Order.CreatedBy = obj.CreatedBy;
                    wsi.Order.ModifiedDate = DateTime.Now;
                    wsi.Order.ModifiedBy = wsi.ActionBy;
                    
                    if (wsi.Order.OrderStatus != obj.OrderStatus)
                    {
                        if (NMCommon.GetSetting("UPDATE_PORDER_DATE_BY_ACTION"))
                            wsi.Order.OrderDate = DateTime.Now;
                    }
                    String rs = wsi.CompareTo(obj);
                    if (rs != "")
                        NMMessagesBL.SaveMessage(Session, wsi.Order.Id, "cập nhật thông tin", rs, wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
                    Accesser.UpdateOrders(wsi.Order);
                    IsUpdate = true;
                }
                else    // tạo mới
                {
                    AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                    wsi.Order.Id = AutomaticValueAccesser.AutoGenerateId("PurchaseOrders");
                    
                    wsi.Order.CreatedDate = DateTime.Now;
                    wsi.Order.CreatedBy = wsi.ActionBy;
                    wsi.Order.ModifiedDate = DateTime.Now;
                    wsi.Order.ModifiedBy = wsi.ActionBy;
                    
                    Accesser.InsertOrders(wsi.Order);
                    NMMessagesBL.SaveMessage(Session, wsi.Order.Id, "khởi tạo tài liệu", "", wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
                }
                #endregion

                #region Details
                if (wsi.Details != null)
                {
                    foreach (PurchaseOrderDetails Item in wsi.Details)
                    {
                        detail = DetailAccesser.GetAllOrderdetailsByID(Item.Id.ToString(), true);
                        if (detail != null)
                        {
                            Item.OrderId = wsi.Order.Id;
                            
                            Item.CreatedBy = detail.CreatedBy;
                            Item.CreatedDate = detail.CreatedDate;
                            Item.ModifiedBy = wsi.ActionBy;
                            Item.ModifiedDate = DateTime.Now;

                            DetailAccesser.UpdateOrderdetails(Item);
                        }
                        else
                        {
                            Item.OrderId = wsi.Order.Id;

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
                        foreach (PurchaseOrderDetails Old in OldDetails)
                        {
                            flag = true;
                            foreach (PurchaseOrderDetails New in wsi.Details)
                            {
                                if (Old.Id == New.Id)
                                {
                                    flag = false;
                                }
                            }
                            if (flag)
                            {
                                DetailAccesser.DeleteOrderdetails(Old);
                            }
                        }
                    }
                }
                #endregion

                tx.Commit();

                //tự động tạo các chứng từ liên quan
                if (wsi.Order.OrderStatus == NMConstant.POStatuses.Order | wsi.Order.OrderStatus == NMConstant.POStatuses.Invoice)
                {
                    //tạo phiếu xuất kho
                    if (NMCommon.GetSetting(NMConstant.Settings.IMPORT_FROM_ORDER))
                    {
                        this.ImportFromPOrder(wsi);
                    }
                    //tạo hóa đơn
                    if (NMCommon.GetSetting(NMConstant.Settings.PI_FROM_ORDER))
                    {
                        this.PInvoiceFromPOrder(wsi);
                    }
                }

                // cap nhat hoa don, phieu xuat kho
                if (IsUpdate)
                {
                    ReconcileInvoiceFromOrder(wsi);
                    ReconcileImportFromOrder(wsi);
                }
            }
            catch (Exception ex)
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return wsi;
        }

        public NMPurchaseOrdersWSI DeleteObject(NMPurchaseOrdersWSI wsi)
        {
            try
            {
                ITransaction tx = Session.BeginTransaction();
                PurchaseOrdersAccesser Accesser = new PurchaseOrdersAccesser(Session);
                PurchaseOrders obj = Accesser.GetAllOrdersByID(wsi.Order.Id, true);
                if (obj == null)
                {
                    wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
                }
                else
                {
                    try
                    {
                        Accesser.DeleteOrders(obj);
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
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return wsi;
        }

        public List<NMPurchaseOrdersWSI> SearchObject(NMPurchaseOrdersWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (wsi.Order != null)
            {
                if (!string.IsNullOrEmpty(wsi.Order.SupplierId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.SupplierId = :SupplierId";
                    ListCriteria.Add("SupplierId", wsi.Order.SupplierId);
                }
                if (!string.IsNullOrEmpty(wsi.Order.OrderTypeId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.OrderTypeId = :OrderTypeId";
                    ListCriteria.Add("OrderTypeId", wsi.Order.OrderTypeId);
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
            if (!string.IsNullOrEmpty(wsi.ActionBy))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.CreatedBy LIKE :ActionBy";
                ListCriteria.Add("ActionBy", wsi.ActionBy);
            }
            if (!string.IsNullOrEmpty(wsi.Keyword))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " (O.Description LIKE :Keyword OR O.Id LIKE :Keyword OR O.SupplierId in (SELECT C.CustomerId FROM Customers AS C WHERE C.CustomerId LIKE :Keyword OR C.CompanyNameInVietnamese LIKE :Keyword OR C.TaxCode LIKE :Keyword OR C.Cellphone LIKE :Keyword))";
            }
            string strCmdCounter = "SELECT COUNT(O.Id) FROM PurchaseOrders AS O" + strCriteria;
            IQuery queryCounter = Session.CreateSQLQuery(strCmdCounter);
            if (wsi.SortField != null && wsi.SortOrder != "")
            {
                strCriteria += " ORDER BY O." + wsi.SortField + " " + wsi.SortOrder;
            }
            else
            {
                strCriteria += " ORDER BY O.OrderDate DESC, O.Id DESC";
            }
            string strCmd = "SELECT O FROM PurchaseOrders AS O" + strCriteria;
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
            List<NMPurchaseOrdersWSI> ListWSI = new List<NMPurchaseOrdersWSI>();
            PurchaseOrdersAccesser Accesser = new PurchaseOrdersAccesser(Session);
            CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
            ParametersAccesser ParameterAccesser = new ParametersAccesser(Session);
            StatusesAccesser StatusAccesser = new StatusesAccesser(Session);
            IList<PurchaseOrders> objs;
            int totalRows = NMCommon.Counter(queryCounter);
            if (wsi.PageSize != 0)
            {
                objs = Accesser.GetOrdersByQuery(query, wsi.PageSize, wsi.PageNum, false);
            }
            else
            {
                objs = Accesser.GetOrdersByQuery(query, false);
            }
            foreach (PurchaseOrders obj in objs)
            {
                wsi = new NMPurchaseOrdersWSI();
                wsi.Order = obj;
                //wsi.Details = obj.OrderDetailsList.Cast<PurchaseOrderDetails>().ToList();
                wsi.Supplier = CustomerAccesser.GetAllCustomersByID(obj.SupplierId, true);
                wsi.CreatedUser = CustomerAccesser.GetAllCustomersByID(obj.CreatedBy, true);
                //wsi.Status = StatusAccesser.GetAllStatusesByID(obj.OrderStatus, true);
                //wsi.TotalRows = totalRows;
                wsi.WsiError = "";
                ListWSI.Add(wsi);
            }
            if (ListWSI.Count > 0)
                ListWSI[0].TotalRows = totalRows;

            return ListWSI;
        }

        // tao phieu nhap
        #region Import

        public NMImportsWSI CreateImport()
        {
            NMImportsWSI importWSI = new NMImportsWSI();
            importWSI.Import.CopyFromPO(this.WSI.Order);

            if (NMCommon.GetSetting("USE_PODATE_FOR_IMPORT_DATE"))
                importWSI.Import.ImportDate = this.WSI.Order.OrderDate;

            importWSI.Details = new List<ImportDetails>();
            ImportDetails Detail;
            foreach (PurchaseOrderDetails Item in this.WSI.Details)
            {
                Detail = new ImportDetails();
                Detail.CopyFromPODetail(Item);

                importWSI.Details.Add(Detail);
            }

            importWSI.Supplier = this.WSI.Supplier;

            return importWSI;
        }

        public NMImportsWSI CreateImport(string poId)
        {
            NMPurchaseOrdersBL poBL = new NMPurchaseOrdersBL();
            NMPurchaseOrdersWSI poWsi = new NMPurchaseOrdersWSI();
            poWsi.Mode = "SEL_OBJ";
            poWsi.Order.Id = poId;

            poWsi = poBL.callSingleBL(poWsi);

            NMImportsWSI importWSI = new NMImportsWSI();
            if (poWsi.WsiError == string.Empty)
            {
                this.WSI = poWsi;
                importWSI = this.CreateImport();
            }
            else
                importWSI.WsiError = poWsi.WsiError;

            return importWSI;
        }

        public NMPurchaseOrdersWSI ImportFromPOrder(NMPurchaseOrdersWSI wsi)
        {
            try
            {
                //kiểm tra đã xuất kho cho đơn hàng hay chưa
                NMImportsBL ImportBL = new NMImportsBL();
                NMImportsWSI ImportWSI = new NMImportsWSI();
                ImportWSI.Mode = "SRC_OBJ";
                ImportWSI.Import.Reference = wsi.Order.Id;
                List<NMImportsWSI> impList = ImportBL.callListBL(ImportWSI);
                if (impList.Count <= 0)
                {
                    this.WSI = wsi;
                    ImportWSI = this.CreateImport();

                    ImportWSI.Mode = "SAV_OBJ";
                    ImportWSI.ActionBy = wsi.ActionBy;

                    ImportWSI = ImportBL.callSingleBL(ImportWSI);
                }
            }
            catch (Exception ex)
            {
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }

            return wsi;
        }

        public string ReconcileImportFromOrder(NMPurchaseOrdersWSI poWsi)
        {
            string error = "";
            try
            {
                NMImportsBL ImportBL = new NMImportsBL();
                NMImportsWSI ImportWSI = new NMImportsWSI();
                ImportWSI.Mode = "SRC_OBJ";
                ImportWSI.Import = new Imports();
                ImportWSI.Import.Reference = poWsi.Order.Id;
                List<NMImportsWSI> WSIs = ImportBL.callListBL(ImportWSI);
                if (WSIs.Count == 1)
                {
                    ImportWSI = new NMImportsWSI();
                    ImportWSI.Mode = "SAV_OBJ";
                    
                    ImportWSI.Import.ImportId = WSIs[0].Import.ImportId;
                    ImportWSI.Import.ImportDate = WSIs[0].Import.ImportDate;
                    ImportWSI.Import.ImportStatus = WSIs[0].Import.ImportStatus;
                    ImportWSI.Import.ImportTypeId = WSIs[0].Import.ImportTypeId;
                    ImportWSI.Import.DeliveryMethod = WSIs[0].Import.DeliveryMethod;

                    ImportWSI.Import.StockId = poWsi.Order.ImportStockId;
                    ImportWSI.Import.SupplierId = poWsi.Order.SupplierId;
                    ImportWSI.Import.Reference = poWsi.Order.Id;
                    ImportWSI.Import.Transport = poWsi.Order.Transportation;

                    ImportWSI.Details = new List<ImportDetails>();
                    ImportDetails Detail;
                    foreach (PurchaseOrderDetails Item in poWsi.Details)
                    {
                        Detail = WSIs[0].Details.Find(d => d.ProductId == Item.ProductId);
                        if (Detail == null)
                        {
                            Detail = new ImportDetails();
                        }
                        Detail.CopyFromPODetail(Item);
                        ImportWSI.Details.Add(Detail);
                    }

                    ImportWSI.ActionBy = poWsi.ActionBy;
                    ImportWSI = ImportBL.callSingleBL(ImportWSI);

                    error = ImportWSI.WsiError;
                }
                else
                {
                    error = "Có quá nhiều Hóa đơn!";
                }
            }
            catch (Exception ex)
            {
                error = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return error;
        }

        #endregion

        // tao phieu nhap
        #region Invoice

        public NMPurchaseInvoicesWSI CreatePurchaseInvoice()
        {
            NMPurchaseInvoicesWSI piWsi = new NMPurchaseInvoicesWSI();
            piWsi.Invoice.CopyFromPO(this.WSI.Order);

            if (NMCommon.GetSetting("USE_PODATE_FOR_PI_DATE"))                
                piWsi.Invoice.InvoiceDate = this.WSI.Order.OrderDate;

            PurchaseInvoiceDetails detail;
            piWsi.Details = new List<PurchaseInvoiceDetails>();
            foreach (PurchaseOrderDetails Item in this.WSI.Details)
            {
                detail = new PurchaseInvoiceDetails();
                detail.CopyFromPODetail(Item);

                piWsi.Details.Add(detail);
            }

            piWsi.Invoice.Amount = this.WSI.Details.Sum(i=>i.Amount);
            piWsi.Invoice.Discount = this.WSI.Details.Sum(i=>i.DiscountAmount);
            piWsi.Invoice.Tax = this.WSI.Details.Sum(i=>i.TaxAmount);
            piWsi.Invoice.TotalAmount = this.WSI.Details.Sum(i=>i.TotalAmount);

            //  tra ve them Supplier
            piWsi.Supplier = this.WSI.Supplier;

            return piWsi;
        }

        public NMPurchaseInvoicesWSI CreatePurchaseInvoice(string poId)
        {
            NMPurchaseOrdersBL POBL = new NMPurchaseOrdersBL();
            NMPurchaseOrdersWSI PO = new NMPurchaseOrdersWSI();
            PO.Mode = "SEL_OBJ";
            PO.Order.Id = poId;
            PO = POBL.callSingleBL(PO);

            NMPurchaseInvoicesWSI piWsi = new NMPurchaseInvoicesWSI();
            if (PO.WsiError == string.Empty)
            {
                this.WSI = PO;
                piWsi = this.CreatePurchaseInvoice();
            }
            else
                piWsi.WsiError = PO.WsiError;
            
            return piWsi;
        }

        public String PInvoiceFromPOrder(NMPurchaseOrdersWSI POWSI)
        {
            string err = "";
            try
            {
                NMPurchaseInvoicesBL PIBL = new NMPurchaseInvoicesBL();
                NMPurchaseInvoicesWSI PIWSI = new NMPurchaseInvoicesWSI();
                PIWSI.Mode = "SRC_OBJ";
                PIWSI.Invoice.Reference = POWSI.Order.Id;
                List<NMPurchaseInvoicesWSI> PIs = PIBL.callListBL(PIWSI);

                if (PIs.Count <= 0)
                {
                    this.WSI = POWSI;
                    PIWSI = this.CreatePurchaseInvoice();

                    PIWSI.Mode = "SAV_OBJ";
                    PIWSI.ActionBy = POWSI.ActionBy;

                    PIWSI = PIBL.callSingleBL(PIWSI);
                    err = PIWSI.WsiError;
                }
            }
            catch (Exception ex)
            {
                err = "Có lỗi xảy ra: " + ex.Message + "\n" + ex.InnerException;
            }
            return err;
        }

        public string ReconcileInvoiceFromOrder(NMPurchaseOrdersWSI poWSI)
        {
            string error = "";
            try
            {
                NMPurchaseInvoicesBL BL = new NMPurchaseInvoicesBL();
                NMPurchaseInvoicesWSI WSI = new NMPurchaseInvoicesWSI();
                WSI.Mode = "SRC_OBJ";
                WSI.Invoice.Reference = poWSI.Order.Id;
                List<NMPurchaseInvoicesWSI> PIs = BL.callListBL(WSI);
                if (PIs.Count == 1)
                {
                    WSI.Mode = "SAV_OBJ";
                    WSI.Invoice.InvoiceId = PIs[0].Invoice.InvoiceId;
                    
                    if (NMCommon.GetSetting("USE_PODATE_FOR_PI_DATE"))
                        WSI.Invoice.InvoiceDate = poWSI.Order.OrderDate;
                    else
                        WSI.Invoice.InvoiceDate = PIs[0].Invoice.InvoiceDate;

                    WSI.Invoice.InvoiceStatus = PIs[0].Invoice.InvoiceStatus;
                    WSI.Invoice.SupplierId = poWSI.Order.SupplierId;
                    WSI.Invoice.StockId = poWSI.Order.ImportStockId;
                    WSI.Invoice.CurrencyId = "VND";
                    WSI.Invoice.ExchangeRate = 1;
                    WSI.Invoice.DescriptionInVietnamese = poWSI.Order.Description;
                    WSI.Invoice.BuyerId = poWSI.Order.BuyerId;
                    WSI.Invoice.InvoiceTypeId = poWSI.Order.InvoiceTypeId;
                    WSI.Invoice.SupplierReference = poWSI.Order.Reference;
                    WSI.ActionBy = poWSI.ActionBy;

                    PurchaseInvoiceDetails detail;
                    WSI.Details = new List<PurchaseInvoiceDetails>();
                    PIs[0].Details = PIs[0].Invoice.DetailsList.ToList();
                    foreach (PurchaseOrderDetails Item in poWSI.Details)
                    {
                        detail = PIs[0].Details.Find(d => d.ProductId == Item.ProductId);
                        if (detail == null)
                        {
                            detail = new PurchaseInvoiceDetails();
                        }

                        detail.CopyFromPODetail(Item);
                        WSI.Details.Add(detail);
                    }
                    WSI = BL.callSingleBL(WSI);

                }
            }
            catch (Exception ex)
            {
                error = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return error;
        }

        #endregion
    }
}
