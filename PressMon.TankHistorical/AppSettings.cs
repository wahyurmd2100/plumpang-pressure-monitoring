﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PressMon.Settings
{
    public static class AppSetting
    {
        public static IConfiguration Configuration { get; set; }
        public static string ConnectionString { get; set; }
    }
}
