// NMDocumentsBL.cs

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
using System.Security.Cryptography;
using System.Data.SqlTypes;

namespace NEXMI
{
    public class NMDocumentsBL
    {
        private readonly  ISession Session = SessionFactory.GetNewSession();
        public NMDocumentsBL()
        {

        }

        public NMDocumentsWSI callSingleBL(NMDocumentsWSI wsi)
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

        public List<NMDocumentsWSI> callListBL(NMDocumentsWSI wsi)
        {
            switch (wsi.Mode)
            {
                case "SRC_OBJ":
                    return SearchObject(wsi);
                default:
                    return new List<NMDocumentsWSI>();
            }
        }

        public NMDocumentsWSI SelectObject(NMDocumentsWSI wsi)
        {
            DocumentsAccesser Acceser = new DocumentsAccesser(Session);
            
            Documents obj;
            if (!string.IsNullOrEmpty(wsi.Document.PredefinedRFID))
                obj = Acceser.GetAllDocumentsByRFID(wsi.Document.PredefinedRFID, true);
            else 
                obj = Acceser.GetAllDocumentsByID(wsi.Document.DocumentId, true);

            if (obj != null)
            {
                wsi.Document = obj;
                CategoriesAccesser CategoryAccesser = new CategoriesAccesser(Session);
                TranslationsAccesser TransAccesser = new TranslationsAccesser(Session);
                ImagesAccesser ImageAccesser = new ImagesAccesser(Session);

                wsi.Category = CategoryAccesser.GetAllCategoriesByID(obj.CategoryId, true);
                wsi.Translation = TransAccesser.GetAllTranslationsByID(wsi.LanguageId, obj.DocumentId, true);
                if (wsi.Translation == null) wsi.Translation = new Translations();
                wsi.Images = ImageAccesser.GetAllImagesByOwner(obj.DocumentId, true);

                if (NMCommon.GetSetting("MANAGE_RFID"))
                {
                    LocationsAccesser locationsAccesser = new LocationsAccesser(Session);
                    DocumentCheckingAccesser ckAccesser = new DocumentCheckingAccesser(Session);
                    wsi.Location = locationsAccesser.GetAllLocationsByID(wsi.Document.LocationId, true);
                    wsi.StoreLocation = locationsAccesser.GetAllLocationsByID(wsi.Document.StoreLocationId, true);
                    wsi.CheckingList = ckAccesser.GetAllDocumentCheckingByDocument(obj.DocumentId, true);
                }
                wsi.WsiError = "";
            }
            else
            {
                wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";
            }
            return wsi;
        }

        public NMDocumentsWSI SaveObject(NMDocumentsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                AutomaticValuesAccesser AutomaticValueAccesser = new AutomaticValuesAccesser(Session);
                DocumentsAccesser Accesser = new DocumentsAccesser(Session);
                Documents obj = Accesser.GetAllDocumentsByID(wsi.Document.DocumentId, true);
                if (obj != null)
                {
                    wsi.Document.CreatedDate = obj.CreatedDate;
                    wsi.Document.CreatedBy = obj.CreatedBy;
                    wsi.Document.ModifiedDate = DateTime.Now;
                    wsi.Document.ModifiedBy = wsi.ActionBy;
                    Accesser.UpdateDocument(wsi.Document);
                }
                else
                {
                    //wsi.Document.DocumentId = AutomaticValueAccesser.AutoGenerateId("Documents");
                    wsi.Document.DocumentId = NMCommon.ConvertToUnSign(wsi.Document.DocumentName);
                    wsi.Document.CreatedDate = DateTime.Now;
                    wsi.Document.CreatedBy = wsi.ActionBy;
                    wsi.Document.ModifiedDate = DateTime.Now;
                    wsi.Document.ModifiedBy = wsi.ActionBy;
                    Accesser.InsertDocument(wsi.Document);
                }
                if (NMCommon.GetSetting("CONNECT_MFILE"))
                {
                    MFILES_RFIDAccesser MFileAccesser = new MFILES_RFIDAccesser(Session);
                    MFILES_RFID mfile = MFileAccesser.GetAllMFILES_RFIDByRFID(wsi.Document.PredefinedRFID, true);
                    
                    if (mfile != null)
                    {
                        mfile.LocationId = wsi.Document.LocationId;
                        MFileAccesser.UpdateDocument(mfile);
                    }
                    else
                    {
                        mfile = new MFILES_RFID();
                        mfile.RFID_NO = wsi.Document.PredefinedRFID;
                        mfile.LocationId = wsi.Document.LocationId;
                        MFileAccesser.InsertDocument(mfile);
                    }
                    if (wsi.Checking != null)
                    {
                        DocumentCheckingAccesser chkAccesser = new DocumentCheckingAccesser(Session);
                        chkAccesser.InsertDocumentChecking(wsi.Checking);
                    }
                    if (wsi.Log != null)
                    {
                        MovementLogAccesser mvAccesser = new MovementLogAccesser(Session);

                        wsi.Log.DocumentId = wsi.Document.DocumentId;
                        wsi.Log.Time = DateTime.Now;
                        wsi.Log.LocationId = wsi.Document.LocationId;

                        mvAccesser.InsertMovementLog(wsi.Log);
                    }
                }
                tx.Commit();
            }
            catch(Exception ex)
            {
                tx.Rollback();
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục \n" + ex.Message + "\n" + ex.InnerException;
            }
            return wsi;
        }

        public NMDocumentsWSI DeleteObject(NMDocumentsWSI wsi)
        {
            ITransaction tx = Session.BeginTransaction();
            try
            {
                DocumentsAccesser Accesser = new DocumentsAccesser(Session);
                TranslationsAccesser TranslationAccesser = new TranslationsAccesser(Session);
                Documents obj = Accesser.GetAllDocumentsByID(wsi.Document.DocumentId, true);
                if (obj != null)
                {
                    try
                    {
                        Accesser.DeleteDocument(obj);
                        Translations Translation;
                        string[] languages = NMCommon.GetAllLanguageId().Split(';');
                        for (int i = 0; i < languages.Length - 1; i++)
                        {
                            Translation = TranslationAccesser.GetAllTranslationsByID(languages[i], obj.DocumentId, true);
                            if (Translation != null)
                                TranslationAccesser.DeleteTranslations(Translation);
                        }
                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        wsi.WsiError = "Không thể xóa.\n" + ex.Message + "\n" + ex.InnerException;
                    }
                }
                else
                {
                    wsi.WsiError = "Dữ liệu không tồn tại hoặc đã bị xóa.";   
                }
            }
            catch(Exception ex)
            {
                wsi.WsiError = "Không thực hiện được!\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục\n" + ex.Message + "\n" + ex.InnerException;
            }
            return wsi;
        }

        public List<NMDocumentsWSI> SearchObject(NMDocumentsWSI wsi)
        {
            Dictionary<String, object> ListCriteria = new Dictionary<String, object>();
            String strCriteria = "";
            if (wsi.Document != null)
            {
                if (!string.IsNullOrEmpty(wsi.Document.TypeId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.TypeId = :TypeId";
                    ListCriteria.Add("TypeId", wsi.Document.TypeId);
                }
                if (!string.IsNullOrEmpty(wsi.Document.CategoryId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.CategoryId = :CategoryId";
                    ListCriteria.Add("CategoryId", wsi.Document.CategoryId);
                }
                if (!string.IsNullOrEmpty(wsi.Document.GroupId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.GroupId = :GroupId";
                    ListCriteria.Add("GroupId", wsi.Document.GroupId);
                }
                if (!string.IsNullOrEmpty(wsi.Document.StatusId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.StatusId = :StatusId";
                    ListCriteria.Add("StatusId", wsi.Document.StatusId);
                }
                if (!string.IsNullOrEmpty(wsi.Document.Owner))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.Owner = :Owner";
                    ListCriteria.Add("Owner", wsi.Document.Owner);
                }
                if (wsi.Document.Highlight != null)
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.Highlight = :Highlight";
                    ListCriteria.Add("Highlight", wsi.Document.Highlight.Value);
                }
                //
                if (!string.IsNullOrEmpty(wsi.Document.MFileID))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.MFileID = :MFileID";
                    ListCriteria.Add("MFileID", wsi.Document.MFileID);
                }
                
                if (wsi.Document.PredefinedRFID == "new")
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.DocumentName = NULL AND O.MFileID != NULL ";
                    //strCriteria += " O.PredefinedRFID = NULL";
                    //ListCriteria.Add("PredefinedRFID", null);
                }
                else if (!string.IsNullOrEmpty(wsi.Document.PredefinedRFID))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.PredefinedRFID = :PredefinedRFID";
                    ListCriteria.Add("PredefinedRFID", wsi.Document.PredefinedRFID);
                }

                if (!string.IsNullOrEmpty(wsi.Document.LocationId))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.LocationId = :LocationId";
                    ListCriteria.Add("LocationId", wsi.Document.LocationId);
                }
                if (!string.IsNullOrEmpty(wsi.Document.DocumentName))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " RTRIM(O.DocumentName) LIKE :DocumentName";
                }
                if (!string.IsNullOrEmpty(wsi.Document.Class))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.Class = :Class";
                    ListCriteria.Add("Class", wsi.Document.Class);
                }
                if (!string.IsNullOrEmpty(wsi.Document.Entity))
                {
                    strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                    strCriteria += " O.Entity = :Entity";
                    ListCriteria.Add("Entity", wsi.Document.Entity);
                }
            }
            if (!string.IsNullOrEmpty(wsi.ActionBy))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.CreatedBy = :ActionBy";
                ListCriteria.Add("ActionBy", wsi.ActionBy);
            }
            if (!string.IsNullOrEmpty(wsi.Keyword))
            {
                strCriteria = NMCommon.AddOperatorForQuery(strCriteria);
                strCriteria += " O.DocumentId in (select T.OwnerId from Translations T where T.Name LIKE :Keyword)";
            }
            string strCmdCounter = "SELECT COUNT(O.DocumentId) FROM Documents AS O" + strCriteria;
            IQuery queryCounter = Session.CreateSQLQuery(strCmdCounter);
            if (wsi.SortField != null && wsi.SortOrder != "")
            {
                strCriteria += " ORDER BY O." + wsi.SortField + " " + wsi.SortOrder;
            }
            else
            {
                strCriteria += " ORDER BY O.DocumentId ASC";
            }
            String strCmd = "SELECT O FROM Documents AS O" + strCriteria;
            IQuery query = Session.CreateQuery(strCmd);
            foreach (var Item in ListCriteria)
            {
                query.SetParameter(Item.Key, Item.Value);
                queryCounter.SetParameter(Item.Key, Item.Value);
            }
            if (!string.IsNullOrEmpty(wsi.Keyword))
            {
                query.SetString("Keyword", "%" + wsi.Keyword + "%");
                queryCounter.SetString("Keyword", "%" + wsi.Keyword + "%");
            }
            if (!string.IsNullOrEmpty(wsi.Document.DocumentName))
            {
                query.SetString("DocumentName", "%" + wsi.Document.DocumentName + "%");
                queryCounter.SetString("DocumentName", "%" + wsi.Document.DocumentName + "%");
            }
            List<NMDocumentsWSI> ListWSI = new List<NMDocumentsWSI>();
            DocumentsAccesser Accesser = new DocumentsAccesser(Session);
            CategoriesAccesser CategoryAccesser = new CategoriesAccesser(Session);
            TranslationsAccesser TransAccesser = new TranslationsAccesser(Session);
            ImagesAccesser ImageAccesser = new ImagesAccesser(Session);
            LocationsAccesser locationsAccesser = new LocationsAccesser(Session);

            IList<Documents> objs;
            int totalRows = NMCommon.Counter(queryCounter);
            if (wsi.PageSize != 0)
            {
                objs = Accesser.GetDocumentsByQuery(query, wsi.PageSize, wsi.PageNum, false);
            }
            else
            {
                objs = Accesser.GetDocumentsByQuery(query, false);
            }
            if (wsi.Limit != 0)
            {
                objs = objs.Take(wsi.Limit).ToList();
            }
            NMDocumentsWSI ItemWSI;
            foreach (Documents obj in objs)
            {
                ItemWSI = new NMDocumentsWSI();
                ItemWSI.Document = obj;

                if (NMCommon.GetSetting("GET_CATEGORY_IN_SRC"))
                    ItemWSI.Category = CategoryAccesser.GetAllCategoriesByID(obj.CategoryId, true);

                if (NMCommon.GetSetting("GET_TRANSLATION_IN_SRC"))
                {
                    ItemWSI.Translation = TransAccesser.GetAllTranslationsByID(wsi.LanguageId, obj.DocumentId, true);
                    if (ItemWSI.Translation == null) ItemWSI.Translation = new Translations();
                }

                if (NMCommon.GetSetting("MANAGE_IMAGES"))
                    ItemWSI.Images = ImageAccesser.GetAllImagesByOwner(obj.DocumentId, true);

                if (NMCommon.GetSetting("MANAGE_LOCATION"))
                {
                    ItemWSI.Location = locationsAccesser.GetAllLocationsByID(ItemWSI.Document.LocationId, true);
                    ItemWSI.StoreLocation = locationsAccesser.GetAllLocationsByID(ItemWSI.Document.StoreLocationId, true);
                }

                ListWSI.Add(ItemWSI);
            }
            if (ListWSI.Count > 0)
                ListWSI[0].TotalRows = totalRows;

            return ListWSI;
        }
    }
}

