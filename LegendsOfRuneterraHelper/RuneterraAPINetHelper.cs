using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
namespace LegendsOfRuneterraHelper
{
    public class RuneterraAPINetHelper
    {
        static RuneterraAPINetHelper instance;

        static readonly string STATIC_DECKLIST_ENDPOINT = "static-decklist";
        static readonly string POSITIONAL_RECTANGLES_ENDPOINT = "positional-rectangles";
        static readonly string EXPEDITION_STATUS_ENDPOINT = "expeditions-state";
        static readonly string GAME_RESULT_ENDPOINT = "game-result";
        static int DEFAULT_PORT = 21337;
        static string DEFAULT_ADDRESS = "127.0.0.1";

         IPEndPoint localEndpoint;
         Ping ping;

        /////////////////////
        // PUBLIC METHODS //
        /////////////////////
        
        public static RuneterraAPINetHelper GetInstance()
        {
            if (instance != null)
            {
                return instance;
            }
            instance = new RuneterraAPINetHelper();
            return instance;

        }

        public bool APIAvailable()
        {
            JsonDocument temp;
            return QueryGameResult(out temp);
        }

        public string GetEndpointString()
        {
            return localEndpoint.ToString();
        }

        public bool SetAddress(string newAddress)
        {
            bool success = IPEndPoint.TryParse(DEFAULT_ADDRESS + ":" + DEFAULT_PORT, out localEndpoint);
            if (success)
            {
                return true;
            }
            Console.Error.WriteLine("Invalid Address");
            return false;
        }

        public bool QueryDeck(out JsonDocument result)
        {
            return Query("decklist", out result);
        }

        public bool QueryPositionalRectangles(out JsonDocument result)
        {
            return Query("positions", out result);
        }

        public bool QueryExpeditionState(out JsonDocument result)
        {
            return Query("expedition", out result);
        }

        public bool QueryGameResult(out JsonDocument result)
        {
            return Query("gameresult", out result);
        }

        /////////////////////
        // PRIVATE METHODS //
        /////////////////////
        
        private bool Query(string endpoint, out JsonDocument result)
        {
            StringBuilder URLBuilder = new StringBuilder();
            URLBuilder.Append("http://" + localEndpoint.ToString() + "/");

            switch (endpoint) 
            {
                case "decklist":
                    {
                        URLBuilder.Append(STATIC_DECKLIST_ENDPOINT);
                        break;
                    }
                case "positions":
                    {
                        URLBuilder.Append(POSITIONAL_RECTANGLES_ENDPOINT);
                        break;
                    }
                case "expedition":
                    {
                        URLBuilder.Append(EXPEDITION_STATUS_ENDPOINT);
                        break;
                    }
                case "gameresult":
                    {
                        URLBuilder.Append(GAME_RESULT_ENDPOINT);
                        break;
                    }
            }

            string URL = URLBuilder.ToString();
            //Console.WriteLine("Querying {0}", URL);

            // Save
            WebRequest decklistRequest = WebRequest.CreateHttp(URL);
            try
            {
                WebResponse response = decklistRequest.GetResponse();

                System.IO.Stream dataStream = response.GetResponseStream();

                result = JsonDocument.Parse(dataStream);
                return true;
            }
            catch (System.Net.WebException e)
            {
                //Console.Error.WriteLine(e.Message);

                result = null;
                return false;
            }
        }

        private RuneterraAPINetHelper()
        {
            SetAddress(DEFAULT_ADDRESS + ":" + DEFAULT_PORT);
            ping = new Ping();
        }


    }
}
