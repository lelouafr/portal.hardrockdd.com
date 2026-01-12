using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Areas.Project.Models.Bid
{
    public class BidInfoViewModel
    {
        public BidInfoViewModel()
        {
            //using var empRepo = new EmployeeRepository();

            //var user = empRepo.GetEmployee(StaticFunctions.GetUserId());
            //Co = user.PRCo;
        }

        public BidInfoViewModel(DB.Infrastructure.ViewPointDB.Data.Bid bid)
        {
            if (bid == null)
                throw new System.ArgumentNullException(nameof(bid));

            #region mapping
            JCCo = bid.Company.JCCompanyParm.JCCo;
            BDCo = bid.BDCo;
            BidId = bid.BidId;
            BidDate = bid.BidDate;
            DueDate = bid.DueDate;
            StartDate = bid.StartDate;
            Description = bid.Description;
            Comments = bid.Comments;
            CustomerId = bid.CustomerId;
            ContactId = bid.ContactId;
            FirmNumber = bid.tFirmId;
            DivisionId = bid.DivisionId;
            Location = bid.Location;
            StateCodeId = bid.StateCodeId;
            FirmType = "OWNER";
            FirmName = bid.Firm?.FirmName;
            // CreatedUser = new Web.WebUserViewModel(bid.CreatedUser);// WebUserRepository.EntityToModel(bid.CreatedUser, "");
            Status = bid.Status;
            BidType = (DB.BidTypeEnum)(bid.BidType ?? 0);
            MobePrice = bid.MobePrice ?? 0;
            DeMobePrice = bid.DeMobePrice ?? 0;
            DelayDeMobePrice = bid.DelayDeMobePrice ?? 0;
            IndustryId = bid.IndustryId;
            ProjectManagerId = bid.ProjectMangerId;
            CreatedUserName = bid.CreatedUser.FullName();
            UniqueAttchID = bid.Attachment.UniqueAttchID;
            GPS = bid.GPS;
            MapSetId = bid.MapSet.MapSetId;
            #endregion

            //Firm = new Firm.FirmViewModel(bid.Firm);
            Customers = new CustomerListViewModel(bid);

            HasLocateRequest = bid.PMLocateRequests.Any();
        }

        [Key]
        [Required]
        [Display(Name = "Co")]
        public byte BDCo { get; set; }

        public byte JCCo { get; set; }

        [Key]
        [Required]
        [Display(Name = "Bid Id")]
        public int BidId { get; set; }

        public string FirmType { get; set; }

        public string StatusClass
        {
            get
            {
                return StaticFunctions.StatusClass(Status);
            }
        }


        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.BidStatusEnum Status { get; set; }


        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Bid Date")]
        public DateTime? BidDate { get; set; }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Type")]
        public DB.BidTypeEnum BidType { get; set; }

        [Required]
        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Anticipated Start Date")]

        public DateTime? StartDate { get; set; }

        [Required]
        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Due Date")]
        public DateTime? DueDate { get; set; }


        [Display(Name = "GPS Coords")]
        [UIHint("TextBox")]
        public string GPS { get; set; }

        [Required]
        [Display(Name = "Owner")]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/Firm/OwnerCombo", ComboForeignKeys = "VendGroupId=BDCo", AddUrl = "/Firm/Add", AddForeignKeys = "VendGroupId=BDCo, FirmType", EditUrl = "/Firm/Form", EditForeignKeys = "VendGroupId=BDCo, FirmNumber")]
        public int? FirmNumber { get; set; }


        [Display(Name = "Owner")]
        [UIHint("Textox")]
        public string FirmName { get; set; }

        [Required]
        [Display(Name = "Division")]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/Division/Combo", ComboForeignKeys = "PMCo=BDCo")]
        public int? DivisionId { get; set; }

        [Required]
        [Display(Name = "Industry")]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/JCCombo/IndustryCombo", ComboForeignKeys = "JCCo=BDCo")]
        public int? IndustryId { get; set; }

        [Required]
        [Display(Name = "Location")]
        [UIHint("TextBox")]
        public string Location { get; set; }

        [Required]
        [Display(Name = "State")]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/HQCombo/StateCombo", ComboForeignKeys = "country='US'")]
        public string StateCodeId { get; set; }

        [Required]
        [Display(Name = "Prj. Manager")]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PRCombo/ProjectManagerCombo", ComboForeignKeys = "PRCo=BDCo")]
        public int? ProjectManagerId { get; set; }

        [Required]
        [Display(Name = "Description")]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroupRow = 5)]
        public string Description { get; set; }

        [Display(Name = "Comments")]
        [UIHint("TextAreaBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroupRow = 6)]
        public string Comments { get; set; }

        //[ReadOnly(true)]
        //[UIHint("WebUserBox")]
        //[Display(Name = "Created User")]
        //public Web.WebUserViewModel CreatedUser { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Created User")]
        public string CreatedUserName { get; set; }

        //[HiddenInput]
        //public Firm.FirmViewModel Firm { get; set; }

        [UIHint("CurrencyBox")]
        [Field(LabelSize = 10, TextSize = 2)]
        public decimal MobePrice { get; set; }

        [UIHint("CurrencyBox")]
        [Field(LabelSize = 10, TextSize = 2)]
        public decimal DeMobePrice { get; set; }

        [UIHint("CurrencyBox")]
        [Field(LabelSize = 10, TextSize = 2)]
        public decimal DelayDeMobePrice { get; set; }

        [Display(Name = "Customer")]
        [UIHint("DropDownBox")]
        [Field(ComboUrl = "/ARCombo/Combo", ComboForeignKeys = "CustGroupId=BDCo")]
        public int? CustomerId { get; set; }

        [Display(Name = "Contact")]
        [UIHint("DropDownBox")]
        [Field(ComboUrl = "/ARCombo/ContactCombo", ComboForeignKeys = "CustGroupId=BDCo, CustomerId", AddUrl = "/Contact/Add", AddForeignKeys = "CustGroupId=BDCo, CustomerId", EditUrl = "/Contact/Form", EditForeignKeys = "CustGroupId=BDCo, CustomerId, ContactId")]
        public int? ContactId { get; set; }

        public Guid UniqueAttchID { get; set; }
        public CustomerListViewModel Customers { get; set; }
        public int MapSetId { get; set; }


        public bool HasLocateRequest { get; set; }

        internal BidInfoViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.Bids.FirstOrDefault(f => f.BDCo == BDCo && f.BidId == BidId);

            if (updObj != null)
            {
                updObj.Description = Description;
                updObj.Comments = Comments;
                updObj.ProjectMangerId = ProjectManagerId;
                updObj.DivisionId = DivisionId;
                updObj.IndustryId = IndustryId;
                updObj.FirmId = FirmNumber;
                updObj.Location = Location;
                updObj.StateCodeId = StateCodeId;
                updObj.BidType = (int)BidType;
                updObj.StartDate = StartDate;
                updObj.DueDate = DueDate;
                updObj.DelayDeMobePrice = DelayDeMobePrice;
                updObj.CustomerId = CustomerId;
                updObj.ContactId = ContactId;
                updObj.GPS = GPS;

                try
                {
                    db.BulkSaveChanges();
                    return new BidInfoViewModel(updObj);
                }
                catch (Exception ex)
                {
                    modelState.AddModelError("", ex.Message);
                    return this;
                }
            }
            else
            {
                modelState.AddModelError("", "Object Doesn't Exist For Update!");
                return this;
            }
        }
    }
}