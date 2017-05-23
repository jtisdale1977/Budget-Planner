using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Budget_Planner.Models
{
    public class BankAccount
    {
        public BankAccount()
        {
            Transactions = new HashSet<Transaction>();
        }

        public BankAccount(string name, double balance)
        {
            Name = name;
            Balance = balance;
        }

        public int Id { get; set; }

        public int? HouseholdId { get; set; }

        [Required(ErrorMessage = "An account name is required.")]
        [Display(Name = "Account Name")]
        public string Name { get; set; }

        [DisplayName("Creation Date")]
        public DateTimeOffset Created { get; set; }

        [DisplayName("Updated Date")]
        public DateTimeOffset Updated { get; set; }

        [DisplayName("Entered By")]
        public string EnteredBy { get; set; }

        [DisplayName("Modified By")]
        public string UpdatedBy { get; set; }

        [AllowHtml]
        [StringLength(100, ErrorMessage = "The transaction description cannot be longer than 100 characters.")]
        public string Notes { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [Range(0, double.MaxValue, ErrorMessage = "Bank Account amount must be greater than 0.")]
        public double Balance { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double ReconcileBalance { get; set; }

        public bool IsSoftDeleted { get; set; }

        public virtual Household Households { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}