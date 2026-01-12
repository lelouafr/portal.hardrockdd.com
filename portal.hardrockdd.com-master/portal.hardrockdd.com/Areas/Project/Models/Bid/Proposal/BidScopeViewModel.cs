using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Project.Models.Bid.Proposal
{
    public class BidScopeListViewModel
    {
        public BidScopeListViewModel()
        {

        }


        public BidScopeListViewModel(DB.Infrastructure.ViewPointDB.Data.Bid bid, DB.ScopeTypeEnum scopeType)
        {
            if (bid == null)
                return;

            #region mapping
            BDCo = bid.BDCo;
            BidId = bid.BidId;
            ScopeTypeId = scopeType;
            #endregion

            List = bid.Scopes.Where(f => f.ScopeTypeId ==(int)scopeType).Select(s => new BidScopeViewModel(s)).ToList();
        }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Co")]
        public byte BDCo { get; set; }

        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Bid Id")]
        public int BidId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [TableField(Width = "5")]
        public DB.ScopeTypeEnum ScopeTypeId { get; set; }

        public List<BidScopeViewModel> List { get; }
    }

    public class BidScopeViewModel
    {
        public BidScopeViewModel()
        {

        }

        public BidScopeViewModel(BidProposalScope scope)
        {
            if (scope == null)
                return;

            #region mapping
            BDCo = scope.BDCo;
            BidId = scope.BidId;
            ScopeTypeId = (DB.ScopeTypeEnum)scope.ScopeTypeId;
            LineId = scope.LineId;
            Title = scope.Title;
            Notes = scope.Notes;
            Status = scope.Status;
            # endregion
        }

        [Key]
        [Required]
        [HiddenInput]
        public byte BDCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        public int BidId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        public DB.ScopeTypeEnum ScopeTypeId { get; set; }

        [Key]
        [Required]
        [Display(Name = "#")]
        [UIHint("LongBox")]
        public int LineId { get; set; }

        [Required]
        [Display(Name = "Title")]
        [UIHint("TextBox")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Notes")]
        [UIHint("TextEditorAreaBox")]
        [AllowHtml]
        public string Notes { get; set; }

        [Required]
        [Display(Name = "Status")]
        [UIHint("EnumBox")]
        public int? Status { get; set; }

        internal BidScopeViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.BidProposalScopes.FirstOrDefault(f => f.BDCo == BDCo &&
                                                                 f.BidId == BidId &&
                                                                 f.ScopeTypeId == (int)ScopeTypeId &&
                                                                 f.LineId == LineId);

            if (updObj != null)
            {
                updObj.Title = Title ?? string.Empty;
                updObj.Notes = Notes ?? string.Empty;

                try
                {
                    db.BulkSaveChanges();
                    return new BidScopeViewModel(updObj);
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