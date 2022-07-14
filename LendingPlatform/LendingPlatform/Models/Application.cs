using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendingPlatform.Models
{
    public class Application
    {
        public double BorrowingAmount { get; set; }
        public double AssetValue { get; set; }
        public int CreditScore { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public bool Complete => SubmittedAt != null && Decision != Definitions.EDecisionStatus.Undefined;
        public float LTVPercentage => AssetValue != 0 && BorrowingAmount != 0 ? (float)(BorrowingAmount / AssetValue) * 100 : 0;
        public string DecisionText { get; set; }
        public Definitions.EDecisionStatus Decision { get; set; }
    }
}
