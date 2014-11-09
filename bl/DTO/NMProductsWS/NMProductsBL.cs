
// NMProductsBL.cs

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
using System.Data;

namespace NEXMI
{
    public class NMProductsBL
    {
        private readonly ISession Session = SessionFactory.GetNewSession();

        public NMProductsBL()
        {
            
        }

        public NMProductsWSI callSingleBL(NMProductsWSI wsi)
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

        public List<NMProductsWSI> callListBL(NMProductsWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                case "SRC_BY_IMP":
                    return SearchByImport(wsi);
                case "SRC_BY_EXP":
                    return SearchByExport(wsi);
                default:
                    return new List<NMProductsWSI>();
            }
        }

        private NMProductsWSI SelectObject(NMProductsWSI wsi)
        {
            try
            {
                ProductsAccesser Acceser = new ProductsAccesser(Session);
                CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
                PricesForSalesInvoiceAccesser PriceAccesser = new PricesForSalesInvoiceAccesser(Session);
                ProductUnitsAccesser UnitAccesser = new ProductUnitsAccesser(Session);
                CategoriesAccesser CategoryAccesser = new CategoriesAccesser(Session);
                ImagesAccesser ImageAccesser = new ImagesAccesser(Session);
                TranslationsAccesser TranslationAccesser = new TranslationsAccesser(Session);
                LocationsAccesser locationsAccesser = new NEXMI.LocationsAccesser(Session);
                ProductPortsAccesser productPortsAccesser = new NEXMI.ProductPortsAccesser(Session);

                Products obj;
                obj = Acceser.GetAllProductsByID(wsi.Product.ProductId, false);
                if (obj != null)
                {
                    wsi.Product = obj;
                    wsi.Manufacture = CustomerAccesser.GetAllCustomersByID(obj.ManufactureId, true);
                    wsi.PriceForSales = PriceAccesser.GetAllPricesforsalesinvoiceCloset(obj.ProductId, wsi.StockId, true);
                    wsi.CostPrice = PriceAccesser.GetAllPricesforExportCloset(obj.ProductId, wsi.StockId, true);
                    wsi.Unit = UnitAccesser.GetAllProductUnitsByID(obj.ProductUnit, true);
                    if (NMCommon.GetSetting("MANAGE_IMAGES"))
                        wsi.Images = ImageAccesser.GetAllImagesByOwner(obj.ProductId, true);
                    if (NMCommon.GetSetting("MANAGE_BOM"))
                        wsi.BoMs = obj.ProductBOMs.Cast<ProductBOMs>().ToList();
                    wsi.Translation = TranslationAccesser.GetAllTranslationsByID(wsi.LanguageId, obj.ProductId, true);
                    if (wsi.Translation == null) wsi.Translation = new Translations();
                    NMCategoriesBL BL = new NMCategoriesBL();
                    wsi.CategoryWSI = new NMCategoriesWSI();
                    wsi.CategoryWSI.Mode = "SEL_OBJ";
                    wsi.CategoryWSI.Category = new Categories();
                    wsi.CategoryWSI.Category.Id = obj.CategoryId;
                    wsi.CategoryWSI.LanguageId = wsi.LanguageId;
                    wsi.CategoryWSI = BL.callSingleBL(wsi.CategoryWSI);

                    if (NMCommon.GetSetting("MANAGE_LOCATION"))
                        wsi.Location = locationsAccesser.GetAllLocationsByID(wsi.Product.LocationId, true);
                    if (NMCommon.GetSetting("MANAGE_PORTS"))
                        wsi.ProductPorts = productPortsAccesser.GetAllProductPortsByProductId(obj.ProductId, true).ToList();
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

        private NMProductsWSI SaveObject(NMProductsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {   
                ProductsAccesser Accesser = new ProductsAccesser(Session);
                ProductPortsAccesser productPortsAccesser = new ProductPortsAccesser(Session);
                IList<ProductPorts> oldPorts = null;
                Products obj = Accesser.GetAllProductsByID(wsi.Product.ProductId, false);
                if (obj != null)
                {
                    if (NMCommon.GetSetting("MANAGE_PORTS"))
                    {
                        oldPorts = productPortsAccesser.GetAllProductPortsByProductId(wsi.Product.ProductId, true);
                    }
                    Session.Evict(obj);
                    wsi.Product.CreatedBy = obj.CreatedBy;
                    wsi.Product.CreatedDate = obj.CreatedDate;
                    wsi.Product.ModifiedBy = wsi.ActionBy;
                    wsi.Product.ModifiedDate = DateTime.Now;
                    String rs = wsi.CompareTo(obj);
                    if (rs != "")
                        NMMessagesBL.SaveMessage(Session, wsi.Product.ProductId, "cập nhật thông tin", rs, wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
                    Accesser.UpdateProducts(wsi.Product);
                }
                else
                {
                    AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);

                    wsi.Product.ProductId = AutomaticValueAccesser.AutoGenerateId("Products");
                    wsi.Product.CreatedBy = wsi.ActionBy;
                    wsi.Product.CreatedDate = DateTime.Now;
                    wsi.Product.ModifiedBy = wsi.ActionBy;
                    wsi.Product.ModifiedDate = DateTime.Now;
                    Accesser.InsertProducts(wsi.Product);
                    NMMessagesBL.SaveMessage(Session, wsi.Product.ProductId, "khởi tạo tài liệu", "", wsi.ActionBy, null, null, NMConstant.MessageTypes.SysLog);
                }

                #region Ports
                if (NMCommon.GetSetting("MANAGE_PORTS"))
                {
                    if (wsi.ProductPorts != null)
                    {
                        ProductPorts port;
                        foreach (var Item in wsi.ProductPorts)
                        {
                            port = productPortsAccesser.GetAllProductPortsByID(Item.Id.ToString(), true);
                            if (port != null)
                            {
                                Item.ProductId = wsi.Product.ProductId;

                                Item.CreatedBy = port.CreatedBy;
                                Item.CreatedDate = port.CreatedDate;
                                Item.ModifiedBy = wsi.ActionBy;
                                Item.ModifiedDate = DateTime.Now;

                                productPortsAccesser.UpdateProductPorts(Item);
                            }
                            else
                            {
                                Item.ProductId = wsi.Product.ProductId;

                                Item.CreatedBy = wsi.ActionBy;
                                Item.CreatedDate = DateTime.Now;
                                Item.ModifiedBy = wsi.ActionBy;
                                Item.ModifiedDate = DateTime.Now;

                                productPortsAccesser.InsertProductPorts(Item);
                            }
                        }
                        if (oldPorts != null)
                        {
                            bool flag = true;
                            foreach (var Old in oldPorts)
                            {
                                flag = true;
                                foreach (var New in wsi.ProductPorts)
                                {
                                    if (Old.Id == New.Id)
                                    {
                                        flag = false;
                                        break;
                                    }
                                }
                                if (flag)
                                {
                                    productPortsAccesser.DeleteProductPorts(Old);
                                }
                            }
                        }
                    }
                }
                #endregion

                InsertProductInstock(wsi.Product);
                
                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return wsi;
        }

        private NMProductsWSI DeleteObject(NMProductsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {    
                ProductsAccesser Accesser = new ProductsAccesser(Session);
                Products obj = Accesser.GetAllProductsByID(wsi.Product.ProductId, true);
                if (obj != null)
                {
                    Accesser.DeleteProducts(obj);
                    try
                    {
                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        wsi.WsiError = "Không xóa được sản phẩm này.\n" + ex.Message + "\n" + ex.InnerException;
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

        private List<NMProductsWSI> SearchObject(NMProductsWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (wsi.Product != null)
            {
                if (!string.IsNullOrEmpty(wsi.Product.CategoryId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.CategoryId in (SELECT PC.Id FROM Categories PC WHERE PC.Id = :CategoryId OR PC.ParentId = :CategoryId)";
                    ListCriteria.Add("CategoryId", wsi.Product.CategoryId);
                }
                if (!string.IsNullOrEmpty(wsi.Product.ManufactureId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.ManufactureId = :ManufactureId";
                    ListCriteria.Add("ManufactureId", wsi.Product.ManufactureId);
                }
                if (!string.IsNullOrEmpty(wsi.Product.TypeId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.TypeId = :TypeId";
                    ListCriteria.Add("TypeId", wsi.Product.TypeId);
                }
                if (!string.IsNullOrEmpty(wsi.Product.BarCode))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.BarCode = :BarCode";
                    ListCriteria.Add("BarCode", wsi.Product.BarCode);
                }
                if (wsi.Product.Discontinued != null)
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.Discontinued = :Discontinued";
                    ListCriteria.Add("Discontinued", wsi.Product.Discontinued.Value);
                }
                if (wsi.Product.Highlight != null)
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.Highlight = :Highlight";
                    ListCriteria.Add("Highlight", wsi.Product.Highlight.Value);
                }
                if (wsi.Product.IsNew != null)
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.IsNew = :IsNew";
                    ListCriteria.Add("IsNew", wsi.Product.IsNew.Value);
                }
                if (wsi.Product.IsEmpty != null)
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.IsEmpty = :IsEmpty";
                    ListCriteria.Add("IsEmpty", wsi.Product.IsEmpty.Value);
                }
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
                strCriteria += " (O.ProductId LIKE :Keyword OR O.ProductCode LIKE :Keyword OR (select T.Name + ';' from Translations T where T.OwnerId = O.ProductId and T.LanguageId = '" + wsi.LanguageId + "') like :Keyword)";
                ListCriteria.Add("Keyword", "%" + wsi.Keyword + "%");
            }
            string strCmdCounter = "SELECT COUNT(O.ProductId) FROM Products AS O" + strCriteria;
            IQuery queryCounter = Session.CreateSQLQuery(strCmdCounter);
            string temp = "";
            if (wsi.SortField != null && wsi.SortOrder != "")
            {
                if (wsi.SortField == "Name")
                {
                    temp = ", Translations T";
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.ProductId = T.OwnerId and T.LanguageId = '" + wsi.LanguageId + "' ORDER BY T.Name " + wsi.SortOrder + "";
                }
                else
                {
                    strCriteria += " ORDER BY O." + wsi.SortField + " " + wsi.SortOrder;
                }
            }
            else
            {
                strCriteria += " ORDER BY O.CreatedDate DESC";
            }
            String strCmd = "SELECT O FROM Products AS O" + temp + " " + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                queryCounter.SetParameter(Item.Key, Item.Value);
                query.SetParameter(Item.Key, Item.Value);
            }
            List<NMProductsWSI> ListWSI = new List<NMProductsWSI>();
            ProductsAccesser Accesser = new ProductsAccesser(Session);
            CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
            PricesForSalesInvoiceAccesser PriceAccesser = new PricesForSalesInvoiceAccesser(Session);
            ProductUnitsAccesser UnitAccesser = new ProductUnitsAccesser(Session);
            CategoriesAccesser CategoryAccesser = new CategoriesAccesser(Session);
            ImagesAccesser ImageAccesser = new ImagesAccesser(Session);
            TranslationsAccesser TranslationAccesser = new TranslationsAccesser(Session);
            LocationsAccesser locationsAccesser = new NEXMI.LocationsAccesser(Session);
            ProductPortsAccesser productPortsAccesser = new NEXMI.ProductPortsAccesser(Session);

            IList<Products> objs;
            int totalRows = NMCommon.Counter(queryCounter);
            if (wsi.PageSize != 0)
            {
                objs = Accesser.GetProductsByQuery(query, wsi.PageSize, wsi.PageNum, false);
            }
            else
            {
                objs = Accesser.GetProductsByQuery(query, false);
            }
            if (wsi.Limit != 0)
            {
                objs = objs.Take(wsi.Limit).ToList();
            }
            NMProductsWSI ItemWSI;
            NMCategoriesBL BL = new NMCategoriesBL();

            bool IsManageManufacture = NMCommon.GetSetting("MANAGE_MANUFACTURE");
            bool IsManagePrice = NMCommon.GetSetting("MANAGE_PRICEs");
            bool IsManageImage = NMCommon.GetSetting("MANAGE_IMAGES");
            bool IsManageBOM = NMCommon.GetSetting("MANAGE_BOM");
            bool IsGetTranslate = NMCommon.GetSetting("GET_TRANSLATION_IN_SRC");
            bool IsGetCate = NMCommon.GetSetting("GET_CATEGORY_IN_SRC");
            bool IsManageLocation = NMCommon.GetSetting("MANAGE_LOCATION");
            bool isManagePort = NMCommon.GetSetting("MANAGE_PORTS");

            foreach (Products obj in objs)
            {
                ItemWSI = new NMProductsWSI();
                ItemWSI.Product = obj;
                ItemWSI.Unit = UnitAccesser.GetAllProductUnitsByID(obj.ProductUnit, true);

                if (IsManageManufacture)
                    ItemWSI.Manufacture = CustomerAccesser.GetAllCustomersByID(obj.ManufactureId, true);
                if (IsManagePrice)
                {
                    ItemWSI.PriceForSales = PriceAccesser.GetAllPricesforsalesinvoiceCloset(obj.ProductId, wsi.StockId, true);
                    ItemWSI.CostPrice = PriceAccesser.GetAllPricesforExportCloset(obj.ProductId, wsi.StockId, true);
                }
                if (IsManageImage)
                    ItemWSI.Images = ImageAccesser.GetAllImagesByOwner(obj.ProductId, true);
                if (IsManageBOM)
                    ItemWSI.BoMs = obj.ProductBOMs.Cast<ProductBOMs>().ToList();
                if (IsGetTranslate)
                {
                    ItemWSI.Translation = TranslationAccesser.GetAllTranslationsByID(wsi.LanguageId, obj.ProductId, true);
                    if (ItemWSI.Translation == null) ItemWSI.Translation = new Translations();
                }
                if (IsGetCate)
                {   
                    ItemWSI.CategoryWSI = new NMCategoriesWSI();
                    ItemWSI.CategoryWSI.Mode = "SEL_OBJ";
                    ItemWSI.CategoryWSI.Category = new Categories();
                    ItemWSI.CategoryWSI.Category.Id = obj.CategoryId;
                    ItemWSI.CategoryWSI.LanguageId = wsi.LanguageId;
                    ItemWSI.CategoryWSI = BL.callSingleBL(ItemWSI.CategoryWSI);
                }
                if (IsManageLocation)
                    ItemWSI.Location = locationsAccesser.GetAllLocationsByID(ItemWSI.Product.LocationId, true);
                if (isManagePort)
                    ItemWSI.ProductPorts = productPortsAccesser.GetAllProductPortsByProductId(obj.ProductId, true).ToList();

                ItemWSI.WsiError = "";
                ListWSI.Add(ItemWSI);
            }

            if(ListWSI.Count > 0)
                ListWSI[0].TotalRows = totalRows;

            return ListWSI;
        }

        private void InsertProductInstock(Products obj)
        {
            StocksAccesser StockAccesser = new StocksAccesser(Session);
            IList<Stocks> Stocks = StockAccesser.GetAllStocks(true);
            ProductInStocksAccesser PISAccesser = new ProductInStocksAccesser(Session);
            ProductInStocks PIS;
            foreach (Stocks Item in Stocks)
            {
                PIS = PISAccesser.GetAllProductinstocksByStockIdAndProductId(Item.Id, obj.ProductId, true);
                if (PIS == null)
                {
                    PIS = new ProductInStocks();
                    PIS.StockId = Item.Id;
                    PIS.ProductId = obj.ProductId;
                    PIS.BeginQuantity = 0;
                    PIS.ImportQuantity = 0;
                    PIS.ExportQuantity = 0;
                    PIS.BadProductInStock = 0;
                    PIS.CreatedDate = DateTime.Now;
                    PIS.CreatedBy = obj.CreatedBy;
                    PIS.ModifiedDate = DateTime.Now;
                    PIS.ModifiedBy = obj.CreatedBy;
                    PISAccesser.InsertProductinstocks(PIS);
                }
            }
        }

        public static string SaveBoM(ProductBOMs obj)
        {
            string error = "";
            ISession Session = SessionFactory.GetNewSession();
            ITransaction tx = Session.BeginTransaction();
            try
            {
                ProductBOMsAccesser Accesser = new ProductBOMsAccesser(Session);
                ProductBOMs temp = Accesser.GetAllProductBOMsByID(obj.ParentId, obj.ProductId, true);
                if (temp != null)
                {
                    obj.Id = temp.Id;
                    Accesser.UpdateProductBOMs(obj);
                }
                else
                {
                    Accesser.InsertProductBOMs(obj);
                }
                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                error = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message;// + "\n" + ex.InnerException;
            }
            return error;
        }

        public static string DeleteBoM(ProductBOMs obj)
        {
            string error = "";
            ISession Session = SessionFactory.GetNewSession();
            ITransaction tx = Session.BeginTransaction();
            try
            {
                ProductBOMsAccesser Accesser = new ProductBOMsAccesser(Session);
                ProductBOMs temp = Accesser.GetAllProductBOMsByID(obj.ParentId, obj.ProductId, true);
                if (temp != null)
                {
                    Accesser.DeleteProductBOMs(temp);
                }
                else
                {
                    error = "Dữ liệu không tồn tại hoặc đã bị xóa.";
                }
                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                error = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return error;
        }

        private List<NMProductsWSI> SearchByExport(NMProductsWSI wsi)
        {
            String strCmd = "SELECT O FROM Products AS O WHERE O.ProductId in (select P.ProductId from Products P, ExportDetails D, Exports M where M.ExportId = D.ExportId and D.ProductId = P.ProductId and M.ExportId = :keyword)";
            IQuery query = Session.CreateQuery(strCmd);
            query.SetString("keyword", wsi.Keyword);
            List<NMProductsWSI> ListWSI = new List<NMProductsWSI>();
            ProductsAccesser Accesser = new ProductsAccesser(Session);
            CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
            PricesForSalesInvoiceAccesser PriceAccesser = new PricesForSalesInvoiceAccesser(Session);
            ProductUnitsAccesser UnitAccesser = new ProductUnitsAccesser(Session);
            CategoriesAccesser CategoryAccesser = new CategoriesAccesser(Session);
            ImagesAccesser ImageAccesser = new ImagesAccesser(Session);
            TranslationsAccesser TranslationAccesser = new TranslationsAccesser(Session);
            IList<Products> objs = Accesser.GetProductsByQuery(query, false);
            NMProductsWSI ItemWSI;
            foreach (Products obj in objs)
            {
                ItemWSI = new NMProductsWSI();
                ItemWSI.Product = obj;
                //ItemWSI.Customer = CustomerAccesser.GetAllCustomersByID(obj.ManufactureId, true);
                //ItemWSI.PriceForSales = PriceAccesser.GetAllPricesforsalesinvoiceCloset(obj.ProductId, true);
                ItemWSI.Unit = UnitAccesser.GetAllProductUnitsByID(obj.ProductUnit, true);
                //ItemWSI.Images = ImageAccesser.GetAllImagesByOwner(obj.ProductId, true);
                //ItemWSI.BoMs = obj.ProductBOMs.Cast<ProductBOMs>().ToList();
                ItemWSI.Translation = TranslationAccesser.GetAllTranslationsByID(wsi.LanguageId, obj.ProductId, true);
                if (ItemWSI.Translation == null) ItemWSI.Translation = new Translations();
                NMCategoriesBL BL = new NMCategoriesBL();
                ItemWSI.CategoryWSI = new NMCategoriesWSI();
                ItemWSI.CategoryWSI.Mode = "SEL_OBJ";
                ItemWSI.CategoryWSI.Category = new Categories();
                ItemWSI.CategoryWSI.Category.Id = obj.CategoryId;
                ItemWSI.CategoryWSI.LanguageId = wsi.LanguageId;
                ItemWSI.CategoryWSI = BL.callSingleBL(ItemWSI.CategoryWSI);
                ItemWSI.WsiError = "";
                ListWSI.Add(ItemWSI);
            }
            return ListWSI;
        }

        private List<NMProductsWSI> SearchByImport(NMProductsWSI wsi)
        {
            String strCmd = "SELECT O FROM Products AS O WHERE O.ProductId in (select P.ProductId from Products P, ImportDetails D, Imports M where M.ImportId = D.ImportId and D.ProductId = P.ProductId and M.ImportId = :keyword)";
            IQuery query = Session.CreateQuery(strCmd);
            query.SetString("keyword", wsi.Keyword);
            List<NMProductsWSI> ListWSI = new List<NMProductsWSI>();
            ProductsAccesser Accesser = new ProductsAccesser(Session);
            CustomersAccesser CustomerAccesser = new CustomersAccesser(Session);
            PricesForSalesInvoiceAccesser PriceAccesser = new PricesForSalesInvoiceAccesser(Session);
            ProductUnitsAccesser UnitAccesser = new ProductUnitsAccesser(Session);
            CategoriesAccesser CategoryAccesser = new CategoriesAccesser(Session);
            ImagesAccesser ImageAccesser = new ImagesAccesser(Session);
            TranslationsAccesser TranslationAccesser = new TranslationsAccesser(Session);
            IList<Products> objs = Accesser.GetProductsByQuery(query, false);
            NMProductsWSI ItemWSI;
            foreach (Products obj in objs)
            {
                ItemWSI = new NMProductsWSI();
                ItemWSI.Product = obj;
                //ItemWSI.Customer = CustomerAccesser.GetAllCustomersByID(obj.ManufactureId, true);
                //ItemWSI.PriceForSales = PriceAccesser.GetAllPricesforsalesinvoiceCloset(obj.ProductId, true);
                ItemWSI.Unit = UnitAccesser.GetAllProductUnitsByID(obj.ProductUnit, true);
                //ItemWSI.Images = ImageAccesser.GetAllImagesByOwner(obj.ProductId, true);
                //ItemWSI.BoMs = obj.ProductBOMs.Cast<ProductBOMs>().ToList();
                ItemWSI.Translation = TranslationAccesser.GetAllTranslationsByID(wsi.LanguageId, obj.ProductId, true);
                if (ItemWSI.Translation == null) ItemWSI.Translation = new Translations();
                NMCategoriesBL BL = new NMCategoriesBL();
                ItemWSI.CategoryWSI = new NMCategoriesWSI();
                ItemWSI.CategoryWSI.Mode = "SEL_OBJ";
                ItemWSI.CategoryWSI.Category = new Categories();
                ItemWSI.CategoryWSI.Category.Id = obj.CategoryId;
                ItemWSI.CategoryWSI.LanguageId = wsi.LanguageId;
                ItemWSI.CategoryWSI = BL.callSingleBL(ItemWSI.CategoryWSI);
                ItemWSI.WsiError = "";
                ListWSI.Add(ItemWSI);
            }
            return ListWSI;
        }
    }
}
