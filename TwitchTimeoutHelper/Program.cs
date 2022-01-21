// See https://aka.ms/new-console-template for more information

using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

var funnyReasons = new[]
{
    "Sometimes in life the truth is better being kept from you, so I'll lie and say this timeout was an accident :/",
    "The problem with the gene pool is that there is no lifeguard.",
    "The only substitute for good manners is fast reflexes.",
    "Never test the depth of the water with both feet.",
    "Learn from your parents' mistakes: use birth control.",
    "I just got lost in thought. It was unfamiliar territory.",
    "Artificial Intelligence usually beats real stupidity.",
    "100,000 sperm and you were the fastest?",
    "Always remember you're unique - just like everyone else.",
    "Genius does what it must, talent does what it can, and you had best do what you're told.",
    "You get enough exercise just pushing your luck.",
    "Never miss a good chance to shut up.",
    "Never underestimate the power of stupid people in large groups.",
    "You have the right to remain silent. Anything you say will be misquoted then used against you.",
    "Are you a ninja? Let's see you dodge this kick.",
    "You are terminated.",
    "I'm driving through a tunnel! I'm going to lose you!"
};

if (!File.Exists("token.txt"))
{
    Console.WriteLine("token.txt not found.");
    Console.ReadKey();
    return;
}
var token = File.ReadAllText("token.txt").Split("\n").Select(line => line.Trim()).ToArray();

if (token.Length < 2)
{
    Console.WriteLine("invalid token.txt file. Ensure there are 2 lines, (First containing username, second containing oauth token)");
    Console.ReadKey();
    return;
}

var rand = new Random();
var credentials = new ConnectionCredentials(token[0], token[1]);
var clientOptions = new ClientOptions
{
    MessagesAllowedInPeriod = 750,
    ThrottlingPeriod = TimeSpan.FromSeconds(30)
};

var customClient = new WebSocketClient(clientOptions);
var client = new TwitchClient(customClient);
client.Initialize(credentials, token[0]);

client.OnMessageReceived += (sender, receivedArgs) =>
{
    if (receivedArgs.ChatMessage.UserType is not (UserType.Viewer or UserType.Moderator)) return;

    if (receivedArgs.ChatMessage.Message.StartsWith("!timeout", StringComparison.OrdinalIgnoreCase))
    {
        var reason = funnyReasons[rand.Next(funnyReasons.Length)];
        client.TimeoutUser(receivedArgs.ChatMessage.Channel, receivedArgs.ChatMessage.Username, TimeSpan.FromMinutes(10), reason);
    }
};

client.OnJoinedChannel += (sender, channelArgs) =>
{
    Console.WriteLine($"Joined channel #{channelArgs.Channel}");
};

Console.WriteLine("Starting bot");
client.Connect();
Console.WriteLine("Bot startup complete. Press any key to quit.");
Console.ReadKey();