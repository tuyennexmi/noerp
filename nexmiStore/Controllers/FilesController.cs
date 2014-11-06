// FilesController.cs

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
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace nexmiStore.Controllers
{
    public class FilesController : Controller
    {
        //
        // GET: /Files/

        public ActionResult Index()
        {
            return View();
        }

        public string UploadDynamic(HttpPostedFileBase fileData, string owner, string type)
        {
            string fileName = "";
            switch (type)
            {
                case "IMG":
                    fileName = UploadImage(fileData);
                    break;
                case "VID":
                    fileName = UploadVideo(fileData);
                    break;
                default:
                    fileName = Upload(fileData);
                    break;
            }
            if (!string.IsNullOrEmpty(fileName))
            {
                NMImagesBL BL = new NMImagesBL();
                NMImagesWSI WSI = new NMImagesWSI();
                WSI.Mode = "SAV_OBJ";
                WSI.Name = fileName;
                WSI.Location = fileName;
                WSI.Owner = owner;
                WSI.TypeId = type;
                WSI.CreatedBy = User.Identity.Name;
                WSI = BL.callSingleBL(WSI);
            }
            return fileName;
        }

        public String Upload(HttpPostedFileBase fileData)
        {
            FileInfo fileInfo = new FileInfo(fileData.FileName);
            string filePathOriginal = Server.MapPath(@"~/uploads/files/");
            if (!Directory.Exists(filePathOriginal))
                Directory.CreateDirectory(filePathOriginal);
            string fileName = fileInfo.Name.Split('.')[0];
            string fileNameTemp = fileName;
            string fileExtension = fileInfo.Extension;
            int i = 1;
            while (System.IO.File.Exists(filePathOriginal + fileNameTemp + fileExtension))
            {
                fileNameTemp = fileName + i;
                i++;
            }
            fileData.SaveAs(filePathOriginal + fileNameTemp + fileExtension);
            return fileNameTemp + fileExtension;
        }

        public bool ThumbnailCallback()
        {
            return false;
        }

        public String UploadImage(HttpPostedFileBase fileData)
        {
            FileInfo fileInfo = new FileInfo(fileData.FileName);
            string filePathOriginal = Server.MapPath("~/uploads/images/");
            if (!Directory.Exists(filePathOriginal))
                Directory.CreateDirectory(filePathOriginal);
            string filePathThumbnail = Server.MapPath("~/uploads/_thumbs/");
            if (!Directory.Exists(filePathThumbnail))
                Directory.CreateDirectory(filePathThumbnail);
            string fileName = fileInfo.Name.Split('.')[0];
            string fileNameTemp = fileName;
            string fileExtension = fileInfo.Extension;
            int i = 1;
            while (System.IO.File.Exists(filePathOriginal + fileNameTemp + fileExtension))
            {
                fileNameTemp = fileName + i;
                i++;
            }
            Image originalImage = new Bitmap(fileData.InputStream);
            Image.GetThumbnailImageAbort callBack = new Image.GetThumbnailImageAbort(ThumbnailCallback);
            Image thumbnailImage;
            //Get original Image Dimensions
            int originalHeight = originalImage.Height;
            int originalWidth = originalImage.Width;
            
            if (originalWidth > 250 && originalWidth > originalHeight)
            {
                thumbnailImage = originalImage.GetThumbnailImage(250, (250 * originalImage.Height) / originalImage.Width, callBack, IntPtr.Zero);
            }
            else if (originalHeight > 250 && originalHeight > originalWidth)
            {
                thumbnailImage = originalImage.GetThumbnailImage((250 * originalImage.Width) / originalImage.Height, 250, callBack, IntPtr.Zero);
            }
            else
            {
                thumbnailImage = originalImage.GetThumbnailImage(originalImage.Width, originalImage.Height, callBack, IntPtr.Zero);
            }

            originalImage.Save(filePathOriginal + fileNameTemp + fileExtension);
            thumbnailImage.Save(filePathThumbnail + fileNameTemp + fileExtension);
            return fileNameTemp + fileExtension;
        }

        public String UploadVideo(HttpPostedFileBase fileData)
        {
            FileInfo fileInfo = new FileInfo(fileData.FileName);
            string filePathOriginal = Server.MapPath("~/uploads/videos/");
            if (!Directory.Exists(filePathOriginal))
                Directory.CreateDirectory(filePathOriginal);
            string filePathThumbnail = Server.MapPath("~/uploads/_thumbs/");
            if (!Directory.Exists(filePathThumbnail))
                Directory.CreateDirectory(filePathThumbnail);
            string fileName = fileInfo.Name.Split('.')[0];
            string fileNameTemp = fileName;
            string fileExtension = fileInfo.Extension;
            int i = 1;
            while (System.IO.File.Exists(filePathOriginal + fileNameTemp + fileExtension))
            {
                fileNameTemp = fileName + i;
                i++;
            }
            fileData.SaveAs(filePathOriginal + fileNameTemp + fileExtension);
            return fileNameTemp + fileExtension;
        }

        public JsonResult DeleteDynamic(String fileName, string type)
        {
            string result = "";
            try
            {
                string filePathOriginal = "", filePathThumbnail = "";
                switch (type)
                {
                    case "Images":
                        filePathOriginal = Server.MapPath("~/uploads/images/" + fileName);
                        try
                        {
                            System.IO.File.Delete(filePathOriginal);
                        }
                        catch { }
                        filePathThumbnail = Server.MapPath("~/uploads/_thumbs/" + fileName);
                        try
                        {
                            System.IO.File.Delete(filePathThumbnail);
                        }
                        catch { }
                        break;
                    case "Videos":
                        filePathOriginal = Server.MapPath("~/uploads/videos/" + fileName);
                        try
                        {
                            System.IO.File.Delete(filePathOriginal);
                        }
                        catch { }
                        filePathThumbnail = Server.MapPath("~/uploads/_thumbs/" + fileName);
                        try
                        {
                            System.IO.File.Delete(filePathThumbnail);
                        }
                        catch { }
                        break;
                    default:
                        filePathOriginal = Server.MapPath("~/uploads/files/" + fileName);
                        try
                        {
                            System.IO.File.Delete(filePathOriginal);
                        }
                        catch { }
                        break;
                }
                NMImagesBL BL = new NMImagesBL();
                NMImagesWSI WSI = new NMImagesWSI();
                WSI.Mode = "DEL_OBJ";
                WSI.Name = fileName;
                WSI = BL.callSingleBL(WSI);
                result = WSI.WsiError;
            }
            catch (Exception ex)
            {
                result = "Không thực hiện được.\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi:"
                    + ex.Message;
            }
            return Json(result);
        }

        public ActionResult DynamicUploader(string elementId, string owner, string type)
        {
            ViewData["ElementId"] = elementId;
            ViewData["Owner"] = owner;
            ViewData["Type"] = type;
            return PartialView();
        }

        public ActionResult ImageUploader()
        {
            return PartialView();
        }

    }
}
