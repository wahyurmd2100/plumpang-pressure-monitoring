using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PressMon.Service.Models;

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
        public List<TankLiveData> GetAllTankLiveData()
        {
            using (_Context = new AppDbContext(GetAllOptions()))
            {
                try
                {
                    var tankLiveDatas = _Context.Tank_Live_Data.ToList();
                    if (tankLiveDatas != null)
                    {
                        return tankLiveDatas;
                    }
                    else
                    {
                        return new List<TankLiveData>();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        //get tanklive data
        public TankLiveData GetTankLiveData(int id)
        {
            using (_Context = new AppDbContext(GetAllOptions()))
            {
                try
                {
                   TankLiveData tankLiveData = _Context.Tank_Live_Data.Single(x => x.TankId==id);
                    if (tankLiveData != null)
                    {
                        return tankLiveData;
                    }
                    else
                    {
                        return new TankLiveData();
                    }

                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        //update tanklivedata 
        public void updateTnkLiveData(TankLiveData tankLiveData)
        {
            using (_Context = new AppDbContext(GetAllOptions()))
            {
                try
                {
                    if (tankLiveData != null)
                    {
                        _Context.Update(tankLiveData);
                        _Context.SaveChanges();
                    }
                    
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        //get products
        public List<Product> getProduct()
        {
            using (_Context = new AppDbContext(GetAllOptions()))
            {
                try
                {
                    var products = _Context.Master_Products.ToList();
                    if (products != null)
                    {
                        return products;
                    }
                    else
                    {
                        return new List<Product>();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        //get tank by Name
        public Tank getTank(string Name)
        {
            using (_Context = new AppDbContext(GetAllOptions()))
            {
                try
                {

                    var tanks = _Context.Tank.ToList();
                    var tank = tanks.FirstOrDefault(t => t.Name == Name);
                    if(tank != null)
                    {
                        return tank;
                    }
                    else
                    {
                        return new Tank();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public void UpdateTank(Tank tank)
        {
            using (_Context = new AppDbContext(GetAllOptions()))
            {
                try
                {
                    _Context.Update(tank);
                    _Context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    throw;
                }

            }

        }
    }
}
