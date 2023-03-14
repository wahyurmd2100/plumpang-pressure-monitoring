using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CSL.Web.Data;
using Microsoft.AspNetCore.Authorization;
using CSL.Web;
using TMS.Web;
using CSL.Web.Models;
using TMS.Web.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace TMS.Web.Controllers
{
    public class TankTicketController : Controller
    {
        private readonly TMSContext _context;
        public IConfiguration _configuration { get; }
        public TankTicketController(TMSContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        // GET: Tanks
        [Authorize]
        public IActionResult Index()
        {
            populateTanks();
            populateTankTicketType();
            return View();
        }
        //Get: data to datatable
        public IActionResult LoadData()
        {
            try
            {

                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();//Skip number of rows count
                var length = Request.Form["length"].FirstOrDefault();//paging length 10, 20
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();// sort column name
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();//sort column direction (asc, desc)
                var searchValue = Request.Form["search[value]"].FirstOrDefault();//search value from (search box)
                int pageSize = length != null ? Convert.ToInt32(length) : 0;//paging size (10, 20, 50, 100)
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                //custom filter
                var TankId = Convert.ToInt32(Request.Form["TankNameFilter"].FirstOrDefault());//Tank Id
                var OperationType = Request.Form["OperationType"].FirstOrDefault().ToString();//Operation Type
                var OperationStatus = Convert.ToInt32(Request.Form["OperationStatus"].FirstOrDefault());//Operation Status
                var TicketNumber = Request.Form["TicketNumber"].FirstOrDefault();//Ticket Number
                var Datefrom = DateTime.Parse(Request.Form["minDateTime"].FirstOrDefault());//Operation Type
                var DateTo = DateTime.Parse(Request.Form["maxDateTime"].FirstOrDefault()).AddDays(1).AddSeconds(-1);//Operation Type

                //
                //get all data
                var tankticket = (from t in _context.Tank_Ticket
                                  join p in _context.Tank on t.Tank_Id equals p.TankId
                                  select new
                                  {
                                      t.Id,
                                      t.Tank_Id,
                                      p.Name,
                                      t.Ticket_Number,
                                      t.Timestamp,
                                      t.Operation_Type,
                                      t.Operation_Status,
                                      t.Measurement_Method,
                                      t.Liquid_Level,
                                      t.Water_Level,
                                      t.Liquid_Temperature,
                                      t.Liquid_Density,
                                      t.Is_Upload_Success,
                                  }).Where(t => t.Timestamp >=Datefrom && t.Timestamp <=DateTo);
                //filter tank name
                if (TankId != 0)
                {
                    tankticket = tankticket.Where(t => t.Tank_Id == TankId);
                }
                //filter by operation type
                if(OperationType != "--Select Operation Type--")
                {
                    tankticket = tankticket.Where(t => t.Operation_Type == OperationType);
                }
                //filter by operation status
                if(OperationStatus != 0)
                {
                    tankticket = tankticket.Where(t => t.Operation_Status == OperationStatus);
                }
                //filter by tank ticket numeber
                if (TicketNumber != "")
                {
                    tankticket = tankticket.Where(t => t.Ticket_Number == TicketNumber);
                }
                //sorting
                if (!string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection))
                {
                    tankticket = tankticket.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                //search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    tankticket = tankticket.Where(m => m.Name.Contains(searchValue));
                }

                //total number of rows counts
                recordsTotal = tankticket.Count();
                //paging
                var data = tankticket.Skip(skip).Take(pageSize).ToList();
                //returning json data
                return Json(new { draw = draw, recordsFilterd = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }
        //get: tankticket/addoredit
        [NoDirectAccess]

        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                populateTanks();
                populateTankTicketType();
                return View(new TankTicket());
            }

            else
            {
                populateTanks();
                populateTankTicketType();
                var tankticket = await _context.Tank_Ticket.FindAsync(id);
                if (tankticket == null)
                {
                    return NotFound();
                }
                return View(tankticket);
            }
        }
        //To protect from overposting attacks, enable the specific properties you want to bind to
        //for more details, see http://go.microsoft.com/fwlink/?LinkId=317598
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, [Bind("Id,Tank_Id,Timestamp,TicketNumber,Operation_Type,Operation_Status,Measurement_Method,Ticket_Number,Do_Number,Shipment_Id,Liquid_Level,Water_Level,Liquid_Temperature,Liquid_Density")] TankTicket tankticket)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    //tankticket.Id = new TankTicket()//
                    string datetime = tankticket.Timestamp.ToString();
                    tankticket.Timestamp = DateTime.Parse(datetime);
                    tankticket.Created_By = User.Identity.Name;
                    tankticket.Updated_Timestamp = DateTime.Now;
                    _context.Add(tankticket);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    try
                    {
                        string datetime = tankticket.Timestamp.ToString();
                        tankticket.Timestamp = DateTime.Parse(datetime);
                        tankticket.Updated_By = User.Identity.Name;
                        tankticket.Updated_Timestamp = DateTime.Now;
                        _context.Update(tankticket);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!TankTicketExist(tankticket.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                return Json(new { isValid = true, html = Helper.RenderRazorViewString(this, "_viewAll", _context.Tank_Ticket.ToList()) });
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewString(this, "AddOrEdit", tankticket) });
        }

        //Post: tankticket/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tankticket = await _context.Tank_Ticket.FindAsync(id);
            _context.Tank_Ticket.Remove(tankticket);
            await _context.SaveChangesAsync();
            return Json(new { html = Helper.RenderRazorViewString(this, "_ViewAll", _context.Tank_Ticket.ToList()) });
        }
        private bool TankTicketExist(int id)
        {
            return _context.Tank_Ticket.Any(t => t.Id == id);
        }
        private void populateTanks(object SelectList = null)
        {
            List<Tank> tanks = new List<Tank>();
            tanks = (from t in _context.Tank select t).ToList();
            var tankip = new Tank()
            {
                TankId = 0,
                Name = "---- Select Tank ----"
            };
            tanks.Insert(0, tankip);
            ViewBag.TankId = tanks;
        }
        private void populateTankTicketType(object SelectList = null)
        {
            var Operation = new List<string> { "--Select Operation Type--", "BLD", "DL", "ICR", "ILS", "IOT", "IS", "IST", "OWN", "PI", "RCR", "ROT", "RPR", "TPL", "TRF" };
            ViewBag.Operation = Operation.Select(o => new SelectListItem { Text = o, Value = o });
        }
        public IActionResult GetData(int id, DateTime timerequest)
        {
            var requestTimeString = timerequest.ToString("MM/dd/yyyy HH:mm"); //convert time request to string

            var tankhistoricals = (from t in _context.Tank_Historical
                                   select new
                                   {
                                       t.Id,
                                       t.TankId,
                                       newTimeStamp = t.TimeStamp.ToString("MM/dd/yyyy HH:mm"),
                                       t.LiquidLevel,
                                       t.WaterLevel,
                                       t.LiquidTemperature,
                                       t.LiquidDensity,
                                       t.VolumeObserved,
                                       t.VolumeNetStandard
                                   }).Where(t => t.TankId == id).ToList();
            
            var tankhistorical = tankhistoricals.FirstOrDefault(t => t.newTimeStamp == requestTimeString);
             if (tankhistorical == null)
            {
                return Json(new TankLiveData());
            }
            return Json(tankhistorical);
        }
        //POST : Tankticket by ID
        //public IActionResult PostData(int id)
        //{
        //    //TankDipPosting.ResponseCode _responseCode = new TankDipPosting.ResponseCode();
        //    //var ticket = _context.Tank_Ticket.FirstOrDefault(t => t.Id == id);
        //    //if (ticket != null)
        //    //{
        //    //    _responseCode = UploadingTicket(ticket);
        //    //    if(_responseCode.Type == "S")
        //    //    {
        //    //        ticket.Is_Upload_Success = 1;
        //    //        ticket.SAP_Response = _responseCode.DescMsg;
        //    //    }
        //    //    else if(_responseCode.Type == "E")
        //    //    {
        //    //        ticket.Is_Upload_Success = 0;
        //    //        ticket.SAP_Response = _responseCode.DescMsg;
        //    //    }
        //    //    _context.Update(ticket);
        //    //    _context.SaveChangesAsync();
        //    //}
        //    //return Json(_responseCode);
        //}
        //process uploading tankticket
        //private TankDipPosting.ResponseCode UploadingTicket(TankTicket ticket)
        //{
        //    TankDipPosting.ResponseCode responseCode = new TankDipPosting.ResponseCode();
        //    //read configuration
        //    TankConfiguration tankConfiguration = new TankConfiguration();
        //    tankConfiguration = _configuration.GetSection("TankConfiguration").Get<TankConfiguration>();
        //    string url = tankConfiguration.UrlPosting;
        //    Tank tank = _context.Tank.FirstOrDefault(t => t.TankId == ticket.Tank_Id);
        //    #region Initial value
        //    TankDipPosting.DippingDataDelivery data = new TankDipPosting.DippingDataDelivery();
        //    data.plant = tankConfiguration.PlantCode;
        //    data.dipDate = ((DateTime)ticket.Timestamp).ToString("ddMMyyyy");
        //    data.dipTime = ((DateTime)ticket.Timestamp).ToString("HHmm");
        //    data.dipTank = ConvertTankNumberToSAPTankNumber(tank.Name);
        //    data.dipEvent = ConvertStatusToDipEvent(ticket.Operation_Status);
        //    data.dipOperation = ticket.Operation_Type;
        //    data.totalHeight = ticket.Liquid_Level.ToString();
        //    data.totalHeightUOM = "MM";
        //    data.waterHeight = ticket.Water_Level.ToString();
        //    data.waterHeightUOM = "MM";
        //    data.celciusMaterialTemp = ticket.Liquid_Temperature.ToString("0.00");
        //    data.celciusMaterialTemp = data.celciusMaterialTemp.Replace('.', ',');
        //    data.celciusTestTemp = ticket.Liquid_Temperature.ToString("0.00");
        //    data.celciusTestTemp = data.celciusTestTemp.Replace('.', ',');
        //    data.kglDensity = ticket.Liquid_Density.ToString("0.0000");
        //    data.kglDensity = data.kglDensity.Replace('.', ',');
        //    #endregion
        //    //create service
        //    string password = string.Format("{0}{1}{2}{3}", data.plant, data.dipDate, data.dipTime, data.dipOperation);
        //    //request
        //    TankDipPosting.DoPostingRequestBody Body = new TankDipPosting.DoPostingRequestBody();
        //    Body.dipData = data;
        //    Body.User = data.plant;
        //    Body.Password = password;
        //    TankDipPosting.DoPostingRequest request = new TankDipPosting.DoPostingRequest();
        //    request.Body = Body;

        //    //Do Posting
        //    TankDipPosting.ZMM_TANKDIP_DELIVERYSoapClient.EndpointConfiguration endpoint = new TankDipPosting.ZMM_TANKDIP_DELIVERYSoapClient.EndpointConfiguration();
        //    TankDipPosting.ZMM_TANKDIP_DELIVERYSoapClient service = new TankDipPosting.ZMM_TANKDIP_DELIVERYSoapClient(endpoint, url);
        //    TankDipPosting.DoPostingResponse response = new TankDipPosting.DoPostingResponse();
        //    response = service.DoPosting(request);
        //    //respon byd
        //    TankDipPosting.DoPostingResponseBody responseBody = response.Body;
        //    responseCode = responseBody.DoPostingResult;
        //    return responseCode;

        //}
        private string ConvertTankNumberToSAPTankNumber(string tanknumber)
        {
            string[] splits = tanknumber.Split('-');
            int tanknum = Convert.ToInt32(splits[1]);

            return "T" + tanknum.ToString("000");
        }
        private string ConvertStatusToDipEvent(int? status)
        {
            if (!status.HasValue)
                return "";

            if (status.Value == 1)
                return "O";

            if (status.Value == 2)
                return "C";

            return "";
        }
    }
}