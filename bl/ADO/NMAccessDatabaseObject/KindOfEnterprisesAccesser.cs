// KindOfEnterprisesAccesser.cs

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
    public class KindOfEnterprisesAccesser
    {
        ISession session;

        public KindOfEnterprisesAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertKindofindustry(KindOfEnterprises kindofenterprisesX)
        {
            session.Merge(kindofenterprisesX);
        }

        public void UpdateKindofindustry(KindOfEnterprises kindofenterprisesX)
        {
            session.Update(kindofenterprisesX);
        }

        public void DeleteKindofindustry(KindOfEnterprises kindofenterprisesX)
        {
            session.Delete(kindofenterprisesX);
        }

        public IList<KindOfEnterprises> GetAllKindofindustry(Boolean evict)
        {
            IQuery query = session.CreateQuery("select k from KindOfEnterprises as k");
            IList<KindOfEnterprises> list = query.List<KindOfEnterprises>();
            if (evict)
            {
                foreach (KindOfEnterprises s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public KindOfEnterprises GetAllKindofindustryByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from KindOfEnterprises as c where c.Id = :x");
            query.SetString("x", id);
            KindOfEnterprises s = (KindOfEnterprises)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<KindOfEnterprises> GetKindofindustryByQuery(IQuery query, Boolean evict)
        {
            IList<KindOfEnterprises> list = query.List<KindOfEnterprises>();
            if (evict)
            {
                foreach (KindOfEnterprises s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }


    }
}
