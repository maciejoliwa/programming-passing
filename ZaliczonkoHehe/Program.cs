using System;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.IO;
using System.Text;

namespace Zaliczonko
{

    class Database
    {
        private string filepath;

        public Database(string pathToDBFile)
        {
            if (!File.Exists(pathToDBFile))
            {
                var fs = File.Create(pathToDBFile);
                fs.Write(new UTF8Encoding(true).GetBytes("[]"));
                fs.Close();
            }

            filepath = pathToDBFile;
        }

        private List<Dictionary<string, JsonElement>> FilterByQuery(List<Dictionary<string, JsonElement>> values, string query)
        {
            var listToReturn = new List<Dictionary<string, JsonElement>>();
            var whereConditions = query.Substring(query.IndexOf("where") + "where".Length).Trim();
            var conditions = whereConditions.Split("and");  // { "id > 0", "name = "Test", ... }
            string[][] slicedConditions = new string[conditions.Length][];

            for (int i = 0; i < conditions.Length; i++)
            {
                var condition = conditions[i].Trim();
                slicedConditions[i] = condition.Split(' ');  // { { "id", ">", "0" }, { "name", "=", "Test" } } 
            }
            
            foreach (Dictionary<string, JsonElement> item in values)
            {
                bool[] shouldBeConsidered = new bool[slicedConditions.Length];
                int i = 0;
                foreach (string[] condition in slicedConditions)
                {
                    var key = condition[0];
                    var comparision = condition[1];
                    var value = condition[2];

                    if (!item.ContainsKey(key))
                    {
                        continue;
                    } 

                    switch (comparision)
                    {
                        case ">":
                            shouldBeConsidered[i] = item[key].GetInt32() > int.Parse(value);
                            break;
                        case "<":
                            shouldBeConsidered[i] = item[key].GetInt32() < int.Parse(value);
                            break;
                        case "=":
                            shouldBeConsidered[i] = item[key].ToString() == value;
                            break;
                        case " ":
                            break;
                        default:
                            break;

                    }

                    i++;
                }
                if (shouldBeConsidered.All(b => b))
                {
                    listToReturn.Add(item);
                }
            }

            return listToReturn;
        }

        public void ExecuteQuery(string query)
        {
            query = query.TrimEnd();
            string fileContents = File.ReadAllText(filepath);
            var values = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(fileContents);

            
            if (values != null)
            {
                if (query.StartsWith("find"))
                {
                    if (query.Contains("where"))
                    {
                        values = FilterByQuery(values, query);
                    }
                    if (query.Split(' ').Length > 1 && query.Split(' ')[1] == "all")
                    {
                        foreach (Dictionary<string, JsonElement> item in values)
                        {
                            Console.Write("{\n");
                            foreach (string key in item.Keys)
                            {
                                Console.WriteLine("\t{0}: {1},", key, item[key]);
                            }
                            Console.Write("}\n");
                        }
                    } else
                    {
                        var item = values.First();
                        Console.Write("{\n");
                        foreach (string key in item.Keys)
                        {
                            Console.WriteLine("\t{0}: {1},", key, item[key]);    
                        }
                        Console.Write("}\n");
                    }
                }
                else if (query == "clear")
                {
                    Console.Clear();
                }
                else if (query == "exit")
                {
                    Environment.Exit(0);
                }
                else if (query == "find one")
                {
                    Console.Write("{\n");
                    foreach (string key in values[0].Keys)
                    {
                        Console.WriteLine("\t{0}: {1},", key, values[0][key]);
                    }
                    Console.Write("}\n");
                }
                else if (query.StartsWith("insert"))
                {
                    var toInsert = query.Substring(query.IndexOf("insert") + "insert".Length).Trim();
                    try
                    {
                        var jsonified = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(toInsert);
                        values.Add(jsonified);
                        var overwriteDbText = JsonSerializer.Serialize(values);

                        File.WriteAllText(filepath, overwriteDbText);
                    }
                    catch (JsonException)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("JSON Error. Possibly an error with provided JSON syntax.");
                        Console.ResetColor();
                    }
                    
                }
            }
        }
    }

    class Entry
    {

        static void Main(string[] args)
        {
            Database database;

            try
            {
                database = new Database(args.First());
            } catch (ArgumentNullException)
            {
                Console.WriteLine("You need to provide an argument with database path!");
                return;
            } 

            while (true)
            {
                Console.Write("> ");
                string userInput = Console.ReadLine();

                if (userInput == null || userInput.Trim().Length == 0)
                {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Invalid input. Please try again.");
                    Console.ResetColor();
                } else
                {
                    database.ExecuteQuery(userInput);
                }


            }               
        }
    }
}