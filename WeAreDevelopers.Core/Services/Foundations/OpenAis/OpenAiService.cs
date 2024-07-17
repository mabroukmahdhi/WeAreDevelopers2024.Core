// ---------------------------------------------------------------
// Copyright (c) Mabrouk Mahdhi. 
//  W/ love for WeAreDevelopers World Congress 2024.
// ---------------------------------------------------------------

using System.Threading.Tasks;
using WeAreDevelopers.Core.Brokers.OpenAis;

namespace WeAreDevelopers.Core.Services.Foundations.OpenAis
{
    public class OpenAiService(IOpenAiBroker openAiBroker) : IOpenAiService
    {
        private readonly IOpenAiBroker openAiBroker = openAiBroker;

        public async ValueTask<string> PostOpenAiPromptAsync(string prompt) =>
            await this.openAiBroker.PostOpenAiPromptAsync(prompt);
    }
}
