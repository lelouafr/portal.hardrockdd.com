using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class Bid: IForum, IWorkFlow
    {
        private VPEntities _db;

        public VPEntities db
        {
            set
            {
                _db = value;
            }
            get
            {
                if (_db == null)
                {
                    _db = VPEntities.GetDbContextFromEntity(this);

                    if (_db == null)
                        _db = this.Company.db;

                    if (_db == null)
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }

        public static string BaseTableName { get { return "budBDBH"; } }

        public int StatusId
        {
            get
            {
                return tStatusId ?? 0;
            }
            set
            {
                if (value != tStatusId)
                {
                    UpdateStatus(value);
                }
            }
        }

        public DB.BidStatusEnum Status
        {
            get
            {
                return (DB.BidStatusEnum)StatusId;
            }
            set
            {
                StatusId = (int)value;
            }
        }

        public int? DivisionId
        {
            get
            {
                return tDivisionId;
            }
            set
            {
                UpdateDivision(value);
            }
        }

        public void UpdateDivision(int? value)
        {
            ////var db = VPEntities.GetDbContextFromEntity(this);;

            if ((Division == null && tDivisionId != null) || Division?.PMCo != Division?.WPDivision?.HQCompany?.JCCo)
            {
                var division = db.ProjectDivisions.FirstOrDefault(f => f.DivisionId == tDivisionId);
                if (division != null)
                {
                    division = db.ProjectDivisions.FirstOrDefault(f => f.DivisionId == division.DivisionId && f.PMCo == division.WPDivision.HQCompany.JCCo);
                    Division = division;
                    JCCo = division.PMCo;
                    tDivisionId = division.DivisionId;
                }
                else
                {
                    Division = null;
                    JCCo = null;
                    tDivisionId = null;
                }
            }
            if (tDivisionId != value)
            {
                var division = db.ProjectDivisions.FirstOrDefault(f => f.DivisionId == value);
                if (division != null)
                {
                    division = db.ProjectDivisions.FirstOrDefault(f => f.DivisionId == division.DivisionId && f.PMCo == division.WPDivision.HQCompany.JCCo);
                    Division = division;
                    JCCo = division.PMCo;
                    tDivisionId = division.DivisionId;
                    ProjectMangerId = division.WPDivision.ManagerId;
                }
                else
                {
                    Division = null;
                    JCCo = null;
                    tDivisionId = null;
                }
                Packages.ToList().ForEach(package =>
                {
                    package.DivisionId = value;
                });
            }

        }

        public int? CustomerId
        {
            get
            {
                return tCustomerId;
            }
            set
            {
                UpdateCustomer(value);
            }
        }

        public void UpdateCustomer(int? value)
        {
            ////var db = VPEntities.GetDbContextFromEntity(this);;

            if (Customer == null && tCustomerId != null)
            {
                var customer = db.Customers.FirstOrDefault(f => f.CustomerId == tCustomerId);
                if (customer != null)
                {
                    Customer = customer;
                    //CustGroupId = customer.CustGroupId;
                    tCustomerId = customer.CustomerId;
                }
                else
                {
                    Customer = null;
                    JCCo = null;
                    tCustomerId = null;
                    ContactId = null;
                }
            }
            if (tCustomerId != value)
            {
                var customer = db.Customers.FirstOrDefault(f => f.CustomerId == value);
                if (customer != null)
                {


                    var bidCust = Customers.FirstOrDefault(f => f.CustomerId == value);
                    if (bidCust == null)
                    {
                        bidCust = new BidCustomer
                        {
                            BDCo = BDCo,
                            BidId = BidId,
                            LineId = Customers.DefaultIfEmpty().Max(f => f == null ? 0 : f.LineId) + 1,
                            CustomerId = value,
                            Bid = this,
                        };
                        Customers.Add(bidCust);
                    }

                    bidCust = Customers.FirstOrDefault(f => f.CustomerId == tCustomerId);
                    if (bidCust != null)
                        Customers.Remove(bidCust);

                    Customer = customer;
                    //CustGroupId = customer.CustGroupId;
                    tCustomerId = customer.CustomerId;
                    ContactId = null;
                }
                else
                {
                    Customer = null;
                    JCCo = null;
                    tCustomerId = null;
                    ContactId = null;
                }
            }

            Packages.ToList().ForEach(package =>
            {
                package.Customer = Customer;
                package.JCCo = JCCo;
                package.CustomerId = value;
            });
        }

        public int? IndustryId
        {
            get
            {
                return tIndustryId;
            }
            set
            {
                UpdateIndustry(value);
            }
        }

        public void UpdateIndustry(int? value)
        {
            ////var db = VPEntities.GetDbContextFromEntity(this);;

            if (Industry == null && tIndustryId != null)
            {
                var industry = db.JCIndustries.FirstOrDefault(f => f.IndustryId == tIndustryId && f.JCCo == (JCCo ?? BDCo));
                if (industry != null)
                {
                    Industry = industry;
                    JCCo = industry.JCCo;
                    tIndustryId = industry.IndustryId;
                }
                else
                {
                    Industry = null;
                    JCCo = null;
                    tIndustryId = null;

                }
            }
            if (tIndustryId != value)
            {
                var industry = db.JCIndustries.FirstOrDefault(f => f.IndustryId == value && f.JCCo == (JCCo ?? BDCo));
                if (industry != null)
                {
                    Industry = industry;
                    JCCo = industry.JCCo;
                    tIndustryId = industry.IndustryId;
                }
                else
                {
                    Industry = null;
                    JCCo = null;
                    tIndustryId = null;
                }
            }

            Packages.ToList().ForEach(package =>
            {
                package.Industry = Industry;
                package.JCCo = JCCo;
                package.IndustryId = value;
            });
        }

        public DateTime? StartDate
        {
            get
            {
                return tStartDate;
            }
            set
            {
                if (tStartDate != value)
                {
                    tStartDate = value;
                    foreach (var package in Packages)
                    {
                        package.StartDate = tStartDate;
                    }
                }
            }
        }

        public List<BidPackage> ActivePackages
        {
            get
            {
                return Packages.Where(f => f.Status != DB.BidStatusEnum.Deleted && f.Status != DB.BidStatusEnum.Canceled).ToList();
            }
        }

        public List<BidBoreLine> ActiveBoreLines
        {
            get
            {
                return BoreLines.Where(f => f.Status != DB.BidStatusEnum.Deleted && f.Status != DB.BidStatusEnum.Canceled).ToList();
            }
        }

        public void UpdateStatus(int value)
        {
            if ((Status == DB.BidStatusEnum.Awarded || Status == DB.BidStatusEnum.Update) && ((DB.BidStatusEnum?)value == DB.BidStatusEnum.Deleted || (DB.BidStatusEnum?)value == DB.BidStatusEnum.Canceled))
                return;

            if (WorkFlow == null)
                AddWorkFlow();
            tStatusId = value;
            var status = (DB.BidStatusEnum)value;

            WorkFlow.CreateSequance(value);
            AddWorkFlowAssignments();

            ActivePackages.ForEach(package => {
                if (package.StatusId < value && package.AwardStatus != DB.BidAwardStatusEnum.Awarded)
                {
                    package.Status = status;
                }
                package.PendingBoreLines.ForEach(bore => {
                    bore.Status = status;
                });
            });

            switch (status)
            {
                case DB.BidStatusEnum.Draft:
                    EmailStatusUpdate();
                    break;
                case DB.BidStatusEnum.Estimate:
                    EmailStatusUpdate();
                    break;
                case DB.BidStatusEnum.SalesReview:
                    EmailStatusUpdate();
                    break;
                case DB.BidStatusEnum.FinalReview:
                    EmailStatusUpdate();
                    break;
                case DB.BidStatusEnum.Proposal:
                    EmailStatusUpdate();
                    break;
                case DB.BidStatusEnum.PendingAward:
                    break;
                case DB.BidStatusEnum.ContractApproval:
                    EmailStatusUpdate();
                    break;
                case DB.BidStatusEnum.Awarded:
                    CreateProjects();
                    EmailStatusUpdate();
                    break;
                case DB.BidStatusEnum.NotAwarded:
                    break;
                case DB.BidStatusEnum.Canceled:
                    EmailStatusUpdate();
                    break;
                case DB.BidStatusEnum.Deleted:
                    break;
                case DB.BidStatusEnum.Template:
                    break;
                case DB.BidStatusEnum.ContractReview:
                    EmailStatusUpdate();
                    break;
                case DB.BidStatusEnum.Update:
                    CreateProjects();
                    EmailStatusUpdate();
                    //Forces Status back to awarded.
                    tStatusId = (int)DB.BidStatusEnum.Awarded;
                    break;
                default:
                    break;
            }
        }

        public WorkFlow GetWorkFlow()
        {
            var workFlow = WorkFlow;
            if (workFlow == null)
            {
                workFlow = AddWorkFlow();
            }

            return workFlow;
        }

        public WorkFlow AddWorkFlow()
        {
            var workFlow = this.WorkFlow;
            if (WorkFlow == null)
            {
                workFlow = new WorkFlow
                {
                    WFCo = BDCo,
                    WorkFlowId = db.GetNextId(WorkFlow.BaseTableName, 1),
                    TableName = BaseTableName,
                    Id = BidId,
                    CreatedBy = db.CurrentUserId,
                    CreatedOn = DateTime.Now,
                    Active = true,

                    Company = Company,
                };
                db.WorkFlows.Add(workFlow);
                WorkFlow = workFlow;
                WorkFlowId = workFlow.WorkFlowId;

                workFlow.AddSequance(this.StatusId);
                this.AddWorkFlowAssignments();
                db.BulkSaveChanges();
            }

            return workFlow;
        }

        public void AddWorkFlowAssignments(bool reset = false)
        {
            if (WorkFlow == null)
                AddWorkFlow();
            if (reset)
            {
                var assignedusers = WorkFlow.CurrentSequance().AssignedUsers.ToList();
                foreach (var user in assignedusers)
                {
                    WorkFlow.CurrentSequance().AssignedUsers.Remove(user);
                }
            }
            switch (Status)
            {
                case DB.BidStatusEnum.Draft:
                    WorkFlow.AddUser(CreatedUser);
                    break;
                case DB.BidStatusEnum.Estimate:
                    WorkFlow.AddEmployee(Division.WPDivision.DivisionManger);
                    WorkFlow.AddByEmail("cory.baker@hardrockdd.com");
                    WorkFlow.AddByEmail("chris.jones@hardrockdd.com");
                    break;
                case DB.BidStatusEnum.SalesReview:
                    WorkFlow.AddUser(CreatedUser);
                    WorkFlow.AddByEmail("cory.baker@hardrockdd.com");
                    WorkFlow.AddByEmail("chris.jones@hardrockdd.com");
                    break;
                case DB.BidStatusEnum.FinalReview:
                    WorkFlow.AddEmployee(Division.WPDivision.DivisionManger);
                    WorkFlow.AddByEmail("cory.baker@hardrockdd.com");
                    break;
                case DB.BidStatusEnum.Proposal:
                    WorkFlow.AddUser(CreatedUser);
                    WorkFlow.AddByEmail("cory.baker@hardrockdd.com");
                    WorkFlow.AddByEmail("chris.jones@hardrockdd.com");
                    break;
                case DB.BidStatusEnum.PendingAward:
                    WorkFlow.AddUser(CreatedUser);
                    WorkFlow.AddByEmail("cory.baker@hardrockdd.com");
                    WorkFlow.AddByEmail("chris.jones@hardrockdd.com");
                    break;
                case DB.BidStatusEnum.ContractReview:
                    WorkFlow.AddByEmail("bobby.hoover@hardrockdd.com");
                    break;
                case DB.BidStatusEnum.ContractApproval:
                    WorkFlow.AddUsersByPosition("FIN-ARMGR");
                    WorkFlow.AddByEmail("robert.tipton@hardrockdd.com");
                    break;
                case DB.BidStatusEnum.Awarded:
                    WorkFlow.AddEmployee(Division.WPDivision.DivisionManger);
                    WorkFlow.AddByEmail("cory.baker@hardrockdd.com");
                    WorkFlow.AddByEmail("chris.jones@hardrockdd.com");
                    break;
                case DB.BidStatusEnum.Update:
                    WorkFlow.AddEmployee(Division.WPDivision.DivisionManger);
                    WorkFlow.AddByEmail("cory.baker@hardrockdd.com");
                    WorkFlow.AddByEmail("chris.jones@hardrockdd.com");
                    break;
                case DB.BidStatusEnum.NotAwarded:
                    WorkFlow.CompleteWorkFlow();
                    break;
                case DB.BidStatusEnum.Canceled:
                    WorkFlow.CompleteWorkFlow();
                    break;
                default:
                    break;
            }

        }

        public void CreateProjects()
        {
            foreach (var package in ActivePackages)
            {
                package.CreateProject();
            }
        }

        public void EmailStatusUpdate()
        {
            if (HttpContext.Current == null)
            {
                return;
            }
            var result = new Models.Views.Bid.Wizard.Email.EmailViewModel(this);
            var viewPath = "";
            var subject = "";
            using var db = new VPEntities();

            //viewPath = "../SM/Request/Email/EmailSubmit";
            switch (Status)
            {
                case DB.BidStatusEnum.Draft:
                    break;
                case DB.BidStatusEnum.Estimate:
                    viewPath = "../BD/Bid/Email/EmailSalesSubmit";
                    subject = string.Format(AppCultureInfo.CInfo(), "Review for Estimate {0} from: {1} ", result.BidInfo.Description, result.BidInfo.CreatedUserName);
                    break;
                case DB.BidStatusEnum.SalesReview:
                    viewPath = "../BD/Bid/Email/EmailEstimateSubmit";
                    subject = string.Format(AppCultureInfo.CInfo(), "Bid Pricing Review for {0} from: {1} ", result.BidInfo.Description, result.BidInfo.CreatedUserName);
                    break;
                case DB.BidStatusEnum.FinalReview:
                    viewPath = "../BD/Bid/Email/EmailFinalReviewSubmit";
                    subject = string.Format(AppCultureInfo.CInfo(), "Final Review for {0} from: {1} ", result.BidInfo.Description, result.BidInfo.CreatedUserName);
                    break;
                case DB.BidStatusEnum.Proposal:
                    viewPath = "../BD/Bid/Email/EmailProposalSent";
                    subject = string.Format(AppCultureInfo.CInfo(), "Bid Ready for Proposal for {0} from: {1} ", result.BidInfo.Description, result.BidInfo.CreatedUserName);
                    break;
                case DB.BidStatusEnum.PendingAward:
                    //result = new Models.Views.Bid.Wizard.Email.EmailEstimateViewModel(bid);
                    //viewPath = "../BD/Bid/Email/EmailEstimateSubmit";
                    //subject = string.Format(AppCultureInfo.CInfo(), "Bid Pricing Review for {0} from: {1} ", result.BidInfo.Description, result.BidInfo.CreatedUserName);
                    break;
                case DB.BidStatusEnum.ContractReview:
                    viewPath = "../BD/Bid/Email/EmailContractReview";
                    subject = string.Format(AppCultureInfo.CInfo(), "Bid Sent for Contract Review for {0} from: {1} ", result.BidInfo.Description, result.BidInfo.CreatedUserName);
                    break;
                case DB.BidStatusEnum.ContractApproval:
                    ;
                    viewPath = "../BD/Bid/Email/EmailContractApproval";
                    subject = string.Format(AppCultureInfo.CInfo(), "Bid Sent for Job Creation for {0} from: {1} ", result.BidInfo.Description, result.BidInfo.CreatedUserName);
                    break;
                case DB.BidStatusEnum.Awarded:
                    viewPath = "../BD/Bid/Email/EmailAwarded";
                    subject = string.Format(AppCultureInfo.CInfo(), "Bid has been Awarded and Jobs Created for {0} from: {1} ", result.BidInfo.Description, result.BidInfo.CreatedUserName);
                    break;
                case DB.BidStatusEnum.NotAwarded:
                    break;
                case DB.BidStatusEnum.Canceled:
                    viewPath = "../BD/Bid/Email/EmailCanceled";
                    subject = string.Format(AppCultureInfo.CInfo(), "Bid has been canced for {0} from: {1} ", result.BidInfo.Description, result.BidInfo.CreatedUserName);
                    break;
                case DB.BidStatusEnum.Update:
                    viewPath = "../BD/Bid/Email/EmailUpdated";
                    subject = string.Format(AppCultureInfo.CInfo(), "Bid has been Updated: {0} from: {1} ", result.BidInfo.Description, result.BidInfo.CreatedUserName);
                    break;
                case DB.BidStatusEnum.Deleted:
                    break;
                case DB.BidStatusEnum.Template:
                    break;
                default:
                    break;
            }

            if (!string.IsNullOrEmpty(viewPath))
            {
                try
                {
                    using System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage()
                    {
                        Body = Code.EmailHelper.RenderViewToString(viewPath, result, false),
                        IsBodyHtml = true,
                        Subject = subject,
                    };

                    foreach (var workFlow in WorkFlow.CurrentSequance().AssignedUsers.ToList())
                    {
                        var user = db.WebUsers.FirstOrDefault(f => f.Id == workFlow.AssignedTo);
                        msg.To.Add(new System.Net.Mail.MailAddress(user.Email));
                    }
                    if (msg.To.Count == 0)
                    {
                        msg.To.Add(new System.Net.Mail.MailAddress("cory.baker@hardrockdd.com"));
                        msg.To.Add(new System.Net.Mail.MailAddress("chris.jones@hardrockdd.com"));
                        msg.To.Add(new System.Net.Mail.MailAddress(CreatedUser.Email));
                    }
                    if (CreatedUser.Email != StaticFunctions.GetCurrentUser().Email)
                    {
                        msg.CC.Add(new System.Net.Mail.MailAddress(StaticFunctions.GetCurrentUser().Email));
                    }
                    Code.EmailHelper.Send(msg);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public void ImportScopeTemplate()
        {
            using var db = new VPEntities();
            var template = db.Bids.FirstOrDefault(f => f.BDCo == 1 && f.BidId == 0);
            foreach (var scope in template.Scopes)
            {
                var item = Scopes.FirstOrDefault(f => f.ScopeTypeId == scope.ScopeTypeId && f.LineId == scope.LineId);
                if (item == null)
                {
                    item = new BidProposalScope()
                    {
                        BDCo = BDCo,
                        BidId = BidId,
                        LineId = scope.LineId,
                        ScopeTypeId = scope.ScopeTypeId,
                        Notes = scope.Notes,
                        Title = scope.Title,
                    };
                    Scopes.Add(item);
                }
                else
                {
                    item.Title = scope.Title;
                    item.Notes = scope.Notes;
                }
            }

        }

        public BidCustomer AddCustomer()
        {
            var result = new BidCustomer
            {
                BDCo = BDCo,
                BidId = BidId,
                LineId = Customers.DefaultIfEmpty().Max(f => f == null ? 0 : f.LineId) + 1,
                CustomerId = 0,
                Bid = this,
            };

            Customers.Add(result);

            return result;
        }

        public BidProposalScope AddScope(DB.ScopeTypeEnum scopeType)
        {
            var result = new BidProposalScope
            {
                BDCo = BDCo,
                BidId = BidId,
                LineId = Scopes.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineId) + 1,
                ScopeTypeId = (byte)scopeType,
                Notes = string.Empty,
                Title = string.Empty
            };
            Scopes.Add(result);
            return result;
        }

        public Forum GetForum()
        {
            var forum = this.Forum;
            if (forum == null)
            {
                forum = AddForum();
            }
            return forum;
        }

        public Forum AddForum()
        {
            var forum = this.Forum;
            if (forum == null)
            {
                forum = new Forum()
                {
                    Co = this.BDCo,
                    ForumId = db.GetNextId(Forum.BaseTableName),
                    RelKeyID = this.KeyID,
                    TableName = BaseTableName,
                    db = this.db
                };

                this.Forum = forum;
                db.BulkSaveChanges();
            }

            return forum;
        }
    }
}