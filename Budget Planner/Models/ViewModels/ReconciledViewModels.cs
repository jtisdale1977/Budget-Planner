using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budget_Planner.Models
{
    public class DashboardViewModel
    {
        public IEnumerable<ReconcileBankAccount> ReconBankAccounts { get; set; }
        public IEnumerable<BudgetItem> BudgetList { get; set; }
    }

    public class ManageAccountsViewModel
    {
        public IEnumerable<ReconcileBankAccount> ReconBankAccounts { get; set; }
    }
}