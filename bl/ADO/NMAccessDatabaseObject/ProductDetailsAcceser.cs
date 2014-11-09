// ProductDetailsAcceser.cs

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
    public class ProductDetailsAcceser
    {
        ISession session;

        public ProductDetailsAcceser(ISession session)
        {
            this.session = session;
        }

        public void InsertProductdetails(ProductDetails productdetailsX)
        {
            session.Merge(productdetailsX);
        }

        public void UpdateProductdetails(ProductDetails productdetailsX)
        {
            session.Update(productdetailsX);
        }

        public void DeleteProductdetails(ProductDetails productdetailsX)
        {
            session.Delete(productdetailsX);
        }

        public IList<ProductDetails> GetAllProductdetails(Boolean evict)
        {
            IQuery query = session.CreateQuery("select p from ProductDetails as p");
            IList<ProductDetails> list = query.List<ProductDetails>();
            if (evict)
            {
                foreach (ProductDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public ProductDetails GetAllProductdetailsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from ProductDetails as c where c.Id = :x");
            query.SetString("x", id);
            ProductDetails s = (ProductDetails)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<ProductDetails> GetProductdetailsByQuery(IQuery query, Boolean evict)
        {
            IList<ProductDetails> list = query.List<ProductDetails>();
            if (evict)
            {
                foreach (ProductDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }


    }
}
