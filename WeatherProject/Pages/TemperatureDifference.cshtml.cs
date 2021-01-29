using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WeatherProject.Services;

namespace WeatherProject.Pages
{
    public class TemperatureDifferenceModel : PageModel
    {
        [BindProperty]
        public string TemperatureDifference { get; set; }
        public void OnGet()
        {
        }

        public void OnPost()
        {
        }
    }
}
