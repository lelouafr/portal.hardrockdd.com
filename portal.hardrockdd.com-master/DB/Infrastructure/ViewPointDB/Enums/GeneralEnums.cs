using System.ComponentModel.DataAnnotations;

namespace DB
{
    public enum TimeSelectionEnum
    {
        [Display(Name = "Last Month")]
        LastMonth,
        [Display(Name = "Last 3 Month")]
        LastThreeMonths,
        [Display(Name = "Last 6 Month")]
        LastSixMonths,
        [Display(Name = "Last Year")]
        LastYear,
        [Display(Name = "All")]
        All
    }

    public enum YesNoEnum
    {
        No,
        Yes
    }

    public enum IPAddressFilteringAction
    {
        Allow,
        Restrict
    }
}