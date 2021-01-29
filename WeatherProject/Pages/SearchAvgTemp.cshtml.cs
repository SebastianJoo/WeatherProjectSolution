using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WeatherProject.Models;
using WeatherProject.Services;

namespace WeatherProject.Pages
{
    public class SearchAvgTempModel : PageModel
    {
        [BindProperty]
        public string Date { get; set; }
        public float AverageTempOutside { get; set; }
        public float AverageTempInside { get; set; }

        public void OnGet()
        {
        }
        public void OnPost()
        {
            DateTime date = new DateTime();
            date = DateTime.Parse(Date);

            var averageOutside = SqlWeatherData.GetAverageTemp(date, "ute");

            var averageInside = SqlWeatherData.GetAverageTemp(date, "inne");

            AverageTempOutside = (float)Math.Round(averageOutside, 1);

            AverageTempInside = (float)Math.Round(averageInside, 1);

        }
    }
}
