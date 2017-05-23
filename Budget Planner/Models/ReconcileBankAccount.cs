using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budget_Planner.Models
{
    public class ReconcileBankAccount
    {
        public BankAccount Account { get; set; }

        public double ReconciledBalance { get; set; }
    }
}