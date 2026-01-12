//using System.ComponentModel.DataAnnotations;

//namespace portal
//{

//    public enum DB.BDDayRoundEnum
//    {
//        [Display(Name = "No Rounding")]
//        None,
//        [Display(Name = "1/4 Day")]
//        QuarterDay,
//        [Display(Name = "1/2 Day")]
//        HalfDay,
//        [Display(Name = "Full Day")]
//        FullDay
//    }

//    public enum DB.ScopeTypeEnum
//    {
//        [Display(Name = "Notes")]
//        Note,
//        [Display(Name = "Title")]
//        Title,
//        [Display(Name = "Clarifications")]
//        Clarification,
//        [Display(Name = "Scope of Work")]
//        Scope,
//        [Display(Name = "Out of Scope")]
//        OutofScope,
//    }

//    public enum BidBoreLineStatusEnum
//    {
//        [Display(Name = "Created")]
//        Draft = DB.BidStatusEnum.Draft,
//        [Display(Name = "Estimate")]
//        Estimate = DB.BidStatusEnum.Estimate,
//        [Display(Name = "Sales Review")]
//        SalesReview = DB.BidStatusEnum.SalesReview,
//        [Display(Name = "Final Review")]
//        FinalReview = DB.BidStatusEnum.FinalReview,
//        [Display(Name = "Proposal")]
//        Proposal = DB.BidStatusEnum.Proposal,
//        [Display(Name = "Pending Award")]
//        PendingAward = DB.BidStatusEnum.PendingAward,
//        [Display(Name = "Contract Approval")]
//        ContractApproval = DB.BidStatusEnum.ContractApproval,
//        Awarded = DB.BidStatusEnum.Awarded,
//        [Display(Name = "Not Award")]
//        NotAwarded = DB.BidStatusEnum.NotAwarded,
//        Canceled = DB.BidStatusEnum.Canceled,
//        Deleted = DB.BidStatusEnum.Deleted,
//        Template = DB.BidStatusEnum.Template,
//    }

//    public enum DB.BidStatusEnum
//    {
//        [Display(Name = "Created")]
//        Draft,
//        [Display(Name = "Estimate")]
//        Estimate,
//        [Display(Name = "Sales Review")]
//        SalesReview,
//        [Display(Name = "Final Review")]
//        FinalReview,
//        [Display(Name = "Proposal")]
//        Proposal,
//        [Display(Name = "Pending Award")]
//        PendingAward,
//        [Display(Name = "Contract Approval")]
//        ContractApproval,
//        Awarded,
//        [Display(Name = "Not Award")]
//        NotAwarded,
//        Canceled,
//        Deleted,
//        Template,
//        [Display(Name = "Contract Review")]
//        ContractReview,
//        Update,
//    }

//    public enum DB.BDPackageCostAllocationType
//    {
//        [Display(Name = "First Bore")]
//        FirstBore,
//        [Display(Name = "Last Bore")]
//        LastBore,
//        [Display(Name = "Bore Footage")]
//        AllBoreLF,
//        [Display(Name = "Bore Days")]
//        AllBoreDay
//    }

//    public enum DB.BidAwardStatusEnum
//    {
//        Pending = DB.BidStatusEnum.PendingAward,
//        Awarded = DB.BidStatusEnum.Awarded,
//        NotAwarded = DB.BidStatusEnum.NotAwarded,
//    }

//    public enum HardnessEnum
//    {
//        Soft,
//        Medium,
//        Hard,
//        VeryHard
//    }

//    public enum DB.BidProductionCalEnum
//    {
//        Rate,
//        Days
//    }

//    public enum DB.BidTypeEnum
//    {
//        Quote,
//        Budgetary,
//        QuickBid
//    }
//}