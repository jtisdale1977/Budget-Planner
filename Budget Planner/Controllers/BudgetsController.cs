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
    public class BudgetsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private Category cat = new Category();
        private Transaction tr = new Transaction();

        // GET: Budgets
        [Authorize]
        public ActionResult Index()
        {
            try
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                var budgets = db.Budgets.Where(b => b.HouseholdId == user.HouseholdId).ToList();

                if (user.Households == null)
                {
                    return RedirectToAction("Create", "Households");
                }

                return View(budgets);
            }
            catch (Exception)
            {
                return RedirectToAction("page404", "Home");
            }
        }

        // GET: Budgets/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return RedirectToAction("ForBudget", "BudgetItems", new { id = id });
        }

        // GET: Budgets/Create
        [Authorize]
        public ActionResult Create(string BudgetName)
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            if (user.Households == null)
            {
                return RedirectToAction("Create", "Households");
            }

            if (user.Households.BankAccounts.Count() == 0)
            {
                return RedirectToAction("Create", "BankAccounts");
            }

            Budget budget = new Budget();

            if (BudgetName != null && BudgetName.Trim() != "")
            {
                budget.Name = BudgetName.Trim();
            }

            budget.HouseholdId = (int)user.HouseholdId;

            var getHouse = db.Households.Where(u => user.HouseholdId == u.Id).ToList();
            ViewBag.HouseholdId = new SelectList(getHouse, "Id", "Name");
            return View(budget);
        }

        // POST: Budgets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Notes,Created,EnteredBy,HouseholdId")] Budget budget)
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            if (user.Households.BankAccounts.Count() == 0)
            {
                return RedirectToAction("Create", "BankAccounts");
            }

            if (user.Households == null)
            {
                return RedirectToAction("Create", "Households");
            }

            if (ModelState.IsValid)
            {
                budget.Created = DateTimeOffset.Now;
                budget.EnteredBy = user.FirstName + " " + user.LastName;
                db.Budgets.Add(budget);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", budget.HouseholdId);
            return View(budget);
        }

        // GET: Budgets/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                Budget budget = db.Budgets.Single(b => b.Id == id && b.HouseholdId == user.HouseholdId);
                if (user.Households == null)
                {
                    return RedirectToAction("Create", "Households");
                }
                ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", budget.HouseholdId);
                return View(budget);
            }

            catch (Exception)
            {
                return RedirectToAction("page404", "Home");
            }
        }

        // POST: Budgets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Notes,Created,Updated,EnteredBy,UpdatedBy,HouseholdId")] Budget budget)
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            if (user.Households == null)
            {
                return RedirectToAction("Create", "Households");
            }

            if (ModelState.IsValid)
            {
                budget.Updated = DateTimeOffset.Now;
                budget.UpdatedBy = user.FirstName + " " + user.LastName;
                db.Entry(budget).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", budget.HouseholdId);
            return View(budget);
        }

        // GET: Budgets/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                Budget budget = db.Budgets.Single(b => b.Id == id && b.HouseholdId == user.HouseholdId);

                if (user.Households == null)
                {
                    return RedirectToAction("Create", "Households");
                }

                return View(budget);
            }
            catch (Exception)
            {
                return RedirectToAction("page404", "Home");
            }
        }

        // POST: Budgets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var user = db.Users.Find(User.Identity.GetUserId());

                if (user.Households == null)
                {
                    return RedirectToAction("Create", "Households");
                }

                Budget budget = db.Budgets.Single(b => b.Id == id && b.HouseholdId == user.HouseholdId);
                db.Budgets.Remove(budget);
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
    }
}