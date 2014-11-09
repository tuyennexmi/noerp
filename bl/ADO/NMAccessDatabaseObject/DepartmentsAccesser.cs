// DepartmentsAccesser.cs

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
    public class DepartmentsAccesser
    {
        ISession session;

        public DepartmentsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertDepartments(Departments departmentsX)
        {
            session.Merge(departmentsX);
        }

        public void UpdateDepartments(Departments departmentsX)
        {
            session.Update(departmentsX);
        }

        public void DeleteDepartments(Departments departmentsX)
        {
            session.Delete(departmentsX);
        }

        public IList<Departments> GetAllDepartments(Boolean evict)
        {
            IQuery query = session.CreateQuery("select d from Departments as d");
            IList<Departments> list = query.List<Departments>();
            if (evict)
            {
                foreach (Departments s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public Departments GetAllDepartmentsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Departments as c where c.Id = :x");
            query.SetString("x", id);
            Departments s = (Departments)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<Departments> GetDepartmentsByQuery(IQuery query, Boolean evict)
        {
            IList<Departments> list = query.List<Departments>();
            if (evict)
            {
                foreach (Departments s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }


    }
}
