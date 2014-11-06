// StocksController.cs

// * http://nexmi.com
// * NoErp Project - Nexmi Open ERP
// * Copyright (C) 2012, Nguyễn Quang Tuyến (tuyen.nq@nexmi.com), AUTHOR.txt (http://nexmi.com/about)
// * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; 
//   either version 2 of the License, or (at your option) any later version. This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
//   without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details. 
//   You should have received a copy of the GNU General Public License along with this library; if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// *

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NEXMI;

namespace nexmiStore.Controllers
{
    public class StocksController : Controller
    {
        //
        // GET: /Stocks/

        #region Stocks
        public ActionResult Stocks()
        {
            return PartialView();
        }

        public ActionResult StockForm(String id, string windowId)
        {
            NEXMI.NMStocksBL BL = new NEXMI.NMStocksBL();
            NEXMI.NMStocksWSI WSI = new NEXMI.NMStocksWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Id = id;
            ViewData["WSI"] = BL.callSingleBL(WSI);
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public JsonResult SaveOrUpdateStock(String id, string telephone, string type, string avatar, string highlight, string insert)
        {
            string error="";
            NMStocksWSI WSI = new NMStocksWSI();
            NMStocksBL BL = new NMStocksBL();
            WSI.Id = id;

            if (!string.IsNullOrEmpty(id))
            {
                WSI.Mode = "SEL_OBJ";
                WSI = BL.callSingleBL(WSI);
            }

            WSI.Mode = "SAV_OBJ";
            WSI.Telephone = telephone;
            WSI.StockType = type;
            WSI.Logo = avatar;
            WSI.Highlight = highlight;
            WSI.CreatedBy = User.Identity.Name;
            WSI.ModifiedBy = User.Identity.Name;
            WSI.SortField = insert;
            WSI = BL.callSingleBL(WSI);
            error = WSI.WsiError;
            if (error == "")
            {
                string[] allLanguageId = NMCommon.GetAllLanguageId().Split(';');
                NMTranslationsBL TranslationBL = new NMTranslationsBL();
                NMTranslationsWSI TranslationWSI;
                string languageId = "";
                for (int i = 0; i < allLanguageId.Length - 1; i++)
                {
                    languageId = allLanguageId[i];
                    TranslationWSI = new NMTranslationsWSI();
                    TranslationWSI.Mode = "SAV_OBJ";
                    TranslationWSI.Translation = new Translations();
                    TranslationWSI.Translation.OwnerId = WSI.Id;
                    TranslationWSI.Translation.LanguageId = languageId;
                    TranslationWSI.Translation.Name = Request.Params["name" + languageId];
                    TranslationWSI.Translation.Address = Request.Params["address" + languageId];
                    TranslationWSI.Translation.ShortDescription = Request.Params["shortDescription" + languageId];
                    TranslationWSI.Translation.Description = NMCryptography.base64Decode(Request.Params["description" + languageId]);
                    TranslationWSI = TranslationBL.callSingleBL(TranslationWSI);
                }
            }
            return Json(WSI.WsiError);
        }

        public JsonResult DeleteStock(String id)
        {
            NMStocksWSI WSI = new NMStocksWSI();
            NMStocksBL BL = new NMStocksBL();
            WSI.Mode = "DEL_OBJ";
            WSI.Id = id;
            WSI = BL.callSingleBL(WSI);
            return Json(WSI.WsiError);
        }
        #endregion

        #region Import
        public ActionResult Imports(string status)
        {
            ViewData["ViewType"] = "list";
            ViewData["Status"] = status;
            return PartialView();
        }

        public ActionResult ImportList(string pageNum, string pageSize, string sortDataField, string sortOrder, string status, 
            string keyword, string typeId, string stockId, string from, string to, string partnerId)
        {
            NMImportsWSI WSI = new NMImportsWSI();
            NMImportsBL BL = new NMImportsBL();
            WSI.Mode = "SRC_OBJ";
            WSI.Import = new Imports();
            WSI.Import.ImportStatus = status;
            WSI.Import.ImportTypeId = typeId;
            if (!String.IsNullOrEmpty(partnerId))
                WSI.Import.SupplierId = partnerId;
            if (GetPermissions.GetViewAll((List<NMPermissionsWSI>)Session["Permissions"], NMConstant.Functions.TransferProduct) == false)
                WSI.Import.StockId = stockId;
            if (GetPermissions.GetViewAll((List<NMPermissionsWSI>)Session["Permissions"], NMConstant.Functions.Import) == false)
                WSI.ActionBy = User.Identity.Name;
            int page = 1;
            try
            {
                page = int.Parse(pageNum);
            }
            catch { }
            //if (!string.IsNullOrEmpty(pageNum))
            //    WSI.PageNum = int.Parse(pageNum);
            WSI.PageNum = page - 1;
            WSI.PageSize = NMCommon.PageSize();
            //WSI.SortField = sortDataField;
            //WSI.SortOrder = sortOrder;
            WSI.Keyword = keyword;
            if (!string.IsNullOrEmpty(from))
                WSI.FromDate = from;
            //else
            //{
            //    double days = NMCommon.GetSettingValue(NMConstant.Settings.DF_LOAD_LIST_DAYS);
            //    WSI.FromDate = DateTime.Today.AddDays(-days).ToString();
            //}
            if (!string.IsNullOrEmpty(to))
                WSI.ToDate = to;
            //else
            //    WSI.ToDate = DateTime.Today.ToString();

            List<NMImportsWSI> WSIs = BL.callListBL(WSI);
            
            ViewData["WSIs"] = WSIs;
            
            if (NMCommon.GetSetting(NMConstant.Settings.MULTI_LINE_DETAIL_ORDER) == false)
                return PartialView("ImportList4OneLineType");
            
            return PartialView();
        }

        public ActionResult ImportForm(String id, string orderId, string mode)
        {
            string customerId = "", customerName = "", invoiceId = "", importDate = DateTime.Now.ToString("yyyy-MM-dd"),
                reference = "", importType = "", stockId = ((NEXMI.NMCustomersWSI)Session["UserInfo"]).Customer.StockId, carrierId = "", carrierName = "", transport = "", exportStock = "", description = "",
                statusId = NMConstant.IMStatuses.Draft;
            NMImportsBL BL = new NMImportsBL();
            NMImportsWSI WSI;
            List<ImportDetails> Details = new List<ImportDetails>();
            ImportDetails Detail;
            if (!string.IsNullOrEmpty(id))
            {
                WSI = new NMImportsWSI();
                WSI.Mode = "SEL_OBJ";
                WSI.Import.ImportId = id;
                WSI = BL.callSingleBL(WSI);
                if (WSI.Supplier != null)
                {
                    customerId = WSI.Supplier.CustomerId;
                    customerName = WSI.Supplier.CompanyNameInVietnamese;
                }
                invoiceId = WSI.Import.InvoiceId;
                importDate = WSI.Import.ImportDate.ToString("yyyy-MM-dd");
                reference = WSI.Import.Reference;
                importType = WSI.Import.ImportTypeId;
                stockId = WSI.Import.StockId;
                if (WSI.Carrier != null)
                {
                    carrierId = WSI.Carrier.CustomerId; carrierName = WSI.Carrier.CompanyNameInVietnamese;
                }
                transport = WSI.Import.Transport;
                if (!string.IsNullOrEmpty(WSI.Import.ExportStockId))
                    exportStock = WSI.Import.ExportStockId;
                description = WSI.Import.DescriptionInVietnamese;
                statusId = WSI.Import.ImportStatus;
                ViewData["DeliveryMethod"] = WSI.Import.DeliveryMethod;
                bool IsManageSerial = NMCommon.GetSetting("SERIALNUMBER");

                Session["ProductSNs"] = WSI.ProductSNs;
                foreach (ImportDetails Item in WSI.Details.OrderBy(d => d.OrdinalNumber))
                {
                    if (IsManageSerial)
                    {
                        Session["SNs" + Item.ProductId] = string.Join(",", (WSI.ProductSNs.Select(i => i).Where(i => i.ProductId == Item.ProductId)).Select(i => i.SerialNumber));
                    }
                    if (mode == "Copy") Item.OrdinalNumber = 0;
                    Details.Add(Item);
                }

            }
            else
            {
                id = "";
                if (!String.IsNullOrEmpty(orderId))
                {
                    NMPurchaseOrdersBL POBL = new NMPurchaseOrdersBL();
                    NMPurchaseOrdersWSI POWSI = new NMPurchaseOrdersWSI();
                    POWSI.Mode = "SEL_OBJ";
                    POWSI.Order.Id = orderId;
                    POWSI = POBL.callSingleBL(POWSI);

                    customerId = POWSI.Supplier.CustomerId;
                    customerName = POWSI.Supplier.CompanyNameInVietnamese;
                    reference = POWSI.Order.Id;
                    
                    POBL.WSI = POWSI;
                    WSI = POBL.CreateImport();
                }
            }
            if (mode == "Copy")
            {
                id = "";
                statusId = NMConstant.IMStatuses.Draft;
                importDate = DateTime.Now.ToString("yyyy-MM-dd");
                stockId = ((NEXMI.NMCustomersWSI)Session["UserInfo"]).Customer.StockId;
                reference = "";
            }
            ViewData["Id"] = id;
            ViewData["CustomerId"] = customerId;
            ViewData["CustomerName"] = customerName;
            ViewData["InvoiceId"] = invoiceId;
            ViewData["ImportDate"] = importDate;
            ViewData["Reference"] = reference;
            ViewData["ImportType"] = importType;
            ViewData["StockId"] = stockId;
            ViewData["CarrierId"] = carrierId;
            ViewData["CarrierName"] = carrierName;
            ViewData["Transport"] = transport;
            ViewData["ExportStock"] = exportStock;
            ViewData["Description"] = description;
            ViewData["StatusId"] = statusId;
            Session["Details"] = Details;
            
            ViewData["StockInput"] = NMCommon.GetSetting(NMConstant.Settings.SELECT_STORE_BY_USER_4_IMPORT);

            return PartialView();
        }

        public ActionResult ImportDetailForm(string productId)
        {
            List<ImportDetails> Details = new List<ImportDetails>();
            if (Session["Details"] != null)
            {
                Details = (List<ImportDetails>)Session["Details"];
            }
            foreach (ImportDetails Item in Details)
            {
                if (Item.ProductId == productId)
                {
                    ViewData["Detail"] = Item;
                    break;
                }
            }
            return PartialView();
        }

        public ActionResult AddImportDetail(int ordinalNumber, string productId, string unitId, double goodQuantity, double badQuantity,
            string description, string strSNs)
        {
            String strError = "";
            try
            {
                List<ImportDetails> Details = new List<ImportDetails>();
                if (Session["Details"] != null)
                {
                    Details = (List<ImportDetails>)Session["Details"];
                }
                
                ImportDetails Detail;
                if (ordinalNumber > 0)
                    Detail = Details.Find(d => d.OrdinalNumber == ordinalNumber);
                else
                    Detail = Details.Find(d => d.ProductId == productId);
                
                if (Detail == null)
                {
                    Detail = new ImportDetails();
                    Detail.ProductId = productId;
                    Detail.UnitId = unitId;
                    Detail.GoodQuantity = goodQuantity;
                    Detail.BadQuantity = badQuantity;
                    Detail.RequiredQuantity = Detail.GoodQuantity;
                    Detail.Description = description;
                    Details.Add(Detail);
                }
                else
                {
                    Detail.ProductId = productId;
                    Detail.UnitId = unitId;
                    Detail.GoodQuantity = goodQuantity;
                    Detail.BadQuantity = badQuantity;
                    Detail.RequiredQuantity = Detail.GoodQuantity;
                    Detail.Description = description;
                }
                Session["SNs" + productId] = strSNs;
                //List<ProductSNs> SNs = new List<ProductSNs>();
                //if (Session["SNs"] != null)
                //{
                //    SNs = (List<ProductSNs>)Session["SNs"];
                //}
                //ProductSNs SN;
                //foreach (string Item in strSNs.Split(','))
                //{
                //    SN = new ProductSNs();
                //    SN.ProductId = productId;
                //    SN.SerialNumber = Item;
                //    SNs.Add(SN);
                //}
                Session["Details"] = Details;
            }
            catch
            {
                strError = "Không thực hiện được.\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.";
            }
            //return Json(strError);

            return PartialView("ImportDetails");
        }

        public JsonResult RemoveImportDetail(string productId)
        {
            List<ImportDetails> Details = new List<ImportDetails>();
            if (Session["Details"] != null)
            {
                Details = (List<ImportDetails>)Session["Details"];
            }
            foreach (ImportDetails Item in Details)
            {
                if (Item.ProductId == productId)
                {
                    Details.Remove(Item);
                    break;
                }
            }
            Session["Details"] = Details;
            return Json("");
        }

        public ActionResult Import(string id)
        {
            NEXMI.NMImportsWSI WSI = new NEXMI.NMImportsWSI();
            NEXMI.NMImportsBL BL = new NEXMI.NMImportsBL();
            WSI.Mode = "SEL_OBJ";
            WSI.Import.ImportId = id;
            WSI = BL.callSingleBL(WSI);
            Session["CurrentObj"] = WSI;

            ViewData["importId"] = WSI.Import.ImportId;
            ViewData["importDate"] = WSI.Import.ImportDate.ToString("dd/MM/yyyy");
            ViewData["status"] = WSI.Import.ImportStatus;
            ViewData["description"] = WSI.Import.DescriptionInVietnamese;
            ViewData["ImportTypeId"] = GlobalValues.GetType( WSI.Import.ImportTypeId);
            ViewData["slImportTypes"] = GlobalValues.GetACCType(WSI.Import.InvoiceTypeId).AccountNumber.NameInVietnamese;    // (WSI.ImportType == null) ? "" : WSI.ImportType.Name;
            ViewData["CarrierName"] = (WSI.Carrier == null) ? "" : WSI.Carrier.CompanyNameInVietnamese;
            ViewData["Transport"] = WSI.Import.Transport;
            ViewData["slStocks"] = (WSI.Stock != null && WSI.Stock.Translation != null) ? WSI.Stock.Translation.Name : "";
            ViewData["Reference"] = WSI.Import.Reference;
            if (WSI.Import.DeliveryMethod == null) WSI.Import.DeliveryMethod = "";
            ViewData["DeliveryMethod"] = WSI.Import.DeliveryMethod;
            // for log and messages
            ViewData["SendTo"] = WSI.Import.CreatedBy;    // dùng để gửi tin nhắn đến người quản lý
            //if (WSI.Order.SalesPersonId != WSI.Order.CreatedBy)  //người tạo khác người quản lý
            //    ViewData["SendTo"] += ";" + WSI.Order.CreatedBy;   //gửi tin đến người tạo

            ViewData["SupplierName"] = (WSI.Supplier == null) ? "" : WSI.Supplier.CompanyNameInVietnamese;
            ViewData["ExportStock"] = (WSI.ExportStock != null && WSI.ExportStock.Translation != null) ? WSI.ExportStock.Translation.Name : "";

            //ViewData["Details"] = WSI.Details.OrderBy(d => d.OrdinalNumber);
            if (NMCommon.GetSetting("SERIALNUMBER"))
            {
                foreach (ImportDetails Item in WSI.Details)
                {
                    Session["SNs" + Item.ProductId] = string.Join(",", (WSI.ProductSNs.Select(i => i).Where(i => i.ProductId == Item.ProductId)).Select(i => i.SerialNumber));
                }
            }
            ViewData["ViewType"] = "detail";
            return PartialView();
        }

        public JsonResult SaveOrUpdateImport(String id, String importDate, string statusId, String supplierId, string importType, string description, string stockId, string invoiceId, string carrierId, string transport,
            string reference, string deliveryMethod, string exportStockId)
        {
            string result = "", error = "";
            NMImportsWSI WSI = new NMImportsWSI();
            NMImportsBL BL = new NMImportsBL();
            WSI.Import = new Imports();
            WSI.Import.ImportId = id;

            if (!string.IsNullOrEmpty(id))
            {
                WSI.Mode = "SEL_OBJ";
                WSI = BL.callSingleBL(WSI);
            }

            WSI.Mode = "SAV_OBJ";
            WSI.Import.ImportDate = DateTime.Parse(importDate);
            WSI.Import.ImportStatus = statusId;
            WSI.Import.ImportTypeId = importType;
            WSI.Import.Reference = reference;
            
            if (!string.IsNullOrEmpty(invoiceId)) WSI.Import.InvoiceId = invoiceId;
            if (!string.IsNullOrEmpty(supplierId)) WSI.Import.SupplierId = supplierId;
            WSI.Import.StockId = stockId;
            WSI.Import.DescriptionInVietnamese = description;
            if (!String.IsNullOrEmpty(carrierId)) WSI.Import.CarrierId = carrierId;
            WSI.Import.Transport = transport;
            WSI.Import.DeliveryMethod = deliveryMethod;
            WSI.ActionBy = User.Identity.Name;

            if (WSI.Import.ImportTypeId == NMConstant.ImportType.Transfer)
            {
                WSI.Import.SupplierId = NMCommon.GetCompany().Customer.CustomerId;
                if (!string.IsNullOrEmpty(exportStockId)) WSI.Import.ExportStockId = exportStockId;
            }

            if (Session["Details"] != null)
            {
                WSI.Details = (List<ImportDetails>)Session["Details"];
                if (NMCommon.GetSetting("SERIALNUMBER"))
                {
                    WSI.ProductSNs = new List<ProductSNs>();
                    ProductSNs SN;
                    foreach (ImportDetails Item in WSI.Details)
                    {
                        string[] arrSNs = Session["SNs" + Item.ProductId].ToString().Split(',');
                        foreach (string x in arrSNs)
                        {
                            SN = new ProductSNs();
                            SN.ProductId = Item.ProductId;
                            SN.SerialNumber = x;
                            WSI.ProductSNs.Add(SN);
                        }
                    }
                }
            }
            WSI = BL.callSingleBL(WSI);
            result = WSI.Import.ImportId;
            error = WSI.WsiError;
            return Json(new { result = result, error = error });
        }

        public JsonResult ConfirmImport(string id)
        {
            string error = "";
            NMImportsWSI WSI = new NMImportsWSI();
            NMImportsBL BL = new NMImportsBL();
            WSI.Mode = "SEL_OBJ";
            WSI.Import = new NEXMI.Imports();
            WSI.Import.ImportId = id;
            WSI = BL.callSingleBL(WSI);
            error = WSI.WsiError;
            if (error == "")
            {
                WSI.Mode = "SAV_OBJ";
                WSI.Import.ImportStatus = NMConstant.IMStatuses.Imported;
                WSI.ActionBy = User.Identity.Name;
                error = BL.callSingleBL(WSI).WsiError;
                //if (error == "" && !string.IsNullOrEmpty(WSI.Import.Reference))
                //{
                //    NMPurchaseOrdersBL POBL = new NMPurchaseOrdersBL();
                //    NMPurchaseOrdersWSI POWSI = new NMPurchaseOrdersWSI();
                //    POWSI.Mode = "SEL_OBJ";
                //    POWSI.Order = new PurchaseOrders();
                //    POWSI.Order.Id = WSI.Import.Reference;
                //    POWSI = POBL.callSingleBL(POWSI);
                //    if (POWSI.WsiError == "")
                //    {
                //        POWSI.Mode = "SAV_OBJ";
                //        POWSI.Order.pr
                //    }
                //}
            }
            return Json(new { error = error });
        }

        public JsonResult DeleteImport(String id)
        {
            NMImportsWSI WSI = new NMImportsWSI();
            NMImportsBL BL = new NMImportsBL();
            WSI.Mode = "DEL_OBJ";
            WSI.Import = new Imports();
            WSI.Import.ImportId = id;
            WSI = BL.callSingleBL(WSI);
            return Json(WSI.WsiError);
        }

        public ActionResult PrintImport(string id)
        {
            NEXMI.NMImportsWSI WSI = new NEXMI.NMImportsWSI();
            NEXMI.NMImportsBL BL = new NEXMI.NMImportsBL();
            WSI.Mode = "SEL_OBJ";
            WSI.Import = new Imports();
            WSI.Import.ImportId = id;
            ViewData["WSI"] = BL.callSingleBL(WSI);
            return PartialView();
        }

        public ActionResult ReceiveForm(string id, string windowId)
        {
            NMImportsBL BL = new NMImportsBL();
            NMImportsWSI WSI = new NMImportsWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Import = new Imports();
            WSI.Import.ImportId = id;
            Session["ImportWSI"] = BL.callSingleBL(WSI);
            //if (windowId == null) windowId = NMCommon.RandomString(8, true);
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public JsonResult ReceiveProducts(string id, string quantities)
        {
            NMImportsBL BL = new NMImportsBL();
            NMImportsWSI WSI = (NMImportsWSI)Session["ImportWSI"];
            WSI.Mode = "SAV_OBJ";
            WSI.Import.ImportStatus = NMConstant.IMStatuses.Imported;
            WSI.ActionBy = User.Identity.Name;
            string[] arr = quantities.Split(',');
            int i = 0;
            foreach (ImportDetails Item in WSI.Details)
            {
                try
                {
                    Item.GoodQuantity = double.Parse(arr[i++]);
                }
                catch
                {
                    Item.GoodQuantity = 0;
                }
            }
            WSI = BL.callSingleBL(WSI);
            if (WSI.WsiError == "") Session["ImportWSI"] = null;
            return Json(new { id = WSI.Import.ImportId, error = WSI.WsiError });
        }

        public ActionResult ImportReturnForm(string id, string windowId)
        {
            NMImportsBL BL = new NMImportsBL();
            NMImportsWSI WSI = new NMImportsWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Import = new Imports();
            WSI.Import.ImportId = id;
            WSI = BL.callSingleBL(WSI);
            Session["ImportWSI"] = WSI;
            ViewData["WindowId"] = windowId;

            return PartialView();
        }

        public JsonResult ImportReturnProducts(string id, string quantities, string description)
        {
            NMImportsWSI ImportWSI = (NMImportsWSI)Session["ImportWSI"];

            NMExportsBL BL = new NMExportsBL();
            NMExportsWSI WSI = new NMExportsWSI();
            WSI.Mode = "SAV_OBJ";
            WSI.Export = new NEXMI.Exports();
            WSI.Export.CustomerId = ImportWSI.Import.SupplierId;
            WSI.Export.ExportDate = DateTime.Today;
            WSI.Export.ExportTypeId = NMConstant.ExportType.Return;
            WSI.Export.StockId = ImportWSI.Import.StockId;
            WSI.Export.Reference = ImportWSI.Import.ImportId;
            WSI.Export.ExportStatus = NMConstant.EXStatuses.Draft;
            WSI.Export.DescriptionInVietnamese = description;
            WSI.Export.DeliveryMethod = NMConstant.ShippingPolicy.Once;
            WSI.ActionBy = User.Identity.Name;
            WSI.Details = new List<ExportDetails>();
            ExportDetails Detail;
            string[] arr = quantities.Split(',');
            int i = 0; double qty = 0;
            foreach (ImportDetails Item in ImportWSI.Details)
            {
                try
                {
                    qty = 0;
                    qty = double.Parse(arr[i++]);
                }
                catch { }
                if (qty > 0)
                {
                    Detail = new ExportDetails();
                    Detail.CopyFromImportDetail(Item);
                    //Detail.ProductId = Item.ProductId;
                    //Detail.StockId = Item.StockId;
                    Detail.RequiredQuantity = qty;
                    WSI.Details.Add(Detail);
                }
            }
            if (WSI.Details.Count > 0)
                WSI = BL.callSingleBL(WSI);
            else
                WSI.WsiError = "Bạn chưa nhập số lượng hàng trả!";

            return Json(new { id = WSI.Export.ExportId, error = WSI.WsiError });
        }
        #endregion

        #region Export
        public ActionResult Exports(string status)
        {
            ViewData["ViewType"] = "list";
            ViewData["Status"] = status;
            return PartialView();
        }

        public ActionResult ExportList(string pageNum, string pageSize, string sortDataField, string sortOrder, string status,
                string keyword, string referenceId, string typeId, string stockId, string from, string to, string partnerId)
        {
            NMExportsWSI WSI = new NMExportsWSI();
            NMExportsBL BL = new NMExportsBL();
            WSI.Mode = "SRC_OBJ";
            if (GetPermissions.Get((List<NMPermissionsWSI>)Session["Permissions"], NMConstant.Functions.Export, "ViewAll") == false)
            {
                WSI.ActionBy = User.Identity.Name;
            }
            //if (!string.IsNullOrEmpty(pageNum))
            //    WSI.PageNum = int.Parse(pageNum);
            int page = 1;
            try
            {
                page = int.Parse(pageNum);
            }
            catch { }            
            WSI.PageNum = page - 1;
            WSI.PageSize = NMCommon.PageSize();
            //WSI.SortField = sortDataField;
            //WSI.SortOrder = sortOrder;
            WSI.Keyword = keyword;
            if (!string.IsNullOrEmpty(from))
                WSI.FromDate = from;
            //else
            //{
            //    double days = NMCommon.GetSettingValue(NMConstant.Settings.DF_LOAD_LIST_DAYS);
            //    WSI.FromDate = DateTime.Today.AddDays(-days).ToString();
            //}
            if (!string.IsNullOrEmpty(to))
                WSI.ToDate = to;
            //else
            //    WSI.ToDate = DateTime.Today.ToString();

            WSI.Export = new Exports();
            WSI.Export.Reference = referenceId;
            WSI.Export.ExportTypeId = typeId;
            WSI.Export.StockId = stockId;
            WSI.Export.ExportStatus = status;
            if (!String.IsNullOrEmpty(partnerId))
                WSI.Export.CustomerId = partnerId;

            List<NMExportsWSI> WSIs = BL.callListBL(WSI);
            ViewData["WSIs"] = WSIs;

            //if (string.IsNullOrEmpty(mode))
            //{
            //    mode = "";
            //}
            //ViewData["Mode"] = mode;

            if (NMCommon.GetSetting(NMConstant.Settings.MULTI_LINE_DETAIL_ORDER) == false)
                return PartialView("ExportList4OneLineType");
            
            return PartialView();
        }


        public ActionResult Export(string id)
        {
            NMExportsBL BL = new NMExportsBL();
            NMExportsWSI WSI = new NMExportsWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Export = new Exports();
            WSI.Export.ExportId = id;
            WSI = BL.callSingleBL(WSI);
            ViewData["WSI"] = WSI;
            ViewData["ViewType"] = "detail";
            if (NMCommon.GetSetting("SERIALNUMBER"))
            {
                foreach (ExportDetails Item in WSI.Details)
                {
                    Session["SNs" + Item.ProductId] = string.Join(",", (WSI.ProductSNs.Select(i => i).Where(i => i.ProductId == Item.ProductId)).Select(i => i.SerialNumber));
                }
            }
            return PartialView();
        }

        public ActionResult ExportForm(string exportId, string orderId, string mode, string typeId)
        {
            string customerId = "", customerName = "", customerAddress = "", reference = "", stockId = ((NEXMI.NMCustomersWSI)Session["UserInfo"]).Customer.StockId,   //UserInfoCache.User.Customer.StockId,
                description = "", status = NEXMI.NMConstant.EXStatuses.Draft, carrierId = "", carrierName = "", transport = "",
                importStockId = "";
            if (typeId == null) typeId = "";
            DateTime exportDate = DateTime.Now;
            List<ExportDetails> Details = new List<ExportDetails>();
            ExportDetails Detail;
            if (!string.IsNullOrEmpty(exportId))
            {
                NMExportsBL BL = new NMExportsBL();
                NMExportsWSI WSI = new NMExportsWSI();
                WSI.Mode = "SEL_OBJ";
                WSI.Export = new Exports();
                WSI.Export.ExportId = exportId;
                WSI = BL.callSingleBL(WSI);
                if (WSI.Customer != null)
                {
                    customerId = WSI.Customer.CustomerId; customerName = WSI.Customer.CompanyNameInVietnamese;
                    customerAddress = WSI.Customer.Address; reference = WSI.Export.Reference;
                }
                if (WSI.Carrier != null)
                {
                    carrierId = WSI.Carrier.CustomerId;
                    carrierName = WSI.Carrier.CompanyNameInVietnamese;
                }
                transport = WSI.Export.Transport;
                exportDate = WSI.Export.ExportDate; description = WSI.Export.DescriptionInVietnamese;
                stockId = WSI.Export.StockId;
                status = WSI.Export.ExportStatus;
                typeId = WSI.Export.ExportTypeId;
                if (!string.IsNullOrEmpty(WSI.Export.ImportStockId))
                    importStockId = WSI.Export.ImportStockId;
                bool IsManageSerial = NMCommon.GetSetting("SERIALNUMBER");
                foreach (ExportDetails Item in WSI.Details)
                {
                    if (IsManageSerial)
                    {
                        Session["SNs" + Item.ProductId] = string.Join(",", (WSI.ProductSNs.Select(i => i).Where(i => i.ProductId == Item.ProductId)).Select(i => i.SerialNumber));
                    }
                    if (mode == "Copy") Item.OrdinalNumber = 0;
                    Details.Add(Item);
                }
            }
            else
            {
                exportId = "";
                if (!string.IsNullOrEmpty(orderId))
                {
                    if (mode == "Other")
                    {
                        reference = orderId;
                        typeId = NMConstant.ExportType.Lend;
                    }
                    else
                    {
                        NMSalesOrdersBL SOBL = new NMSalesOrdersBL();
                        NMSalesOrdersWSI SOWSI = new NMSalesOrdersWSI();
                        SOWSI.Mode = "SEL_OBJ";
                        SOWSI.Order.OrderId = orderId;
                        SOWSI = SOBL.callSingleBL(SOWSI);

                        customerId = SOWSI.Customer.CustomerId; 
                        customerName = SOWSI.Customer.CompanyNameInVietnamese;
                        customerAddress = SOWSI.Customer.Address; 
                        reference = SOWSI.Order.OrderId;
                        typeId = NMConstant.ExportType.ForCustomers;
                        status = NMConstant.EXStatuses.Draft;
                        foreach (SalesOrderDetails Item in SOWSI.Details)
                        {
                            Detail = new ExportDetails();
                            Detail.CopyFromSODetail(Item);

                            Details.Add(Detail);
                        }
                    }
                }
            }
            if (mode == "Copy")
            {
                exportId = "";
                status = NMConstant.EXStatuses.Draft;
                exportDate = DateTime.Now;
                stockId = ((NEXMI.NMCustomersWSI)Session["UserInfo"]).Customer.StockId;
                reference = "";
            }

            if (typeId == NMConstant.ExportType.Transfer)
            {
                customerId = NMCommon.GetCompany().Customer.CustomerId;
                customerName = NMCommon.GetCompany().Customer.CompanyNameInVietnamese;
            }

            ViewData["Id"] = exportId;
            ViewData["CustomerId"] = customerId;
            ViewData["CustomerName"] = customerName;
            ViewData["CustomerAddress"] = customerAddress;
            ViewData["CarrierId"] = carrierId;
            ViewData["CarrierName"] = carrierName;
            ViewData["Transport"] = transport;
            ViewData["ExportDate"] = exportDate.ToString("yyyy-MM-dd");
            ViewData["ExportStatus"] = status;
            ViewData["ExportTypeId"] = typeId;
            ViewData["Reference"] = reference;
            ViewData["Description"] = description;
            ViewData["ImportStockId"] = importStockId;
            ViewData["slStocks"] = stockId;
            Session["Details"] = Details;

            ViewData["StockInput"] = NMCommon.GetSetting(NMConstant.Settings.SELECT_STORE_BY_USER_4_EXPORT);

            ViewData["ViewType"] = "detail";

            return PartialView();
        }

        public JsonResult SaveOrUpdateExport(string id, string exportDate, string customerId, string statusId, string reference, string description,
            string exportTypeId, string stockId, string carrierId, string transport, string deliveryMethod, string importStockId)
        {
            string result = "", error = "";
            NMExportsBL BL = new NMExportsBL();
            NMExportsWSI WSI = new NMExportsWSI();
            WSI.Export = new Exports();
            WSI.Export.ExportId = id;
            
            if (!string.IsNullOrEmpty(id))
            {
                WSI.Mode = "SEL_OBJ";
                WSI = BL.callSingleBL(WSI);
            }

            WSI.Mode = "SAV_OBJ";
            WSI.Export.ExportDate = DateTime.Parse(exportDate);
            if (!string.IsNullOrEmpty(customerId))
                WSI.Export.CustomerId = customerId;
            WSI.Export.Reference = reference;
            WSI.Export.DescriptionInVietnamese = description;
            WSI.Export.ExportTypeId = exportTypeId;
            WSI.Export.ExportStatus = statusId;
            WSI.Export.StockId = stockId;
                        
            if (WSI.Export.ExportTypeId == NMConstant.ExportType.Transfer)
            {
                WSI.Export.CustomerId = NMCommon.GetCompany().Customer.CustomerId;
                if (!string.IsNullOrEmpty(importStockId))
                {
                    WSI.Export.ImportStockId = importStockId;
                }
            }

            WSI.Export.DeliveryMethod = deliveryMethod;
            if (!string.IsNullOrEmpty(carrierId)) WSI.Export.CarrierId = carrierId;
            WSI.Export.Transport = transport;            
            WSI.ActionBy = User.Identity.Name;
            
            if (Session["Details"] != null)
            {   
                WSI.Details = (List<ExportDetails>)Session["Details"];
                if (NMCommon.GetSetting("SERIALNUMBER"))
                {
                    WSI.ProductSNs = new List<ProductSNs>();
                    ProductSNs SN;
                    foreach (ExportDetails Item in WSI.Details)
                    {
                        if (Session["SNs" + Item.ProductId] != null)
                        {
                            string[] arrSNs = Session["SNs" + Item.ProductId].ToString().Split(',');
                            foreach (string x in arrSNs)
                            {
                                SN = new ProductSNs();
                                SN.ProductId = Item.ProductId;
                                SN.StockId = WSI.Export.StockId;
                                SN.SerialNumber = x;
                                WSI.ProductSNs.Add(SN);
                            }
                        }
                    }
                }
            }
            WSI = BL.callSingleBL(WSI);
            result = WSI.Export.ExportId;
            error = WSI.WsiError;
            return Json(new { result = result, error = error });
        }

        public ActionResult ExportDetailForm(string productId)
        {
            List<ExportDetails> Details = new List<ExportDetails>();
            if (Session["Details"] != null)
            {
                Details = (List<ExportDetails>)Session["Details"];
            }
            foreach (ExportDetails Item in Details)
            {
                if (Item.ProductId == productId)
                {
                    ViewData["Detail"] = Item;
                    break;
                }
            }
            return PartialView();
        }

        public ActionResult AddExportDetail(string ordinalNumber, string productId, string unitId,string quantity, string description, string strSNs)
        {
            String strError = "";
            try
            {
                List<ExportDetails> Details = new List<ExportDetails>();
                if (Session["Details"] != null)
                {
                    Details = (List<ExportDetails>)Session["Details"];
                }
                bool flag = true;
                ExportDetails Detail;
                if (!string.IsNullOrEmpty(ordinalNumber))
                {
                    int ordNum = int.Parse(ordinalNumber);
                    if (ordNum > 0)
                        Detail = Details.Find(d => d.OrdinalNumber == ordNum);
                    else
                        Detail = Details.Find(d => d.ProductId == productId);
                }
                else
                    Detail = Details.Find(d => d.ProductId == productId);

                if (Detail == null)
                {
                    Detail = new ExportDetails();
                    Detail.ProductId = productId;
                    Detail.UnitId = unitId;
                    Detail.Quantity = double.Parse(quantity);
                    Detail.RequiredQuantity = Detail.Quantity;
                    Detail.Description = description;
                    Details.Add(Detail);
                }
                else
                {
                    Detail.ProductId = productId;
                    Detail.UnitId = unitId;
                    Detail.Quantity = double.Parse(quantity);
                    Detail.RequiredQuantity = Detail.Quantity;
                    Detail.Description = description;
                }
                
                Session["SNs" + productId] = strSNs;
                Session["Details"] = Details;
            }
            catch
            {
                strError = "Không thực hiện được.\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.";
            }
            //return Json(new { strError = strError });
            return PartialView("ExportDetails");
        }

        public JsonResult RemoveExportDetail(string productId)
        {
            List<ExportDetails> Details = new List<ExportDetails>();
            if (Session["Details"] != null)
            {
                Details = (List<ExportDetails>)Session["Details"];
            }
            foreach (ExportDetails Item in Details)
            {
                if (Item.ProductId == productId)
                {
                    Details.Remove(Item);
                    break;
                }
            }
            Session["Details"] = Details;
            return Json("");
        }

        public JsonResult DeleteExport(string id)
        {
            NMExportsBL BL = new NMExportsBL();
            NMExportsWSI WSI = new NMExportsWSI();
            WSI.Mode = "DEL_OBJ";
            WSI.Export = new Exports();
            WSI.Export.ExportId = id;
            WSI = BL.callSingleBL(WSI);
            return Json(WSI.WsiError);
        }

        public ActionResult DeliverForm(string id, string windowId)
        {
            NMExportsBL BL = new NMExportsBL();
            NMExportsWSI WSI = new NMExportsWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Export = new Exports();
            WSI.Export.ExportId = id;
            WSI = BL.callSingleBL(WSI);

            ViewData["Enable"] = "Yes";
            // lay so luong ton kho
            NMProductInStocksBL pisBL = new NMProductInStocksBL();
            NMProductInStocksWSI pisWSI;
            List<NMProductInStocksWSI> pisList = new List<NMProductInStocksWSI>();

            pisWSI = new NMProductInStocksWSI();
            pisWSI.Mode = "SRC_OBJ";

            foreach (ExportDetails itm in WSI.Details)
            {
                pisWSI.PIS = new ProductInStocks();
                pisWSI.PIS.StockId = WSI.Export.StockId;
                pisWSI.PIS.ProductId = itm.ProductId;
                pisWSI = pisBL.callSingleBL(pisWSI);

                //pisWSI = pisList.Find(i => i.PIS.ProductId == itm.ProductId);
                
                try
                {
                    if (Math.Round(pisWSI.PIS.BeginQuantity + pisWSI.PIS.ImportQuantity - pisWSI.PIS.ExportQuantity - pisWSI.PIS.BadProductInStock, 3) < itm.RequiredQuantity)
                    {
                        ViewData["Enable"] = "No";
                        //break;
                    }
                }
                catch
                {
                    ViewData["Enable"] = "No";
                    //break;
                }
                pisList.Add(pisWSI);
            }

            Session["ExportWSI"] = WSI;
            Session["PISList"] = pisList;
            ViewData["Id"] = id;
            ViewData["WindowId"] = windowId;
            
            return PartialView();
        }

        public JsonResult DeliveryProducts(string id, string quantities)
        {
            NMExportsBL BL = new NMExportsBL();
            NMExportsWSI WSI = (NMExportsWSI)Session["ExportWSI"];
            List<NMProductInStocksWSI> pisList = (List<NMProductInStocksWSI>)Session["PISList"];
            NMProductInStocksWSI pisWSI;
            WSI.Mode = "SAV_OBJ";
            WSI.Export.ExportStatus = NMConstant.EXStatuses.Delivered;
            WSI.ActionBy = User.Identity.Name;
            string[] arr = quantities.Split(',');
            bool checkResult = true;
            int i = 0;
            foreach (ExportDetails Item in WSI.Details)
            {
                try
                {
                    Item.Quantity = double.Parse(arr[i++]);
                }
                catch
                {
                    Item.Quantity = 0;
                }
                pisWSI = pisList.Find(p => p.PIS.ProductId == Item.ProductId);
                double inStock = pisWSI.PIS.BeginQuantity + pisWSI.PIS.ImportQuantity - pisWSI.PIS.ExportQuantity;
                if (Math.Round(inStock, 2) < Item.Quantity)
                {
                    checkResult = false;
                    break;
                }
            }
            if (checkResult)
            {
                WSI = BL.callSingleBL(WSI);
                if (WSI.WsiError == "")
                {
                    Session["ExportWSI"] = null;
                    Session["PISList"] = null;
                }
            }
            else
                WSI.WsiError = "Số lượng xuất phải nhỏ hơn số lượng tồn!";
            return Json(new { id = WSI.Export.ExportId, error = WSI.WsiError });
        }

        public ActionResult ReturnForm(string id, string windowId)
        {
            NMExportsBL BL = new NMExportsBL();
            NMExportsWSI WSI = new NMExportsWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Export = new Exports();
            WSI.Export.ExportId = id;
            WSI = BL.callSingleBL(WSI);
            Session["ExportWSI"] = WSI;
            ViewData["Id"] = id;
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public JsonResult ReturnProducts(string id, string quantities, string description)
        {
            NMExportsWSI ExportWSI = (NMExportsWSI)Session["ExportWSI"];

            NMImportsBL BL = new NMImportsBL();
            NMImportsWSI WSI = new NMImportsWSI();
            WSI.Mode = "SAV_OBJ";
            WSI.Import = new NEXMI.Imports();
            WSI.Import.SupplierId = ExportWSI.Customer.CustomerId;
            WSI.Import.ImportTypeId = NMConstant.ImportType.Return;
            WSI.Import.StockId = ExportWSI.Export.StockId;
            WSI.Import.Reference = ExportWSI.Export.ExportId;
            WSI.Import.ImportStatus = NMConstant.IMStatuses.Draft;
            WSI.Import.DescriptionInVietnamese = description;
            WSI.Import.DeliveryMethod = NMConstant.DeliveryMethod.Once;
            WSI.ActionBy = User.Identity.Name;
            WSI.Details = new List<ImportDetails>();
            ImportDetails Detail;
            string[] arr = quantities.Split(',');
            int i = 0; double qty = 0;
            foreach (ExportDetails Item in ExportWSI.Details)
            {
                try
                {
                    qty = 0;
                    qty = double.Parse(arr[i++]);
                }
                catch { }
                if (qty > 0)
                {
                    Detail = new ImportDetails();
                    Detail.ProductId = Item.ProductId;
                    Detail.StockId = Item.StockId;
                    Detail.RequiredQuantity = qty;
                    WSI.Details.Add(Detail);
                }
            }
            if (WSI.Details.Count > 0)
                WSI = BL.callSingleBL(WSI);
            else
                WSI.WsiError = "Bạn chưa nhập số lượng hàng trả!";

            return Json(new { id = WSI.Import.ImportId, error = WSI.WsiError });
        }

        public JsonResult TransferProducts(string id, string status)
        {
            string error = "";
            try
            {
                NMExportsBL BL = new NMExportsBL();
                NMExportsWSI WSI = new NMExportsWSI();
                WSI.Mode = "SEL_OBJ";
                WSI.Export = new Exports();
                WSI.Export.ExportId = id;
                WSI = BL.callSingleBL(WSI);
                error = WSI.WsiError;
                if (error == "")
                {
                    if (WSI.Export.ExportStatus != NMConstant.EXStatuses.Delivered)
                    {
                        WSI.Mode = "SAV_OBJ";
                        WSI.ActionBy = User.Identity.Name;
                        WSI.Export.ExportStatus = NMConstant.EXStatuses.Delivered;
                        WSI = BL.callSingleBL(WSI);
                        error = WSI.WsiError;
                        if (error == "" && WSI.Export.ExportTypeId == NMConstant.ExportType.Transfer)
                        {
                            NMImportsWSI ImportWSI = new NMImportsWSI();
                            NMImportsBL ImportBL = new NMImportsBL();
                            ImportWSI.Mode = "SAV_OBJ";
                            ImportWSI.Import = new Imports();
                            ImportWSI.Import.SupplierId = WSI.Export.CustomerId;
                            ImportWSI.Import.ImportDate = WSI.Export.ExportDate;
                            ImportWSI.Import.ImportTypeId = NMConstant.ImportType.Transfer;
                            ImportWSI.Import.ImportStatus = NMConstant.IMStatuses.Ready;
                            ImportWSI.Import.DescriptionInVietnamese = WSI.Export.DescriptionInVietnamese;
                            ImportWSI.Import.StockId = WSI.Export.ImportStockId;
                            ImportWSI.Import.ExportStockId = WSI.Export.StockId;
                            ImportWSI.Import.CarrierId = WSI.Export.CarrierId;
                            ImportWSI.Import.Transport = WSI.Export.Transport;
                            ImportWSI.ActionBy = User.Identity.Name;
                            ImportWSI.Import.Reference = WSI.Export.ExportId;
                            ImportWSI.Details = new List<ImportDetails>();
                            ImportDetails ImportDetail;
                            foreach (ExportDetails Item in WSI.Details)
                            {
                                ImportDetail = new ImportDetails();
                                ImportDetail.OrdinalNumber = 0;
                                ImportDetail.ProductId = Item.ProductId;
                                try
                                {
                                    ImportDetail.GoodQuantity = Item.Quantity;
                                    ImportDetail.RequiredQuantity = ImportDetail.GoodQuantity;
                                }
                                catch { }
                                ImportWSI.Details.Add(ImportDetail);
                            }
                            error = ImportBL.callSingleBL(ImportWSI).WsiError;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                error = "" + ex.Message + "\n" + ex.InnerException;
            }
            return Json(error);
        }

        public ActionResult PrintExport(string id)
        {
            NEXMI.NMExportsWSI WSI = new NEXMI.NMExportsWSI();
            NEXMI.NMExportsBL BL = new NEXMI.NMExportsBL();
            WSI.Mode = "SEL_OBJ";
            WSI.Export = new Exports();
            WSI.Export.ExportId = id;
            ViewData["WSI"] = BL.callSingleBL(WSI);

            if (NMCommon.GetSettingValue("EXPORT_FORM") == 2)
                return PartialView("PrintExport-02VT");

            return PartialView();
        }
        #endregion

        #region ProductInStocks
        public ActionResult ProductInStocks(String stockId)
        {
            ViewData["StockId"] = stockId;
            return PartialView();
        }

        public ActionResult ProductInStockList(string pageNum, string sortDataField, string sortOrder,
            string stockId, string categoryId, string keyword, string from, string to)
        {
            NMProductInStocksBL BL = new NMProductInStocksBL();
            NMProductInStocksWSI WSI = new NMProductInStocksWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.Keyword = keyword;
            WSI.PIS = new ProductInStocks();
            WSI.PIS.StockId = stockId;
            WSI.CategoryId = categoryId;
            //try
            //{
            //    WSI.PageNum = int.Parse(pageNum);
            //}
            //catch{
            //    WSI.PageNum = 0;
            //}
            //WSI.PageSize = NMCommon.PageSize();
            //WSI.SortField = sortDataField;
            //WSI.SortOrder = sortOrder;
            WSI.Filter.FromDate = from;
            WSI.Filter.ToDate = to;

            List<NMProductInStocksWSI> WSIs = BL.callListBL(WSI);
            ViewData["WSIs"] = WSIs;
            
            //lấy tất cả record NKC
            //NMMonthlyGeneralJournalsBL mgjBL = new NMMonthlyGeneralJournalsBL();
            //NMMonthlyGeneralJournalsWSI mgjWSI = new NMMonthlyGeneralJournalsWSI();
            //mgjWSI.Mode = "SRC_OBJ";
            //mgjWSI.MGJ.AccountId = "131";

            //int month = DateTime.Today.Month;
            //int year = DateTime.Today.Year;
            //if (month == 0)
            //{
            //    month = 12;
            //    year -= 1;
            //}

            //if (!string.IsNullOrEmpty(from))
            //    mgjWSI.Filter.FromDate = from;
            //else
            //    mgjWSI.Filter.FromDate = new DateTime(year, month, 01).ToString();

            //if (!string.IsNullOrEmpty(to))
            //    mgjWSI.Filter.ToDate = to;
            //else
            //    mgjWSI.Filter.ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)).ToString();

            //ViewData["mgjWSIs"] = mgjBL.callListBL(mgjWSI);

            return PartialView();
        }

        public ActionResult ProductInStockListFromMGJ(string pageNum, string sortDataField, string sortOrder,
            string stockId, string categoryId, string keyword, string from, string to)
        {
            //lấy tất cả record NKC
            NMMonthlyGeneralJournalsBL BL = new NMMonthlyGeneralJournalsBL();
            NMMonthlyGeneralJournalsWSI WSI = new NMMonthlyGeneralJournalsWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.MGJ.AccountId = "131";

            int month = DateTime.Today.Month;
            int year = DateTime.Today.Year;
            if (month == 0)
            {
                month = 12;
                year -= 1;
            }

            if (!string.IsNullOrEmpty(from))
                WSI.Filter.FromDate = from;
            else
                WSI.Filter.FromDate = new DateTime(year, month, 01).ToString();

            if (!string.IsNullOrEmpty(to))
                WSI.Filter.ToDate = to;
            else
                WSI.Filter.ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)).ToString();

            ViewData["WSIs"] = BL.callListBL(WSI);

            // lay danh sach san pham
            NMProductsBL ProductsBL = new NMProductsBL();
            NMProductsWSI ProductsWSI = new NMProductsWSI();
            ProductsWSI.Mode = "SRC_OBJ";
            ProductsWSI.Product = new Products();
            ProductsWSI.Keyword = keyword;
            ProductsWSI.Product.CategoryId = categoryId;
            
            ProductsWSI.SortField = sortDataField;
            ProductsWSI.SortOrder = sortOrder;
            if (GetPermissions.Get((List<NMPermissionsWSI>)Session["Permissions"], NMConstant.Functions.Products, "ViewAll") == false)
            {
                ProductsWSI.ActionBy = User.Identity.Name;
            }
            ViewData["ProductsWSI"] = ProductsBL.callListBL(ProductsWSI);

            ViewData["dtFrom"] = from;
            ViewData["dtTo"] = to;

            return PartialView();
        }

        public JsonResult GetProductInStocks(int pageNum, int pageSize, string sortDataField, string sortOrder,
            string stockId, string categoryId, string keyword)
        {
            NMProductInStocksBL BL = new NMProductInStocksBL();
            NMProductInStocksWSI WSI = new NMProductInStocksWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.Keyword = keyword;
            WSI.PIS = new ProductInStocks();
            WSI.PIS.StockId = stockId;
            WSI.CategoryId = categoryId;
            WSI.PageNum = pageNum;
            WSI.PageSize = pageSize;
            WSI.SortField = sortDataField;
            WSI.SortOrder = sortOrder;
            List<NMProductInStocksWSI> WSIs = BL.callListBL(WSI);
            int totalRows = 0;
            try
            {
                totalRows = WSIs[0].TotalRows;
            }
            catch { }
            var data = WSIs.Select(i => new
            {
                StockName = (i.StockWSI != null && i.StockWSI.Translation != null) ? i.StockWSI.Translation.Name : "",
                ProductNameInVietnamese = (i.ProductWSI.Translation == null) ? "" : "[" + i.ProductWSI.Product.ProductCode + "] " + i.ProductWSI.Translation.Name,
                GoodQuantity = (i.PIS.BeginQuantity + i.PIS.ImportQuantity - i.PIS.ExportQuantity - i.PIS.BadProductInStock),
                Amount = (i.PIS.BeginQuantity + i.PIS.ImportQuantity - i.PIS.ExportQuantity - i.PIS.BadProductInStock) * i.PIS.CostPrice,
                BadQuantity = i.PIS.BadProductInStock,
                Begin = i.PIS.BeginQuantity,
                Import = i.PIS.ImportQuantity,
                Export = i.PIS.ExportQuantity,
                Min = i.PIS.MinQuantity,
                Max = i.PIS.MaxQuantity,
                Status = ((i.PIS.BeginQuantity + i.PIS.ImportQuantity - i.PIS.ExportQuantity - i.PIS.BadProductInStock) > i.PIS.MaxQuantity) ? "Vượt" : ""
            });
            return Json(new { Rows = data, TotalRows = totalRows }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ProductInStockForm(String productId, String stockId)
        {
            ViewData["ProductId"] = productId;
            ViewData["StockId"] = stockId;
            return PartialView();
        }

        public ActionResult UpdateStock()
        {
            return PartialView();
        }

        public ActionResult UpdateStockUC(string stockId, string categoryId)
        {
            NEXMI.NMProductInStocksBL BL = new NEXMI.NMProductInStocksBL();
            NEXMI.NMProductInStocksWSI WSI = new NEXMI.NMProductInStocksWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.PIS = new NEXMI.ProductInStocks();
            WSI.PIS.StockId = stockId;
            WSI.CategoryId = categoryId;

            ViewData["WSIs"] = BL.callListBL(WSI);
            return PartialView();
        }

        public JsonResult AddToList(string stockId, string productId, string goodQty, string badQty, string price)
        {
            string error = "";
            try
            {
                List<ImportDetails> list = new List<ImportDetails>();
                if (Session["List"] != null)
                    list = (List<ImportDetails>)Session["List"];
                bool flag = true;
                foreach (ImportDetails Item in list)
                {
                    if (Item.ProductId == productId && Item.StockId == stockId)
                    {
                        flag = false;
                        if (goodQty == "" && badQty == "")
                        {
                            list.Remove(Item);
                            break;
                        }
                        else
                        {
                            try
                            {
                                Item.GoodQuantity = double.Parse(goodQty);
                            }
                            catch { }
                            try
                            {
                                Item.BadQuantity = double.Parse(badQty);
                            }
                            catch { }
                            try
                            {
                                Item.Price = double.Parse(price);
                            }
                            catch { }
                        }
                    }
                }
                if (flag)
                {
                    if (goodQty != "" || badQty != "")
                    {
                        ImportDetails obj = new ImportDetails();
                        obj.StockId = stockId;
                        obj.ProductId = productId;
                        try
                        {
                            obj.GoodQuantity = double.Parse(goodQty);
                        }
                        catch { }
                        try
                        {
                            obj.BadQuantity = double.Parse(badQty);
                        }
                        catch { }
                        try
                        {
                            obj.Price = double.Parse(price);
                        }
                        catch { }
                        list.Add(obj);
                    }
                }
                Session["List"] = list;
            }
            catch (Exception ex)
            {
                error = "Lỗi!\n" + ex.Message;
            }
            return Json(error);
        }

        /// <summary>
        /// cap nhat so luong ton va gia tot xau trong kho
        /// </summary>
        /// <returns></returns>
        public JsonResult UpdateGBProductInStock()
        {
            string error = "";
            if (Session["List"] != null)
            {
                int num = 0;
                List<ImportDetails> pislist = new List<ImportDetails>();
                pislist = (List<ImportDetails>)Session["List"];
                NMProductInStocksBL BL = new NMProductInStocksBL();
                NMProductInStocksWSI WSI;

                NMMonthlyGeneralJournalsBL mgbl = new NMMonthlyGeneralJournalsBL();
                NMMonthlyGeneralJournalsWSI mgwsi = new NMMonthlyGeneralJournalsWSI();
                List<NMMonthlyGeneralJournalsWSI> mgjList;
                List<MonthlyGeneralJournals> mgList = new List<MonthlyGeneralJournals>();
                MonthlyGeneralJournals mgj;

                foreach (ImportDetails Item in pislist)
                {
                    WSI = new NMProductInStocksWSI();
                    WSI.Mode = "SEL_OBJ";
                    WSI.PIS = new NEXMI.ProductInStocks();
                    WSI.PIS.ProductId = Item.ProductId;
                    WSI.PIS.StockId = Item.StockId;
                    WSI = BL.callSingleBL(WSI);

                    WSI.Mode = "SAV_OBJ";
                    WSI.PIS.BeginQuantity = Item.GoodQuantity;                    
                    WSI.PIS.CostPrice = Item.Price;
                    WSI.PIS.BadProductInStock = Item.BadQuantity;
                    WSI.ActionBy = User.Identity.Name;
                    WSI = BL.callSingleBL(WSI);
                    if (WSI.WsiError != "")
                    {
                        num += 1;
                        error = "Có " + num + " sản phẩm không cập nhật được.\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục!";
                        break;
                    }

                    mgj = mgList.Where(m => m.ProductId == Item.ProductId & m.StockId == Item.StockId).FirstOrDefault();
                    if (mgj == null)
                    {
                        mgj = new MonthlyGeneralJournals();
                        mgj.ProductId = Item.ProductId;
                        mgj.UnitId = NMCommon.GetProductUnit(Item.ProductId);
                        mgj.AccountId = NMCommon.GetProductType(Item.ProductId);    // "1561";
                        mgj.ImportQuantity = pislist.Where(p => p.ProductId == Item.ProductId).Sum(s => s.GoodQuantity);
                        mgj.ExportQuantity = 0;
                        mgj.DebitAmount = pislist.Where(p => p.ProductId == Item.ProductId).Sum(s => s.Amount);
                        mgj.CreditAmount = 0;
                        mgj.IssueId = "SDDK";
                        mgj.PartnerId = "COMPANY";
                        mgj.ActionBy = User.Identity.Name;
                        mgj.StockId = Item.StockId;
                        mgj.CurrencyId = "VND";
                        mgj.ExchangeRate = 1;
                        mgj.IsBegin = true;
                        mgj.Descriptions = "Số dư đầu kỳ.";

                        mgList.Add(mgj);
                    }
                }

                if (error == "")
                {
                    foreach (MonthlyGeneralJournals itm in mgList)
                    {
                        mgwsi = new NMMonthlyGeneralJournalsWSI();
                        // tim co da luu chua
                        mgwsi.Mode = "SRC_OBJ";
                        mgwsi.MGJ.IssueId = itm.IssueId;
                        mgwsi.MGJ.PartnerId = itm.PartnerId;
                        mgwsi.MGJ.AccountId = itm.AccountId;
                        mgwsi.MGJ.ProductId = itm.ProductId;
                        mgwsi.MGJ.StockId = itm.StockId;
                        
                        mgjList = mgbl.callListBL(mgwsi);

                        if (mgjList.Count > 0)
                        {
                            itm.JournalId = mgjList[0].MGJ.JournalId;
                        }

                        itm.IssueDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01);
                        mgwsi.MGJ = itm;

                        mgwsi.Filter.ActionBy = User.Identity.Name;
                        mgwsi.Mode = "SAV_OBJ";
                        mgwsi = mgbl.callSingleBL(mgwsi);
                    }

                    Session.Remove("List");
                }
            }

            return Json(error);
        }

        public ActionResult InventoryNorms()
        {
            return PartialView();
        }

        public ActionResult InventoryNormsUC(string stockId, string categoryId)
        {
            NEXMI.NMProductInStocksBL BL = new NEXMI.NMProductInStocksBL();
            NEXMI.NMProductInStocksWSI WSI = new NEXMI.NMProductInStocksWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.PIS = new NEXMI.ProductInStocks();
            WSI.PIS.StockId = stockId;
            WSI.CategoryId = categoryId;

            ViewData["WSIs"] = BL.callListBL(WSI);
            return PartialView();
        }

        /// <summary>
        /// cap nhat gia tri Min Max cho moi kho
        /// </summary>
        /// <returns></returns>
        public JsonResult UpdateMMProductInStock()
        {
            string error = ""; int num = 0;
            List<ImportDetails> list = new List<ImportDetails>();
            if (Session["List"] != null)
            {
                list = (List<ImportDetails>)Session["List"];
                NMProductInStocksBL BL = new NMProductInStocksBL();
                NMProductInStocksWSI WSI;
                foreach (ImportDetails Item in list)
                {
                    WSI = new NMProductInStocksWSI();
                    WSI.Mode = "SEL_OBJ";
                    WSI.PIS = new NEXMI.ProductInStocks();
                    WSI.PIS.ProductId = Item.ProductId;
                    WSI.PIS.StockId = Item.StockId;
                    WSI = BL.callSingleBL(WSI);

                    WSI.Mode = "SAV_OBJ";
                    WSI.PIS.MaxQuantity = Item.GoodQuantity;
                    WSI.PIS.MinQuantity = Item.BadQuantity;
                    WSI.ActionBy = User.Identity.Name;
                    WSI = BL.callSingleBL(WSI);
                    if (WSI.WsiError != "")
                    {
                        num += 1;
                        error = "Có " + num + " sản phẩm không cập nhật được.\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục!";
                    }

                }
                Session.Remove("List");
            }
            return Json(error);
        }

        //public JsonResult SaveMonthlyInventoryControl(string stockId, string productId, string beginQuantity, string importQuantity, string exportQuantity, string endQuantity)
        //{
        //    NMMonthlyInventoryControlBL BL = new NMMonthlyInventoryControlBL();
        //    NMMonthlyInventoryControlWSI WSI = new NMMonthlyInventoryControlWSI();
        //    WSI.Mode = "SAV_OBJ";
        //    WSI.ProductId = productId;
        //    WSI.StockId = stockId;
        //    WSI.BeginQuantity = beginQuantity;
        //    WSI.ImportQuantity = importQuantity;
        //    WSI.ExportQuantity = exportQuantity;
        //    WSI.EndQuantity = endQuantity;
        //    WSI.CreatedBy = User.Identity.Name;
        //    WSI.ModifiedBy = User.Identity.Name;
        //    WSI = BL.callSingleBL(WSI);

        //    return Json(WSI.WsiError);
        //}
        #endregion

        #region TransferProducts
        public ActionResult TransferProductList()
        {
            ViewData["IMStatus"] = NMConstant.IMStatuses.Draft + "," + NMConstant.IMStatuses.Ready;
            ViewData["IMTypeId"] = NMConstant.ImportType.Transfer;
            ViewData["EXStatus"] = NMConstant.EXStatuses.Draft;
            ViewData["EXTypeId"] = NMConstant.ExportType.Transfer;
            return View();
        }

        public ActionResult TransferProductForm()
        {
            Session["Details"] = null;
            return PartialView();
        }

        public JsonResult Transfer(string fromStock, string toStock, string txtDate, string carrierId, string transport, string description)
        {
            //if (Session["Details"] == null)
            //return Json(new { error = "Vui lòng nhập chi tiết hàng!", id = "" });
            string error = "", exportId = "";
            try
            {
                NMExportsWSI ExportWSI = new NMExportsWSI();
                NMExportsBL ExportBL = new NMExportsBL();
                ExportWSI.Mode = "SAV_OBJ";
                ExportWSI.Export = new Exports();
                ExportWSI.Export.ExportDate = DateTime.Parse(txtDate);
                ExportWSI.Export.ExportTypeId = NMConstant.ExportType.Transfer;
                ExportWSI.Export.ExportStatus = NMConstant.EXStatuses.Draft;
                ExportWSI.Export.DescriptionInVietnamese = description;
                ExportWSI.Export.StockId = fromStock;
                ExportWSI.Export.ImportStockId = toStock;
                if (!string.IsNullOrEmpty(carrierId))
                    ExportWSI.Export.CarrierId = carrierId;
                ExportWSI.Export.Transport = transport;
                ExportWSI.ActionBy = User.Identity.Name;
                ExportWSI.Details = (List<ExportDetails>)Session["Details"];
                ExportWSI = ExportBL.callSingleBL(ExportWSI);
                error = ExportWSI.WsiError;
                exportId = ExportWSI.Export.ExportId;
            }
            catch (Exception ex)
            {
                error = "Không thực hiện được!\nLiên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message;
            }
            return Json(new { error = error, id = exportId });
        }
        #endregion

        #region Cancel Products
        public ActionResult CancelProducts()
        {
            ViewData["Status"] = NMConstant.IMStatuses.Draft;
            ViewData["TypeId"] = NMConstant.ImportType.Transfer;



            return View();
        }
        #endregion

        public ActionResult CloseInventory()
        {
            return PartialView();
        }

        public ActionResult CloseInventoryUC(string stockId, string categoryId, string keyword)
        {
            NMMonthlyInventoryControlBL BL = new NMMonthlyInventoryControlBL();
            NMMonthlyInventoryControlWSI WSI = new NMMonthlyInventoryControlWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.MIC = new MonthlyInventoryControl();
            WSI.MIC.StockId = stockId;
            WSI.CategoryId = categoryId;
            WSI.Filter.Keyword = keyword;
            ViewData["WSIs"] = BL.callListBL(WSI);
            return PartialView();
        }

        public JsonResult SaveCloseInventory(string stockId)
        {
            NMMonthlyInventoryControlBL BL = new NMMonthlyInventoryControlBL();
            NMMonthlyInventoryControlWSI WSI = new NMMonthlyInventoryControlWSI();
            WSI.Mode = "CLO_INV";
            WSI.MIC = new MonthlyInventoryControl();
            WSI.MIC.StockId = stockId;
            WSI.Filter.ActionBy = User.Identity.Name;
            WSI = BL.callSingleBL(WSI);
            return Json(WSI.WsiError);
        }

        #region Export cho LO

        public ActionResult ExportPartsFromLO(string orderId, string mode, string typeId)
        {
            string customerId = "", customerName = "", customerAddress = "", reference = "", stockId = ((NEXMI.NMCustomersWSI)Session["UserInfo"]).Customer.StockId,   //UserInfoCache.User.Customer.StockId,
                description = "", status = NEXMI.NMConstant.EXStatuses.Draft, carrierId = "", carrierName = "", transport = "",
                importStockId = "";
            if (typeId == null) typeId = "";
            DateTime exportDate = DateTime.Now;
            List<ExportDetails> Details = new List<ExportDetails>();
            ExportDetails Detail;
            //if (!string.IsNullOrEmpty(exportId))
            //{
            //    NMExportsBL BL = new NMExportsBL();
            //    NMExportsWSI WSI = new NMExportsWSI();
            //    WSI.Mode = "SEL_OBJ";
            //    WSI.Export = new Exports();
            //    WSI.Export.ExportId = exportId;
            //    WSI = BL.callSingleBL(WSI);
            //    if (WSI.Customer != null)
            //    {
            //        customerId = WSI.Customer.CustomerId; customerName = WSI.Customer.CompanyNameInVietnamese;
            //        customerAddress = WSI.Customer.Address; reference = WSI.Export.Reference;
            //    }
            //    if (WSI.Carrier != null)
            //    {
            //        carrierId = WSI.Carrier.CustomerId;
            //        carrierName = WSI.Carrier.CompanyNameInVietnamese;
            //    }
            //    transport = WSI.Export.Transport;
            //    exportDate = WSI.Export.ExportDate; description = WSI.Export.DescriptionInVietnamese;
            //    stockId = WSI.Export.StockId;
            //    status = WSI.Export.ExportStatus;
            //    typeId = WSI.Export.ExportTypeId;
            //    if (!string.IsNullOrEmpty(WSI.Export.ImportStockId))
            //        importStockId = WSI.Export.ImportStockId;
            //    foreach (ExportDetails Item in WSI.Details)
            //    {
            //        Session["SNs" + Item.ProductId] = string.Join(",", (WSI.ProductSNs.Select(i => i).Where(i => i.ProductId == Item.ProductId)).Select(i => i.SerialNumber));
            //        if (mode == "Copy") Item.OrdinalNumber = 0;
            //        Details.Add(Item);
            //    }
            //}
            //else
            //{
            //    
            string exportId = "";
            if (!string.IsNullOrEmpty(orderId))
            {
                NMSalesOrdersBL SOBL = new NMSalesOrdersBL();
                NMSalesOrdersWSI SOWSI = new NMSalesOrdersWSI();
                SOWSI.Mode = "SEL_OBJ";
                SOWSI.Order = new NEXMI.SalesOrders();
                SOWSI.Order.OrderId = orderId;
                SOWSI = SOBL.callSingleBL(SOWSI);
                customerId = SOWSI.Customer.CustomerId; customerName = SOWSI.Customer.CompanyNameInVietnamese;
                customerAddress = SOWSI.Customer.Address; reference = SOWSI.Order.OrderId;
                
                NMProductsBL pBL = new NMProductsBL();
                NMProductsWSI pWSI = new NMProductsWSI();
                pWSI.Mode = "SEL_OBJ";

                int month = DateTime.Today.Month - SOWSI.Order.OrderDate.Month;
                foreach (SalesOrderDetails Item in SOWSI.Details)
                {
                    pWSI.Product.ProductId = Item.ProductId;
                    pWSI = pBL.callSingleBL(pWSI);

                    if (pWSI.BoMs != null)
                    {
                        foreach (var part in pWSI.BoMs)
                        {
                            if (part.ReplacementTime == month | part.ReplacementTime * 2 == month | part.ReplacementTime * 4 == month)
                            {
                                NMProductsBL partBL = new NMProductsBL();
                                NMProductsWSI partWSI = new NMProductsWSI();
                                partWSI.Mode = "SEL_OBJ";
                                partWSI.Product.ProductId = part.ProductId;
                                partWSI = partBL.callSingleBL(pWSI);

                                Detail = new ExportDetails();
                                Detail.ProductId = part.ProductId;
                                Detail.UnitId = partWSI.Product.ProductUnit;
                                Detail.RequiredQuantity = Item.Quantity;
                                Details.Add(Detail);
                            }
                        }
                    }
                }
            }
            //}
            //if (mode == "Copy")
            //{
            //    exportId = "";
            //    status = NMConstant.EXStatuses.Draft;
            //    exportDate = DateTime.Now;
            //    stockId = ((NEXMI.NMCustomersWSI)Session["UserInfo"]).Customer.StockId;
            //    reference = "";
            //}
            ViewData["Id"] = exportId;
            ViewData["CustomerId"] = customerId;
            ViewData["CustomerName"] = customerName;
            ViewData["CustomerAddress"] = customerAddress;
            ViewData["CarrierId"] = carrierId;
            ViewData["CarrierName"] = carrierName;
            ViewData["Transport"] = transport;
            ViewData["ExportDate"] = exportDate.ToString("yyyy-MM-dd");
            ViewData["ExportStatus"] = status;
            ViewData["ExportTypeId"] = typeId;
            ViewData["Reference"] = reference;
            ViewData["Description"] = description;
            ViewData["ImportStockId"] = importStockId;
            ViewData["slStocks"] = stockId;
            Session["Details"] = Details;

            ViewData["StockInput"] = NMCommon.GetSetting(NMConstant.Settings.SELECT_STORE_BY_USER_4_EXPORT);

            ViewData["ViewType"] = "detail";

            return PartialView();
        }

        #endregion
    }
}
