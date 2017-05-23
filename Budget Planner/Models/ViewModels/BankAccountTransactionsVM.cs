using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budget_Planner.Models.ViewModels
{
    public class BankAccountTransactionsVM
    {
        public BankAccountTransactionsVM()
        {
            Transactions = new HashSet<Transaction>();
        }

        public virtual BankAccount BankAccounts { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}