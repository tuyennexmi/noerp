// ProductInStocksAccesser.cs

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
    public class ProductInStocksAccesser
    {
        ISession session;

        public ProductInStocksAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertProductinstocks(ProductInStocks productinstocksX)
        {
            session.Merge(productinstocksX);
        }

        public void UpdateProductinstocks(ProductInStocks productinstocksX)
        {
            session.Update(productinstocksX);
        }

        public void DeleteProductinstocks(ProductInStocks productinstocksX)
        {
            session.Delete(productinstocksX);
        }

        public IList<ProductInStocks> GetAllProductinstocks(Boolean evict)
        {
            IQuery query = session.CreateQuery("select p from ProductInStocks as p");
            IList<ProductInStocks> list = query.List<ProductInStocks>();
            if (evict)
            {
                foreach (ProductInStocks s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public ProductInStocks GetAllProductinstocksByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from ProductInStocks as c where c.OrdinalNumber = :x");
            query.SetString("x", id);
            ProductInStocks s = (ProductInStocks)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public ProductInStocks GetAllProductinstocksByStockIdAndProductId(String stockId, String productId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from ProductInStocks as c where c.StockId = :x and c.ProductId = :y");
            query.SetString("x", stockId);
            query.SetString("y", productId);
            ProductInStocks s = (ProductInStocks)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<ProductInStocks> GetProductinstocksByQuery(IQuery query, Boolean evict)
        {
            IList<ProductInStocks> list = query.List<ProductInStocks>();
            if (evict)
            {
                foreach (ProductInStocks s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<ProductInStocks> GetProductinstocksByQuery(IQuery query, int pageSize, int pageNum, Boolean evict)
        {
            int startIndex = pageSize * (pageNum + 1 - 1);
            IList<ProductInStocks> list = query.SetFirstResult(startIndex).SetMaxResults(pageSize).List<ProductInStocks>();
            if (evict)
            {
                foreach (ProductInStocks s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }
    }
}
