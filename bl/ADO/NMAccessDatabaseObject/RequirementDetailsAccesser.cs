// RequirementDetailsAccesser.cs

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
    public class RequirementDetailsAccesser
    {
        ISession session;

        public RequirementDetailsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertRequirementdetails(RequirementDetails requirementdetailsX)
        {
            session.Merge(requirementdetailsX);
        }

        public void UpdateRequirementdetails(RequirementDetails requirementdetailsX)
        {
            session.Update(requirementdetailsX);
        }

        public void DeleteRequirementdetails(RequirementDetails requirementdetailsX)
        {
            session.Delete(requirementdetailsX);
        }

        public IList<RequirementDetails> GetAllRequirementdetails(Boolean evict)
        {
            IQuery query = session.CreateQuery("select r from RequirementDetails as r");
            IList<RequirementDetails> list = query.List<RequirementDetails>();
            if (evict)
            {
                foreach (RequirementDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public RequirementDetails GetAllRequirementdetailsByID(String id, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from RequirementDetails as c where c.Id = :x");
            query.SetString("x", id);
            RequirementDetails s = (RequirementDetails)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<RequirementDetails> GetRequirementdetailsByQuery(IQuery query, Boolean evict)
        {
            IList<RequirementDetails> list = query.List<RequirementDetails>();
            if (evict)
            {
                foreach (RequirementDetails s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }


    }
}
