// ---------------------------------------------------------------
// Copyright (c) Mabrouk Mahdhi. 
//  W/ love for WeAreDevelopers World Congress 2024.
// ---------------------------------------------------------------

using System.Threading.Tasks;

namespace WeAreDevelopers.Core.Brokers.OpenAis
{
    public interface IOpenAiBroker
    {
        ValueTask<string> PostOpenAiPromptAsync(string prompt);
    }
}
