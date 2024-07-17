// ---------------------------------------------------------------
// Copyright (c) Mabrouk Mahdhi. 
//  W/ love for WeAreDevelopers World Congress 2024.
// ---------------------------------------------------------------

using System.Threading.Tasks;

namespace WeAreDevelopers.Core.Services.Foundations.OpenAis
{
    public interface IOpenAiService
    {
        ValueTask<string> PostOpenAiPromptAsync(string prompt);
    }
}
