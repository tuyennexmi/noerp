// LocationsAccesser.cs

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
    public class LocationsAccesser
    {
        ISession session;

        public LocationsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertLocations(Locations LocationsX)
        {
            session.Merge(LocationsX);
        }

        public void UpdateLocations(Locations LocationsX)
        {
            session.Update(LocationsX);
        }

        public void DeleteLocations(Locations LocationsX)
        {
            session.Delete(LocationsX);
        }

        public IList<Locations> GetAllLocations(Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Locations as c");
            IList<Locations> list = query.List<Locations>();
            if (evict)
            {
                foreach (Locations s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public Locations GetAllLocationsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Locations as c where c.Id = :x");
            query.SetString("x", id);
            Locations s = (Locations)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<Locations> GetLocationsByQuery(IQuery query, Boolean evict)
        {
            IList<Locations> list = query.List<Locations>();
            if (evict)
            {
                foreach (Locations s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }


    }
}
