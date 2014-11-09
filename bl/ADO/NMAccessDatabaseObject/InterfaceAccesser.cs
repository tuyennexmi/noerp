// InterfaceAccesser.cs

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
    public class InterfaceAccesser
    {
        ISession session;

        public InterfaceAccesser()
        {
            this.session = SessionFactory.GetNewSession();
        }

        public InterfaceAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertInterface(Interface InterfaceX)
        {
            session.Merge(InterfaceX);
        }

        public void UpdateInterface(Interface InterfaceX)
        {
            session.Update(InterfaceX);
        }

        public void DeleteInterface(Interface InterfaceX)
        {
            session.Delete(InterfaceX);
        }

        public IList<Interface> GetAllInterface(Boolean evict)
        {
            IQuery query = session.CreateQuery("select r from Interface as r");
            IList<Interface> list = query.List<Interface>();
            if (evict)
            {
                foreach (Interface s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public Interface GetAllInterfaceByID(String Id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Interface as c where c.Id = :y");            
            query.SetString("y", Id);
            Interface s = (Interface)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<Interface> GetInterfaceByQuery(IQuery query, Boolean evict)
        {
            IList<Interface> list = query.List<Interface>();
            if (evict)
            {
                foreach (Interface s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }


    }
}
