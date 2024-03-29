﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
//using System.Web.Security;
using graduation_project_ideas_2.Models;

namespace graduation_project_ideas_2.Controllers
{
    public class usersController : Controller
    {
        private IA_DBEntities3 db = new IA_DBEntities3();

        // GET: users
        public ActionResult Index()
        {
            return View(db.users.ToList());
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(users user)
        {
            
            foreach (var obj in db.users)
            {
                if (obj.username.Equals(user.username) && obj.password.Equals(user.password))
                {
                    foreach (var item in db.ROLE)
                    {
                        if (item.type == obj.usertype)
                        {
                            Session["UserID"] = obj.Id.ToString();
                            Session["UserName"] = obj.username.ToString();
                            //ModelState.Clear();
                            return RedirectToAction("Index", item.name);

                        }
                    }
                }
            }
            
            return View(user);

        }

        //public ActionResult LogOut()
        //{
        //    FormsAuthentication.SignOut();
        //    //Session.Abandon();
        //    return RedirectToAction("Index", "Home");
        //}

        public ActionResult Logout()
        {
            HttpContext.Session.RemoveAll();//This will remove all session from application
            FormsAuthentication.SignOut();
            return Redirect("login");
        }

        // GET: users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            users users = db.users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // GET: users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,fullname,username,email,password,usertype")] users users,HttpPostedFileBase std_trans)
        {

            
            if (ModelState.IsValid)
            {
                var fileName = Path.GetFileName(std_trans.FileName);
                string path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                std_trans.SaveAs(path);
                string std_trans2 = fileName;

                teamleaders teams = new teamleaders {
                    std_gpa = double.Parse(Request["std_gpa"]),
                    std_skills = Request["std_skills"],
                    std_level = Request["std_level"],
                    std_trans = std_trans2,
                    user_id = users.Id

                };


                db.teamleaders.Add(teams);
                db.users.Add(users);
                db.SaveChanges();
                
                //ViewBag.pgname = "project";
                return RedirectToAction("Login");
            }

            return View(users);
        }

        // GET: users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            users users = db.users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,fullname,username,email,password,usertype")] users users)
        {
            if (ModelState.IsValid)
            {
                db.Entry(users).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(users);
        }

        // GET: users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            users users = db.users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            users users = db.users.Find(id);
            db.users.Remove(users);
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
