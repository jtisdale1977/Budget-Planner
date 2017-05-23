using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Budget_Planner.Models;
using Microsoft.AspNet.Identity;
using Budget_Planner.Models.ViewModels;

namespace Budget_Planner.Controllers
{
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private Category cat = new Category();
        private BankAccount ba = new BankAccount();

        // GET: Transactions
        [Authorize]
        public ActionResult Index()
        {
            try
            {
                var user = db.Users.Find(User.Identity.GetUserId());

                if (user.Households == null)
                {
                    return RedirectToAction("Create", "Households");
                }

                var transactions = db.Transactions.Where(t => t.BankAccounts.HouseholdId == user.HouseholdId).Include(t => t.Categories).OrderByDescending(t => t.Date).ToList();
                return View(transactions);
            }

            catch (Exception)
            {
                return RedirectToAction("page404", "Index");
            }
        }

        // GET: Transactions/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                var user = db.Users.Find(User.Identity.GetUserId());

                if (user.Households == null)
                {
                    return RedirectToAction("Create", "Households");
                }

                Transaction transaction = db.Transactions.Single(t => t.Id == id && t.BankAccounts.HouseholdId == user.HouseholdId);
                return View(transaction);
            }

            catch (Exception)
            {
                return RedirectToAction("page404", "Index");
            }

        }

        // GET: Transactions/Create
        [Authorize]
        public ActionResult Create()
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            if (user.Households == null)
            {
                return RedirectToAction("Create", "Household");
            }

            if (user.Households.BankAccounts.Count() == 0)
            {
                return RedirectToAction("Create", "BankAccount");
            }

            Transaction transactions = new Transaction();
            transactions.EnteredById = user.Id;
            var getAccount = db.BankAccounts.Where(u => user.HouseholdId == u.HouseholdId).ToList();

            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes.ToList(), "Id", "Name");
            ViewBag.TransactionType = new SelectList(db.TransactionTypes, "Id", "Name");
            ViewBag.BankAccount = new SelectList(db.BankAccounts, "Id", "Name");
            ViewBag.BankAccountsId = new SelectList(getAccount, "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            ViewBag.EnteredById = new SelectList(db.Users, "Id", "FirstName");

            return View(transactions);
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "BankAccountsId,Notes,Date,Amount,TransactionTypeId,CategoryId,EnteredById,Reconciled,ReconcileAmount")] Transaction transaction)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            if (user.Households == null)
            {
                return RedirectToAction("Create", "Household");
            }

            if (user.Households.BankAccounts == null)
            {
                return RedirectToAction("Create", "BankAccount");
            }

            if (ModelState.IsValid)
            {
                transaction.Date = DateTimeOffset.UtcNow;

                if (transaction.ReconcileAmount == transaction.Amount)
                {
                    transaction.Reconciled = true;
                }
                else
                {
                    transaction.Reconciled = false;
                }

                db.Transactions.Add(transaction);
                db.SaveChanges();

                // Change account balance based upon trans amount
                var bankaccount = db.BankAccounts.Find(transaction.BankAccountsId);
                var depositTransactionTypeId = GetTransasctionTypeId("Deposit");
                var withdrawalTransactionTypeId = GetTransasctionTypeId("Withdrawal");

                if (transaction.TransactionTypeId == depositTransactionTypeId)       // trans was deposit
                {
                    bankaccount.Balance += transaction.Amount;
                    bankaccount.ReconcileBalance += transaction.ReconcileAmount;
                }
                else if (transaction.TransactionTypeId == withdrawalTransactionTypeId)    // trans was withdrawal
                {
                    bankaccount.Balance -= transaction.Amount;
                    bankaccount.ReconcileBalance -= transaction.ReconcileAmount;
                }

                db.Entry(bankaccount).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index", "Transactions");
                //db.Transactions.Add(transaction);
                //db.SaveChanges();
                //return RedirectToAction("Index");
            }

            var getAccount = db.BankAccounts.Where(u => user.HouseholdId == u.HouseholdId).ToList();

            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes.ToList(), "Id", "Name");
            ViewBag.TransactionType = new SelectList(db.TransactionTypes, "Id", "Name", transaction.TransactionType);
            ViewBag.BankAccount = new SelectList(db.BankAccounts, "Id", "Name", transaction.BankAccounts);
            ViewBag.BankAccountsId = new SelectList(getAccount, "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            ViewBag.EnteredById = new SelectList(db.Users, "Id", "FirstName", transaction.EnteredById);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                if (user.Households == null)
                {
                    return RedirectToAction("Create", "Household");
                }

                Transaction transaction = db.Transactions.Find(id);
                transaction.EnteredById = user.Id;

                ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "Name", transaction.TransactionType);
                ViewBag.BankAccount = new SelectList(db.BankAccounts, "Id", "Name", transaction.BankAccounts);
                ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
                ViewBag.EnteredById = new SelectList(db.Users, "Id", "FirstName", transaction.EnteredById);
                TempData["PreviousTransaction"] = transaction;
                return View(transaction);
            }

            catch (Exception)
            {
                return RedirectToAction("page404", "Index");
            }
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AccountId,Description,Date,Amount,Type,CategoryId,EnteredById,Reconciled,ReconcileAmount")] Transaction transaction)
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            if (user.Households == null)
            {
                return RedirectToAction("Create", "Household");
            }

            if (ModelState.IsValid)
            {
                transaction.EnteredById = user.Id;

                var previousTransaction = (Transaction)TempData["PreviousTransaction"];

                // need to see if account balance needs adjusting ...
                var previousTransactionTypeId = previousTransaction.TransactionTypeId;
                var previousTransactionAmount = previousTransaction.Amount;
                var previousTransactionReconcileAmount = previousTransaction.ReconcileAmount;
                var account = db.BankAccounts.FirstOrDefault(a => a.Id == transaction.BankAccountsId);

                if (transaction.ReconcileAmount == transaction.Amount)
                {
                    transaction.Reconciled = true;
                }
                else
                {
                    transaction.Reconciled = false;
                }

                if (previousTransactionTypeId == transaction.TransactionTypeId)
                {
                    // type of transaction stayed the same (still a deposit, still a withdrawal, etc...)
                    if (transaction.TransactionTypeId == GetTransasctionTypeId("Deposit"))
                    {
                        // transaction was a deposit that is still a deposit
                        if (transaction.Amount != previousTransactionAmount)
                        {
                            // transaction amount for a deposit was changed
                            account.Balance += (transaction.Amount - previousTransactionAmount);
                        }

                        if (transaction.ReconcileAmount != previousTransactionReconcileAmount)
                        {
                            // transaction reconciled amount for a deposit was changed
                            account.ReconcileBalance += (transaction.ReconcileAmount - previousTransactionReconcileAmount);
                        }
                    }
                    else if (transaction.TransactionTypeId == GetTransasctionTypeId("Withdrawal"))
                    {
                        // transaction was a withdrawal that is still a withdrawal
                        if (transaction.Amount != previousTransactionAmount)
                        {
                            // transaction amount for a withdrawal was changed
                            account.Balance -= (transaction.Amount - previousTransactionAmount);
                        }

                        if (transaction.ReconcileAmount != previousTransactionReconcileAmount)
                        {
                            // transaction reconciled amount for a withdrawal was changed
                            account.ReconcileBalance -= (transaction.ReconcileAmount - previousTransactionReconcileAmount);
                        }
                    }
                }
                else
                {
                    // type of transaction changed (went from deposit to withdrawal or went from withdrawal to deposit, for example)
                    if (previousTransactionTypeId == GetTransasctionTypeId("Deposit"))
                    {
                        // transaction was previously a deposit that has since changed
                        if (transaction.TransactionTypeId == GetTransasctionTypeId("Withdrawal"))
                        {
                            // transaction was changed from a deposit to a withdrawal
                            if (transaction.Amount != previousTransactionAmount)
                            {
                                // transaction went from deposit to withdrawal and the amount was changed
                                account.Balance -= (previousTransactionAmount + transaction.Amount);
                            }
                            else
                            {
                                // transaction went from deposit to withdrawal and the amount did not change
                                account.Balance -= (previousTransactionAmount * 2);
                            }

                            if (transaction.ReconcileAmount != previousTransactionReconcileAmount)
                            {
                                // transaction went from deposit to withdrawal and the reconciled amount was changed
                                account.ReconcileBalance -= (previousTransactionReconcileAmount + transaction.ReconcileAmount);
                            }
                            else
                            {
                                // transaction went from deposit to withdrawal and the reconciled amount did not change
                                account.ReconcileBalance -= (previousTransactionReconcileAmount * 2);
                            }
                        }
                    }
                    else if (previousTransactionTypeId == GetTransasctionTypeId("Withdrawal"))
                    {
                        // transaction was previously a withdrawal that has now changed
                        if (transaction.TransactionTypeId == GetTransasctionTypeId("Deposit"))
                        {
                            // transaction was changed from a withdrawal to a deposit
                            if (transaction.Amount != previousTransactionAmount)
                            {
                                // transaction went from withdrawal to a deposit and the amount changed
                                account.Balance += (previousTransactionAmount + transaction.Amount);
                            }
                            else
                            {
                                // transaction went from withdrawal to a deposit and the amount did not change
                                account.Balance += (previousTransactionAmount * 2);
                            }

                            if (transaction.ReconcileAmount != previousTransactionReconcileAmount)
                            {
                                // transaction went from withdrawal to a deposit and the reconciled amount changed
                                account.ReconcileBalance += (previousTransactionReconcileAmount + transaction.ReconcileAmount);
                            }
                            else
                            {
                                // transaction went from withdrawal to a deposit and the reconciled amount did not change
                                account.ReconcileBalance += (previousTransactionReconcileAmount * 2);
                            }
                        }
                    }
                }
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var getAccount = db.BankAccounts.Where(u => user.HouseholdId == u.HouseholdId).ToList();

            ViewBag.BankAccountsId = new SelectList(getAccount, "Id", "Name");
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "Name", transaction.TransactionType);
            ViewBag.BankAccount = new SelectList(db.BankAccounts, "Id", "Name", transaction.BankAccounts);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            ViewBag.EnteredById = new SelectList(db.Users, "Id", "FirstName", transaction.EnteredById);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                var user = db.Users.Find(User.Identity.GetUserId());

                if (user.Households == null)
                {
                    return RedirectToAction("Create", "Household");
                }

                Transaction transaction = db.Transactions.Single(t => t.Id == id && t.BankAccounts.HouseholdId == user.HouseholdId);

                return View(transaction);
            }

            catch (Exception)
            {
                return RedirectToAction("page404", "Index");
            }
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var user = db.Users.Find(User.Identity.GetUserId());

                if (user.Households == null)
                {
                    return RedirectToAction("Create", "Household");
                }

                Transaction transaction = db.Transactions.Single(t => t.Id == id && t.BankAccounts.HouseholdId == user.HouseholdId);
                var bankAccount = db.BankAccounts.Find(transaction.BankAccountsId);

                if (transaction.TransactionTypeId == GetTransasctionTypeId("Deposit"))
                {
                    bankAccount.Balance -= transaction.Amount;
                    bankAccount.ReconcileBalance -= transaction.ReconcileAmount;
                }

                else if (transaction.TransactionTypeId == GetTransasctionTypeId("Withdrawal"))
                {
                    bankAccount.Balance += transaction.Amount;
                    bankAccount.ReconcileBalance += transaction.ReconcileAmount;
                }

                db.Transactions.Remove(transaction);
                db.Entry(bankAccount).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            catch (Exception)
            {
                return RedirectToAction("page404", "Index");
            }

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public ActionResult ForAccount(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                var user = db.Users.Find(User.Identity.GetUserId());

                if (user.Households == null)
                {
                    return RedirectToAction("Create", "Households");
                }

                BankAccountTransactionsVM baVM = new BankAccountTransactionsVM();
                var account = db.BankAccounts.Single(a => a.Id == id && a.HouseholdId == user.HouseholdId);
                baVM.BankAccounts = account;

                var transactions = db.Transactions.Where(
                   t => t.BankAccountsId == id &&
                   t.BankAccounts.HouseholdId == user.HouseholdId).ToList().OrderByDescending(t => t.Date);

                foreach (var transaction in transactions)
                {
                    baVM.Transactions.Add(transaction);
                }

                return View(baVM);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }


        private int FindUserHouseholdId()
        {
            var user = db.Users.FirstOrDefault(u => u.Email == User.Identity.Name);
            var userHouseholdId = user.Households.Id;

            return userHouseholdId;
        }


        private int GetTransasctionTypeId(string transactionTypeName)
        {
            var transactionTypeId = db.TransactionTypes.Single(t => t.Name == transactionTypeName).Id;

            return transactionTypeId;
        }
    }
}
