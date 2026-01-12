using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Project.Models.Locate
{
    public class LocateImportViewModel
    {
        public LocateImportViewModel()
        {

        }

        public LocateImportViewModel(DB.Infrastructure.ViewPointDB.Data.PMLocate_Import import)
        {
            if (import == null)
                return;

            ImportId = import.ImportId;
            LineId = import.LineId;

            Description = import.Description;
            RequestName = import.RequestName;
            GPSCoords = import.GPS;
            General = import.General;
            Owner = import.Owner;
            ProjectName = import.ProjectName;

            RefIds = import.RefId;

            ExcelFile = import.Import.FileName;
            ExcelSheet = import.ExcelSheet.SheetName;
        }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "#")]
        public int ImportId { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "#")]
        public int LineId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Description")]
        [Field(LabelSize = 2, TextSize = 10)]
        public string Description { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Owner")]
        public string Owner { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Org. Date")]
        public DateTime? OriginalStartDate { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Request By")]
        public string RequestName { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "GPS Coords")]
        public string GPSCoords { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "Location")]
        public string Location { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "General")]
        public string General { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "ProjectName")]
        [Field(LabelSize = 2, TextSize = 10)]
        public string ProjectName { get; set; }


        [UIHint("TextAreaBox")]
        [Display(Name = "Refs")]
        [Field(LabelSize = 2, TextSize = 10)]
        public string RefIds { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Excel File")]
        [Field(LabelSize = 2, TextSize = 10)]
        public string ExcelFile { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Excel Sheet")]
        [Field(LabelSize = 2, TextSize = 10)]
        public string ExcelSheet { get; set; }

        internal LocateImportViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.PMLocate_Imports.FirstOrDefault(f => f.ImportId == this.ImportId && f.LineId == this.LineId);

            if (updObj != null)
            {
                //updObj.Description = this.Description;
                //updObj.GPS = this.GPSCoords;
                //updObj.General = General;
                //updObj.Owner = Owner;
                //updObj.ProjectName = ProjectName;

                try
                {
                    db.BulkSaveChanges();
                    return new LocateImportViewModel(updObj);
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