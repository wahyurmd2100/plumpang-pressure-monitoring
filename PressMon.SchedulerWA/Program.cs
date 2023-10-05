using PressMon.SchedulerWA.WaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TMS.Web.Models.sendWA;

namespace PressMon.SchedulerWA
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var pressMonData = new PressMonData();
            var pressMonResponse = await pressMonData.GetApiDataAsync();
            var whatsAppSender = new WhatsAppSender();
            var pressPT01 = "";
            var pressPT02 = "";

            if (pressMonResponse != null)
            {
                foreach (var data in pressMonResponse.data)
                {
                    Console.WriteLine($"ID: {data.id}, LocationName: {data.locationName}, Pressure: {data.pressure}, TimeStamp: {data.timeStamp}");
                    if (data.locationName == "M-01")
                    {
                        pressPT01 = data.pressure.ToString();
                    }else if (data.locationName == "M-02")
                    {
                        pressPT02 = data.pressure.ToString();
                    }
                }

                foreach (var contact in pressMonResponse.contacts)
                {
                    //Console.WriteLine($"ContactID: {contact.contactID}, ContactName: {contact.contactName}, ContactNumber: {contact.contactNumber}");
                    var response = await whatsAppSender.sendWA(contact.contactNumber, pressPT01, pressPT02);
                    if (response != null)
                    {
                        Console.WriteLine($"WhatsApp message to {contact.contactName} sent successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"WhatsApp message to {contact.contactName} sending failed.");
                    }
                }
            }
            else
            {
                Console.WriteLine("PressMonData is null. Unable to post.");
            }

        }

    }
}
