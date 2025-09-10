using CloudinaryDotNet.Actions;
using Efreshli.Application.Services.AIServices;
using OpenAI;
using OpenAI.Chat;
using Org.BouncyCastle.Asn1.Crmf;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class OpenAIService : IAIService
{
    private readonly ChatClient _chat;

    public OpenAIService(ChatClient chat)
    {
        _chat = chat;
    }

   

    public async Task<List<string>> SummarizeReviewsAsync(List<string> reviews)
    {
        var reviewsText = string.Join(" - ", reviews);

        var messages = new List<ChatMessage>
                {
                    new SystemChatMessage("أنت مساعد يلخّص مراجعات العملاء بشكل قصير ومفيد وبنقاط وترجع النقاط في مصفوفة JSON فقط بدون أي نص إضافي."),
                    new UserChatMessage($"لخّص المراجعات التالية في 4-6 نقاط: {reviewsText}")
                };

                var options = new ChatCompletionOptions
                {
                    MaxOutputTokenCount = 200,
                    Temperature = 0.2f
                };

        ChatCompletion completion = await _chat.CompleteChatAsync(messages, options);
        string response = completion.Content[0].Text;

        // Clean the response to get pure JSON array
        return CleanJsonResponse(response);
    }



    #region Helper Methods
    private List<string> CleanJsonResponse(string response)
    {
        // Remove markdown code block markers if present
        response = response.Replace("```json", "")
                           .Replace("```", "")
                           .Trim();

        try
        {
            // Try direct deserialization
            var jsonArray = JsonSerializer.Deserialize<List<string>>(response);
            if (jsonArray != null)
                return jsonArray;
        }
        catch (JsonException)
        {
            // Try regex fallback
            var match = Regex.Match(response, @"\[(.|\s)*\]", RegexOptions.Singleline);
            if (match.Success)
            {
                try
                {
                    var jsonArray = JsonSerializer.Deserialize<List<string>>(match.Value);
                    if (jsonArray != null)
                        return jsonArray;
                }
                catch { /* ignore, fallback below */ }
            }
        }

        // If everything fails, just wrap response in a list so caller won’t break
        return new List<string> { response };
    } 
    #endregion
}