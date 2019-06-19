using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using graduation_project_ideas_2.Models;
namespace graduation_project_ideas_2.Controllers
{
    public class StudentController : Controller
    {
        private IA_DBEntities3 db = new IA_DBEntities3();
        public ActionResult Index()
        {
            ViewBag.fullname = Session["username"];
            ViewBag.userid = Session["userid"];
            ViewBag.teamleader_id = new SelectList(db.teamleaders, "Id");
            var zaft = int.Parse(Session["userid"].ToString());
            var leader = db.teamleaders.SingleOrDefault(y => y.user_id == zaft);
            ViewBag.leader = leader;
            var user = db.users.SingleOrDefault(d => d.Id == leader.user_id);
            ViewBag.user = user;
            var project = db.projects.SingleOrDefault(f => f.teamleader_id == leader.user_id);
            ViewBag.project = project;
            var members = db.members.ToList();
            List<members> member2 = new List<members>();
            foreach (members x in members)
            {
                if (x.teamleader_id == leader.user_id)
                {
                    member2.Add(x);
                }
            }

            ViewBag.member2 = member2;
            ViewBag.numberRow = member2.Count();
            return View();
        }
        [HttpGet]
        public ActionResult AddMember()
        {

            ViewBag.fullname = Session["username"];
            ViewBag.userid = Session["userid"];
            ViewBag.user_id = new SelectList(db.teamleaders, "Id");
            return View();
        }

        [HttpPost]
        public ActionResult AddMember(string fullname,string email,string mem_level, string mem_skills, float mem_gpa, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                var zaft = int.Parse(Session["userid"].ToString()); 
                members member2 = new members();
                member2.teamleader_id = zaft;
                member2.fullname = fullname;
                member2.email = email;
                member2.mem_level = mem_level;
                member2.mem_skills = mem_skills;
                member2.mem_gpa = mem_gpa;
                string fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                file.SaveAs(path);
                string mem_trans2 = fileName;
                member2.mem_trans = mem_trans2;
                db.members.Add(member2);
                db.SaveChanges();
                return RedirectToAction("AddMember", "Student");
            }
            return View();

        }

        [HttpGet]
        public ActionResult AddProject()
        {

            ViewBag.fullname = Session["username"];
            ViewBag.userid = Session["userid"];
            ViewBag.user_id = new SelectList(db.teamleaders, "Id");
            return View();

        }


        [HttpPost]
        public ActionResult AddProject(projects idea)
        {
            if (ModelState.IsValid)
            {
                var zaft = int.Parse(Session["userid"].ToString());
                var projects = db.projects.Where(x => x.teamleader_id == zaft).ToList();
                if (projects.Count() == 0)
                {
                    idea.teamleader_id = zaft;
                    db.projects.Add(idea);
                    db.SaveChanges();
                    return Json(new { result = 0 });
                }
            }
            return RedirectToAction("SelectDept");

        }
        [HttpGet]
        public ActionResult SelectDept()
        {
            DeptProf deptprof = new DeptProf();
            deptprof.departments = db.Department.ToList();
            ViewBag.fullname = Session["username"];
            ViewBag.userid = Session["userid"];
            ViewBag.user_id = new SelectList(db.teamleaders, "Id");
            return View(deptprof);
        }

        [HttpPost]
        public ActionResult SelectDept(DeptProf deptprof)
        {
            var zaft = int.Parse(Session["userid"].ToString());
            var allorders = db.orders.Where(x => x.teamleader_id == zaft).ToList();
            var profs = db.orders.Where(x => x.professor_id == deptprof.pro_user &&
            x.teamleader_id == zaft).ToList();
            if (allorders.Count() >= 3)
            {
                return Json(new { result = 0 }, JsonRequestBehavior.AllowGet);
            }
            else if (profs.Count() != 0)
            {
                return Json(new { result = -1 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //pro id bta3 team leader
                var pro = db.projects.SingleOrDefault(x => x.teamleader_id == zaft);
                orders order = new orders();
                //team leader id all f order equal session 
                order.teamleader_id = zaft;
                //id project bta3 order equal team leader id
                order.project_id = pro.Id;
                //id professour ally f order equal id prof aly f user
                order.professor_id = deptprof.pro_user;
                order.status = 0;
                db.orders.Add(order);
                db.SaveChanges();

                var leader = db.teamleaders.SingleOrDefault(x => x.user_id == zaft);
                var professor = db.professors.SingleOrDefault(x => x.Id == deptprof.pro_user);
                var pro_mail = professor.users.email;
                string subject = "Request To Discuss projetc";
                string body = "Team " + leader.users.username + "want to discuss project with you...";
                ProfessorController obj = new ProfessorController();
                obj.ShowRequests(pro_mail, subject, body);
                return Json(new { result = 1 }, JsonRequestBehavior.AllowGet);
            }


        }

        public JsonResult SelectProf(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<professors> profList = db.professors.Where(x => x.dept_id == id).ToList();

            List<XYZ> xyz = new List<XYZ>();
            XYZ y;
            foreach (professors x in profList)
            {
                var user = db.users.SingleOrDefault(m => m.Id == x.pro_user);
                y = new XYZ();
                y.Id = x.Id;
                y.fullname = user.fullname;
                xyz.Add(y);
            }
            return Json(xyz, JsonRequestBehavior.AllowGet);
        }

        // start code mail
        //[HttpPost]
        //public ActionResult Sendmail(string to, string subject, string body)
        //{
        //    try
        //    {

        //        string SenderEmail = System.Configuration.ConfigurationManager.AppSettings["SenderEmail"].ToString();
        //        string SenderPassword = System.Configuration.ConfigurationManager.AppSettings["SenderPassword"].ToString();
        //        //class 3lshan a7dd 7agat al gemail
        //        SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
        //        //gemail bet3aml m3 al "https" lazm a3rfwa anha true
        //        client.EnableSsl = true;
        //        //time
        //        client.Timeout = 100000;
        //        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        //        client.UseDefaultCredentials = false;
        //        //b3mlw 3lshan a7dd username w pass
        //        client.Credentials = new NetworkCredential(SenderEmail, SenderPassword);
        //        //class l msg a3rf men b3t w b3t ah
        //        MailMessage msg = new MailMessage(SenderEmail, to, subject, body);
        //        //msg.To.Add();
        //        msg.IsBodyHtml = true;
        //        msg.BodyEncoding = UTF8Encoding.UTF8;
        //        //bb3t al email
        //        client.Send(msg);
        //        //db.student.Add(stu);
        //        //db.SaveChanges();

        //        return Json(new { result = 1 }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(new { result = 0 }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        ////end code

        [HttpGet]
        public ActionResult deletemem(int id)
        {
            var selectedStd = db.members.Find(id);
            return View(selectedStd);
        }
        [HttpPost]
        [ActionName("deletemem")]
        public ActionResult deletemem2(int id)
        {
            var selectedStd = db.members.Find(id);
            var deletedStd = db.members.Remove(selectedStd);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult editmem(int id)
        {
            ViewBag.fullname = Session["username"];
            ViewBag.userid = Session["userid"];
            ViewBag.user_id = new SelectList(db.teamleaders, "Id");
            var selectedStd = db.members.Find(id);
            return View(selectedStd);
        }
        [HttpPost]
        public ActionResult editmem(int id, float mem_gpa, string mem_skills, string mem_level, string fullname, string email)
        {
            var zaft = int.Parse(Session["userid"].ToString());
            var user = db.members.FirstOrDefault(c => c.Id == id);
            user.mem_gpa = mem_gpa;
            user.mem_skills = mem_skills;
            user.mem_level = mem_level;
            user.teamleader_id = zaft;
            user.fullname = fullname;
            user.email = email;

            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult editleader(int id)
        {
            var selectedStd = db.teamleaders.Find(id);
            return View(selectedStd);
        }
        [HttpPost]
        public ActionResult editleader(int teamleader_id, users users, float std_gpa, string std_skills, string std_level)
        {
            var leader = db.teamleaders.SingleOrDefault(c => c.user_id == teamleader_id);
            leader.users.fullname = users.fullname;
            leader.users.username = users.username;
            leader.users.email = users.email;
            leader.users.password = users.password;
            leader.users.usertype = 2;
            leader.std_gpa = std_gpa;
            leader.std_skills = std_skills;
            leader.std_level = std_level;
            leader.user_id = teamleader_id;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        //    [HttpGet]
        //    public ActionResult editaccount(int id)
        //    {

        //        var selectedStd = db.users.Find(id);
        //        return View(selectedStd);
        //    }
        //    [HttpPost]
        //    public ActionResult editaccount(int id, string fullname, string username, string email, string password)
        //    {

        //        var account = db.users.FirstOrDefault(c => c.Id == id);
        //        account.fullname = fullname;
        //        account.username = username;
        //        account.email = email;
        //        account.password = password;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    public ActionResult editIdea(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }
        //        projects projects = db.projects.Find(id);
        //        if (projects == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        return View(projects);
        //    }

        //    [HttpPost]
        //    public ActionResult editIdea(projects proj)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            db.Entry(proj).State = EntityState.Modified;
        //            db.SaveChanges();
        //            return RedirectToAction("Index");
        //        }
        //        return View(proj);
        //    }

        //}
    }
}