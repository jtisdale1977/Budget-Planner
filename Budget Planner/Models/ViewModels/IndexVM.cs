using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budget_Planner.Models
{
    public class IndexVM
    {
        public List<Household> Households { get; set; }
        public List<Transaction> Transcations { get; set; }
        public List<BudgetItem> Budgets { get; set; }

        public IndexVM()
        {
            Households = new List<Household>();
            Transcations = new List<Transaction>();
            Budgets = new List<BudgetItem>();
        }
    }
}