using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Budget_Planner.Models
{
    public class BudgetItem
    {
        public BudgetItem()
        {
            Amount = 0;
        }

        public BudgetItem(string categoryName, double amount)
        {
            Categories = new Category();
            Categories.Name = categoryName;
            Amount = amount;
        }

        public int Id { get; set; }

        public int BudgetId { get; set; }

        public int CategoryId { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Range(0.0, double.MaxValue, ErrorMessage = "Budget Item amount must be greater than 0.")]
        public double Amount { get; set; }

        [AllowHtml]
        [DisplayName("Budget Item Notes")]
        public string Notes { get; set; }

        [DisplayName("Creation Date")]
        public DateTimeOffset Created { get; set; }

        [DisplayName("Modified Date")]
        public DateTimeOffset Updated { get; set; }

        [DisplayName("Entered By")]
        public string EnteredBy { get; set; }

        [DisplayName("Modified By")]
        public string UpdatedBy { get; set; }

        public virtual Budget Budgets { get; set; }

        public virtual Category Categories { get; set; }
    }
}