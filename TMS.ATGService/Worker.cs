using EasyModbus;
using System.Text.Json;
using TMS.ATGService.Models;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

namespace TMS.ATGService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly tankConfiguration _tankConfig;
        private readonly DBHerper _dbHelper;
        private ModbusClient _modbusClient;
        //tank table
        private List<TankTable> _tankTables;
        public Worker(ILogger<Worker> logger, tankConfiguration tankConfig)
        {
            _logger = logger;
            _tankConfig = tankConfig;
            _modbusClient = new ModbusClient();
            _dbHelper = new DBHerper();
            _tankTables = new List<TankTable>();
            GetTankTable();
            _modbusClient.IPAddress = _tankConfig.tankIp;
            _modbusClient.Port = _tankConfig.tankPort;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                UpdateData();
                await Task.Delay(1000, stoppingToken);
            }
        }
        //get data tank tank
       
        private void UpdateData()
        {   
            foreach (TankDetail td in _tankConfig.tankDetail)
           {
                try
                {

                    _modbusClient.Connect();
                    int[] data = _modbusClient.ReadHoldingRegisters(td.startAddr, td.stopAddr);
                    #region update tank Live Data
                    TankLiveData tankLiveData = new TankLiveData();
                    tankLiveData = _dbHelper.GetTankLiveData(td.TankId);
                    tankLiveData.Level = ConvertToFloat(data[0], data[1]);
                    tankLiveData.Temperature = ConvertToFloat(data[2], data[3])/10; //get temperature
                    tankLiveData.GrossVolume = ConvertToFloat(data[4], data[5]); 
                    tankLiveData.Density = ConvertToFloat(data[6], data[7])/1000; //get density
                    tankLiveData.NetVolume = GetVolume(td.TankName, tankLiveData.Level);
                    tankLiveData.TimeStamp = DateTime.Now;
                    _dbHelper.updateTnkLiveData(tankLiveData);
                    #endregion

                    var json = JsonSerializer.Serialize(tankLiveData);
                    _logger.LogInformation("Worker running at: {time},ATG Is Connected on TANK ={0}: list data = {1}", DateTimeOffset.Now, td.TankName, json);
                    _modbusClient.Disconnect();
                }
                catch (Exception e)
                {
                    _logger.LogInformation("Worker running at: {time}, {0}", DateTimeOffset.Now, e.Message);
                }               

            }
           
        }
        //Convert to float
        private float ConvertToFloat(int Array1, int Array2)
        {
            int[] newArray = new int[2];
            newArray[0] = Array1;
            newArray[1] = Array2;
            return ModbusClient.ConvertRegistersToFloat(newArray, ModbusClient.RegisterOrder.HighLow);
        }
        //get tank table
        private void GetTankTable()
        {
            foreach (TankDetail td in _tankConfig.tankDetail)
            {
                try
                {
                    //get doc CSV;
                    IWorkbook bookfile;
                    string path = string.Format("TankTable/{0}.xlsx", td.TankName);
                    using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        bookfile = new XSSFWorkbook(file);
                    }
                    ISheet sheet = bookfile.GetSheet(td.TankName);
                    //get data tank table
                    TankTable tankTable = new TankTable();
                    tankTable.TankName = td.TankName;
                    tankTable.TankTableDetails = new List<TankTableDetail>();
                    for (int row = 1; row < sheet.LastRowNum; row++)
                    {
                        TankTableDetail ttDetail = new TankTableDetail();
                        if (sheet.GetRow(row) != null)
                        {
                            string data1 = sheet.GetRow(row).GetCell(0).ToString();
                            string data2 = sheet.GetRow(row).GetCell(1).ToString();
                            
                            ttDetail.Level = Convert.ToDouble(data1);
                            ttDetail.Volume = Convert.ToDouble(data2);
                            tankTable.TankTableDetails.Add(ttDetail);
                            //_logger.LogInformation("read data at: {time}, {0},{1}", DateTimeOffset.Now, ttDetail.Level, ttDetail.Volume);
                        }
                    }
                    _tankTables.Add(tankTable);
                }
                catch (Exception e)
                {
                    _logger.LogInformation("Worker running at: {time}, {0}", DateTimeOffset.Now, e.Message);
                }

            }
        }
            //GET VOLUME
        private double GetVolume(string TankName, double level)
        {
            // UpdateData();
            double resultVolume=0;
            double lowerLevelLimit = 0; //x1
            double lowerVolumeLimit = 0; //y1

            double upperLevelLimit = 0; //x2
            double upperVolumeLimit = 0; //y2
            TankTable tb = _tankTables.Where(t => t.TankName == TankName).First();
            for(int col=0; col < tb.TankTableDetails.Count; col++)
            {
                if(level == tb.TankTableDetails[col].Level)
                {
                    resultVolume = tb.TankTableDetails[col].Volume;
                    return resultVolume;
                }
                if(tb.TankTableDetails[col].Level < level)
                {
                    lowerLevelLimit = tb.TankTableDetails[col].Level;
                    lowerVolumeLimit = tb.TankTableDetails[col].Volume;
                }
                else if(tb.TankTableDetails[col].Level > level)
                {
                    upperLevelLimit = tb.TankTableDetails[col].Level;
                    upperVolumeLimit = tb.TankTableDetails[col].Volume;
                    break;
                }               
            }
            //calculate interpolation
            resultVolume = lowerVolumeLimit + ((level - lowerLevelLimit) / (upperLevelLimit - lowerLevelLimit)) * (upperVolumeLimit - lowerVolumeLimit);

            return resultVolume;
        }
        
    }
}