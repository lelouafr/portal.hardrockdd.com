using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class PMLocateRequest: IWorkFlow
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
                _db ??= VPContext.GetDbContextFromEntity(this);
                return _db;
            }
        }

        public static string BaseTableName { get { return "budPMLR"; } }

        public WorkFlow WorkFlow
        {
            get
            {
                var workFlow = this.WFWorkFlow;
                if (WFWorkFlow == null)
                {
                    workFlow = AddWorkFlow();
                }

                return workFlow;
            }
        }

        public int StatusId
        {
            get
            {
                return tStatusId;
            }
            set
            {
                if (value != tStatusId)
                {
                    UpdateStatus(value);
                }
            }
        }
        public PMRequestStatusEnum Status { get => (PMRequestStatusEnum)StatusId; set => StatusId = (int)value; }

        public PMLocateTypeEnum LocateType
        {
            get => (PMLocateTypeEnum)LocateTypeId;
            set => LocateTypeId = (int)value;
        }

        public int? BidId
        {
            get => tBidId;
            set => UpdateBid(value);
        }


        public string GPS
        {
            get => tGPS;
            set
            {
                if (tGPS != value)
                {
                    char[] delimiterChars = { ',', ' ' };
                    var coords = value.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
                    if (coords.Length == 2)
                    {
                        GPSLat = decimal.TryParse(coords[0], out decimal outLat) ? outLat : 0;
                        GPSLong = decimal.TryParse(coords[1], out decimal outLong) ? outLong : 0;

                        //var mapCoords = MapSet.Coords.FirstOrDefault(f => f.GPSCoords == value);
                        //if (mapCoords == null)
                        //    mapCoords = MapSet.Coords.FirstOrDefault();
                        tGPS = value;
                        //if (mapCoords == null)
                        //{
                        //   // mapCoords = MapSet.AddCoord();
                        //    mapCoords.GPSCoords = GPS;
                        //    mapCoords.GPSLat = GPSLat;
                        //    mapCoords.GPSLong = GPSLong;
                        //}
                        //if (mapCoords.GPSCoords != value)
                        //{
                        //    mapCoords.GPSCoords = value;
                        //    mapCoords.GPSLat = GPSLat;
                        //    mapCoords.GPSLong = GPSLong;
                        //}
                    }
                    else
                    {
                        GPSLat = null;
                        GPSLong = null;
                        tGPS = null;
                    }
                }
            }
        }
        private void UpdateStatus(int value)
        {;
            if (Status == PMRequestStatusEnum.Import)
            {
                tStatusId = value;
                WorkFlow.CreateSequence(value);
                AddWorkFlowAssignments();
            }
            else
            {
                var oldValue = tStatusId;

                tStatusId = value;
                WorkFlow.CreateSequence(value);
                AddWorkFlowAssignments();
                switch (Status)
                {
                    case PMRequestStatusEnum.New:
                        break;
                    case PMRequestStatusEnum.Submitted:
                        if(oldValue == (int)DB.PMRequestStatusEnum.New)
                            EmailStatusUpdate();
                        break;
                    case PMRequestStatusEnum.Pending:
                        break;
                    case PMRequestStatusEnum.Completed:
                        EmailStatusUpdate();
                        break;
                    case PMRequestStatusEnum.Import:
                        break;
                    default:
                        break;
                }
            }

        }

        private void UpdateBid(int? value)
        {
            if (Bid == null && tBidId != null)
            {
                var bid = db.Bids.FirstOrDefault(f => f.BidId == tBidId);
                if (bid != null)
                {
                    tBidId = bid.BidId;
                    BDCo = bid.BDCo;
                    Bid = bid;
                    AnticipatedStartDate ??= bid.StartDate;
                    StateId = bid.StateCodeId;
                    Description = bid.Description;
                }                    
            }

            if (tBidId != value && value != null)
            {
                var bid = db.Bids.FirstOrDefault(f => f.BidId == value);
                if (bid != null)
                {
                    tBidId = bid.BidId;
                    BDCo = bid.BDCo;
                    Bid = bid;
                    AnticipatedStartDate = bid.StartDate;
                    StateId = bid.StateCodeId;
                    Description = bid.Description;
                }
                else
                {
                    tBidId = null;
                    BDCo = null;
                    Bid = null;
                }
                this.Locates.ToList().ForEach(e => {
                    if (e.BidId != this.BidId)
                    {
                        e.BidId = this.BidId;
                    }
                });
            }
            else if(value == null)
            {
                tBidId = null;
                BDCo = null;
                Bid = null;
            }
            if (Status == PMRequestStatusEnum.Import && Bid != null)
            {
                Status = PMRequestStatusEnum.Completed;
            }
            else if(Bid == null)
            {
                var import = db.PMLocate_Imports.FirstOrDefault(f => f.RequestId == this.RequestId);
                if (import != null)
                {
                    Status = PMRequestStatusEnum.Import;
                    Description = import.Description;
                    AnticipatedStartDate = null;
                }
                    
            }
        }

        public PMLocate AddLocate()
        {
            var locate = new PMLocate()
            {
                Request = this,
                db = db,

                RequestId = this.RequestId,

                LocateId = db.GetNextId(PMLocate.BaseTableName),
                StatusId = 0,
                Description =  Bid?.Description ?? this.Description,
                BDCo = this.BDCo,
                BidId = this.BidId,
                AnticipatedStartDate = this.AnticipatedStartDate,

                StateId = this.StateId,
                RequestedBy = this.RequestedBy,
                RequestedOn = this.RequestedOn,

                CreatedBy = db.CurrentUserId,
                CreatedOn = DateTime.Now,
                HQCo = db.GetCurrentEmployee().PRCompanyParm.HQCompanyParm.HQCo,

            };
            this.Locates.Add(locate);

            return locate;
        }

        public PMLocate AddLocate(PMLocate_Import import)
        {
            var locate = new PMLocate()
            {
                Request = this,
                db = db,

                LocateId = db.GetNextId(PMLocate.BaseTableName),
                RequestId = this.RequestId,
                Status = PMLocateStatusEnum.Import,
                BDCo = this.BDCo,
                BidId = this.BidId,
                Description = import.Description,
                Location = import.Description,
                Comments = import.Comments,
                GPS = import.GPS,
                RequestedBy = import.RequestedBy,
                RequestedOn = DateTime.TryParse(import.OriginalDate, out DateTime outreqDate) ? outreqDate : DateTime.Now,
                CreatedBy = db.CurrentUserId,
                CreatedOn = DateTime.Now,

                TempImportId = import.ImportId,
                TempImportLineId = import.LineId,
                HQCo = db.GetCurrentEmployee().PRCompanyParm.HQCompanyParm.HQCo,
            };
            if (string.IsNullOrEmpty(locate.tCounty) || string.IsNullOrEmpty(locate.CrossStreet) || string.IsNullOrEmpty(locate.tCity))
            {
                char[] descdelimiterChars = { '/' };
                var parseDesc = locate.Location?.Split(descdelimiterChars, StringSplitOptions.RemoveEmptyEntries);
                if (parseDesc != null)
                {
                    for (int i = 0; i < parseDesc.Length; i++)
                    {
                        if (i == 0)
                            locate.tCrossStreet = parseDesc[i];
                        else if (i == 1)
                            locate.tCrossStreet += "/" + parseDesc[i];
                        else if (i == 2)
                            locate.tCity = parseDesc[i];
                        else if (i == 3)
                            locate.tCounty = parseDesc[i];
                    }
                }
            }

            import.LocateId = locate.LocateId;
            this.Locates.Add(locate);
            return locate;
        }

        public void EmailStatusUpdate()
        {
            if (HttpContext.Current == null)
            {
                return;
            }
            var result = new DB.Infrastructure.EmailModels.LocateRequestEmailViewModel(this);
            var viewPath = "";
            var subject = "";
            using var db = new VPContext();
            viewPath = "Email/RequestEmail";
            switch (Status)
            {
                case PMRequestStatusEnum.New:
                    break;
                case PMRequestStatusEnum.Submitted:
                    subject = string.Format("Locate Requested By {0}", RequestedUser.FullName());
                    break;
                case PMRequestStatusEnum.Pending:
                    break;
                case PMRequestStatusEnum.Completed:
                    subject = string.Format("Locate Request Complete By {0}", RequestedUser.FullName());
                    break;
                case PMRequestStatusEnum.Import:
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
                    //msg.To.Add("glen.lewis@hardrockis.com");
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

        public WorkFlow GetWorkFlow()
        {
            return WorkFlow;
        }

        public WorkFlow AddWorkFlow()
        {
            var workFlow = this.WFWorkFlow;
            if (WFWorkFlow == null)
            {
                var comp = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == 1);
                workFlow = new WorkFlow
                {
                    WFCo = comp.HQCo,
                    WorkFlowId = db.GetNextId(WorkFlow.BaseTableName, 1),
                    TableName = BaseTableName,
                    Id = RequestId,
                    CreatedBy = db.CurrentUserId,
                    CreatedOn = DateTime.Now,
                    Active = true,

                    Company = comp,
                };

                db.WorkFlows.Add(workFlow);
                WFWorkFlow = workFlow;

                WorkFlowId = workFlow.WorkFlowId;
                WFCo = workFlow.WFCo;

                workFlow.AddSequence(this.StatusId);
                this.AddWorkFlowAssignments();
                db.BulkSaveChanges();
            }

            return workFlow;
        }

        public void AddWorkFlowAssignments(bool reset = false)
        {
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
                case PMRequestStatusEnum.New:
                    WorkFlow.AddUser(CreatedUser);
                    break;
                case PMRequestStatusEnum.Submitted:
                    WorkFlow.AddUser(RequestedUser);
                    WorkFlow.AddUsersByPosition("OP-ENGD");
                    break;
                case PMRequestStatusEnum.Pending:
                    WorkFlow.AddUser(RequestedUser);
                    WorkFlow.AddUsersByPosition("OP-ENGD");
                    break;
                case PMRequestStatusEnum.Completed:
                    WorkFlow.AddUser(RequestedUser);
                    WorkFlow.AddUsersByPosition("OP-ENGD");
                    WorkFlow.CompleteSequence();
                    break;
                case PMRequestStatusEnum.Import:
                    WorkFlow.AddUsersByPosition("OP-ENGD");
                    break;
                default:
                    break;
            }
            WorkFlow.AddUsersByPosition("IT-DIR");

        }
    }

    public static class PMLocateRequestExt
    {

        public static PMLocateRequest AddLocateRequest(this VPContext db, PMLocate_Import import)
        {
            var request = new PMLocateRequest()
            {
                //Locate = this,
                db = db,
                RequestId = db.GetNextId(PMLocateRequest.BaseTableName),
                Status  = PMRequestStatusEnum.Import,
                Description = import.Description,
                ProjectName = import.ProjectName,
                General_Import = import.General,
                Comments = import.Comments,
                
                RequestedBy = import.RequestedBy,
                RequestedOn = import.OriginalDateDT ?? DateTime.Now,
                CreatedBy = db.CurrentUserId,
                CreatedOn = DateTime.Now,
            };
            if (request.ProjectName != null && request.ProjectName.Length >= 250 )
            {
                request.ProjectName = import.ProjectName.Substring(0, 249);
                request.Comments += import.ProjectName;
            }
            import.RequestId = request.RequestId;
            db.PMLocateRequests.Add(request);
            return request;
        }
        public static PMLocateRequest AddLocateRequest(this VPContext db, PMLocateRequest request)
        {
            if (request == null)
                return db.AddLocateRequest();

            var locate = new PMLocateRequest()
            {
                db = db,

                BidId = request.BidId,
                RequestId = db.GetNextId(PMLocateRequest.BaseTableName),
                CreatedBy = request.CreatedBy,
                CreatedOn = request.CreatedOn,
                RequestedBy = request.RequestedBy,
                RequestedOn = request.RequestedOn,
                StatusId = request.StatusId,
                LocateTypeId = request.LocateTypeId,
                DivisionId = db.GetCurrentEmployee().DivisionId,
                Comments = request.Comments,
                StateId = request.StateId,
                Description = request.Description,
                AnticipatedStartDate = request.AnticipatedStartDate,
                Status = request.Status,
            };
            db.PMLocateRequests.Add(locate);


            return locate;
        }


        public static PMLocateRequest AddLocateRequest(this VPContext db, PMLocate locate)
        {
            if (locate == null)
                return db.AddLocateRequest();

            var request = new PMLocateRequest()
            {
                db = db,

                RequestId = db.GetNextId(PMLocateRequest.BaseTableName),
                CreatedBy = locate.CreatedBy,
                CreatedOn = locate.CreatedOn,
                RequestedBy = locate.RequestedBy,
                RequestedOn = locate.RequestedOn,
                StatusId = locate.StatusId,
                LocateTypeId = 0,
                DivisionId = db.GetCurrentEmployee().DivisionId,
                Comments = locate.Comments,
                StateId = locate.StateId,
                Description = locate.Description,
                AnticipatedStartDate = locate.AnticipatedStartDate,
                Status = PMRequestStatusEnum.Import,
            };
            locate.RequestId = request.RequestId;
            locate.Request = request;
            request.Locates.Add(locate);
            db.PMLocateRequests.Add(request);


            return request;
        }
        public static PMLocateRequest AddLocateRequest(this VPContext db)
        {
            var locate = new PMLocateRequest()
            {
                db = db,

                RequestId = db.GetNextId(PMLocateRequest.BaseTableName),
                CreatedBy = db.CurrentUserId,
                CreatedOn = DateTime.Now,
                RequestedBy = db.CurrentUserId,
                RequestedOn = DateTime.Now,
                StatusId = 0,
                LocateTypeId = 0,
                DivisionId = db.GetCurrentEmployee().DivisionId,
            };
            db.PMLocateRequests.Add(locate);

            return locate;

        }

        public static PMLocateRequest AddLocateRequest(this VPContext db, Bid bid)
        {
            var locate = new PMLocateRequest()
            {
                db = db,

                RequestId = db.GetNextId(PMLocateRequest.BaseTableName),
                CreatedBy = db.CurrentUserId,
                CreatedOn = DateTime.Now,
                RequestedBy = db.CurrentUserId,
                RequestedOn = DateTime.Now,
                StatusId = 0,
                LocateTypeId = 0,
                DivisionId = db.GetCurrentEmployee().DivisionId,
                BDCo = bid.BDCo,
                BidId = bid.BidId,
            };

            db.PMLocateRequests.Add(locate);

            return locate;

        }
    }
}
