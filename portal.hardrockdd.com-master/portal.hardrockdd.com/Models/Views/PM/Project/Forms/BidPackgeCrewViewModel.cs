using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.PM.Project.Form
{
    public class BidPackageCrewListViewModel
    {
        public BidPackageCrewListViewModel()
        {
            List = new List<BidPackgeCrewViewModel>();
        }

        public BidPackageCrewListViewModel(BidPackage package)
        {
            if (package == null) throw new System.ArgumentNullException(nameof(package));

            BDCo = package.BDCo;
            BidId = package.BidId;
            PackageId = package.PackageId;
            List = package.Crews.Select(s => new BidPackgeCrewViewModel(s)).ToList();
        }


        [Key]
        public byte BDCo { get; set; }

        [Key]
        public int BidId { get; set; }

        [Key]
        public int PackageId { get; set; }

        [Key]
        public string ProjectId { get; set; }

        public List<BidPackgeCrewViewModel> List { get; }
    }

    public class BidPackgeCrewViewModel
    {
        public BidPackgeCrewViewModel()
        {

        }

        public BidPackgeCrewViewModel(DB.Infrastructure.ViewPointDB.Data.BDPackageCrew crew)
        {
            if (crew == null) throw new System.ArgumentNullException(nameof(crew));

            BDCo = crew.BDCo;
            BidId = crew.BidId;
            PackageId = crew.PackageId;
            SeqId = crew.SeqId;
            CrewId = crew.CrewId;
            StartDate = crew.StartDate;
        }
        [Key]
        [HiddenInput]
        public byte BDCo { get; set; }

        [Key]
        public int BidId { get; set; }

        [Key]
        public int PackageId { get; set; }

        [Key]
        public int SeqId { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Project")]
        public string ProjectId { get; set; }


        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/PRCombo/JobTypeCrewCombo", ComboForeignKeys = "")]
        [Display(Name = "Crew")]
        public string CrewId { get; set; }



    }
}