using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3;
using System.Net.Mail;

var aiTalk = new IATalk("el origen de dragon ball z");
await aiTalk.Run(5);


public class IATalk
{
    private string _topic;
    private string _messageA;
    private string _messageB;

    private string _apikey = "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";

    private OpenAIService _OpenAIService;
    private readonly string _initialText = "Como comenzarias con una pregunta sobre una platica sobre ";
    private readonly string _continueText = "contesta a lo siguiente de forma que parezca un debate: ";

    
    public string InitQuestion
    {
        get
        {
            return _initialText + _topic;
        }
    }

    public IATalk(string topic)
    {
        _topic= topic;
        _OpenAIService = new OpenAIService(new OpenAiOptions()
        { ApiKey = _apikey }
            );
    }

    public async Task Run(int limit = 10)
    {

        Console.WriteLine("el tema para platicar IA" + _topic);
        await Send(InitQuestion, (string response) => {
            _messageA = response;
            Console.WriteLine("A dice " + _messageA);
            Console.WriteLine("--------------------------");


        });

        for (int i = 0; i < limit; i++)
        {
            await Send(_continueText + _messageA, (string response) =>
            {
                _messageB = response;
                Console.WriteLine("B dice " + _messageB);
                Console.WriteLine("--------------------------");
            });
            await Send(_continueText + _messageB, (string response) =>
            {
                _messageA = response;
                Console.WriteLine("A dice " + _messageA);
                Console.WriteLine("--------------------------");
            });




        }
    }
        private async Task Send(string message, Action<string> fnSetResponse)
        {
            var completionResult = await _OpenAIService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
            {
                Messages = new List<ChatMessage>
            { ChatMessage.FromUser(message)
            },
                Model = Models.ChatGpt3_5Turbo
            });
            if (completionResult.Successful)
            {
                fnSetResponse(completionResult.Choices.First().Message.Content);
            }
            else {
                if (completionResult.Error == null)
                {
                    throw new Exception("Unknow error");
                }
                Console.Write($"{completionResult.Error.Code}: {completionResult.Error.Message}");
            }

        }
    }