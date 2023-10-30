using Caching.Redis;
using Newtonsoft.Json;
using NLog;
using Quartz;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentServiceBO
{
    [DisallowConcurrentExecution]
    public class InstrumentJob : IJob
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static RedisClientManager RedisClientManager { get; set; }

        public InstrumentJob()
        {
            RedisClientManager = RedisClientManager.Instance;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                logger.Warn($"JOB Çalıştı. {DateTime.Now}");

                var calendar = Talker.GetGtpCalendar();

                if (calendar != null)
                {
                    if (calendar.Today == calendar.AvailableBusinessDay)
                    {
                        TimeSpan session1EndSpan = new TimeSpan(calendar.Session1End / 60, calendar.Session1End % 60, 0);
                        DateTime session1EndTime = new DateTime(calendar.Today.Year, calendar.Today.Month, calendar.Today.Day).Add(session1EndSpan);

                        logger.Warn($"Bir sonraki tetikleme saati. {session1EndTime}");

                        ITrigger nextTrigger = TriggerBuilder.Create()
                                                            .WithIdentity("InstrumentTriggerForAvailableSession1", "group1")
                                                            .StartAt(session1EndTime)
                                                            .Build();
                    }
                    else
                    {
                        TimeSpan session1StartSpan = new TimeSpan(calendar.Session1Start / 60, calendar.Session1Start % 60, 0);
                        DateTime session1StartTime = new DateTime(calendar.NextBusinessDay.Year, calendar.NextBusinessDay.Month, calendar.NextBusinessDay.Day).Add(session1StartSpan).AddHours(-2);

                        logger.Warn($"Bir sonraki tetikleme saati. {session1StartTime}");

                        ITrigger nextTrigger = TriggerBuilder.Create()
                                                            .WithIdentity("InstrumentTriggerForAvailableSession2", "group1")
                                                            .StartAt(session1StartTime)
                                                            .Build();
                    }
                }

                var gds = Talker.GetInstrumentList();
                logger.Warn($"Enstruman listesi çekildi. Sayısı = {gds.Tables[0].Rows.Count} - {DateTime.Now}");

                DataRow[] rows = gds.Tables[0].Select("IS_MAIN = 1");
                SecurityInfo sec;
                List<SecurityInfo> securityInfoList = new List<SecurityInfo>();
                List<Instrument> instrumentList = new List<Instrument>();


                if (gds != null
                    && gds.Tables != null
                    && gds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in gds.Tables[0].Rows)
                    {
                        Instrument val = new Instrument
                        {
                            f = row["FIN_INST_ID"].ToString(),
                            n = GetCodeForClearExtention(row["CODE"].ToString()),
                            i = row["CODE"].ToString(),
                            g = row["EQUITY_TYPE"].ToString(),
                        };

                        sec = new SecurityInfo();
                        sec.N = GetCodeForClearExtention(row["CODE"].ToString());
                        sec.I = row["CODE"].ToString();
                        sec.F = row["FIN_INST_ID"].ToString();
                        val.r = sec.R = row["PRICE_REFERENCE_ID"].ToString(); //rules[0][1].ToString();

                        sec.G = row["EQUITY_TYPE"].ToString();
                        val.mc = sec.MC = HasColumn(row, "MARKET_CODE") && row["MARKET_CODE"] != DBNull.Value ? row["MARKET_CODE"].ToString() : string.Empty;
                        val.gc = sec.GC = HasColumn(row, "GROUP_CODE") && row["GROUP_CODE"] != DBNull.Value ? row["GROUP_CODE"].ToString() : string.Empty;
                        val.v = sec.V = row["IS_WARRANT"].ToString();
                        sec.S = row["TRADING_SESSION_DESC"].ToString();
                        val.desc = sec.D = row["DESCRIPTION"] != DBNull.Value ? row["DESCRIPTION"].ToString() : string.Empty;
                        val.ub = sec.UB = HasColumn(row, "USE_T1_BALANCE") && row["USE_T1_BALANCE"] != DBNull.Value && row["USE_T1_BALANCE"].ToString().Equals("1") ? 1 : 0;
                        val.st = sec.ST = HasColumn(row, "STATUS") && row["STATUS"] != DBNull.Value && row["STATUS"].ToString().Equals("1") ? 1 : 0;
                        val.om = sec.OM = HasColumn(row, "OUT_OF_MARKET") && row["OUT_OF_MARKET"] != DBNull.Value && row["OUT_OF_MARKET"].ToString().Equals("1") ? 1 : 0;
                        val.bs = sec.BS = HasColumn(row, "BOARD_STATU") && row["BOARD_STATU"] != DBNull.Value && row["BOARD_STATU"].ToString().Equals("1") ? 1 : 0;
                        val.sf = sec.SF = HasColumn(row, "SHORTFALL") && row["SHORTFALL"] != DBNull.Value && row["SHORTFALL"].ToString().Equals("1") ? 1 : 0;
                        sec.FP = HasColumn(row, "FREE_PRICE") && row["FREE_PRICE"] != DBNull.Value && row["FREE_PRICE"].ToString().Equals("1");
                        val.d = sec.LastUpd = HasColumn(row, "PRICE_LAST_UPD") && row["PRICE_LAST_UPD"] != DBNull.Value ? ((DateTime)row["PRICE_LAST_UPD"]).ToString("HH:mm:ss") : "-";

                        decimal lowerLimit = 0;
                        if (row["LOWER_LIMIT_CALCULATED"] != DBNull.Value)
                        {
                            lowerLimit = row["LOWER_LIMIT_CALCULATED"] != DBNull.Value
                                ? decimal.Round(Convert.ToDecimal(row["LOWER_LIMIT_CALCULATED"]), 3, MidpointRounding.AwayFromZero)
                                : 0;
                        }
                        if (lowerLimit <= 0 && row["LOWER_LIMIT"] != DBNull.Value)
                        {
                            lowerLimit = row["LOWER_LIMIT"] != DBNull.Value
                                ? decimal.Round(Convert.ToDecimal(row["LOWER_LIMIT"]), 3, MidpointRounding.AwayFromZero)
                                : 0;
                        }
                        val.ll = sec.LowerLimit = lowerLimit;

                        decimal upperLimit = 0;
                        if (row["UPPER_LIMIT_CALCULATED"] != DBNull.Value)
                        {
                            upperLimit = row["UPPER_LIMIT_CALCULATED"] != DBNull.Value
                            ? decimal.Round(Convert.ToDecimal(row["UPPER_LIMIT_CALCULATED"]), 3, MidpointRounding.AwayFromZero)
                            : 0;
                        }
                        if (upperLimit <= 0 && row["UPPER_LIMIT"] != DBNull.Value)
                        {
                            upperLimit = row["UPPER_LIMIT"] != DBNull.Value
                            ? decimal.Round(Convert.ToDecimal(row["UPPER_LIMIT"]), 3, MidpointRounding.AwayFromZero)
                            : 0;
                        }
                        val.ul = sec.UpperLimit = upperLimit;

                        if (sec != null
                            && !string.IsNullOrEmpty(sec.F)
                            && !securityInfoList.Any(x => x.F.Equals(sec.F)))
                        {
                            securityInfoList.Add(sec);
                        }

                        val.y = decimal.Round(Convert.ToDecimal(row["YESTERDAY_CLOSE_PRICE"]), 3, MidpointRounding.AwayFromZero);
                        val.l = decimal.Round(Convert.ToDecimal(row["LAST_PRICE"]), 3, MidpointRounding.AwayFromZero);
                        val.a = decimal.Round(Convert.ToDecimal(row["ASK_PRICE"]), 3, MidpointRounding.AwayFromZero);
                        val.b = decimal.Round(Convert.ToDecimal(row["BID_PRICE"]), 3, MidpointRounding.AwayFromZero);
                        val.t = row["VB_TRANSACTABLE"].ToString();

                        if (!string.IsNullOrEmpty(val.f) && !instrumentList.Any(x => x.f.Equals(val.f)))
                            instrumentList.Add(val);
                    }
                }

                string cacheKeyHashedList = "TSREST:SECURITY_INFO:HASHED_INSTRUMENTS";
                string cacheKeyList = "TSREST:INSTRUMENTS";

                List<HashEntry> hashEntryList = securityInfoList
                .Select(x => new HashEntry(GetCodeForClearExtention(x.N), JsonConvert.SerializeObject(x)))
                .ToList();


                //var redisDatabase = context.Scheduler.Context["RedisDatabase"] as IDatabase;
                RedisClientManager.Delete(cacheKeyHashedList);
                logger.Warn($"Redis Key silindi. {cacheKeyHashedList} - {DateTime.Now}");
                RedisClientManager.SetHash(cacheKeyHashedList, hashEntryList.ToArray());
                logger.Warn($"Redis Key eklendi. {cacheKeyHashedList} - {DateTime.Now}");
                RedisClientManager.Delete(cacheKeyList);
                logger.Warn($"Redis Key silindi. {cacheKeyList} - {DateTime.Now}");
                RedisClientManager.Set(cacheKeyList, JsonConvert.SerializeObject(instrumentList));
                logger.Warn($"Redis Key eklendi. {cacheKeyList} - {DateTime.Now}");

                logger.Warn($"JOB Tamamlandı. {DateTime.Now}");
            }
            catch (Exception ex)
            {
                logger.Error($"Error: {ex.Message} Date: {DateTime.Now}");
            }
        }


        public string GetCodeForClearExtention(string displayCode)
        {
            if (!string.IsNullOrEmpty(displayCode))
            {
                if (displayCode.EndsWith(".F1") || displayCode.EndsWith(".EUR"))
                {
                    displayCode = displayCode.Replace(".V", "")
                        .Replace(".G", "");
                }
                else
                {
                    displayCode = displayCode.Replace(".E", "")
                        .Replace(".V", "")
                        .Replace(".G", "")
                        .Replace(".F", "")
                        .Replace(".S1", "");
                }
            }
            return displayCode;
        }

        public static bool HasColumn(DataRow row, string column)
        {
            return row.Table.Columns.Contains(column) ? true : false;
        }
    }
}
