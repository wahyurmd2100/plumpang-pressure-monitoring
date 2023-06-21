using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusManagerLib
{
    public sealed class StoreLogData
    {
        private string filename;
        private static volatile StoreLogData instance;
        private static object syncObject = new object();

        private StoreLogData()
        {
        }

        public static StoreLogData Instance
        {
            get
            {
                if (StoreLogData.instance == null)
                {
                    object syncObject = StoreLogData.syncObject;
                    bool lockTaken = false;
                    try
                    {
                        Monitor.Enter(syncObject, ref lockTaken);
                        if (StoreLogData.instance == null)
                            StoreLogData.instance = new StoreLogData();
                    }
                    finally
                    {
                        if (lockTaken)
                            Monitor.Exit(syncObject);
                    }
                }
                return StoreLogData.instance;
            }
        }

        public void Store(string message)
        {
            if (this.filename == null)
                return;
            using (StreamWriter streamWriter = new StreamWriter(this.Filename, true))
                streamWriter.WriteLine(message);
        }

        public void Store(string message, DateTime timestamp)
        {
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(this.Filename, true))
                    streamWriter.WriteLine(timestamp.ToString("dd.MM.yyyy H:mm:ss.ff ") + message);
            }
            catch
            {
            }
        }

        public string Filename
        {
            get => this.filename;
            set => this.filename = value;
        }
    }
}
