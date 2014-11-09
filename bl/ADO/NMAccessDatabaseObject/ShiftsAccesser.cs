// ShiftsAccesser.cs

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

namespace NEXMI
{
    public class ShiftsAccesser
    {
        ISession session;

        public ShiftsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertShifts(Shifts shiftsX)
        {
            session.Merge(shiftsX);
        }

        public void UpdateShifts(Shifts shiftsX)
        {
            session.Update(shiftsX);
        }

        public void DeleteShifts(Shifts shiftsX)
        {
            session.Delete(shiftsX);
        }

        public IList<Shifts> GetAllShifts(Boolean evict)
        {
            IQuery query = session.CreateQuery("select f from Shifts as f order by f.Id");
            IList<Shifts> list = query.List<Shifts>();
            if (evict)
            {
                foreach (Shifts s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public Shifts GetAllShiftsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Shifts as c where c.Id = :x");
            query.SetString("x", id);
            Shifts s = (Shifts)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<Shifts> GetShiftsByQuery(IQuery query, Boolean evict)
        {
            IList<Shifts> list = query.List<Shifts>();
            if (evict)
            {
                foreach (Shifts s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }
    }
}
