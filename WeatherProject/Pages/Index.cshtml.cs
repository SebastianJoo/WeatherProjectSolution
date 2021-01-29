using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherProject.Models;
using WeatherProject.Services;

namespace WeatherProject.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public string Message { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }

        public void OnPost()
        {
            var dbExists = SqlWeatherData.CheckDatabaseExists();

            if (dbExists)
            {
                Message = "Databasen existerar redan";
            }
            else
            {
                SqlWeatherData.FillDb();
                
                Message = "Databasen är fylld";
            }
        }
    }
}
