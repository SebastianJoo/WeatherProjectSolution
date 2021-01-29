using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherProject.Models
{
   
    public class Balcony
    {
        public DateTime DateAndTime { get; set; }
        public TimeSpan DoorOpenTime { get; set; }
        public int OpenedAmount { get; set; }
        public Balcony(DateTime dateAndTime, TimeSpan doorOpenTime, int openedAmount )
        {
            DateAndTime = dateAndTime;
            DoorOpenTime = doorOpenTime;
            OpenedAmount = openedAmount;
        }
    }
}
