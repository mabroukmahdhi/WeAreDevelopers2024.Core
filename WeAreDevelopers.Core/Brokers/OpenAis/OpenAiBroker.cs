// ---------------------------------------------------------------
// Copyright (c) Mabrouk Mahdhi. 
//  W/ love for WeAreDevelopers World Congress 2024.
// ---------------------------------------------------------------

using System.Threading.Tasks;
using Standard.AI.OpenAI.Clients.OpenAIs;
using Standard.AI.OpenAI.Models.Services.Foundations.ChatCompletions;

namespace WeAreDevelopers.Core.Brokers.OpenAis
{
    public class OpenAiBroker(IOpenAIClient openAIClient) : IOpenAiBroker
    {
        private readonly IOpenAIClient openAIClient = openAIClient;

        public async ValueTask<string> PostOpenAiPromptAsync(string prompt)
        {
            var chatCompletion =
                CreateChatCompletion(prompt);

            ChatCompletion resultChatCompletion =
              await openAIClient.ChatCompletions.SendChatCompletionAsync(
                  chatCompletion);

            return resultChatCompletion.Response.Choices[0].Message.Content;
        }

        private static ChatCompletion CreateChatCompletion(string prompt)
        {
            return new ChatCompletion
            {
                Request = new ChatCompletionRequest
                {
                    Model = "gpt-3.5-turbo",
                    Messages =
                    [
                        new() {
                           Content = prompt,
                            Role = "user",
                        }
                    ],
                    MaxTokens = 600
                }
            };
        }
    }
}
