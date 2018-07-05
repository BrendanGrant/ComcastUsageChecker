using System;
using System.Collections.Generic;
using System.Text;

namespace ComcastUsageChecker
{
    public class Device
    {
        public string id { get; set; }
        public double usage { get; set; }
    }

    public class UsageMonth
    {
        public string policyName { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public double homeUsage { get; set; }
        public double allowableUsage { get; set; }
        public string unitOfMeasure { get; set; }
        public List<Device> devices { get; set; }
        public double additionalBlocksUsed { get; set; }
        public double additionalCostPerBlock { get; set; }
        public double additionalUnitsPerBlock { get; set; }
        public double additionalIncluded { get; set; }
        public double additionalUsed { get; set; }
        public double additionalPercentUsed { get; set; }
        public double additionalRemaining { get; set; }
        public double billableOverage { get; set; }
        public double overageCharges { get; set; }
        public double overageUsed { get; set; }
        public int currentCreditAmount { get; set; }
        public int maxCreditAmount { get; set; }
        public string policy { get; set; }
    }

    public class DataUsage
    {
        public int courtesyUsed { get; set; }
        public int courtesyRemaining { get; set; }
        public int courtesyAllowed { get; set; }
        public bool inPaidOverage { get; set; }
        public List<UsageMonth> usageMonths { get; set; }
    }
}
