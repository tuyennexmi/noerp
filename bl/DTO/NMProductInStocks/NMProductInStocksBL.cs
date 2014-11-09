// NMProductInStocksBL.cs

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

namespace NEXMI
{
    public class NMProductInStocksBL
    {
        private readonly ISession Session = SessionFactory.GetNewSession();
        public NMProductInStocksBL()
        {

        }

        public NMProductInStocksWSI callSingleBL(NMProductInStocksWSI wsi)
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

        public List<NMProductInStocksWSI> callListBL(NMProductInStocksWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMProductInStocksWSI>();
            }
        }

        public NMProductInStocksWSI SelectObject(NMProductInStocksWSI wsi)
        {
            ProductInStocksAccesser Accesser = new ProductInStocksAccesser(Session);
            ProductInStocks obj = Accesser.GetAllProductinstocksByStockIdAndProductId(wsi.PIS.StockId, wsi.PIS.ProductId, true);
            if (obj != null)
            {
                MonthlyGeneralJournalsAccesser mgjAccesser = new MonthlyGeneralJournalsAccesser(Session);
                //obj.BeginQuantity = NMCommon.ICGetBeginAmount(obj.StockId, obj.ProductId);
                //obj.ImportQuantity = NMCommon.ICGetImportAmount(obj.StockId, obj.ProductId);
                //obj.ExportQuantity = NMCommon.ICGetExportAmount(obj.StockId, obj.ProductId);
                obj.CostPrice = mgjAccesser.ICExportPriceCalculate(obj.StockId, obj.ProductId, "1561");

                obj.BeginQuantity = mgjAccesser.ICGetOpenBeginAmount(obj.StockId, obj.ProductId);
                obj.ImportQuantity = mgjAccesser.ICGetOpenImportAmount(obj.StockId, obj.ProductId);
                obj.ExportQuantity = mgjAccesser.ICGetOpenExportAmount(obj.StockId, obj.ProductId);

                wsi.PIS = obj;
                NMProductsBL ProductBL = new NMProductsBL();
                NMProductsWSI ProductWSI = new NMProductsWSI();
                ProductWSI.Mode = "SEL_OBJ";
                ProductWSI.Product = new Products();
                ProductWSI.Product.ProductId = obj.ProductId;
                wsi.ProductWSI = ProductBL.callSingleBL(ProductWSI);
                NMStocksBL StockBL = new NMStocksBL();
                NMStocksWSI StockWSI = new NMStocksWSI();
                StockWSI.Mode = "SEL_OBJ";
                StockWSI.Id = obj.StockId;
                wsi.StockWSI = StockBL.callSingleBL(StockWSI);
                wsi.WsiError = "";
            }
            else
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            return wsi;
        }

        public NMProductInStocksWSI SaveObject(NMProductInStocksWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                ProductInStocksAccesser Accesser = new ProductInStocksAccesser(Session);
                ProductInStocks obj = Accesser.GetAllProductinstocksByStockIdAndProductId(wsi.PIS.StockId, wsi.PIS.ProductId, true);
                if (obj != null)
                {
                    wsi.PIS.OrdinalNumber = obj.OrdinalNumber;
                    wsi.PIS.CreatedDate = DateTime.Now;
                    wsi.PIS.CreatedBy = wsi.ActionBy;
                    Accesser.UpdateProductinstocks(wsi.PIS);
                }
                else
                {

                    wsi.PIS.CreatedDate = DateTime.Now;
                    wsi.PIS.CreatedBy = wsi.ActionBy;
                    Accesser.InsertProductinstocks(wsi.PIS);
                }
                tx.Commit();
            }
            catch
            {
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.";
                tx.Rollback();
            }
            return wsi;
        }

        public NMProductInStocksWSI DeleteObject(NMProductInStocksWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            ProductInStocksAccesser Accesser = new ProductInStocksAccesser(Session);
            ProductInStocks obj = Accesser.GetAllProductinstocksByStockIdAndProductId(wsi.PIS.StockId, wsi.PIS.ProductId, true);
            if (obj != null)
            {
                Accesser.DeleteProductinstocks(obj);
                try
                {
                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.";
                }
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public List<NMProductInStocksWSI> SearchObject(NMProductInStocksWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (wsi.PIS != null)
            {
                if (!string.IsNullOrEmpty(wsi.PIS.StockId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.StockId = :StockId";
                    ListCriteria.Add("StockId", wsi.PIS.StockId);
                }
                if (!string.IsNullOrEmpty(wsi.PIS.ProductId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.ProductId = :ProductId";
                    ListCriteria.Add("ProductId", wsi.PIS.ProductId);
                }
            }
            if (!string.IsNullOrEmpty(wsi.CategoryId))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.ProductId in (SELECT P.ProductId FROM Products AS P WHERE P.CategoryId = :CategoryId)";
                ListCriteria.Add("CategoryId", wsi.CategoryId);
            }
            if (!string.IsNullOrEmpty(wsi.ActionBy))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.CreatedBy = :CreatedBy";
                ListCriteria.Add("CreatedBy", wsi.ActionBy);
            }
            if (!string.IsNullOrEmpty(wsi.Keyword))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " (O.ProductId in (SELECT P.ProductId FROM Products AS P WHERE P.ProductId LIKE :Keyword OR P.ProductCode LIKE :Keyword OR (select T.Name + ';' from Translations T where T.OwnerId = P.ProductId and T.LanguageId = '" + wsi.LanguageId + "') like :Keyword) OR O.StockId in (SELECT S.Id FROM Stocks AS S WHERE S.Id LIKE :Keyword OR (select T.Name + ';' from Translations T where T.OwnerId = S.Id and T.LanguageId = '" + wsi.LanguageId + "') like :Keyword))";
            }
            string strCmdCounter = "SELECT COUNT(O.OrdinalNumber) FROM ProductInStocks AS O" + strCriteria.Replace("NM", "");
            IQuery queryCounter = Session.CreateSQLQuery(strCmdCounter);
            if (wsi.SortField != null && wsi.SortOrder != "")
            {
                strCriteria += " ORDER BY O." + wsi.SortField + " " + wsi.SortOrder;
            }
            else
            {
                strCriteria += " ORDER BY O.StockId, O.ProductId ASC";
            }
            String strCmd = "SELECT O FROM ProductInStocks AS O" + strCriteria;
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

            // dieu kien ngay thang 
            int month = DateTime.Today.Month;
            int year = DateTime.Today.Year;
            if (month == 0)
            {
                month = 12;
                year -= 1;
            }
            if (string.IsNullOrEmpty(wsi.Filter.FromDate))
                wsi.Filter.FromDate = new DateTime(year, month, 01).ToString();

            if (string.IsNullOrEmpty(wsi.Filter.ToDate))
                wsi.Filter.ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)).ToString();

            List<NMProductInStocksWSI> ListWSI = new List<NMProductInStocksWSI>();
            ProductInStocksAccesser Accesser = new ProductInStocksAccesser(Session);
            ProductsAccesser ProductAccesser = new ProductsAccesser(Session);
            StocksAccesser StockAccesser = new StocksAccesser(Session);
            IList<ProductInStocks> objs;
            
            if (wsi.PageSize != 0)
            {
                objs = Accesser.GetProductinstocksByQuery(query, wsi.PageSize, wsi.PageNum, false);
            }
            else
            {
                objs = Accesser.GetProductinstocksByQuery(query, false);
            }

            MonthlyGeneralJournalsAccesser mgjAccesser = new MonthlyGeneralJournalsAccesser(Session);
            NMProductInStocksWSI objWSI;

            foreach (ProductInStocks obj in objs)
            {
                obj.CostPrice = mgjAccesser.ICExportPriceCalculate(obj.StockId, obj.ProductId, NMCommon.GetProductType(obj.ProductId));

                obj.BeginQuantity = mgjAccesser.ICGetBeginAmountInPeriod(obj.StockId, obj.ProductId, wsi.Filter.FromDate, wsi.Filter.ToDate);
                obj.ImportQuantity = mgjAccesser.ICGetImportAmountInPeriod(obj.StockId, obj.ProductId, wsi.Filter.FromDate, wsi.Filter.ToDate);
                obj.ExportQuantity = mgjAccesser.ICGetExportAmountInPeriod(obj.StockId, obj.ProductId, wsi.Filter.FromDate, wsi.Filter.ToDate);

                objWSI = new NMProductInStocksWSI();
                objWSI.PIS = obj;

                ListWSI.Add(objWSI);
            }
            if(ListWSI.Count > 0)
                ListWSI[0].TotalRows = NMCommon.Counter(queryCounter);

            return ListWSI;
        }
    }
}
