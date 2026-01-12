using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Project.Models.Locate
{
    public class LocateSequenceListViewModel
    {
        public LocateSequenceListViewModel()
        {
            List = new List<LocateSequenceViewModel>
            {
                new LocateSequenceViewModel()
            };
        }

        public LocateSequenceListViewModel(PMLocate locate)
        {
            if (locate == null)
                return;

            LocateId = locate.LocateId;

            List = locate.Sequences.Select(s => new LocateSequenceViewModel(s)).ToList();
        }


        [Key]
        [UIHint("TextBox")]
        [Display(Name = "#")]
        public int LocateId { get; set; }

        public List<LocateSequenceViewModel>? List { get; }

    }

    public class LocateSequenceViewModel
    {
        public LocateSequenceViewModel()
        {

        }

        public LocateSequenceViewModel(DB.Infrastructure.ViewPointDB.Data.PMLocateSequence sequence)
        {
            if (sequence == null) throw new System.ArgumentNullException(nameof(sequence));

            LocateId = sequence.LocateId;
            SeqId = sequence.SeqId;
            LocateRefId = sequence.LocateRefId;
            StatusId = sequence.Status;
            Status = sequence.Status.ToString();

            StartDate = sequence.StartDate;
            ExpDate = sequence.EndDate;

            UniqueAttchId = sequence.Attachment.UniqueAttchID;
            AttachmentId = sequence.Attachment.Files.FirstOrDefault()?.AttachmentId;
        }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "#")]
        public int LocateId { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "#")]
        public int SeqId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Ref Id")]
        public string LocateRefId { get; set; }

        [Display(Name = "Status")]
        [UIHint("EnumBox")]
        public DB.PMLocateStatusEnum? StatusId { get; set; }

        [Display(Name = "Status")]
        [UIHint("TextBox")]
        public string Status { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "StartDate")]
        public DateTime? StartDate { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Expire Date")]
        public DateTime? ExpDate { get; set; }

        public Guid? UniqueAttchId { get; set; }

        public int? AttachmentId { get; set; }

        internal LocateSequenceViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.PMLocateSequences.FirstOrDefault(f => f.LocateId == this.LocateId && f.SeqId == this.SeqId);

            if (updObj != null)
            {

                try
                {
                    db.BulkSaveChanges();
                    return new LocateSequenceViewModel(updObj);
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