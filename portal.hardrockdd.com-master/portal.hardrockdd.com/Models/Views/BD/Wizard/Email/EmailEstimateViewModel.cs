using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace portal.Models.Views.Bid.Wizard.Email
{
    public class EmailViewModel
    {
        public EmailViewModel()
        {

        }

        public EmailViewModel(DB.Infrastructure.ViewPointDB.Data.Bid bid)
        {
            if (bid == null)
            {
                throw new System.ArgumentNullException(nameof(bid));
            }
            BDCo = bid.BDCo;
            BidId = bid.BidId;

            BidInfo = new Forms.Header.BidInfoViewModel(bid);
            Packages = new Forms.Package.PackageListViewModel(bid, true);

            var workFlow = bid.WorkFlow.CurrentSequance();
            if (workFlow != null)
            {
                Comments = workFlow?.Comments;
            }
        }

        [Key]
        public byte BDCo { get; set; }

        [Key]
        public int BidId { get; set; }

        public string Comments { get; set; }

        public Forms.Header.BidInfoViewModel BidInfo { get; set; }

        public Forms.Package.PackageListViewModel Packages { get; set; }


    }
}