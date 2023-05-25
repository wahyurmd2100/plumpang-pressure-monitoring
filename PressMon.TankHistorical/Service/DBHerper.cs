using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PressMon.Models;
using PressMon.Settings;

namespace PressMon.Service
{
    public class DBHerper
    {
        private AppDbContext _Context;

        private DbContextOptions<AppDbContext> GetAllOptions()
        {
            var optionBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionBuilder.UseSqlServer(AppSetting.ConnectionString);
            return optionBuilder.Options;
        }
        public ErrorMessage SetHistorycalData()
        {
            using (_Context = new AppDbContext(GetAllOptions()))
            {
                ErrorMessage errorMessage = new ErrorMessage();
                try
                {
                    var tankLiveDatas = _Context.Tank_Live_Data.ToList();
                    if (tankLiveDatas != null)
                    {
                        foreach (TankLiveData tlData in tankLiveDatas)
                        {
                            TankHistorical tankHistorical = new TankHistorical();
                            tankHistorical.TankId = tlData.TankId;
                            tankHistorical.TimeStamp = DateTime.Now;
                            tankHistorical.LiquidLevel = tlData.LiquidLevel;
                            tankHistorical.WaterLevel = tlData.WaterLevel;
                            tankHistorical.LiquidTemperature = tlData.LiquidTemperature;
                            tankHistorical.LiquidDensity = tlData.LiquidDensity;
                            tankHistorical.VolumeObserved = tlData.VolumeObserved;
                            tankHistorical.VolumeNetStandard = tlData.VolumeNetStandard;
                            _Context.Add(tankHistorical);
                            _Context.SaveChanges();
                        }
                        errorMessage.Status = "Success";
                        errorMessage.Message = "--";
                        
                    }
                }
                catch (Exception e)
                {
                    errorMessage.Status = "Error";
                    errorMessage.Message = e.Message;
                }
                return errorMessage;
            }
        }
        //get tanklive data
    }
    public class ErrorMessage
    {
        public string Status { get; set; }
        public string Message { get; set; }
    }
}
