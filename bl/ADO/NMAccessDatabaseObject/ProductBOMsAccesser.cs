// ProductBOMsAccesser.cs

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
    public class ProductBOMsAccesser
    {
        ISession session;

        public ProductBOMsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertProductBOMs(ProductBOMs productsX)
        {
            session.Merge(productsX);
        }

        public void UpdateProductBOMs(ProductBOMs productsX)
        {
            session.Update(productsX);
        }

        public void DeleteProductBOMs(ProductBOMs productsX)
        {
            session.Delete(productsX);
        }

        public IList<ProductBOMs> GetAllProductBOMs(Boolean evict)
        {
            IQuery query = session.CreateQuery("select p from ProductBOMs as p");
            IList<ProductBOMs> list = query.List<ProductBOMs>();
            if (evict)
            {
                foreach (ProductBOMs s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public ProductBOMs GetAllProductBOMsByID(String parentId, string productId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from ProductBOMs as c where c.ParentId = :x and c.ProductId = :y");
            query.SetString("x", parentId);
            query.SetString("y", productId);
            ProductBOMs s = (ProductBOMs)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<ProductBOMs> GetProductBOMsByQuery(IQuery query, Boolean evict)
        {
            IList<ProductBOMs> list = query.List<ProductBOMs>();
            if (evict)
            {
                foreach (ProductBOMs s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<ProductBOMs> GetProductBOMsByQuery(IQuery query, int pageSize, int pageNum, Boolean evict)
        {
            int startIndex = pageSize * (pageNum + 1 - 1);
            IList<ProductBOMs> list = query.SetFirstResult(startIndex).SetMaxResults(pageSize).List<ProductBOMs>();
            if (evict)
            {
                foreach (ProductBOMs s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }
    }
}
