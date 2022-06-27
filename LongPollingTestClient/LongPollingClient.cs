using EFTServer;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;


namespace HttpClientSample
{
    class LongPollingClient
    {
        static HttpClient client = new HttpClient();
        static int RetryCount = 0;
       
      
        //Function to POST Purchase Request to Server
        static async Task<int> CreatePurchaseAsync(Purchase purchase , Action<string> Callback)
        {
            string LocationURL = string.Empty;
            var response = await client.PostAsJsonAsync(
                "purchase", purchase);
            
            response.EnsureSuccessStatusCode();
           
            // return URI of the created resource.
            LocationURL = response.Headers.Location.ToString();

            Callback(LocationURL);
            return purchase.ID;
           
        }  
        
        static async void Callback(string LocationURL)
        {
            Purchase purchase = new Purchase();
            while(true)
            {
                purchase = await GetPurchaseAsync(LocationURL);
                if(purchase.Status !="Success")
                {
                    ShowMessage("Please Wait...Processing\n\n" + purchase.Status + "\n");
                    Thread.Sleep(2000);
                }
                else
                {
                    ShowMessage("Transaction Completed");
                    break;
                }
            }
        }

        private static void ShowMessage(String Message)
        {
            Console.WriteLine(Message);
        }

        //Function to call GET method to read Request status
        static async Task<Purchase> GetPurchaseAsync(string path)
        {
            Purchase purchase = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                purchase = await response.Content.ReadAsAsync<Purchase>();
            }                                           
            return purchase;
        }

        //Entry function Main() to start HttpClient for LongPolling
        static void Main()
        {
            Console.WriteLine("========================HTTPClient application- LongPollingClient===================");
            Console.WriteLine("");
            int i=0;
            // Update port # in the following line.
            client.BaseAddress = new Uri("https://localhost:5001/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
           

            while (true)
            {
                if (RetryCount <= 2)
                {
                    RunAsync().GetAwaiter().GetResult();
                }
                else
                {
                    ShowMessage("Server is disconnected");
                    break;
                }
            }

            ShowMessage("Press any key to close the application");
            Console.ReadKey();
        }

        static async Task RunAsync()
        {         
            try
            {
                Console.WriteLine("Create New Purchase Request (Y/N) ");                
                ConsoleKeyInfo result = Console.ReadKey();
                ShowMessage("");

                if ((result.KeyChar == 'Y') || (result.KeyChar == 'y'))
                {
                    Console.WriteLine("Enter PurchaseID :");
                    int ID = Int32.Parse(Console.ReadLine());
                    Console.WriteLine("Enter MerchantID :");
                    int MerchantID = Int32.Parse(Console.ReadLine());
                    Console.WriteLine("Enter Amount :");
                    float Amount = Int32.Parse(Console.ReadLine());
                    Console.WriteLine("");
                   
                    // Create a new Purchase
                    Purchase purchase = new Purchase
                    {
                        ID = ID,
                        MerchantID = MerchantID,
                        Amount = Amount,
                        Status = "Request Received"
                    };
                    var PurchaseID = await CreatePurchaseAsync(purchase,Callback);
                }

                else if ((result.KeyChar == 'N') || (result.KeyChar == 'n'))
                {
                    ShowMessage("Please Try again");
                    ShowMessage("");
                }
                else
                {
                    ShowMessage("Please Enter Valid Input (Y/N)");
                    ShowMessage("");
                }
            }
            catch (Exception e)
            {
                RetryCount++;
                ShowMessage("Trying to connect to server");
                //Console.WriteLine(e.Message);                
            }
        }
    }
}