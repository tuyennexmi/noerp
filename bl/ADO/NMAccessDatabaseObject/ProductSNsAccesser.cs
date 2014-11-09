// ProductSNsAccesser.cs

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
    public class ProductSNsAccesser
    {
        ISession session;

        public ProductSNsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertProductSNs(ProductSNs productsX)
        {
            session.Merge(productsX);
        }

        public void UpdateProductSNs(ProductSNs productsX)
        {
            session.Update(productsX);
        }

        public void DeleteProductSNs(ProductSNs productsX)
        {
            session.Delete(productsX);
        }

        public IList<ProductSNs> GetAllProductSNs(Boolean evict)
        {
            IQuery query = session.CreateQuery("select p from ProductSNs as p");
            IList<ProductSNs> list = query.List<ProductSNs>();
            if (evict)
            {
                foreach (ProductSNs s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<ProductSNs> GetAllProductSNsByReferenceId(string id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select p from ProductSNs as p where p.ImportId = :x or p.ExportId = :x");
            query.SetString("x", id);
            IList<ProductSNs> list = query.List<ProductSNs>();
            if (evict)
            {
                foreach (ProductSNs s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public ProductSNs GetAllProductSNsByID(String productId, string serialNumber, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from ProductSNs as c where c.ProductId = :x and c.SerialNumber = :y");
            query.SetString("x", productId);
            query.SetString("y", serialNumber);
            ProductSNs s = (ProductSNs)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public ProductSNs GetAllProductSNsByID(String productId, String stockId, string serialNumber, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from ProductSNs as c where c.ProductId = :x and c.StockId = :s and c.SerialNumber = :y");
            query.SetString("x", productId);
            query.SetString("s", stockId);
            query.SetString("y", serialNumber);
            ProductSNs s = (ProductSNs)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<ProductSNs> GetProductSNsByQuery(IQuery query, Boolean evict)
        {
            IList<ProductSNs> list = query.List<ProductSNs>();
            if (evict)
            {
                foreach (ProductSNs s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<ProductSNs> GetProductSNsByQuery(IQuery query, int pageSize, int pageNum, Boolean evict)
        {
            int startIndex = pageSize * (pageNum + 1 - 1);
            IList<ProductSNs> list = query.SetFirstResult(startIndex).SetMaxResults(pageSize).List<ProductSNs>();
            if (evict)
            {
                foreach (ProductSNs s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }
    }
}
