// CMSController.cs

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
using System.Web;
using System.Web.Mvc;
using NEXMI;
using System.Text.RegularExpressions;

namespace nexmiStore.Controllers
{
    public class CMSController : Controller
    {
        //
        // GET: /CMS/

        public ActionResult Introduces()
        {
            return PartialView();
        }

        public ActionResult News()
        {
            return PartialView();
        }
        
        public ActionResult Supports()
        {
            return PartialView();
        }

        public ActionResult Recruits()
        {
            return PartialView();
        }

        public ActionResult WorkGuides()
        {
            return PartialView();
        }

        public ActionResult DocumentList(string typeId)
        {
            //string functionId = "";
            //if (typeId == NMConstant.DocumentTypes.Introduce)
            //{
            //    functionId = NMConstant.Functions.Introduces;
            //}
            //else
            //{
            //    functionId = NMConstant.Functions.News;
            //}
            //NMDocumentsBL BL = new NMDocumentsBL();
            //NMDocumentsWSI WSI = new NMDocumentsWSI();
            //WSI.Mode = "SRC_OBJ";
            //WSI.Document = new Documents();
            //WSI.Document.TypeId = typeId;
            //WSI.Document.Owner = owner;
            //if (GetPermissions.Get((List<NMPermissionsWSI>)Session["Permissions"], functionId, "ViewAll") == false)
            //{
            //    WSI.ActionBy = User.Identity.Name;
            //}
            //WSI.PageNum = page;
            //WSI.PageSize = NMCommon.PageSize();
            //WSI.Keyword = keyword;
            //List<NEXMI.NMDocumentsWSI> DOCs = BL.callListBL(WSI);

            //ViewData["WSIs"] = DOCs;
            //ViewData["Page"] = page + 1;
            //if (DOCs.Count > 0)
            //    ViewData["TotalRows"] = DOCs[0].TotalRows;
            //else
            //    ViewData["TotalRows"] = "";
            ViewData["TypeId"] = typeId;
            //ViewData["Owner"] = owner;
            //ViewData["Keyword"] = keyword;

            return PartialView();
        }

        public ActionResult DocumentForm(string id, string typeId)
        {
            NMDocumentsBL BL = new NMDocumentsBL();
            NMDocumentsWSI WSI = new NMDocumentsWSI();
            WSI.Document = new Documents();
            string owner = "";
            if (!string.IsNullOrEmpty(id))
            {
                WSI.Mode = "SEL_OBJ";
                WSI.Document.DocumentId = id;
                WSI = BL.callSingleBL(WSI);
                typeId = WSI.Document.TypeId;
                owner = WSI.Document.Owner;
                NMImagesBL ImageBL = new NMImagesBL();
                NMImagesWSI ImageWSI = new NMImagesWSI();
                ImageWSI.Mode = "SRC_OBJ";
                ImageWSI.Owner = WSI.Document.DocumentId;
                ImageWSI.TypeId = "IMG";
                List<NMImagesWSI> ImageWSIs = ImageBL.callListBL(ImageWSI);
                ViewData["ImageWSIs"] = ImageWSIs;
            }
            ViewData["WSI"] = WSI;
            ViewData["TypeId"] = typeId;
            ViewData["Owner"] = owner;
            return PartialView();
        }

        public JsonResult SaveOrUpdateDocument()
        {
            string error = "";
            NMDocumentsBL BL = new NMDocumentsBL();
            NMDocumentsWSI WSI = new NMDocumentsWSI();
            WSI.Mode = "SAV_OBJ";
            WSI.Document = new Documents();
            WSI.Document.DocumentId = Request.Params["id"];
            WSI.Document.DocumentName = Request.Params["name" + "vi"];
            WSI.Document.TypeId = Request.Params["typeId"];
            WSI.Document.Owner = Request.Params["owner"];
            WSI.Document.CategoryId = Request.Params["categoryId"];
            WSI.Document.Highlight = Boolean.Parse(Request.Params["highlight"]);
            WSI.Document.StatusId = NMConstant.DocStatuses.Draft;
            WSI.ActionBy = User.Identity.Name;
            WSI = BL.callSingleBL(WSI);
            error = WSI.WsiError;
            if (error == "")
            {
                string[] allLanguageId = NMCommon.GetAllLanguageId().Split(';');
                NMTranslationsBL TranslationBL = new NMTranslationsBL();
                NMTranslationsWSI TranslationWSI;
                string languageId = "";
                for (int i = 0; i < allLanguageId.Length - 1; i++)
                {
                    languageId = allLanguageId[i];
                    TranslationWSI = new NMTranslationsWSI();
                    TranslationWSI.Mode = "SAV_OBJ";
                    TranslationWSI.Translation = new Translations();
                    TranslationWSI.Translation.OwnerId = WSI.Document.DocumentId;
                    TranslationWSI.Translation.LanguageId = languageId;
                    TranslationWSI.Translation.Name = Request.Params["name" + languageId];
                    TranslationWSI.Translation.ShortDescription = Request.Params["shortDescription" + languageId];
                    TranslationWSI.Translation.Description = NMCryptography.base64Decode(Request.Params["description" + languageId]);
                    TranslationWSI = TranslationBL.callSingleBL(TranslationWSI);
                }
                string images = Request.Params["images"];
                if (images != "")
                {
                    try
                    {
                        NMImagesBL ImageBL = new NMImagesBL();
                        NMImagesWSI ImageWSI;
                        string[] tempImages = Regex.Split(images, "ROWS");
                        string[] image;
                        foreach (string item in tempImages)
                        {
                            image = Regex.Split(item, "COLS");
                            ImageWSI = new NMImagesWSI();
                            ImageWSI.Mode = "SAV_OBJ";
                            ImageWSI.Name = image[0].ToString();
                            ImageWSI.Description = image[1].ToString();
                            ImageWSI.IsDefault = image[2].ToString();
                            ImageWSI.Owner = WSI.Document.DocumentId;
                            ImageWSI.TypeId = NMConstant.FileTypes.Images;
                            ImageWSI.CreatedBy = User.Identity.Name;
                            ImageWSI = ImageBL.callSingleBL(ImageWSI);
                            error = ImageWSI.WsiError;
                        }
                    }
                    catch
                    {
                        error = "Không lưu được hình ảnh.";
                    }
                }
            }
            return Json(new { id = WSI.Document.DocumentId, error = error });
        }

        public JsonResult DeleteDocument(string id)
        {
            NMDocumentsBL BL = new NMDocumentsBL();
            NMDocumentsWSI WSI = new NMDocumentsWSI();
            WSI.Mode = "DEL_OBJ";
            WSI.Document = new Documents();
            WSI.Document.DocumentId = id;
            WSI = BL.callSingleBL(WSI);
            return Json(WSI.WsiError);
        }

        public ActionResult Rules()
        {
            return PartialView();
        }

        public ActionResult Comments()
        {
            return PartialView();
        }

        public JsonResult DeleteMessage(string id)
        {
            NMMessagesBL BL = new NMMessagesBL();
            NMMessagesWSI WSI = new NMMessagesWSI();
            WSI.Mode = "DEL_OBJ";
            WSI.Message = new Messages();
            WSI.Message.MessageId = id;
            WSI = BL.callSingleBL(WSI);
            return Json(WSI.WsiError);
        }
    }
}
