using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budget_Planner.Models.ViewModels
{
    public class JoinHouseholdVM
    {
        public string UserId { get; set; }

        public int HouseholdToJoinId { get; set; }

        public string HouseholdToJoinName { get; set; }
    }
}