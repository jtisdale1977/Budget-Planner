using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budget_Planner.Models
{
    public class TransactionType
    {
        public TransactionType()
        {
            Transactions = new HashSet<Transaction>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}