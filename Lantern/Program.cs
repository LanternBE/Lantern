namespace Lantern;

public static class Program { // TODO: This class is just temporary, I want to create a class to manage multiple servers, so maybe to create Hubs, etc!

    private static async Task Main() {

        var bedrockServer = new BedrockServer();
        await bedrockServer.StartAsync();
    }
}