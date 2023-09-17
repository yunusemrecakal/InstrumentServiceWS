using NLog;
using Quartz;
using Quartz.Impl;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentServiceBO
{
    public class QuartzCore
    {
        public async void Start()
        {
            var redisConnectionString = ConfigurationManager.AppSettings["RedisConnection"].ToString();
            var redis = ConnectionMultiplexer.Connect(redisConnectionString);
            var redisDatabase = redis.GetDatabase(Convert.ToInt32(ConfigurationManager.AppSettings["RedisConnectionDbId"]));

            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = await schedulerFactory.GetScheduler();
            scheduler.Context.Put("RedisDatabase", redisDatabase);

            var job = JobBuilder.Create<InstrumentJob>()
                .WithIdentity("InstrumentJob")
                .Build();

            int jobTriggerMinute = Convert.ToInt32(ConfigurationManager.AppSettings["JobTriggerMinute"]);

            //string[] JobStartTime = ConfigurationManager.AppSettings["JobStartTime"].Split(':');
            //int startHour = Convert.ToInt32(JobStartTime[0]);
            //int startMinute = Convert.ToInt32(JobStartTime[1]);

            var trigger = TriggerBuilder.Create()
                .WithIdentity("instrumentTriggerName")
                //.StartAt(DateBuilder.TodayAt(startHour, startMinute, 0))
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(jobTriggerMinute)
                    .RepeatForever())
                .Build();

            await scheduler.ScheduleJob(job, trigger);

            await scheduler.Start();

            Console.WriteLine("Uygulama çalışıyor... Çıkmak için herhangi bir tuşa basın.");
            Console.ReadKey();

            await scheduler.Shutdown();
            LogManager.Shutdown();
            redis.Close();
        }
    }
}
