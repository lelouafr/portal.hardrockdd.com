using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Areas.Applicant.Models.Report
{
    public class SummaryByPositionListViewModel
    {
        public SummaryByPositionListViewModel()
        {

        }

        public SummaryByPositionListViewModel(DB.Infrastructure.ViewPointDB.Data.VPContext db)
        {
            if (db == null)
                return;
            var appList = db.WebApplicants.Where(f => f.Applications.Any(f => f.tStatusId == (int)DB.WAApplicationStatusEnum.Submitted)).ToList();
            var posList = appList.SelectMany(s => s.CurrentApplication().AppliedPositions).ToList();

            List = posList.GroupBy(g => new { g.tPositionCodeId, g.HRPositionCode.Description })
                              .Select(s => new SummaryByPositionViewModel()
                              {
                                  PositionCodeId = s.Key.tPositionCodeId,
                                  PosistionCode = s.Key.Description,
                                  ApplicantCnt = s.Count(),
                              }).ToList();

        }

        public SummaryByPositionListViewModel(List<DB.Infrastructure.ViewPointDB.Data.WebApplicant> appList)
        {
            if (appList == null)
                return;
            var posList = appList.SelectMany(s => s.CurrentApplication().AppliedPositions).ToList();

            List = posList.GroupBy(g => new { g.tPositionCodeId, g.HRPositionCode.Description })
                              .Select(s => new SummaryByPositionViewModel()
                              {
                                  PositionCodeId = s.Key.tPositionCodeId,
                                  PosistionCode = s.Key.Description,
                                  ApplicantCnt = s.Count(),
                              }).ToList();

        }

        public DB.WAApplicationStatusEnum Status { get; set; }

        public List<SummaryByPositionViewModel> List { get; }
    }
    public class SummaryByPositionViewModel
    {
        public SummaryByPositionViewModel()
        {

        }

        public string PositionCodeId { get; set; }

        public string PosistionCode { get; set; }

        public int ApplicantCnt { get; set; }
    }
}