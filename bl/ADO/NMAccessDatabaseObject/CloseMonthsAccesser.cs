// CloseMonthsAccesser.cs

// * http://nexmi.com
// * NoErp Project - Nexmi Open ERP
// * Copyright (C) 2012, Nguy�?n Quang Tuy�?n (tuyen.nq@nexmi.com), AUTHOR.txt (http://nexmi.com/about)
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
    public class CloseMonthsAccesser
    {
        ISession session;

        public CloseMonthsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertCloseMonths(CloseMonths CloseMonthsX)
        {
            session.Merge(CloseMonthsX);
        }

        public void UpdateCloseMonths(CloseMonths CloseMonthsX)
        {
            session.Update(CloseMonthsX);
        }

        public void DeleteCloseMonths(CloseMonths CloseMonthsX)
        {
            session.Delete(CloseMonthsX);
        }

        public IList<CloseMonths> GetAllCloseMonths(Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from CloseMonths as c");
            IList<CloseMonths> list = query.List<CloseMonths>();
            if (evict)
            {
                foreach (CloseMonths s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public CloseMonths GetAllCloseMonthsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from CloseMonths as c where c.CloseMonth = :x");
            query.SetString("x", id);
            CloseMonths s = (CloseMonths)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<CloseMonths> GetCloseMonthsByQuery(IQuery query, Boolean evict)
        {
            IList<CloseMonths> list = query.List<CloseMonths>();
            if (evict)
            {
                foreach (CloseMonths s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }


    }
}
