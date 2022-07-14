using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendingPlatform.Models
{
    public static class Definitions
    {
        public enum EDecisionStatus
        {
            Undefined,
            Accepted,
            Rejected,
            Error
        }
    }
}
