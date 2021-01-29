using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherProject.Models
{
    public class TemperatureData
    {
        public int Id { get; set; }
        public DateTime DateAndTime { get; set; }
        public string Location { get; set; }
        public float Temp { get; set; }
        public int Humidity { get; set; }
    }
}
