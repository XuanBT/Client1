using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Pechkin;
using ClientProject.Models;

namespace ClientProject.Controllers
{
    public class HomeController : Controller
    {
        //private QL_PHONGMACHEntities DBContext = new QL_PHONGMACHEntities();
        private phoa262d_PhongMachQuangEntities DBContext = new phoa262d_PhongMachQuangEntities();
        private List<CHITIETTOATHUOC> DrugDetailList = new List<CHITIETTOATHUOC>();
        int dem = 0;
        public ActionResult AdminHomePage()
        {
            return View();
        }

        // phan benh nhan
        [HttpGet]
        public ActionResult PatientPage()
        {
            return View(DBContext.BENHNHANs.OrderBy(p => p.HOTEN).ToList());
        }

        [HttpPost]
        public ActionResult PatientPage(BENHNHAN bn)
        {
            string mabn = bn.MA_BENHNHAN;
            if (mabn=="0")
            {
                BENHNHAN NewItem = new BENHNHAN();
                NewItem.HOTEN = bn.HOTEN;
                if (!string.IsNullOrWhiteSpace(bn.DIACHI))
                    NewItem.DIACHI = bn.DIACHI;
                else
                    NewItem.DIACHI = "";
                if (!string.IsNullOrWhiteSpace(bn.SODT))
                    NewItem.SODT = bn.SODT;
                else
                    NewItem.SODT = "";
                if (bn.NGAYSINH!=null)
                    NewItem.NGAYSINH = bn.NGAYSINH;
                NewItem.NAMSINH = bn.NAMSINH;
                NewItem.BENHNHANID = bn.BENHNHANID;
                if (bn.GIOITINH == "nam")
                    NewItem.GIOITINH = "nam";
                else
                    NewItem.GIOITINH = "nu";
                DateTime dateitem= DateTime.Now;
                mabn = "BN"+dateitem.Minute + dateitem.Second + dateitem.Millisecond;
                NewItem.MA_BENHNHAN = mabn;
                DBContext.BENHNHANs.Add(NewItem);
                DBContext.SaveChanges();
                Session["CheckPatient"] = "Insert";

            }
            else{
                var UpdateItem = DBContext.BENHNHANs.Where(p=>p.MA_BENHNHAN==bn.MA_BENHNHAN).FirstOrDefault();
                if(UpdateItem!=null)
                {
                    UpdateItem.HOTEN = bn.HOTEN;
                    UpdateItem.DIACHI = bn.DIACHI;
                    UpdateItem.SODT = bn.SODT;
                    UpdateItem.NGAYSINH = bn.NGAYSINH;
                    UpdateItem.NAMSINH = bn.NAMSINH;
                    UpdateItem.BENHNHANID = bn.BENHNHANID;
                    if (bn.GIOITINH == "nam")
                        UpdateItem.GIOITINH = "nam";
                    else
                        UpdateItem.GIOITINH = "nu";
                    DateTime dateitem = DateTime.Now;                                           
                    DBContext.SaveChanges();
                    Session["CheckPatient"] = "Edit";
                }
            }
            return RedirectToAction("PatientPage","Home");
        }

        [HttpPost]
        public ActionResult DeletePatient(string id)
        {
            var DeletedItem = DBContext.BENHNHANs.Where(p => p.MA_BENHNHAN == id).FirstOrDefault();       
                // Nho xoa danh sach toa thuoc cua benh nhan
                var ToaThuocList = DBContext.TOATHUOCs.Where(p => p.MA_BENHNHAN == id).ToList();
                string maTT = "";
                if (ToaThuocList.Count > 0 && ToaThuocList[0]!=null)
                {
                    for (int i = 0; i < ToaThuocList.Count;i++ )
                    {
                        maTT = ToaThuocList[i].MA_TOATHUOC;
                        var chitietList = DBContext.CHITIETTOATHUOCs.Where(m => m.MA_TOATHUOC == maTT).ToList();
                        if (chitietList.Count > 0 && chitietList[0]!=null)
                        {
                            foreach (var item in chitietList)
                            {
                                DBContext.CHITIETTOATHUOCs.Remove(item);
                            }
                        }
                        var theongayList = DBContext.CTTT_THEONGAY.Where(m => m.MA_TOATHUOC == maTT).ToList();
                        if (theongayList.Count > 0)
                        {
                            foreach (var item1 in theongayList)
                            {
                                DBContext.CTTT_THEONGAY.Remove(item1);
                            }
                        }
                        DBContext.TOATHUOCs.Remove(ToaThuocList[i]);
                    }
                }
                DBContext.BENHNHANs.Remove(DeletedItem);
                DBContext.SaveChanges();     
            return Content("");
        }

        [HttpGet]
        public JsonResult CheckFullName(string mabn,string hoten)
        {
            if(mabn=="0")
            {
                var item = DBContext.BENHNHANs.Where(p => p.HOTEN == hoten).FirstOrDefault();
                if (item != null)
                    return Json("Invalid", JsonRequestBehavior.AllowGet);
                else
                    return Json("Valid", JsonRequestBehavior.AllowGet);
            }
            else
            {
                var item = DBContext.BENHNHANs.Where(p => p.HOTEN == hoten&&p.MA_BENHNHAN!=mabn).FirstOrDefault();
                if (item != null)
                    return Json("Invalid", JsonRequestBehavior.AllowGet);
                else
                    return Json("Valid", JsonRequestBehavior.AllowGet);
            }
          
        }
      [HttpPost]
        public ActionResult GetPatientNumber()
        {
            var Item = DBContext.BENHNHANs.Count();
            return Content(Item+"");
        }
        // ket thuc benh nhan

        //phan toa thuoc
        public ActionResult PrescriptionPage(string key="")
        {
            string query;
            ViewBag.PatientList = DBContext.BENHNHANs.ToList();
            ViewBag.GeneralPresList = DBContext.TOATHUOCs.ToList();
            ViewBag.DSThuoc = DBContext.DANHSACHTHUOCs.ToList();
            if(!string.IsNullOrWhiteSpace(key))
            {
                query = "select BENHNHAN.MA_BENHNHAN,MA_TOATHUOC,BENHNHAN.HOTEN,KETQUA_CHUANDOAN,HUYETAP,MACHTIM,NGAYLAPTOA,NGAYTAIKHAM,BENHNHAN.BENHNHANID,TOATHUOC.LOIKHUYEN,TOATHUOC.HENTAIKHAM from BENHNHAN,TOATHUOC where BENHNHAN.BENHNHANID=TOATHUOC.MA_BENHNHAN AND BENHNHAN.HOTEN LIKE '" + key + "%'";
                ViewBag.PrescriptionList = DBContext.Database.SqlQuery<PrescriptionModel>(query).ToList();
            }
            else
            {
                query = "select  BENHNHAN.MA_BENHNHAN,MA_TOATHUOC,BENHNHAN.HOTEN,KETQUA_CHUANDOAN,HUYETAP,MACHTIM,NGAYLAPTOA,NGAYTAIKHAM,BENHNHAN.BENHNHANID,TOATHUOC.LOIKHUYEN,TOATHUOC.HENTAIKHAM from BENHNHAN,TOATHUOC where BENHNHAN.BENHNHANID=TOATHUOC.MA_BENHNHAN";
                ViewBag.PrescriptionList = DBContext.Database.SqlQuery<PrescriptionModel>(query).ToList();
            }
          
            return View();
        }

        [HttpPost]
        public ActionResult UpdatePrescriptionPage(TOATHUOC item,ICollection<CHITIETTOATHUOC> PDetailList,ICollection<CTTT_THEONGAY> DrugForDayList)
        {
            if (item.MA_TOATHUOC.Equals("0"))
            {
                DateTime dt = DateTime.Now;
                int index = 1;
                string machitietTT = "";
                string matoathuoc = "TT" + dt.Year + dt.Month + dt.Day + dt.Hour + dt.Minute + dt.Second + dt.Millisecond;
                TOATHUOC NewItem = new TOATHUOC();
                NewItem.MA_TOATHUOC = matoathuoc;
                NewItem.MA_BENHNHAN = item.MA_BENHNHAN;
                NewItem.KETQUA_CHUANDOAN = item.KETQUA_CHUANDOAN;
                NewItem.HUYETAP = item.HUYETAP;
                NewItem.MACHTIM = item.MACHTIM;
                NewItem.NGAYLAPTOA = DateTime.Now;
                //NewItem.NGAYTAIKHAM = item.NGAYTAIKHAM;
                NewItem.HENTAIKHAM = item.HENTAIKHAM;
                NewItem.LOIKHUYEN = item.LOIKHUYEN;
                DBContext.TOATHUOCs.Add(NewItem);
                DBContext.SaveChanges();
                Session["CheckPrescription"] = "Insert";
                if (PDetailList != null&&PDetailList.Count>0)
                {
                    foreach (var item1 in PDetailList)
                    {
                        if (!string.IsNullOrWhiteSpace(item1.TENTHUOC))
                        {
                            CHITIETTOATHUOC NewCT = new CHITIETTOATHUOC();
                            machitietTT = "CTTT" + dt.Year + dt.Month + dt.Day + dt.Hour + dt.Minute + dt.Second + dt.Millisecond + index;
                            NewCT.MA_CHITIETTOATHUOC = machitietTT;
                            NewCT.TENTHUOC = item1.TENTHUOC;
                            NewCT.CACHDUNG = item1.CACHDUNG;
                            NewCT.SOLUONG = item1.SOLUONG;
                            NewCT.DONVI = item1.DONVI;
                            NewCT.LIEULUONG_BUOISANG = item1.LIEULUONG_BUOISANG;
                            NewCT.LIEULUONG_BUOITRUA = item1.LIEULUONG_BUOITRUA;
                            NewCT.LIEULUONG_BUOICHIEU = item1.LIEULUONG_BUOICHIEU;
                            NewCT.LIEULUONG_BUOITOI = item1.LIEULUONG_BUOITOI;
                            NewCT.MA_TOATHUOC = matoathuoc;
                            DBContext.CHITIETTOATHUOCs.Add(NewCT);
                            index++;
                        }
                    }
                    DBContext.SaveChanges();
                }
                if (DrugForDayList != null && DrugForDayList.Count > 0)
                {
                    foreach (var item2 in DrugForDayList)
                    {
                        if(!string.IsNullOrWhiteSpace(item2.TN_TENTHUOC))
                        {                
                            CTTT_THEONGAY NewCT = new CTTT_THEONGAY();
                            NewCT.TN_TENTHUOC = item2.TN_TENTHUOC;
                            NewCT.TN_CACHDUNG = item2.TN_CACHDUNG;
                            NewCT.TN_SOLUONG = item2.TN_SOLUONG;
                            NewCT.TN_DONVI = item2.TN_DONVI;
                            NewCT.LL_THU2 = item2.LL_THU2;
                            NewCT.LL_THU3 = item2.LL_THU3;
                            NewCT.LL_THU4 = item2.LL_THU4;
                            NewCT.LL_THU5 = item2.LL_THU5;
                            NewCT.LL_THU6 = item2.LL_THU5;
                            NewCT.LL_THU7 = item2.LL_THU5;
                            NewCT.LL_CHUNHAT = item2.LL_CHUNHAT;
                            NewCT.MA_TOATHUOC = matoathuoc;
                            NewCT.TN_MOTA = item2.TN_MOTA;
                            // lieu luong chieu
                            NewCT.LLC_THU2 = item2.LLC_THU2;
                            NewCT.LLC_THU3 = item2.LLC_THU3;
                            NewCT.LLC_THU4 = item2.LLC_THU4;
                            NewCT.LLC_THU5 = item2.LLC_THU5;
                            NewCT.LLC_THU6 = item2.LLC_THU6;
                            NewCT.LLC_THU7 = item2.LLC_THU7;
                            NewCT.LLC_CHUNHAT = item2.LLC_CHUNHAT;
                            DBContext.CTTT_THEONGAY.Add(NewCT);
                        }
                    }
                    DBContext.SaveChanges();
                }
            }
            else
            {
                 DateTime dt = DateTime.Now;
                int index = 1;
                string machitietTT = "";
                var NewItem = DBContext.TOATHUOCs.Where(p => p.MA_TOATHUOC == item.MA_TOATHUOC).FirstOrDefault();
                NewItem.MA_BENHNHAN = item.MA_BENHNHAN;
                NewItem.KETQUA_CHUANDOAN = item.KETQUA_CHUANDOAN;
                NewItem.HUYETAP = item.HUYETAP;
                NewItem.MACHTIM = item.MACHTIM;
                NewItem.NGAYLAPTOA = item.NGAYLAPTOA;
                NewItem.NGAYTAIKHAM = item.NGAYTAIKHAM;
                DBContext.SaveChanges();
                Session["CheckPrescription"] = "Edit";
                // cap nhat lai chitiettoathuoc
                var PrescriptionDetailDeleteList = DBContext.CHITIETTOATHUOCs.Where(p=>p.MA_TOATHUOC==item.MA_TOATHUOC).ToList();
                foreach(var DeletedItem in PrescriptionDetailDeleteList)
                {
                    DBContext.CHITIETTOATHUOCs.Remove(DeletedItem);
                    DBContext.SaveChanges();
                }
                if (PDetailList != null && PDetailList.Count>0)
                {
                    foreach (var item1 in PDetailList)
                    {
                        if (!string.IsNullOrWhiteSpace(item1.TENTHUOC))
                        {
                            CHITIETTOATHUOC NewCT = new CHITIETTOATHUOC();
                            machitietTT = "CTTT" + dt.Year + dt.Month + dt.Day + dt.Hour + dt.Minute + dt.Second + dt.Millisecond + index;
                            NewCT.MA_CHITIETTOATHUOC = machitietTT;
                            NewCT.TENTHUOC = item1.TENTHUOC;
                            NewCT.CACHDUNG = item1.CACHDUNG;
                            NewCT.SOLUONG = item1.SOLUONG;
                            NewCT.DONVI = item1.DONVI;
                            NewCT.LIEULUONG_BUOISANG = item1.LIEULUONG_BUOISANG;
                            NewCT.LIEULUONG_BUOITRUA = item1.LIEULUONG_BUOITRUA;
                            NewCT.LIEULUONG_BUOICHIEU = item1.LIEULUONG_BUOICHIEU;
                            NewCT.LIEULUONG_BUOITOI = item1.LIEULUONG_BUOITOI;
                            NewCT.MA_TOATHUOC = item.MA_TOATHUOC;
                            DBContext.CHITIETTOATHUOCs.Add(NewCT);
                            index++;
                        }
                    }
                    DBContext.SaveChanges();
                }
                // cap nhat lai  chitiet toa thuoc theo ngay
                var TT_Theongayds = DBContext.CTTT_THEONGAY.Where(p => p.MA_TOATHUOC == item.MA_TOATHUOC).ToList() ;
                foreach(var i in TT_Theongayds)
                {
                    DBContext.CTTT_THEONGAY.Remove(i);
                }
                DBContext.SaveChanges();
                if (DrugForDayList != null && DrugForDayList.Count > 0)
                {
                    foreach (var item2 in DrugForDayList)
                    {
                        if (!string.IsNullOrWhiteSpace(item2.TN_TENTHUOC))
                        {
                            CTTT_THEONGAY NewCT = new CTTT_THEONGAY();
                            NewCT.TN_TENTHUOC = item2.TN_TENTHUOC;
                            NewCT.TN_CACHDUNG = item2.TN_CACHDUNG;
                            NewCT.TN_SOLUONG = item2.TN_SOLUONG;
                            NewCT.TN_DONVI = item2.TN_DONVI;
                            NewCT.LL_THU2 = item2.LL_THU2;
                            NewCT.LL_THU3 = item2.LL_THU3;
                            NewCT.LL_THU4 = item2.LL_THU4;
                            NewCT.LL_THU5 = item2.LL_THU5;
                            NewCT.LL_THU6 = item2.LL_THU5;
                            NewCT.LL_THU7 = item2.LL_THU5;
                            NewCT.LL_CHUNHAT = item2.LL_CHUNHAT;
                            NewCT.MA_TOATHUOC = item.MA_TOATHUOC;
                            NewCT.TN_MOTA = item2.TN_MOTA;
                            NewCT.LLC_THU2 = item2.LLC_THU2;
                            NewCT.LLC_THU3 = item2.LLC_THU3;
                            NewCT.LLC_THU4 = item2.LLC_THU4;
                            NewCT.LLC_THU5 = item2.LLC_THU5;
                            NewCT.LLC_THU6 = item2.LLC_THU6;
                            NewCT.LLC_THU7 = item2.LLC_THU7;
                            NewCT.LLC_CHUNHAT = item2.LLC_CHUNHAT;
                            DBContext.CTTT_THEONGAY.Add(NewCT);
                        }
                    }
                    DBContext.SaveChanges();
                }
            }
         
            return RedirectToAction("PrescriptionPage","Home");
        }

        [HttpPost]
        public ActionResult DeletePrescription(string id)
        {
            try
            {
                // Nho xoa danh sach toa thuoc cua benh nhan
                var ToaThuocList = DBContext.TOATHUOCs.Where(p => p.MA_TOATHUOC == id).FirstOrDefault();
                if (ToaThuocList!=null)
                {
                    string maTT = ToaThuocList.MA_TOATHUOC;
                        var chitietList = DBContext.CHITIETTOATHUOCs.Where(m => m.MA_TOATHUOC == maTT).ToList();
                        if (chitietList.Count > 0)
                        {
                            foreach (var item in chitietList)
                            {
                                DBContext.CHITIETTOATHUOCs.Remove(item);
                            }
                        }

                    // xoa cac loai thuoc theo ngay
                        var theongayList = DBContext.CTTT_THEONGAY.Where(m => m.MA_TOATHUOC == maTT).ToList();
                        if (theongayList.Count > 0)
                        {
                            foreach (var item1 in theongayList)
                            {
                                DBContext.CTTT_THEONGAY.Remove(item1);
                            }
                        }
                        DBContext.TOATHUOCs.Remove(ToaThuocList);
                  
                }
                //DBContext.BENHNHANs.Remove(DeletedItem);
                DBContext.SaveChanges();
                Session["delete"] = "success";
                return Content("valid");
            }
            catch
            {
                Session["delete"] = "failure";
                return Content("invalid");
            }
           
        }

        [HttpPost]
        public JsonResult GetPrescriptionDetailList(string id)
        {
            var DetailList = DBContext.CHITIETTOATHUOCs.Where(p => p.MA_TOATHUOC==id).ToList();
            return Json(DetailList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDrugForWeekList(string id)
        {
            var DetailList = DBContext.CTTT_THEONGAY.Where(p => p.MA_TOATHUOC == id).ToList();
            return Json(DetailList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetPresInfor(string matoathuoc)
        {
            var Item = DBContext.TOATHUOCs.Where(p => p.MA_TOATHUOC == matoathuoc).FirstOrDefault();
            return Json(Item, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetPatientInforandPresInfor(string matoathuoc, string mabenhnhan)
        {
            string query = "select HOTEN,NGAYSINH,SODT,DIACHI,GIOITINH,HUYETAP,MACHTIM,KETQUA_CHUANDOAN from BENHNHAN,TOATHUOC where TOATHUOC.MA_TOATHUOC='" + matoathuoc + "' and BENHNHAN.MA_BENHNHAN='" + mabenhnhan + "'";
            var Item =DBContext.Database.SqlQuery<PatientModel>(query).FirstOrDefault();
            return Json(Item, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetPatientInformation(string benhnhanID)
        {
            var Item = DBContext.BENHNHANs.Where(p=>p.BENHNHANID==benhnhanID).FirstOrDefault();
            return Json(Item, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult LoadTTList(string benhnhanCode)
        {
            List<TOATHUOC> TTList = new List<TOATHUOC>();
            var benhnhanId = DBContext.BENHNHANs.Where(p => p.BENHNHANID == benhnhanCode).FirstOrDefault();
            if(benhnhanId!=null){
                TTList = DBContext.TOATHUOCs.Where(p => p.MA_BENHNHAN == benhnhanId.BENHNHANID).ToList<TOATHUOC>();
            }

            return Json(TTList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult LoadTTListByKeyWord(string hoten)
        {
            List<TOATHUOC> TTList = new List<TOATHUOC>();
            var benhnhanId = DBContext.BENHNHANs.Where(p => p.HOTEN.StartsWith(hoten)).FirstOrDefault();
            if (benhnhanId != null)
            {
                TTList = DBContext.TOATHUOCs.Where(p => p.MA_BENHNHAN == benhnhanId.BENHNHANID).ToList<TOATHUOC>();
            }

            return Json(TTList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult LoadPatientListByKeyWord(string keyWord)
        {
            var patientList = DBContext.BENHNHANs.Where(p => p.HOTEN.StartsWith(keyWord.TrimStart())).ToList();
            return Json(patientList, JsonRequestBehavior.AllowGet);
        }


        // Ket thuc toa thuoc

        // phan tao giao dien toa thuoc cho download pdf
        public ActionResult PrescriptionPrint(string id="")
        {
            var toathuoc = DBContext.TOATHUOCs.Where(p => p.MA_TOATHUOC == id).FirstOrDefault();
            try
            {
                ViewBag.benhnhan = DBContext.BENHNHANs.Where(p => p.MA_BENHNHAN == toathuoc.MA_BENHNHAN).FirstOrDefault();
                var dsthuoc = DBContext.CHITIETTOATHUOCs.Where(p => p.MA_TOATHUOC == toathuoc.MA_TOATHUOC).ToList();
                ViewBag.dsthuoc = dsthuoc;
                ViewBag.dsthuoctheotuan = DBContext.CTTT_THEONGAY.Where(p=>p.MA_TOATHUOC==toathuoc.MA_TOATHUOC).ToList();
                return View(toathuoc);
            }
            catch
            {
                return View();
            }
            
        }

        public ActionResult DownloadPDF(string id="")
        {

            string n = "http://localhost:3907/Home/PrescriptionPrint/" + id + "";
            Session["PDF"] = "yes";
            return new Rotativa.UrlAsPdf(n) { FileName = "ToaThuoc.pdf" };
        }

        public ActionResult Login()
        {
            if (TempData["message"]!=null)
            {
                ViewBag.MessageError = TempData["message"].ToString();
            }
            return View();
        }

        [HttpPost]
        public ActionResult CheckLogin(FormCollection FItem)
        {
            if (string.IsNullOrWhiteSpace(FItem["username"].ToString()) || string.IsNullOrWhiteSpace(FItem["password"].ToString()))
            {
                TempData["message"] = "Tên đăng nhập hoặc mật khẩu không nên để trống";
                return RedirectToAction("Login");
            }
            if (FItem["username"].ToString() == "btquang1974" && FItem["password"].ToString() == "quang1#2#")
            {
                Session["username"] = FItem["username"].ToString();
                return RedirectToAction("AdminHomePage", "Home");
            }
            else
            {
                TempData["message"] = "Tên đăng nhập hoặc mật khẩu không hợp lệ";
                return RedirectToAction("Login");
            }
        }

        public ActionResult Logout()
        {
            Session["username"] = null;
            return RedirectToAction("Login");
        }
    }
}
