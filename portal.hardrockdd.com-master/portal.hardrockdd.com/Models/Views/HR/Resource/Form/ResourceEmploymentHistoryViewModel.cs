using Newtonsoft.Json;
using portal.Code.Data.VP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;

namespace portal.Models.Views.HR.Resource.Form
{
    public class ResourceEmploymentHistoryListViewModel
    {
        public ResourceEmploymentHistoryListViewModel()
        {
            List = new List<ResourceEmploymentHistoryViewModel>();
        }


        public ResourceEmploymentHistoryListViewModel(HRResource resource)
        {
            if (resource == null) throw new System.ArgumentNullException(nameof(resource));

            Co = resource.HRCo;
            ResourceId = resource.HRRef;

            List = resource.EmploymentHistory.Select(s => new ResourceEmploymentHistoryViewModel(s)).ToList();
        }

        [Key]
        public byte Co { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Id")]
        public int ResourceId { get; set; }

        public List<ResourceEmploymentHistoryViewModel> List { get;  }
    }

    public class ResourceEmploymentHistoryViewModel
    {
        public ResourceEmploymentHistoryViewModel()
        {

        }
        
        public ResourceEmploymentHistoryViewModel(Code.Data.VP.EmploymentHistory history)
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