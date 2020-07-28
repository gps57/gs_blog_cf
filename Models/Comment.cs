using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gs_blog_cf.Models
{
    public class Comment
    {
        // Create a property that acts as a unique identifier for any single comment
        public int Id { get; set; }

        // Foreign Key (FK) This is how I refer to the Primary Key of the BlogPost I am attached to
        public int BlogPostId { get; set; }

        // Foreign Key (FK) This is how I refer to the user who wrote the comment
        // Entity Framework (EF) has decided for us that a string will be used for the Id of a PK
        public string AuthorId { get; set; }

        // this is how I record and store the content of my comment
        public string Body { get; set; }

        // if i wanted to record how many hours behind or ahead of the Prime Meridian I can use DateTimeOffset class
        // public DateTimeOffset Created { get; set; }
        public DateTime Created { get; set; }

        // If the comment has not been updated, it won't need a Updated value.  That's what the '?' character
        // is for - to tell the code that this parameter is not required.  The value can be 'null'.
        public DateTime? Updated { get; set; }

        // strings are "nullable" by default.
        // This property will likely only be used by a Moderator, or possibly by an Admin
        public string UpdatedReason { get; set; }

        // Our Navigational properties

        // how can I inform this comment that it is tightly coupled with it's parent of Author?
        // in other words how to I denote the parent of this comment and Then subsequently query it?
        // by adding the "virtual" property
        public virtual ApplicationUser Author { get; set; }

        // how do I tell the Comment who its Parent BlogPost is?
        public virtual BlogPost BlogPost { get; set; }
    }
}