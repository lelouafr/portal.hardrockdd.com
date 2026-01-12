using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.AP.CreditCard
{
    public class CreditImportListViewModel
    {
        public CreditImportListViewModel()
        {

        }
        public CreditImportListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));

            Co = company.HQCo;
            List = company.CreditImports.Select(s => new CreditImportViewModel(s)).ToList();
        }

        [Key]   
        public byte Co { get; set; }

        public List<CreditImportViewModel> List { get; }
    }
    public class CreditImportViewModel
    {
        public CreditImportViewModel()
        {

        }

        public CreditImportViewModel(CreditImport import)
        {
            if (import == null) throw new System.ArgumentNullException(nameof(import));

            Co = import.Co;
            ImportId = import.ImportId;
            Source = import.Source;
            FileName = import.FileName;
            NumberOfLines = import.NumberofLines;
            LinesAdded = import.LinesAdded;
            LinesSkipped = import.LinesSkipped;
            ErrorLines = import.ErrorLines;
            ImportDate = (DateTime)import.CreatedOn?.Date;

            ImportedBy = string.Format(AppCultureInfo.CInfo(), "{0} {1}", import.CreatedUser.FirstName, import.CreatedUser.LastName);
            //import.CreatedBy
        }

        [Key]
        public byte Co { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "#")]
        public int ImportId { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Date")]
        public DateTime ImportDate { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "By")]
        public string ImportedBy { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Source")]
        public string Source { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "File")]
        public string FileName { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Lines")]

        public long? NumberOfLines { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Added")]
        public long? LinesAdded { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Skipped")]
        public long? LinesSkipped { get; set; }

        [UIHint("LongBox")]
        [Display(Name = "Errors")]
        public long? ErrorLines { get; set; }
    }
}