using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.HumanResource.Models.Resource.Form
{
    public class EmploymentHistoryListViewModel
    {
        public EmploymentHistoryListViewModel()
        {
            List = new List<EmploymentHistoryViewModel>();
        }


        public EmploymentHistoryListViewModel(HRResource resource)
        {
            if (resource == null) throw new System.ArgumentNullException(nameof(resource));

            Co = resource.HRCo;
            ResourceId = resource.HRRef;

            List = resource.EmploymentHistory.Select(s => new EmploymentHistoryViewModel(s)).ToList();
        }

        [Key]
        public byte Co { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Id")]
        public int ResourceId { get; set; }

        public List<EmploymentHistoryViewModel> List { get;  }
    }

    public class EmploymentHistoryViewModel
    {
        public EmploymentHistoryViewModel()
        {

        }
        
        public EmploymentHistoryViewModel(EmploymentHistory history)
        {
            if (history == null) throw new System.ArgumentNullException(nameof(history));

            HRCo = history.HRCo;
            ResourceId = history.HRRef;

            SeqId = history.Seq;
            DateChanged = history.DateChanged;
            HistoryCode = history.Code;
            Type = history.Type;
            Notes = history.Notes;
        }

        [Key]
        [HiddenInput]
        public byte HRCo { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Id")]
        public int ResourceId { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "#")]
        public int SeqId { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Date")]
        public DateTime DateChanged { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "HistoryCode")]
        public string HistoryCode { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/HRCombo/HistoryCodeTypeCombo", ComboForeignKeys = "HRCo")]
        [Display(Name = "Type")]
        public string Type { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Notes")]
        public string Notes { get; set; }
    }
}