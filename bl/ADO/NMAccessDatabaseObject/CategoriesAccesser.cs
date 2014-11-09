// CategoriesAccesser.cs

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
    public class CategoriesAccesser
    {
        ISession session;

        public CategoriesAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertCategories(Categories CategoriesX)
        {
            session.Merge(CategoriesX);
        }

        public void UpdateCategories(Categories CategoriesX)
        {
            session.Update(CategoriesX);
        }

        public void DeleteCategories(Categories CategoriesX)
        {
            session.Delete(CategoriesX);
        }

        public IList<Categories> GetAllCategories(Boolean evict)
        {
            IQuery query = session.CreateQuery("select p from Categories as p");
            IList<Categories> list = query.List<Categories>();
            if (evict)
            {
                foreach (Categories s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public Categories GetAllCategoriesByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Categories as c where c.Id = :x");
            query.SetString("x", id);
            Categories s = (Categories)query.UniqueResult();
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

        public IList<Categories> GetCategoriesByQuery(IQuery query, Boolean evict)
        {
            IList<Categories> list = query.List<Categories>();
            if (evict)
            {
                foreach (Categories s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<Categories> GetCategoriesByQuery(IQuery query, int pageSize, int pageNum, Boolean evict)
        {
            int startIndex = pageSize * (pageNum + 1 - 1);
            IList<Categories> list = query.SetFirstResult(startIndex).SetMaxResults(pageSize).List<Categories>();
            if (evict)
            {
                foreach (Categories s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }
    }
}
