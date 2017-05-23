using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budget_Planner.Models.ViewModels
{
    public class DashboardVM
    {
        public DashboardVM()
        {
            BankAccounts = new HashSet<BankAccount>();
            Transactions = new HashSet<Transaction>();
            Comments = new HashSet<Comment>();
        }

        public double HouseholdIncome { get; set; }
        public double HouseholdExpenses { get; set; }
        
        public virtual Household HouseHold { get; set; }
        public virtual ApplicationUser Members { get; set; }
        public virtual Budget Budgets { get; set; }
        public virtual ICollection<BankAccount> BankAccounts { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<Transaction> WithdrawalsForCurrentMonth { get; set; }
    }
}