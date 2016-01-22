using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using WebCommonFunction;
using MvcPCRMatEng.Models;
using System.Transactions;
using System.IO;
using Rotativa;
using System.Data.Entity.Validation;

namespace MvcPCRMatEng.Controllers
{
    public class HomeController : Controller
    {
        TNCSecurity secure = new TNCSecurity();

        PCRMatEngEntities dbPcr = new PCRMatEngEntities();

        public ActionResult Index()
        {
            ViewBag.Menu = 0;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Menu = 8;
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Menu = 9;
            return View();
        }

        [Check_Authen]
        public ActionResult Main()
        {
            ViewBag.Menu = 1;
            return View();
        }

        [Check_Authen]
        public ActionResult InProcess()
        {
            ViewBag.Menu = 2;
            return View();
        }

        [Check_Authen]
        public ActionResult Completed()
        {
            ViewBag.Menu = 3;
            var purpose = from a in dbPcr.TM_Purpose
                          select a;
            ViewBag.Purpose = purpose;
            return View();
        }

        [Check_Authen]
        public ActionResult Rejected()
        {
            ViewBag.Menu = 4;
            return View();
        }

        [Check_Authen]
        public ActionResult DownloadViewPDF(string groupcode, string year, int runno)
        {
            var model = dbPcr.TD_PCR.Find(groupcode, year, runno);
            var query = (from a in dbPcr.TD_Transaction.Where(w => w.group_code == groupcode && w.year == year && w.run_no == runno
                         && w.status_id < 3).OrderBy(o => o.status_id).ThenByDescending(o => o.round_no).ThenBy(o => o.lv_id).ToList()
                         join b in dbPcr.V_User_Info.ToList() on a.actor equals b.emp_code
                         //orderby a.status_id ascending, a.round_no descending, a.lv_id ascending
                         select new { a.act_dt, a.lv_id, b.emp_fname }).Take(4);
            
            foreach (var item in query)
            {
                if (item.lv_id == 1)
                {
                    ViewBag.P1 = item.emp_fname;
                    ViewBag.D1 = item.act_dt.ToString("dd/MM/yyyy");
                }
                else if (item.lv_id == 2)
                {
                    ViewBag.P2 = item.emp_fname;
                    ViewBag.D2 = item.act_dt.ToString("dd/MM/yyyy");
                }
                else if (item.lv_id == 3)
                {
                    ViewBag.P3 = item.emp_fname;
                    ViewBag.D3 = item.act_dt.ToString("dd/MM/yyyy");
                }
                else if (item.lv_id == 4)
                {
                    ViewBag.P4 = item.emp_fname;
                    ViewBag.D4 = item.act_dt.ToString("dd/MM/yyyy");
                }
            }
            
            return new ViewAsPdf(model);
        }

        [HttpPost]
        public JsonResult InProgressList(int group = 0, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int org_group = int.Parse(Session["PCRME_Org"].ToString());
                byte lv = byte.Parse(Session["PCRME_ULv"].ToString());
                
                var query = dbPcr.V_Tran.Where(w => w.active == true && w.group_id == org_group);

                if (group == 0)//Wait
                {
                    query = query.Where(w => w.lv_id == lv && w.org_id == org_group);
                }

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.group_code,
                        s.year,
                        s.run_no,
                        s.status_name,
                        s.lv_name,
                        s.change_content,
                        s.act_dt,
                        s.lv_id,
                        s.status_id
                    }).OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize);

                //Return result to jTable
                return Json(new { Result = "OK", Records = output, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult InProcessList(int group = 0, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int org_group = int.Parse(Session["PCRME_Org"].ToString());
                byte lv = byte.Parse(Session["PCRME_ULv"].ToString());

                var query = dbPcr.V_Tran.Where(w => w.active == true);

                if (group == 0)//Wait
                {
                    query = query.Where(w => w.lv_id == lv && w.org_id == org_group);
                }
                else
                {
                    if (lv <= 3)
                    {
                        query = query.Where(w => w.group_id == org_group);
                    }
                }

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.group_code,
                        s.year,
                        s.run_no,
                        s.status_name,
                        s.lv_name,
                        s.change_content,
                        s.act_dt,
                        s.lv_id,
                        s.status_id
                    }).OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize);

                //Return result to jTable
                return Json(new { Result = "OK", Records = output, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult CompletedList(string pcr_no, string compound, string line, byte purpose, string rank, DateTime? dtFrom, DateTime? dtTo, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int org_group = int.Parse(Session["PCRME_Org"].ToString());
                byte lv = byte.Parse(Session["PCRME_ULv"].ToString());

                var query = dbPcr.V_Tran.Where(w => w.status_id == 10 && w.group_id == org_group);

                if (!string.IsNullOrEmpty(pcr_no))
                {
                    query = query.Where(w => w.pcr_no.Contains(pcr_no));
                    //query = query.Where(w => w.pcr_no.IndexOf(pcr_no, StringComparison.OrdinalIgnoreCase) >= 0);
                }
                if (!string.IsNullOrEmpty(compound))
                {
                    query = query.Where(w => w.compounditem.Contains(compound));
                    //query = query.Where(w => w.compounditem.IndexOf(compound, StringComparison.OrdinalIgnoreCase) >= 0);
                }
                if (!string.IsNullOrEmpty(line))
                {
                    query = query.Where(w => w.line.Contains(line));
                }
                if (purpose != 0)
                {
                    query = query.Where(w => w.purpose_id == purpose);
                }
                if (!string.IsNullOrEmpty(rank))
                {
                    query = query.Where(w => w.ipqc_rank == rank);
                }
                if (dtFrom.HasValue && dtTo.HasValue)
                {
                    query = query.Where(w => w.act_dt >= dtFrom && w.act_dt <= dtTo);
                }

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.group_code,
                        s.year,
                        s.run_no,
                        s.compounditem,
                        s.line,
                        //s.status_name,
                        //s.lv_name,
                        s.change_content,
                        s.purpose_name,
                        s.act_dt,
                        //s.lv_id,
                        s.status_id,
                        s.ipqc_rank
                    }).OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize);

                //Return result to jTable
                return Json(new { Result = "OK", Records = output, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult RejectedList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int org_group = int.Parse(Session["PCRME_Org"].ToString());
                byte lv = byte.Parse(Session["PCRME_ULv"].ToString());

                var query = dbPcr.V_Tran.Where(w => w.status_id == 11 && w.group_id == org_group);

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.group_code,
                        s.year,
                        s.run_no,
                        s.status_name,
                        s.lv_name,
                        s.change_content,
                        s.act_dt,
                        s.lv_id,
                        s.status_id
                    }).OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize);

                //Return result to jTable
                return Json(new { Result = "OK", Records = output, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [Check_Authen]
        public ActionResult New_PCR()
        {
            ViewBag.Menu = 1;
            var purpose = from a in dbPcr.TM_Purpose
                          select a;
            ViewBag.Purpose = purpose;
            int org_group = int.Parse(Session["PCRME_Org"].ToString());
            //TNCRunNumber run = new TNCRunNumber();
            var groupcode = GetGroupCode(org_group);
            ViewBag.Code = "PCR-" + groupcode + "-" + DateTime.Now.ToString("yy") + "-XXX";
            return View();
        }

        [HttpPost]
        [Check_Authen]
        public ActionResult CreatePCR(HttpPostedFileBase fileCompound, IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                int org_group = int.Parse(Session["PCRME_Org"].ToString());
                string actor = Session["PCRME_Auth"].ToString();
                string group_code = GetGroupCode(org_group);
                DateTime dt = DateTime.Now;
                string yy = dt.ToString("yy");

                TNCRunNumber run = new TNCRunNumber();
                int run_no = run.GetOnlyRunNo(2, group_code);
                //string pcr_no = run.GetRunNumber(2, group_code);

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    bool check_doc = dbPcr.TD_PCR.Any(w => w.group_code == group_code && w.year == yy && w.run_no == run_no);
                    if (!check_doc)
                    {
                        TD_PCR pcr = new TD_PCR();
                        pcr.group_code = group_code;
                        pcr.year = yy;
                        pcr.run_no = run_no;
                        pcr.compounditem = Request.Form["txtCompound"].ToString();
                        pcr.line = Request.Form["txtLine"].ToString();
                        pcr.purpose_id = byte.Parse(Request.Form["selPurpose"].ToString());
                        pcr.change_content = Request.Form["txaChangecontent"].ToString();
                        pcr.reason = Request.Form["txaReason"].ToString();

                        // Add data to DB TD_File //
                        string subPath = "~/UploadFiles/" + yy + "/" + group_code + "/" + run_no + "/";
                        if (!Directory.Exists(Server.MapPath(subPath)))
                            Directory.CreateDirectory(Server.MapPath(subPath));

                        if (fileCompound != null && fileCompound.ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(fileCompound.FileName);
                            var path = Path.Combine(Server.MapPath(subPath), fileName);
                            fileCompound.SaveAs(path);

                            pcr.compounditem_file = subPath + fileName;
                        }
                        dbPcr.TD_PCR.Add(pcr);

                        foreach (var file in files)
                        {
                            if (file != null && file.ContentLength > 0)
                            {
                                var fileName = Path.GetFileName(file.FileName);
                                var path = Path.Combine(Server.MapPath(subPath), fileName);
                                if (SaveFiletoDB(group_code, yy, run_no, fileName, subPath, actor))
                                {
                                    file.SaveAs(path);
                                }
                            }
                        }

                        byte lv = byte.Parse(Session["PCRME_ULv"].ToString());
                        NextStep(group_code, yy, run_no, 1, lv, org_group, 1, actor);
                        dbPcr.SaveChanges();

                        SendEmail(group_code, yy, run_no, GetEmailSup(org_group));

                        scope.Complete();
                    }
                }
                run.SetRunNumber(2, group_code);
                return RedirectToAction("InProcess", "Home");
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    var error = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    var errorMes = "";
                    foreach (var ve in eve.ValidationErrors)
                    {
                        errorMes += string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Check_Authen]
        public ActionResult Edit_PCR(string groupcode, string year, int runno)
        {
            int org_group = int.Parse(Session["PCRME_Org"].ToString());
            byte level = byte.Parse(Session["PCRME_ULv"].ToString());

            TD_PCR pcr = dbPcr.TD_PCR.Find(groupcode, year, runno);

            if (pcr == null)
            {
                return HttpNotFound();
            }

            ViewBag.Code = "PCR-" + groupcode + "-" + year + "-" + runno.ToString("000");
            var purpose = from a in dbPcr.TM_Purpose
                          select a;
            ViewBag.Purpose = purpose;
            
            return View(pcr);
        }

        [Check_Authen]
        [HttpPost]
        public ActionResult UpdatePCR(HttpPostedFileBase fileCompound, IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                var gc = Request.Form["hdGC"].ToString();
                var yy = Request.Form["hdYY"].ToString();
                var rn = int.Parse(Request.Form["hdRN"].ToString());

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    TD_PCR pcr = dbPcr.TD_PCR.Find(gc, yy, rn);
                    if (pcr != null)
                    {
                        pcr.compounditem = Request.Form["txtCompound"].ToString();
                        pcr.line = Request.Form["txtLine"].ToString();
                        pcr.purpose_id = byte.Parse(Request.Form["selPurpose"].ToString());
                        pcr.change_content = Request.Form["txaChangecontent"].ToString();
                        pcr.reason = Request.Form["txaReason"].ToString();

                        // Add data to DB TD_File //
                        string subPath = "~/UploadFiles/" + yy + "/" + gc + "/" + rn + "/";
                        if (!Directory.Exists(Server.MapPath(subPath)))
                            Directory.CreateDirectory(Server.MapPath(subPath));

                        if (fileCompound != null && fileCompound.ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(fileCompound.FileName);
                            var path = Path.Combine(Server.MapPath(subPath), fileName);
                            fileCompound.SaveAs(path);

                            pcr.compounditem_file = subPath + fileName;
                        }

                        string actor = Session["PCRME_Auth"].ToString();

                        foreach (var file in files)
                        {
                            if (file != null && file.ContentLength > 0)
                            {
                                var fileName = Path.GetFileName(file.FileName);
                                var path = Path.Combine(Server.MapPath(subPath), fileName);
                                if (SaveFiletoDB(gc, yy, rn, fileName, subPath, actor))
                                {
                                    file.SaveAs(path);
                                }
                            }
                        }
                        byte lv = byte.Parse(Session["PCRME_ULv"].ToString());
                        int org_group = int.Parse(Session["PCRME_Org"].ToString());
                        NextStep(gc, yy, rn, 9, lv, org_group, 5, actor);

                        dbPcr.SaveChanges();
                        scope.Complete();
                    }
                }
                return RedirectToAction("InProcess", "Home");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult DeleteFile(string groupcode, string year, int runno, string filename)
        {
            //string fullPath = Request.MapPath("~/UploadFiles/" + year + "/" + groupcode + "/" + runno + "/" + filename);
            //if (System.IO.File.Exists(fullPath))
            //{
            //    System.IO.File.Delete(fullPath);

            //    TD_Files fl = dbPcr.TD_Files.Find(groupcode, year, runno, filename);
            //    if (fl != null)
            //    {
            //        dbPcr.TD_Files.Remove(fl);
            //        dbPcr.SaveChanges();
            //    }
            //}

            TD_Files fl = dbPcr.TD_Files.Find(groupcode, year, runno, filename);
            if (fl != null)
            {
                fl.upload_by = Session["PCRME_Auth"].ToString();
                fl.delete_dt = DateTime.Now;
                dbPcr.SaveChanges();
                return Json(new { success = true, response = "Delete file successfuly !!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, response = "Failed to delete file !!" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult RestoreFile(string groupcode, string year, int runno, string filename)
        {
            TD_Files fl = dbPcr.TD_Files.Find(groupcode, year, runno, filename);
            if (fl != null)
            {
                fl.upload_by = Session["PCRME_Auth"].ToString();
                fl.delete_dt = null;
                dbPcr.SaveChanges();
                return Json(new { success = true, response = "Restore file successfuly !!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, response = "Failed to restore file !!" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult NewPCR()
        {
            var purpose = from a in dbPcr.TM_Purpose
                          select a;
            ViewBag.Purpose = purpose;
            int org_group = int.Parse(Session["PCRME_Org"].ToString());
            var groupcode = GetGroupCode(org_group);
            ViewBag.Code = "PCR-" + groupcode + "-" + DateTime.Now.ToString("yy") + "-XXX";
            return View();
        }
   
        [HttpGet]
        public ActionResult _ShowDetailCompleted(string groupcode, string year, int runno)
        {
            var query = dbPcr.TD_PCR.Where(w => w.group_code == groupcode && w.year == year
                                                && w.run_no == runno).FirstOrDefault();
            if (query != null)
            {
                ViewBag.Issuer = GetActorInTransaction(groupcode, year, runno, 1, 1);
                ViewBag.Checker = GetActorInTransaction(groupcode, year, runno, 2, 2);
                ViewBag.Approver = GetActorInTransaction(groupcode, year, runno, 2, 3);

                return PartialView(query);
            }
            else
            {
                TempData["noty"] = "Error loading DETAIL data, try again later !!!";
                return PartialView();
            }
        }

        [HttpGet]
        public ActionResult _ShowDetail(string groupcode, string year, int runno, byte st, byte lv)
        {
            int sup_group = 0;
            byte ulv = byte.Parse(Session["PCRME_ULv"].ToString());
            if (Session["PCRME_SupGroup"] != null && Session["PCRME_SupGroup"].ToString() != "")
            {
                sup_group = int.Parse(Session["PCRME_SupGroup"].ToString());
            }
            
            var query = dbPcr.V_Detail_Current.Where(w => w.group_code == groupcode && w.year == year 
                                                    && w.run_no == runno).FirstOrDefault();
            if (query != null)
            {
                ViewBag.Issuer = GetActorInTransaction(groupcode, year, runno, 1, 1);
                ViewBag.Checker = GetActorInTransaction(groupcode, year, runno, 2, 2);
                ViewBag.Approver = GetActorInTransaction(groupcode, year, runno, 2, 3);

                if (query.lv_id == ulv)
                {
                    if (query.status_id == 3)
                    {
                        ViewBag.FormApprove = 11;
                    }
                    else if (query.status_id == 4)
                    {
                        ViewBag.FormApprove = 12;
                    }
                    else if (sup_group != 0 && query.group_id == sup_group)
                    {
                        ViewBag.FormApprove = 2;//2
                    }
                    else
                    {
                        ViewBag.FormApprove = ulv;
                    }
                }

                return PartialView(query);
            }
            else
            {
                TempData["noty"] = "Error loading DETAIL data, try again later !!!";
                return PartialView();
            }
        }

        [HttpGet]
        public ActionResult _ShowFiles(string groupcode, string year, int runno)
        {
            var query = dbPcr.TD_Files.Where(w => w.group_code == groupcode && w.year == year && w.run_no == runno && w.delete_dt == null).OrderBy(o => o.upload_dt);
            if (query != null)
            {
                return PartialView(query);
            }
            else
            {
                TempData["noty"] = "Error loading FILES, try again later !!!";
                return PartialView();
            }
        }

        //[HttpGet]
        //public ActionResult _ShowIPQC(string groupcode, string year, int runno, byte lv, byte st)
        //{
        //    var query = (from a in dbPcr.TD_Transaction
        //                 where a.group_code == groupcode && a.year == year && a.run_no == runno && a.active == true
        //                 select a).FirstOrDefault();

        //    return PartialView(query);
        //}

        [HttpGet]
        public ActionResult _ShowImplement(string groupcode, string year, int runno)
        {
            var query = dbPcr.TD_PCR.Find(groupcode, year, runno);
            if (query != null)
            {
                return PartialView(query);
            }
            else
            {
                TempData["noty"] = "Error loading IMPLEMENT data, try again later !!!";
                return PartialView();
            }
        }

        [HttpGet]
        public ActionResult _ShowIPQC(string groupcode, string year, int runno)
        {
            var query = dbPcr.TD_PCR.Find(groupcode, year, runno);
            if (query != null)
            {
                return PartialView(query);
            }
            else
            {
                TempData["noty"] = "Error loading Attach IPQC data, try again later !!!";
                return PartialView();
            }
        }

        [HttpGet]
        public ActionResult _ShowTransaction(string groupcode, string year, int runno)
        {
            //var query = dbPcr.TD_Transaction.Where(w => w.group_code == groupcode && w.year == year && w.run_no == runno).OrderBy(o => o.act_dt);
            var query = dbPcr.V_Transaction.Where(w => w.group_code == groupcode && w.year == year && w.run_no == runno).OrderBy(o => o.act_dt).ThenBy(o => o.active);
            if (query != null)
            {
                return PartialView(query);
            }
            else
            {
                TempData["noty"] = "Error loading Transaction data, try again later !!!";
                return PartialView();
            }
        }

        [Check_Authen]
        [HttpGet]
        public ActionResult _FormAddIPQC(V_Detail_Current data)
        {
            //ViewBag.GC = data.group_code;
            //ViewBag.YY = data.year;
            //ViewBag.RN = data.run_no;
            return PartialView(data);
        }

        [Check_Authen]
        [HttpPost]
        public ActionResult AddIPQC()
        {
            try
            {
                var gc = Request.Form["hdGC"].ToString();
                var yy = Request.Form["hdYY"].ToString();
                var rn = int.Parse(Request.Form["hdRN"].ToString());
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    TD_PCR pcr = dbPcr.TD_PCR.Find(gc, yy, rn);
                    if (pcr != null)
                    {
                        pcr.ipqc_rank = Request.Form["selIPQC"].ToString();
                        pcr.ipqc_compound = Request.Form["txtIPQCCP"].ToString();

                        int org_group = int.Parse(Session["PCRME_Org"].ToString());
                        string actor = Session["PCRME_Auth"].ToString();

                        byte lv = byte.Parse(Session["PCRME_ULv"].ToString());
                        NextStep(gc, yy, rn, 2, lv, org_group, 2, actor);

                        dbPcr.SaveChanges();
                    }
                    scope.Complete();
                }
                return RedirectToAction("InProcess", "Home");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult _FormReturn(V_Detail_Current data)
        {
            //ViewBag.GC = data.group_code;
            //ViewBag.YY = data.year;
            //ViewBag.RN = data.run_no;
            //ViewBag.ST = data.status_id;
            return PartialView(data);
        }

        [Check_Authen]
        [HttpPost]
        public ActionResult ReturnToIssuer()
        {
            try
            {
                var gc = Request.Form["hdGC"].ToString();
                var yy = Request.Form["hdYY"].ToString();
                int rn = int.Parse(Request.Form["hdRN"].ToString());
                byte st = byte.Parse(Request.Form["hdST"].ToString());
                string reason = Request.Form["txaReason"];

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    int org_group = int.Parse(Session["PCRME_Org"].ToString());
                    string actor = Session["PCRME_Auth"].ToString();

                    byte lv = byte.Parse(Session["PCRME_ULv"].ToString());
                    NextStep(gc, yy, rn, st, lv, org_group, 4, actor, reason);
                    dbPcr.SaveChanges();

                    scope.Complete();
                }
                return RedirectToAction("InProcess", "Home");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult _FormApprove(V_Detail_Current data)
        {
            return PartialView(data);
        }

        [Check_Authen]
        [HttpPost]
        public ActionResult Approve()
        {
            try
            {
                var gc = Request.Form["hdGC"].ToString();
                var yy = Request.Form["hdYY"].ToString();
                int rn = int.Parse(Request.Form["hdRN"].ToString());
                byte st = byte.Parse(Request.Form["hdST"].ToString());
                byte act = byte.Parse(Request.Form["selJudgement"].ToString());
                string comment = Request.Form["txaComment"];

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    int org_group = int.Parse(Session["PCRME_Org"].ToString());
                    string actor = Session["PCRME_Auth"].ToString();

                    byte lv = byte.Parse(Session["PCRME_ULv"].ToString());

                    NextStep(gc, yy, rn, st, lv, org_group, act, actor, comment);
                    dbPcr.SaveChanges();

                    scope.Complete();
                }
                return RedirectToAction("InProcess", "Home");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult _FormImplement(V_Detail_Current data)
        {
            return PartialView(data);
        }

        [Check_Authen]
        [HttpPost]
        public ActionResult AddImplement(HttpPostedFileBase fileCompound)
        {
            try
            {
                var gc = Request.Form["hdGC"].ToString();
                var yy = Request.Form["hdYY"].ToString();
                int rn = int.Parse(Request.Form["hdRN"].ToString());
                byte st = byte.Parse(Request.Form["hdST"].ToString());

                string pcr_no = "PCR-" + gc + "-" + yy + "-" + rn.ToString("000");

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    TD_PCR pcr = dbPcr.TD_PCR.Find(gc, yy, rn);
                    if (pcr != null)
                    {
                        DateTime dt = DateTime.Now;
                        pcr.start_batch = Request.Form["txtBatch"].ToString();
                        pcr.imp_dt = Request.Form["dtEff"] != null ? ParseToDate(Request.Form["dtEff"].ToString()) : dt;
                        pcr.start_compound = Request.Form["txtCompound"].ToString();

                        // Add data to DB TD_File //
                        string subPath = "~/UploadFiles/" + yy + "/" + gc + "/" + pcr_no + "/";
                        if (!Directory.Exists(Server.MapPath(subPath)))
                            Directory.CreateDirectory(Server.MapPath(subPath));

                        if (fileCompound != null && fileCompound.ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(fileCompound.FileName);
                            var path = Path.Combine(Server.MapPath(subPath), fileName);
                            fileCompound.SaveAs(path);

                            pcr.start_compound_file = subPath + fileName;
                        }
                    }
                    int org_group = int.Parse(Session["PCRME_Org"].ToString());
                    string actor = Session["PCRME_Auth"].ToString();

                    byte lv = byte.Parse(Session["PCRME_ULv"].ToString());

                    if (pcr.ipqc_rank == "No")
                    {
                        NextStep(gc, yy, rn, st, lv, org_group, 6, actor, "RankNo");
                    }
                    else
                    {
                        NextStep(gc, yy, rn, st, lv, org_group, 6, actor);
                    }
                    dbPcr.SaveChanges();

                    scope.Complete();
                }
                return RedirectToAction("InProcess", "Home");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult _FormAttachIPQC(V_Detail_Current data)
        {
            return PartialView(data);
        }

        [Check_Authen]
        [HttpPost]
        public ActionResult AttachIPQC(HttpPostedFileBase fileIPQC)
        {
            try
            {
                var gc = Request.Form["hdGC"].ToString();
                var yy = Request.Form["hdYY"].ToString();
                int rn = int.Parse(Request.Form["hdRN"].ToString());
                byte st = byte.Parse(Request.Form["hdST"].ToString());

                //string pcr_no = "PCR-" + gc + "-" + yy + "-" + rn.ToString("000");

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    TD_PCR pcr = dbPcr.TD_PCR.Find(gc, yy, rn);
                    if (pcr != null)
                    {
                        //DateTime dt = DateTime.Now;

                        // Add data to DB TD_File //
                        string subPath = "~/UploadFiles/" + yy + "/" + gc + "/" + rn + "/";
                        if (!Directory.Exists(Server.MapPath(subPath)))
                            Directory.CreateDirectory(Server.MapPath(subPath));

                        if (fileIPQC != null && fileIPQC.ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(fileIPQC.FileName);
                            var path = Path.Combine(Server.MapPath(subPath), fileName);
                            fileIPQC.SaveAs(path);

                            pcr.ipqc_file = subPath + fileName;
                        }
                    }
                    int org_group = int.Parse(Session["PCRME_Org"].ToString());
                    string actor = Session["PCRME_Auth"].ToString();

                    byte lv = byte.Parse(Session["PCRME_ULv"].ToString());

                    NextStep(gc, yy, rn, st, lv, org_group, 6, actor);
                    
                    dbPcr.SaveChanges();

                    scope.Complete();
                }
                return RedirectToAction("InProcess", "Home");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DateTime ParseToDate(string inputDT)
        {
            char[] delimiters = new char[] { '-', '/', ' ' };
            var temp = inputDT.Split(delimiters);
            DateTime outputDT = DateTime.Parse(temp[2] + "-" + temp[1] + "-" + temp[0]);
            return outputDT;
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            string username = Request.Form["Username"].ToString();
            string pass = Request.Form["Password"].ToString();
            var chklogin = secure.Login(username, pass, true);//set false to true for Real
            if (chklogin != null)
            {
                if (chklogin.dept_id == 18 || chklogin.dept_id == 44)//Check is Material Engineering or System
                {
                    var pos_lv = (from lv in dbPcr.TM_Level
                                  where lv.pos_min <= chklogin.position_level && lv.pos_max >= chklogin.position_level
                                  orderby lv.lv_id ascending
                                  select lv).FirstOrDefault();

                    if (pos_lv != null)
                    {
                        Session["PCRME_ULv"] = pos_lv.lv_id;

                        if (pos_lv.lv_id <= 3)
                        {
                            Session["PCRME_Org"] = chklogin.group_id;
                        }
                        else if (pos_lv.lv_id == 4)
                        {
                            Session["PCRME_Org"] = chklogin.dept_id;
                        }
                        else if (pos_lv.lv_id >= 5)
                        {
                            Session["PCRME_Org"] = chklogin.plant_id;
                        }
                    }

                    Session["PCRME_Auth"] = chklogin.emp_code;
                    Session["PCRME_Group"] = chklogin.group_name;

                    if (chklogin.emp_lname.Length > 2)
                    {
                        Session["PCRME_Name"] = chklogin.emp_fname + " " + chklogin.emp_lname.Substring(0, 2) + ".";
                    }
                    else
                    {
                        Session["PCRME_Name"] = chklogin.emp_fname + " " + chklogin.emp_lname;
                    }

                    //-----------------Check User Type-----------------//
                    var chk_sysuser = (from a in dbPcr.TM_User
                                       where a.emp_code == chklogin.emp_code
                                       select a).FirstOrDefault();

                    Session["PCRME_UType"] = chk_sysuser != null ? chk_sysuser.utype_id : 0;

                    //-----------------Check is Supervisor-----------------//
                    var chk_sup = (from a in dbPcr.TM_Supervisor
                                   where a.emp_code == chklogin.emp_code
                                   select a).FirstOrDefault();

                    if (chk_sup != null)
                    {
                        Session["PCRME_ULv"] = 2;//2 = Level Supervisor
                        Session["PCRME_SupGroup"] = chk_sup.group_id;
                    }

                    if (Session["PCRME_Redirect"] != null)
                    {
                        string url = Session["PCRME_Redirect"].ToString();
                        Session.Remove("PCRME_Redirect");
                        return Redirect(url);
                    }
                    else
                    {
                        return RedirectToAction("InProcess", "Home");
                    }
                }
                else
                {
                    TempData["noty"] = "You are not authorized to access this system !!!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["noty"] = "Username or Password is incorrect !!!";
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Logout()
        {
            Session.Remove("PCRME_Auth");
            Session.Remove("PCRME_Name");
            Session.Remove("PCRME_Group");
            Session.Remove("PCRME_UType");
            Session.Remove("PCRME_ULv");
            Session.Remove("PCRME_Org");
            Session.Remove("PCRME_SupGroup");
            return RedirectToAction("Index", "Home");
        }

        private void NextStep(string group_code, string yy, int run_no, byte status, byte lv, int org_group, byte act = 0, string actor = null, string comment = "")
        {
            TNCOrganization tnc_org = new TNCOrganization();
            tnc_org.GetApprover(actor);
            var round = GetCurrentRound(group_code, yy, run_no);
            var issue_group = GetIssueGroup(group_code, yy, run_no);

            if (act == 1)//Issue
            {
                InsertTransaction(group_code, yy, run_no, round, status, lv, org_group, false, act, actor);
                InsertTransaction(group_code, yy, run_no, round, ++status, ++lv, org_group);
                //SendEmail(group_code, yy, run_no, GetEmailSup(org_group));
            }
            else if(act == 2)//Approve
            {
                UpdateTransaction(group_code, yy, run_no, round, status, lv, org_group, act, actor, comment);

                if (lv < 4)//Supervisor, Mgr.
                {
                    InsertTransaction(group_code, yy, run_no, round, status, (byte)(tnc_org.OrgLevel + 2), tnc_org.OrgId);
                    SendEmail(group_code, yy, run_no, tnc_org.ManagerEMail);
                }
                else if (lv >= 4)//Dept. Up
                {
                    InsertTransaction(group_code, yy, run_no, round, ++status, 1, issue_group);
                    SendEmail(group_code, yy, run_no, GetActorInTransaction(group_code, yy, run_no, 1, 1, 1));
                }
            }
            else if (act == 3)//Reject
            {
                UpdateTransaction(group_code, yy, run_no, round, status, lv, org_group, act, actor, comment);
                InsertTransaction(group_code, yy, run_no, round, 11, 0, 0, false, 10);
                SendEmail(group_code, yy, run_no, GetEmailAll(group_code, yy, run_no), 3, comment);//Rejected
            }
            else if (act == 4)//Return to Issuer
            {
                UpdateTransaction(group_code, yy, run_no, round, status, lv, org_group, act, actor, comment);
                //var get_issuer_org = dbPcr.TD_Transaction.Where(w => w.group_code == group_code && w.year == yy && w.run_no == run_no && w.round_no == round && w.status_id == 1 && w.lv_id == 1).Select(s => s.org_id).FirstOrDefault();

                InsertTransaction(group_code, yy, run_no, ++round, 9, 1, issue_group);
                SendEmail(group_code, yy, run_no, GetEmailAll(group_code, yy, run_no), 1, comment);//Return to Issuer
            }
            else if (act == 5)//Edited
            {
                UpdateTransaction(group_code, yy, run_no, round, status, lv, org_group, act, actor, comment);
                InsertTransaction(group_code, yy, run_no, round, 2, ++lv, org_group);
                SendEmail(group_code, yy, run_no, GetEmailSup(org_group));
            }
            else if (act == 6)//Fill data
            {
                UpdateTransaction(group_code, yy, run_no, round, status, lv, org_group, act, actor);
                if (comment == "RankNo") ++status;
                if (status == 3)
                {
                    InsertTransaction(group_code, yy, run_no, round, ++status, lv, org_group);
                }
                else if (status == 4)
                {
                    InsertTransaction(group_code, yy, run_no, round, 10, 0, 0, false, 10);
                    SendEmail(group_code, yy, run_no, GetEmailAll(group_code, yy, run_no) + "," + GetEmailinGroup(issue_group), 2);
                }
            }
        }

        private int GetIssueGroup(string groupcode, string year, int runno)
        {
            var query = (from a in dbPcr.TD_Transaction
                         where a.group_code == groupcode && a.year == year && a.run_no == runno && a.status_id == 1
                         select a).FirstOrDefault();
            return query != null ? query.org_id : (byte)0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group_code">Check/Set Group Code</param>
        /// <param name="yy">Check/Set Year</param>
        /// <param name="run_no">Check/Set  Run No.</param>
        /// <param name="round">Check/Set  Round</param>
        /// <param name="status">Check/Set Status</param>
        /// <param name="lv">Check/Set User Level</param>
        /// <param name="org">Check/Set Organize No.</param>
        /// <param name="act">Set Action</param>
        /// <param name="actor">Set Actor</param>
        /// <param name="active">Set Active</param>
        private bool InsertTransaction(string group_code, string yy, int run_no, byte round, byte status, byte lv, int org, bool active = true, byte act = 0, string actor = null)
        {
            var chk_tran = dbPcr.TD_Transaction.Any(w => w.group_code == group_code && w.year == yy && w.run_no == run_no && w.round_no == round && w.status_id == status && w.lv_id == lv && w.org_id == org);

            if (!chk_tran)
            {
                TD_Transaction tran = new TD_Transaction();
                tran.group_code = group_code;
                tran.year = yy;
                tran.run_no = run_no;
                tran.round_no = round;
                tran.status_id = status;
                tran.lv_id = lv;
                tran.org_id = org;
                tran.action_id = act;
                tran.actor = actor;
                tran.act_dt = DateTime.Now;
                tran.active = active;
                dbPcr.TD_Transaction.Add(tran);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group_code">Check Group Code</param>
        /// <param name="yy">Check Year</param>
        /// <param name="run_no">Check Run No.</param>
        /// <param name="round">Check Round</param>
        /// <param name="status">Check Status</param>
        /// <param name="lv">Check Level</param>
        /// <param name="org">Check Organize</param>
        /// <param name="act">Set Action</param>
        /// <param name="actor">Set Actor</param>
        /// <param name="comment">Set Comment</param>
        private bool UpdateTransaction(string group_code, string yy, int run_no, byte round, byte status, byte lv, int org, byte act, string actor, string comment = "")
        {
            var chk_tran = dbPcr.TD_Transaction.Where(w => w.group_code == group_code && w.year == yy && w.run_no == run_no && w.round_no == round && w.status_id == status && w.lv_id == lv && w.org_id == org && w.active == true).FirstOrDefault();

            if (chk_tran != null)
            {
                chk_tran.action_id = act;
                chk_tran.actor = actor;
                chk_tran.comment = comment;
                chk_tran.active = false;
                chk_tran.act_dt = DateTime.Now;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SaveFiletoDB(string group_code, string yy, int run_no, string filename, string path, string uploadby)
        {
            try
            {
                var check = dbPcr.TD_Files.Any(w => w.group_code == group_code && w.year == yy && w.run_no == run_no && w.file_name == filename);
                if (!check)
                {
                    TD_Files file = new TD_Files();
                    file.group_code = group_code;
                    file.year = yy;
                    file.run_no = run_no;
                    file.file_name = filename;
                    file.path = path;
                    file.upload_by = uploadby;
                    file.upload_dt = DateTime.Now;
                    dbPcr.TD_Files.Add(file);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string GetGroupCode(int group_id)
        {
            var get_code = (from a in dbPcr.TM_GroupCode
                            where a.group_id == group_id && a.del_flag == false
                            select a).FirstOrDefault();

            if (get_code != null)
            {
                return get_code.group_code;
            }
            else
            {
                return "";
            }
        }

        private byte GetCurrentRound(string groupcode, string year, int runno)
        {
            var query = (from a in dbPcr.TD_Transaction
                         where a.group_code == groupcode && a.year == year && a.run_no == runno
                         orderby a.round_no descending
                         select a).FirstOrDefault();
            return query != null ? query.round_no : (byte)0;
        }

        public ActionResult Test()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TestRoot()
        {
            string actor = Request.Form["txtEmp"].ToString();
            //string actor = Session["PCRME_Auth"].ToString();
            TNCOrganization tnc_org = new TNCOrganization();
            tnc_org.GetApprover(actor);

            TempData["Level"] = tnc_org.OrgLevel + 2;
            TempData["Org"] = tnc_org.OrgId;
            TempData["Email"] = tnc_org.ManagerEMail;

            return RedirectToAction("Test");
        }

        public string GetActorInTransaction(string groupcode, string year, int runno, byte status, byte lv, byte email = 0)
        {
            var actor = (from w in dbPcr.TD_Transaction
                         where w.group_code == groupcode && w.year == year && w.run_no == runno
                         && w.status_id == status && w.lv_id == lv && w.actor != null
                         orderby w.round_no descending
                         select w.actor).FirstOrDefault();

            if (actor != null)
            {
                using (var db = new TNC_ADMINEntities())
                {
                    var info = db.tnc_user.Find(actor);
                    if (email == 1)
                    {
                        return info.email;
                    }
                    else
                    {
                        return info.emp_fname;
                    }
                }
            }
            else
            {
                return "";
            }
        }

        public bool SendEmail(string groupcode, string year, int runno, string receiver, int type=0, string comment = "")
        {
            if (!string.IsNullOrEmpty(receiver))
            {
                string mailto = "";
                char[] delimiter = new char[] { ',' };
                string[] email = receiver.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                int max_email = 47;
                if (email.Length > max_email)//Max send email = 50
                {
                    for (int i = 0; i < max_email; i++)
                    {
                        mailto += "," + email[i];
                    }
                    mailto = mailto.Substring(1);
                }
                else
                {
                    mailto = receiver;
                }

                var get_detail = (from a in dbPcr.TD_PCR
                                  where a.group_code == groupcode && a.year == year && a.run_no == runno
                                  select a).FirstOrDefault();

                if (get_detail != null)
                {
                    TNCUtility tnc_util = new TNCUtility();
                    string subject = "";
                    string body = "";
                    string int_link = "http://webExternal/PcrMat";
                    var pcr_no = "PCR-" + get_detail.group_code + "-" + year + "-" + runno.ToString("000");
                    //var purpose = get_detail.TM_Purpose.purpose_name;

                    if (type == 0)//Default Approve
                    {
                        subject = pcr_no + " wait for Approve.";
                        body = "Dear. All Concern,<br /><br />" +
                            //"MailTo :" + mailto + "<br />" +
                            "<b>PCR No. : </b>" + pcr_no + "<br />" +
                            "<b>Compound Name/Item code : </b>" + get_detail.compounditem + "<br />" +
                            //"<b>Purpose : </b>" + get_detail.TM_Purpose.purpose_name + "<br />" +
                            "<b>Change Content : </b>" + get_detail.change_content + "<br />" +
                            "<b>Link : </b><a href='" + int_link + "'>Click</a><br />" +
                            "<br /><b>*This message is automatic sending from Document Control, please do not reply*</b>";
                    }
                    else if (type == 1) // Return to Issuer
                    {
                        subject = pcr_no + " is returned to you.";
                        body = "Dear. Issuer,<br /><br />" +
                            //"MailTo :" + mailto + "<br />" +
                            "<b>PCR No. : </b>" + pcr_no + "<br />" +
                            "<b>Compound Name/Item code : </b>" + get_detail.compounditem + "<br />" +
                            //"<b>Purpose : </b>" + get_detail.TM_Purpose.purpose_name + "<br />" +
                            "<b>Change Content : </b>" + get_detail.change_content + "<br />" +
                            "<b>Reason for return : </b>" + comment + "<br />" +
                            "<b>Link : </b><a href='" + int_link + "'>Click</a><br />" +
                            "<br /><b>*This message is automatic sending from Document Control, please do not reply*</b>";
                    }
                    else if (type == 2) // Completed
                    {
                        subject = pcr_no + " was completed.";
                        body = "Dear. All Concern,<br /><br />" +
                            //"MailTo :" + mailto + "<br />" +
                            "<b>PCR No. : </b>" + pcr_no + "<br />" +
                            "<b>Compound Name/Item code : </b>" + get_detail.compounditem + "<br />" +
                            //"<b>Purpose : </b>" + get_detail.TM_Purpose.purpose_name + "<br />" +
                            "<b>Change Content : </b>" + get_detail.change_content + "<br />" +
                            "<b>Link : </b><a href='" + int_link + "'>Click</a><br />" +
                            "<br /><b>*This message is automatic sending from Document Control, please do not reply*</b>";
                    }
                    else if (type == 3) // Rejected
                    {
                        subject = pcr_no + " was rejected.";
                        body = "Dear. All Concern,<br /><br />" +
                            //"MailTo :" + mailto + "<br />" +
                            "<b>PCR No. : </b>" + pcr_no + "<br />" +
                            "<b>Compound Name/Item code : </b>" + get_detail.compounditem + "<br />" +
                            //"<b>Purpose : </b>" + get_detail.TM_Purpose.purpose_name + "<br />" +
                            "<b>Change Content : </b>" + get_detail.change_content + "<br />" +
                            "<b>Reason for reject : </b>" + comment + "<br />" +
                            "<b>Link : </b><a href='" + int_link + "'>Click</a><br />" +
                            "<br /><b>*This message is automatic sending from Document Control, please do not reply*</b>";
                    }
                    var result = tnc_util.SendMail(23, "TNCAutoMail-PCRMat@nok.co.th", mailto, subject, body);
                    return result;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private string GetEmailSup(int group_id)
        {
            var email = "";

            var get_email = from a in dbPcr.V_Supervisor
                            where a.email_flag == true && a.group_id == group_id && a.email != null && a.email != ""
                            select a.email;

            foreach (var item in get_email)
            {
                email += "," + item;
            }
            return !string.IsNullOrEmpty(email) ? email.Substring(1) : email;
        }

        private string GetEmailAll(string group_code, string yy, int run_no)
        {
            var email = "";

            var get_email = (from a in dbPcr.V_Transaction
                            where a.group_code == group_code && a.year == yy && a.run_no == run_no && a.email != null && a.email != ""
                            select a.email).Distinct();

            foreach (var item in get_email)
            {
                email += "," + item;
            }
            return !string.IsNullOrEmpty(email) ? email.Substring(1) : email;
        }

        private string GetEmailinGroup(int group_id)
        {
            var email = "";

            var get_email = from a in dbPcr.V_User_Info
                            where a.emp_status == "A" && a.group_id == group_id && a.email != null && a.email != ""
                            select a.email;

            foreach (var item in get_email)
            {
                email += "," + item;
            }
            return !string.IsNullOrEmpty(email) ? email.Substring(1) : email;
        }

        protected override void Dispose(bool disposing)
        {
            dbPcr.Dispose();
            base.Dispose(disposing);
        }
        
    }
}
