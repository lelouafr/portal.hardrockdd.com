using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class PMLocate: IAttachment
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

        public static string BaseTableName { get { return "budPMLM"; } }

        public HQAttachment Attachment
        {
            get
            {
                if (HQAttachment != null)
                {
                    return HQAttachment;

                }
                var comp = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == (this.HQCo ?? 1));
                UniqueAttchID ??= Guid.NewGuid();
                var attachment = new HQAttachment()
                {
                    HQCo = comp.HQCo,
                    UniqueAttchID = (Guid)UniqueAttchID,
                    TableKeyId = this.LocateId,
                    TableName = BaseTableName,
                    HQCompanyParm = comp,

                    db = this.db,
                };
                db.HQAttachments.Add(attachment);
                attachment.BuildDefaultFolders();
                HQAttachment = attachment;
                db.BulkSaveChanges();

                return Attachment;
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
                    TableKeyId = this.LocateId,
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

        public int StatusId
        {
            get => tStatusId;
            set => UpdateStatus(value);
        }

        public PMLocateStatusEnum Status
        {
            get => (PMLocateStatusEnum)StatusId;
            set => StatusId = (int)value;
        }

        public int? BidId
        {
            get => tBidId;
            set => UpdateBid(value);
        }

        public string CrossStreet
        {
            get => tCrossStreet;
            set
            {
                if (tCrossStreet != value)
                {
                    tCrossStreet = value;
                    UpdateLocation();
                }
            }
        }

        public string City
        {
            get => tCity;
            set
            {
                if (tCity != value)
                {
                    tCity = value;
                    UpdateLocation();
                }
            }
        }

        public string County
        {
            get => tCounty;
            set
            {
                if (tCounty != value)
                {
                    tCounty = value;
                    UpdateLocation();
                }
            }
        }

        public string Description
        {
            get => tDescription;
            set
            {
                if (tDescription != value)
                {
                    var cord = MapSet.Coords.FirstOrDefault(f => f.Title == value);
                    if (cord == null)
                        cord = MapSet.Coords.FirstOrDefault();
                    if (cord == null)
                    {
                        cord = MapSet.AddCoord();
                        cord.GPSCoords = GPS;
                    }

                    cord.Title = value;
                    tDescription = value;
                }
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
                    var coords = value.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
                    if (coords.Length == 2)
                    {
                        GPSLat = decimal.TryParse(coords[0], out decimal outLat) ? outLat : 0;
                        GPSLong = decimal.TryParse(coords[1], out decimal outLong) ? outLong : 0;

                        var mapCoords = MapSet.Coords.FirstOrDefault(f => f.GPSCoords == value);
                        if (mapCoords == null)
                            mapCoords = MapSet.Coords.FirstOrDefault();
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
                    }
                }
            }
        }

        public string LocateRefIds 
        { 
            get
            {
                var str = "";

                foreach (var seq in Sequences.Where(f => !string.IsNullOrEmpty(f.LocateRefId)))
                {
                    if (!string.IsNullOrEmpty(str))
                        str += " ";
                    str += seq.LocateRefId;
                }

                return str;
            }
            set
            {
                if(!string.IsNullOrEmpty(value))
                {
                    if (value != LocateRefIds)
                    {
                        char[] delimiterChars = { '|', ' ' };
                        var sequances = value.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries).ToList();
                        sequances = sequances.Select(e => e.Trim() .ToUpper() .Replace(".0", "")).ToList();

                        foreach (var seqStr in sequances)
                        {
                            var locSeq = Sequences.FirstOrDefault(f => f.LocateRefId == seqStr);
                            if (locSeq == null)
                            {
                                locSeq = AddSequence();
                                locSeq.LocateRefId = seqStr;
                                locSeq.Status = DB.PMLocateStatusEnum.Active;

                                Sequences.ToList().ForEach(e => {
                                    if (e.SeqId < locSeq.SeqId)
                                    {
                                        e.Status = DB.PMLocateStatusEnum.Closed;
                                    }
                                });
                            }
                        }
                    }
                }
            }
        }

        public void UpdateLocation()
        {
            Location = string.Format("{0}/{1}/{2}", tCrossStreet, tCity, tCounty);
        }


        private void UpdateStatus(int value)
        {
           
            tStatusId = value;
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
                    StateId = bid.StateCodeId;
                    Description = bid.Description;
                    AnticipatedStartDate = bid.StartDate;
                }
                else
                {
                    tBidId = null;
                    BDCo = null;
                    Bid = null;
                }

                if (this.Request.BidId != this.BidId)
                {
                    this.Request.BidId = value;
                }
                this.Assignments.Clear();
            }
            else if (value == null)
            {
                tBidId = null;
                BDCo = null;
                Bid = null;
            }
            if (Status == PMLocateStatusEnum.Import && Bid != null)
            {
                Status = PMLocateStatusEnum.Active;
            }
        }

        public string GetSharePointRootFolderPath()
        {
            throw new NotImplementedException();
        }

        public SPList GetSharePointList()
        {
            throw new NotImplementedException();
        }

        private PMLocateSequence _CurrentSequence;
        public PMLocateSequence CurrentSequence()
        {
            if (_CurrentSequence == null)
            {
                _CurrentSequence = this.Sequences.OrderByDescending(o => o.SeqId).FirstOrDefault();
                if (_CurrentSequence == null)
                {
                    _CurrentSequence = this.AddSequence();
                    db.BulkSaveChanges();
                }
            }
            return _CurrentSequence;
        }

        public PMLocateSequence AddSequence()
        {
            var currentSeq = this.Sequences.OrderByDescending(o => o.SeqId).FirstOrDefault();
            var sequence = new PMLocateSequence()
            {
                Locate = this,
                db = db,

                LocateId = this.LocateId,
                SeqId = this.Sequences.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
                StateId = this.StateId,
                Status = PMLocateStatusEnum.New,
                //StartDate = currentSeq?.EndDate?.AddDays(1),
            };

            this.Sequences.Add(sequence);
            _CurrentSequence = sequence;
            return sequence;
        }

        public PMLocateAssignment AddAssignment()
        {
            var assignment = new PMLocateAssignment()
            {
                Locate = this,
                db = db,

                LocateId = this.LocateId,
                SeqId = this.Assignments.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
            };

            this.Assignments.Add(assignment);
            return assignment;
        }
    }

    public static class PMLocateExt
    {

        public static PMLocate AddLocate(this VPContext db)
        {
            var locate = new PMLocate()
            {
                //Locate = this,
                db = db,
                LocateId = db.GetNextId(PMLocate.BaseTableName),
                StatusId = 0,
                CreatedBy = db.CurrentUserId,
                CreatedOn = DateTime.Now,
            };
            db.PMLocates.Add(locate);
            return locate;
        }

        public static PMLocate AddLocate(this VPContext db, PMLocate_Import import)
        {
            var locate = new PMLocate()
            {
                //Locate = this,
                db = db,
                LocateId = db.GetNextId(PMLocate.BaseTableName),
                Status = PMLocateStatusEnum.Import,
                Description = import.Description,
                Comments = import.Comments,
                GPS = import.GPS,
                RequestedBy = import.RequestedBy,
                RequestedOn = import.OriginalDateDT ?? DateTime.Now,
                CreatedBy = db.CurrentUserId,
                CreatedOn = DateTime.Now,

                TempImportId = import.ImportId,
                TempImportLineId = import.LineId,
            };
            import.LocateId = locate.LocateId;
            db.PMLocates.Add(locate);
            return locate;
        }
    }
}
