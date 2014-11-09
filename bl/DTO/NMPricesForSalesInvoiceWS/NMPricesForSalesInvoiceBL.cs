// NMPricesForSalesInvoiceBL.cs

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
using System.Collections;
using System.Linq;
using System.Text;
using NHibernate;
using System.Security.Cryptography;
using System.Data.SqlTypes;

namespace NEXMI
{
    public class NMPricesForSalesInvoiceBL
    {
        private readonly  ISession Session = SessionFactory.GetNewSession();

        public NMPricesForSalesInvoiceBL()
        {

        }

        public NMPricesForSalesInvoiceWSI callSingleBL(NMPricesForSalesInvoiceWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SAV_OBJ":
                    return SaveObject(wsi);
                case "SEL_OBJ":
                    return SelectObject(wsi);
                case "DEL_OBJ":
                    return DeleteObject(wsi);
                case "SEL_MAX":
                    return SelectMax(wsi);
                case "SEL_CUR":
                    return SelectCurrent(wsi);
                default:
                    return wsi;
            }
        }

        public List<NMPricesForSalesInvoiceWSI> callListBL(NMPricesForSalesInvoiceWSI wsi)
        {
            List<NMPricesForSalesInvoiceWSI> ListWSI = new List<NMPricesForSalesInvoiceWSI>();
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    ListWSI = SearchObject(wsi);
                    break;
            }
            return ListWSI;
        }

        public NMPricesForSalesInvoiceWSI SelectObject(NMPricesForSalesInvoiceWSI wsi)
        {
            PricesForSalesInvoiceAccesser Accesser = new PricesForSalesInvoiceAccesser(Session);
            PricesForSalesInvoice obj;
            obj = Accesser.GetAllPricesforsalesinvoiceByID(wsi.PriceForSale.Id.ToString(), true);
            if (obj != null)
            {
                wsi.PriceForSale = obj;
                wsi.WsiError = "";
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public NMPricesForSalesInvoiceWSI SelectMax(NMPricesForSalesInvoiceWSI wsi)
        {
            PricesForSalesInvoiceAccesser Accesser = new PricesForSalesInvoiceAccesser(Session);
            PricesForSalesInvoice obj;
            obj = Accesser.GetAllPricesforsalesinvoiceCloset(wsi.PriceForSale.ProductId, wsi.PriceForSale.StockId, false);
            if (obj != null)
            {
                wsi.PriceForSale = obj;
                wsi.WsiError = "";
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public NMPricesForSalesInvoiceWSI SelectCurrent(NMPricesForSalesInvoiceWSI wsi)
        {
            PricesForSalesInvoiceAccesser Accesser = new PricesForSalesInvoiceAccesser(Session);
            PricesForSalesInvoice obj;
            obj = Accesser.GetAllPricesforsalesinvoiceCurrent(wsi.PriceForSale.ProductId, false);
            if (obj != null)
            {
                wsi.PriceForSale = obj;
                wsi.WsiError = "";
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public NMPricesForSalesInvoiceWSI SaveObject(NMPricesForSalesInvoiceWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                PricesForSalesInvoiceAccesser Accesser = new PricesForSalesInvoiceAccesser(Session);
                PricesForSalesInvoice obj = Accesser.GetAllPricesforsalesinvoiceCloset(wsi.PriceForSale.ProductId, wsi.PriceForSale.StockId, true);
                if (obj != null)
                {
                    if (obj.Price != wsi.PriceForSale.Price)
                    {
                        obj = Accesser.GetAllPricesforsalesinvoiceByDateAndProductId(wsi.PriceForSale.DateOfPrice.ToString(), wsi.PriceForSale.ProductId, wsi.PriceForSale.StockId, true);
                        if (obj != null)
                        {
                            wsi.PriceForSale.Id = obj.Id;
                            wsi.PriceForSale.CreatedDate = obj.CreatedDate;
                            wsi.PriceForSale.CreatedBy = obj.CreatedBy;
                            Accesser.UpdatePricesforsalesinvoice(wsi.PriceForSale);
                        }
                        else
                        {
                            wsi.PriceForSale.CreatedDate = DateTime.Now;
                            wsi.PriceForSale.CreatedBy = wsi.ActionBy;
                            Accesser.InsertPricesforsalesinvoice(wsi.PriceForSale);
                        }
                    }
                }
                else
                {
                    wsi.PriceForSale.CreatedDate = DateTime.Now;
                    wsi.PriceForSale.CreatedBy = wsi.ActionBy;
                    Accesser.InsertPricesforsalesinvoice(wsi.PriceForSale);
                }
                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message + "\n" + ex.InnerException;
            }
            return wsi;
        }

        public NMPricesForSalesInvoiceWSI DeleteObject(NMPricesForSalesInvoiceWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                PricesForSalesInvoiceAccesser Accesser = new PricesForSalesInvoiceAccesser(Session);
                PricesForSalesInvoice obj = Accesser.GetAllPricesforsalesinvoiceByID(wsi.PriceForSale.Id.ToString(), true);
                if (obj != null)
                {
                    Accesser.DeletePricesforsalesinvoice(obj);
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

        public List<NMPricesForSalesInvoiceWSI> SearchObject(NMPricesForSalesInvoiceWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (wsi.PriceForSale != null)
            {
                if (!string.IsNullOrEmpty(wsi.PriceForSale.ProductId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.ProductId = :ProductId";
                    ListCriteria.Add("ProductId", wsi.PriceForSale.ProductId);
                }
                if (!string.IsNullOrEmpty(wsi.PriceForSale.CreatedBy))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.CreatedBy = :CreatedBy";
                    ListCriteria.Add("CreatedBy", wsi.PriceForSale.CreatedBy);
                }
            }
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
            string strCmdCounter = "SELECT COUNT(O.Id) FROM PricesForSalesInvoice AS O" + strCriteria;
            IQuery queryCounter = Session.CreateSQLQuery(strCmdCounter);
            if (wsi.SortField != null && wsi.SortOrder != "")
            {
                strCriteria += " ORDER BY O." + wsi.SortField + " " + wsi.SortOrder;
            }
            else
            {
                strCriteria += " ORDER BY O.Id DESC";
            }
            string strCmd = "SELECT O FROM PricesForSalesInvoice AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                queryCounter.SetParameter(Item.Key, Item.Value);
                query.SetParameter(Item.Key, Item.Value);
            }
            List<NMPricesForSalesInvoiceWSI> ListWSI = new List<NMPricesForSalesInvoiceWSI>();
            PricesForSalesInvoiceAccesser Accesser = new PricesForSalesInvoiceAccesser(Session);
            IList<PricesForSalesInvoice> objs;
            int totalRows = NMCommon.Counter(queryCounter);
            if (wsi.PageSize != 0)
            {
                objs = Accesser.GetPricesforsalesinvoiceByQuery(query, wsi.PageSize, wsi.PageNum, false);
            }
            else
            {
                objs = Accesser.GetPricesforsalesinvoiceByQuery(query, false);
            }
            foreach (PricesForSalesInvoice obj in objs)
            {
                wsi = new NMPricesForSalesInvoiceWSI();
                wsi.PriceForSale = obj;
                ListWSI.Add(wsi);
            }
            return ListWSI;
        }
    }
}
