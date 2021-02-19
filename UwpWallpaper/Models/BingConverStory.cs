﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UwpWallpaper.Models
{
    /// <summary>
    /// bing壁纸故事详情，现在好像接收不到数据了
    /// </summary>
    public class BingConverStory
    {

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("attribute")]
        public string Attribute { get; set; }

        [JsonProperty("para1")]
        public string Para1 { get; set; }

        [JsonProperty("para2")]
        public string Para2 { get; set; }

        [JsonProperty("provider")]
        public string Provider { get; set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty("primaryImageUrl")]
        public string PrimaryImageUrl { get; set; }

        [JsonProperty("Country")]
        public string Country { get; set; }

        [JsonProperty("City")]
        public string City { get; set; }

        [JsonProperty("Longitude")]
        public string Longitude { get; set; }

        [JsonProperty("Latitude")]
        public string Latitude { get; set; }

        [JsonProperty("Continent")]
        public string Continent { get; set; }

        [JsonProperty("CityInEnglish")]
        public string CityInEnglish { get; set; }

        [JsonProperty("CountryCode")]
        public string CountryCode { get; set; }
    }
}
