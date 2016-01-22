using MvcPCRMatEng.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;

namespace MvcPCRMatEng.Controllers
{
    public class MasterController : Controller
    {
        //
        // GET: /Master/

        PCRMatEngEntities dbPcr = new PCRMatEngEntities();
        TNC_ADMINEntities dbTNC = new TNC_ADMINEntities();

        //---------------------------------------------//
        [Check_Authen]
        public ActionResult Purpose()
        {
            ViewBag.Menu = 9;
            return View();
        }

        [HttpPost]
        public JsonResult PurposeList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var query = dbPcr.TM_Purpose;

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.purpose_id,
                        s.purpose_name,
                        s.del_flag
                        //del_flag = s.del_flag == true ? "Yes" : "No"
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
        public JsonResult CreatePurpose()
        {
            try
            {
                var formData = byte.Parse(Request.Form["purpose_id"].ToString());
                var dbData = dbPcr.TM_Purpose.Where(w => w.purpose_id == formData).FirstOrDefault();
                if (dbData == null)
                {
                    TM_Purpose data = new TM_Purpose();
                    data.purpose_id = formData;
                    data.purpose_name = Request.Form["purpose_name"].ToString();
                    data.update_dt = DateTime.Now;
                    data.update_by = Session["PCRME_Auth"].ToString();

                    dbPcr.TM_Purpose.Add(data);
                }

                dbPcr.SaveChanges();

                return Json(new { Result = "OK", Record = dbPcr.TM_Purpose.OrderByDescending(o => o.update_dt).FirstOrDefault() });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdatePurpose()
        {
            try
            {
                var id = byte.Parse(Request.Form["purpose_id"].ToString());
                var data = dbPcr.TM_Purpose.Find(id);
                data.purpose_name = Request.Form["purpose_name"].ToString();
                data.del_flag = Request.Form["del_flag"].ToString() == "0" ? false : true;
                data.update_dt = DateTime.Now;
                data.update_by = Session["PCRME_Auth"].ToString();
                dbPcr.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeletePurpose()
        {
            try
            {
                var id = byte.Parse(Request.Form["purpose_id"].ToString());
                var data = dbPcr.TM_Purpose.Find(id);
                //data.del_flag = true;
                //data.update_dt = DateTime.Now;
                //data.update_by = Session["PCRME_Auth"].ToString();
                dbPcr.TM_Purpose.Remove(data);
                dbPcr.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //---------------------------------------------//
        [Check_Authen]
        public ActionResult Action()
        {
            ViewBag.Menu = 9;
            return View();
        }

        [HttpPost]
        public JsonResult ActionList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var query = dbPcr.TM_Action;

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.action_id,
                        s.action_name,
                        s.del_flag
                        //del_flag = s.del_flag == true ? "Yes" : "No"
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
        public JsonResult CreateAction()
        {
            try
            {
                var formData = byte.Parse(Request.Form["action_id"].ToString());
                var dbData = dbPcr.TM_Action.Where(w => w.action_id == formData).FirstOrDefault();
                if (dbData == null)
                {
                    TM_Action data = new TM_Action();
                    data.action_id = formData;
                    data.action_name = Request.Form["action_name"].ToString();
                    data.update_dt = DateTime.Now;
                    data.update_by = Session["PCRME_Auth"].ToString();

                    dbPcr.TM_Action.Add(data);
                }

                dbPcr.SaveChanges();

                return Json(new { Result = "OK", Record = dbPcr.TM_Action.OrderByDescending(o => o.update_dt).FirstOrDefault() });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateAction()
        {
            try
            {
                var id = byte.Parse(Request.Form["action_id"].ToString());
                var data = dbPcr.TM_Action.Find(id);
                data.action_name = Request.Form["action_name"].ToString();
                data.del_flag = Request.Form["del_flag"].ToString() == "0" ? false : true;
                data.update_dt = DateTime.Now;
                data.update_by = Session["PCRME_Auth"].ToString();
                dbPcr.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteAction()
        {
            try
            {
                var id = byte.Parse(Request.Form["action_id"].ToString());
                var data = dbPcr.TM_Action.Find(id);
                //data.del_flag = true;
                //data.update_dt = DateTime.Now;
                //data.update_by = Session["PCRME_Auth"].ToString();
                dbPcr.TM_Action.Remove(data);
                dbPcr.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //---------------------------------------------//
        [Check_Authen]
        public ActionResult Status()
        {
            ViewBag.Menu = 9;
            return View();
        }

        [HttpPost]
        public JsonResult StatusList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var query = dbPcr.TM_Status;

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.status_id,
                        s.status_name,
                        s.min_lv,
                        s.max_lv,
                        s.del_flag
                        //del_flag = s.del_flag == true ? "Yes" : "No"
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
        public JsonResult CreateStatus()
        {
            try
            {
                var formData = byte.Parse(Request.Form["status_id"].ToString());
                var dbData = dbPcr.TM_Status.Where(w => w.status_id == formData).FirstOrDefault();
                if (dbData == null)
                {
                    TM_Status data = new TM_Status();
                    data.status_id = formData;
                    data.status_name = Request.Form["status_name"].ToString();
                    data.min_lv = byte.Parse(Request.Form["min_lv"].ToString());
                    data.max_lv = byte.Parse(Request.Form["max_lv"].ToString());
                    data.update_dt = DateTime.Now;
                    data.update_by = Session["PCRME_Auth"].ToString();

                    dbPcr.TM_Status.Add(data);
                }

                dbPcr.SaveChanges();

                return Json(new { Result = "OK", Record = dbPcr.TM_Status.OrderByDescending(o => o.update_dt).FirstOrDefault() });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateStatus()
        {
            try
            {
                var id = byte.Parse(Request.Form["status_id"].ToString());
                var data = dbPcr.TM_Status.Find(id);
                data.status_name = Request.Form["status_name"].ToString();
                data.min_lv = byte.Parse(Request.Form["min_lv"].ToString());
                data.max_lv = byte.Parse(Request.Form["max_lv"].ToString());
                data.del_flag = Request.Form["del_flag"].ToString() == "0" ? false : true;
                data.update_dt = DateTime.Now;
                data.update_by = Session["PCRME_Auth"].ToString();
                dbPcr.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteStatus()
        {
            try
            {
                var id = byte.Parse(Request.Form["status_id"].ToString());
                var data = dbPcr.TM_Status.Find(id);
                //data.del_flag = true;
                //data.update_dt = DateTime.Now;
                //data.update_by = Session["PCRME_Auth"].ToString();
                dbPcr.TM_Status.Remove(data);
                dbPcr.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //---------------------------------------------//
        [Check_Authen]
        public ActionResult Level()
        {
            ViewBag.Menu = 9;
            return View();
        }

        [HttpPost]
        public JsonResult LevelList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var query = dbPcr.TM_Level;

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.lv_id,
                        s.lv_name,
                        s.pos_min,
                        s.pos_max,
                        s.del_flag
                        //del_flag = s.del_flag == true ? "Yes" : "No"
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
        public JsonResult CreateLevel()
        {
            try
            {
                var formData = byte.Parse(Request.Form["lv_id"].ToString());
                var min = byte.Parse(Request.Form["pos_min"].ToString());
                var max = byte.Parse(Request.Form["pos_max"].ToString());
                var dbData = dbPcr.TM_Level.Where(w => w.lv_id == formData).FirstOrDefault();
                if (dbData == null)
                {
                    TM_Level data = new TM_Level();
                    data.lv_id = formData;
                    data.lv_name = Request.Form["lv_name"].ToString();
                    data.pos_min = min;
                    data.pos_max = max;
                    data.update_dt = DateTime.Now;
                    data.update_by = Session["PCRME_Auth"].ToString();

                    dbPcr.TM_Level.Add(data);
                }

                dbPcr.SaveChanges();

                return Json(new { Result = "OK", Record = dbPcr.TM_Level.OrderByDescending(o => o.update_dt).FirstOrDefault() });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateLevel()
        {
            try
            {
                var id = byte.Parse(Request.Form["lv_id"].ToString());
                var min = byte.Parse(Request.Form["pos_min"].ToString());
                var max = byte.Parse(Request.Form["pos_max"].ToString());
                var data = dbPcr.TM_Level.Find(id);
                data.lv_name = Request.Form["lv_name"].ToString();
                data.pos_min = min;
                data.pos_max = max;
                data.del_flag = Request.Form["del_flag"].ToString() == "0" ? false : true;
                data.update_dt = DateTime.Now;
                data.update_by = Session["PCRME_Auth"].ToString();
                dbPcr.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteLevel()
        {
            try
            {
                var id = byte.Parse(Request.Form["lv_id"].ToString());
                var data = dbPcr.TM_Level.Find(id);
                //data.del_flag = true;
                //data.update_dt = DateTime.Now;
                //data.update_by = Session["PCRME_Auth"].ToString();
                dbPcr.TM_Level.Remove(data);
                dbPcr.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //---------------------------------------------//
        [Check_Authen]
        public ActionResult GroupCode()
        {
            ViewBag.Menu = 9;
            return View();
        }

        [HttpPost]
        public JsonResult GCodeList(string code, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var query = dbPcr.TM_GroupCode;
                    //from a in dbPcr.TM_GroupCode select a;

                //if (!string.IsNullOrEmpty(code))
                //{
                //    query = query.Where(w => w.group_code.Contains(code));
                //}

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.group_id,
                        s.group_code,
                        s.del_flag
                        //del_flag = s.del_flag == true ? "Yes" : "No"
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
        public JsonResult CreateGCode()
        {
            try
            {
                var gpid = int.Parse(Request.Form["group_id"].ToString());
                var dbData = dbPcr.TM_GroupCode.Where(w => w.group_id == gpid).FirstOrDefault();
                if (dbData == null)
                {
                    TM_GroupCode data = new TM_GroupCode();
                    data.group_id = gpid;
                    data.group_code = Request.Form["group_code"].ToString();
                    data.update_dt = DateTime.Now;
                    data.update_by = Session["PCRME_Auth"].ToString();

                    dbPcr.TM_GroupCode.Add(data);
                }

                dbPcr.SaveChanges();

                return Json(new { Result = "OK", Record = dbPcr.TM_GroupCode.OrderByDescending(o => o.update_dt).FirstOrDefault() });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateGCode()
        {
            try
            {
                var id = int.Parse(Request.Form["group_id"].ToString());
                var data = dbPcr.TM_GroupCode.Find(id);
                data.group_code = Request.Form["group_code"].ToString();
                data.del_flag = Request.Form["del_flag"].ToString() == "0" ? false : true;
                data.update_dt = DateTime.Now;
                data.update_by = Session["PCRME_Auth"].ToString();
                dbPcr.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteGCode()
        {
            try
            {
                var gpid = int.Parse(Request.Form["group_id"].ToString());
                var data = dbPcr.TM_GroupCode.Find(gpid);
                //data.del_flag = true;
                //data.update_dt = DateTime.Now;
                //data.update_by = Session["PCRME_Auth"].ToString();
                dbPcr.TM_GroupCode.Remove(data);
                dbPcr.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //---------------------------------------------//
        [Check_Authen]
        public ActionResult UserType()
        {
            ViewBag.Menu = 9;
            return View();
        }

        [HttpPost]
        public JsonResult UserTypeList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var query = dbPcr.TM_UserType;

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.utype_id,
                        s.utype_name,
                        del_flag = s.del_flag == true ? "Yes" : "No"
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
        public JsonResult CreateUserType()
        {
            try
            {
                var formData = byte.Parse(Request.Form["utype_id"].ToString());
                var dbData = dbPcr.TM_UserType.Where(w => w.utype_id == formData).FirstOrDefault();
                if (dbData == null)
                {
                    TM_UserType data = new TM_UserType();
                    data.utype_id = formData;
                    data.utype_name = Request.Form["utype_name"].ToString();
                    data.update_dt = DateTime.Now;
                    data.update_by = Session["PCRME_Auth"].ToString();

                    dbPcr.TM_UserType.Add(data);
                }

                dbPcr.SaveChanges();

                return Json(new { Result = "OK", Record = dbPcr.TM_UserType.OrderByDescending(o => o.update_dt).FirstOrDefault() });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateUserType()
        {
            try
            {
                var id = byte.Parse(Request.Form["utype_id"].ToString());
                var data = dbPcr.TM_UserType.Find(id);
                data.utype_name = Request.Form["utype_name"].ToString();
                data.update_dt = DateTime.Now;
                data.update_by = Session["PCRME_Auth"].ToString();
                dbPcr.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteUserType()
        {
            try
            {
                var id = byte.Parse(Request.Form["utype_id"].ToString());
                var data = dbPcr.TM_UserType.Find(id);
                data.del_flag = true;
                data.update_dt = DateTime.Now;
                data.update_by = Session["PCRME_Auth"].ToString();
                //dbPcr.TM_Action.Remove(data);
                dbPcr.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //---------------------------------------------//
        [Check_Authen]
        public ActionResult Users()
        {
            ViewBag.Menu = 9;
            return View();
        }

        [HttpPost]
        public JsonResult UserList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var query = dbPcr.TM_User;

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.emp_code,
                        s.utype_id,
                        s.update_dt
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
        public JsonResult CreateUser()
        {
            try
            {
                var user = Request.Form["emp_code"].ToString();
                var utype = byte.Parse(Request.Form["utype_id"].ToString());
                var dbData = dbPcr.TM_User.Where(w => w.emp_code == user && w.utype_id == utype).FirstOrDefault();
                if (dbData == null)
                {
                    TM_User data = new TM_User();
                    data.emp_code = user;
                    data.utype_id = utype;
                    data.update_dt = DateTime.Now;
                    data.update_by = Session["PCRME_Auth"].ToString();

                    dbPcr.TM_User.Add(data);
                }

                dbPcr.SaveChanges();

                return Json(new { Result = "OK", Record = dbPcr.TM_User.OrderByDescending(o => o.update_dt).FirstOrDefault() });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteUser()
        {
            try
            {
                var user = Request.Form["emp_code"].ToString();
                var utype = byte.Parse(Request.Form["utype_id"].ToString());
                var data = dbPcr.TM_User.Find(user, utype);
                dbPcr.TM_User.Remove(data);
                dbPcr.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //---------------------------------------------//
        [Check_Authen]
        public ActionResult Supervisor()
        {
            ViewBag.Menu = 9;
            return View();
        }

        [HttpPost]
        public JsonResult SupList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var query = dbPcr.TM_Supervisor;

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.emp_code,
                        s.group_id,
                        s.update_dt,
                        s.email_flag
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
        public JsonResult CreateSup()
        {
            try
            {
                var user = Request.Form["emp_code"].ToString();
                var gpid = int.Parse(Request.Form["group_id"].ToString());
                var dbData = dbPcr.TM_Supervisor.Where(w => w.emp_code == user && w.group_id == gpid).FirstOrDefault();
                if (dbData == null)
                {
                    TM_Supervisor data = new TM_Supervisor();
                    data.emp_code = user;
                    data.group_id = gpid;
                    data.email_flag = Request.Form["email_flag"].ToString() == "0" ? false : true;
                    data.update_dt = DateTime.Now;
                    data.update_by = Session["PCRME_Auth"].ToString();

                    dbPcr.TM_Supervisor.Add(data);
                }

                dbPcr.SaveChanges();

                return Json(new { Result = "OK", Record = dbPcr.TM_Supervisor.OrderByDescending(o => o.update_dt).FirstOrDefault() });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteSup()
        {
            try
            {
                var user = Request.Form["emp_code"].ToString();
                var gpid = int.Parse(Request.Form["group_id"].ToString());
                var data = dbPcr.TM_Supervisor.Find(user, gpid);
                dbPcr.TM_Supervisor.Remove(data);
                dbPcr.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateSup()
        {
            try
            {
                var user = Request.Form["emp_code"].ToString();
                var gpid = int.Parse(Request.Form["group_id"].ToString());
                var data = dbPcr.TM_Supervisor.Find(user, gpid);
                data.email_flag = Request.Form["email_flag"].ToString() == "0" ? false : true;
                data.update_dt = DateTime.Now;
                data.update_by = Session["PCRME_Auth"].ToString();
                dbPcr.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //---------------------------------------------//
        [HttpPost]
        public JsonResult GetUTypeList()
        {
            try
            {
                var result = dbPcr.TM_UserType
                    .Select(r => new { DisplayText = r.utype_name, Value = r.utype_id });
                return Json(new { Result = "OK", Options = result });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetLevelList()
        {
            try
            {
                var result = dbPcr.TM_Level
                    .Select(r => new { DisplayText = r.lv_name, Value = r.lv_id });
                return Json(new { Result = "OK", Options = result });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetInternalGroupList()
        {
            try
            {
                var result = dbPcr.TM_GroupCode
                    .Select(r => new { DisplayText = r.group_code, Value = r.group_id });
                return Json(new { Result = "OK", Options = result });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetTNCGroupList()
        {
            try
            {
                var result = dbTNC.tnc_group_master.OrderBy(o => o.group_name)
                    .Select(r => new { DisplayText = r.group_name, Value = r.id });
                return Json(new { Result = "OK", Options = result });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetTNCUserList()
        {
            try
            {
                var result = dbTNC.tnc_user.OrderBy(o => o.emp_fname).ThenBy(o => o.emp_lname).Select(r => new { DisplayText = r.emp_fname + " " + r.emp_lname, Value = r.emp_code });
                return Json(new { Result = "OK", Options = result });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}
