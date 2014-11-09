// SettingsAccesser.cs

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
    public class SettingsAccesser
    {
        ISession session;

        public SettingsAccesser()
        {
            this.session = SessionFactory.GetNewSession();
        }

        public SettingsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertSettings(Settings typesX)
        {
            session.Merge(typesX);
        }

        public void UpdateSettings(Settings typesX)
        {
            session.Update(typesX);
        }

        public void DeleteSettings(Settings typesX)
        {
            session.Delete(typesX);
        }

        public IList<Settings> GetAllSettings(Boolean evict)
        {
            IQuery query = session.CreateQuery("select r from Settings as r");
            IList<Settings> list = query.List<Settings>();
            if (evict)
            {
                foreach (Settings s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public Settings GetAllSettingsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Settings as c where c.Id = :x");
            query.SetString("x", id);
            Settings s = (Settings)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<Settings> GetSettingsByQuery(IQuery query, Boolean evict)
        {
            IList<Settings> list = query.List<Settings>();
            if (evict)
            {
                foreach (Settings s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }


    }
}
