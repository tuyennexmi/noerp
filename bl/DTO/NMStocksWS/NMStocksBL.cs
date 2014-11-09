// NMStocksBL.cs

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
using System.Security.Cryptography;
using System.Data.SqlTypes;

namespace NEXMI
{
    public class NMStocksBL
    {
        private readonly  ISession Session = SessionFactory.GetNewSession();
        public NMStocksBL()
        {

        }

        public NMStocksWSI callSingleBL(NMStocksWSI wsi)
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

        public List<NMStocksWSI> callListBL(NMStocksWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMStocksWSI>();
            }
        }

        public NMStocksWSI SelectObject(NMStocksWSI wsi)
        {
            StocksAccesser Accesser = new StocksAccesser(Session);
            TypesAccesser TypesAccesser = new TypesAccesser(Session);
            TranslationsAccesser TranslationAccesser = new TranslationsAccesser(Session);
            Stocks obj;
            obj = Accesser.GetAllStocksByID(wsi.Id, false);
            if (obj != null)
            {
                wsi.Id = obj.Id;
                wsi.Name = obj.Name;
                wsi.Address = obj.Address;
                wsi.Logo = obj.Logo;
                wsi.Telephone = obj.Telephone;
                wsi.Highlight = obj.Highlight.ToString().ToLower();
                wsi.CreatedDate = obj.CreatedDate.ToString();
                wsi.CreatedBy = obj.CreatedBy;
                Types StockType = TypesAccesser.GetAllTypesByID(obj.StockType, true);
                if (StockType != null)
                {
                    wsi.StockType = StockType.Id;
                    wsi.TypeName = StockType.Name;
                }
                wsi.Translation = TranslationAccesser.GetAllTranslationsByID(wsi.LanguageId, obj.Id, true);
                if (wsi.Translation == null) wsi.Translation = new Translations();
                wsi.WsiError = "";
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public NMStocksWSI SaveObject(NMStocksWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                StocksAccesser Accesser = new StocksAccesser(Session);
                Stocks obj = Accesser.GetAllStocksByID(wsi.Id, true);
                if (obj != null)
                {
                    obj.Name = wsi.Name;
                    obj.Address = wsi.Address;
                    obj.Telephone = wsi.Telephone;
                    obj.Logo = wsi.Logo;
                    obj.StockType = wsi.StockType;
                    obj.Highlight = Boolean.Parse(wsi.Highlight);
                    Accesser.UpdateStocks(obj);
                }
                else
                {
                    obj = new Stocks();
                    obj.Id = AutomaticValueAccesser.AutoGenerateId("Stocks");
                    obj.Name = wsi.Name;
                    obj.Address = wsi.Address;
                    obj.StockType = wsi.StockType;
                    obj.Highlight = Boolean.Parse(wsi.Highlight);
                    obj.CreatedDate = DateTime.Now;
                    obj.CreatedBy = wsi.CreatedBy;
                    Accesser.InsertStocks(obj);
                }
                //if (!string.IsNullOrEmpty(wsi.SortField))
                
                InsertProductToStock(obj);
                
                tx.Commit();
                
                wsi.Id = obj.Id;
            }
            catch (Exception ex)
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return wsi;
        }

        public NMStocksWSI DeleteObject(NMStocksWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                StocksAccesser Accesser = new StocksAccesser(Session);
                Stocks obj = Accesser.GetAllStocksByID(wsi.Id, true);
                if (obj != null)
                {
                    Accesser.DeleteStocks(obj);
                    try
                    {
                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        wsi.WsiError = "Không thể xóa.";
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

        public List<NMStocksWSI> SearchObject(NMStocksWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (!string.IsNullOrEmpty(wsi.CreatedBy))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.CreatedBy = :CreatedBy";
                ListCriteria.Add("CreatedBy", wsi.CreatedBy);
            }
            if (!string.IsNullOrEmpty(wsi.Highlight))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.Highlight = :Highlight";
                ListCriteria.Add("Highlight", bool.Parse(wsi.Highlight));
            }
            if (!string.IsNullOrEmpty(wsi.Keyword))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " (select T.Name + ';' from Translations T where T.OwnerId = O.Id and T.LanguageId = '" + wsi.LanguageId + "') like :Keyword or (select T.Address + ';' from Translations T where T.OwnerId = O.Id and T.LanguageId = '" + wsi.LanguageId + "') like :Keyword";
            }
            string strCmdCounter = "SELECT COUNT(O.Id) FROM Stocks AS O" + strCriteria;
            IQuery queryCounter = Session.CreateSQLQuery(strCmdCounter);
            if (wsi.SortField != null && wsi.SortOrder != "")
            {
                strCriteria += " ORDER BY O." + wsi.SortField + " " + wsi.SortOrder;
            }
            else
            {
                strCriteria += " ORDER BY O.Name ASC";
            }
            String strCmd = "SELECT O FROM Stocks AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                query.SetParameter(Item.Key, Item.Value);
                queryCounter.SetParameter(Item.Key, Item.Value);
            }
            if (!string.IsNullOrEmpty(wsi.Keyword))
            {
                query.SetString("Keyword", "%" + wsi.Keyword + "%");
                queryCounter.SetString("Keyword", "%" + wsi.Keyword + "%");
            }
            List<NMStocksWSI> ListWSI = new List<NMStocksWSI>();
            StocksAccesser Accesser = new StocksAccesser(Session);
            TypesAccesser TypesAccesser = new TypesAccesser(Session);
            TranslationsAccesser TranslationAccesser = new TranslationsAccesser(Session);
            IList<Stocks> objs;
            int totalRows = NMCommon.Counter(queryCounter);
            if (wsi.PageSize != 0)
            {
                objs = Accesser.GetStocksByQuery(query, wsi.PageSize, wsi.PageNum, false);
            }
            else
            {
                objs = Accesser.GetStocksByQuery(query, false);
            }
            if (wsi.Limit != 0)
            {
                objs = objs.Take(wsi.Limit).ToList();
            }
            NMStocksWSI ItemWSI;
            foreach (Stocks obj in objs)
            {
                ItemWSI = new NMStocksWSI();
                ItemWSI.Id = obj.Id;
                ItemWSI.Name = obj.Name;
                ItemWSI.Address = obj.Address;
                ItemWSI.Logo = obj.Logo;
                ItemWSI.Telephone = obj.Telephone;
                ItemWSI.Highlight = obj.Highlight.ToString().ToLower();
                ItemWSI.CreatedDate = obj.CreatedDate.ToString();
                ItemWSI.CreatedBy = obj.CreatedBy;
                Types StockType = TypesAccesser.GetAllTypesByID(obj.StockType, true);
                if (StockType != null)
                {
                    ItemWSI.StockType = StockType.Id;
                    ItemWSI.TypeName = StockType.Name;
                }
                ItemWSI.Translation = TranslationAccesser.GetAllTranslationsByID(wsi.LanguageId, obj.Id, true);
                if (ItemWSI.Translation == null) ItemWSI.Translation = new Translations();
                ListWSI.Add(ItemWSI);
            }
            return ListWSI;
        }

        public void InsertProductToStock(Stocks obj)
        {
            ProductsAccesser ProductAccesser = new ProductsAccesser(Session);
            IList<Products> Products = ProductAccesser.GetAllProducts(true);
            ProductInStocksAccesser PISAccesser = new ProductInStocksAccesser(Session);
            ProductInStocks PIS;
            foreach (Products Item in Products)
            {
                PIS = PISAccesser.GetAllProductinstocksByStockIdAndProductId(obj.Id, Item.ProductId, true);
                if (PIS == null)
                {
                    PIS = new ProductInStocks();
                    PIS.StockId = obj.Id;
                    PIS.ProductId = Item.ProductId;
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
    }
}
