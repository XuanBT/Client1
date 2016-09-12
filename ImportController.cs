using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using ClientProject.Models;

namespace ClientProject.Controllers
{
    public class ImportController : Controller
    {
        private phoa262d_PhongMachQuangEntities DBContext = new phoa262d_PhongMachQuangEntities();
        public ActionResult ImportDescription()
        {
            if (TempData["message"] != null)
            {
                ViewBag.thongbao = TempData["message"].ToString();
            }
            return View(DBContext.DANHSACHTHUOCs.ToList());
        }
        [HttpPost]
        public ActionResult SaveDesList()
        {
            DataSet ds = new DataSet();
            if (Request.Files["file"].ContentLength > 0)
            {
                string fileExtension = System.IO.Path.GetExtension(Request.Files["file"].FileName);

                if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {
                    string fileLocation = Server.MapPath("~/Content/") + Request.Files["file"].FileName;
                    if (System.IO.File.Exists(fileLocation))
                    {
                        System.IO.File.Delete(fileLocation);
                    }

                    Request.Files["file"].SaveAs(fileLocation);
                    string excelConnectionString = string.Empty;
                    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";

                    //connection String for xls file format.
                    if (fileExtension == ".xls")
                    {
                        excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                    }

                        //connection String for xlsx file format.
                    else if (fileExtension == ".xlsx")
                    {

                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    }

                    //Create Connection to Excel work book and add oledb namespace
                    OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                    excelConnection.Open();
                    DataTable dt = new DataTable();

                    dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                    if (dt == null)
                    {
                        return null;
                    }

                    String[] excelSheets = new String[dt.Rows.Count];
                    int t = 0;

                    //excel data saves in temp file here.
                    foreach (DataRow row in dt.Rows)
                    {
                        excelSheets[t] = row["TABLE_NAME"].ToString();
                        t++;
                    }
                    OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                    string query = string.Format("Select * from [{0}]", excelSheets[0]);
                    using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                    {
                        dataAdapter.Fill(ds);
                    }
                    excelConnection.Close();
                }

                if (fileExtension.ToString().ToLower().Equals(".xml"))
                {
                    string fileLocation = Server.MapPath("~/Content/") + Request.Files["FileUpload"].FileName;
                    if (System.IO.File.Exists(fileLocation))
                    {
                        System.IO.File.Delete(fileLocation);
                    }

                    Request.Files["FileUpload"].SaveAs(fileLocation);
                    XmlTextReader xmlreader = new XmlTextReader(fileLocation);
                    // DataSet ds = new DataSet();
                    ds.ReadXml(xmlreader);
                    xmlreader.Close();
                }
                if (ds.Tables[0].Rows.Count > 1)
                {

                    string erro = "Row";                 
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        string tenthuongmai = ds.Tables[0].Rows[i][1].ToString();
                        string tenbietduoc = ds.Tables[0].Rows[i][2].ToString();
                        string hamluong = ds.Tables[0].Rows[i][4].ToString();
                        if(!string.IsNullOrWhiteSpace(tenthuongmai)){
                            try
                            {
                                DANHSACHTHUOC NItem = new DANHSACHTHUOC();
                                NItem.TENTHUONGMAI = tenthuongmai;
                                NItem.TENBIETDUOC = tenbietduoc;
                                NItem.HAMLUONG = hamluong;
                                DBContext.DANHSACHTHUOCs.Add(NItem);
                                DBContext.SaveChanges();
                            }
                            catch
                            {
                                erro += "" + (i + 1) + "-";
                            }
                          
                        }
                    }
                    if (erro == "Row")
                    {
                        erro = "Danh sách thuốc đã được lưu thành công";
                    }
                    else
                    {
                        erro = "Please check: " + erro;
                    }
                    TempData["message"] = erro;
                }            
            }
            return RedirectToAction("ImportDescription");
        }
    }
}
