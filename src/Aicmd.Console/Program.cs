using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using Spectre.Console;
using System.ClientModel;

var snapUserCommonPath = Environment.GetEnvironmentVariable("SNAP_COMMON");
var appSettingsPath = Path.Combine(snapUserCommonPath ?? string.Empty, "appsettings.json");
var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile(appSettingsPath, optional: false)
    .AddEnvironmentVariables(prefix: "AICMD_")
    .Build();

var endpoint = config["ENDPOINT"] ?? throw new InvalidOperationException("Endpoint must be set.");
var model = config["MODEL"] ?? throw new InvalidOperationException("The model must be set.");
var apiKey = config["API_KEY"];

if (string.IsNullOrWhiteSpace(apiKey))
{
    apiKey = "no-key-set";
}

var historyPath = History.GetPath();
var history = History.Load(historyPath);

// Write the prompt to stderr so stdout stays clean for the generated command.
await Console.Error.WriteAsync("> ");

// Open /dev/tty directly so user input works even when stdout is being captured
// by the shell wrapper.
string description;
using (var tty = new FileStream("/dev/tty", FileMode.Open, FileAccess.Read))
{
    description = TerminalInput.ReadLine(tty, history).Trim();
}

if (string.IsNullOrWhiteSpace(description))
    return;

History.Save(historyPath, history, description);

var clientOptions = new OpenAIClientOptions { Endpoint = new Uri(endpoint) };
var chatClient = new OpenAIClient(new ApiKeyCredential(apiKey), clientOptions)
    .GetChatClient(model);

var agent = chatClient.AsAIAgent(
    instructions: "You translate natural-language task descriptions into bash one-liners. "
        + "Respond with ONLY the bash command - no explanation, no markdown, no code fences. "
        + "The command must be ready to run as-is in a standard bash shell.",
    name: "BashTranslator");

// Spinner renders to stderr so stdout stays clean for the generated command.
var stderr = AnsiConsole.Create(new AnsiConsoleSettings
{
    Out = new AnsiConsoleOutput(Console.Error),
});

var command = await stderr.Status()
    .Spinner(Spinner.Known.Dots)
    .StartAsync(StatusMessage(), _ => agent.RunAsync(description));

Console.WriteLine(command.Text.Trim());

static string StatusMessage()
{
    string[] messages =
    [
        "Consulting the bash gods...",
        "Translating to shell-speak...",
        "Summoning the one-liner...",
        "Asking the terminal oracle...",
        "Piping your thoughts...",
        "Grepping through the matrix...",
        "sudo thinking...",
        "Parsing your intent...",
        "Compiling your wishes...",
        "Aliasing your description...",
        "Shelling out...",
        "Redirecting creativity...",
        "exec-ing your idea..."
    ];
    return messages[Random.Shared.Next(messages.Length)];
}
