// ImagesAccesser.cs

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

namespace NEXMI
{
    public class ImagesAccesser
    {
        ISession session;

        public ImagesAccesser()
        {
            this.session = SessionFactory.GetNewSession();
        }

        public ImagesAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertImages(Images ImagesX)
        {
            session.Merge(ImagesX);
        }

        public void UpdateImages(Images ImagesX)
        {
            session.Update(ImagesX);
        }

        public void DeleteImages(Images ImagesX)
        {
            session.Delete(ImagesX);
        }

        public IList<Images> GetAllImages(Boolean evict)
        {
            IQuery query = session.CreateQuery("select b from Images as b");
            IList<Images> list = query.List<Images>();
            if (evict)
            {
                foreach (Images s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<Images> GetAllImagesByOwner(string ownerId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Images as c where c.Owner = :x");
            query.SetString("x", ownerId);
            IList<Images> list = query.List<Images>();
            if (evict)
            {
                foreach (Images s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public Images GetAllImagesByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Images as c where c.Id = :x");
            query.SetString("x", id);
            Images s = (Images)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public Images GetAllImagesByName(String name, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Images as c where c.Name = :x");
            query.SetString("x", name);
            Images s = (Images)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<Images> GetImagesByQuery(IQuery query, Boolean evict)
        {
            IList<Images> list = query.List<Images>();
            if (evict)
            {
                foreach (Images s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }


    }
}
