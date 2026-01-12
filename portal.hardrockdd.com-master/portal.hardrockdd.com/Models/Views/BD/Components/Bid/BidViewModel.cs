//using Microsoft.AspNet.Identity;
//using portal.Repository.VP.PR;
//using System;
//using System.ComponentModel;
//using System.ComponentModel.DataAnnotations;
//using System.Web;
//using System.Web.Mvc;

//namespace portal.Models.Views.Bid
//{
//    public class BidViewModel
//    {
//        public BidViewModel()
//        {
//            using var empRepo = new EmployeeRepository();

//            var user = empRepo.GetEmployee(StaticFunctions.GetUserId());
//            Co = user.PRCo;
//        }
        
//        public BidViewModel(DB.Infrastructure.ViewPointDB.Data.Bid bid)
//        {
//            if (bid == null)
//            {
//                throw new System.ArgumentNullException(nameof(bid));
//            }
//            #region mapping
//            Co = bid.Co;
//            BidId = bid.BidId;
//            BidDate = bid.BidDate;
//            DueDate = bid.DueDate;
//            StartDate = bid.StartDate;
//            Description = bid.Description;
//            Comments = bid.Comments;
//            //CustomerId = bid.CustomerId;
//            FirmNumber = bid.FirmId;
//            DivisionId = bid.DivisionId;
//            Location = bid.Location;
//            FirmType = "OWNER";
//            CreatedUser = new Web.WebUserViewModel(bid.CreatedUser);// WebUserRepository.EntityToModel(bid.CreatedUser, "");
//            Status = (DB.BidStatusEnum)(bid.Status ?? 0);
//            BidType = (DB.BidTypeEnum)(bid.BidType ?? 0);
//            MobePrice = bid.MobePrice ?? 0;
//            DeMobePrice = bid.DeMobePrice ?? 0;
//            DelayDeMobePrice = bid.DelayDeMobePrice ?? 0;
//            IndustryId = bid.IndustryId;
//            ProjectManagerId = bid.ProjectMangerId;
            
//            #endregion

//            Firm = new Firm.FirmViewModel(bid.Firm);
//            Customers = new BidCustomerListViewModel(bid);
//        }

//        [Key]
//        [HiddenInput]
//        [Required]
//        [Display(Name = "Co")]
//        public byte Co { get; set; }

//        [Key]
//        [HiddenInput]
//        [Required]
//        [Display(Name = "Bid Id")]
//        public int BidId { get; set; }

//        [HiddenInput]
//        public string FirmType { get; set; }

//        [HiddenInput]
//        public string StatusClass
//        {
//            get
//            {
//                return StaticFunctions.StatusClass(Status);
//            }
//        }


//        [ReadOnly(true)]
//        [UIHint("EnumBox")]
//        [Display(Name = "Status")]
//        public DB.BidStatusEnum Status { get; set; }


//        [ReadOnly(true)]
//        [UIHint("DateBox")]
//        [DataType(DataType.Date)]
//        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
//        [Display(Name = "Bid Date")]
//        public DateTime? BidDate { get; set; }

//        [Required]
//        [UIHint("EnumBox")]
//        [Display(Name = "Type")]
//        public DB.BidTypeEnum BidType { get; set; }

//        [Required]
//        [UIHint("DateBox")]
//        [DataType(DataType.Date)]
//        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
//        [Display(Name = "Anticipated Start Date")]

//        public DateTime? StartDate { get; set; }

//        [Required]
//        [UIHint("DateBox")]
//        [DataType(DataType.Date)]
//        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
//        [Display(Name = "Due Date")]
//        public DateTime? DueDate { get; set; }

//        [Required]
//        [Display(Name = "Owner")]
//        [UIHint("DropdownBox")]
//        [Field(ComboUrl = "/Firm/OwnerCombo", ComboForeignKeys = "Co", AddUrl = "/Firm/Add", AddForeignKeys = "Co, FirmType", EditUrl = "/Firm/Form", EditForeignKeys = "Co, FirmNumber")]
//        public int? FirmNumber { get; set; }

//        [Required]
//        [Display(Name = "Division")]
//        [UIHint("DropdownBox")]
//        [Field(ComboUrl = "/Division/Combo", ComboForeignKeys = "Co")]
//        public int? DivisionId { get; set; }

//        [Required]
//        [Display(Name = "Industry")]
//        [UIHint("DropdownBox")]
//        [Field(ComboUrl = "/JCCombo/IndustryCombo", ComboForeignKeys = "Co")]
//        public int? IndustryId { get; set; }


//        //[Required]
//        //[Display(Name = "Market")]
//        //[UIHint("DropdownBox")]
//        //[Field(ComboUrl = "/JCCombo/IndustryMarketCombo", ComboForeignKeys = "Co,IndustryId")]
//        //public int? MarketId { get; set; }

//        [Required]
//        [Display(Name = "Location")]
//        [UIHint("TextBox")]
//        public string Location { get; set; }


//        [Required]
//        [Display(Name = "Prj. Manager")]
//        [UIHint("DropdownBox")]
//        [Field(ComboUrl = "/PRCombo/ProjectManagerCombo", ComboForeignKeys = "Co")]
//        public int? ProjectManagerId { get; set; }

//        [Required]
//        [Display(Name = "Description")]
//        [UIHint("TextBox")]
//        [Field(LabelSize = 2, TextSize = 10, FormGroupRow = 5)]
//        public string Description { get; set; }

//        [Display(Name = "Comments")]
//        [UIHint("TextAreaBox")]
//        [Field(LabelSize = 2, TextSize = 10, FormGroupRow = 6)]
//        public string Comments { get; set; }

//        [ReadOnly(true)]
//        [UIHint("WebUserBox")]
//        [Display(Name = "Created User")]
//        public Web.WebUserViewModel CreatedUser { get; set; }

//        [HiddenInput]
//        public Firm.FirmViewModel Firm { get; set; }

//        [UIHint("CurrencyBox")]
//        [Field(LabelSize =10, TextSize = 2)]
//        public decimal MobePrice { get; set; }

//        [UIHint("CurrencyBox")]
//        [Field(LabelSize = 10, TextSize = 2)]
//        public decimal DeMobePrice { get; set; }

//        [UIHint("CurrencyBox")]
//        [Field(LabelSize = 10, TextSize = 2)]
//        public decimal DelayDeMobePrice { get; set; }

//        public BidCustomerListViewModel Customers { get; set; }
//    }


//}