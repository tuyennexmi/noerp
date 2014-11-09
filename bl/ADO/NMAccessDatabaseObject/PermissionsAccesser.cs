// PermissionsAccesser.cs

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
    public class PermissionsAccesser
    {
        ISession session;

        public PermissionsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertPermissions(Permissions PermissionsX)
        {
            session.Merge(PermissionsX);
        }

        public void UpdatePermissions(Permissions PermissionsX)
        {
            session.Update(PermissionsX);
        }

        public void DeletePermissions(Permissions PermissionsX)
        {
            session.Delete(PermissionsX);
        }

        public void DeletePermissionsByUserId(string userId)
        {
            IQuery query = session.CreateSQLQuery("delete from Permissions where UserId = :x");
            query.SetString("x", userId);
            query.ExecuteUpdate();
        }

        public IList<Permissions> GetAllPermissions(Boolean evict)
        {
            IQuery query = session.CreateQuery("select p from Permissions as p");
            IList<Permissions> list = query.List<Permissions>();
            if (evict)
            {
                foreach (Permissions s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public Permissions GetAllPermissionsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Permissions as c where c.Id = :x");
            query.SetString("x", id);
            Permissions s = (Permissions)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public Permissions GetAllPermissionsByUserIdAndFunctionId(String userId, String functionId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Permissions as c where c.UserId = :x and c.FunctionId = :y");
            query.SetString("x", userId);
            query.SetString("y", functionId);
            Permissions s = (Permissions)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public Permissions GetAllPermissionsByGroupUserIdAndFunctionId(String groupId, String functionId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Permissions as c where c.UserGroupId = :x and c.FunctionId = :y");
            query.SetString("x", groupId);
            query.SetString("y", functionId);
            Permissions s = (Permissions)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<Permissions> GetPermissionsByQuery(IQuery query, Boolean evict)
        {
            IList<Permissions> list = query.List<Permissions>();
            if (evict)
            {
                foreach (Permissions s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }


    }
}
