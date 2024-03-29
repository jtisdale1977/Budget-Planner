﻿using Budget_Planner.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budget_Planner.Helpers
{
    public static class HouseholdHelper
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        public static Household GetHousehold(this string userId)
        {
            var user = db.Users.Find(userId);
            if (user == null || user.HouseholdId == null)
            {
                return null;
            }

            var hh = db.Households.Find(user.HouseholdId);

            return hh; //returns entire household
        }

        public static IdentityMessage CreateJoinMessage(this Invitation user)
        {
            var invitedUser = db.Invitations.FirstOrDefault(u => u.Id == user.Id);
            var msg = new IdentityMessage();
            var dt = DateTime.Now.AddDays(7).ToLongDateString();
            msg.Destination = invitedUser.Email; //ConfigurationManager.AppSettings["ContactEmail"];
            msg.Body = invitedUser.InvitedBy + " " + "has invited you to join their household on Pocket Gaurd! To access Pocket Gaurd extensive tools for financial management, copy the following Invite Code and then visit the Pocket Gaurd website by clicking <a href=\"http://jtisdale-budgeter.azurewebsites.net\">here</a>. After registering, enter your Invite Code in the indicated text box to join the household. <br/>This code is only active until" + dt + ", after which point you can request a new code from " + invitedUser.InvitedBy + ".<br/><br/>Invite Code:" + invitedUser.InviteCode;
            msg.Subject = "Invitation to join Pocket Gaurd";

            return msg;
        }

        public static List<Category> AddStandardCategories(this List<CategoryStandard> categories, Household household)
        {
            var CatList = new List<Category>();
            foreach (var category in categories)
            {
                var newCategory = new Category()
                {
                    Name = category.Name,
                    HouseholdId = household.Id
                };
                CatList.Add(newCategory);
            }
            return CatList;
        }
    }
}