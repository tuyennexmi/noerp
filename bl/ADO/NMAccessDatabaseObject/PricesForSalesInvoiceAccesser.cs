// PricesForSalesInvoiceAccesser.cs

// * http://nexmi.com
// * NoErp Project - Nexmi Open ERP
// * Copyright (C) 2012, Nguyê?n Quang Tuyê?n (tuyen.nq@nexmi.com), AUTHOR.txt (http://nexmi.com/about)
// * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; 
//   either version 2 of the License, or (at your option) any later version. This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
//   without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details. 
//   You should have received a copy of the GNU General Public License along with this library; if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// *

using System.Collections.Generic;
using System.Text;
using System;
using NHibernate;
using NHibernate.Cfg;
using NEXMI;

namespace NEXMI
{
    public class PricesForSalesInvoiceAccesser
    {
        ISession session;

        public PricesForSalesInvoiceAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertPricesforsalesinvoice(PricesForSalesInvoice pricesforsalesinvoiceX)
        {
            session.Merge(pricesforsalesinvoiceX);
        }

        public void UpdatePricesforsalesinvoice(PricesForSalesInvoice pricesforsalesinvoiceX)
        {
            session.Update(pricesforsalesinvoiceX);
        }

        public void DeletePricesforsalesinvoice(PricesForSalesInvoice pricesforsalesinvoiceX)
        {
            session.Delete(pricesforsalesinvoiceX);
        }

        public IList<PricesForSalesInvoice> GetAllPricesforsalesinvoice(Boolean evict)
        {
            IQuery query = session.CreateQuery("select p from PricesForSalesInvoice as p");
            IList<PricesForSalesInvoice> list = query.List<PricesForSalesInvoice>();
            if (evict)
            {
                foreach (PricesForSalesInvoice s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public PricesForSalesInvoice GetAllPricesforsalesinvoiceByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from PricesForSalesInvoice as c where c.Id = :x");
            query.SetString("x", id);
            PricesForSalesInvoice s = (PricesForSalesInvoice)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public PricesForSalesInvoice GetAllPricesforsalesinvoiceByDateAndProductId(String dateOfPrice, String id, String stockId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from PricesForSalesInvoice as c where c.DateOfPrice = :x and c.ProductId = :y and c.StockId = :s ");
            query.SetString("x", dateOfPrice);
            query.SetString("y", id);
            query.SetString("s", stockId);
            PricesForSalesInvoice s = (PricesForSalesInvoice)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<PricesForSalesInvoice> GetAllPricesforsalesinvoiceCloset(String productId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from PricesForSalesInvoice as c where c.ProductId = :x and c.IsCost = :isc and c.CreatedDate = (select MAX(CreatedDate) from PricesForSalesInvoice as c1 where c1.ProductId = :x and c1.IsCost = :isc)");
            query.SetString("x", productId);            
            query.SetBoolean("isc", false);
            IList<PricesForSalesInvoice> list = query.List<PricesForSalesInvoice>();
            if (evict)
            {
                foreach (PricesForSalesInvoice s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public PricesForSalesInvoice GetAllPricesforsalesinvoiceCloset(String productId, String stockId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from PricesForSalesInvoice as c where c.StockId = :s and c.ProductId = :x and c.IsCost = :isc and c.CreatedDate = (select MAX(CreatedDate) from PricesForSalesInvoice as c1 where c1.StockId = :s and c1.ProductId = :x and c1.IsCost = :isc)");
            query.SetString("x", productId);
            query.SetString("s", stockId);
            query.SetBoolean("isc", false);
            PricesForSalesInvoice s = (PricesForSalesInvoice)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public PricesForSalesInvoice GetAllPricesforExportCloset(String productId, String stockId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from PricesForSalesInvoice as c where c.StockId = :s and c.ProductId = :x and c.IsCost = :isc and c.CreatedDate = (select MAX(CreatedDate) from PricesForSalesInvoice as c1 where c1.StockId = :s and c1.ProductId = :x and c1.IsCost = :isc)");
            query.SetString("x", productId);
            query.SetString("s", stockId);
            query.SetBoolean("isc", true);
            PricesForSalesInvoice s = (PricesForSalesInvoice)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public PricesForSalesInvoice GetAllPricesforsalesinvoiceCurrent(String productId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from PricesForSalesInvoice as c where c.ProductId = :x and c.PriceDiscontinued = 'true'").SetFirstResult(0);
            query.SetString("x", productId);
            PricesForSalesInvoice s = (PricesForSalesInvoice)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<PricesForSalesInvoice> GetPricesforsalesinvoiceByQuery(IQuery query, Boolean evict)
        {
            IList<PricesForSalesInvoice> list = query.List<PricesForSalesInvoice>();
            if (evict)
            {
                foreach (PricesForSalesInvoice s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<PricesForSalesInvoice> GetPricesforsalesinvoiceByQuery(IQuery query, int pageSize, int pageNum, Boolean evict)
        {
            int startIndex = pageSize * (pageNum + 1 - 1);
            IList<PricesForSalesInvoice> list = query.SetFirstResult(startIndex).SetMaxResults(pageSize).List<PricesForSalesInvoice>();
            if (evict)
            {
                foreach (PricesForSalesInvoice s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }
    }
}
