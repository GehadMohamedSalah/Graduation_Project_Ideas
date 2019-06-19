using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using graduation_project_ideas_2.Models;


namespace graduation_project_ideas_2.Controllers
{
    public class AdminController : Controller
    {
        IA_DBEntities3 db = new IA_DBEntities3();
       // gb_project db = new gb_project();
        // GET: Admin
        public ActionResult Index()
        {
            return RedirectToAction("AddProf2");
        }

        
        public ActionResult showproff()
        {
            var profs = db.users.Where(x => x.usertype == 3).ToList();
            List<users> users = new List<users>();
            foreach(users x in profs)
            {
                users.Add(x);
            }
            ViewBag.profs = profs;
            ViewBag.numberRow = profs.Count();
            return View();
        }

        [HttpGet]
        public ActionResult Add_Del_Pro(int id)
        {
            users professor = db.users.Find(id);
            return View(professor);
        }

        // POST: projects/Delete/5
        [HttpPost, ActionName("Add_Del_Pro")]
        public ActionResult delete(int id)
        {
            users professor = db.users.Find(id);
            db.users.Remove(professor);
            db.SaveChanges();
            return RedirectToAction("showproff");
        }

        [HttpGet]
        public ActionResult AddProf2()
        {
            users prof = new users();
            return View(prof);
        }

        [HttpPost]
        public ActionResult AddProf2(users prof)
        {

            var username = db.users.Where(x => x.username == prof.username).ToList();
            if (ModelState.IsValid)
            {
                if (username.Count() == 0)
                {
                    prof.usertype = 3;
                    db.users.Add(prof);
                    db.SaveChanges();
                    return Json(new { result = 0 });
                }
            }
            return RedirectToAction("showproff");




        }

        public ActionResult ShowReport()
        {
            var profs = db.professors.ToList();
            var countprofs = profs.Count();
            List<string> prof_name = new List<string>();
            List<int> count_orders = new List<int>();
            foreach(professors m in profs)
            {
                var prof_orders = db.orders.Where(x => x.professor_id == m.Id && x.status == 1).ToList();
                prof_name.Add(m.users.fullname);
                count_orders.Add(prof_orders.Count());
            }
            ViewBag.prof_name = prof_name;
            ViewBag.count_orders = count_orders;
            ViewBag.countprofs = countprofs;
            return View();
        }

    }
}