﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using gs_blog_cf.Helpers;
using gs_blog_cf.Models;
using gs_blog_cf.ViewModels;
using PagedList;
using PagedList.Mvc;

namespace gs_blog_cf.Controllers
{
    public class BlogPostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BlogPosts
        public ActionResult Index(int? page, string searchStr)
        {
            ViewBag.Search = searchStr;
            var blogList = IndexSearch(searchStr);

            int pageSize = 10; // Specifies the number of posts per page
            int pageNumber = (page ?? 1); // ?? null coalescing operator - if a page is given, use it, otherwise, use 1

            var model = blogList.ToPagedList(pageNumber, pageSize);

            //var model = db.BlogPosts.OrderByDescending(b => b.Created).ToPagedList(pageNumber, pageSize);
            return View(model);
        }

        // GET: Published Blogs
        public ActionResult Published(int? page, string searchStr)
        {
            ViewBag.Search = searchStr;
            var blogList = IndexSearch(searchStr);

            int pageSize = 10; // Specifies the number of posts per page
            int pageNumber = (page ?? 1); // ?? null coalescing operator - if a page is given, use it, otherwise, use 1

            var model = blogList.Where(b => b.Published).ToPagedList(pageNumber, pageSize);

            //var model = db.BlogPosts.OrderByDescending(b => b.Created).ToPagedList(pageNumber, pageSize);
            return View(model);
        }

        public IQueryable<BlogPost> IndexSearch(string searchStr)
        {
            IQueryable<BlogPost> result = null;
            if(searchStr != null)
            {
                result = db.BlogPosts.AsQueryable();
                result = result.Where(b => b.Title.Contains(searchStr) ||
                                           b.Body.Contains(searchStr) ||
                                           b.Comments.Any(c => c.Body.Contains(searchStr) ||
                                           c.Author.FirstName.Contains(searchStr) ||
                                           c.Author.LastName.Contains(searchStr) ||
                                           c.Author.DisplayName.Contains(searchStr) ||
                                           c.Author.Email.Contains(searchStr)));
            }
            else
            {
                result = db.BlogPosts.AsQueryable();
            }

            return result.OrderByDescending(p => p.Created);
        }

        // GET: BlogPosts/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    BlogPost blogPost = db.BlogPosts.Find(id);

        //    if (blogPost == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    return View(blogPost);
        //}

        // GET: BlogPosts/Details/slug
        public ActionResult Details(string slug)
        {
            var detailsPage = new BlogPostsDetailsVM();

            if (String.IsNullOrWhiteSpace(slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            detailsPage.FeaturedPost = db.BlogPosts.FirstOrDefault(b => b.Slug == slug);

            if(detailsPage.FeaturedPost == null)
            {
                return HttpNotFound();
            }

            // TODO: come back to this and make sure the Featured Blog is not in this list.
            detailsPage.RecentPosts = db.BlogPosts.Where(b => b.Published).OrderByDescending(createdDate => createdDate.Created).Take(5).ToList();

            return View(detailsPage);
        }

        // GET: BlogPosts/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from over posting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "Id,Title,Created,Body,Abstract,Published")] BlogPost blogPost, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                var slug = StringUtilities.URLFriendly(blogPost.Title);

                // asks the question as to whether or not the slug is blank
                if (String.IsNullOrWhiteSpace(slug))
                {
                    ModelState.AddModelError("Title", "Invalid Title. Try Again!");
                    return View(blogPost);
                }
                
                // determines whether or not this slug has already been recorded
                if (db.BlogPosts.Any(b => b.Slug == slug))
                {
                    ModelState.AddModelError("Title", "The title appears to have been used before. It must be unique.");
                    return View(blogPost);
                }

                blogPost.Slug = slug;

                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    blogPost.MediaPath = "/Uploads/" + fileName;
                }

                blogPost.Created = DateTime.Now;

                db.BlogPosts.Add(blogPost);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Created,Title,Slug,Body,Abstract,MediaPath,Published")] BlogPost blogPost, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                var slug = StringUtilities.URLFriendly(blogPost.Title);

                // determine whether or not the title has changed by comparing the old and the new slug\
                if (slug != blogPost.Slug)
                {
                    // asks the question as to whether or not the slug is blank
                    if (String.IsNullOrWhiteSpace(slug))
                    {
                        ModelState.AddModelError("Title", "Invalid Title. Try Again!");
                        return View(blogPost);
                    }

                    // determines whether or not this slug has already been recorded
                    if (db.BlogPosts.Any(b => b.Slug == slug))
                    {
                        ModelState.AddModelError("Title", "The title appears to have been used before. It must be unique.");
                        return View(blogPost);
                    }

                    blogPost.Slug = slug;
                }

                // TODO: need to determine in they changed the image during their edits.
                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    blogPost.MediaPath = "/Uploads/" + fileName;
                }

                blogPost.Updated = DateTime.Now;
                db.Entry(blogPost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blogPost);
        }

        [Authorize(Roles = "Admin")]
        // GET: BlogPosts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BlogPost blogPost = db.BlogPosts.Find(id);
            db.BlogPosts.Remove(blogPost);
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
