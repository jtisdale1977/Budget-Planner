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
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net.Mime;

namespace Budget_Planner.Controllers
{
    public class InvitationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private Household hh = new Household();

        // GET: Invitations
        [Authorize]
        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var invitations = db.Invitations.Include(i => i.Households).Where(u => user.HouseholdId == u.HouseholdId).ToList();
            Household household = db.Households.Find(user.HouseholdId);

            if (household == null)
            {
                return RedirectToAction("Create", "Households");
            }

            return View(invitations);
        }

        // GET: Invitations/Details/5
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
                    RedirectToAction("Create", "Households");
                }

                Invitation invitation = db.Invitations.Single(i => i.Id == id && i.HouseholdId == user.HouseholdId);

                return View(invitation);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // GET: Invitations/Create
        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var userHouseholdId = db.Households.AsNoTracking().Where(u => user.HouseholdId == u.Id).ToList();
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name");
            return View();
        }

        // POST: Invitations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Create([Bind(Include = "HouseholdId,Email,InviteCode,Accepted,Expired,Generated,Expiration")] Invitation invitation)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                var existingUser = db.Users.Where(u => u.Email == invitation.Email).FirstOrDefault();

                Household household = db.Households.Find(user.HouseholdId);
                invitation.InviteCode = Guid.NewGuid();
                invitation.HouseholdId = household.Id;
                db.Invitations.Add(invitation);
                await db.SaveChangesAsync();

                try
                {
                    //Creating Email Message
                    MailMessage inviteMessage = new MailMessage();
                    inviteMessage.To.Add(new MailAddress(invitation.Email, invitation.Email));
                    inviteMessage.From = new MailAddress(user.Email, "From");
                    inviteMessage.Subject = "Pocket Gaurd: An Invitation To Join A Household";


                    //if invitee IS already registered
                    if (existingUser != null)
                    {
                        var callbackUrlForExistingUser = Url.Action("JoinHousehold", "Account", new { invitationHouseholdId = invitation.HouseholdId }, protocol: Request.Url.Scheme);

                        string bodytext = String.Concat("<p>I would like to cordially invite you to join my household <mark>", household.Name,
                                    "</mark> in the Pocket Gaurd app household budgeting system.", "</p> <p><a href='", callbackUrlForExistingUser, "'>Join</a></p>");

                        inviteMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(bodytext, null, MediaTypeNames.Text.Html));
                    }
                    //if invitee is NOT already registered
                    else
                    {
                        var callbackUrl = Url.Action("Register_Join", "Account", new { invitationHouseholdId = invitation.HouseholdId, invitationId = invitation.Id, inviteCode = invitation.InviteCode }, protocol: Request.Url.Scheme);

                        string html = String.Concat("<p>I would like to cordially invite you to join my household <mark>", household.Name,
                                        "</mark> in the Pocket Gaurd app household budgeting system.</p> <p><a href='", callbackUrl, "'>Join</a></p>");

                        inviteMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));
                    }
                    //Send Email
                    var svc = new PersonalEmail();
                    await svc.SendAsync(inviteMessage);

                    return RedirectToAction("Index", "Invitations");
                }
                catch (Exception ex)
                {
                    ViewBag.Exception = ex.Message;
                    return View(invitation);
                }
            }


            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", invitation.HouseholdId);
            return View(invitation);
        }

        //// GET: Invitations/Delete/5
        //[Authorize]
        //public ActionResult Delete(int? id)
        //{
        //    var user = db.Users.Find(User.Identity.GetUserId());
        //    Invitation invitation = db.Invitations.FirstOrDefault(x => x.Id == id);
        //    Household household = db.Households.FirstOrDefault(x => x.Id == invitation.HouseholdId);

        //    if (!household.Members.Contains(user))
        //    {
        //        return RedirectToAction("page404", "Home");
        //    }
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    if (invitation == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(invitation);
        //}

        //// POST: Invitations/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //[Authorize]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    var user = db.Users.Find(User.Identity.GetUserId());
        //    Invitation invitation = db.Invitations.FirstOrDefault(x => x.Id == id);
        //    Household household = db.Households.FirstOrDefault(x => x.Id == invitation.HouseholdId);

        //    if (!household.Members.Contains(user))
        //    {
        //        return RedirectToAction("page404", "Home");
        //    }

        //    db.Invitations.Remove(invitation);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
            var user = db.Users.FirstOrDefault(u => u.Email == User.Identity.Name);
            var userHouseholdId = user.Households.Id;

            return userHouseholdId;
        }
    }
}
