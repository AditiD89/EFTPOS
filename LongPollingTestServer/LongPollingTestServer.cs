
using EFTServer;
using Quartz;
using Quartz.Impl;


public class LongPollingTestServer
{

    static async Task Main(string[] args)
    {
         var builder = WebApplication.CreateBuilder(args);
        // Add services to the container.   
        builder.Services.AddControllers();      
        var app = builder.Build();

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
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

}




         
         

 

