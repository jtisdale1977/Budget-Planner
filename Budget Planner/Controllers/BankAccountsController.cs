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

namespace Budget_Planner.Controllers
{
    [Authorize]
    public class BankAccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private Transaction tr = new Transaction();
        private Budget bg = new Budget();
        private Category cat = new Category();

        // GET: Accounts
        [Authorize]
        public ActionResult Index()
        {
            try
            {
                var userHouseholdId = FindUserHouseholdId();
                var bankaccounts = db.BankAccounts.Where(u => u.HouseholdId == userHouseholdId).ToList();
                Household household = db.Households.Find(userHouseholdId);

                if (household == null)
                {
                    return RedirectToAction("Create", "Households");
                }

                ViewBag.BAHouseholdName = db.Households.FirstOrDefault(u => u.Id == userHouseholdId).Name;
                return View(bankaccounts.OrderByDescending(a => a.Created));
            }
            catch (Exception)
            {
                return RedirectToAction("page404", "Home");
            }
        }

        // GET: Accounts/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                var userHouseholdId = FindUserHouseholdId();
                BankAccount bankaccount = db.BankAccounts.Single(a => a.Id == id && a.HouseholdId == userHouseholdId);

                if (bankaccount == null)
                {
                    return RedirectToAction("Create", "BankAccounts");
                }
                return View(bankaccount);
            }
            catch (Exception)
            {
                return RedirectToAction("page404", "Home");
            }
        }

        // GET: Accounts/Create
        [Authorize]
        public ActionResult Create(string bankAccountName)
        {
            // check to make sure accountName is an actual string!

            var bankAccount = new BankAccount();
            var userHouseholdId = FindUserHouseholdId();

            if (bankAccountName != null && bankAccountName.Trim() != "")
            {
                bankAccount.Name = bankAccountName.Trim();
            }

            bankAccount.HouseholdId = userHouseholdId;
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name");

            return View(bankAccount);
        }
        //public ActionResult Create()
        //{
        //    ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name");
        //    return View();
        //}

        // POST: Accounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HouseholdId,Name,Notes,Created,Balance,EnteredBy")] BankAccount bankaccount)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var userHouseholdId = FindUserHouseholdId();

            if (ModelState.IsValid)
            {
                bankaccount.Created = new DateTimeOffset(DateTime.Now);
                bankaccount.EnteredBy = user.FirstName + " " + user.LastName;
                //bankaccount.HouseholdId = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name).HouseholdId.Value;
                //bankaccount.ReconcileBalance = bankaccount.Balance;
                db.BankAccounts.Add(bankaccount);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", bankaccount.HouseholdId);
            return View(bankaccount);
        }

        // GET: Accounts/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                var userHouseholdId = FindUserHouseholdId();
                BankAccount bankaccount = db.BankAccounts.Single(a => a.Id == id && a.HouseholdId == userHouseholdId);

                if (bankaccount == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    //return RedirectToAction("page404", "Home");
                }

                ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", bankaccount.HouseholdId);
                return View(bankaccount);
            }
            catch (Exception)
            {
                return RedirectToAction("page404", "Home");
            }
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,HouseholdId,Created,Updated,EnteredBy,UpdateBy,Name,Notes,Balance,ReconcileBalance")] BankAccount bankaccount)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var userHouseholdId = FindUserHouseholdId();

            if (ModelState.IsValid)
            {
                bankaccount.Updated = DateTimeOffset.Now;
                bankaccount.UpdatedBy = user.FirstName + user.LastName;
                db.Entry(bankaccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", bankaccount.HouseholdId);
            return View(bankaccount);
        }

        // GET: Accounts/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                var userHouseholdId = FindUserHouseholdId();
                BankAccount bankaccount = db.BankAccounts.Single(a => a.Id == id && a.HouseholdId == userHouseholdId);

                if (bankaccount == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                return View(bankaccount);
            }

            catch (Exception)
            {
                return RedirectToAction("page404", "Home");
            }
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var userHouseholdId = FindUserHouseholdId();
                BankAccount bankaccount = db.BankAccounts.Single(a => a.Id == id && a.HouseholdId == userHouseholdId);
                db.BankAccounts.Remove(bankaccount);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return RedirectToAction("page404", "Home");
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

        private int FindUserHouseholdId()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var userHouseholdId = user.Households.Id;

            return userHouseholdId;
        }
    }
}

