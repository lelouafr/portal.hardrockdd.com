using Newtonsoft.Json;
using portal.Code.Data.VP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;

namespace portal.Models.Views.HR.Resource
{
    public class ResourceListViewModel
    {
        public ResourceListViewModel()
        {
            List = new List<ResourceViewModel>();
        }


        public ResourceListViewModel(Code.Data.VP.HRPosition position)
        {
            if (position == null) throw new System.ArgumentNullException(nameof(position));

            HRCo = position.HRCo;

            List = position.Resources.Where(f => f.ActiveYN == "Y").Select(s => new ResourceViewModel(s)).ToList().OrderBy(o => o.Name).ToList();
        }

        public ResourceListViewModel(Code.Data.VP.HQCompanyParm company)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));

            HRCo = company.HQCo;

            List = company.HRCompanyParm.HRResources.Where(f => f.HRRef < 900000).Select(s => new ResourceViewModel(s)).ToList().OrderBy(o => o.Name).ToList();
        }

        public ResourceListViewModel(Code.Data.VP.HQCompanyParm company, HRResource resource)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));
            if (resource == null) throw new System.ArgumentNullException(nameof(resource));

            HRCo = company.HQCo;

            List = company.HRCompanyParm.HRResources.Where(f => f.ActiveYN == "Y" && f.ReportsTo == resource.HRRef).Select(s => new ResourceViewModel(s)).ToList().OrderBy(o => o.Name).ToList();
        }

        public ResourceListViewModel(Code.Data.VP.HQCompanyParm company, string active)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));

            HRCo = company.HQCo;

            if (active == "N")
            {
                List = company.HRCompanyParm.HRResources.Where(f => f.ActiveYN == active && f.HRRef < 900000).OrderByDescending(f => f.TermDate).Select(s => new ResourceViewModel(s)).ToList();
            }
            else
            {
                List = company.HRCompanyParm.HRResources.Where(f => f.ActiveYN == active && f.HRRef < 900000).Select(s => new ResourceViewModel(s)).ToList().OrderBy(o => o.Name).ToList(); ;
            }
        }

        [Key]
        public byte HRCo { get; set; }
        
        public List<ResourceViewModel> List { get;  }


        
    }

    public class ResourceViewModel
    {
        public ResourceViewModel()
        {

        }
        
        public ResourceViewModel(Code.Data.VP.HRResource resource)
        {
            if (resource == null) throw new System.ArgumentNullException(nameof(resource));

            HRCo = resource.HRCo;
            ResourceId = resource.HRRef;

            Name = resource.FullName();
            Status = resource.EmployeeStatus();
            PositionId = resource.PositionCode;
            Position = resource.Position?.Description;

            StatusString = Status.ToString();
        }

        [Key]
        [HiddenInput]
        public byte HRCo { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Id")]
        public int ResourceId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/HRCombo/PositionCodeCombo", ComboForeignKeys = "HRCo")]
        [Display(Name = "Position")]
        public string PositionId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Position")]
        public string Position { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public HRActiveStatusEnum Status { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public string StatusString { get; set; }
    }
}