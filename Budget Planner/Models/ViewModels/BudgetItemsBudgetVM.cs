using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Budget_Planner.Models.ViewModels
{
    public class BudgetItemsBudgetVM
    {
        public int CategoryId { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "Budget Item amount must be greater than 0.")]
        public double Amount { get; set; }

        [AllowHtml]
        [DisplayName("Budget Item Notes")]
        public string Notes { get; set; }

        public virtual BudgetItem BudgetItems { get; set; }
    }
}