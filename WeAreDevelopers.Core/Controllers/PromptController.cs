// ---------------------------------------------------------------
// Copyright (c) Mabrouk Mahdhi. 
//  W/ love for WeAreDevelopers World Congress 2024.
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;
using WeAreDevelopers.Core.Models.Attendees;
using WeAreDevelopers.Core.Models.Prompts;
using WeAreDevelopers.Core.Services.Foundations.OpenAis;

namespace WeAreDevelopers.Core.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PromptController(IOpenAiService openAiService) : RESTFulController
    {
        private readonly IOpenAiService openAiService = openAiService;

        [HttpPost]
        public async ValueTask<ActionResult<Attendee>> PostPromptAsync(Prompt prompt)
        {
            try
            {
                var result =
                    await this.openAiService.PostOpenAiPromptAsync(prompt.Text);

                return Ok(result);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }
    }
}
