namespace DB.Infrastructure.ViewPointDB.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;

    public partial class CompanyDivision
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
                if (_db == null)
                {
                    _db = VPContext.GetDbContextFromEntity(this);

                    if (_db == null)
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }

		public DailyTicket AddDailyTicket(DateTime workDate, DTFormEnum formType)
		{
			var curUser = db.GetCurrentUser();
			var dailyTicket = new DailyTicket
			{
				DTCo = (byte)DTCo,
                WPDivisionId = this.DivisionId,
				TicketId = DailyTicket.GetNextTicketId(),
				WorkDate = workDate,
				CreatedBy = db.CurrentUserId,
				CreatedOn = DateTime.Now,
				ParentTicketId = null,

				CreatedUser = curUser,
				HQCompanyParm = this.HQCompany,
				db = this.db,
				Status = DailyTicketStatusEnum.Draft,
				FormType = formType
			};
			db.DailyTickets.Add(dailyTicket);

			return dailyTicket;
		}
		public static string BaseTableName { get { return "budWPDM"; } }


    }
}