using gs_blog_cf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gs_blog_cf.ViewModels
{
    public class BlogPostsDetailsVM
    {
        public BlogPost FeaturedPost { get; set; }

        // could/should we have "related" posts rather than "recent" posts?
        public ICollection<BlogPost> RecentPosts { get; set; }

        public BlogPostsDetailsVM()
        {
            RecentPosts = new HashSet<BlogPost>();
        }
    }
}