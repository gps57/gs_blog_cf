using gs_blog_cf.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
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
            EmailModel model = new EmailModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(EmailModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var body = "<p>Email From: <bold>{0}</bold>({1})</p><p>Message:</p><p>{2}</p>";
                    var from = "MyPortfolio<example@email.com>";
                    model.Body = "This is a message from your blog site.  The name and the email of the contacting person is above.";

                    var sendTo = ConfigurationManager.AppSettings["emailto"];

                    var email = new MailMessage(from, sendTo)
                    {
                        Subject = "Blog Contact Message",
                        //Body = "A test string for the body of the email.",
                        Body = string.Format(body, model.FromName, model.FromEmail, model.Body),
                        IsBodyHtml = true
                    };

                    var svc = new EmailService();
                    await svc.SendAsync(email);

                    return View(new EmailModel());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                }
            }
            return View(model);
        }
    }
}