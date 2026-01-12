using System.ComponentModel.DataAnnotations;

namespace DB
{

    public enum BDDayRoundEnum
    {
        [Display(Name = "No Rounding")]
        None,
        [Display(Name = "1/4 Day")]
        QuarterDay,
        [Display(Name = "1/2 Day")]
        HalfDay,
        [Display(Name = "Full Day")]
        FullDay
    }

    public enum ScopeTypeEnum
    {
        [Display(Name = "Notes")]
        Note,
        [Display(Name = "Title")]
        Title,
        [Display(Name = "Clarifications")]
        Clarification,
        [Display(Name = "Scope of Work")]
        Scope,
        [Display(Name = "Out of Scope")]
        OutofScope,
    }

    public enum BidBoreLineStatusEnum
    {
        [Display(Name = "Created")]
        Draft = BidStatusEnum.Draft,
        [Display(Name = "Estimate")]
        Estimate = BidStatusEnum.Estimate,
        [Display(Name = "Sales Review")]
        SalesReview = BidStatusEnum.SalesReview,
        [Display(Name = "Final Review")]
        FinalReview = BidStatusEnum.FinalReview,
        [Display(Name = "Proposal")]
        Proposal = BidStatusEnum.Proposal,
        [Display(Name = "Pending Award")]
        PendingAward = BidStatusEnum.PendingAward,
        [Display(Name = "Contract Approval")]
        ContractApproval = BidStatusEnum.ContractApproval,
        Awarded = BidStatusEnum.Awarded,
        [Display(Name = "Not Award")]
        NotAwarded = BidStatusEnum.NotAwarded,
        Canceled = BidStatusEnum.Canceled,
        Deleted = BidStatusEnum.Deleted,
        Template = BidStatusEnum.Template,
    }

    public enum BidStatusEnum
    {
        [Display(Name = "Created")]
        Draft,
        [Display(Name = "Estimate")]
        Estimate,
        [Display(Name = "Sales Review")]
        SalesReview,
        [Display(Name = "Final Review")]
        FinalReview,
        [Display(Name = "Proposal")]
        Proposal,
        [Display(Name = "Pending Award")]
        PendingAward,
        [Display(Name = "Contract Approval")]
        ContractApproval,
        Awarded,
        [Display(Name = "Not Award")]
        NotAwarded,
        Canceled,
        Deleted,
        Template,
        [Display(Name = "Contract Review")]
        ContractReview,
        Update,
    }

    public enum BDPackageCostAllocationType
    {
        [Display(Name = "First Bore")]
        FirstBore,
        [Display(Name = "Last Bore")]
        LastBore,
        [Display(Name = "Bore Footage")]
        AllBoreLF,
        [Display(Name = "Bore Days")]
        AllBoreDay
    }

    public enum BidAwardStatusEnum
    {
        Pending = BidStatusEnum.PendingAward,
        Awarded = BidStatusEnum.Awarded,
        NotAwarded = BidStatusEnum.NotAwarded,
    }

    public enum HardnessEnum
    {
        Soft,
        Medium,
        Hard,
        VeryHard
    }

    public enum BidProductionCalEnum
    {
        Rate,
        Days
    }

    public enum BidTypeEnum
    {
        Quote,
        Budgetary,
        QuickBid
    }
}