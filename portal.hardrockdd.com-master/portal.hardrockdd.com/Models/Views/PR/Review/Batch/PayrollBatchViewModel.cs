using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Payroll
{

    public class PayrollBatchListViewModel
    {
        public PayrollBatchListViewModel()
        {

        }
        public PayrollBatchListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company)
        {
            if (company == null)
            {
                throw new System.ArgumentNullException(nameof(company));
            }
            #region mapping
            Co = company.HQCo;            
            #endregion

            List = company.Batches.Where(f => f.TableName == "PRTB").Select(s => new PayrollBatchViewModel(s)).ToList();

        }

        [Key]
        [ReadOnly(true)]
        [HiddenInput]
        [Required]
        [Display(Name = "Co")]
        public byte Co { get; set; }

        public List<PayrollBatchViewModel> List { get; }
    }

    public class PayrollBatchViewModel
    {
        public PayrollBatchViewModel()
        {

        }

        public PayrollBatchViewModel(DB.Infrastructure.ViewPointDB.Data.Batch batch)
        {
            if (batch == null)
            {
                throw new System.ArgumentNullException(nameof(batch));
            }
            #region mapping
            Co = batch.Co;
            Mth = batch.Mth;
            BatchId = batch.BatchId;
            Status = batch.Status;
            PREndDate = (DateTime)batch.PREndDate;
            PostDate = batch.DatePosted;
            CreatedOn = batch.DateCreated;
            CreatedUser = new Web.WebUserViewModel(batch.CreatedUser.PREmployee);
            Notes = batch.Notes;
            #endregion
        }
        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Co")]
        public byte Co { get; set; }

        [Key]
        [ReadOnly(true)]
        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Period")]
        public DateTime Mth { get; set; }

        [Key]
        [ReadOnly(true)]
        [HiddenInput]
        [Required]
        [Display(Name = "Id")]
        public int BatchId { get; set; }

        public byte Status { get; set; }

        [ReadOnly(true)]
        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "PR End Date")]
        public DateTime PREndDate { get; set; }

        [ReadOnly(true)]
        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Post Date")]
        public DateTime? PostDate { get; set; }

        [ReadOnly(true)]
        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Created Date")]
        public DateTime CreatedOn { get; set; }

        [Required]
        [Display(Name = "Notes")]
        [UIHint("TextAreaBox")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Notes", FormGroupRow = 1)]
        public string Notes { get; set; }

        [ReadOnly(true)]
        [UIHint("WebUserBox")]
        [Display(Name = "Created User")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Project Info", FormGroupRow = 6)]
        public Web.WebUserViewModel CreatedUser { get; set; }
    }
}
