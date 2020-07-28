﻿using gs_blog_cf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace gs_blog_cf.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            // var publishedBlogPosts = db.BlogPosts.Where(b => b.Published).ToList();
            var publishedBlogPosts = db.BlogPosts.Where(b => b.Published).OrderByDescending(createdDate => createdDate.Created).Take(5).ToList();
            return View(publishedBlogPosts);
        }

        public ActionResult Archive()
        {
            var publishedBlogPosts = db.BlogPosts.Where(b => b.Published).OrderByDescending(createdDate => createdDate.Created).ToList();
            return View(publishedBlogPosts);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}