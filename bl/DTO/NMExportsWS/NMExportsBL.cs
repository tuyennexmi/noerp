// NMExportsBL.cs

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
using System.Collections;
using NHibernate;
using NEXMI;

namespace NEXMI
{
    public class NMExportsBL
    {
        private readonly ISession Session;
        
        public NMExportsWSI WSI = new NMExportsWSI();

        public NMExportsBL()
        {
            this.Session = SessionFactory.GetNewSession();
        }

        public NMExportsBL(ISession ses)
        {
            this.Session = ses;
        }

        public NMExportsWSI callSingleBL(NMExportsWSI wsi)
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

        public List<NMExportsWSI> callListBL(NMExportsWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMExportsWSI>();
            }
        }

        public NMExportsWSI SelectObject(NMExportsWSI wsi)
        {
            try
            {
                ExportsAccesser Accesser = new ExportsAccesser(Session);                
                Exports obj = Accesser.GetAllExportsByID(wsi.Export.ExportId, false);

                if (obj != null)
                {
                    CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
                    StocksAccesser StockAccesser = new StocksAccesser(Session);
                    ExportDetailsAccesser detailAccesser = new ExportDetailsAccesser(Session);

                    wsi.Export = obj;
                    wsi.Details = detailAccesser.GetAllExportdetailsByExportId(obj.ExportId, true).ToList();
                    if (!String.IsNullOrEmpty(obj.CustomerId))
                    {
                        wsi.Customer = CustomerAccesser.GetAllCustomersByID(obj.CustomerId, true);
                    }
                    wsi.CreatedBy = CustomerAccesser.GetAllCustomersByID(obj.CreatedBy, true);

                    if (NMCommon.GetSetting("SERIALNUMBER"))
                    {
                        ProductSNsAccesser SNAccesser = new ProductSNsAccesser(Session);
                        wsi.ProductSNs = SNAccesser.GetAllProductSNsByReferenceId(obj.ExportId, true).ToList();
                    }

                    if (!String.IsNullOrEmpty(obj.CarrierId))
                    {
                        wsi.Carrier = CustomerAccesser.GetAllCustomersByID(obj.CarrierId, true);
                    }
                    NMStocksBL StockBL = new NMStocksBL();
                    NMStocksWSI StockWSI = new NMStocksWSI();
                    StockWSI.Mode = "SEL_OBJ";
                    StockWSI.Id = obj.StockId;
                    wsi.Stock = StockBL.callSingleBL(StockWSI);
                    if (!String.IsNullOrEmpty(obj.ImportStockId))
                    {
                        StockWSI = new NMStocksWSI();
                        StockWSI.Mode = "SEL_OBJ";
                        StockWSI.Id = obj.ImportStockId;
                        wsi.ImportStock = StockBL.callSingleBL(StockWSI);
                    }
                    NMProductsBL ProductBL = new NMProductsBL();
                    NMProductsWSI ProductWSI = new NMProductsWSI();
                    ProductWSI.Mode = "SRC_BY_EXP";
                    ProductWSI.Keyword = wsi.Export.ExportId;
                    wsi.ProductWSIs = ProductBL.callListBL(ProductWSI);
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

        public NMExportsWSI SaveObject(NMExportsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                ExportsAccesser Accesser = new ExportsAccesser(Session);
                ExportDetailsAccesser DetailAccesser = new ExportDetailsAccesser(Session);
                AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                ProductSNsAccesser SNAccesser = new ProductSNsAccesser(Session);
                PricesForSalesInvoiceAccesser PriceAccesser = new PricesForSalesInvoiceAccesser(Session);
                
                IList<ProductSNs> SNsTemp = new List<ProductSNs>();
                ExportDetails OldDetail, Detail;
                Exports obj = Accesser.GetAllExportsByID(wsi.Export.ExportId, false);

                bool flag = false;
                //da ton tai => cap nhat
                #region Da luu

                if (obj != null)
                {
                    List<ExportDetails> OldDetails = obj.ExportdetailsList.Cast<ExportDetails>().ToList();
                    if (NMCommon.GetSetting(NMConstant.Settings.SERIALNUMBER))
                    {
                        SNsTemp = SNAccesser.GetAllProductSNsByReferenceId(obj.ExportId, true);
                    }
                    Session.Evict(obj);

                    if (wsi.Export.ExportTypeId == NMConstant.ExportType.Transfer)
                    {
                        wsi.Export.CustomerId = "COMPANY";
                    }
                    //chỉ cập nhật mà không giao
                    wsi.Export.CreatedDate = obj.CreatedDate;
                    wsi.Export.CreatedBy = obj.CreatedBy;
                    wsi.Export.ModifiedDate = DateTime.Now;
                    wsi.Export.ModifiedBy = wsi.ActionBy;

                    if (wsi.Export.ExportStatus != obj.ExportStatus)
                    {
                        if (NMCommon.GetSetting("UPDATE_EXPORT_DATE_BY_ACTION"))
                            wsi.Export.ExportDate = DateTime.Now;
                    }
                    String rs = wsi.CompareTo(obj);
                    if (rs != "") NMMessagesBL.SaveMessage(Session, wsi.Export.ExportId, "cập nhật thông tin", rs, wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);

                    Accesser.UpdateExports(wsi.Export);

                    #region chi tiet

                    foreach (ExportDetails Item in wsi.Details)
                    {
                        Item.StockId = wsi.Export.StockId;
                        Item.ExportId = wsi.Export.ExportId;
                        OldDetail = OldDetails.Select(i => i).Where(i => i.OrdinalNumber == Item.OrdinalNumber).FirstOrDefault();
                        if (OldDetail != null)
                        {
                            Item.CreatedDate = OldDetail.CreatedDate;
                            Item.CreatedBy = OldDetail.CreatedBy;
                            Item.ModifiedDate = DateTime.Now;
                            Item.ModifiedBy = wsi.ActionBy;

                            DetailAccesser.UpdateExportdetails(Item);
                        }
                        else
                        {
                            Item.CreatedDate = DateTime.Now;
                            Item.CreatedBy = wsi.ActionBy;
                            Item.ModifiedDate = DateTime.Now;
                            Item.ModifiedBy = wsi.ActionBy;

                            DetailAccesser.InsertExportdetails(Item);
                        }
                    }
                    if (OldDetails != null)
                    {
                        flag = true;
                        foreach (ExportDetails Old in obj.ExportdetailsList)
                        {
                            flag = true;
                            foreach (ExportDetails New in wsi.Details)
                            {
                                if (Old.OrdinalNumber == New.OrdinalNumber)
                                {
                                    flag = false;
                                    break;
                                }
                            }
                            if (flag)
                            {
                                DetailAccesser.DeleteExportdetails(Old);
                            }
                        }
                    }
                    #endregion
                }
                #endregion

                //chua ton tai => tao moi
                #region Tao moi
                else
                {
                    wsi.Export.ExportId = AutomaticValueAccesser.AutoGenerateId("Exports");
                    //wsi.Export.ExportDate = DateTime.Today;
                    if (wsi.Export.ExportTypeId == NMConstant.ExportType.Transfer)
                    {
                        wsi.Export.CustomerId = "COMPANY";
                    }
                    wsi.Export.CreatedDate = DateTime.Now;
                    wsi.Export.CreatedBy = wsi.ActionBy;
                    wsi.Export.ModifiedDate = DateTime.Now;
                    wsi.Export.ModifiedBy = wsi.ActionBy;

                    Accesser.InsertExports(wsi.Export);
                    NMMessagesBL.SaveMessage(Session, wsi.Export.ExportId, "khởi tạo tài liệu", "", wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);

                    foreach (ExportDetails Item in wsi.Details)
                    {
                        Item.OrdinalNumber = 0;
                        Item.StockId = wsi.Export.StockId;
                        Item.ExportId = wsi.Export.ExportId;

                        Item.CreatedDate = DateTime.Now;
                        Item.CreatedBy = wsi.ActionBy;
                        Item.ModifiedDate = DateTime.Now;
                        Item.ModifiedBy = wsi.ActionBy;

                        DetailAccesser.InsertExportdetails(Item);
                    }
                    if (wsi.ProductSNs != null)
                    {
                        foreach (ProductSNs SN in wsi.ProductSNs)
                        {
                            SN.ExportId = wsi.Export.ExportId;
                            SN.StockId = wsi.Export.StockId;
                            SNAccesser.InsertProductSNs(SN);
                        }
                    }
                }
                #endregion

                #region Serial
                if (wsi.ProductSNs != null)
                {
                    ProductSNs SNTemp;
                    foreach (ProductSNs SN in wsi.ProductSNs)
                    {
                        SNTemp = SNAccesser.GetAllProductSNsByID(SN.ProductId, SN.SerialNumber, true);
                        if (SNTemp == null)
                        {
                            SN.StockId = wsi.Export.StockId;
                            SNAccesser.InsertProductSNs(SN);
                        }
                    }

                    foreach (ProductSNs Old in SNsTemp)
                    {
                        flag = true;
                        if (wsi.ProductSNs != null)
                        {
                            foreach (ProductSNs New in wsi.ProductSNs)
                            {
                                if (Old.ProductId == New.ProductId && Old.SerialNumber == New.SerialNumber)
                                {
                                    flag = false;
                                    break;
                                }
                            }
                            if (flag)
                            {
                                SNAccesser.DeleteProductSNs(Old);
                            }
                        }
                    }
                }
                #endregion

                #region Giao hang
                //nếu đã giao hàng
                if (wsi.Export.ExportStatus == NMConstant.EXStatuses.Delivered)
                {
                    List<ExportDetails> TempDetails = new List<ExportDetails>();
                    foreach (ExportDetails Item in wsi.Details)
                    {
                        if (Item.Quantity < Item.RequiredQuantity)
                        {
                            //lập danh sách hàng chưa xuất hết
                            Detail = new ExportDetails();
                            Detail.Copy(Item);                            
                            Detail.RequiredQuantity = Item.RequiredQuantity - Item.Quantity;
                            Detail.Quantity = Detail.RequiredQuantity;
                            
                            Detail.CreatedDate = DateTime.Now;
                            Detail.CreatedBy = wsi.ActionBy;
                            Detail.ModifiedDate = DateTime.Now;
                            Detail.ModifiedBy = wsi.ActionBy;

                            TempDetails.Add(Detail);
                        }
                        if (Item.Quantity > 0)
                        {
                            //NMCommon.UpdateStock(Session, null, Item, wsi.Export.ExportTypeId);
                            // luu cac serial
                            if ((NMCommon.GetSetting(NMConstant.Settings.SERIALNUMBER)) && (wsi.ProductSNs != null))
                            {
                                    for (int i = 0; i < Item.Quantity; i++)
                                    {
                                        wsi.ProductSNs.Remove(wsi.ProductSNs.Select(x => x).Where(x => x.ProductId == Item.ProductId).FirstOrDefault());
                                    }
                            }
                        }
                        else
                        {
                            wsi.Export.ExportdetailsList.Remove(Item);
                            DetailAccesser.DeleteExportdetails(Item);
                        }
                    }
                    //  cập nhật phiếu xuất
                    //Accesser.UpdateExports(wsi.Export);
                    //  ghi sổ NKC -> giá vốn hàng bán                            
                    this.WriteGeneralJournals(wsi);
                    //  cập nhật trạng thái
                    NMMessagesBL.SaveMessage(Session, wsi.Export.ExportId, "thay đổi trạng thái", "Đã giao hàng", wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
                    //  cập nhật trạng thái SO liên quan
                    if (!string.IsNullOrEmpty(wsi.Export.Reference))
                    {
                        NMCommon.UpdateObjectStatus(this.Session, "SalesOrders", wsi.Export.Reference, NMConstant.SOStatuses.Delivery, null, wsi.ActionBy, "Trạng thái: => Đã giao hàng.\n");
                    }

                    //Lưu phiếu mới
                    if (TempDetails.Count > 0)
                    {
                        Exports NewEx = new Exports();
                        NewEx.ExportId = AutomaticValueAccesser.AutoGenerateId("Exports");
                        NewEx.BackOrderOf = wsi.Export.ExportId;
                        NewEx.CarrierId = wsi.Export.CarrierId;

                        NewEx.CreatedBy = wsi.ActionBy;
                        NewEx.CreatedDate = DateTime.Now;
                        NewEx.ModifiedBy = wsi.ActionBy;
                        NewEx.ModifiedDate = DateTime.Now;

                        NewEx.CustomerId = wsi.Export.CustomerId;
                        NewEx.DeliveryMethod = wsi.Export.DeliveryMethod;
                        NewEx.DescriptionInVietnamese = wsi.Export.DescriptionInVietnamese;
                        NewEx.ExportBatchId = wsi.Export.ExportBatchId;
                        NewEx.ExportDate = DateTime.Today;
                        NewEx.ExportStatus = obj.ExportStatus;
                        NewEx.ExportTypeId = wsi.Export.ExportTypeId;
                        NewEx.ImportStockId = wsi.Export.ImportStockId;
                        NewEx.InvoiceId = wsi.Export.InvoiceId;
                        NewEx.Reference = wsi.Export.Reference;
                        NewEx.StockId = wsi.Export.StockId;
                        NewEx.Transport = wsi.Export.Transport;
                        Accesser.InsertExports(NewEx);
                        foreach (ExportDetails Item in TempDetails)
                        {
                            Item.ExportId = NewEx.ExportId;
                            Item.RequiredQuantity = Item.Quantity;
                            DetailAccesser.InsertExportdetails(Item);
                        }
                        if (wsi.ProductSNs != null)
                        {
                            foreach (ProductSNs SN in wsi.ProductSNs)
                            {
                                SN.ImportId = NewEx.ExportId;
                                SN.StockId = NewEx.StockId;
                                SNAccesser.UpdateProductSNs(SN);
                            }
                        }
                        NMMessagesBL.SaveMessage(Session, NewEx.ExportId, "khởi tạo tài liệu", "", wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
                        wsi.Export = NewEx;
                    }
                }
                #endregion

                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            
            return wsi;
        }

        public NMExportsWSI DeleteObject(NMExportsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                ExportsAccesser Accesser = new ExportsAccesser(Session);
                Exports obj = Accesser.GetAllExportsByID(wsi.Export.ExportId, false);
                if (obj != null)
                {
                    if (obj.ExportStatus != NMConstant.EXStatuses.Delivered)
                    {
                        Accesser.DeleteExports(obj);
                        try
                        {
                            tx.Commit();
                        }
                        catch
                        {
                            tx.Rollback();
                            wsi.WsiError = "Không được xóa phiếu này.";
                        }
                    }
                    else
                    {
                        wsi.WsiError = "Không được xóa phiếu này.";
                    }
                }
                else
                {
                    wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
                }
            }
            catch (Exception ex)
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return wsi;
        }

        public void CheckProductInInventoryControl(ExportDetails newObj)
        {
            MonthlyInventoryControlAccesser Accesser = new MonthlyInventoryControlAccesser(Session);
            MonthlyInventoryControl obj = Accesser.GetAllMonthlyinventorycontrolByStockIdAndProductId(newObj.StockId, newObj.ProductId, true);
            if (obj != null)
            {
                obj = new MonthlyInventoryControl();
                obj.StockId = newObj.StockId;
                obj.ProductId = newObj.ProductId;
                obj.CreatedDate = DateTime.Now;
                obj.CreatedBy = newObj.CreatedBy;
                Accesser.InsertMonthlyinventorycontrol(obj);
            }
        }

        public List<NMExportsWSI> SearchObject(NMExportsWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (wsi.Export != null)
            {
                if (!string.IsNullOrEmpty(wsi.Export.Reference))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.Reference = :Reference";
                    ListCriteria.Add("Reference", wsi.Export.Reference);
                }
                if (!string.IsNullOrEmpty(wsi.Export.CustomerId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.CustomerId = :CustomerId";
                    ListCriteria.Add("CustomerId", wsi.Export.CustomerId);
                }
                if (!string.IsNullOrEmpty(wsi.Export.ExportTypeId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.ExportTypeId = :ExportTypeId";
                    ListCriteria.Add("ExportTypeId", wsi.Export.ExportTypeId);
                }
                if (!string.IsNullOrEmpty(wsi.Export.StockId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.StockId = :StockId";
                    ListCriteria.Add("StockId", wsi.Export.StockId);
                }
                if (!string.IsNullOrEmpty(wsi.Export.ExportStatus))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.ExportStatus = :ExportStatus";
                    ListCriteria.Add("ExportStatus", wsi.Export.ExportStatus);
                }
            }
            if (!string.IsNullOrEmpty(wsi.FromDate))
            {   
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.ExportDate >= :FromDate ";
                ListCriteria.Add("FromDate", DateTime.Parse(wsi.FromDate));
            }
            if (!string.IsNullOrEmpty(wsi.ToDate))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.ExportDate <= :ToDate";
                ListCriteria.Add("ToDate", DateTime.Parse(wsi.ToDate));
            }
            if (!string.IsNullOrEmpty(wsi.ActionBy))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.CreatedBy = :ActionBy";
                ListCriteria.Add("ActionBy", wsi.ActionBy);
            }
            if (!string.IsNullOrEmpty(wsi.Keyword))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " (O.ExportId LIKE :Keyword OR O.Reference LIKE :Keyword OR O.CustomerId in (SELECT C.CustomerId FROM Customers AS C WHERE C.CustomerId LIKE :Keyword OR C.CompanyNameInVietnamese LIKE :Keyword OR C.TaxCode LIKE :Keyword OR C.Cellphone LIKE :Keyword))";
                ListCriteria.Add("Keyword", wsi.Keyword);
            }
            string strCmdCounter = "SELECT COUNT(O.ExportId) FROM Exports AS O" + strCriteria;
            IQuery queryCounter = Session.CreateSQLQuery(strCmdCounter);
            if (wsi.SortField != null && wsi.SortOrder != "")
            {
                strCriteria += " ORDER BY O." + wsi.SortField + " " + wsi.SortOrder;
            }
            else
            {
                strCriteria += " ORDER BY O.ExportDate DESC";
            }
            string strCmd = "SELECT O FROM Exports AS O" + strCriteria;
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
            List<NMExportsWSI> ListWSI = new List<NMExportsWSI>();
            ExportsAccesser Accesser = new ExportsAccesser(Session);
            CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
            StocksAccesser StockAccesser = new StocksAccesser(Session);
            StatusesAccesser StatusAccesser = new StatusesAccesser(Session);
            TypesAccesser TypeAccesser = new TypesAccesser(Session);
            IList<Exports> objs;
            int totalRows = NMCommon.Counter(queryCounter);
            if (wsi.PageSize != 0)
            {
                objs = Accesser.GetExportsByQuery(query, wsi.PageSize, wsi.PageNum, false);
            }
            else
            {
                objs = Accesser.GetExportsByQuery(query, false);
            }

            bool IsManageSerial = NMCommon.GetSetting("SERIALNUMBER");
            ProductSNsAccesser SNAccesser = new ProductSNsAccesser(Session);

            foreach (Exports obj in objs)
            {
                wsi = new NMExportsWSI();
                wsi.Export = obj;
                wsi.Details = obj.ExportdetailsList.Cast<ExportDetails>().ToList();
                wsi.Customer = CustomerAccesser.GetAllCustomersByID(obj.CustomerId, true);
                wsi.CreatedBy = CustomerAccesser.GetAllCustomersByID(obj.CreatedBy, true);
                //wsi.Status = StatusAccesser.GetAllStatusesByID(obj.ExportStatus, true);
                //wsi.ExportType = TypeAccesser.GetAllTypesByID(obj.ExportTypeId, true);
                wsi.Carrier = CustomerAccesser.GetAllCustomersByID(obj.CarrierId, true);
                
                if (IsManageSerial)
                    wsi.ProductSNs = SNAccesser.GetAllProductSNsByReferenceId(obj.ExportId, true).ToList();

                //NMStocksBL StockBL = new NMStocksBL();
                //NMStocksWSI StockWSI = new NMStocksWSI();
                //StockWSI.Mode = "SEL_OBJ";
                //StockWSI.Id = obj.StockId;
                //wsi.Stock = StockBL.callSingleBL(StockWSI);
                //if (!String.IsNullOrEmpty(obj.ImportStockId))
                //{
                //    StockWSI = new NMStocksWSI();
                //    StockWSI.Mode = "SEL_OBJ";
                //    StockWSI.Id = obj.ImportStockId;
                //    wsi.ImportStock = StockBL.callSingleBL(StockWSI);
                //}

                ListWSI.Add(wsi);
            }
            if (ListWSI.Count > 0)
                ListWSI[0].TotalRows = totalRows;
            return ListWSI;
        }

        private void InvoiceFromExport(NMExportsWSI wsi)
        {
            SalesInvoicesAccesser Accesser = new SalesInvoicesAccesser(Session);
            SalesInvoiceDetailsAccesser DetailAccesser = new SalesInvoiceDetailsAccesser(Session);
            List<SalesInvoiceDetails> OldDetails = null;
            SalesInvoiceDetails detail;
            SalesInvoices Invoice = new SalesInvoices();
            
            AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
            Invoice.InvoiceId = AutomaticValueAccesser.AutoGenerateId("SalesInvoices");
            //Invoice.Discount = wsi.Details.Sum(i => i.DiscountAmount);
            //Invoice.Tax = wsi.Details.Sum(i => i.TaxAmount);
            //Invoice.Amount = wsi.Details.Sum(i => i.Amount);
            //Invoice.TotalAmount = wsi.Details.Sum(i => i.TotalAmount) + wsi.Invoice.ShipCost + wsi.Invoice.OtherCost;
            Invoice.CreatedDate = DateTime.Now;
            Invoice.CreatedBy = wsi.ActionBy;
            Invoice.ModifiedDate = DateTime.Now;
            Invoice.ModifiedBy = wsi.ActionBy;

            Accesser.InsertSalesinvoices(Invoice);
            NMMessagesBL.SaveMessage(Session, Invoice.InvoiceId, "khởi tạo tài liệu", "", wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
            
            foreach (ExportDetails Item in wsi.Details)
            {
                detail = new SalesInvoiceDetails();
                detail.CopyFromExportDetail(Item);
                detail.InvoiceId = Invoice.InvoiceId;

                detail.CreatedBy = wsi.ActionBy;
                detail.CreatedDate = DateTime.Now;
                detail.ModifiedBy = wsi.ActionBy;
                detail.ModifiedDate = DateTime.Now;

                DetailAccesser.InsertSalesinvoicedetails(detail);
            }

            return;
        }

        private void WriteGeneralJournals(NMExportsWSI wsi)
        {
            MonthlyGeneralJournals MGJ = new MonthlyGeneralJournals();
            MonthlyGeneralJournalsAccesser mgjAccesser = new MonthlyGeneralJournalsAccesser(this.Session);

            mgjAccesser.DeletemonthlygeneraljournalsByExport(wsi.Export.ExportId);

            Products pro = new Products();
            ProductsAccesser proAcc = new ProductsAccesser(this.Session);
            double waPrice = 0;
            foreach (ExportDetails itm in wsi.Details)
            {
                pro = proAcc.GetAllProductsByID(itm.ProductId, true);
                waPrice = mgjAccesser.ICExportPriceCalculate(itm.StockId, itm.ProductId, pro.TypeId);

                if (wsi.Export.ExportTypeId == NMConstant.ExportType.ForCustomers)
                {
                    //ghi nợ    => Gia von hang ban
                    MGJ = new MonthlyGeneralJournals();
                    MGJ.EXID = wsi.Export.ExportId;
                    MGJ.IssueDate = wsi.Export.ExportDate;
                    MGJ.PartnerId = wsi.Export.CustomerId;
                    if (wsi.Export.ExportTypeId == NMConstant.ExportType.Transfer)
                        if (string.IsNullOrEmpty(wsi.Export.CustomerId))
                            MGJ.PartnerId = "COMPANY";
                    MGJ.StockId = wsi.Export.StockId;
                    //MGJ.SIID = wsi.Export.Reference;
                    MGJ.ActionBy = wsi.ActionBy;

                    MGJ.CreatedBy = wsi.ActionBy;
                    MGJ.ModifiedBy = wsi.ActionBy;

                    MGJ.CurrencyId = "VND";
                    MGJ.ExchangeRate = 1;
                    MGJ.IsBegin = false;

                    MGJ.ProductId = itm.ProductId;
                    MGJ.UnitId = itm.UnitId;    // pro.ProductUnit;
                    MGJ.AccountId = "632";
                    
                    MGJ.ImportQuantity = itm.Quantity;
                    MGJ.ExportQuantity = 0;
                    MGJ.DebitAmount = itm.Quantity * waPrice;
                    MGJ.CreditAmount = 0;
                    MGJ.Descriptions = "Ghi nhận tăng Giá vốn hàng bán";
                    mgjAccesser.InsertMonthlygeneraljournals(MGJ);
                }
                //ghi có => Ton kho hang hoa
                MGJ = new MonthlyGeneralJournals();
                MGJ.EXID = wsi.Export.ExportId;
                MGJ.IssueDate = wsi.Export.ExportDate;
                MGJ.PartnerId = wsi.Export.CustomerId;
                if (wsi.Export.ExportTypeId == NMConstant.ExportType.Transfer)
                    if (string.IsNullOrEmpty(wsi.Export.CustomerId))
                        MGJ.PartnerId = "COMPANY";
                MGJ.StockId = wsi.Export.StockId;
                MGJ.ActionBy = wsi.ActionBy;

                MGJ.CreatedBy = wsi.ActionBy;
                MGJ.ModifiedBy = wsi.ActionBy;

                MGJ.CurrencyId = "VND";
                MGJ.ExchangeRate = 1;
                MGJ.IsBegin = false;

                MGJ.ProductId = itm.ProductId;
                MGJ.UnitId = itm.UnitId;    // pro.ProductUnit;
                MGJ.AccountId = pro.TypeId;
                
                MGJ.ImportQuantity = 0;
                MGJ.ExportQuantity = itm.Quantity;
                MGJ.DebitAmount = 0;
                MGJ.CreditAmount = itm.Quantity * waPrice;
                MGJ.Descriptions = "Ghi nhận giảm Tồn kho hàng hóa";
                mgjAccesser.InsertMonthlygeneraljournals(MGJ);
            }

            return;
        }

        //public void FromSalesOrder(NMSalesOrdersWSI SOWSI)
        //{   
        //    this.WSI.Export.CopyFromSO(SOWSI.Order);

        //    if (NMCommon.GetSetting("USE_SODATE_FOR_EXPORT_DATE"))
        //        WSI.Export.ExportDate = SOWSI.Order.OrderDate;

        //    this.WSI.Details = new List<ExportDetails>();
        //    ExportDetails Detail;
        //    foreach (SalesOrderDetails Item in SOWSI.Details)
        //    {
        //        Detail = new ExportDetails();
        //        Detail.CopyFromSODetail(Item);

        //        this.WSI.Details.Add(Detail);
        //    }
        //}

        //public string FromSalesOrder(string soId)
        //{
        //    NMSalesOrdersBL SOBL = new NMSalesOrdersBL();
        //    NMSalesOrdersWSI SOWSI = new NMSalesOrdersWSI();
        //    SOWSI.Mode = "SEL_OBJ";

        //    SOWSI.Order.OrderId = soId;
        //    SOWSI = SOBL.callSingleBL(SOWSI);

        //    if (SOWSI.WsiError == string.Empty)
        //    {
        //        this.FromSalesOrder(SOWSI);
        //    }

        //    return SOWSI.WsiError;
        //}

        // tao Hoa don

        #region Invoice

        public NMSalesInvoicesWSI CreateInvoice()
        {
            NMSalesInvoicesWSI siWsi = new NMSalesInvoicesWSI();

            return siWsi;
        }

        public NMSalesInvoicesWSI CreateInvoice(string exId)
        {
            NMSalesInvoicesWSI siWsi = new NMSalesInvoicesWSI();

            return siWsi;
        }


        #endregion

    }
}
