using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using graduation_project_ideas_2.Models;

namespace graduation_project_ideas_2.Controllers
{
    public class HomeController : Controller
    {
        private IA_DBEntities3 db = new IA_DBEntities3();
        //gb_project db = new gb_project();
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult showproff1()
        {
            var profs = db.users.Where(x => x.usertype == 3).ToList();
            List<users> users = new List<users>();
            foreach (users x in profs)
            {
                users.Add(x);
            }
            //var professors = db.professors.ToList();
            //List<professors> professor2 = new List<professors>();
            //foreach (professors x in professors)
            //{
            //    professor2.Add(x);
            //}
            //ViewBag.professor2 = professor2;
            //List<String> name = new List<String>();
            //List<String> name2 = new List<String>();
            //foreach (professors x in professor2)
            //{
            //    var user = db.users.SingleOrDefault(k => k.Id == x.pro_user);
            //    name.Add(user.fullname);
            //    name2.Add(user.email);
            //}
            //ViewBag.name = name;
            //ViewBag.name2 = name2;
            ViewBag.profs = profs;
            ViewBag.numberRow = profs.Count();
            return View();
        }

        [HttpPost]
        public ActionResult Index(users user, string std_level, string std_skills, float std_gpa, HttpPostedFileBase file)
        {
            var username = db.users.Where(x => x.username == user.username).ToList();
            if (ModelState.IsValid)
            {
                if (username.Count() == 0)
                {
                    /* string std_trans*/
                    teamleaders leader = new teamleaders();
                    leader.std_level = std_level;
                    leader.std_skills = std_skills;
                    leader.std_gpa = std_gpa;
                    // leader.std_trans = std_trans;
                    string fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                    file.SaveAs(path);
                    string std_trans2 = fileName;
                    leader.std_trans = std_trans2;
                    user.usertype = 2;
                    db.users.Add(user);
                    leader.user_id = user.Id;
                    db.teamleaders.Add(leader);
                    db.SaveChanges();
                    return RedirectToAction("Login", "users");     
                }
            }
            return View(); 


        }

        public ActionResult showproff()
        {

            var professors = db.professors.ToList();
            List<professors> professor2 = new List<professors>();
            if (professors.Equals(null))
            {
                return HttpNotFound();
            }
            else
            {
                foreach (professors x in professors)
                {
                    professor2.Add(x);
                }
                ViewBag.professor2 = professor2;
                List<String> name = new List<String>();
                List<String> name2 = new List<String>();
                foreach (professors x in professor2)
                {
                    var user = db.users.SingleOrDefault(k => k.Id == x.pro_user);
                    name.Add(user.fullname);
                    name2.Add(user.email);
                }
                ViewBag.name = name;
                ViewBag.name2 = name2;
                ViewBag.numberRow = name.Count();
                return View();
            }
            
        }
    }
}