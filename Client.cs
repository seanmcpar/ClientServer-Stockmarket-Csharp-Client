using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace StockClient
{
    class Client : IDisposable
    {
        private const int port = 10001;
        private readonly StreamReader reader;
        private readonly StreamWriter writer;
        private long traderId;
        private long currentHolderId;
        private long numberOfTraders;
        private List<long> traders = new List<long>();

        public Client()
        {
            TcpClient tcpClient = new TcpClient("localhost", port);
            NetworkStream stream = tcpClient.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);
            writer.WriteLine("CONNECT");
            writer.Flush();
        }

        public void RunClient()
        {
            traderId = Convert.ToInt64(reader.ReadLine());
            currentHolderId = Convert.ToInt64(reader.ReadLine());
            numberOfTraders = Convert.ToInt32(reader.ReadLine());
            
            for (int i = 0; i < numberOfTraders; i++)
            {
                long traderId = Convert.ToInt64(reader.ReadLine());
                traders.Add(traderId);
            }

            DisplayMarketToConsole();
            while (true)
            {

                try
                {
                    string message = null;
                    string command = reader.ReadLine();

                    switch (command)
                    {
                        case "NEW_TRADER":
                            long newTrader = Convert.ToInt64(reader.ReadLine());
                            numberOfTraders++;
                            traders.Add(newTrader);
                            message = $"New trader joined - Trader {newTrader}";
                            break;

                        case "NEW_STOCKHOLDER":
                            long newHolder = Convert.ToInt64(reader.ReadLine());
                            message = $"Stock has been traded to {newHolder}.";
                            currentHolderId = newHolder;
                            break;

                        case "TRADER_DISCONNECTED":
                            numberOfTraders--;
                            long disconnectedTrader = Convert.ToInt64(reader.ReadLine());
                            message = $"Trader {disconnectedTrader} has left the market.";
                            traders.Remove(disconnectedTrader);
                            break;

                        case "RECEIVE_TRADE":
                            message = "You have been traded the stock.";
                            ReceiveTrade();
                            break;

                        case "CONFIRM_TRADE":
                            long confirmedNewHolder = Convert.ToInt64(reader.ReadLine());
                            currentHolderId = confirmedNewHolder;
                            message = $"You have traded the stock to {confirmedNewHolder}.";
                            break;
                    }
                    DisplayMarketToConsole(message);
                }
                catch (Exception)
                {

                    //restart server
                }
            }
        }

        public void DisplayMarketToConsole(string message = null)
        {
            Console.Clear();
            if (message != null)
            {
                Console.WriteLine($"Update: {message}");
            }
            Console.WriteLine($"\nYour ID: {traderId}");
            Console.WriteLine($"\nCurrent Stock Holder ID: {currentHolderId}");
            Console.WriteLine($"\n{numberOfTraders} traders in Market:");
            for (int i = 0; i < numberOfTraders; i++)
            {
                Console.WriteLine($"Trader {traders[i]} : Has stock? {(traders[i]==currentHolderId ? "Yes" : "No" )}");
            }
            if (currentHolderId == traderId)
            {
                Console.WriteLine("\nEnter T to trade stock.");
            }
        }
    
        public void ReceiveTrade()
        {
            currentHolderId = traderId;
            Console.WriteLine("You have been traded the stock.");
            Console.WriteLine("\nEnter T to trade stock.");
        }

        public void Trade(long newHolder)
        {
            if (traders.Contains(newHolder))
            {
                writer.WriteLine($"START_TRADE {newHolder}");
                writer.Flush();
            }
            else
            {
                Console.WriteLine("\nUnknown Trader.");
                Console.WriteLine("\nEnter T to trade stock.");
            }
        }

        public bool IsCurrentStockHolder()
        {
            return currentHolderId == traderId;
        }

        public void Dispose()
        {
            reader.Close();
            writer.Close();
        }
    }
}
