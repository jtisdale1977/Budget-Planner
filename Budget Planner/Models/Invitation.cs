using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budget_Planner.Models
{
    public class Invitation
    {
        public int Id { get; set; }

        public int HouseholdId { get; set; }

        [Required(ErrorMessage = "Please provide an email address for the invited party.")]
        [EmailAddress]
        public string Email { get; set; }

        public Guid InviteCode { get; set; }

        public string MemberId { get; set; }

        public bool Accepted { get; set; }

        public DateTimeOffset Generated { get; set; }

        public virtual Household Households { get; set; }

        public virtual ApplicationUser Members { get; set; }
    }
}