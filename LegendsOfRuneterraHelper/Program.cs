using System;
using System.Net;
using System.Text.Json;
using System.Threading;

namespace LegendsOfRuneterraHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            RuneterraAPINetHelper apiHelper = RuneterraAPINetHelper.GetInstance();
            RuneterraAPIDataDragon dataDragon = RuneterraAPIDataDragon.GetInstance();

            Console.WriteLine("Trying to connect to game server at {0}", apiHelper.GetEndpointString());

            bool wantToExit = false;

            while (!wantToExit)
            {
                // Try to find a running instance of the game
                while (!apiHelper.APIAvailable())
                {
                    Thread.Sleep(1000);
                    Console.WriteLine("Could not connect... Retrying in 1 second");
                }
                Console.WriteLine("Connected!");
                Console.WriteLine();

                JsonDocument deck;
                JsonDocument expedition;
                JsonDocument gameResult;
                JsonDocument positionalRectangles;


                JsonElement deckRoot;
                JsonElement expeditionRoot;
                JsonElement gameResultRoot;
                JsonElement positionalRectanglesRoot;
                JsonElement Rectangles;


                int delay = 5000;

                // Set locale
                dataDragon.SetLocale("en_us");
                // Find the directories with our data files 
                dataDragon.LocateDataFilePaths();
                // Load the card data JSON
                dataDragon.ParseData();

                Console.WriteLine("Beginning Game Data Queries");
                Console.WriteLine();

                // Continually query and output the API state for now
                // Sends a query every delay seconds
                while (apiHelper.APIAvailable())
                {
                    apiHelper.QueryDeck(out deck);
                    apiHelper.QueryExpeditionState(out expedition);
                    apiHelper.QueryGameResult(out gameResult);
                    apiHelper.QueryPositionalRectangles(out positionalRectangles);

                    deckRoot = deck.RootElement;
                    expeditionRoot = expedition.RootElement;
                    gameResultRoot = gameResult.RootElement;
                    positionalRectanglesRoot = positionalRectangles.RootElement;

                    JsonSerializerOptions o = new JsonSerializerOptions();
                    o.WriteIndented = true;

                    if (positionalRectanglesRoot.TryGetProperty("Rectangles", out Rectangles))
                    {
                        //Console.WriteLine(Rectangles);
                        //Console.WriteLine();
                    }

                    //Console.WriteLine("Deck: " + deckRoot);

                    // Get Deck Cards
                    JsonElement cardsRoot;
                    deckRoot.TryGetProperty("CardsInDeck", out cardsRoot);

                    // If the cards exist
                    if (cardsRoot.ValueKind != JsonValueKind.Null)
                    {
                        // { ID : count }
                        foreach (JsonProperty e in cardsRoot.EnumerateObject())
                        {
                            string cardName = dataDragon.GetCardName(e.Name.ToString());
                            Console.WriteLine(e.Value + "x " + cardName);
                        }
                    }
                    Console.WriteLine();

                    //Console.WriteLine("Expedition: " + expeditionRoot);
                    //Console.WriteLine("Last Game Result: " + gameResultRoot);
                    //Console.WriteLine("Positional Rectangles: " + positionalRectanglesRoot);
                    Console.WriteLine();
                    // Sleep for delay seconds
                    Thread.Sleep(delay);
                }
            }
        }
    }
}
