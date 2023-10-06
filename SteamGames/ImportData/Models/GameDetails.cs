using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportData.Models
{
    public class GameDetails
    {
        public string type { get; set; }
        public string name { get; set; }
        public string steam_appid { get; set; }
        public string required_age { get; set; }
        public string is_free { get; set; }
        public string dlc { get; set; }
        public string short_description { get; set; }
        public string categories { get; set; }
        public string genres { get; set; }

    }

}
