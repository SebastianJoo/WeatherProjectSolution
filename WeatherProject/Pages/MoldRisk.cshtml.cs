using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WeatherProject.Pages
{
    public class MoldRiskModel : PageModel
    {
        [BindProperty]
        public int Number { get; set; }
        public string Location { get; set; }
        public void OnGet()
        {
        }
        public void OnPost()
        {
            if (Number == 1)
            {
                Location = "inne";
            }
            if (Number == 2)
            {
                Location = "ute";
            }
        }
    }
}
