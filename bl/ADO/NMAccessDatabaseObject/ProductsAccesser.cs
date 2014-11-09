// ProductsAccesser.cs

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
    public class ProductsAccesser
    {
        ISession session;

        public ProductsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertProducts(Products productsX)
        {
            session.Merge(productsX);
        }

        public void UpdateProducts(Products productsX)
        {
            session.Update(productsX);
        }

        public void DeleteProducts(Products productsX)
        {
            session.Delete(productsX);
        }

        public IList<Products> GetAllProducts(Boolean evict)
        {
            IQuery query = session.CreateQuery("select p from Products as p");
            IList<Products> list = query.List<Products>();
            if (evict)
            {
                foreach (Products s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public Products GetAllProductsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Products as c where c.ProductId = :x");
            query.SetString("x", id);
            Products s = (Products)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public int Counter(IQuery query)
        {
            int count = 0;
            try
            {
                count = (int)query.UniqueResult();
            }
            catch { }
            return count;
        }

        public IList<Products> GetProductsByQuery(IQuery query, Boolean evict)
        {
            IList<Products> list = query.List<Products>();
            if (evict)
            {
                foreach (Products s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<Products> GetProductsByQuery(IQuery query, int pageSize, int pageNum, Boolean evict)
        {
            int startIndex = pageSize * (pageNum + 1 - 1);
            IList<Products> list = query.SetFirstResult(startIndex).SetMaxResults(pageSize).List<Products>();
            if (evict)
            {
                foreach (Products s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }
    }
}
