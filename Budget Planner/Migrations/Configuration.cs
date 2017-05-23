namespace Budget_Planner.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Budget_Planner.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Budget_Planner.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }
            if (!context.Roles.Any(r => r.Name == "HOH"))
            {
                roleManager.Create(new IdentityRole { Name = "HOH" });
            }
            if (!context.Roles.Any(r => r.Name == "House Member"))
            {
                roleManager.Create(new IdentityRole { Name = "House Member" });
            }
            //if (!context.Roles.Any(r => r.Name == "Unassigned"))
            //{
            //    roleManager.Create(new IdentityRole { Name = "Unassigned" });
            //}

            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "jtisdale1977@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "jtisdale1977@gmail.com",
                    Email = "jtisdale1977@gmail.com",
                    FirstName = "Justin",
                    LastName = "Tisdale",
                    DisplayName = "Justin Tisdale"
                }, "Abc123!");
            }
            if (!context.Users.Any(u => u.Email == "admindemo@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "admindemo@mailinator.com",
                    Email = "admindemo@mailinator.com",
                    FirstName = "Admin",
                    LastName = "Demo",
                    DisplayName = "Admin Demo"
                }, "Abc123!");
            }
            if (!context.Users.Any(u => u.Email == "hohdemo@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "hohdemo@mailinator.com",
                    Email = "hohdemo@mailinator.com",
                    FirstName = "HOH",
                    LastName = "Demo",
                    DisplayName = "HOH Demo"
                }, "Abc123!");
            }
            if (!context.Users.Any(u => u.Email == "memberdemo@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "memberdemo@mailinator.com",
                    Email = "memberdemo@mailinator.com",
                    FirstName = "Member",
                    LastName = "Demo",
                    DisplayName = "Member Demo"
                }, "Abc123!");
            }
            //if (!context.Users.Any(u => u.Email == "unassed@mailinator.com"))
            //{
            //    userManager.Create(new ApplicationUser
            //    {
            //        UserName = "unassed@mailinator.com",
            //        Email = "unassed@mailinator.com",
            //        FirstName = "Unassigned",
            //        LastName = "Unassigned",
            //        DisplayName = "Unassigned"
            //    }, "Abc&123!");
            //}

            var Admin = userManager.FindByEmail("jtisdale1977@gmail.com").Id;
            userManager.AddToRole(Admin, "Admin");

            var AdminDemo = userManager.FindByEmail("admindemo@mailinator.com").Id;
            userManager.AddToRole(AdminDemo, "Admin");

            var HOHDemo = userManager.FindByEmail("hohdemo@mailinator.com").Id;
            userManager.AddToRole(HOHDemo, "HOH");

            var MemberDemo = userManager.FindByEmail("memberdemo@mailinator.com").Id;
            userManager.AddToRole(MemberDemo, "House Member");

            //var Unassigned = userManager.FindByEmail("unassed@mailinator.com").Id;
            //userManager.AddToRole(Unassigned, "Unassigned");

            if (!context.Categories.Any(u => u.Name == "Home Theatre"))
            { context.Categories.Add(new Category { Name = "Home Theatre" }); }

            if (!context.Categories.Any(u => u.Name == "Emergency Fund"))
            { context.Categories.Add(new Category { Name = "Emergency Fund" }); }

            if (!context.Categories.Any(u => u.Name == "Housing"))
            { context.Categories.Add(new Category { Name = "Housing" }); }

            if (!context.Categories.Any(u => u.Name == "Taxes"))
            { context.Categories.Add(new Category { Name = "Taxes" }); }

            if (!context.Categories.Any(u => u.Name == "Utilities"))
            { context.Categories.Add(new Category { Name = "Utilities" }); }

            if (!context.Categories.Any(u => u.Name == "Health Care"))
            { context.Categories.Add(new Category { Name = "Health Care" }); }

            if (!context.Categories.Any(u => u.Name == "Automotive"))
            { context.Categories.Add(new Category { Name = "Automotive" }); }

            if (!context.Categories.Any(u => u.Name == "Food and Groceries"))
            { context.Categories.Add(new Category { Name = "Food and Groceries" }); }

            if (!context.Categories.Any(u => u.Name == "Personal Care"))
            { context.Categories.Add(new Category { Name = "Personal Care" }); }

            if (!context.Categories.Any(u => u.Name == "Vacation"))
            { context.Categories.Add(new Category { Name = "Vacation" }); }


            if (!context.Warnings.Any(u => u.WarningLevel == "None"))
            { context.Warnings.Add(new Warning { WarningLevel = "None" }); }

            if (!context.Warnings.Any(u => u.WarningLevel == "50"))
            { context.Warnings.Add(new Warning { WarningLevel = "50" }); }

            if (!context.Warnings.Any(u => u.WarningLevel == "100"))
            { context.Warnings.Add(new Warning { WarningLevel = "100" }); }

            if (!context.Warnings.Any(u => u.WarningLevel == "250"))
            { context.Warnings.Add(new Warning { WarningLevel = "250" }); }

            if (!context.Warnings.Any(u => u.WarningLevel == "500"))
            { context.Warnings.Add(new Warning { WarningLevel = "500" }); }

            if (!context.Warnings.Any(u => u.WarningLevel == "750"))
            { context.Warnings.Add(new Warning { WarningLevel = "750" }); }

            if (!context.Warnings.Any(u => u.WarningLevel == "1000"))
            { context.Warnings.Add(new Warning { WarningLevel = "1000" }); }

            if (!context.BankAccounts.Any(u => u.Name == "Checking"))
            { context.BankAccounts.Add(new BankAccount { Name = "Checking" }); }

            if (!context.BankAccounts.Any(u => u.Name == "Savings"))
            { context.BankAccounts.Add(new BankAccount { Name = "Savings" }); }

            if (!context.TransactionTypes.Any(u => u.Name == "Deposit"))
            { context.TransactionTypes.Add(new TransactionType { Name = "Deposit" }); }

            if (!context.TransactionTypes.Any(u => u.Name == "Withdrawal"))
            { context.TransactionTypes.Add(new TransactionType { Name = "Withdrawal" }); }

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
