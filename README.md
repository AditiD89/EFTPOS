# DotNetTestServerClient

## Project structure

| Folder                      | Description   
| --------------------------- | -------------
| LongPollingServer           | A project to Create HTTP Server performs Asynchrounous processing
| LongPollingTestClient       | A project to Create HTTPCLient
| SharedLibrary               | A shared dlls for database operations

## Getting started
* Install SQL Server and SSMS 18 and Configure Database details in to "DBConnection.json" file in LongPollingServer project.
* Run Purchase_SQL.sql script to add Table dbo.Purchase and Respective Stored Procedures.
* copy DBConnection.json from ...\LongPollingTestServer to \LongPollingTestServer\bin\Debug\net6.0\
* Run LongPollingServer.exe first to start the server
* Run LongPollingTestClient and follow the instruction on console to perform the transaction 
* Create Purchase Request by passing values for PurchaseID, MerchantID and Amount and observe the Transaction process.

##### Example usage for LongPollingServer
``` csharp
public class LongPollingTestServer
{

    static async Task Main(string[] args)
    {
         var builder = WebApplication.CreateBuilder(args);
        // Add services to the container.   
        builder.Services.AddControllers();
      
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        //builder.Services.AddEndpointsApiExplorer();
        //builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            //app.UseSwagger();
            //app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        //app.Run();   

        Task T1 = app.RunAsync();      
        

        // Grab the Scheduler instance from the Factory
        StdSchedulerFactory factory = new StdSchedulerFactory();
        IScheduler scheduler = await factory.GetScheduler();

        // and start it off
       await scheduler.Start();
        Console.WriteLine("===================Purchase Request Data Update Task==================");
        // define the job and tie it to our HelloJob class
        IJobDetail job = JobBuilder.Create<DataUpdateJob>()
            .WithIdentity("job1", "group1")
            .Build();
               
        // Trigger the job to run now, and then repeat every 10 seconds
        ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity("trigger1", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(3)
                .RepeatForever())
            .Build();

        // Tell quartz to schedule the job using our trigger
        await scheduler.ScheduleJob(job, trigger);
        

        // and last shut down the scheduler when you are ready to close your program
        //await scheduler.Shutdown(); 

        Console.WriteLine("Press any key to close the application");
        Console.ReadKey();
               
    }

```

##### Example usage for LongPollingTestClient
``` csharp
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
            Console.WriteLine("Connection Established with Server");
            Console.WriteLine("");

            while (true)
            {
                if (RetryCount <= 2)
                {
                    RunAsync().GetAwaiter().GetResult();
                }
                else
                {
                    Console.WriteLine("Server is disconnected");
                    break;
                }
            }

            Console.WriteLine("Press any key to close the application");
            Console.ReadKey();
        }

        static async Task RunAsync()
        {         
            try
            {
                Console.WriteLine("Create New Purchase Request (Y/N) ");                
                ConsoleKeyInfo result = Console.ReadKey();
                Console.WriteLine("");

                if ((result.KeyChar == 'Y') || (result.KeyChar == 'y'))
                {
                    Console.Write("Enter PurchaseID :");
                    int ID = Int32.Parse(Console.ReadLine());
                    Console.Write("Enter MerchantID :");
                    int MerchantID = Int32.Parse(Console.ReadLine());
                    Console.Write("Enter Amount :");
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
                    var url = await CreatePurchaseAsync(purchase);
                    Console.WriteLine("");
                    Console.WriteLine("Updating Purchase Request Status");
                    Console.WriteLine("");                    

                    while (true)
                    {
                        purchase = await GetPurchaseAsync("Purchase/GetRecord/" + purchase.ID);
                        if (purchase.Status != "Success")
                        {
                            await GetPurchaseAsync(url.PathAndQuery);
                            ShowCustomPurchase(purchase);
                            Console.WriteLine("");
                            Thread.Sleep(2000);
                        }
                        else
                        {
                            Console.WriteLine("Response code : 200(OK)");
                            Console.WriteLine("Processing Completed");
                            ShowCustomPurchase(purchase);
                            Console.WriteLine("");
                            Console.WriteLine("Transaction Completed");
                            break;
                        }
                    }
                    Console.WriteLine("");

                }

                else if ((result.KeyChar == 'N') || (result.KeyChar == 'n'))
                {
                    Console.WriteLine("Please Try again");
                    Console.WriteLine("");
                }
                else
                {
                    Console.WriteLine("Please Enter Valid Input (Y/N)");
                    Console.WriteLine("");
                }
            }
            catch (Exception e)
            {
                RetryCount++;
                Console.WriteLine("Trying to connect to server");
                //Console.WriteLine(e.Message);                
            }
        }
    }
    ...

## Release notes

### 1.5.0.0 (2020-01-28)
• Added Demo POS for product level blocking transactions
• TEST & Async POS updated to support new cloud pairing process

### 1.4.5.0 (2018-12-14)
• Added in Void transaction type...
• Added in a check on msg length for parsing Duplicate Receipt responses so it can handle TPP duplicate responses
• Fixed 'Display Swipe Card' slave command
• Added in support for Input On POS display requests
• Added in MerchantNumber field for GetLastReceipt

### 1.4.3.0 (2018-10-09)
* Deleted a hard-coded TxnRef in TestPOS GetLast and ReprintReceipt command
* Fixed bug in MessageParser that padded the TxnRef rather than leaving it blank, so the EFTClient didn't like it

### 1.4.2.0 (2018-09-19)
* Added new ReceiptAutoPrint modes for EFTRequests
* Updated MessageParser to use non-deprecated properties
* Updated TestPOS ClientViewModel to do the same

### 1.4.1.3 (2018-09-12)
* Fixed for EFTTransactionResponse and typo

### 1.4.1.2 (2018-09-12)
* Changes to fields ReceiptAutoPrint, CutReceipt, AccountType and DateSettlement

### 1.4.1.1 (2018-08-29)
* Added support for EFTGetLastTransactionRequest by TxnRef

### 1.4.1.0 (2018-07-17)
* Updated PadField to support IList<PadTag>

### 1.4.0.0 (2018-04-30)
* Added IDialogUIHandler for easier handling of POS custom dialogs.
* Updated MessageParser to allow for custom parsing.

### 1.3.5.0 (2018-02-16)
* Added support for .NET Standard 2.0
* Added support for basket data API
* Updated some property names to bring EFTClientIP more inline with the existing ActiveX interface. Old property names have been marked obsolete, but are still supported.

### 1.3.3.0 (2017-10-26)
* Changed internal namespaces from `PCEFTPOS.*` (`PCEFTPOS.Net`, `PCEFTPOS.Messaging` etc) to `PCEFTPOS.EFTClient.IPInterface`. This was causing issues when combining the EFTClientIP Nuget package with the actual PCEFTPOS lib. EFTClientIP needs to remain totally self-contained. 

### 1.3.2.0 (2017-19-09)
* Updated nuspec for v1.3.2.0 release.

### 1.3.1.0 (2017-09-13)
* Changed namespace from `PCEFTPOS.API.IPInterface` to `PCEFTPOS.EFTClient.IPInterface` for new package
* Created signed NuGet package

### 1.2.1.0 (2017-07-11)
* Added CloudLogon event to EFTClientIP

### 1.1.0.0 (2017-06-30)
* Fixed a bug that would cause the component to hang if an unknown message was received 
* Improved handling of messages received across multiple IP packets
* Added support for Pay at Table

### 1.0.0.1 (2016-10-28)
* Initial release
