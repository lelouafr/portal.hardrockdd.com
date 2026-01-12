using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{

    public partial class CreditCardImage
    {
        public VPContext _db;

        public VPContext db
        {
            set
            {
                _db = value;
            }
            get
            {
                if (_db == null)
                {
                    _db = VPContext.GetDbContextFromEntity(this);

                    if (_db == null)
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }
        public static string BaseTableName { get { return "budCMIB"; } }

        //public HQAttachment Attachment
        //{
        //    get
        //    {
        //        var HQAttachment = db.HQAttachments.FirstOrDefault(f => f.HQCo == this.CCCo && f.UniqueAttchID == this.UniqueAttchID);
        //        var compParm = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == this.CCCo);
        //        if (HQAttachment == null && UniqueAttchID != null)
        //        {
        //            HQAttachment = new HQAttachment()
        //            {
        //                HQCo = this.CCCo,
        //                UniqueAttchID = (Guid)UniqueAttchID,
        //                TableKeyId = this.KeyID,
        //                TableName = BaseTableName,
        //                HQCompanyParm = compParm,
        //            };

        //            db.HQAttachments.Add(HQAttachment);
        //        }
        //        else if (HQAttachment == null && UniqueAttchID == null)
        //        {
        //            HQAttachment = new HQAttachment()
        //            {
        //                HQCo = this.CCCo,
        //                UniqueAttchID = Guid.NewGuid(),
        //                TableKeyId = this.KeyID,
        //                TableName = BaseTableName,
        //                HQCompanyParm = compParm,
        //            };
        //            UniqueAttchID = HQAttachment.UniqueAttchID;
        //            db.HQAttachments.Add(HQAttachment);
        //        }

        //        return HQAttachment;
        //    }
        //}

        public HQAttachment Attachment
        {
            get
            {
                if (HQAttachment != null)
                {
                    if (this.HQAttachment.SharePointList == null && EmployeeId == 100798)
                    {
                        var root = HQAttachment.GetRootFolder();
                        this.HQAttachment.SharePointList = GetSharePointList();
                        root.SharePointFolderPath = GetSharePointRootFolderPath();

                        var folder = root.GetFolderSharePoint();
                        if (folder == null)
                            folder = root.CreateFolderSharePoint();

                        HQAttachment.SharePointRootFolder = root.SharePointFolderPath;
                        root.StorageLocation = DB.HQAttachmentStorageEnum.DBAndSharePoint;
                    }
                    return HQAttachment;
                }
                    
                var compParm = db.HQCompanyParms.FirstOrDefault(f => f.HQCo == this.CCCo);
                UniqueAttchID ??= Guid.NewGuid();
                var attachment = new HQAttachment()
                {
                    HQCo = this.CCCo,
                    UniqueAttchID = (Guid)UniqueAttchID,
                    TableKeyId = this.KeyID,
                    TableName = BaseTableName,
                    HQCompanyParm = compParm,

                    db = this.db,
                };
                db.HQAttachments.Add(attachment);
                attachment.BuildDefaultFolders();
                HQAttachment = attachment;
                db.BulkSaveChanges();

                return Attachment;
            }
        }

        private string GetUncPath()
        {
            return null;
            //if (this.EmployeeId != 100798)
            //    return null;

            //return string.Format(@"\Employee - Documents\{0} ({1})\Credit Card\{2}\{3}", this.Employee.FullName, this.EmployeeId, this.Mth.Year, this.Mth.ToString("MM - MMM"));
        }

        public SPList GetSharePointList()
        {
            var listName = string.Format(@"Documents");

            var site = db.SPTenates.FirstOrDefault().GetSite("Employee");
            var list = site.GetList(listName, true);

            return list;
        }


        public string GetSharePointRootFolderPath()
        {
            if (this.Employee == null)
            {
                return null;
            }
            var badChar = new List<string>() { "\"", ",", ".", "*", ":", "<", ">", "?", "/", "\\", "|" };
            var empName = this.Employee.FullName;

            badChar.ForEach(c => { empName = empName.Replace(c, ""); });

            var path = string.Format(@"{0} ({1})/Credit Card/{2}/{3}", empName, this.EmployeeId, this.Mth.Year, this.Mth.ToString("MM - MMM"));

            return path;
        }
    }
}