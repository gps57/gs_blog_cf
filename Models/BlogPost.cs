using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace gs_blog_cf.Models
{
    public class BlogPost
    {
        

        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }

        [AllowHtml]
        public string Body { get; set; }

        public string Abstract { get; set; }

        // I'm using MediaPath instead of MediaURL as the name of this property.
        public string MediaPath { get; set; }
        public bool Published { get; set; }

        // Navigational properties
        public virtual ICollection<Comment> Comments { get; set; }

        // constructor
        public BlogPost()
        {
            Comments = new HashSet<Comment>();
        }
    }
}