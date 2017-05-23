using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Budget_Planner.Models;
using Budget_Planner.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Budget_Planner.Controllers
{
    [Authorize]
    public class HouseholdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper urh = new UserRolesHelper();
        private Budget bg = new Budget();
        private Category cat = new Category();
        private BankAccount ba = new BankAccount();
        private Invitation iv = new Invitation();
        private Transaction tr = new Transaction();


        // GET: Households
        [Authorize]
        public ActionResult Index()
        {

            var user = db.Users.Find(User.Identity.GetUserId());
            Household household = db.Households.Find(user.HouseholdId);
            //var comments = db.Comments.Where(c => c.HouseholdId == household.Id).ToList();

            if (household == null)
            {
                return RedirectToAction("Create", "Households");
            }

            return RedirectToAction("Details", new { id = user.HouseholdId });
        }

        // GET: Households/Details/5
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
                Household household = db.Households.Single(h => h.Id == id && h.Id == user.HouseholdId);
                if (household == null)
                {
                    return HttpNotFound();
                }
                return View(household);
            }

            catch (Exception)
            {
                return RedirectToAction("page404", "Home");
            }
        }

        // GET: Households/Create
        [Authorize]
        public ActionResult Create()
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            if (user.Households != null)
            {
                ViewBag.Message = "You can only be a member of one household.";
                return RedirectToAction("IndexLand", "Home");
            }
            return View();
        }

        // POST: Households/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name")] Household household)
        {
            if (ModelState.IsValid)
            {
                var roleStore = new RoleStore<IdentityRole>(db);
                var roleManager = new RoleManager<IdentityRole>(roleStore);
                var userStore = new UserStore<ApplicationUser>(db);
                var userManager = new UserManager<ApplicationUser>(userStore);

                var user = db.Users.Find(User.Identity.GetUserId());
                db.Households.Add(household);
                userManager.AddToRole(user.Id, "HOH");
                db.SaveChanges();
                user.HouseholdId = household.Id;

                household.BankAccounts.Add(new BankAccount
                {
                    Name = "Checking",
                    Created = new DateTimeOffset(DateTime.Now),
                    Balance = 0,
                    ReconcileBalance = 0,
                });
                household.BankAccounts.Add(new BankAccount
                {
                    Name = "Savings",
                    Created = new DateTimeOffset(DateTime.Now),
                    Balance = 0,
                    ReconcileBalance = 0,
                });

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(household);
        }

        // GET: Households/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Household household)
        {
            if (ModelState.IsValid)
            {
                db.Entry(household).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(household);
        }

        [Authorize]
        public ActionResult Leave()
        {
            try
            {
                var user = db.Users.Find(User.Identity.GetUserId());

                if (user.Households == null)
                {
                    return RedirectToAction("Create", "Households");
                }
                Household household = db.Households.Single(h => h.Id == user.HouseholdId);
                return View(household);
            }

            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        public ActionResult LeaveConfirmed()
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            if (user.Households == null)
            {
                return RedirectToAction("Create", "Households");
            }
            if (user.Households.Members.Count == 1)
            {
                db.Households.Remove(user.Households);
            }

            user.HouseholdId = null;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("IndexLand", "Home");
        }


        // GET: Households/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            BankAccount bankaccount = db.BankAccounts.FirstOrDefault(x => x.Id == id);

            Household household = db.Households.Find(id);

            if (!household.Members.Contains(user))
            {
                return RedirectToAction("page404", "Home");
            }
            db.Households.Remove(household);
            db.SaveChanges();

            return RedirectToAction("IndexHOH", "Home");
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