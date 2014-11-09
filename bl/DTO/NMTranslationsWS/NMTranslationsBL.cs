// NMTranslationsBL.cs

// * http://nexmi.com
// * NoErp Project - Nexmi Open ERP
// * Copyright (C) 2012, Nguyễn Quang Tuyến (tuyen.nq@nexmi.com), AUTHOR.txt (http://nexmi.com/about)
// * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; 
//   either version 2 of the License, or (at your option) any later version. This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
//   without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details. 
//   You should have received a copy of the GNU General Public License along with this library; if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// *

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace NEXMI
{
    public class NMTranslationsBL
    {
        private ISession Session = SessionFactory.GetNewSession();
        public NMTranslationsBL()
        {

        }

        public NMTranslationsWSI callSingleBL(NMTranslationsWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SAV_OBJ":
                    return SaveObject(wsi);
                case "SEL_OBJ":
                    return SelectObject(wsi);
                case "DEL_OBJ":
                    return DeleteObject(wsi);
                default:
                    return wsi;
            }
        }

        public List<NMTranslationsWSI> callListBL(NMTranslationsWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMTranslationsWSI>();
            }
        }

        public NMTranslationsWSI SelectObject(NMTranslationsWSI wsi)
        {
            TranslationsAccesser Acceser = new TranslationsAccesser(Session);
            Translations obj;
            obj = Acceser.GetAllTranslationsByID(wsi.Translation.LanguageId, wsi.Translation.OwnerId, true);
            if (obj != null)
            {
                wsi.Translation = obj;
                wsi.WsiError = "";
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public NMTranslationsWSI SaveObject(NMTranslationsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                TranslationsAccesser Accesser = new TranslationsAccesser(Session);
                Translations obj = Accesser.GetAllTranslationsByID(wsi.Translation.LanguageId, wsi.Translation.OwnerId, true);
                if (obj != null)
                {
                    wsi.Translation.Id = obj.Id;
                    Accesser.UpdateTranslations(wsi.Translation);
                }
                else
                {
                    Accesser.InsertTranslations(wsi.Translation);
                }
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục";
            }
            return wsi;
        }

        public NMTranslationsWSI DeleteObject(NMTranslationsWSI wsi)
        {
            try
            {
                ITransaction tx = Session.BeginTransaction();
                TranslationsAccesser Accesser = new TranslationsAccesser(Session);
                Translations obj = Accesser.GetAllTranslationsByID(wsi.Translation.LanguageId, wsi.Translation.OwnerId, true);
                if (obj == null)
                {
                    wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
                }
                else
                {
                    try
                    {
                        Accesser.DeleteTranslations(obj);
                        tx.Commit();
                    }
                    catch
                    {
                        wsi.WsiError = "Không được xóa.";
                        tx.Rollback();
                    }
                }
            }
            catch
            {
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục";
            }
            return wsi;
        }

        public List<NMTranslationsWSI> SearchObject(NMTranslationsWSI wsi)
        {
            List<NMTranslationsWSI> ListWSI = new List<NMTranslationsWSI>();
            //Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            //String strCriteria = "";
            //if (wsi.Name != "")
            //{
            //    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
            //    strCriteria += " O.Name LIKE :Name";
            //    ListCriteria.Add("Name", "%" + wsi.Name + "%");
            //}
            //if (wsi.ObjectName != "")
            //{
            //    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
            //    strCriteria += " O.ObjectName = :ObjectName";
            //    ListCriteria.Add("ObjectName", wsi.ObjectName);
            //}
            //String strCmd = "SELECT O FROM Types AS O" + strCriteria;
            //IQuery query = Session.CreateQuery(strCmd);
            //foreach (var Item in ListCriteria)
            //{
            //    query.SetParameter(Item.Key, Item.Value);
            //}
            //TypesAccesser Accesser = new TypesAccesser(Session);
            //IList<Types> objs;
            //objs = Accesser.GetTypesByQuery(query, false);
            //foreach (Types obj in objs)
            //{
            //    wsi = new NMTranslationsWSI();
            //    wsi.Id = obj.Id;
            //    wsi.Name = obj.Name;
            //    wsi.Description = obj.Description;
            //    wsi.ObjectName = obj.ObjectName;
            //    wsi.CreatedDate = obj.CreatedDate.ToString();
            //    wsi.CreatedBy = obj.CreatedBy;
            //    ListWSI.Add(wsi);
            //}
            return ListWSI;
        }       
    }
}
