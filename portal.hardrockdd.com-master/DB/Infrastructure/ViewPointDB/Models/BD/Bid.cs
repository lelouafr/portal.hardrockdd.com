using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class Bid: IForum, IWorkFlow, IAttachment
    {
        private VPContext _db;

        public VPContext db
        {
            set
            {
                _db = value;
            }
            get
            {
                _db ??= this.Company.db;
                _db ??= VPContext.GetDbContextFromEntity(this);
                return _db;
            }
        }

        public static string BaseTableName { get { return "budBDBH"; } }

        public HQAttachment Attachment
        {
            get
            {
                if (HQAttachment != null)
                {
                    if (BidId >= 3006 && Firm != null && !string.IsNullOrEmpty(tDescription))
                    {
                        var root = HQAttachment.GetRootFolder();
                        if (root.StorageLocation == DB.HQAttachmentStorageEnum.DB)
                        {
                            var folderLoc = GetSharePointRootFolderPath();
                            if (folderLoc != null)
                            {
                                var list = GetSharePointList();

                                HQAttachment.SharePointList = list;

                                root.SharePointFolderPath = folderLoc;
                                var folder = root.GetFolderSharePoint();
                                if (folder == null)
                                    folder = root.CreateFolderSharePoint();

                                HQAttachment.SharePointRootFolder = root.SharePointFolderPath;
                                root.StorageLocation = DB.HQAttachmentStorageEnum.SharePoint;

                                db.BulkSaveChanges();
                            }
                        }
                    }
                    return HQAttachment;
                }

                UniqueAttchID ??= Guid.NewGuid();
                var attachment = new HQAttachment()
                {
                    db = this.db,
                    HQCompanyParm = this.Company,
                    HQCo = this.BDCo,
                    UniqueAttchID = (Guid)UniqueAttchID,
                    TableKeyId = this.KeyID,
                    TableName = BaseTableName,
                    //SPListId = this.GetSharePointList()?.ListId,
                };
                db.HQAttachments.Add(attachment);
                attachment.BuildDefaultFolders();
                HQAttachment = attachment;
                db.BulkSaveChanges();

                return HQAttachment;
            }
        }


        public WPMapSet MapSet
        {
            get
            {
                if (WPMapSet != null)
                {
                    return WPMapSet;
                }
                var mapset = new WPMapSet()
                {
                    MapSetId = db.GetNextId(WPMapSet.BaseTableName),
                    TableKeyId = this.BidId,
                    TableName = BaseTableName,

                    db = this.db,
                };
                this.MapSetId = mapset.MapSetId;

                var cord = mapset.AddCoord();
                cord.Title = Description;
                cord.GPSCoords = GPS;

                db.WPMapSets.Add(mapset);
                WPMapSet = mapset;
                db.BulkSaveChanges();

                return MapSet;
            }
        }

        public string GPS
        {
            get => tGPS;
            set
            {
                if (tGPS != value)
                {
                    char[] delimiterChars = { ',', ' ' };
                    value ??= string.Empty;
                    var mapCoords = MapSet.Coords.FirstOrDefault(f => f.GPSCoords == tGPS);
                    if (mapCoords == null)
                        mapCoords = MapSet.Coords.FirstOrDefault(f => f.GPSCoords == value);
                    if (mapCoords == null)
                        mapCoords = MapSet.Coords.FirstOrDefault();
                    var coords = value.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
                    if (coords.Length == 2)
                    {
                        GPSLat = decimal.TryParse(coords[0], out decimal outLat) ? outLat : 0;
                        GPSLong = decimal.TryParse(coords[1], out decimal outLong) ? outLong : 0;

                        tGPS = value;
                        if (mapCoords == null)
                        {
                            mapCoords = MapSet.AddCoord();
                            mapCoords.GPSCoords = GPS;
                            mapCoords.GPSLat = GPSLat;
                            mapCoords.GPSLong = GPSLong;
                        }
                        if (mapCoords.GPSCoords != value)
                        {
                            mapCoords.GPSCoords = value;
                            mapCoords.GPSLat = GPSLat;
                            mapCoords.GPSLong = GPSLong;
                        }
                    }
                    else
                    {
                        GPSLat = null;
                        GPSLong = null;
                        tGPS = null;
                        mapCoords.GPSCoords = null;
                        mapCoords.GPSLat = null;
                        mapCoords.GPSLong = null;
                    }
                }
            }
        }

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

        public BidStatusEnum Status
        {
            get
            {
                return (BidStatusEnum)StatusId;
            }
            set
            {
                StatusId = (int)value;
            }
        }

        public string Description
        {
            get => tDescription;
            set => UpdateDescription(value);
        }

        public void UpdateDescription(string value)
        {
            if (value == null)
                return;

            var badChar = new List<string>() { "\"", "*", ":", "<", ">", "?", "/", "\\", "|" };
            badChar.ForEach(c => { value = value.Replace(c, ""); });

            if (tDescription != value)
            {
                tDescription = value;
                if (Attachment.GetRootFolder().StorageLocation == HQAttachmentStorageEnum.SharePoint ||
                    Attachment.GetRootFolder().StorageLocation == HQAttachmentStorageEnum.DBAndSharePoint)
                    this.Attachment.SharePointRootFolder = GetSharePointRootFolderPath();
            }
        }

        public int? FirmId { get => tFirmId; set => UpdateFirm(value); }

        public void UpdateFirm(int? value)
        {
            if ((Firm == null && tFirmId != null) )
            {
                var firm = db.Firms.FirstOrDefault(f => f.FirmNumber == tFirmId);
                if (firm != null)
                {
                    Firm = firm;
                    tFirmId = firm.FirmNumber;
                }
                else
                {
                    Firm = null;
                    tFirmId = null;
                }
            }
            if (tFirmId != value)
            {
                var firm = db.Firms.FirstOrDefault(f => f.FirmNumber == value);
                if (firm != null)
                {
                    Firm = firm;
                    tFirmId = firm.FirmNumber;
                }
                else
                {
                    Firm = null;
                    tFirmId = null;
                }

                if (Attachment.GetRootFolder().StorageLocation == HQAttachmentStorageEnum.SharePoint ||
                    Attachment.GetRootFolder().StorageLocation == HQAttachmentStorageEnum.DBAndSharePoint)
                    this.Attachment.SharePointRootFolder = GetSharePointRootFolderPath();
            }

        }

        private string GetUncPath()
        {
            //return null;
            
            return string.Format(@"\Bids - {0} Bids\{1}\{2}", CreatedOn.Year, Firm?.FirmName.Trim() ?? "Unknown", tDescription.Trim());
        }

        public string GetSharePointRootFolderPath()
        {
            if (Firm == null || tDescription == null)
            {
                return null;
            }
            var badChar = new List<string>() { "\"", ",", ".", "*", ":", "<", ">", "?", "/", "\\", "|" };
            var firm = Firm?.FirmName.Trim() ?? "Unknown";
            var project = (tDescription ?? "").Trim();

            badChar.ForEach(c => { firm = firm.Replace(c, ""); });
            badChar.ForEach(c => { project = project.Replace(c, ""); });

            var path = string.Format(@"{0}/{1}", firm, project);

            return path;
        }

        public SPList GetSharePointList()
        {
            var listName = string.Format(@"{0} Bids", CreatedOn.Year);

            var site = db.SPTenates.FirstOrDefault().GetSite("Bids");
            var list = site.GetList(listName, true);
                        
            return list;
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
            ////var db = VPContext.GetDbContextFromEntity(this);;

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
            ////var db = VPContext.GetDbContextFromEntity(this);;

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
            ////var db = VPContext.GetDbContextFromEntity(this);;

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

        private List<BidPackage> _ActivePackages;
        public List<BidPackage> ActivePackages
        {
            get
            {
                if (_ActivePackages == null)
                    _ActivePackages = Packages.Where(f => f.Status != BidStatusEnum.Deleted && f.Status != BidStatusEnum.Canceled).ToList();
                return _ActivePackages;
            }
            set { _ActivePackages = value; }
        }

        private List<BidBoreLine> _ActiveBoreLines;
        public List<BidBoreLine> ActiveBoreLines
        {
            get
            {
                if (_ActiveBoreLines == null)
                    _ActiveBoreLines = BoreLines.Where(f => f.Status != BidStatusEnum.Deleted && f.Status != BidStatusEnum.Canceled).ToList();
                return _ActiveBoreLines;
            }
            set { _ActiveBoreLines = value; }
        }

        public void UpdateStatus(int value)
        {
            if ((Status == BidStatusEnum.Awarded || Status == BidStatusEnum.Update) && ((BidStatusEnum?)value == BidStatusEnum.Deleted || (BidStatusEnum?)value == BidStatusEnum.Canceled))
                return;

            if (WorkFlow == null)
                AddWorkFlow();
            tStatusId = value;
            var status = (BidStatusEnum)value;

            WorkFlow.CreateSequence(value);
            AddWorkFlowAssignments();

            ActivePackages.ForEach(package => {
                if (package.StatusId < value && package.AwardStatus != BidAwardStatusEnum.Awarded)
                {
                    package.Status = status;
                }
                package.PendingBoreLines.ForEach(bore => {
                    bore.Status = status;
                });
            });

            switch (status)
            {
                case BidStatusEnum.Draft:
                    EmailStatusUpdate();
                    break;
                case BidStatusEnum.Estimate:
                    EmailStatusUpdate();
                    break;
                case BidStatusEnum.SalesReview:
                    EmailStatusUpdate();
                    break;
                case BidStatusEnum.FinalReview:
                    EmailStatusUpdate();
                    break;
                case BidStatusEnum.Proposal:
                    EmailStatusUpdate();
                    break;
                case BidStatusEnum.PendingAward:
                    break;
                case BidStatusEnum.ContractApproval:
                    EmailStatusUpdate();
                    break;
                case BidStatusEnum.Awarded:
                    CreateProjects();
                    EmailStatusUpdate();
                    break;
                case BidStatusEnum.NotAwarded:
                    break;
                case BidStatusEnum.Canceled:
                    EmailStatusUpdate();
                    break;
                case BidStatusEnum.Deleted:
                    break;
                case BidStatusEnum.Template:
                    break;
                case BidStatusEnum.ContractReview:
                    EmailStatusUpdate();
                    break;
                case BidStatusEnum.Update:
                    CreateProjects();
                    EmailStatusUpdate();
                    //Forces Status back to awarded.
                    tStatusId = (int)BidStatusEnum.Awarded;
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

                workFlow.AddSequence(this.StatusId);
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
                var assignedusers = WorkFlow.CurrentSequence().AssignedUsers.ToList();
                foreach (var user in assignedusers)
                {
                    WorkFlow.CurrentSequence().AssignedUsers.Remove(user);
                }
            }
            switch (Status)
            {
                case BidStatusEnum.Draft:
                    WorkFlow.AddUser(CreatedUser);
                    break;
                case BidStatusEnum.Estimate:
                    AddBidDivision(Division?.WPDivision);
                    this.ActivePackages.ForEach(e => AddBidDivision(e.Division?.WPDivision, true));
                    break;
                case BidStatusEnum.SalesReview:
                    //WorkFlow.AddUser(CreatedUser);
                    //this.ActivePackages.ForEach(e => { 
                    //            WorkFlow.AddEmployee(Division?.WPDivision?.SalesManager);
                    //            WorkFlow.AddEmployee(Division?.WPDivision?.GeneralManager);
                    //        } );
                    this.ActivePackages.ForEach(e => AddBidDivision(e.Division?.WPDivision, true));
                    break;
                case BidStatusEnum.FinalReview:
                    AddBidDivision(Division?.WPDivision);
                    this.ActivePackages.ForEach(e => AddBidDivision(e.Division?.WPDivision, true));
                    break;
                case BidStatusEnum.Proposal:
                    WorkFlow.AddUser(CreatedUser);
                    AddBidDivision(Division?.WPDivision);
                    this.ActivePackages.ForEach(e => AddBidDivision(e.Division?.WPDivision, true));
                    break;
                case BidStatusEnum.PendingAward:
                    WorkFlow.AddUser(CreatedUser);
                    this.ActivePackages.ForEach(e => AddBidDivision(e.Division?.WPDivision, true));
                    break;
                case BidStatusEnum.ContractReview:
                    WorkFlow.AddUsersByPosition("CFO");
                    break;
                case BidStatusEnum.ContractApproval:
                    WorkFlow.AddUsersByPosition("FIN-ARMGR");
                    WorkFlow.AddUsersByPosition("FIN-AR");
                    break;
                case BidStatusEnum.Awarded:
                    WorkFlow.AddUsersByPosition("FIN-ARMGR");
                    WorkFlow.AddUsersByPosition("FIN-AR");
                    this.ActivePackages.ForEach(e => AddBidDivision(e.Division?.WPDivision, true));
                    WorkFlow.CompleteWorkFlow();
                    break;
                case BidStatusEnum.Update:
                    AddBidDivision(Division?.WPDivision);
                    this.ActivePackages.ForEach(e => AddBidDivision(e.Division?.WPDivision, true));
                    break;
                case BidStatusEnum.NotAwarded:
                    WorkFlow.AddUser(CreatedUser);
                    WorkFlow.CompleteWorkFlow();
                    break;
                case BidStatusEnum.Canceled:
                    WorkFlow.AddUser(CreatedUser);
                    WorkFlow.CompleteWorkFlow();
                    break;
                default:
                    break;
            }
            //WorkFlow.AddUsersByPosition("CEO");

        }

        private void AddBidDivision(CompanyDivision division, bool includeParent = false)
        {
            if (division == null)
                return;
            WorkFlow.AddEmployee(division.BidManager);
            WorkFlow.AddEmployee(division.SalesManager);
            WorkFlow.AddEmployee(division.GeneralManager);
            WorkFlow.AddEmployee(division.DivisionManger);

            if (includeParent)
            {
                AddBidDivision(division.ParentDivision);
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
            var result = new EmailModels.BidEmailViewModel(this);
            var viewPath = "";
            var subject = "";
            using var db = new VPContext();

            //viewPath = "../SM/Request/Email/EmailSubmit";
            switch (Status)
            {
                case BidStatusEnum.Draft:
                    break;
                case BidStatusEnum.Estimate:
                    viewPath = "Email/EmailSalesSubmit";
                    subject = string.Format("Review for Estimate {0} from: {1} ", result.BidInfo.Description, result.BidInfo.CreatedUserName);
                    break;
                case BidStatusEnum.SalesReview:
                    viewPath = "Email/EmailEstimateSubmit";
                    subject = string.Format("Bid Pricing Review for {0} from: {1} ", result.BidInfo.Description, result.BidInfo.CreatedUserName);
                    break;
                case BidStatusEnum.FinalReview:
                    viewPath = "Email/EmailFinalReviewSubmit";
                    subject = string.Format("Final Review for {0} from: {1} ", result.BidInfo.Description, result.BidInfo.CreatedUserName);
                    break;
                case BidStatusEnum.Proposal:
                    viewPath = "Email/EmailProposalSent";
                    subject = string.Format("Bid Ready for Proposal for {0} from: {1} ", result.BidInfo.Description, result.BidInfo.CreatedUserName);
                    break;
                case BidStatusEnum.PendingAward:
                    //result = new Models.Views.Bid.Wizard.Email.EmailEstimateViewModel(bid);
                    //viewPath = "Email/EmailEstimateSubmit";
                    //subject = string.Format("Bid Pricing Review for {0} from: {1} ", result.BidInfo.Description, result.BidInfo.CreatedUserName);
                    break;
                case BidStatusEnum.ContractReview:
                    viewPath = "Email/EmailContractReview";
                    subject = string.Format("Bid Sent for Contract Review for {0} from: {1} ", result.BidInfo.Description, result.BidInfo.CreatedUserName);
                    break;
                case BidStatusEnum.ContractApproval:
                    ;
                    viewPath = "Email/EmailContractApproval";
                    subject = string.Format("Bid Sent for Job Creation for {0} from: {1} ", result.BidInfo.Description, result.BidInfo.CreatedUserName);
                    break;
                case BidStatusEnum.Awarded:
                    viewPath = "Email/EmailAwarded";
                    subject = string.Format("Bid has been Awarded and Jobs Created for {0} from: {1} ", result.BidInfo.Description, result.BidInfo.CreatedUserName);
                    break;
                case BidStatusEnum.NotAwarded:
                    break;
                case BidStatusEnum.Canceled:
                    viewPath = "Email/EmailCanceled";
                    subject = string.Format("Bid has been canced for {0} from: {1} ", result.BidInfo.Description, result.BidInfo.CreatedUserName);
                    break;
                case BidStatusEnum.Update:
                    viewPath = "Email/EmailUpdated";
                    subject = string.Format("Bid has been Updated: {0} from: {1} ", result.BidInfo.Description, result.BidInfo.CreatedUserName);
                    break;
                case BidStatusEnum.Deleted:
                    break;
                case BidStatusEnum.Template:
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
                        Body = Services.EmailHelper.RenderViewToString(viewPath, result, false),
                        IsBodyHtml = true,
                        Subject = subject,
                    };

                    foreach (var workFlow in WorkFlow.CurrentSequence().AssignedUsers.ToList())
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
                    if (CreatedUser.Email != db.GetCurrentUser().Email)
                    {
                        msg.CC.Add(new System.Net.Mail.MailAddress(db.GetCurrentUser().Email));
                    }
                    Services.EmailHelper.Send(msg);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public void ImportScopeTemplate()
        {
            using var db = new VPContext();
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

        public BidProposalScope AddScope(ScopeTypeEnum scopeType)
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

        public void Recalculate(bool force = false)
        {
            if (force)
                this.BoreLines.ToList().ForEach(e => e.RecalcNeeded = true);

            foreach (var package in this.ActivePackages)
            {
                package.Recalculate();
            }
        }

        public BidPackage AddPackage()
        {
            var currentGroundDensity = ActivePackages.DefaultIfEmpty().Max(max => max == null ? 1 : max.GroundDensityId);
            var boreTypeId = ActivePackages.DefaultIfEmpty().Max(max => max == null ? null : max.BoreTypeId);
            var marketId = ActivePackages.DefaultIfEmpty().Max(max => max == null ? null : max.MarketId);
            var pipesize = ActivePackages.DefaultIfEmpty().Max(max => max == null ? null : max.PipeSize);

            var package = new BidPackage()
            {
                db = this.db,
                Bid = this,

                BDCo = BDCo,
                BidId = BidId,
                PackageId = this.Packages.DefaultIfEmpty().Max(max => max == null ? 0 : max.PackageId) + 1,
                Description = "",
                tGroundDensityId = 1,
                tDivisionId = DivisionId,
                JCCo = JCCo,
                tIndustryId = IndustryId,
                tMarketId = marketId,
                IncludeOnProposal = true,
                tBoreTypeId = boreTypeId,
                tPipeSize = pipesize ?? 0,
            };

            this.Packages.Add(package);
            this.ActivePackages.Add(package);
            //_ActivePackages = null;
            return package;
        }

        public void SetupDefaultsForPackages()
        {
            foreach (var package in ActivePackages)
            {
                package.ImportDefaultProductionRates();
                package.ImportDefaultCostItems("CI-");
                package.ImportDefaultCostItems("RE-");
            }
        }
    }
}