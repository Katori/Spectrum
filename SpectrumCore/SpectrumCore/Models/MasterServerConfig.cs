using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpectrumCore.Models
{
    public class MasterServerConfig
    {
        public string RDS_DB_NAME { get; set; }
        public string RDS_USERNAME { get; set; }
        public string RDS_PASSWORD { get; set; }
        public string RDS_HOSTNAME { get; set; }
        public string RDS_PORT { get; set; }
    }
}
