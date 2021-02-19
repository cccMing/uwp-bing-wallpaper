using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqliteManager.Models
{
    //http://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1

    public partial class BingArchive
    {
        [JsonProperty("images")]
        public Image[] Images { get; set; }

        [JsonProperty("tooltips")]
        public Tooltips Tooltips { get; set; }
    }

    public partial class Image
    {
        [JsonProperty("startdate")]
        public string Startdate { get; set; }

        [JsonProperty("fullstartdate")]
        public string Fullstartdate { get; set; }

        [JsonProperty("enddate")]
        public string Enddate { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("urlbase")]
        public string Urlbase { get; set; }

        [JsonProperty("copyright")]
        public string Copyright { get; set; }

        [JsonProperty("copyrightlink")]
        public string Copyrightlink { get; set; }

        [JsonProperty("quiz")]
        public string Quiz { get; set; }

        [JsonProperty("wp")]
        public bool Wp { get; set; }

        [JsonProperty("hsh")]
        public string Hsh { get; set; }

        [JsonProperty("drk")]
        public long Drk { get; set; }

        [JsonProperty("top")]
        public long Top { get; set; }

        [JsonProperty("bot")]
        public long Bot { get; set; }

        [JsonProperty("hs")]
        public object[] Hs { get; set; }
    }

    public partial class Tooltips
    {
        [JsonProperty("loading")]
        public string Loading { get; set; }

        [JsonProperty("previous")]
        public string Previous { get; set; }

        [JsonProperty("next")]
        public string Next { get; set; }

        [JsonProperty("walle")]
        public string Walle { get; set; }

        [JsonProperty("walls")]
        public string Walls { get; set; }
    }
}
