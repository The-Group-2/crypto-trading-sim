using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CryptoSimulator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var httpClient = new HttpClient();
            var coinbaseUrl = "https://api.coinbase.com/v2";
            var cryptoData = new List<CryptoData>();

            // Retrieve a list of supported cryptocurrencies
            var response = await httpClient.GetAsync($"{coinbaseUrl}/currencies");
            var currenciesJson = await response.Content.ReadAsStringAsync();
            var currencies = JsonConvert.DeserializeObject<CurrencyResponse>(currenciesJson);

            // Retrieve current price and historical data for each cryptocurrency
            foreach (var currency in currencies.Data)
            {
                response = await httpClient.GetAsync($"{coinbaseUrl}/prices/{currency.Id}-USD/spot");
                var priceJson = await response.Content.ReadAsStringAsync();
                var priceResponse = JsonConvert.DeserializeObject<PriceResponse>(priceJson);

                response = await httpClient.GetAsync($"{coinbaseUrl}/prices/{currency.Id}-USD/historic?period=day");
                var historicJson = await response.Content.ReadAsStringAsync();
                var historicResponse = JsonConvert.DeserializeObject<HistoricResponse>(historicJson);

                cryptoData.Add(new CryptoData
                {
                    Name = currency.Name,
                    Ticker = currency.Id,
                    Price = priceResponse.Data.Amount,
                    HistoricPrices = historicResponse.Data.Prices
                });
            }

            // Display the list of cryptocurrencies and allow the user to select one
            Console.WriteLine("Select a cryptocurrency to simulate trading:");
            for (var i = 0; i < cryptoData.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {cryptoData[i].Name} ({cryptoData[i].Ticker})");
            }
            var selection = int.Parse(Console.ReadLine()) - 1;
            var selectedCrypto = cryptoData[selection];

            // Display the current price and historical data for the selected cryptocurrency
            Console.WriteLine($"Current price for {selectedCrypto.Name} ({selectedCrypto.Ticker}): ${selectedCrypto.Price}");
            Console.WriteLine($"Historical prices for {selectedCrypto.Name} ({selectedCrypto.Ticker}):");
            foreach (var historicPrice in selectedCrypto.HistoricPrices)
            {
                Console.WriteLine($"{historicPrice.Time.ToShortDateString()}: ${historicPrice.Price}");
            }

            // Simulate trading based on user input
            Console.WriteLine("Enter starting funds:");
            var startingFunds = decimal.Parse(Console.ReadLine());
            var currentFunds = startingFunds;
            var currentHolding = 0m;
                        while (true)
            {
                Console.WriteLine("Enter an action (buy/sell/quit):");
                var action = Console.ReadLine().ToLower();
                if (action == "quit")
                {
                    break;
                }

                if (action != "buy" && action != "sell")
                {
                    Console.WriteLine("Invalid action.");
                    continue;
                }

                Console.WriteLine($"Current funds: ${currentFunds}");
                Console.WriteLine($"Current holding: {currentHolding} {selectedCrypto.Ticker}");

                Console.WriteLine("Enter amount to trade:");
                var tradeAmount = decimal.Parse(Console.ReadLine());

                if (action == "buy")
                {
                    var purchasePrice = selectedCrypto.Price * tradeAmount;
                    if (purchasePrice > currentFunds)
                    {
                        Console.WriteLine("Insufficient funds.");
                        continue;
                    }
                    currentFunds -= purchasePrice;
                    currentHolding += tradeAmount;
                    Console.WriteLine($"Purchased {tradeAmount} {selectedCrypto.Ticker} for ${purchasePrice}.");
                }
                else // sell
                {
                    if (tradeAmount > currentHolding)
                    {
                        Console.WriteLine("Insufficient holding.");
                        continue;
                    }
                    var salePrice = selectedCrypto.Price * tradeAmount;
                    currentFunds += salePrice;
                    currentHolding -= tradeAmount;
                    Console.WriteLine($"Sold {tradeAmount} {selectedCrypto.Ticker} for ${salePrice}.");
                }
            }

            Console.WriteLine($"Final funds: ${currentFunds}");
            Console.WriteLine($"Final holding: {currentHolding} {selectedCrypto.Ticker}");
        }
    }

    class CryptoData
    {
        public string Name { get; set; }
        public string Ticker { get; set; }
        public decimal Price { get; set; }
        public List<HistoricPrice> HistoricPrices { get; set; }
    }

    class CurrencyResponse
    {
        public List<CurrencyData> Data { get; set; }
    }

    class CurrencyData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Min_size { get; set; }
    }

    class PriceResponse
    {
        public PriceData Data { get; set; }
    }

    class PriceData
    {
        public string Base { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
    }

    class HistoricResponse
    {
        public HistoricData Data { get; set; }
    }

    class HistoricData
    {
        public List<HistoricPrice> Prices { get; set; }
    }

    class HistoricPrice
    {
        public DateTime Time { get; set; }
        public decimal Price { get; set; }
    }
}

            
