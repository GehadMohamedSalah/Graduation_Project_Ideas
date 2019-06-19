using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using graduation_project_ideas_2.Models;


namespace graduation_project_ideas_2.Controllers
{
    
    public class ProfessorController : Controller
    {
        private IA_DBEntities3 db = new IA_DBEntities3();

        public ActionResult Index()
        {
            return RedirectToAction("AddPersonalInfo");

        }

        public ActionResult showteam(int id)
        {
            var leader = db.teamleaders.SingleOrDefault(y => y.user_id == id);
            ViewBag.leader = leader;
            var user = db.users.SingleOrDefault(d => d.Id == id);
            ViewBag.user= user;
            var project = db.projects.SingleOrDefault(f => f.teamleader_id == id);
            ViewBag.project = project;
            var members = db.members.ToList();
            List<members> member2 = new List<members>();
            foreach (members x in members)
            {
                if (x.teamleader_id == id)
                {
                    member2.Add(x);
                }
            }

            ViewBag.member2 = member2;
            ViewBag.numberRow = member2.Count();
            return View();

        }

        [HttpGet]
        public ActionResult editaccount()
        {
            ViewBag.fullname = Session["username"];
            ViewBag.userid = Session["userid"];
            ViewBag.user_id = new SelectList(db.users, "Id");
            int UserID = Convert.ToInt32(Session["userid"]);
            var user = db.professors.SingleOrDefault(c => c.pro_user == UserID);
            return View(user);
        }
        [HttpPost]
        public ActionResult editaccount(users users , int exp_year, string pro_desc , int Id)
        {
            if (ModelState.IsValid)
            {
                var zaft = int.Parse(Session["userid"].ToString());
                var account = db.professors.SingleOrDefault(c => c.pro_user == Id);
                var user = db.users.SingleOrDefault(c => c.Id == Id);
                account.users.fullname = users.fullname;
                account.users.username = users.username;
                account.users.email = users.email;
                account.users.password = users.password;
                account.pro_desc = pro_desc;
                account.exp_year = exp_year;
                db.SaveChanges();
                return RedirectToAction("editaccount");
            }
            return View();
            
        }
        [HttpGet]
        public ActionResult AddPersonalInfo()
        {
            ViewBag.fullname = Session["username"];
            ViewBag.userid = Session["userid"];
            ViewBag.user_id = new SelectList(db.professors, "Id");
            return View();
        }

        [HttpPost]
        public ActionResult AddPersonalInfo(professors professor)
        {
            var zaft = int.Parse(Session["userid"].ToString());
            var p = db.professors.Where(x => x.pro_user == zaft).ToList();
            if (ModelState.IsValid)
            {
                if (p.Count() == 0)
                {
                    professor.pro_user = zaft;
                    db.professors.Add(professor);
                    db.SaveChanges();
                    return Json(new { result = 0 }, JsonRequestBehavior.AllowGet);
                }
            }
            return RedirectToAction("editaccount");
            
            
        }

        // GET: Professor
        //[HttpGet]
        //public ActionResult Edit2()
        //{
        //    ViewBag.fullname = Session["username"];
        //    ViewBag.userid = Session["userid"];
        //    ViewBag.user_id = new SelectList(db.professors, "Id");
        //    int UserID = Convert.ToInt32(Session["userid"]);
        //    var prof = db.professors.SingleOrDefault(c => c.pro_user == UserID);
        //    return View(prof);
        //}

        //[HttpPost]
        //public ActionResult Edit2(int year, string desc)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var zaft = int.Parse(Session["userid"].ToString());
        //        var user = db.professors.FirstOrDefault(c => c.pro_user == zaft);
        //        user.exp_year = year;
        //        user.pro_desc = desc;
        //        db.SaveChanges();
        //        return RedirectToAction("Edit2");
        //    }
        //    return View();
        //}

        [HttpGet]
        public ActionResult ShowRequests()
        {
            ViewBag.fullname = Session["username"];
            ViewBag.userid = Session["userid"];
            ViewBag.user_id = new SelectList(db.users, "Id");
            var zaft = int.Parse(Session["userid"].ToString());
            var prof = db.professors.SingleOrDefault(y => y.pro_user == zaft);
            var pro_user = db.users.SingleOrDefault(x => x.Id == prof.pro_user);
            ViewBag.pro_name = pro_user.fullname;
            var order2 = db.orders.ToList();
            List<orders> allOrders = new List<orders>();
            foreach (orders x in order2)
            {
                if(x.professor_id==prof.Id && x.status == 0)
                {
                    allOrders.Add(x);
                }
            }
            List<String> nameProject = new List<String>();
            List<String> nameMember= new List<String>();
            List<String> mailsto = new List<String>();
            List<int> leader_id = new List<int>();
            // allOrders 
            foreach (orders x in allOrders)
            {
                var projects = db.projects.SingleOrDefault(k => k.Id == x.project_id);
                nameProject.Add(projects.proj_name);
                var leader = db.teamleaders.SingleOrDefault(m => m.user_id == x.teamleader_id);
                var member = db.users.SingleOrDefault(z => z.Id == leader.user_id);
                nameMember.Add(member.username);
                leader_id.Add(member.Id);
                mailsto.Add(member.email);
            }
            ViewBag.memberdetails = nameMember;
            ViewBag.projectdetails = nameProject;
            ViewBag.mailsto = mailsto;
            ViewBag.leader_id = leader_id;
            ViewBag.numberRow = nameProject.Count();
            return View();
        }



        [HttpPost]
        public ActionResult ShowRequests(string to, string subject, string body)
        {
            try
            {

                string SenderEmail = System.Configuration.ConfigurationManager.AppSettings["SenderEmail"].ToString();
                string SenderPassword = System.Configuration.ConfigurationManager.AppSettings["SenderPassword"].ToString();
                //class 3lshan a7dd 7agat al gemail
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                //gemail bet3aml m3 al "https" lazm a3rfwa anha true
                client.EnableSsl = true;
                //time
                client.Timeout = 100000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                //b3mlw 3lshan a7dd username w pass
                client.Credentials = new NetworkCredential(SenderEmail, SenderPassword);
                //class l msg a3rf men b3t w b3t ah
                MailMessage msg = new MailMessage(SenderEmail, to, subject, body);
                //msg.To.Add();
                msg.IsBodyHtml = true;
                msg.BodyEncoding = UTF8Encoding.UTF8;
                //bb3t al email
                client.Send(msg);
                //db.student.Add(stu);
                //db.SaveChanges();

                return Json(new { result = 1 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { result = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult accept(int leader_id , int pro_id)
        {
            var user_id = db.professors.SingleOrDefault(c => c.pro_user == pro_id);
            var order = db.orders.SingleOrDefault(c => c.professor_id == user_id.Id && c.teamleader_id == leader_id);
            order.status = 1;
            var orderdel = db.orders.Where(c => c.teamleader_id == leader_id).ToList();
            orderdel.Remove(order);
            foreach(orders x in orderdel)
            {
                db.orders.Remove(x);
                db.SaveChanges();
            }
            return RedirectToAction("ShowTeams");
        }


        public ActionResult reject(int leader_id, int pro_id)
        {
            var user_id = db.professors.SingleOrDefault(c => c.pro_user == pro_id);
            var order = db.orders.SingleOrDefault(c => c.professor_id == user_id.Id && c.teamleader_id == leader_id);
            db.orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("ShowTeams");
        }


        [HttpGet]
        public ActionResult ShowTeams()
        {
            ViewBag.fullname = Session["username"];
            ViewBag.userid = Session["userid"];
            ViewBag.user_id = new SelectList(db.users, "Id");
            var zaft = int.Parse(Session["userid"].ToString());
            var prof = db.professors.SingleOrDefault(y => y.pro_user == zaft);
            var pro_user = db.users.SingleOrDefault(x => x.Id == prof.pro_user);
            ViewBag.pro_name = pro_user.fullname;
            var order2 = db.orders.ToList();
            List<orders> allOrders = new List<orders>();
            foreach (orders x in order2)
            {
                if (x.professor_id == prof.Id && x.status == 1)
                {
                    allOrders.Add(x);
                }
            }
            List<String> nameProject = new List<String>();
            List<String> nameMember = new List<String>();
            List<String> mailsto = new List<String>();
            List<int> leader_id = new List<int>();
            // allOrders 
            foreach (orders x in allOrders)
            {
                var projects = db.projects.SingleOrDefault(k => k.Id == x.project_id);
                nameProject.Add(projects.proj_name);
                var leader = db.teamleaders.SingleOrDefault(m => m.user_id == x.teamleader_id);
                var member = db.users.SingleOrDefault(z => z.Id == leader.user_id);
                nameMember.Add(member.username);
                leader_id.Add(member.Id);
                mailsto.Add(member.email);
            }
            ViewBag.memberdetails = nameMember;
            ViewBag.projectdetails = nameProject;
            ViewBag.mailsto = mailsto;
            ViewBag.leader_id = leader_id;
            ViewBag.numberRow = nameProject.Count();
            return View();
        }

    }
}