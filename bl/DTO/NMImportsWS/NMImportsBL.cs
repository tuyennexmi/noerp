// NMImportsBL.cs

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

namespace NEXMI
{
    public class NMImportsBL
    {
        private readonly ISession Session;

        public NMImportsWSI WSI = new NMImportsWSI();

        public NMImportsBL()
        {
            this.Session = SessionFactory.GetNewSession();
        }

        public NMImportsBL(ISession ses)
        {
            this.Session = ses;
        }

        public NMImportsWSI callSingleBL(NMImportsWSI wsi)
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

        public List<NMImportsWSI> callListBL(NMImportsWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMImportsWSI>();
            }
        }

        public NMImportsWSI SelectObject(NMImportsWSI wsi)
        {
            try
            {
                ImportsAccesser Accesser = new ImportsAccesser(Session);                
                Imports obj;
                obj = Accesser.GetAllImportsByID(wsi.Import.ImportId, false);
                if (obj != null)
                {
                    CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
                    StocksAccesser StockAccesser = new StocksAccesser(Session);
                    //StatusesAccesser StatusAccesser = new StatusesAccesser(Session);
                    //TypesAccesser TypeAccesser = new TypesAccesser(Session);
                    
                    ImportDetailsAccesser detailAccesser = new ImportDetailsAccesser(Session);

                    wsi.Import = obj;
                    wsi.Details = detailAccesser.GetAllImportdetailsByImportId(obj.ImportId, true).ToList();   //.ImportDetailsList.Cast<ImportDetails>().ToList();
                    if (!String.IsNullOrEmpty(obj.SupplierId))
                    {
                        wsi.Supplier = CustomerAccesser.GetAllCustomersByID(obj.SupplierId, true);
                    }
                    wsi.CreatedUser = CustomerAccesser.GetAllCustomersByID(obj.CreatedBy, true);
                    //wsi.Status = StatusAccesser.GetAllStatusesByID(obj.ImportStatus, true);
                    //wsi.ImportType = TypeAccesser.GetAllTypesByID(obj.ImportTypeId, true);
                    if (NMCommon.GetSetting("SERIALNUMBER"))
                    {
                        ProductSNsAccesser SNAccesser = new ProductSNsAccesser(Session);
                        wsi.ProductSNs = SNAccesser.GetAllProductSNsByReferenceId(obj.ImportId, true).ToList();
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
                    if (!String.IsNullOrEmpty(obj.ExportStockId))
                    {
                        StockWSI = new NMStocksWSI();
                        StockWSI.Mode = "SEL_OBJ";
                        StockWSI.Id = obj.ExportStockId;
                        wsi.ExportStock = StockBL.callSingleBL(StockWSI);
                    }
                    NMProductsBL ProductBL = new NMProductsBL();
                    NMProductsWSI ProductWSI = new NMProductsWSI();
                    ProductWSI.Mode = "SRC_BY_IMP";
                    ProductWSI.Keyword = wsi.Import.ImportId;
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

        public NMImportsWSI SaveObject(NMImportsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                ImportsAccesser Accesser = new ImportsAccesser(Session);
                ImportDetailsAccesser DetailAccesser = new ImportDetailsAccesser(Session);
                AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                ProductSNsAccesser SNAccesser = new ProductSNsAccesser(Session);
                IList<ProductSNs> SNsTemp = new List<ProductSNs>();
                ImportDetails OldDetail, Detail;
                Imports obj = Accesser.GetAllImportsByID(wsi.Import.ImportId, false);
                bool flag = false;

                #region da ton tai
                if (obj != null)
                {
                    List<ImportDetails> OldDetails = obj.ImportDetailsList.Cast<ImportDetails>().ToList();
                    if (NMCommon.GetSetting(NMConstant.Settings.SERIALNUMBER))
                    {
                        SNsTemp = SNAccesser.GetAllProductSNsByReferenceId(obj.ImportId, true);
                    }
                    Session.Evict(obj);

                    wsi.Import.CreatedDate = obj.CreatedDate;
                    wsi.Import.CreatedBy = obj.CreatedBy;
                    wsi.Import.ModifiedBy = wsi.ActionBy;
                    wsi.Import.ModifiedDate = DateTime.Now;

                    if (wsi.Import.ImportStatus != obj.ImportStatus)
                    {
                        if (NMCommon.GetSetting("UPDATE_IMPORT_DATE_BY_ACTION"))
                            wsi.Import.ImportDate = DateTime.Now;
                    }
                    String rs = wsi.CompareTo(obj);
                    if (rs != "") NMMessagesBL.SaveMessage(Session, wsi.Import.ImportId, "cập nhật thông tin", rs, wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);

                    Accesser.UpdateImports(wsi.Import);

                    #region Chi tiết
                    foreach (ImportDetails Item in wsi.Details)
                    {
                        Item.StockId = wsi.Import.StockId;
                        Item.ImportId = wsi.Import.ImportId;
                        OldDetail = OldDetails.Select(i => i).Where(i => i.OrdinalNumber == Item.OrdinalNumber).FirstOrDefault();
                        if (OldDetail != null)
                        {
                            Item.CreatedDate = OldDetail.CreatedDate;
                            Item.CreatedBy = OldDetail.CreatedBy;
                            Item.ModifiedDate = DateTime.Now;
                            Item.ModifiedBy = wsi.ActionBy;

                            DetailAccesser.UpdateImportdetails(Item);
                        }
                        else
                        {
                            Item.CreatedDate = DateTime.Now;
                            Item.CreatedBy = wsi.ActionBy;
                            Item.ModifiedDate = DateTime.Now;
                            Item.ModifiedBy = wsi.ActionBy;

                            DetailAccesser.InsertImportdetails(Item);
                        }
                    }
                    if (OldDetails != null)
                    {
                        flag = true;
                        foreach (ImportDetails Old in obj.ImportDetailsList)
                        {
                            flag = true;
                            foreach (ImportDetails New in wsi.Details)
                            {
                                if (Old.OrdinalNumber == New.OrdinalNumber)
                                {
                                    flag = false;
                                    break;
                                }
                            }
                            if (flag)
                            {
                                DetailAccesser.DeleteImportdetails(Old);
                            }
                        }
                    }
                    #endregion
                }
                #endregion

                #region Tao moi
                //chua ton tai => tao moi
                else
                {   
                    wsi.Import.ImportId = AutomaticValueAccesser.AutoGenerateId("Imports");
                    //wsi.Import.ImportDate = DateTime.Today;

                    wsi.Import.CreatedDate = DateTime.Now;
                    wsi.Import.CreatedBy = wsi.ActionBy;
                    wsi.Import.ModifiedBy = wsi.ActionBy;
                    wsi.Import.ModifiedDate = DateTime.Now;

                    Accesser.InsertImports(wsi.Import);
                    NMMessagesBL.SaveMessage(Session, wsi.Import.ImportId, "khởi tạo tài liệu", "", wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);

                    foreach (ImportDetails Item in wsi.Details)
                    {
                        Item.OrdinalNumber = 0;
                        Item.StockId = wsi.Import.StockId;
                        Item.ImportId = wsi.Import.ImportId;

                        Item.CreatedDate = DateTime.Now;
                        Item.CreatedBy = wsi.ActionBy;
                        Item.ModifiedDate = DateTime.Now;
                        Item.ModifiedBy = wsi.ActionBy;

                        DetailAccesser.InsertImportdetails(Item);
                    }
                    if (wsi.ProductSNs != null)
                    {
                        foreach (ProductSNs SN in wsi.ProductSNs)
                        {
                            SN.ImportId = wsi.Import.ImportId;
                            SN.StockId = wsi.Import.StockId;
                            SNAccesser.InsertProductSNs(SN);
                        }
                    }
                }
                #endregion

                #region Serial

                if (wsi.ProductSNs != null)
                {
                    if (wsi.ProductSNs.Count > 0)
                    {
                        ProductSNs SNTemp;
                        foreach (ProductSNs SN in wsi.ProductSNs)
                        {
                            SNTemp = SNAccesser.GetAllProductSNsByID(SN.ProductId, SN.SerialNumber, true);
                            if (SNTemp == null)
                            {
                                SN.StockId = wsi.Import.StockId;
                                SNAccesser.InsertProductSNs(SN);
                            }
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

                #region Nhap kho
                if (wsi.Import.ImportStatus == NMConstant.IMStatuses.Imported)
                {   
                    List<ImportDetails> TempDetails = new List<ImportDetails>();
                    foreach (ImportDetails Item in wsi.Details)
                    {
                        Item.ModifiedDate = DateTime.Now;
                        Item.ModifiedBy = wsi.ActionBy;

                        if (Item.GoodQuantity < Item.RequiredQuantity)
                        {
                            //tạo danh sách chi tiết cho phiếu mới
                            Detail = new ImportDetails();
                            Detail.ProductId = Item.ProductId;
                            Detail.RequiredQuantity = Item.RequiredQuantity - Item.GoodQuantity;
                            Detail.GoodQuantity = Detail.RequiredQuantity;
                            Detail.StockId = Item.StockId;

                            Detail.CreatedDate = DateTime.Now;
                            Detail.CreatedBy = wsi.ActionBy;
                            Detail.ModifiedDate = DateTime.Now;
                            Detail.ModifiedBy = wsi.ActionBy;

                            TempDetails.Add(Detail);
                        }
                        if (Item.GoodQuantity > 0)  //nhập số lượng này
                        {
                            // luu cac serial
                            if (NMCommon.GetSetting(NMConstant.Settings.SERIALNUMBER) && (wsi.ProductSNs != null))
                            {
                                for (int i = 0; i < Item.GoodQuantity; i++)
                                {
                                    wsi.ProductSNs.Remove(wsi.ProductSNs.Select(x => x).Where(x => x.ProductId == Item.ProductId).FirstOrDefault());
                                }
                            }
                        }
                        else
                        {
                            wsi.Import.ImportDetailsList.Remove(Item);
                            DetailAccesser.DeleteImportdetails(Item);
                        }
                    }

                    //  ghi sổ NKC -> giá vốn hàng bán                            
                    this.WriteGeneralJournals(wsi);
                    //  log
                    NMMessagesBL.SaveMessage(Session, wsi.Import.ImportId, "thay đổi trạng thái", "Đã nhập", wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
                    //cập nhật trạng thái PO liên quan
                    if (!string.IsNullOrEmpty(wsi.Import.Reference))
                    {
                        NMCommon.UpdateObjectStatus(Session, "PurchaseOrders", wsi.Import.Reference, NMConstant.POStatuses.Inventory, null, wsi.ActionBy, "Trạng thái: => Đã nhập kho. Số phiếu:" + wsi.Import.ImportId + " \n");
                    }

                    // nếu số lượng thực nhập kho nhỏ hơn số lượng y.c => tạo thêm phiếu cho lần sau
                    if (TempDetails.Count > 0)
                    {
                        //Lưu phiếu mới
                        Imports NewIm = new Imports();
                        NewIm.ImportId = AutomaticValueAccesser.AutoGenerateId("Imports");
                        NewIm.BackOrderOf = wsi.Import.ImportId;
                        NewIm.CarrierId = wsi.Import.CarrierId;

                        NewIm.CreatedBy = wsi.ActionBy;
                        NewIm.CreatedDate = DateTime.Now;
                        NewIm.ModifiedBy = wsi.ActionBy;
                        NewIm.ModifiedDate = DateTime.Now;

                        NewIm.SupplierId = wsi.Import.SupplierId;
                        NewIm.DeliveryMethod = wsi.Import.DeliveryMethod;
                        NewIm.DescriptionInVietnamese = wsi.Import.DescriptionInVietnamese;
                        NewIm.ImportBatchId = wsi.Import.ImportBatchId;
                        NewIm.ImportDate = DateTime.Today;
                        NewIm.ImportStatus = obj.ImportStatus;
                        NewIm.ImportTypeId = wsi.Import.ImportTypeId;
                        NewIm.InvoiceTypeId = wsi.Import.InvoiceTypeId;
                        NewIm.ExportStockId = wsi.Import.ExportStockId;
                        NewIm.InvoiceId = wsi.Import.InvoiceId;
                        NewIm.Reference = wsi.Import.Reference;
                        NewIm.StockId = wsi.Import.StockId;
                        NewIm.Transport = wsi.Import.Transport;
                        Accesser.InsertImports(NewIm);
                        foreach (ImportDetails Item in TempDetails)
                        {
                            Item.ImportId = NewIm.ImportId;
                            DetailAccesser.InsertImportdetails(Item);
                        }
                        if (wsi.ProductSNs != null)
                        {
                            foreach (ProductSNs SN in wsi.ProductSNs)
                            {
                                SN.ImportId = NewIm.ImportId;
                                SN.StockId = NewIm.StockId;
                                SNAccesser.UpdateProductSNs(SN);
                            }
                        }
                        NMMessagesBL.SaveMessage(Session, NewIm.ImportId, "khởi tạo tài liệu", "", wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
                        wsi.Import = NewIm;
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

        public NMImportsWSI DeleteObject(NMImportsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                ImportsAccesser Accesser = new ImportsAccesser(Session);
                Imports obj = Accesser.GetAllImportsByID(wsi.Import.ImportId, false);
                if (obj != null)
                {
                    if (obj.ImportStatus == NMConstant.IMStatuses.Imported)
                    {
                        List<ImportDetails> ImportDetails = obj.ImportDetailsList.Cast<ImportDetails>().ToList();
                        foreach (ImportDetails Item in ImportDetails)
                        {
                            NMCommon.UpdateStock(Session, Item, null);
                        }
                    }
                    Accesser.DeleteImports(obj);
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

        public void CheckProductInInventoryControl(ImportDetails newObj)
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

        public List<NMImportsWSI> SearchObject(NMImportsWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (wsi.Import != null)
            {
                if (!string.IsNullOrEmpty(wsi.Import.InvoiceId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.InvoiceId = :InvoiceId";
                    ListCriteria.Add("InvoiceId", wsi.Import.InvoiceId);
                }
                if (!string.IsNullOrEmpty(wsi.Import.SupplierId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.SupplierId = :SupplierId";
                    ListCriteria.Add("SupplierId", wsi.Import.SupplierId);
                }
                if (!string.IsNullOrEmpty(wsi.Import.ImportTypeId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.ImportTypeId = :ImportTypeId";
                    ListCriteria.Add("ImportTypeId", wsi.Import.ImportTypeId);
                }
                if (!string.IsNullOrEmpty(wsi.Import.Reference))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.Reference = :Reference";
                    ListCriteria.Add("Reference", wsi.Import.Reference);
                }
                if (!string.IsNullOrEmpty(wsi.Import.StockId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.StockId = :StockId";
                    ListCriteria.Add("StockId", wsi.Import.StockId);
                }
                if (!string.IsNullOrEmpty(wsi.Import.ImportStatus))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " CHARINDEX(O.ImportStatus, :ImportStatus) > 0";
                    ListCriteria.Add("ImportStatus", wsi.Import.ImportStatus);
                }
            }
            if (!string.IsNullOrEmpty(wsi.FromDate))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.ImportDate >= :FromDate ";
                ListCriteria.Add("FromDate", DateTime.Parse(wsi.FromDate));
            }
            if (!string.IsNullOrEmpty(wsi.ToDate))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.ImportDate <= :ToDate";
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
                strCriteria += " (O.ImportId LIKE :Keyword OR O.SupplierId in (SELECT C.CustomerId FROM Customers AS C WHERE C.CustomerId LIKE :Keyword OR C.CompanyNameInVietnamese LIKE :Keyword OR C.TaxCode LIKE :Keyword OR C.Cellphone LIKE :Keyword) OR O.ImportId in (select SN.ImportId from ProductSNs SN where SN.SerialNumber like :Keyword))";
                ListCriteria.Add("Keyword", "%" + wsi.Keyword + "%");
            }
            string strCmdCounter = "SELECT COUNT(O.ImportId) FROM Imports AS O" + strCriteria;
            IQuery queryCounter = Session.CreateSQLQuery(strCmdCounter);
            if (wsi.SortField != null && wsi.SortOrder != "")
            {
                strCriteria += " ORDER BY O." + wsi.SortField + " " + wsi.SortOrder;
            }
            else
            {
                strCriteria += " ORDER BY O.ImportDate DESC";
            }
            string strCmd = "SELECT O FROM Imports AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                queryCounter.SetParameter(Item.Key, Item.Value);
                query.SetParameter(Item.Key, Item.Value);
            }
            List<NMImportsWSI> ListWSI = new List<NMImportsWSI>();
            ImportsAccesser Accesser = new ImportsAccesser(Session);
            CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
            StocksAccesser StockAccesser = new StocksAccesser(Session);
            StatusesAccesser StatusAccesser = new StatusesAccesser(Session);
            TypesAccesser TypeAccesser = new TypesAccesser(Session);
            ProductSNsAccesser SNAccesser = new ProductSNsAccesser(Session);
            IList<Imports> objs;
            int totalRows = NMCommon.Counter(queryCounter);
            if (wsi.PageSize != 0)
            {
                objs = Accesser.GetImportsByQuery(query, wsi.PageSize, wsi.PageNum, false);
            }
            else
            {
                objs = Accesser.GetImportsByQuery(query, false);
            }

            bool IsManageSerial = NMCommon.GetSetting("SERIALNUMBER");

            foreach (Imports obj in objs)
            {
                wsi = new NMImportsWSI();
                wsi.Import = obj;
                wsi.Details = obj.ImportDetailsList.Cast<ImportDetails>().ToList();
                wsi.Supplier = CustomerAccesser.GetAllCustomersByID(obj.SupplierId, true);
                wsi.CreatedUser = CustomerAccesser.GetAllCustomersByID(obj.CreatedBy, true);
                //wsi.Status = StatusAccesser.GetAllStatusesByID(obj.ImportStatus, true);
                //wsi.ImportType = TypeAccesser.GetAllTypesByID(obj.ImportTypeId, true);
                wsi.Carrier = CustomerAccesser.GetAllCustomersByID(obj.CarrierId, true);
                if (IsManageSerial)
                    wsi.ProductSNs = SNAccesser.GetAllProductSNsByReferenceId(obj.ImportId, true).ToList();
                //NMStocksBL StockBL = new NMStocksBL();
                //NMStocksWSI StockWSI = new NMStocksWSI();
                //StockWSI.Mode = "SEL_OBJ";
                //StockWSI.Id = obj.StockId;
                //wsi.Stock = StockBL.callSingleBL(StockWSI);
                //if (!String.IsNullOrEmpty(obj.ExportStockId))
                //{
                //    StockWSI = new NMStocksWSI();
                //    StockWSI.Mode = "SEL_OBJ";
                //    StockWSI.Id = obj.ExportStockId;
                //    wsi.ExportStock = StockBL.callSingleBL(StockWSI);
                //}
                ListWSI.Add(wsi);
            }
            if (ListWSI.Count > 0)
                ListWSI[0].TotalRows = totalRows;
            return ListWSI;
        }

        private void WriteGeneralJournals(NMImportsWSI wsi)
        {
            MonthlyGeneralJournals MGJ = new MonthlyGeneralJournals();
            MonthlyGeneralJournalsAccesser mgjAccesser = new MonthlyGeneralJournalsAccesser(this.Session);

            mgjAccesser.DeletemonthlygeneraljournalsByImport(wsi.Import.ImportId);
            
            //ghi nợ
            Products pro = new Products();
            ProductsAccesser proAcc = new ProductsAccesser(this.Session);
            
            foreach (ImportDetails itm in wsi.Details)
            {
                MGJ = new MonthlyGeneralJournals();
                MGJ.IMID = wsi.Import.ImportId;
                MGJ.IssueDate = wsi.Import.ImportDate;
                MGJ.PartnerId = wsi.Import.SupplierId;
                if (wsi.Import.ImportTypeId == NMConstant.ImportType.Transfer)
                    if (string.IsNullOrEmpty(wsi.Import.SupplierId))
                        MGJ.PartnerId = "COMPANY";
                //MGJ.PIID = wsi.Import.Reference;
                MGJ.StockId = wsi.Import.StockId;
                MGJ.ActionBy = wsi.ActionBy;

                MGJ.CreatedBy = wsi.ActionBy;
                MGJ.ModifiedBy = wsi.ActionBy;

                MGJ.CurrencyId = "VND";
                MGJ.ExchangeRate = 1;
                MGJ.IsBegin = false;

                pro = proAcc.GetAllProductsByID(itm.ProductId, true);

                MGJ.ProductId = itm.ProductId;
                MGJ.UnitId = pro.ProductUnit;
                MGJ.AccountId = pro.TypeId;
                MGJ.ImportQuantity = itm.GoodQuantity;
                MGJ.ExportQuantity = 0;
                if (wsi.Import.ImportTypeId == NMConstant.ImportType.Transfer)
                    MGJ.DebitAmount = itm.GoodQuantity * mgjAccesser.ICGetTransferPrice(wsi.Import.Reference, itm.ProductId);
                else
                    MGJ.DebitAmount = itm.GoodQuantity * itm.Price;
                MGJ.CreditAmount = 0;
                MGJ.Descriptions = "Ghi nhận tăng Tồn kho hàng hóa";
                mgjAccesser.InsertMonthlygeneraljournals(MGJ);
            }

            return;
        }

        //
        // dùng trong trường hợp load sẳn PO để có thông tin về khách hàng...
        //
        //public void FromPurchaseOrder(NMPurchaseOrdersWSI POWSI)
        //{
        //    WSI.Import.SupplierId = POWSI.Order.SupplierId;
        //    WSI.Import.Reference = POWSI.Order.Id;
        //    WSI.Import.SupplierReference = POWSI.Order.Reference;

        //    WSI.Import.StockId = POWSI.Order.ImportStockId;
        //    //NewIm.ExportStockId = wsi.Order.ExportStockId;

        //    //NewIm.CarrierId = wsi.Order.CarrierId;
        //    WSI.Import.Transport = POWSI.Order.Transportation;

        //    if (NMCommon.GetSetting("USE_PODATE_FOR_IMPORT_DATE"))
        //        WSI.Import.ImportDate = POWSI.Order.OrderDate;
        //    else
        //        WSI.Import.ImportDate = DateTime.Today;

        //    WSI.Import.ImportStatus = NMConstant.IMStatuses.Draft;
        //    WSI.Import.ImportTypeId = NMConstant.ImportType.Direct;
        //    WSI.Import.InvoiceTypeId = POWSI.Order.InvoiceTypeId;

        //    WSI.Import.DeliveryMethod = NMConstant.DeliveryMethod.Once; //wsi.Order.DeliveryMethod

        //    WSI.Import.DescriptionInVietnamese = POWSI.Order.Description;

        //    WSI.Details = new List<ImportDetails>();
        //    ImportDetails Detail;
        //    foreach (PurchaseOrderDetails Item in POWSI.Details)
        //    {
        //        Detail = new ImportDetails();
        //        Detail.CopyFromPODetail(Item);
        //        WSI.Details.Add(Detail);
        //    }
        //}

        //
        // dùng trong trường hợp không cần load PO lên view để có thông tin về khách hàng...
        //
        //public string FromPurchaseOrder(string poId)
        //{
        //    NMPurchaseOrdersBL POBL = new NMPurchaseOrdersBL();
        //    NMPurchaseOrdersWSI POWSI = new NMPurchaseOrdersWSI();
        //    POWSI.Mode = "SEL_OBJ";
        //    POWSI.Order.Id = poId;
        //    POWSI = POBL.callSingleBL(POWSI);

        //    if (POWSI.WsiError == string.Empty)
        //    {
        //        this.FromPurchaseOrder(POWSI);
        //    }

        //    return POWSI.WsiError;
        //}

        #region Invoice

        public NMPurchaseInvoicesWSI CreateInvoice()
        {
            NMPurchaseInvoicesWSI piWsi = new NMPurchaseInvoicesWSI();

            return piWsi;
        }

        public NMPurchaseInvoicesWSI CreateInvoice(string exId)
        {
            NMPurchaseInvoicesWSI piWsi = new NMPurchaseInvoicesWSI();

            return piWsi;
        }


        #endregion
    }
}
