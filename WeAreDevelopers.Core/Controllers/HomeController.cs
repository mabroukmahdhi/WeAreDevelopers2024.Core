// ---------------------------------------------------------------
// Copyright (c) Mabrouk Mahdhi. 
//  W/ love for WeAreDevelopers World Congress 2024.
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;

namespace WeAreDevelopers.Core.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public string Get() =>
            "Welcome to WeAreDevelopers World Congress 2024";
    }
}
