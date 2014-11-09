// TranslationsAccesser.cs

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
    public class TranslationsAccesser
    {
        ISession session;

        public TranslationsAccesser()
        {
            this.session = SessionFactory.GetNewSession();
        }

        public TranslationsAccesser(ISession session)
        {
            this.session = session;
        }

        public void InsertTranslations(Translations translationsX)
        {
            session.Merge(translationsX);
        }

        public void UpdateTranslations(Translations translationsX)
        {
            session.Update(translationsX);
        }

        public void DeleteTranslations(Translations translationsX)
        {
            session.Delete(translationsX);
        }

        public IList<Translations> GetAllTranslations(Boolean evict)
        {
            IQuery query = session.CreateQuery("select r from Translations as r");
            IList<Translations> list = query.List<Translations>();
            if (evict)
            {
                foreach (Translations s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }

        public Translations GetAllTranslationsByID(String languageId, String ownerId, Boolean evict)
        {
            IQuery query = session.CreateQuery("select c from Translations as c where c.LanguageId = :x and c.OwnerId = :y");
            query.SetString("x", languageId);
            query.SetString("y", ownerId);
            Translations s = (Translations)query.UniqueResult();
            if (evict)
            {
                session.Evict(s);
            }
            return s;
        }

        public IList<Translations> GetTranslationsByQuery(IQuery query, Boolean evict)
        {
            IList<Translations> list = query.List<Translations>();
            if (evict)
            {
                foreach (Translations s in list)
                {
                    session.Evict(s);
                }
            }
            return list;
        }


    }
}
