using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherProject.Models
{
    public class MoldData
    {
        public DateTime DateAndTime { get; set; }
        public float Temp { get; set; }
        public int Humidity { get; set; }
        public double MoldRisk { get; set; }
    }
}
