using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Models.Views.GL.Account
{
    public class AccountListViewModel
    {

        public AccountListViewModel()
        {

        }

        public AccountListViewModel(byte glco, VPContext db)
        {
            GLCo = glco;
            List = db.udGLAC_AccountNumber(glco).ToList().Select(s => new AccountViewModel(s)).ToList();

        }

        public byte GLCo { get; set; }

        public List<AccountViewModel> List { get; }
    }
    public class AccountViewModel
    {

        public AccountViewModel()
        {

        }

        public AccountViewModel(udGLAC_AccountNumber_Result account)
        {
            GLCo = account.GLCo;
            GLAcct = account.GLAcct;
            AccountId = account.AccountId;
            Description = account.Description;
            AccountNumber = account.AccountNumber;
        }

        public byte GLCo { get; set; }
        public string GLAcct { get; set; }
        public string AccountId { get; set; }
        public string Description { get; set; }
        public long? AccountNumber { get; set; }

    }
}