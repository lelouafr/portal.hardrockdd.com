using Steeltoe.Common.Net;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using DB.Infrastructure.ViewPointDB.Data;
using System.Linq;
using DB.Infrastructure.Services;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using ExcelDataReader;
using System.Data;
using Newtonsoft.Json;
using System.Text;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using System.Diagnostics;
namespace ImportPortal
{

    class Program
    {
        static void Main(string[] args)
        {
            ConvertResourceToApplicant();
			//LocateSequanceCorrectStatus();
			//ImportLocateFiles();
			//ProcessLocateFileJson();
			//ProcessLocatedImportDataIntoRequests();
		}

       
		public static void CreateEmployeeCrewHistory()
		{
			using var db = new VPContext();
			var employees = db.PayrollEntries
				.Include("Employee")
				.Include("Employee.Crews")
				.Include("Employee.PRCrew")
				.Where(f => f.CrewId != null)
				.GroupBy(g => new { g.Employee })
				.Select(s => new
				{
					s.Key.Employee,
					list = s.GroupBy(d => new { d.PostDate, d.PRCrew })
					.Select(d => new
					{
						d.Key.PRCrew,
						d.Key.PostDate
					})
					.OrderBy(o => o.PostDate)
					.ToList()
				})
				.OrderBy(o => o.Employee.EmployeeId)
				.ToList();


			foreach (var emp in employees)
			{
				emp.Employee.Crews.Clear();
				PREmployeeCrew prior = null;
				foreach (var date in emp.list)
				{
					var addNew = true;
					if (prior != null && prior.CrewId == date.PRCrew.CrewId)
					{
						addNew = false;
					}

					if (addNew)
					{
						if (prior != null)
							prior.EndDate = date.PostDate.AddDays(-1);

						prior = new PREmployeeCrew()
						{
							PRCo = emp.Employee.PRCo,
							EmployeeId = emp.Employee.EmployeeId,
							PREmployee = emp.Employee,
							SeqId = emp.Employee.Crews.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
							StartDate = date.PostDate,
							CrewId = date.PRCrew.CrewId
						};

						emp.Employee.Crews.Add(prior);

					}
				}

				db.BulkSaveChanges();

			}

			var emps = db.Employees.Where(f => !f.Crews.Any()).ToList();

			foreach (var emp in emps)
			{
				var prior = new PREmployeeCrew()
				{
					PRCo = emp.PRCo,
					EmployeeId = emp.EmployeeId,
					PREmployee = emp,
					SeqId = emp.Crews.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
					StartDate = emp.HireDate,
					CrewId = "ADMIN"
				};

				emp.Crews.Add(prior);
			}
			db.BulkSaveChanges();

		}

		public static void CreateEmployeeDivisionHistory()
		{
            using var db = new VPContext();
            var employees = db.PayrollEntries
                .Include("Employee")
                .Include("Employee.Divisions")
                .Include("JCJob")
                .Include("JCJob.Division")
                .Include("JCJob.Division.WPDivision")
                .Where(f => f.JobId != null)
                .GroupBy(g => new { g.Employee })
                .Select(s => new
                {
                    s.Key.Employee,
                    list = s.GroupBy(d => new { d.PostDate, d.JCJob })
                    .Select(d => new
                    {
                        d.Key.JCJob,
                        d.Key.JCJob.Division.WPDivision,
                        d.Key.PostDate
                    })
                    .OrderBy(o => o.PostDate)
                    .ToList()
                })
                .OrderBy(o => o.Employee.EmployeeId)
                .ToList();


            foreach (var emp in employees)
            {
                emp.Employee.Divisions.Clear();
                PREmployeeDivision prior = null;
                foreach (var date in emp.list)
                {
                    var addNew = true;
                    if (prior != null && prior.DivisionId == date.WPDivision.DivisionId)
                    {
                        addNew = false;
                    }

                    if (addNew)
                    {
                        if (prior != null)
                            prior.EndDate = date.PostDate.AddDays(-1);

                        prior = new PREmployeeDivision()
                        {
                            PRCo = emp.Employee.PRCo,
                            EmployeeId = emp.Employee.EmployeeId,
                            PREmployee = emp.Employee,
                            SeqId = emp.Employee.Divisions.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
                            StartDate = date.PostDate,
                            DivisionId = date.WPDivision.DivisionId
                        };

                        emp.Employee.Divisions.Add(prior);

                    }
                }

                db.BulkSaveChanges();

            }

            var emps = db.Employees.Where(f => !f.Divisions.Any()).ToList();

            foreach (var emp in emps)
            {
				var prior = new PREmployeeDivision()
				{
					PRCo = emp.PRCo,
					EmployeeId = emp.EmployeeId,
					PREmployee = emp,
					SeqId = emp.Divisions.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
					StartDate = emp.HireDate,
					DivisionId = 1
				};

				emp.Divisions.Add(prior);
			}
			db.BulkSaveChanges();

		}
        public class LogInfo
        {
            public long InstanceId { get; set; }
            public long EventId { get; set; }
            public string MachineName { get; set; }
            public string UserName { get; set; }
            public DateTime EventTime { get; set; }

            public string Msg { get; set; }
        }

        public static void UpdateAssetPhoneNumbers()
        {

            using var db = new VPContext();
            var assets = db.HRCompanyAssets.Where(f => f.AssetCategory == "Phone").ToList();
            foreach (var asset in assets)
            {
                var model = new portal.Areas.HumanResource.Models.Assets.AssetViewModel(asset);
                model.ProcessUpdate(db, null);
            }
        }

        public static void LocateSequanceCorrectStatus()
        {
            using var db = new VPContext();
            var locates = db.PMLocates.Include("Sequences").ToList();

            foreach (var loc in locates)
            {
                var cur = loc.CurrentSequence();
                foreach (var seq in loc.Sequences)
                {
                    if (seq.SeqId < cur.SeqId)
                        seq.Status = DB.PMLocateStatusEnum.Closed;
                }

                if (cur.EndDate <= DateTime.Now.AddDays(-30).Date)
                {
                    cur.Status = DB.PMLocateStatusEnum.Closed;
                }

                if (cur.EndDate >= DateTime.Now.AddDays(-14).Date && cur.EndDate >= DateTime.Now.AddDays(-30).Date)
                {
                    cur.Status = DB.PMLocateStatusEnum.Closed;
                }
            }

            db.BulkSaveChanges();
        }

        public static void FixSharePointList()
        {
            using var db = new VPContext();
            var tenate = db.SPTenates.FirstOrDefault();
            var site = tenate.GetSite("Jobs");
            db.BulkSaveChanges();

        }

        private static void ProcessLocateFileJson()
        {
            using var db = new VPContext();
            var imports = db.WPExcelImports.ToList();
            imports = imports.Where(f => f.Processed == false).OrderByDescending(o => o.LastModifiedDate).ToList();
            var requestors = db.budPMLM_RequestNames.ToList();
            foreach (var import in imports)
            {
                import.LocateImportLines.Clear();
                foreach (var sheet in import.Sheets)
                {
                    var json = sheet.GetJson();
                    var list = sheet.PMLocateData();

                    var lineId = import.LocateImportLines.DefaultIfEmpty().Max(max => max == null ? 0 : max.LineId) + 1;
                    list.ForEach(e => {
                        e.LineId = lineId++;
                        e.SheetId = sheet.SheetId;
                        //e.Description ??= "";
                        //e.Comments ??= "";
                        //e.General ??= "";
                        //e.Owner ??= "";
                        //e.ProjectName ??= "";
                        //e.GPS ??= "";
                        if (e.RequestedBy == null)
                        {
                            e.RequestedBy = requestors.FirstOrDefault(f => f.RequestName == (e.RequestName ?? " "))?.RequestedBy;
                            e.RequestedBy ??= "System";
                        }
                        if (e.Description != null)
                        {
                            e.Description = e.Description.Trim();
                            e.Description = e.Description.Length > 250 ? e.Description.Substring(0, 249) : e.Description;
                        }
                        if (e.Comments != null)
                        {
                            e.Comments = e.Comments.Trim();
                            e.Comments = e.Comments.Length > 1000 ? e.Comments.Substring(0, 999) : e.Comments;
                        }
                        if (e.General != null)
                        {
                            e.General = e.General.Trim();
                            e.General = e.General.Length > 1000 ? e.General.Substring(0, 999) : e.General;
                        }
                        if (e.Owner != null)
                        {
                            e.Owner = e.Owner.Trim();
                            e.Owner = e.Owner.Length > 1000 ? e.Owner.Substring(0, 999) : e.Owner;
                        }
                        if (e.ProjectName != null)
                        {
                            e.ProjectName = e.ProjectName.Trim();
                            e.ProjectName = e.ProjectName.Length > 1000 ? e.ProjectName.Substring(0, 999) : e.ProjectName;
                        }
                        if (e.GPS != null)
                        {
                            e.GPS = e.GPS.Trim();
                            e.GPS = e.GPS.Replace("Â°", "");
                            e.GPS = e.GPS.Length > 100 ? e.GPS.Substring(0, 99) : e.GPS;
                        }


                        import.LocateImportLines.Add(e);
                    });
                    db.BulkSaveChanges();
                    Console.WriteLine(string.Format("Processed {0}, sheet {1}, rows: {2},", import.FileName, sheet.SheetName, list.Count));

                }
                import.Processed = true;
                db.BulkSaveChanges();

            }

        }

        private static void ImportLocateFiles()
        {

            LocateImportFolder(@"C:\Users\glen.lewis\Hard Rock Directional Drilling\Locates - Documents");
            //_xlApp.Quit();
            //Marshal.ReleaseComObject(_xlApp);
        }

        private static void LocateImportFolder(string folderLoc)
        {
            foreach (var subFolder in Directory.GetDirectories(folderLoc))
            {
                LocateImportFolder(subFolder);
            }

            using var db = new VPContext();
            List<PMLocateSequence> locSeqs = null;// = db.PMLocateSequences.ToList();
           

            //var tasks = new List<System.Threading.Tasks.Task>();
            var files = Directory.GetFiles(folderLoc).Select(s => new FileInfo(s)).ToList().OrderByDescending(f => f.LastWriteTime).ToList();
            foreach (var fileInfo in files)
            {
                if (fileInfo.LastWriteTime.Year >= 2022)
                {
                    if (fileInfo.Extension.Contains("xls") ||
                        fileInfo.Extension.Contains("xlsx") &&
                        fileInfo.Name.ToLower().Contains("tracker"))
                    {
                        Console.WriteLine(string.Format("{0} Added", fileInfo.Name));
                        LoadExcelFile(fileInfo);
                    }
                    else if (fileInfo.Extension.Contains("pdf") && fileInfo.Name.ToLower().Contains("ticket"))
                    {
                        if  (locSeqs == null)
                            locSeqs = db.PMLocateSequences.Where(f => f.LocateRefId != null).ToList();
                        Console.WriteLine(string.Format("{0} Added", fileInfo.Name));

                        var fileName = fileInfo.Name.ToLower();
                        fileName = fileName.Replace("ticket", "");
                        fileName = fileName.Replace(fileInfo.Extension, "");
                        fileName = fileName.Trim();

                        if (locSeqs.Where(f => f.LocateRefId.ToLower() == fileName).Any())
                        {
                            var seq = db.PMLocateSequences.FirstOrDefault(f => f.LocateRefId.ToLower() == fileName);
                            ImportLocatePdf(fileInfo, seq);
                        }
                    }
                }                    
            }
            
            //tasks.ForEach(f => f.Start());
            //System.Threading.Tasks.Task.WaitAll(tasks.ToArray());
        }
        
        public static void ImportLocatePdf(FileInfo fileInfo, PMLocateSequence? seq)
        {
           
            if (seq != null)
            {
                var s = seq;
                var l = s.Locate;
                if (!s.Attachment.Files.Any(f => f.OrigFileName == fileInfo.Name))
                {
                    Console.WriteLine(string.Format("{0} Added", fileInfo.Name));
                    var ms = new MemoryStream();
                    FileStream stream = System.IO.File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read);
                    stream.CopyTo(ms);

                    s.Attachment.GetRootFolder().AddFile(fileInfo.Name, ms.ToArray());
                    seq.db.BulkSaveChanges();
                    stream.Dispose();
                    ms.Dispose();
                }
            }
        }

        public static void ProcessLocatedImportDataIntoRequests()
        {
            using var db = new VPContext();
            var imports = db.PMLocate_Imports
                .Where(f => f.GPS != null && 
                            f.RefId != null && 
                            (f.Import.FileName.Contains("22") || f.Import.FileName.Contains("21")) &&
                            f.ExcelSheet.SheetName != "CLOSED PROJECTS"
                    )
                .ToList()
                ;
            var requestors = db.budPMLM_RequestNames.ToList();
            foreach (var import in imports)
            {
                if (import.RequestedBy == null)
                {
                    import.RequestedBy = requestors.FirstOrDefault(f => f.RequestName == (import.RequestName ?? " "))?.RequestedBy;
                    import.RequestedBy ??= "System";
                }
            }
            db.BulkSaveChanges();
            //imports = imports.Where(f => f.Description == "I-10 FRONTAGE/HOEFS RD/PECOS/REEVES").ToList();
            //imports = imports.Where(f => f.Import.Sheets.FirstOrDefault(s => s.SheetId == f.SheetId).SheetName != "CLOSED PROJECTS").ToList();
            var importsG = imports.GroupBy(g => new
            {
                g.SheetId,
                g.Description,
                g.Comments,
                g.GPS,
                g.RequestedBy,
                g.General,
                g.Owner,
                g.ProjectName,
                g.OriginalDate,
                g.RequestName,
                g.RefId,
                g.StartDate,
                g.EndDate
            }).Select(s => new PMLocate_Import() {                
                SheetId = s.Key.SheetId,
                Description = s.Key.Description,
                Comments = s.Key.Comments,
                GPS = s.Key.GPS,
                RequestedBy = s.Key.RequestedBy,
                General = s.Key.General,
                Owner = s.Key.Owner,
                ProjectName = s.Key.ProjectName,
                OriginalDate = s.Key.OriginalDate,
                RequestName = s.Key.RequestName,
                RefId = s.Key.RefId,
                StartDate = s.Key.StartDate,
                EndDate = s.Key.EndDate,
                ImportId = s.Max(max => max.ImportId),
                LineId = s.Where(f => f.ImportId == s.Max(max => max.ImportId)).Max(s => s.LineId),
                RequestId = s.Max(max => max.RequestId ?? 0),
                LocateId = s.Max(max => max.LocateId ?? 0),
            })
            .OrderByDescending(o => o.OriginalDateDT)
            .ToList();


            var requests = db.PMLocateRequests.Include("Locates").ToList();
            var locateSeqs = db.PMLocateSequences.Include("Locate").ToList();
            var owners = db.budPMLM_Owners.ToList();
            char[] delimiterChars = { '|' };
            foreach (var importg in importsG)
            {
                var import = imports.FirstOrDefault(f => f.ImportId == importg.ImportId && f.LineId == importg.LineId);

                if (import.RequestId == null || import.LocateId == null || true)
                {
                    var refs = import.RefId.Replace("\r\n", " ").Trim();
                    refs = import.RefId.Replace(" ", "|").Trim();

                    if (import.RequestedBy == null)
                    {
                        import.RequestedBy = requestors.FirstOrDefault(f => f.RequestName == (import.RequestName ?? " "))?.RequestedBy;
                        import.RequestedBy ??= "System";

                    }

                    var sequances = refs.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries).ToList();
                    sequances = sequances.Select(e => e.Trim()
                                                        .ToUpper()
                                                        .Replace(".0", "")
                                                        //.Replace("P", "")
                                                        )
                        .ToList();
                    var locSeq = locateSeqs.FirstOrDefault(f => sequances.Contains(f.LocateRefId));
                    var locate = locSeq?.Locate;
                    PMLocateRequest request = null;

                    if (!string.IsNullOrEmpty(import.Description))
                    {
                        request = requests.FirstOrDefault(f => f.Description == import.Description &&
                                                               f.ProjectName == import.ProjectName &&
                                                               f.General_Import == import.General &&
                                                               f.RequestedBy == import.RequestedBy);
                        //if (request == null)
                        //    request = requests.FirstOrDefault(f => f.Description == import.Description && f.RequestedBy == import.RequestedBy);

                        //if (request == null)
                        //    request = requests.FirstOrDefault(f => f.Description == import.Description);
                        if (request == null)
                            request = locate?.Request;

                        if (request != null)
                        {
                            if (request.Description != import.Description)
                                request.Description ??= import.Description;
                            if (request.ProjectName != import.ProjectName)
                            {
                                request.ProjectName ??= import.ProjectName;
                                if (request.ProjectName != null && request.ProjectName.Length >= 250)
                                {
                                    request.ProjectName = import.ProjectName.Substring(0, 249);
                                    request.Comments += import.ProjectName;
                                }
                            }
                            if (request.General_Import != import.General)
                                request.General_Import ??= import.General;
                            if (request.RequestedBy != import.RequestedBy)
                                request.RequestedBy ??= import.RequestedBy;
                        }
                    }
                    else
                    {
                        request = locate?.Request;
                    }

                    if (request == null)
                    {
                        request = db.AddLocateRequest(import);
                        requests.Add(request);
                    }

                    if (locate == null)
                    {
                        locate = request.AddLocate(import);
                    }
                    else
                    {
                        if (locate.Status == DB.PMLocateStatusEnum.Import)
                        {

                            if (locate.RequestId != request.RequestId)
                            {
                                locate.RequestId = request.RequestId;
                                locate.Request = request;
                            }

                            if (locate.Description != import.Description && locate.Description == null)
                                locate.Description = import.Description;

                            if (locate.Location != import.Description)
                            {
                                locate.Location = import.Description;
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

                            if (locate.Comments != import.Comments)
                                locate.Comments = import.Comments;

                            if (locate.GPS != import.GPS && locate.GPS == null)
                                locate.GPS = import.GPS;

                            if (locate.RequestedBy != import.RequestedBy)
                                locate.RequestedBy = import.RequestedBy;

                            var reqDate = DateTime.TryParse(import.OriginalDate, out DateTime outreqDate) ? outreqDate : DateTime.Now;
                            if (locate.RequestedOn != reqDate)
                                locate.RequestedOn = reqDate;

                            if (locate.TempImportId != import.ImportId && locate.TempImportId == null)
                            {
                                locate.TempImportId = import.ImportId;
                                locate.TempImportLineId = import.LineId;
                            }
                            if (!string.IsNullOrEmpty(locate.Location) &&
                                (string.IsNullOrEmpty(locate.County) ||
                                string.IsNullOrEmpty(locate.CrossStreet) ||
                                string.IsNullOrEmpty(locate.City)))
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
                        }
                    }

                    if (import.LocateId != locate.LocateId && import.LocateId == null)
                        import.LocateId = locate.LocateId;

                    foreach (var seqStr in sequances)
                    {
                        var seq = seqStr.Trim().ToUpper();
                        if (seq.Contains(".0"))
                            seq = seq.Replace(".0", "");
                        locSeq = locateSeqs.FirstOrDefault(f => f.LocateRefId == seq);
                        if (locSeq == null)
                        {

                            locSeq = locate.AddSequence();
                            locSeq.StartDate = null;
                            locSeq.EndDate = null;
                            locSeq.LocateRefId = seq;
                            locSeq.Status = DB.PMLocateStatusEnum.Active;
                            locateSeqs.Add(locSeq);

                            locate.Sequences.ToList().ForEach(e => {
                                if (e.SeqId < locSeq.SeqId)
                                {
                                    e.Status = DB.PMLocateStatusEnum.Closed;
                                }
                            });
                        }

                        if (locSeq.StartDate != null && locSeq.EndDate == null)
                        {
                            locSeq.EndDate = locSeq.StartDate.Value.AddDays(14);
                        }
                    }

                    locSeq = locate.Sequences.FirstOrDefault(f => f.LocateRefId == sequances.LastOrDefault());
                    if (locSeq != null)
                    {
                        if (import.StartDateDT != locSeq.StartDate && import.StartDateDT != null)
                            locSeq.StartDate = import.StartDateDT;

                        if (import.EndDateDT != locSeq.EndDate && import.EndDateDT != null)
                            locSeq.EndDate = import.EndDateDT;

                        if (locSeq.StartDate != null && locSeq.EndDate == null)
                            locSeq.EndDate = locSeq.StartDate.Value.AddDays(14);
                    }

                    locSeq = locate.Sequences.FirstOrDefault(f => f.LocateRefId == sequances.FirstOrDefault());
                    if (locSeq != null)
                    {
                        if (locSeq.StartDate == null)
                        {
                            DateTime? startDate = import.OriginalDateDT;
                            if (startDate != locSeq.StartDate)
                                locSeq.StartDate = startDate;
                        }

                        if (locSeq.EndDate == null)
                        {
                            if (locSeq.StartDate != null)
                                locSeq.EndDate = locSeq.StartDate.Value.AddDays(14);
                        }
                    }


                    if (locate.Request == null)
                    {
                        db.AddLocateRequest(locate);
                    }
                    if (import.RequestId != locate.RequestId)
                        import.RequestId = locate.RequestId;

                    db.BulkSaveChanges();
                }
                Console.WriteLine(string.Format("{0} - {1} Processed Locate: {2}", import.ImportId, import.LineId, import.Description));
            }
        }
        
        public static void LoadExcelFile(FileInfo fileInfo)
        {
            using var db = new VPContext();
            var import = db.AddExcelImport(fileInfo);
            //var import = db.WPExcelImports.FirstOrDefault(f => f.FilePath == fileInfo.FullName);
            //if (import == null)
            var headerRowIndex = 0;
            if (!import.Sheets.Any() || import.LastModifiedDate != fileInfo.LastWriteTime)
            {
                if (import.ImportTypeId != 1)
                    import.ImportTypeId = 1;
                if (import.LastModifiedDate != fileInfo.LastWriteTime)
                {
                    import.LastModifiedDate = fileInfo.LastWriteTime;
                    import.Processed = false;
                }

                FileStream stream = System.IO.File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelReader = ExcelReaderFactory.CreateReader(stream);
                
                DataSet result = excelReader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                    {
                            
                        EmptyColumnNamePrefix = "Column ",
                        UseHeaderRow = true,
                        ReadHeaderRow = (reader) =>
                        {
                            bool empty = true;
                            headerRowIndex = 0;

                            while (empty)
                            {
                                var fldCnt = reader.FieldCount >= 50 ? 50 : reader.FieldCount;

                                for (var i = 0; i < fldCnt && empty; i++)
                                {
                                    var val = reader[i]?.ToString();
                                    empty = string.IsNullOrWhiteSpace(val);
                                }

                                if (empty)
                                {
                                    empty = reader.Read(); // Only continue if more content is available
                                    headerRowIndex++; // Keep track of the first row position.
                                }
                            }
                        },
                        FilterRow = (reader) =>
                        {
                            bool empty = true;
                            var fldCnt = reader.FieldCount >= 50 ? 50 : reader.FieldCount;

                            for (var i = 0; i < fldCnt && empty; i++)
                            {
                                var val = reader[i]?.ToString();
                                empty = string.IsNullOrWhiteSpace(val);
                            }

                            return !empty;
                        },
                        FilterColumn = (reader, index) =>
                        {
                            bool empty = false;
                            string sheet = reader.Name;
                            if (index >= 50)
                                return false;

                            // Start reading the table from the beginning
                            reader.Reset();

                            // EDIT: Head over the our current excel sheet
                            while (reader.Name != sheet)
                                if (!reader.NextResult())
                                    break;


                            // Head to the first row with content
                            int rowIndex = 0;
                            while (rowIndex < headerRowIndex)
                            {
                                reader.Read();
                                rowIndex++;
                            }

                            var sampleRow = 0;
                            while (reader.Read())
                            {
                                sampleRow++;
                                if (sampleRow >= 50)
                                {
                                    break;
                                }
                                // Decide if the current column is empty
                                if (reader[index] == null || string.IsNullOrEmpty(reader[index].ToString()))
                                    continue;

                                empty = true;
                                break;
                            }

                            // Start over again (This allows the reader to automatically read the rest of the content itself)
                            reader.Reset();

                            // EDIT: Head over the our current excel sheet
                            while (reader.Name != sheet)
                                if (!reader.NextResult())
                                    break;

                            reader.Read();

                            // Head over to the first row with content
                            rowIndex = 0;
                            while (rowIndex < headerRowIndex)
                            {
                                reader.Read();
                                rowIndex++;
                            }

                            // Return info on whether this column should be ignored or not.
                            return empty;
                        }
                    }    
                });

                var collection = result.Tables;
                foreach (DataTable table in result.Tables)
                {
                    var colIdx = 1;
                    foreach (DataColumn col in table.Columns)
                    {
                        var name = col.ColumnName.Trim();
                        
                        if (name.ToLower() == "texas 811" ||
                            name.ToLower() == "column 0" ||
                            name.ToLower() == "cbud #" ||
                            (name.ToLower() == "" && colIdx == 1))
                            name = "RefId";
                        else if (name.ToLower() == "" && colIdx == 11)
                            name = "Project Name";
                        else if (name.ToLower() == "expire date")
                            name = "Expiration";
                        else if (name.ToLower() == "job number" ||
                                 name.ToLower() == "job#")
                            name = "JobId";
                        else if (name == "")
                            name = string.Format("Column {0}", colIdx);

                        col.ColumnName = name;
                        colIdx++;
                    }
                    var sheet = import.AddSheet(table.TableName);
                    Console.WriteLine(string.Format("{0}, sheet {1}, rows: {2},",import.FileName, sheet.SheetName, table.Rows.Count));
                    var json = JsonConvert.SerializeObject(table);

                    var data = Encoding.UTF8.GetBytes(json);

                    sheet.JsonData = data;
                }
                excelReader.Dispose();
                stream.Dispose();
            }
            db.BulkSaveChanges();
        }

        //private static Excel.Application _xlApp;
        //private static void LocateImportFile(string filePath)
        //{
        //    if (_xlApp == null)
        //        _xlApp = new Excel.Application();
        //    var xlApp = _xlApp;
        //    try
        //    {

        //        Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(filePath);

        //        foreach (var xlWorksheet in xlWorkbook.Sheets)
        //        {
        //            var xlRange = ((Excel._Worksheet)xlWorksheet).UsedRange;
        //            var headerCheck = new List<string>() { 
        //                "TEXAS 811",
        //                "Lone Star 811",
        //                "Description Owner",
        //                "General",
        //                "Sub",
        //                "Job#",
        //                "Start Date",
        //                "Expiration",
        //                "GPS Coordinates",
        //                "Project Name",
        //                "Comments",
        //                "Original Request Date",
        //                "Requested By"
        //            };
        //            var headers = new List<string>();
        //            Excel.Range firstRow = xlRange.Rows[1];
        //            for (int c = 1; c <= firstRow.Columns.Count; c++)
        //            {
        //                Excel.Range header = (Excel.Range)firstRow.Columns[c];
        //                headers.Add(header.Value2 != null ? header.Value2.ToString(): string.Empty);

        //                Marshal.ReleaseComObject(header);
        //            }
        //            var headerMatchCnt = headerCheck.Where(f => headers.Any(a => a == f)).Count();
        //            if (headerMatchCnt == headerCheck.Count)
        //            {
        //                var iRow = 2;

        //                for (int r = iRow; r <= xlRange.Rows.Count; r++)
        //                {

        //                    Excel.Range row = (Excel.Range)xlRange.Rows[r];
        //                    var dic = new Dictionary<string, string>();
        //                    for (int c = 1; c <= row.Columns.Count; c++)
        //                    {
        //                        Excel.Range val = (Excel.Range)firstRow.Columns[c];
        //                        dic.Add(headers[c], val.Value2.ToString());
        //                        Marshal.ReleaseComObject(val);
        //                    }
        //                    Marshal.ReleaseComObject(row);
        //                }
        //            }

        //            Marshal.ReleaseComObject(firstRow);
        //            Marshal.ReleaseComObject(xlRange);
        //        }
        //        xlWorkbook.Close();
        //        Marshal.ReleaseComObject(xlWorkbook);
        //    }
        //    catch (Exception)
        //    {

        //    }
        //}

        public static void MoveDTJob()
        {
            using var db = new VPContext();
            var calendar = db.Calendars.FirstOrDefault(f => f.Date == new DateTime(2022, 10, 31));
            var weekId = db.Calendars.FirstOrDefault(f => f.Date == new DateTime(2022, 10, 31)).Week ?? 0;
            var dates = db.Calendars.Where(f => f.Week == weekId).Select(s => s.Date).ToList();
            var tickets = db.DailyTickets.Where(f => f.FormId == (int)DB.DTFormEnum.JobFieldTicket &&
                                                    f.tWorkDate >= dates.Min() && f.tWorkDate <= dates.Max() &&
                                                    (f.tStatusId == (int)DB.DailyTicketStatusEnum.Submitted || 
                                                    f.tStatusId == (int)DB.DailyTicketStatusEnum.Approved || 
                                                    f.tStatusId == (int)DB.DailyTicketStatusEnum.Draft)            
                            ).ToList();

            foreach (var ticket in tickets)
            {
                if (ticket.UniqueAttchID != null)
                {
                    if (ticket.HQAttachment.GetRootFolder().StorageLocation == DB.HQAttachmentStorageEnum.DB)
                    {
                        var att = ticket.Attachment;
                        Console.WriteLine(string.Format("{0} - {1}", ticket.TicketId, att.SharePointRootFolder));
                        System.Threading.Thread.Sleep(5000);
                    }
                }
            }
        }

        public static void MapCCToSharePoint()
        {
            using var db = new VPContext();
            var trans = db.CreditCardImages.Where(f => f.EmployeeId == 100798 && f.AttachmentId != null).ToList();
            foreach (var image in trans)
            {
                var att = image.Attachment;
                var file = att.Files.FirstOrDefault(f => f.AttachmentId == image.AttachmentId);
                if (file != null)
                    file.ThumbnailAttachmentID = image.ThumbAttachmentId;

                db.BulkSaveChanges();
            }
        }

        public static void MapBidToSharePoint()
        {

            using var db = new VPContext();
            var bids = db.Bids.Where(f => f.CreatedOn.Year == 2022 && f.tDescription != null && f.tFirmId != null).OrderByDescending(f => f.BidId).ToList();

            foreach (var bid in bids)
            {
                var att = bid.Attachment;
                var root = att.GetRootFolder();
                if(root.StorageLocation == DB.HQAttachmentStorageEnum.DB)
                {
                    var folderLoc = bid.GetSharePointRootFolderPath();
                    var list = bid.GetSharePointList();
                    Console.WriteLine(string.Format("{0} - {1}", folderLoc, bid.BidId));

                    att.SharePointList = list;

                    root.SharePointFolderPath = folderLoc;
                    var folder = root.GetFolderSharePoint();
                    if (folder != null)
                    {
                        var i = 0;
                    }
                    if (folder == null)
                        folder = root.CreateFolderSharePoint();
                    att.SharePointRootFolder = root.SharePointFolderPath;
                    root.StorageLocation = DB.HQAttachmentStorageEnum.SharePoint;

                    //root.SyncFromSource();
                    db.BulkSaveChanges();
                }
            }
        }

        public static void MergeFirms()
        {
            using var db = new VPContext();
            var firms = db.Firms.Where(f => f.FirmTypeId == "OWNER").ToList();
            var delList = new List<Firm>();
            foreach (var firm in firms)
            {
                if (!delList.Any(f=> f.FirmNumber == firm.FirmNumber))
                {

                    var isDuplicate = firms.Any(f => f.FirmName.ToLower().Trim() == firm.FirmName.ToLower().Trim() && f.FirmNumber != firm.FirmNumber);
                    if (isDuplicate)
                    {
                        var dupList = firms.Where(f => f.FirmName.ToLower().Trim() == firm.FirmName.ToLower().Trim() && f.FirmNumber != firm.FirmNumber).ToList();
                        foreach (var dup in dupList)
                        {
                            dup.Bids.ToList().ForEach(f => f.FirmId = firm.FirmNumber);
                        }
                        delList.AddRange(dupList);
                    }
                }
            }
            db.Firms.RemoveRange(delList);
            db.BulkSaveChanges();
        }
        
        public static void SharePointSetup()
        {

            using var db = new VPContext();
            //var tenate = db.SPTenates.FirstOrDefault(f => f.Url == "https://hardrockdirectionaldrilling.sharepoint.com");
            //var site = tenate.GetSite("Bids");
            var tenate = db.SPTenates.FirstOrDefault();

            tenate.UserName = "glen.lewis@hardrockis.com";


            //site.SyncListFromSharePoint();
            //db.BulkSaveChanges();

            //var bid = db.Bids.FirstOrDefault(f => f.BidId == 2899);

            //var att = bid.Attachment;


            //var list = att.SharePointList;
            //var site = list.Site;
            //var tenate = site.Tenate;

            //var context = site.SharePointClient.Context;
            //var web = context.Web;

            //context.Load(web, web => web.RootFolder);
            //context.ExecuteQuery();

            //var spList = context.Web.GetList(list.Name, false);
            //web.Context.Load(spList, f => f);
            //web.Context.Load(spList, f => f.RootFolder);
            //web.Context.ExecuteQuery();
            //att.SyncFromSource();

            db.BulkSaveChanges();
        }

        public static void SyncAPDocumentsToInvoice()
        {
            using var db = new VPContext();
            var documents = db.APDocuments
                .Include("HQAttachment")
                .Include("HQAttachment.Files")
                .Where(f => f.Mth >= new DateTime(2022,9,1))
                .ToList();

            var aptrans = db.APTrans
                .Include("HQAttachment")
                .Include("HQAttachment.Files")
                .Where(f => (f.APCo == 1 || f.APCo == 10) &&
                             f.Mth >= new DateTime(2022, 9, 1))
                .ToList();

            foreach (var tran in aptrans)
            {
                var document = documents.FirstOrDefault(f => f.APCo == tran.APCo && f.Mth == tran.Mth && f.APRef == tran.APRef);
                if (document != null)
                {
                    var attachment = tran.Attachment;
                    foreach (var file in document.Attachment.Files)
                    {
                        var tranFile = attachment.Files.FirstOrDefault(f => f.OrigFileName == file.OrigFileName);
                        if (tranFile == null)
                            tranFile = attachment.GetRootFolder().AddFile(file.OrigFileName, file.GetData());
                    }
                }
            }
        }

        //public static void APDocAtt()
        //{
        //    using var db = new VPContext();
        //    var docs = db.APDocuments.Where(f => f.UniqueAttchID == null).ToList().OrderBy(o => o.DocId).ToList();

        //    foreach     (var doc in docs)
        //    {
        //        var att = doc.Attachment;
        //    }
        //}

        //public static void CCImagesMoveToShare()
        //{
        //    using var db = new VPContext();
        //    var images = db.CreditCardImages.Where(f => f.EmployeeId == 100798 && f.AttachmentId != null).ToList();

        //    foreach (var image in images)
        //    {
        //        var uncPath = image.Attachment.UncPath;
        //        Console.WriteLine(string.Format("Image Id {0} - {1}", image.AttachmentId, image.Attachment.UncPath, image.ImageName));
        //    }
        //}

        //public static void APDocumentsMoveToAttachmet()
        //{
        //    var db = new VPContext();
        //    var docs = db.APDocuments.Select(s => new { APCo = s.APCo, DocId = s.DocId }).ToList().OrderBy(o=> o.DocId).ToList();
        //    List<System.Threading.Tasks.Task> tasks = new List<System.Threading.Tasks.Task>();

        //    foreach (var docId in docs)
        //    {

        //        var task = new System.Threading.Tasks.Task(() =>
        //        {
        //            var threadDb = new VPContext();
        //            var doc = threadDb.APDocuments.FirstOrDefault(f => f.APCo == docId.APCo && f.DocId == docId.DocId);
        //            Console.WriteLine(string.Format("Doc Id {0} ", doc.DocId));                    
        //            doc.Attachment.GetRootFolder().AddFile(doc.DocumentName);
        //            threadDb.BulkSaveChanges();
        //        });
        //        tasks.Add(task);
        //        if (tasks.Count >= 1)
        //        {
        //            tasks.ForEach(f => f.Start());
        //            System.Threading.Tasks.Task.WaitAll(tasks.ToArray());
        //            tasks = new List<System.Threading.Tasks.Task>();
        //        }
        //    }
        //    tasks.ForEach(f => f.Start());
        //    System.Threading.Tasks.Task.WaitAll(tasks.ToArray());

        //}

        //public static void CheclCCGLAcct()
        //{
        //    var db = new VPContext();
        //    var list = db.CreditTransactions.Where(f => f.Mth == new DateTime(2022,8,1)).ToList();

        //    foreach (var trans in list)
        //    {
        //        foreach (var code in trans.Coding)
        //        {
        //            if (code.GLAcct == null)
        //            {
        //                Console.WriteLine(string.Format("Trans Id {0} ", trans.TransId));
        //                code.UpdateGLAccountInfo(code.GLAcct);
        //                db.BulkSaveChanges();
        //            }
        //        }
        //    }
        //}

        //public static void PosReqRebuildWorkFlow()
        //{
        //    var db = new VPContext();
        //    var list = db.HRPositionRequests.ToList();

        //    list.ForEach(e => e.GenerateWorkFlowAssignments());
        //    db.BulkSaveChanges();

        //    db.Dispose();
        //}

        public static void ConvertResourceToApplicant()
        {
            var db = new VPContext();
            var user = db.WebUsers.FirstOrDefault();

            var resources = db.HRResources
                .Where(f => f.HRRef > 200000)
                .OrderBy(o => o.KeyID)
                .ToList();

            
            
            
            List<System.Threading.Tasks.Task> tasks = new List<System.Threading.Tasks.Task>();
            foreach (var resource in resources)
            {
                if (!string.IsNullOrEmpty(resource.SSN) && !string.IsNullOrEmpty(resource.Email))
                {
                    //var task = new System.Threading.Tasks.Task(() =>
                    //{
                        var threadDb = new VPContext();
                        var threadResource = threadDb.HRResources.FirstOrDefault(f => f.HRCo == resource.HRCo && f.HRRef == resource.HRRef);
                        var removeResource = threadResource.HRRef >= 900000;

						var applicant = threadDb.AddApplicant(threadResource, removeResource);
                        Console.WriteLine(string.Format("Resource {0} Converted to applicant {1}", threadResource.HRRef, applicant.ApplicantId));
                        if (applicant.CurrentApplication()?.ApplicationDate <= DateTime.Now.AddMonths(-12))
                        {
                            applicant.CurrentApplication().Status = DB.WAApplicationStatusEnum.Canceled;
						}
                        threadDb.BulkSaveChanges();
                        threadDb.Dispose();
                    //});
                    //tasks.Add(task);
                }
                else
                {
					Console.WriteLine(string.Format("Resource {0} missing SSN", resource.HRRef));
				}
                //if (tasks.Count >= 100)
                //{
                //    tasks.ForEach(f => f.Start());
                //    System.Threading.Tasks.Task.WaitAll(tasks.ToArray());
                //    tasks = new List<System.Threading.Tasks.Task>();
                //}
            }
            //tasks.ForEach(f => f.Start());
            //System.Threading.Tasks.Task.WaitAll(tasks.ToArray());

            db.Dispose();
        }

        //public static void ConvertSingleResourceToApplicant()
        //{
        //    var db = new VPContext();
        //    var resources = db.HRResources
        //        .Where(f => f.ApplicationDate >= new DateTime(2021, 1, 1)
        //                    && f.HRRef == 908142)
        //        .OrderBy(o => o.KeyID)
        //        .ToList();

        //    //List<System.Threading.Tasks.Task> tasks = new List<System.Threading.Tasks.Task>();
        //    foreach (var resource in resources)
        //    {
        //        if (!string.IsNullOrEmpty(resource.Email))
        //        {
        //            //var task = new System.Threading.Tasks.Task(() =>
        //            //{
        //                var threadDb = new VPContext();
        //                var threadResource = threadDb.HRResources.FirstOrDefault(f => f.HRCo == resource.HRCo && f.HRRef == resource.HRRef);
        //                var applicant = threadDb.AddApplicant(threadResource);
        //                Console.WriteLine(string.Format("Resource {0} Converted to applicant {1}", resource.HRRef, applicant.ApplicantId));
        //                threadDb.BulkSaveChanges();
        //                threadDb.Dispose();
        //            //});
        //            //tasks.Add(task);
        //        }
        //        //if (tasks.Count >= 100)
        //        //{
        //        //    tasks.ForEach(f => f.Start());
        //        //    System.Threading.Tasks.Task.WaitAll(tasks.ToArray());
        //        //    tasks = new List<System.Threading.Tasks.Task>();
        //        //}
        //    }
        //    //tasks.ForEach(f => f.Start());
        //    //System.Threading.Tasks.Task.WaitAll(tasks.ToArray());

        //    db.Dispose();
        //}

        //public static void CheckDocAttachmentinVP()
        //{
        //    var db = new VPContext();

        //    var docSeqs = db.APDocuments.Where(f => f.APCo == 1).ToList();

        //    var aptrans = db.APTrans
        //        .Include("HQAttachment")
        //        .Include("HQAttachment.Files")
        //        .Where(f => f.APCo == 1).ToList();
        //    foreach (var aptran in aptrans)
        //    {
        //        var docSeq = docSeqs.FirstOrDefault(f => f.VendorId == aptran.VendorId && f.APRef == aptran.APRef && f.Mth == aptran.Mth);

        //        if (docSeq != null)
        //        {
        //            var files = aptran.Attachment.Files;

        //            Console.WriteLine(string.Format("AP Trans Attachments count: {0}, TrandId: {1}, Vendor: {2}, Ref: {3}", files?.Count(), aptran.APTransId, aptran.VendorId, aptran.APRef));
        //            if (!aptran.Attachment.Files.Any())
        //            {
        //                Console.WriteLine(string.Format("AP Trans Missing Attachments: {0}, Vendor: {1}, REf: {2}", aptran.APTransId, aptran.VendorId, aptran.APRef));
        //                aptran.Attachment.GetRootFolder().AddFile(docSeq.DocumentName, docSeq.DocData);
        //                db.BulkSaveChanges();
        //            }
        //        }
        //    }
        //}
    }
}
