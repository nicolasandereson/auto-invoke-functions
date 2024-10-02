using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

string yourDeploymentName = "";
string yourEndpoint = "";
string yourApiKey = "";

var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(
    yourDeploymentName,
    yourEndpoint,
    yourApiKey,
    "gpt-35-turbo-16k");
var kernel = builder.Build();


kernel.ImportPluginFromType<MusicLibraryPlugin>();
kernel.ImportPluginFromType<MusicConcertPlugin>();
kernel.ImportPluginFromPromptDirectory("Prompts");

OpenAIPromptExecutionSettings settings = new()
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

var songSuggesterFunction = kernel.CreateFunctionFromPrompt(
    promptTemplate: @"Based on the user's recently played music:
        {{$recentlyPlayedSongs}}
        recommend a song to the user from the music library:
        {{$musicLibrary}}",
    functionName: "SuggestSong",
    description: "Recommend a song from the library"
);

kernel.Plugins.AddFromFunctions("SuggestSong", [songSuggesterFunction]);

string prompt = @"Add this song to the recently played songs list:  title: 'Touch', artist: 'Cat's Eye', genre: 'Pop'";

var result = await kernel.InvokePromptAsync(prompt, new(settings));

Console.WriteLine(result);