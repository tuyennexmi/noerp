// DirectoriesController.cs

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
using System.Drawing;
using System.IO;
using nexmiStore.Models;
using System.Text.RegularExpressions;
using NEXMI;

namespace nexmiStore.Controllers
{
    public class DirectoriesController : Controller
    {
        //
        // GET: /Directories/

        public ActionResult Index()
        {
            return View();
        }

        #region Product Categories
        public ActionResult ProductCategories()
        {
            return PartialView();
        }

        public ActionResult ProductCategoryForm(String id, string parentElement, string objectName, string windowId)
        {
            if (!string.IsNullOrEmpty(id))
            {
                NEXMI.NMCategoriesBL BL = new NEXMI.NMCategoriesBL();
                NEXMI.NMCategoriesWSI WSI = new NEXMI.NMCategoriesWSI();
                WSI.Mode = "SEL_OBJ";
                WSI.Category = new Categories();
                WSI.Category.Id = id;
                WSI.LanguageId = Session["Lang"].ToString();
                WSI = BL.callSingleBL(WSI);
                if (WSI.Category != null)
                {
                    ViewData["Id"] = WSI.Category.Id;
                    ViewData["Code"] = WSI.Category.CustomerCode;
                    ViewData["name"] = WSI.Category.Name;
                    ViewData["description"] = WSI.Category.Description;
                    objectName = WSI.Category.ObjectName;
                    if (!string.IsNullOrEmpty(WSI.Category.Image))
                        ViewData["avatar"] = Url.Content("~/Content/avatars/") + WSI.Category.Image;
                    string parentId = WSI.Category.ParentId;
                    WSI = new NMCategoriesWSI();
                    WSI.Mode = "SEL_OBJ";
                    WSI.Category = new Categories();
                    WSI.Category.Id = parentId;
                    WSI.LanguageId = Session["Lang"].ToString();
                    WSI = BL.callSingleBL(WSI);
                    if (WSI.WsiError == "")
                    {
                        ViewData["CategoryId"] = WSI.Category.Id;
                        ViewData["CategoryName"] = WSI.FullName;
                    }
                }
            }
            ViewData["ParentElement"] = parentElement;
            ViewData["ObjectName"] = objectName;
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public JsonResult SaveOrUpdateProductCategory(String id, String code, string objectName, String parentId, string fileName, 
            string imageUrl)
        {
            NMCategoriesWSI WSI = new NMCategoriesWSI();
            NMCategoriesBL BL = new NMCategoriesBL();
            WSI.Category = new Categories();
            WSI.Category.Id = id;

            if (!string.IsNullOrEmpty(id))
            {
                WSI.Mode = "SEL_OBJ";
                WSI = BL.callSingleBL(WSI);
            }

            WSI.Mode = "SAV_OBJ";
            WSI.Category.CustomerCode = code;
            WSI.Category.ObjectName = objectName;
            if (!string.IsNullOrEmpty(parentId))
                WSI.Category.ParentId = parentId;
            else
                WSI.Category.ParentId = null;
            if (!string.IsNullOrEmpty(fileName))
            {
                UserControlController UC = new UserControlController();
                WSI.Category.Image = UC.Upload(imageUrl, Server.MapPath(@"~/Content/avatars/"), fileName);
            }
            WSI.ActionBy = User.Identity.Name;
            WSI = BL.callSingleBL(WSI);
            if (WSI.WsiError == "")
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
                    TranslationWSI.Translation.OwnerId = WSI.Category.Id;
                    TranslationWSI.Translation.LanguageId = languageId;
                    TranslationWSI.Translation.Name = Request.Params["name" + languageId];
                    TranslationWSI.Translation.Description = NMCryptography.base64Decode(Request.Params["description" + languageId]);
                    TranslationWSI = TranslationBL.callSingleBL(TranslationWSI);
                }
            }
            return Json(new { error = WSI.WsiError, id = WSI.Category.Id });
        }

        public JsonResult DeleteProductCategory(String id)
        {
            NMCategoriesWSI WSI = new NMCategoriesWSI();
            NMCategoriesBL BL = new NMCategoriesBL();
            WSI.Mode = "DEL_OBJ";
            WSI.Category = new Categories();
            WSI.Category.Id = id;
            WSI = BL.callSingleBL(WSI);
            return Json(WSI.WsiError);
        }
        #endregion

        #region Products
        public ActionResult ProductList(string groupId, string pageNum, string categoryId, string keyword, string typeId, string view)
        {
            if (string.IsNullOrEmpty(groupId)) groupId = NMConstant.ProductGroups.Product;
            int page = 1;
            try
            {
                page = int.Parse(pageNum);
            }
            catch { }

            NMProductsBL BL = new NMProductsBL();
            NMProductsWSI WSI = new NMProductsWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.Product = new Products();
            WSI.Product.CategoryId = categoryId;
            WSI.Product.GroupId = groupId;
            WSI.Product.TypeId = typeId;
            WSI.StockId = ((NMCustomersWSI)Session["UserInfo"]).Customer.StockId;
            if (GetPermissions.GetViewAll((List<NMPermissionsWSI>)Session["Permissions"], NMConstant.Functions.Products) == false)
            {
                WSI.ActionBy = User.Identity.Name;
            }
            WSI.PageNum = page - 1;
            WSI.PageSize = NMCommon.PageSize();
            WSI.Keyword = NMCryptography.base64Decode(keyword);
            WSI.LanguageId = Session["Lang"].ToString();
            List<NEXMI.NMProductsWSI> WSIs = BL.callListBL(WSI);

            ViewData["WSIs"] = WSIs;
            ViewData["Page"] = page;
            
            ViewData["ViewType"] = view;
            ViewData["Keyword"] = keyword;
            ViewData["CategoryId"] = categoryId;
            ViewData["GroupId"] = groupId;
            ViewData["typeId"] = typeId;

            if (view == "kanban")
                return PartialView("ProductGrid");

            return PartialView();
        }

        public ActionResult Products(string groupId, String productCategoryId, string typeId)
        {
            ViewData["ProductCategoryId"] = productCategoryId;
            NEXMI.NMCategoriesWSI PCWSI = new NEXMI.NMCategoriesWSI();
            NEXMI.NMCategoriesBL PCBL = new NEXMI.NMCategoriesBL();
            PCWSI.Mode = "SRC_OBJ";
            ViewData["PCWSIs"] = PCBL.callListBL(PCWSI);
            if (string.IsNullOrEmpty(groupId)) groupId = NMConstant.ProductGroups.Product;
            ViewData["GroupId"] = groupId;
            ViewData["TypeId"] = typeId;
            ViewData["ViewType"] = "kanban";
            return PartialView();
        }

        public ActionResult ProductDetail(String id)
        {
            string avatar = "Content/UI_Images/noimage.png";
            List<NEXMI.NMImagesWSI> ImageWSIs = new List<NMImagesWSI>();
            if (!string.IsNullOrEmpty(id))
            {
                NEXMI.NMProductsBL BL = new NEXMI.NMProductsBL();
                NEXMI.NMProductsWSI WSI = new NEXMI.NMProductsWSI();
                WSI.Mode = "SEL_OBJ";
                WSI.StockId = ((NMCustomersWSI)Session["UserInfo"]).Customer.StockId;
                WSI.Product = new Products();
                WSI.Product.ProductId = id;
                WSI.LanguageId = Session["Lang"].ToString();
                WSI = BL.callSingleBL(WSI);
                if (WSI.Translation == null)
                    WSI.Translation = new Translations();
                ViewData["ProductId"] = WSI.Product.ProductId;
                ViewData["ProductCode"] = WSI.Product.ProductCode;
                ViewData["ProductName"] = WSI.Translation.Name;
                if (WSI.Product.GroupId == null) WSI.Product.GroupId = "";
                ViewData["GroupId"] = WSI.Product.GroupId;
                if (WSI.Product.TypeId == null) WSI.Product.TypeId = "";
                ViewData["TypeId"] = WSI.Product.TypeId;
                ViewData["slProductUnits"] = (WSI.Unit == null) ? "" : WSI.Unit.Name;
                ViewData["VAT"] = WSI.Product.VATRate.ToString("N3");
                ViewData["ShortDescription"] = WSI.Translation.ShortDescription;
                ViewData["Warranty"] = WSI.Product.Warranty;
                ViewData["BarCode"] = WSI.Product.BarCode;
                ViewData["CostPrice"] = (GetPermissions.GetViewPrice((List<NMPermissionsWSI>)Session["Permissions"], NEXMI.NMConstant.Functions.Products)) ? WSI.Product.CostPrice.ToString("N3") : "";
                // for log and messages
                ViewData["SendTo"] = WSI.Product.CreatedBy;    // dùng để gửi tin nhắn đến người quản lý
                //if (WSI.Order.SalesPersonId != WSI.Order.CreatedBy)  //người tạo khác người quản lý
                //    ViewData["SendTo"] += ";" + WSI.Order.CreatedBy;   //gửi tin đến người tạo

                if (WSI.Product.Discontinued != null)
                    if (WSI.Product.Discontinued.Value == true)
                        ViewData["Discontinued"] = "checked";
                ViewData["Description"] = WSI.Translation.Description;
                if (WSI.CategoryWSI.Category != null)
                {
                    NMCategoriesBL CategoryBL = new NMCategoriesBL();
                    NMCategoriesWSI CategoryWSI = new NMCategoriesWSI();
                    CategoryWSI.Mode = "SEL_OBJ";
                    CategoryWSI.Category = new Categories();
                    CategoryWSI.Category.Id = WSI.Product.CategoryId;
                    CategoryWSI = CategoryBL.callSingleBL(CategoryWSI);
                    ViewData["CategoryId"] = CategoryWSI.Category.Id;
                    ViewData["CategoryName"] = CategoryWSI.FullName;
                }
                if (WSI.Manufacture != null)
                {
                    ViewData["CustomerId"] = WSI.Manufacture.CustomerId;
                    ViewData["CustomerName"] = WSI.Manufacture.CompanyNameInVietnamese;
                }
                ViewData["Price"] = (WSI.PriceForSales == null) ? "" : (GetPermissions.GetViewPrice((List<NMPermissionsWSI>)Session["Permissions"], NEXMI.NMConstant.Functions.Products)) ? WSI.PriceForSales.Price.ToString("N3") : "";

                ViewData["ImageWSIs"] = WSI.Images;
                if (WSI.Images.Count > 0)
                {
                    var ImageDefault = WSI.Images.Where(i => i.IsDefault.ToString().ToLower() == "true").FirstOrDefault();
                    if (ImageDefault == null)
                    {
                        ImageDefault = WSI.Images[0];
                    }
                    avatar = "uploads/_thumbs/" + ImageDefault.Name;
                }
                ViewData["BoMs"] = WSI.BoMs;
            }
            ViewData["Avatar"] = avatar;
            ViewData["ViewType"] = "detail";
            return PartialView();
        }

        public ActionResult ProductForm(String id, string pgroupId, string parentId, string windowId, string typeId)
        {
            string avatar = "Content/UI_Images/noimage.png";
            List<NEXMI.NMImagesWSI> ImageWSIs = new List<NMImagesWSI>();
            //if (groupId == "") groupId = NMConstant.ProductGroups.Product;
            if (!string.IsNullOrEmpty(id))
            {
                NEXMI.NMProductsBL BL = new NEXMI.NMProductsBL();
                NEXMI.NMProductsWSI WSI = new NEXMI.NMProductsWSI();
                WSI.Mode = "SEL_OBJ";
                WSI.Product = new Products();
                WSI.Product.ProductId = id;
                WSI.StockId = ((NMCustomersWSI)Session["UserInfo"]).DefaultStock.Id;
                WSI = BL.callSingleBL(WSI);
                ViewData["ProductId"] = WSI.Product.ProductId;
                ViewData["ProductCode"] = WSI.Product.ProductCode;
                //ViewData["ProductName"] = WSI.Product.ProductNameInVietnamese;
                ViewData["slProductUnits"] = WSI.Product.ProductUnit;
                typeId = WSI.Product.TypeId;
                pgroupId = WSI.Product.GroupId;
                ViewData["VAT"] = WSI.Product.VATRate;
                //ViewData["ShortDescription"] = WSI.Product.ShortDescription;
                ViewData["Warranty"] = WSI.Product.Warranty;
                ViewData["BarCode"] = WSI.Product.BarCode;
                ViewData["CostPrice"] = WSI.Product.CostPrice;
                //ViewData["Discount"] = WSI.Product.DefaultDiscount;
                if (WSI.Product.Discontinued != null)
                    if (WSI.Product.Discontinued.Value == true)
                        ViewData["Discontinued"] = "checked";
                if (WSI.Product.Highlight != null)
                    if (WSI.Product.Highlight.Value == true)
                        ViewData["Highlight"] = "checked";
                if (WSI.Product.IsNew != null)
                    if (WSI.Product.IsNew.Value == true)
                        ViewData["IsNew"] = "checked";
                if (WSI.Product.IsEmpty != null)
                    if (WSI.Product.IsEmpty.Value == true)
                        ViewData["IsEmpty"] = "checked";
                //try
                //{
                //    ViewData["Description"] = NEXMI.NMCryptography.base64Decode(WSI.Product.Description);
                //}
                //catch
                //{
                    //ViewData["Description"] = WSI.Product.Description;
                //}
                if (WSI.CategoryWSI.Category != null)
                {
                    NMCategoriesBL CategoryBL = new NMCategoriesBL();
                    NMCategoriesWSI CategoryWSI = new NMCategoriesWSI();
                    CategoryWSI.Mode = "SEL_OBJ";
                    CategoryWSI.Category = new Categories();
                    CategoryWSI.Category.Id = WSI.Product.CategoryId;
                    CategoryWSI = CategoryBL.callSingleBL(CategoryWSI);
                    ViewData["CategoryId"] = CategoryWSI.Category.Id;
                    ViewData["CategoryName"] = CategoryWSI.FullName;
                }
                if (WSI.Manufacture != null)
                {
                    ViewData["CustomerId"] = WSI.Manufacture.CustomerId;
                    ViewData["CustomerName"] = WSI.Manufacture.CompanyNameInVietnamese;
                }
                ViewData["Price"] = (WSI.PriceForSales == null) ? 0 : WSI.PriceForSales.Price;

                NEXMI.NMImagesBL ImageBL = new NEXMI.NMImagesBL();
                NEXMI.NMImagesWSI ImageWSI = new NEXMI.NMImagesWSI();
                ImageWSI.Mode = "SRC_OBJ";
                ImageWSI.Owner = id;
                ImageWSI.TypeId = "IMG";
                ImageWSIs = ImageBL.callListBL(ImageWSI);
                ViewData["ImageWSIs"] = ImageWSIs;
                if (ImageWSIs.Count > 0)
                {
                    var ImageDefault = ImageWSIs.Where(i => i.IsDefault.ToUpper() == "true".ToUpper()).FirstOrDefault();
                    if (ImageDefault == null)
                    {
                        ImageDefault = ImageWSIs[0];
                    }
                    avatar = "uploads/_thumbs/" + ImageDefault.Name;
                }
            }
            if (string.IsNullOrEmpty(pgroupId)) pgroupId = NMConstant.ProductGroups.Product;
            ViewData["GroupId"] = pgroupId;
            ViewData["TypeId"] = typeId;
            ViewData["Avatar"] = avatar;
            ViewData["ParentId"] = parentId;
            ViewData["WindowId"] = windowId;
            ViewData["ViewType"] = "detail";
            return PartialView();
        }

        public JsonResult SaveOrUpdateProduct(string id, string productCode, string unit, string productCategory, string manufactureId, string VAT,
            string images, string price, string costPrice, string barCode, string isDiscontinued, string isHighlight, string isNew, string isEmpty,
            string defaultDiscount, string typeId, string pgroupId)
        {
            string result = "";
            NMProductsWSI WSI = new NMProductsWSI();
            NMProductsBL BL = new NMProductsBL();
            WSI.Product = new Products();
            
            if (!string.IsNullOrEmpty(id))
            {
                WSI.Mode = "SEL_OBJ";
                WSI.Product.ProductId = id;
                WSI = BL.callSingleBL(WSI);
            }

            WSI.Mode = "SAV_OBJ";
            WSI.Product.ProductCode = productCode;
            //WSI.Product.ProductNameInVietnamese = name;
            if (!string.IsNullOrEmpty(typeId))
                WSI.Product.TypeId = typeId;
            if (!string.IsNullOrEmpty(pgroupId))
                WSI.Product.GroupId = pgroupId;
            WSI.Product.ProductUnit = unit;
            if (!string.IsNullOrEmpty(productCategory))
                WSI.Product.CategoryId = productCategory;
            if (!string.IsNullOrEmpty(manufactureId))
                WSI.Product.ManufactureId = manufactureId;
            try
            {
                WSI.Product.VATRate = double.Parse(VAT);
            }
            catch { }
            //WSI.Product.ShortDescription = shortDescription;
            //WSI.Product.Description = NEXMI.NMCryptography.base64Decode(description);
            WSI.Product.BarCode = barCode;
            //WSI.Product.Warranty = warranty;
            try
            {
                WSI.Product.Discontinued = bool.Parse(isDiscontinued);
            }
            catch { }
            try
            {
                WSI.Product.Highlight = bool.Parse(isHighlight);
            }
            catch { }
            try
            {
                WSI.Product.IsNew = bool.Parse(isNew);
            }
            catch { }
            try
            {
                WSI.Product.IsEmpty = bool.Parse(isEmpty);
            }
            catch { }
            try { WSI.Product.CostPrice = double.Parse(costPrice); } catch { }
            //try { WSI.Product.DefaultDiscount = double.Parse(defaultDiscount); } catch { }
            WSI.ActionBy = User.Identity.Name;
            WSI = BL.callSingleBL(WSI);
            if (WSI.WsiError == "")
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
                    TranslationWSI.Translation.OwnerId = WSI.Product.ProductId;
                    TranslationWSI.Translation.LanguageId = languageId;
                    TranslationWSI.Translation.Name = Request.Params["name" + languageId];
                    TranslationWSI.Translation.ShortDescription = Request.Params["shortDescription" + languageId];
                    TranslationWSI.Translation.Description = NMCryptography.base64Decode(Request.Params["description" + languageId]);
                    TranslationWSI.Translation.Warranty = Request.Params["warranty" + languageId]; 
                    TranslationWSI = TranslationBL.callSingleBL(TranslationWSI);
                }
                if (!string.IsNullOrEmpty(price))
                {
                    result = SaveProductPrice(WSI.Product.ProductId, double.Parse(price)).Data.ToString();
                }
                if (images != "")
                {
                    try
                    {
                        NMImagesBL ImageBL = new NMImagesBL();
                        NMImagesWSI ImageWSI;
                        string[] tempImages = Regex.Split(images, "ROWS");
                        string[] image;
                        foreach (string item in tempImages)
                        {
                            image = Regex.Split(item, "COLS");
                            ImageWSI = new NMImagesWSI();
                            ImageWSI.Mode = "SAV_OBJ";
                            ImageWSI.Name = image[0].ToString();
                            ImageWSI.Description = image[1].ToString();
                            ImageWSI.IsDefault = image[2].ToString();
                            ImageWSI.Owner = WSI.Product.ProductId;
                            ImageWSI.TypeId = NMConstant.FileTypes.Images;
                            ImageWSI.CreatedBy = User.Identity.Name;                            
                            ImageWSI = ImageBL.callSingleBL(ImageWSI);
                            result = ImageWSI.WsiError;
                        }
                    }
                    catch
                    {
                        result = "Không lưu được hình ảnh.";
                    }
                }
            }
            else
            {
                result = WSI.WsiError;
            }

            return Json(new { id = WSI.Product.ProductId, error = result });
        }

        public JsonResult DeleteProduct(String id)
        {
            NMProductsWSI WSI = new NMProductsWSI();
            NMProductsBL BL = new NMProductsBL();
            WSI.Mode = "DEL_OBJ";
            WSI.Product = new Products();
            WSI.Product.ProductId = id;
            WSI = BL.callSingleBL(WSI);
            return Json(WSI.WsiError);
        }

        public JsonResult GetProductUnits(String productCategoryId)
        {
            NMProductUnitsWSI PUNTWSI = new NMProductUnitsWSI();
            NMProductUnitsBL PUNIBL = new NMProductUnitsBL();
            PUNTWSI.Mode = "SRC_OBJ";
            PUNTWSI.ProductCategoryId = productCategoryId;
            List<NMProductUnitsWSI> PUNTWSIs = PUNIBL.callListBL(PUNTWSI);            
            return Json(PUNTWSIs);
        }

        public JsonResult SaveProductPrice(String productId, double price)
        {
            NMStocksBL stBL = new NMStocksBL();
            NMStocksWSI stWsi = new NMStocksWSI();
            stWsi.Mode = "SRC_OBJ";
            List<NMStocksWSI> stList = stBL.callListBL(stWsi);

            NMPricesForSalesInvoiceWSI PriceWSI = new NMPricesForSalesInvoiceWSI();
            NMPricesForSalesInvoiceBL PriceBL = new NMPricesForSalesInvoiceBL();
            PriceWSI.Mode = "SAV_OBJ";
            PriceWSI.PriceForSale = new PricesForSalesInvoice();
            PriceWSI.PriceForSale.ProductId = productId;
            PriceWSI.PriceForSale.DateOfPrice = DateTime.Today;
            PriceWSI.PriceForSale.Price = price;
            PriceWSI.ActionBy = User.Identity.Name;

            foreach (NMStocksWSI itm in stList)
            {
                PriceWSI.PriceForSale.StockId = itm.Id;
                PriceWSI = PriceBL.callSingleBL(PriceWSI);
            }
            return Json(PriceWSI.WsiError);
        }

        public ActionResult PricesForSalesInvoiceHistory(String productId)
        {            
            String productName = "";
            
            NEXMI.NMProductsBL BL = new NEXMI.NMProductsBL();
            NEXMI.NMProductsWSI WSI = new NEXMI.NMProductsWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Product = new Products();
            WSI.Product.ProductId = productId;
            WSI = BL.callSingleBL(WSI);

            productId = WSI.Product.ProductId;
            productName = (WSI.Translation == null) ? "" : WSI.Translation.Name;
            NEXMI.NMPricesForSalesInvoiceWSI PriceWSI = new NEXMI.NMPricesForSalesInvoiceWSI();
            NEXMI.NMPricesForSalesInvoiceBL PriceBL = new NEXMI.NMPricesForSalesInvoiceBL();
            PriceWSI.Mode = "SRC_OBJ";
            PriceWSI.PriceForSale = new PricesForSalesInvoice();
            PriceWSI.PriceForSale.ProductId = WSI.Product.ProductId;
            ViewData["PriceWSIs"] = PriceBL.callListBL(PriceWSI);
            ViewData["productId"]=productId;
            ViewData["productName"] = productName;

            return PartialView();
        }

        public ActionResult GetProductInStocks(String productId)
        {
            if (!String.IsNullOrEmpty(productId))
            {
                NMStocksWSI swsi = new NMStocksWSI();
                NMStocksBL sbl = new NMStocksBL();
                swsi.Mode = "SRC_OBJ";
                List<NMStocksWSI> stock = sbl.callListBL(swsi);
                ViewData["stock"] = stock;
                NMProductInStocksBL ibl = new NMProductInStocksBL();
                NMProductInStocksWSI iwsi = new NMProductInStocksWSI();                
                iwsi.Mode = "SRC_OBJ";
                iwsi.PIS = new ProductInStocks();
                iwsi.PIS.ProductId = productId;
                ViewData["ProductInStocks"] = ibl.callListBL(iwsi);
            }
            return PartialView();
        }

        public ActionResult BoMs(string elementId, string productId, List<ProductBOMs> objs)
        {
            if (elementId == null) elementId = NMCommon.RandomString(5, true);
            ViewData["ElementId"] = elementId;
            ViewData["ProductId"] = productId;
            ViewData["Objs"] = objs;
            return PartialView();
        }

        public JsonResult SaveBoMLine(string parentId, string productId, string quantity, string lossRate, string description, string replacement,
            string validFrom, string validUntil, string windowId)
        {
            if (windowId == null) windowId = "";
            string error = "";
            try
            {
                ProductBOMs obj = new ProductBOMs();
                obj.ParentId = parentId;
                obj.ProductId = productId;
                obj.Quantity = int.Parse(quantity);
                obj.Description = description;
                //tỷ lệ hư hỏng
                try
                {
                    obj.LossRate = double.Parse(lossRate);
                }
                catch {
                    obj.LossRate = 0;
                }
                //thời điểm thay thế
                try
                {
                    obj.ReplacementTime = int.Parse(replacement);
                }
                catch
                {
                    obj.ReplacementTime = 0;
                }
                //
                try
                {
                    obj.ValidFrom = DateTime.Parse(validFrom);
                }
                catch { }
                try
                {
                    obj.ValidUntil = DateTime.Parse(validUntil);
                }
                catch { }
                if (!string.IsNullOrEmpty(parentId))
                {
                    error = NMProductsBL.SaveBoM(obj);
                }
                else
                {
                    List<ProductBOMs> objs = new List<ProductBOMs>();
                    if (Session["BoMs" + windowId] != null) objs = (List<ProductBOMs>)Session["BoMs" + windowId];
                    int index = objs.FindIndex(i => i.ParentId == parentId && i.ProductId == productId);
                    if (index == -1) objs.Add(obj);
                    else objs[index] = obj;
                    Session["BoMs" + windowId] = objs;
                }
            }
            catch (Exception ex)
            {
                error = "Không thực hiện được!\n" + ex.Message;
            }
            return Json(error);
        }

        public JsonResult RemoveBoMLine(string parentId, string productId)
        {
            string error = "";
            if (!string.IsNullOrEmpty(parentId))
            {
                ProductBOMs obj = new ProductBOMs();
                obj.ParentId = parentId;
                obj.ProductId = productId;
                error = NMProductsBL.DeleteBoM(obj);
            }
            return Json(error);
        }

        #endregion

        #region Areas
        public ActionResult Areas(string windowId)
        {
            NEXMI.NMAreasBL BL = new NEXMI.NMAreasBL();
            NEXMI.NMAreasWSI WSI = new NEXMI.NMAreasWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.Area = new Areas();
            WSI.Area.ParentId = "root";
            ViewData["WSIs"] = BL.callListBL(WSI);
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public ActionResult AreaForm(String id, string parentElement, string windowId)
        {
            NMAreasBL BL = new NMAreasBL();
            NMAreasWSI WSI = new NMAreasWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Area = new Areas();
            WSI.Area.Id = id;
            WSI = BL.callSingleBL(WSI);
            //NMAreasWSI WSI = GlobalValues.GetAreas(id);
            ViewData["Id"] = WSI.Area.Id;
            ViewData["Name"] = WSI.Area.Name;
            ViewData["ParentId"] = "";
            ViewData["ParentName"] = "";
            if (WSI.Parent != null)
            {
                ViewData["ParentId"] = WSI.Parent.Id;
                ViewData["ParentName"] = WSI.Parent.Name;
            }
            ViewData["ParentElement"] = parentElement;
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public JsonResult GetAreas(string parentId)
        {
            NMAreasBL BL = new NMAreasBL();
            NMAreasWSI WSI = new NMAreasWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.Area = new Areas();
            WSI.Area.ParentId = parentId;
            List<NMAreasWSI> WSIs = BL.callListBL(WSI);
            return Json(WSIs);
        }

        public JsonResult SaveOrUpdateArea(String id, string name, string parentId, string zipCode)
        {
            NMAreasWSI WSI = new NMAreasWSI();
            NMAreasBL BL = new NMAreasBL();
            WSI.Area = new Areas();
            WSI.Area.Id = id;

            if (!string.IsNullOrEmpty(id))
            {
                WSI.Mode = "SEL_OBJ";
                WSI = BL.callSingleBL(WSI);
            }

            WSI.Mode = "SAV_OBJ";
            WSI.WsiError = "";
            WSI.Area.Name = name;
            if (!string.IsNullOrEmpty(parentId)) WSI.Area.ParentId = parentId;
            WSI.Area.ZipCode = zipCode;
            WSI.ActionBy = User.Identity.Name;
            WSI = BL.callSingleBL(WSI);
            //GlobalValues.SetAreas();
            return Json(new { error = WSI.WsiError, id = WSI.Area.Id });
        }

        public JsonResult DeleteArea(String id)
        {
            NMAreasWSI WSI = new NMAreasWSI();
            NMAreasBL BL = new NMAreasBL();
            WSI.Mode = "DEL_OBJ";
            WSI.Area = new Areas();
            WSI.Area.Id = id;
            WSI = BL.callSingleBL(WSI);
            return Json(WSI.WsiError);
        }
        #endregion

        #region Units
        public ActionResult Units()
        {
            NEXMI.NMProductUnitsBL BL = new NEXMI.NMProductUnitsBL();
            NEXMI.NMProductUnitsWSI WSI = new NEXMI.NMProductUnitsWSI();
            WSI.Mode = "SRC_OBJ";
            ViewData["WSIs"] = BL.callListBL(WSI);
            return PartialView();
        }

        public ActionResult UnitForm(String id, string windowId)
        {
            NMProductUnitsBL BL = new NMProductUnitsBL();
            NMProductUnitsWSI WSI = new NMProductUnitsWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Id = id;
            WSI = BL.callSingleBL(WSI);
            ViewData["Id"] = WSI.Id;
            ViewData["Name"] = WSI.Name;

            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public JsonResult SaveOrUpdateUnit(String id, string name)
        {
            NMProductUnitsBL BL = new NMProductUnitsBL();
            NMProductUnitsWSI WSI = new NMProductUnitsWSI();
            WSI.Id = id;

            if (!string.IsNullOrEmpty(id))
            {
                WSI.Mode = "SEL_OBJ";
                WSI = BL.callSingleBL(WSI);
            }

            WSI.Mode = "SAV_OBJ";
            WSI.Name = name;
            WSI = BL.callSingleBL(WSI);
            return Json(WSI.WsiError);
        }

        public JsonResult DeleteUnit(String id)
        {
            NMProductUnitsBL BL = new NMProductUnitsBL();
            NMProductUnitsWSI WSI = new NMProductUnitsWSI();
            WSI.Mode = "DEL_OBJ";
            WSI.Id = id;
            WSI = BL.callSingleBL(WSI);
            return Json(WSI.WsiError);
        }
        #endregion

        #region Shifts
        public ActionResult Shifts()
        {
            NEXMI.NMShiftsWSI ShiftWSI = new NEXMI.NMShiftsWSI();
            NEXMI.NMShiftsBL ShiftBL = new NEXMI.NMShiftsBL();
            ShiftWSI.Mode = "SRC_OBJ";
            ViewData["PCWSIs"] = ShiftBL.callListBL(ShiftWSI);            
            return PartialView();
        }
        public ActionResult ShiftForm(String id)
        {
            String name = "", start = "", finish = "", des = "";
            if (id != null)
            {                
                NEXMI.NMShiftsBL BL = new NEXMI.NMShiftsBL();
                NEXMI.NMShiftsWSI WSI = new NEXMI.NMShiftsWSI();
                WSI.Mode = "SEL_OBJ";
                WSI.Id = id;
                WSI = BL.callSingleBL(WSI);
                id = WSI.Id;
                name = WSI.Name;
                start = DateTime.Parse(WSI.Start).ToString("hh:mm");
                finish = DateTime.Parse(WSI.Finish).ToString("hh:mm");
                des = WSI.Description;
            } 
            ViewData["id"] = id;
            ViewData["name"]=name;
            ViewData["start"]=start;
            ViewData["finish"]=finish;
            ViewData["des"] = des;
            return PartialView();
        }
        public JsonResult SaveOrUpdateShift(String id, String name, String start, String finish, String des)
        {
            NMShiftsBL BL = new NMShiftsBL();
            NMShiftsWSI WSI = new NMShiftsWSI();
            WSI.Id = id;

            if (!string.IsNullOrEmpty(id))
            {
                WSI.Mode = "SEL_OBJ";
                WSI = BL.callSingleBL(WSI);
            }

            WSI.Mode = "SAV_OBJ";
            WSI.Name = name;
            WSI.Start = DateTime.Parse(start).ToString();
            WSI.Finish = DateTime.Parse(finish).ToString();
            WSI.Description = des;
            WSI = BL.callSingleBL(WSI);
            return Json(WSI.WsiError);
        }
        public JsonResult DeleteShift(String id)
        {
            NMShiftsBL BL = new NMShiftsBL();
            NMShiftsWSI WSI = new NMShiftsWSI();
            WSI.Mode = "DEL_OBJ";
            WSI.Id = id;
            WSI = BL.callSingleBL(WSI);
            return Json(WSI.WsiError);
        }
        #endregion

        #region Banks

        public ActionResult Banks()
        {
            NEXMI.NMBanksBL BL = new NEXMI.NMBanksBL();
            NEXMI.NMBanksWSI WSI = new NEXMI.NMBanksWSI();
            WSI.Mode = "SRC_OBJ";
            ViewData["WSIs"] = BL.callListBL(WSI);

            return PartialView();
        }

        public ActionResult BankForm(String id, string windowId)
        {
            NEXMI.NMBanksBL BL = new NEXMI.NMBanksBL();
            NEXMI.NMBanksWSI WSI = new NEXMI.NMBanksWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Bank.Id = id;
            WSI = BL.callSingleBL(WSI);
            ViewData["Id"] = WSI.Bank.Id;
            ViewData["Name"] = WSI.Bank.Name;
            ViewData["Code"] = WSI.Bank.Code;
            ViewData["Address"] = WSI.Bank.Address;

            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public JsonResult SaveOrUpdateBank(String id, string code, string name, string add)
        {
            NEXMI.NMBanksBL BL = new NEXMI.NMBanksBL();
            NEXMI.NMBanksWSI WSI = new NEXMI.NMBanksWSI();
            WSI.Bank.Id = id;

            if (!string.IsNullOrEmpty(id))
            {
                WSI.Mode = "SEL_OBJ";
                WSI = BL.callSingleBL(WSI);
            }

            WSI.Mode = "SAV_OBJ";
            WSI.Bank.Code = code;
            WSI.Bank.Name = name;
            WSI.Bank.Address = add;
            WSI = BL.callSingleBL(WSI);
            return Json(WSI.WsiError);
        }


        public JsonResult DeleteBank(String id)
        {
            NEXMI.NMBanksBL BL = new NEXMI.NMBanksBL();
            NEXMI.NMBanksWSI WSI = new NEXMI.NMBanksWSI();
            WSI.Mode = "DEL_OBJ";
            WSI.Bank.Id = id;
            WSI = BL.callSingleBL(WSI);
            return Json(WSI.WsiError);
        }
        #endregion

        public ActionResult SupplierOfProduct(string productId)
        {
            if (!string.IsNullOrEmpty(productId))
            {
                NMMonthlyGeneralJournalsBL BL = new NMMonthlyGeneralJournalsBL();
                NMMonthlyGeneralJournalsWSI WSI = new NMMonthlyGeneralJournalsWSI();
                WSI.Mode = "SRC_OBJ";
                WSI.MGJ.ProductId = productId;
                WSI.MGJ.AccountId = "1561";
                ViewData["WSIs"] = BL.callListBL(WSI);
            }
            else
            {
                ViewData["WSIs"] = new List<NMMonthlyGeneralJournalsWSI>();
            }

            //lấy danh sách khách hàng
            NMCustomersBL CBL = new NMCustomersBL();
            NMCustomersWSI CWSI = new NMCustomersWSI();
            CWSI.Mode = "SRC_OBJ";
            CWSI.Customer.GroupId = NMConstant.CustomerGroups.Supplier;
            ViewData["Suppliers"] = CBL.callListBL(CWSI);

            return PartialView();
        }

        public ActionResult CustomersBuyProduct(string productId)
        {
            if (!string.IsNullOrEmpty(productId))
            {
                NMMonthlyGeneralJournalsBL BL = new NMMonthlyGeneralJournalsBL();
                NMMonthlyGeneralJournalsWSI WSI = new NMMonthlyGeneralJournalsWSI();
                WSI.Mode = "SRC_OBJ";
                WSI.MGJ.ProductId = productId;
                WSI.MGJ.AccountId = "5111";
                ViewData["WSIs"] = BL.callListBL(WSI);
            }
            else
            {
                ViewData["WSIs"] = new List<NMMonthlyGeneralJournalsWSI>();
            }

            //lấy danh sách khách hàng
            NMCustomersBL CBL = new NMCustomersBL();
            NMCustomersWSI CWSI = new NMCustomersWSI();
            CWSI.Mode = "SRC_OBJ";
            CWSI.Customer.GroupId = NMConstant.CustomerGroups.Customer;
            ViewData["Customers"] = CBL.callListBL(CWSI);

            return PartialView();
        }

    }
}
