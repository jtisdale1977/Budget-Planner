using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Budget_Planner.Models
{
    public class Transaction
    {
        public Transaction() { }

        public Transaction(BankAccount bankaccount, Category category, TransactionType transactionType, ApplicationUser enteredBy, double amount)
        {
            BankAccounts = bankaccount;
            Categories = category;
            TransactionType = transactionType;
            EnteredBy = enteredBy;
            Amount = amount;
        }

        public int Id { get; set; }

        public int BankAccountsId { get; set; }

        [AllowHtml]
        [StringLength(250, ErrorMessage = "The transaction description cannot be longer than 250 characters.")]
        public string Notes { get; set; }

        public DateTimeOffset Date { get; set; }

        public string TransactionDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public double Amount { get; set; }

        public int TransactionTypeId { get; set; }

        public int CategoryId { get; set; }

        //userId
        public string EnteredById { get; set; }

        [DisplayName("Creation Date")]
        public DateTimeOffset Created { get; set; }

        [DisplayName("Updated Date")]
        public DateTimeOffset Updated { get; set; }

        [DisplayName("Modified By")]
        public string UpdatedBy { get; set; }

        public bool Reconciled { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public double ReconcileAmount { get; set; }

        public virtual Category Categories { get; set; }

        public virtual Budget Budgets { get; set; }

        //userId
        public virtual ApplicationUser EnteredBy { get; set; }

        public virtual BankAccount BankAccounts { get; set; }

        public virtual TransactionType TransactionType { get; set; }
    }
}