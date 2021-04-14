using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace LegendsOfRuneterraHelper
{
    class RuneterraAPIDataDragon
    {
        static RuneterraAPIDataDragon instance;

        Dictionary<string, string> pathDict;
        Dictionary<string, JsonElement> cardJsonDict;
        List<int> setIDs;
        string locale;

        private RuneterraAPIDataDragon()
        {
            pathDict = new Dictionary<string, string>();
            cardJsonDict = new Dictionary<string, JsonElement>();
            setIDs = new List<int>();
        }

        public static RuneterraAPIDataDragon GetInstance()
        {
            if (instance != null)
            {
                return instance;
            }
            instance = new RuneterraAPIDataDragon();
            return instance;

        }

        public void SetLocale(string localeIn)
        {
            locale = localeIn;
        }

        // Data Loading

        public int LocateDataFilePaths()
        {
            Console.WriteLine("Locating data for locale {0}", locale);

            string currentDir = Directory.GetCurrentDirectory();

            IEnumerable<string> directories = Directory.EnumerateDirectories(currentDir);
            
            int filesFound = 0;

            foreach (string absoluteDir in directories)
            {
                string localDir = absoluteDir.Substring(currentDir.Length + 1);

                // Core
                if (localDir.StartsWith("core-" + locale))
                {
                    Console.WriteLine("Core directory found: {0}", localDir);

                    pathDict.Add("core", absoluteDir);
                    filesFound++;
                    continue;
                }

                // Data
                // Can do checks for lite ver here
                if (localDir.StartsWith("set") && localDir.EndsWith(locale))
                {
                    Console.WriteLine("Set directory found: {0}", localDir);

                    int setNumber = Convert.ToInt32(localDir[3]) - 48;

                    pathDict.Add("set" + setNumber, absoluteDir);
                    setIDs.Add(setNumber);

                    filesFound++;
                    continue;
                }
            }

            Console.WriteLine("Finished locating data");
            return filesFound;
        }


        public void ParseData()
        {
            Console.WriteLine("Loading card JSON data");
            string subdirectoryStructure = "\\" + locale + "\\data\\";
            int count;

            // Cards
           foreach (int i in setIDs)
            {
                count = 0;

                //ew
                string path = pathDict["set" + i] + subdirectoryStructure + "set" + i + "-" + locale + ".json";

                if (File.Exists(path))
                {
                    Console.WriteLine("Loading Set {0}", i);

                    Stream sr = new StreamReader(path).BaseStream;
                    JsonDocument setRootDoc = JsonDocument.Parse(sr);

                    JsonElement rootArray = setRootDoc.RootElement;
                    foreach(JsonElement card in rootArray.EnumerateArray())
                    {
                        cardJsonDict.Add(card.GetProperty("cardCode").ToString(), card);
                        count++;
                    }

                    sr.Close();
                }

                Console.WriteLine("Loaded {0} cards from set {1}", count, i);

            }
        }

        public string GetCardName(string ID)
        {
            if (cardJsonDict.ContainsKey(ID))
            {
                return cardJsonDict[ID].GetProperty("name").ToString();
            }
            return "";
        }

        // State Query
        public bool CoreDataLoaded(string locale)
        {
            return pathDict.ContainsKey("core-" + locale);
        }

        public bool SetDataLoaded(string locale, int set)
        {
            // Lite only for now
            return pathDict.ContainsKey("set" + set + "-lite-" + locale);
        }
    }
}
