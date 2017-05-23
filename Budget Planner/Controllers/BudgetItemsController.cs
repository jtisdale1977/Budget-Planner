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
    [Authorize]
    public class BudgetItemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private Category cat = new Category();

        // GET: BudgetItems
        [Authorize]
        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var budgetItems = db.BudgetItems.Where(b => b.Budgets.HouseholdId == user.HouseholdId).ToList();
            Household household = db.Households.Find(user.HouseholdId);
            if (household == null)
            {
                return RedirectToAction("Create", "Households");
            }

            ViewBag.HouseholdName = household.Name;
            return View(budgetItems);
        }

        public ActionResult ForBudget(int? id)
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

                Budget budget = db.Budgets.Single(b => b.Id == id && b.HouseholdId == user.HouseholdId);
                ViewBag.BudgetName = budget.Name;

                return View(budget.BudgetItems.ToList());
            }

            catch (Exception)
            {
                return RedirectToAction("page404", "Home");
            }
        }

        // GET: BudgetItems/Details/5
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
                BudgetItem budgetItem = db.BudgetItems.Single(b => b.Id == id && b.Budgets.HouseholdId == user.HouseholdId);

                if (user.Households == null)
                {
                    return RedirectToAction("Create", "Households");
                }

                return View(budgetItem);
            }

            catch (Exception)
            {
                return RedirectToAction("page404", "Home");
            }
        }

        // GET: BudgetItems/Create
        [Authorize]
        public ActionResult Create()
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            if (user.Households == null)
            {
                return RedirectToAction("Create", "Households");
            }

            if (user.Households.BankAccounts.Count() == 0)
            {
                return RedirectToAction("Create", "Accounts");
            }
            else
            {
                if (user.Households.Budgets.Count() == 0)
                {
                    return RedirectToAction("Create", "Budgets");
                }
            }

            var budgetItemsBudgetVm = new BudgetItemsBudgetVM();

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            //ViewBag.CategoryId = new SelectList(db.Households.Single(u => u.Id == user.HouseholdId).Categories, "Id", "Name");
            //ViewBag.CategoryId = new SelectList(db.Budgets.Where(b => b.HouseholdId == user.HouseholdId), "Id", "Name");
            ViewBag.BudgetId = new SelectList(db.Budgets.Where(b => b.HouseholdId == user.HouseholdId), "Id", "Name");
            return View(budgetItemsBudgetVm);
        }

        // POST: BudgetItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BudgetId,CategoryId,Amount,EnteredBy,Created,Notes")] BudgetItem budgetItem)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());

                if (user.Households == null)
                {
                    return RedirectToAction("Create", "Households");
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        budgetItem.Created = DateTimeOffset.Now;
                        budgetItem.EnteredBy = user.FirstName + " " + user.LastName;
                        var budget = db.Budgets.Single(b => b.Id == budgetItem.BudgetId && b.HouseholdId == user.HouseholdId);
                        var incomingBudgetCategoryName = db.Categories.Find(budgetItem.CategoryId).Name;
                        var budgetIncomingBudgetItemBelongsTo = user.Households.Budgets.Single(b => b.Id == budgetItem.BudgetId);
                        var bib = budgetIncomingBudgetItemBelongsTo.BudgetItems.FirstOrDefault(bi => bi.Categories.Name == incomingBudgetCategoryName);

                        if (bib != null)
                        {
                            bib.Amount += budgetItem.Amount;
                            db.Entry(bib).State = EntityState.Modified;
                        }

                        else
                        {
                            db.BudgetItems.Add(budgetItem);
                        }

                        db.SaveChanges();
                    }

                    catch (Exception)
                    {
                        return RedirectToAction("page404", "Home");
                    }

                    return RedirectToAction("Index", "BudgetItems", new { id = budgetItem.BudgetId });
                }

                return RedirectToAction("Create");
            }

            ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "Name", budgetItem.BudgetId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", budgetItem.CategoryId);
            return View(budgetItem);
        }

        // GET: BudgetItems/Edit/5
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
                BudgetItem budgetItem = db.BudgetItems.Single(b => b.Id == id && b.Budgets.HouseholdId == user.HouseholdId);

                if (user.Households == null)
                {
                    return RedirectToAction("Create", "Households");
                }

                var getBudget = db.Budgets.Where(u => u.HouseholdId == user.HouseholdId).ToList();

                ViewBag.BudgetId = new SelectList(getBudget, "Id", "Name", budgetItem.BudgetId);
                ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", budgetItem.CategoryId);
                return View(budgetItem);
            }
            catch
            {
                return RedirectToAction("page404", "Home");
            }
        }

        // POST: BudgetItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BudgetId,CategoryId,Amount,Updated,UpdatedBy,Notes")] BudgetItem budgetItem)
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            if (user.Households == null)
            {
                return RedirectToAction("Create", "Households");
            }

            if (ModelState.IsValid)
            {
                budgetItem.Updated = DateTimeOffset.Now;
                budgetItem.UpdatedBy = user.FirstName + " " + user.LastName;
                db.Entry(budgetItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "BudgetItems", new { id = budgetItem.BudgetId });
            }

            ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "Name", budgetItem.BudgetId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", budgetItem.CategoryId);
            return View(budgetItem);
        }

        // GET: BudgetItems/Delete/5
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
                BudgetItem budgetItem = db.BudgetItems.Single(b => b.Id == id && b.Budgets.HouseholdId == user.HouseholdId);

                if (user.Households == null)
                {
                    return RedirectToAction("Create", "Households");
                }

                return View(budgetItem);
            }
            catch (Exception)
            {
                return RedirectToAction("page404", "Home");
            }
        }

        // POST: BudgetItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                BudgetItem budgetItem = db.BudgetItems.FirstOrDefault(b => b.Id == id);

                if (user.Households == null)
                {
                    return RedirectToAction("Create", "Households");
                }

                db.BudgetItems.Remove(budgetItem);
                db.SaveChanges();
                return RedirectToAction("Index", "BudgetItems", new { id = budgetItem.BudgetId });
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