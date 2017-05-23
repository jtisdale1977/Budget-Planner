using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Budget_Planner.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public int HouseholdId { get; set; }

        [DisplayName("Posted By")]
        public string AuthorId { get; set; }

        [AllowHtml]
        public string Body { get; set; }

        [DisplayName("Creation Date")]
        public DateTimeOffset Created { get; set; }

        [DisplayName("Modified Date")]
        public DateTimeOffset Updated { get; set; }

        [AllowHtml]
        public string UpdateReason { get; set; }

        public virtual ApplicationUser Members { get; set; }

        public virtual Household Households { get; set; }
    }
}