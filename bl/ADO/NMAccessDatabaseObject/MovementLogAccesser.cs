// MovementLogAccesser.cs

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
    public class MovementLogAccesser
    {
        ISession session;

        public MovementLogAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertMovementLog(MovementLog movementLogX)
        {
            session.Merge(movementLogX);
        }

        public void UpdateMovementLog(MovementLog movementLogX)
        {
            session.Update(movementLogX);
        }

        public void DeleteMovementLog(MovementLog movementLogX)
        {
            session.Delete(movementLogX);
        }

        public IList<MovementLog> GetAllMovementLog(Boolean evict)
        {
            IQuery query = session.CreateQuery("select e from MovementLog as e");
            IList<MovementLog> list = query.List<MovementLog>();
            if (evict)
            {
                foreach (MovementLog s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public MovementLog GetAllMovementLogByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from MovementLog as c where c.Id = :x");
            query.SetString("x", id);
            MovementLog s = (MovementLog)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<MovementLog> GetAllMovementLogByDocument(String documentId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select e from MovementLog as e where e.DocumentId = :x");
            query.SetString("x", documentId);
            IList<MovementLog> list = query.List<MovementLog>();
            if (evict)
            {
                foreach (MovementLog s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public MovementLog GetAllMovementLogCloset(String currencyId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from MovementLog as c where c.Id = :x and c.Time = (select MAX(Time) from MovementLog as c1 where c1.Id = :x)");
            query.SetString("x", currencyId);
            MovementLog s = (MovementLog)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<MovementLog> GetMovementLogByQuery(IQuery query, Boolean evict)
        {
            IList<MovementLog> list = query.List<MovementLog>();
            if (evict)
            {
                foreach (MovementLog s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }


    }
}
