using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClientProject.Models;

namespace ClientProject.Controllers
{
    public class ImageController : Controller
    {
        private phoa262d_PhongMachQuangEntities db = new phoa262d_PhongMachQuangEntities();
        public ActionResult UpdateBackgroundImage()
        {
            return View(db.IMAGEs.Where(p=>p.Flage=="LOGO").FirstOrDefault());
        }
        public ActionResult ProcessImage(HttpPostedFileBase Uploadfile,IMAGE Item)
        {
            if(Item.ID==0)
            {
                string imagepath = GetImagePath(Uploadfile);
                if (!string.IsNullOrWhiteSpace(imagepath))
                {
                    IMAGE newItem = new IMAGE();
                    newItem.ImagePath = imagepath;
                    newItem.Flage = "LOGO";
                    db.IMAGEs.Add(newItem);
                    db.SaveChanges();
                }
            }
            else
            {
                string imagepath = GetImagePath(Uploadfile);
                var UpdatedItem = db.IMAGEs.Where(p=>p.ID==Item.ID).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(imagepath))
                {
                    UpdatedItem.ImagePath = imagepath;                                 
                }
                db.SaveChanges();
            }

            return RedirectToAction("UpdateBackgroundImage");
        }
        public bool HasFile(HttpPostedFileBase file)
        {
            return (file != null && file.ContentLength > 0) ? true : false;
        }
        public string GetImagePath(HttpPostedFileBase ImageFile)
        {
            string ImagePath = string.Empty;
            if (HasFile(ImageFile))
            {
                try
                {
                    var bitmap = System.Drawing.Bitmap.FromStream(Request.Files[0].InputStream);
                }
                catch
                {

                    return string.Empty;
                }

                string filename = Path.GetFileName(ImageFile.FileName);
                //filename = DateTime.Now.ToString("ddMMyyyy_hhmmss") + filename;
                string path = Path.Combine(Server.MapPath("~/UploadedImages"), filename);
                ImageFile.SaveAs(path);
                ImagePath = Path.Combine("/UploadedImages/", filename);/// Phan nay luu vao co so du lieu
                return ImagePath;
            }
            return string.Empty;
        }
    }
}
