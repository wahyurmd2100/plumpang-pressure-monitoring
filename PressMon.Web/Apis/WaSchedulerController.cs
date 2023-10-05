using Microsoft.AspNetCore.Mvc;
using PressMon.Web.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMS.Web.Models;

namespace TMS.Web.Apis
{
    [Route("api/schedulerData")]
    [ApiController]
    public class WaSchedulerController : ControllerBase
    {
        private readonly DataContext _context;
        public WaSchedulerController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<WaContactList>> GetWaContactList()
        {
            var contactList = _context.WaContactLists.ToList();
            var pressData = _context.LiveDatas.ToList();
            return Ok(new {Contacts = contactList, Data = pressData});
        }
    }
}
