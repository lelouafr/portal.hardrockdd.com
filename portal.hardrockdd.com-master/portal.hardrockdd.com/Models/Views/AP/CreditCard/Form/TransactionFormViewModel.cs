using portal.Models.Views.AP.CreditCard.Employee;

namespace portal.Models.Views.AP.CreditCard.Form
{
    public class TransactionFormViewModel
    {
        public TransactionFormViewModel()
        {

        }

        public TransactionFormViewModel(DB.Infrastructure.ViewPointDB.Data.CreditTransaction trans)
        {
            if (trans == null) throw new System.ArgumentNullException(nameof(trans));
            CCCo = trans.CCCo;
            TransId = trans.TransId;

            BasicInfo = new InfoViewModel(trans);
            Coding = new CodingInfoListViewModel(trans);
            Merchant = new Vendor.Merchant.Forms.MerchantFormViewModel(trans.Merchant);
            Images = new ImageBankListViewModel(trans, true);
            Lines = new TransactionLineListViewModel(trans);
            Actions = new ActionViewModel(trans);
            Forum = new Forums.ForumLineListViewModel(trans.GetForum());
        }

        public byte CCCo { get; set; }

        public long TransId { get; set; }

        public Vendor.Merchant.Forms.MerchantFormViewModel Merchant { get; set; }

        public InfoViewModel BasicInfo { get; set; }

        public CodingInfoListViewModel Coding { get; set; }

        public ImageBankListViewModel Images { get; set; }

        public TransactionLineListViewModel Lines { get; set; }

        public ActionViewModel Actions { get; set; }

        public Forums.ForumLineListViewModel Forum { get; set; }
    }
}