using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace portal.Areas.Project.Models.Locate
{
    public class FormViewModel
    {
        public FormViewModel()
        {

        }

        public FormViewModel(DB.Infrastructure.ViewPointDB.Data.PMLocate locate)
        {
            if (locate == null)
                return;

            LocateId = locate.LocateId;

            Info = new LocateViewModel(locate);
            Sequences = new LocateSequenceListViewModel(locate);
            Assignments = new LocateAssignmentListViewModel(locate);
            UniqueAttchId = locate.CurrentSequence().Attachment.UniqueAttchID;
            MapSetId = locate.MapSet.MapSetId;
            if (locate.budPMLM_Import != null)
            {
                
                Import = new LocateImportViewModel(locate.budPMLM_Import);
            }

        }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "#")]
        public int LocateId { get; set; }

        public LocateViewModel Info { get; set; }

        public LocateSequenceListViewModel Sequences { get; set; }

        public LocateAssignmentListViewModel Assignments { get; set; }

        public LocateImportViewModel Import { get; set; }

        public Guid? UniqueAttchId { get; set; }

        public int MapSetId { get; set; }
    }
}