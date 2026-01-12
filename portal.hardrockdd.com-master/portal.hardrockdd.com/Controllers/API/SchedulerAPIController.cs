//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.Views.DHTMLX;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web.Http;

//namespace portal.Controllers.API
//{
//    public class JobSchedulerAPIController : ApiController
//    {
//        public IEnumerable<WebAPIEvent> Get()
//        {
//            using var db = new VPContext();

//            return db.Jobs
//                .ToList()
//                .Select(e => (WebAPIEvent)e);
//        }

//        // GET: api/scheduler/5
//        public WebAPIEvent Get(int id)
//        {
//            return (WebAPIEvent)db.SchedulerEvents.Find(id);
//        }

//        // PUT: api/scheduler/5
//        [HttpPut]
//        public IHttpActionResult EditSchedulerEvent(int id, WebAPIEvent webAPIEvent)
//        {
//            var updatedSchedulerEvent = (SchedulerEvent)webAPIEvent;
//            updatedSchedulerEvent.Id = id;
//            db.Entry(updatedSchedulerEvent).State = EntityState.Modified;
//            db.SaveChanges();

//            return Ok(new
//            {
//                action = "updated"
//            });
//        }

//        // POST: api/scheduler/5
//        [HttpPost]
//        public IHttpActionResult CreateSchedulerEvent(WebAPIEvent webAPIEvent)
//        {
//            var newSchedulerEvent = (SchedulerEvent)WebAPIEvent;
//            db.SchedulerEvents.Add(newSchedulerEvent);
//            db.SaveChanges();

//            return Ok(new
//            {
//                tid = newSchedulerEvent.Id,
//                action = "inserted"
//            });
//        }

//        // DELETE: api/scheduler/5
//        [HttpDelete]
//        public IHttpActionResult DeleteSchedulerEvent(int id)
//        {
//            var schedulerEvent = db.SchedulerEvents.Find(id);
//            if (schedulerEvent != null)
//            {
//                db.SchedulerEvents.Remove(schedulerEvent);
//                db.SaveChanges();
//            }

//            return Ok(new
//            {
//                action = "deleted"
//            });
//        }
//    }
//}