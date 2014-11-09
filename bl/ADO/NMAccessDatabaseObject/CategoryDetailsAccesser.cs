// CategoryDetailsAccesser.cs

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
    public class CategoryDetailsAccesser
    {
        ISession session;

        public CategoryDetailsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertProductCategoryDetails(CategoryDetails ProductCategoryDetailsX)
        {
            session.Merge(ProductCategoryDetailsX);
        }

        public void UpdateProductCategoryDetails(CategoryDetails ProductCategoryDetailsX)
        {
            session.Update(ProductCategoryDetailsX);
        }

        public void DeleteProductCategoryDetails(CategoryDetails ProductCategoryDetailsX)
        {
            session.Delete(ProductCategoryDetailsX);
        }

        public IList<CategoryDetails> GetAllProductCategoryDetails(Boolean evict)
        {
            IQuery query = session.CreateQuery("select from ProductCategoryDetails");
            IList<CategoryDetails> list = query.List<CategoryDetails>();
            if (evict)
            {
                foreach (CategoryDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public CategoryDetails GetAllProductCategoryDetailsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from ProductCategoryDetails as c where c.Id = :x");
            query.SetString("x", id);
            CategoryDetails s = (CategoryDetails)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<CategoryDetails> GetProductCategoryDetailsByQuery(IQuery query, Boolean evict)
        {
            IList<CategoryDetails> list = query.List<CategoryDetails>();
            if (evict)
            {
                foreach (CategoryDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }


    }
}
