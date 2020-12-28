using System;
using System.Threading;

namespace StockClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press enter to connect to server...");

            try
            {
                Console.ReadLine();

                using (Client client = new Client())
                {
                    Console.WriteLine("Connected.");

                    Thread thread1 = new Thread(() => client.RunClient());
                    thread1.Start();

                    while (true)
                    {
                        var choice = Console.ReadLine().Trim().ToUpper();

                        
                        switch (choice)
                        {
                            case "T":
                                if (client.IsCurrentStockHolder())
                                {
                                    Console.WriteLine("Enter the ID of the trader you want to send the stock:");

                                    if (Int64.TryParse(Console.ReadLine(), out long sendTo))
                                    {
                                        client.Trade(sendTo);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Not a valid number!\n");
                                        Console.WriteLine("\nEnter T to trade stock.");
                                    }                                                                 
                                }
                                else
                                {
                                    Console.WriteLine("You don't have the stock.");
                                }
                                break;

                            default:
                                Console.WriteLine($"Unknown command: {choice}");
                                if (client.IsCurrentStockHolder())
                                {
                                    Console.WriteLine("\nEnter T to trade stock.");
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                
            }
        }
    }
}
