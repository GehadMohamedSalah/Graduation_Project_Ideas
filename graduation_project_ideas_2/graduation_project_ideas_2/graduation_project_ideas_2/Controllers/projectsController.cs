using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using graduation_project_ideas_2.Models;

namespace graduation_project_ideas_2.Controllers
{
    public class projectsController : Controller
    {
        private IA_DBEntities3 db = new IA_DBEntities3();

        // GET: projects
        public ActionResult Index()
        {
            ViewBag.fullname = Session["username"];
            ViewBag.userid = Session["userid"];
            var projects = db.projects.Include(p => p.teamleaders);
            return View(projects.ToList());
        }

        // GET: projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            projects projects = db.projects.Find(id);
            if (projects == null)
            {
                return HttpNotFound();
            }
            return View(projects);
        }

        // GET: projects/Create
        public ActionResult Addmember()
        {
            ViewBag.fullname = Session["username"];
            ViewBag.userid = Session["userid"];
            ViewBag.teamleader_id = new SelectList(db.teamleaders, "Id", "std_skills");
            return View();
        }

        // POST: projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Addmember(members m , HttpPostedFileBase mem_trans)
        {
            //string btnName;

            if (ModelState.IsValid)
            {
                var fileName = Path.GetFileName(mem_trans.FileName);
                string path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                mem_trans.SaveAs(path);
                string std_trans2 = fileName;

                members teams = new members
                {
                    mem_gpa = double.Parse(Request["mem_gpa"]),
                    mem_skills = Request["mem_skills"],
                    mem_level = Request["mem_level"],
                    mem_trans = std_trans2,
                    teamleader_id = int.Parse(Session["userid"].ToString())

                };

                db.members.Add(teams);
                db.SaveChanges();
                ViewBag.btnName = "project";
                return RedirectToAction("AddMember","Student");
            }

            //ViewBag.teamleader_id = new SelectList(db.teamleaders, "Id", "std_skills", projects.teamleader_id);
            return View();
        }

        // GET: projects/Create
        public ActionResult AddProject()
        {
            ViewBag.fullname = Session["username"];
            ViewBag.userid = Session["userid"];
            ViewBag.teamleader_id = new SelectList(db.teamleaders, "Id", "std_skills");
            return View();
        }

        // POST: projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProject(members m, HttpPostedFileBase mem_trans)
        {
            //string btnName;

            if (ModelState.IsValid)
            {
          

                projects proj = new projects
                {
                    proj_name = Request["proj_name"],
                    proj_desc = Request["proj_desc"],
                    teamleader_id = int.Parse(Session["userid"].ToString())
                   

                };

                db.projects.Add(proj);
                db.SaveChanges();
                ViewBag.btnName = "project";
                return RedirectToAction("AddMember", "Student");
            }

            //ViewBag.teamleader_id = new SelectList(db.teamleaders, "Id", "std_skills", projects.teamleader_id);
            return View();
        }



        // GET: projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            projects projects = db.projects.Find(id);
            if (projects == null)
            {
                return HttpNotFound();
            }
            ViewBag.teamleader_id = new SelectList(db.teamleaders, "Id", "std_skills", projects.teamleader_id);
            return View(projects);
        }

        // POST: projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,proj_name,proj_desc,proj_tools,teamleader_id")] projects projects)
        {
            if (ModelState.IsValid)
            {
                db.Entry(projects).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.teamleader_id = new SelectList(db.teamleaders, "Id", "std_skills", projects.teamleader_id);
            return View(projects);
        }

        // GET: projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            projects projects = db.projects.Find(id);
            if (projects == null)
            {
                return HttpNotFound();
            }
            return View(projects);
        }

        // POST: projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            projects projects = db.projects.Find(id);
            db.projects.Remove(projects);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
