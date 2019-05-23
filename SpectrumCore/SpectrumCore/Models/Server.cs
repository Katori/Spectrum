using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpectrumCore.Models
{
    [System.Serializable]
    public class Server
    {
        [Key]
        public int Id { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public int Players { get; set; }
        public int PlayedGames { get; set; }
        public System.DateTime LastCheckedIn { get; set; }
        public string ConnectionString
        {
            get
            {
                return Address + ":" + Port;
            }
            set
            {
                if (value.Contains(":"))
                {
                    string[] valSplit = value.Split(':');
                    if (valSplit.Length > 0)
                    {
                        Address = valSplit[0];
                        Port = int.Parse(valSplit[1]);
                    }
                    else
                    {
                        Address = "null";
                        Port = 0;
                    }
                }
                else
                {
                    Address = "null";
                    Port = 0;
                }
            }
        }
    }

    [System.Serializable]
    public class StatusMessage
    {
        public bool Shutdown;
    }
}
