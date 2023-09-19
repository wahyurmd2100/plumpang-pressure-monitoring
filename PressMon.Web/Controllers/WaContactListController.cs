using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using PressMon.Web.Data;
using PressMon.Web;
using System.Threading.Tasks;
using TMS.Web.Models;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.InkML;
using System.Net.Http;
using DocumentFormat.OpenXml.Drawing;
using TMS.Web.Models.sendWA;
using System.Text.Json;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Path = System.IO.Path;

namespace TMS.Web.Controllers
{
    public class WaContactListController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public WaContactListController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult LoadData()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();// Skip number of Rows count  
                var length = Request.Form["length"].FirstOrDefault(); // Paging Length 10,20  
                var searchValue = Request.Form["search[value]"].FirstOrDefault(); // Search Value from (Search box)  
                int pageSize = length != null ? Convert.ToInt32(length) : 0; //Paging Size (10, 20, 50,100)  
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                //get all data
                var contact = (from p in _context.WaContactLists
                               select new
                               {
                                   p.ContactID,
                                   p.ContactName,
                                   p.ContactNumber
                               });

                if (!string.IsNullOrEmpty(searchValue))
                {
                    contact = contact.Where(d => d.ContactName.Contains(searchValue) || d.ContactNumber.Contains(searchValue));
                }

                contact = contact.OrderBy(t => t.ContactName);

                //total number of rows counts   
                recordsTotal = contact.Count();
                //Paging   
                var data = contact.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                return View(new WaContactList());
            }
            else
            {
                var tank = await _context.WaContactLists.FindAsync(id);
                if (tank == null)
                {
                    return NotFound();
                }
                return View(tank);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, [Bind("ContactID, ContactName, ContactNumber")] WaContactList waContactList
            )
        {

            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    _context.Add(waContactList);
                    await _context.SaveChangesAsync();

                }
                else
                {
                    try
                    {
                        _context.Update(waContactList);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!contactExist(waContactList.ContactID))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                return Json(new { isValid = true, html = Helper.RenderRazorViewString(this, "_ViewAll", _context.WaContactLists.ToList()) });
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewString(this, "AddOrEdit", waContactList) });

        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contact = await _context.WaContactLists.FindAsync(id);
            _context.WaContactLists.Remove(contact);
            await _context.SaveChangesAsync();
            return Json(new { html = Helper.RenderRazorViewString(this, "_ViewAll", _context.WaContactLists.ToList()) });
        }

        private bool contactExist(int id)
        {
            return _context.WaContactLists.Any(e => e.ContactID == id);
        }

        [HttpPost]
        public async Task<IActionResult> SendWA(List<int> contactID)
        {
            var apiUrl = "https://multichannel.qiscus.com/whatsapp/v1/uwqks-1zpihp7ouot8pyv/4394/messages";
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Qiscus-App-Id", "uwqks-1zpihp7ouot8pyv");
            httpClient.DefaultRequestHeaders.Add("Qiscus-Secret-Key", "407e74a091d93e91db35b5c8c1b0b708");
            var responseMsg = "";

            var pressPT01 = _context.LiveDatas.FirstOrDefault(n => n.LocationName == "M-01");
            var pressPT02 = _context.LiveDatas.FirstOrDefault(n => n.LocationName == "M-02");
            var timestamp = DateTime.Now.ToString("dd/MMM/yy HH:mm");

            var numWAList = _context.WaContactLists
                            .Where(c => contactID.Contains(c.ContactID))
                            .Select(c => c.ContactNumber )
                            .ToList();

            try
            {
                foreach (var numWA in numWAList)
                {
                    // Prepare your data to be sent in the POST request
                    var parameters = new List<Parameter>
                    {
                        new Parameter { type = "text", text = pressPT01.Pressure.ToString() }, // Replace "4.4" with your actual value
                        new Parameter { type = "text", text = pressPT02.Pressure.ToString() }, // Replace "4.8" with your actual value
                        new Parameter { type = "text", text = timestamp } // Replace with your actual date value
                    };
                    var components = new List<Component>
                    {
                        new Component { type = "body", parameters = parameters }
                    };
                    var language = new Language { policy = "deterministic", code = "id" };
                    var template = new Template { @namespace = "6e85836a_6b63_48a8_9d2f_e8c8c365e157", name = "pressure_manual_send", language = language, components = components };
                    var requestData = new RequestMessage() { to = numWA, type = "template", template = template };

                    // Serialize the data to JSON
                    var jsonPayload = JsonSerializer.Serialize(requestData);

                    // Prepare the HTTP content with the JSON payload
                    var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync(apiUrl, httpContent);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        responseMsg = $"Response : {responseContent}";
                    }
                    else
                    {
                        responseMsg = $"Error : {response.StatusCode} - {response.ReasonPhrase}";
                    }
                }
                
                return Json(new { success = true, message = responseMsg });
            }
            catch (Exception ex)
            {
                // Handle exceptions and errors here
                return Json(new { success = false, message = ex.Message });
            }
        }


    }
}
