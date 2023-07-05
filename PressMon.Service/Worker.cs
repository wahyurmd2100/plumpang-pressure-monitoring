using System.Text.Json;
using System.Net.Http;
using ModbusManagerLib;
using ModbusManagerLib;
using System.Text;
using System;
using FluentModbus;

namespace PressMon.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly SensorConfig _sensorConfig;
        private ModbusManagerLib.ModbusManager _modbusManager;
        private readonly string _serverAddress;
        private ModbusTcpServer _tcpServer;
        public Worker(ILogger<Worker> logger, SensorConfig sensorConfig, string serverAddress)
        {
            _logger = logger;
            _sensorConfig = sensorConfig;
            _modbusManager = new ModbusManagerLib.ModbusManager();
            _modbusManager.IPAddress = _sensorConfig.IpAddress;
            _modbusManager.Port = _sensorConfig.Port;
            _serverAddress = serverAddress;
            _tcpServer = new ModbusTcpServer();
            _tcpServer.Start();
        }
        /// <summary>
        /// Excute 
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _modbusManager.Connect();
                    foreach (var sensor in _sensorConfig.Sensors)
                    {
                        
                        int address = sensor.Address - 30001; //kenapa dikurang 40001 karena kalau di library madbus pakainya array bukan alamat register. 
                        int[] values = _modbusManager.ReadInputRegisters(address, 1);
                        sensor.Value = (float)values[0] / 1000;
                        var json = JsonSerializer.Serialize(sensor);
                        _logger.LogInformation("Worker running at: {time} , Data ={0}", DateTimeOffset.Now, json);
                    }
                    _modbusManager.Disconnect();
                    PostData(_serverAddress, _sensorConfig.Sensors);
                    SetModbusMaster(_sensorConfig.Sensors);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("Worker running at: {time} , Modbus Expection Message ={0}", DateTimeOffset.Now, ex.Message);
                }
                await Task.Delay(_sensorConfig.TimeLoop, stoppingToken);
            }
        }
        /// <summary>
        /// Post Data to Server
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sensors"></param>
        private async void PostData(string url, List<Sensor> sensors)
        {
            HttpClient client = new HttpClient();
            
            foreach (Sensor sensor in sensors)
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
        private void SetModbusMaster(List<Sensor> sensors)
        {
            Span<short> registers = _tcpServer.GetHoldingRegisters();
            int Address = 10;
            lock (_tcpServer.Lock)
            {
                foreach (Sensor sensor in sensors)
                {
                    float value =(float) Math.Round(sensor.Value, 2);
                    registers.SetLittleEndian<float>(address: Address, setValue(value));
                    _tcpServer.Update();
                    Address += 2;

                }
            }
        }
        private float setValue(double data)
        {
            float value = (float)data;
            byte[] Arry = BitConverter.GetBytes(value);
            byte[] xArry = new byte[4];
            xArry[0] = Arry[1];
            xArry[1] = Arry[0];
            xArry[2] = Arry[3];
            xArry[3] = Arry[2];
            return BitConverter.ToSingle(xArry, 0);
        }

    }
}