using Budget_Planner.Helpers;
using Budget_Planner.Models;
using Budget_Planner.Models.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Budget_Planner.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper urh = new UserRolesHelper();
        private Budget bg = new Budget();
        private Category cat = new Category();
        private BankAccount ba = new BankAccount();
        private Invitation iv = new Invitation();
        private Transaction tr = new Transaction();


        public ActionResult IndexLand()
        {
            return View();
        }

        [Authorize]
        public ActionResult Index(int? SelectedBudgetId)
        {
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    var user = db.Users.Find(User.Identity.GetUserId());

                    if (user.HouseholdId == null)
                    {
                        return RedirectToAction("Create", "Households");
                    }

                    DashboardVM dashboardVm = new DashboardVM();
                    dashboardVm.Members = user;
                    var userHouseholdId = user.HouseholdId;

                    dashboardVm.HouseHold = db.Households.Single(h => h.Id == userHouseholdId);
                    dashboardVm.BankAccounts = db.BankAccounts.Where(a => a.HouseholdId == userHouseholdId).ToList();

                    var listOfAccountIds = dashboardVm.BankAccounts.Select(t => t.Id).ToList();

                    dashboardVm.Transactions = db.Transactions.Where(t => listOfAccountIds.Contains(t.BankAccountsId)).OrderByDescending(t => t.Date).ToList();

                    dashboardVm.HouseholdExpenses = dashboardVm.Transactions.Where(t => t.TransactionType.Name == "Withdrawal").Sum(t => t.Amount);
                    dashboardVm.HouseholdIncome = dashboardVm.Transactions.Where(t => t.TransactionType.Name == "Deposit").Sum(t => t.Amount);

                    var todaysDate = DateTime.Now;
                    DateTime firstDayOfTodaysMonth = new DateTime(todaysDate.Year, todaysDate.Month, 1);

                    dashboardVm.WithdrawalsForCurrentMonth = dashboardVm.Transactions.Where(
                                                                     t => t.TransactionType.Name == "Withdrawal"
                                                                     && (t.Date >= firstDayOfTodaysMonth && t.Date <= todaysDate)).ToList();   // gets transactions from beginning of this month up until today

                    if (SelectedBudgetId != null)
                    {
                        dashboardVm.Budgets = db.Budgets.FirstOrDefault(b => b.HouseholdId == user.HouseholdId && b.Id == SelectedBudgetId);
                    }

                    else
                    {
                        dashboardVm.Budgets = db.Budgets.FirstOrDefault(b => b.HouseholdId == user.HouseholdId);
                    }

                    ViewBag.SelectedBudgetId = new SelectList(db.Budgets.Where(b => b.HouseholdId == userHouseholdId), "Id", "Name");
                    return View(dashboardVm);
                }

                catch (Exception)
                {
                    return RedirectToAction("page404", "Home");
                }
            }

            else
            {
                // user doesn't have household id, make them register and create one there (create new household or join one)
                return RedirectToAction("Login", "Account");
            }
        }

        [Authorize]
        public ActionResult IndexHOH()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            Household household = db.Households.Find(user.HouseholdId);
            return View(household);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}