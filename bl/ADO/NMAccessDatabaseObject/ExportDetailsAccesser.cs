// ExportDetailsAccesser.cs

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
    public class ExportDetailsAccesser
    {
        ISession session;

        public ExportDetailsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertExportdetails(ExportDetails exportdetailsX)
        {
            session.Merge(exportdetailsX);
        }

        public void UpdateExportdetails(ExportDetails exportdetailsX)
        {
            session.Update(exportdetailsX);
        }

        public void DeleteExportdetails(ExportDetails exportdetailsX)
        {
            session.Delete(exportdetailsX);
        }

        public IList<ExportDetails> GetAllExportdetails(Boolean evict)
        {
            IQuery query = session.CreateQuery("select e from ExportDetails as e");
            IList<ExportDetails> list = query.List<ExportDetails>();
            if (evict)
            {
                foreach (ExportDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public IList<ExportDetails> GetAllExportdetailsByExportId(String exportId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select e from ExportDetails as e where e.ExportId = :y");
            query.SetString("y", exportId);
            IList<ExportDetails> list = query.List<ExportDetails>();
            if (evict)
            {
                foreach (ExportDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public ExportDetails GetAllExportdetailsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from ExportDetails as c where c.OrdinalNumber = :x");
            query.SetString("x", id);
            ExportDetails s = (ExportDetails)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public ExportDetails GetAllExportdetailsByProductIdAndExportId(String productId, String exportId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from ExportDetails as c where c.ProductId = :x and c.ExportId = :y");
            query.SetString("x", productId);
            query.SetString("y", exportId);
            ExportDetails s = (ExportDetails)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<ExportDetails> GetExportdetailsByQuery(IQuery query, Boolean evict)
        {
            IList<ExportDetails> list = query.List<ExportDetails>();
            if (evict)
            {
                foreach (ExportDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }


    }
}
