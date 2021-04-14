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
            const string VERSION = "0.1";

            RuneterraAPINetHelper apiHelper = RuneterraAPINetHelper.GetInstance();
            RuneterraAPIDataDragon dataDragon = RuneterraAPIDataDragon.GetInstance();

            bool wantToExit = false;
            int reconnectDelay = 5000;
            int refreshDelay = 5000;

            JsonDocument deck;
            JsonDocument expedition;
            JsonDocument gameResult;
            JsonDocument positionalRectangles;


            JsonElement deckRoot;
            JsonElement expeditionRoot;
            JsonElement gameResultRoot;
            JsonElement positionalRectanglesRoot;
            JsonElement Rectangles;

            // Data
            bool shownDeck = false;
            JsonElement cardsRoot;
            JsonSerializerOptions o = new JsonSerializerOptions();
            o.WriteIndented = true;


            /////////////////////
            // BEGIN EXECUTION //
            /////////////////////

            Console.WriteLine("Legends of Runeterra(tm) Helper v{0}", VERSION);

            // Setup Data Dragon
            // Set locale
            dataDragon.SetLocale("en_us");
            // Find the directories with our data files 
            dataDragon.LocateDataFilePaths();
            // Load the card data JSON
            dataDragon.ParseData();

            while (!wantToExit)
            {
                Console.WriteLine("Trying to connect to game server at {0}", apiHelper.GetEndpointString());

                // Try to find a running instance of the game
                while (!apiHelper.APIAvailable())
                {
                    Thread.Sleep(reconnectDelay);
                    Console.WriteLine("Could not connect... Retrying in {0}s", reconnectDelay / 1000.0);
                }

                Console.WriteLine("Connected!");
                Console.WriteLine();
                Console.WriteLine("Beginning Game Data Queries");
                Console.WriteLine();

                // Continually query and output the API state for now
                // Sends a query every delay milliseconds
                while (apiHelper.APIAvailable())
                {
                    apiHelper.QueryDeck(out deck);
                    //apiHelper.QueryExpeditionState(out expedition);
                    //apiHelper.QueryGameResult(out gameResult);
                    //apiHelper.QueryPositionalRectangles(out positionalRectangles);

                    deckRoot = deck.RootElement;
                    //expeditionRoot = expedition.RootElement;
                    //gameResultRoot = gameResult.RootElement;
                    //positionalRectanglesRoot = positionalRectangles.RootElement;

                    /*
                    if (positionalRectanglesRoot.TryGetProperty("Rectangles", out Rectangles))
                    {
                        Console.WriteLine(Rectangles);
                        Console.WriteLine();
                    }
                    */
                    //Console.WriteLine("Deck: " + deckRoot);

                    // Get Deck Cards
                    deckRoot.TryGetProperty("CardsInDeck", out cardsRoot);

                    // If the cards exist
                    if (cardsRoot.ValueKind != JsonValueKind.Null)
                    {
                        if (!shownDeck) // Have not shown this deck
                        {
                            shownDeck = true;
                            // Show the deck!
                            foreach (JsonProperty e in cardsRoot.EnumerateObject())
                            {
                                dataDragon.PrintCardProperty(e, true);
                            }

                            Console.WriteLine();
                        }
                        // If have shown, we don't care - set a longer check timeout?
                        // Manage requests with a queue system or something so each one can have its own timeout?
                    }
                    else // Null deck, so the game ended. Prep for new deck
                    {
                        shownDeck = false;
                    }

                    //Console.WriteLine("Expedition: " + expeditionRoot);
                    //Console.WriteLine("Last Game Result: " + gameResultRoot);
                    //Console.WriteLine("Positional Rectangles: " + positionalRectanglesRoot);

                    // Sleep for delay seconds
                    Thread.Sleep(refreshDelay);
                }
            }
        }
    }
}
