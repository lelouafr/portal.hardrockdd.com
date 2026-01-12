//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//namespace portal.Repository.VP.JC
//{
//    public partial class ContractRepository //: IDisposable
//    {
//        //public static JCContract Init(Job job)
//        //{
//        //    if (job == null)
//        //    {
//        //        throw new ArgumentNullException(nameof(job));
//        //    }

//        //    var model = new JCContract
//        //    {
//        //        //JCCo = package.Co,
//        //        //Description = package.Description,
//        //        //JobStatus = 2,
//        //        //VendorGroup = package.Co,
//        //        //Division = package.Divsion.Description,
//        //        //Owner = package.Bid.Firm.FirmName,
//        //    };

//        //    return model;
//        //}

//        //public static JCContract Init(BidPackage package)
//        //{
//        //    if (package == null)
//        //    {
//        //        throw new ArgumentNullException(nameof(package));
//        //    }
//        //    var description = string.Format(AppCultureInfo.CInfo(), "{0} - {1}", package.Bid.Firm.FirmName, package.Description);
//        //    var model = new JCContract
//        //    {
//        //        JCCo = (byte)package.Division.WPDivision.HQCompany.JCCo,
//        //        Description = description,
//        //        ContractStatus = 2,
//        //        Customer = package.Customer,
//        //        CustGroupId = package.Customer.CustGroupId,
//        //        CustomerId = package.CustomerId,
//        //        StartDate = package.StartDate ?? package.Bid.StartDate
//        //    };
//        //    model.StartMonth = new DateTime(model.StartDate.Value.Year, model.StartDate.Value.Month, 1);

//        //    model.ContractId = package.Project.JobId;
//        //    model.JCCompanyParm = package.Division.WPDivision.HQCompany.JCCompanyParm;
//        //    model = InitDefault(model);
//        //    return model;
//        //}


//        //public static JCContract Init(BidBoreLine bore)
//        //{
//        //    if (bore == null)
//        //    {
//        //        throw new ArgumentNullException(nameof(bore));
//        //    }
//        //    var description = string.Format(AppCultureInfo.CInfo(), "{0} - {1} {2:F2}\" - {3}", bore.Package.Bid.Firm.FirmName, bore.Package.Description, bore.PipeSize, bore.Description);
//        //    var model = new JCContract();
//        //    model.JCCo = (byte)bore.Division.WPDivision.HQCompany.JCCo;
//        //    model.Description = description;
//        //    model.ContractStatus = 1;
//        //    model.CustGroupId = bore.Bid.Company.CustGroupId;
//        //    model.CustomerId = bore.CustomerId;
//        //    model.CustomerReference = bore.Package.CustomerReference;
//        //    model.StartDate = bore.Package.StartDate ?? bore.Bid.StartDate; ;
//        //    model.StartMonth = new DateTime(model.StartDate.Value.Year, model.StartDate.Value.Month, 1);

//        //    model.ContractId = bore.Job.JobId;
//        //    model.JCCompanyParm = bore.Division.WPDivision.HQCompany.JCCompanyParm;
//        //    model = InitDefault(model);

//        //    return model;
//        //}

//        public static JCContract InitDefault(JCContract contract)
//        {
//            if (contract == null)
//            {
//                throw new ArgumentNullException(nameof(contract));
//            }

//            contract.DepartmentId = "1";
//            contract.ContractStatus = 2;
//            contract.OriginalDays = 0;
//            contract.CurrentDays = 0;
//            //contract.StartDate = new DateTime(2020,1,1);
//            contract.TaxInterface = "N";
//            contract.TaxGroup = contract.JCCompanyParm.HQCompanyParm.TaxGroupId;
//            contract.RetainagePCT = 0;
//            contract.DefaultBillType = "N";
//            contract.OrigContractAmt = 0;
//            contract.ContractAmt = 0;
//            contract.BilledAmt = 0;
//            contract.ReceivedAmt = 0;
//            contract.CurrentRetainAmt = 0;
//            contract.SIMetric = "N";
//            contract.BillOnCompletionYN = "N";
//            contract.CompleteYN = "N";
//            contract.RoundOpt = "N";
//            contract.ReportRetgItemYN = "N";
//            contract.JBFlatBillingAmt = 0;
//            contract.JBLimitOpt = "N";
//            contract.ClosePurgeFlag = "N";
//            contract.UpdateJCCI = "N";
//            contract.MaxRetgOpt = "N";
//            contract.MaxRetgPct = 0;
//            contract.MaxRetgAmt = 0;
//            contract.InclACOinMaxYN = "Y";
//            contract.MaxRetgDistStyle = "C";
//            contract.AUUseTrustAccounts = "N";




//            return contract;
//        }
//    }
//}
