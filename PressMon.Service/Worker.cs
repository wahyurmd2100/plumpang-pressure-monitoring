using System.Text.Json;
using System.Net.Http;
using ModbusManagerLib;
using ModbusManagerLib;
using System.Text;
using System;

namespace PressMon.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly SensorConfig _sensorConfig;
        private ModbusManagerLib.ModbusManager _modbusManager;
        private readonly string _serverAddress;
        public Worker(ILogger<Worker> logger, SensorConfig sensorConfig, string serverAddress)
        {
            _logger = logger;
            _sensorConfig = sensorConfig;
            _modbusManager = new ModbusManagerLib.ModbusManager();
            _modbusManager.IPAddress = _sensorConfig.IpAddress;
            _modbusManager.Port = _sensorConfig.Port;
            _serverAddress = serverAddress;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _modbusManager.Connect();
                    foreach (var sensor in _sensorConfig.Sensors)
                    {

                        int address = sensor.Address - 40001;
                        int[] values = _modbusManager.ReadHoldingRegisters(address, 2);
                        sensor.Value = ModbusManagerLib.ModbusManager.ConvertRegistersToFloat(values, ModbusManagerLib.ModbusManager.RegisterOrder.HighLow);
                        var json = JsonSerializer.Serialize(sensor);
                        _logger.LogInformation("Worker running at: {time} , Data ={0}", DateTimeOffset.Now, json);
                    }
                    _modbusManager.Disconnect();
                    PostData(_serverAddress, _sensorConfig.Sensors);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("Worker running at: {time} , Modbus Expection Message ={0}", DateTimeOffset.Now, ex.Message);
                }
                await Task.Delay(_sensorConfig.TimeLoop, stoppingToken);
            }
        }
        private async void PostData(string url, List<Sensor> sensors)
        {
            HttpClient client = new HttpClient();
            foreach(Sensor sensor in sensors)
            {
                try
                {
                    var postSensor = new PostSensor() { LocationName = sensor.Location, Pressure = sensor.Value };
                    // Serialize class into JSON
                    var payload = JsonSerializer.Serialize(postSensor);

                    // Wrap our JSON inside a StringContent object
                    var content = new StringContent(payload, Encoding.UTF8, "application/json");

                    // Post to the endpoint
                    var response = await client.PostAsync(url, content);
                }
                catch(Exception ex)
                {
                    _logger.LogInformation("Worker running at: {time} , Server Exception Message ={0}", DateTimeOffset.Now, ex.Message);
                }
                
            }
        }
        
    }
}