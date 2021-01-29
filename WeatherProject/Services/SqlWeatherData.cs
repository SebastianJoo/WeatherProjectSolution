using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WeatherProject.Models;

namespace WeatherProject.Services
{
    public static class SqlWeatherData
    {
        static readonly string filePath = @"TemperaturData.csv";

        public static void FillDb()
        {
            using (var db = new WeatherDbContext())
            {
                NumberFormatInfo provider = new NumberFormatInfo
                {
                    NumberDecimalSeparator = ".",
                    NumberGroupSeparator = ",",
                    CurrencyDecimalDigits = 1
                };

                List<string> lines = File.ReadAllLines(filePath).ToList();

                foreach (var line in lines)
                {
                    string[] entries = line.Split(',');

                    var temperatureData = new TemperatureData
                    {
                        DateAndTime = DateTime.Parse(entries[0]),
                        Location = entries[1],
                        Temp = Convert.ToSingle(entries[2], provider),
                        Humidity = int.Parse(entries[3])
                    };

                    db.Temperatures.Add(temperatureData);
                }
                db.SaveChanges();
            }
        }
        public static IEnumerable<TemperatureData> GetAllTemperatureData()
        {
            var allTemperatures = new List<TemperatureData>();
            using (var db = new WeatherDbContext())
            {
                foreach (var temp in db.Temperatures)
                {
                    allTemperatures.Add(temp);
                }
            }
            return allTemperatures;
        }
        public static float GetAverageTemp(DateTime date, string location)
        {
            using (var db = new WeatherDbContext())
            {
                var averageTemp = db.Temperatures
               .Where(t => t.DateAndTime.Date == date && t.Location.ToLower() == location)
               .Average(t => t.Temp);

                return averageTemp;
            }
        }
        public static IEnumerable<TemperatureData> GetHighestTemps(string location)
        {
            var orderedTemperatures = new List<TemperatureData>();

            using (var db = new WeatherDbContext())
            {
                var temperatureQuery = db.Temperatures
                     .Where(t => t.Location.ToLower() == location)
                     .GroupBy(t => t.DateAndTime.Date)
                     .Select(t => new TemperatureData { DateAndTime = t.Key, Temp = t.Average(c => c.Temp) })
                     .OrderByDescending(t => t.Temp)
                     .AsEnumerable();

                foreach (var temperatureData in temperatureQuery)
                {
                    temperatureData.Temp = (float)Math.Round(temperatureData.Temp, 1);
                    orderedTemperatures.Add(temperatureData);
                }
            }
            return orderedTemperatures;
        }
        public static IEnumerable<TemperatureData> GetLowestHumidity(string location)
        {
            var orderedHumidity = new List<TemperatureData>();

            using (var db = new WeatherDbContext())
            {
                var humidityQuery = db.Temperatures
                    .Where(t => t.Location.ToLower() == location)
                    .GroupBy(t => t.DateAndTime.Date)
                    .Select(t => new TemperatureData { DateAndTime = t.Key, Humidity = (int)t.Average(h => h.Humidity) })
                    .OrderBy(h => h.Humidity)
                    .AsEnumerable();

                foreach (var temperatureData in humidityQuery)
                {
                    orderedHumidity.Add(temperatureData);
                }

            }
            return orderedHumidity;
        }
        public static List<string> GetTemperatureDifference()
        {
            List<string> tempDifferenceList = new List<string>();

            using (var db = new WeatherDbContext())
            {
                var q = db.Temperatures
                    .GroupBy(t => t.DateAndTime.Date)
                    .Select(t => new { Date = t.Key, InsideAverage = t.Where(t => t.Location == "inne").Average(x => x.Temp), OutsideAverage = t.Where(x => x.Location == "ute").Average(x => x.Temp) })
                    .OrderBy(t => (t.InsideAverage - t.OutsideAverage) >= 0 ? (t.InsideAverage - t.OutsideAverage) : -(t.InsideAverage - t.OutsideAverage));

                foreach (var day in q)
                {
                    var averageInsideTemperature = Math.Round(day.InsideAverage, 1);
                    var averageOutsideTemperature = Math.Round(day.OutsideAverage, 1);

                    var temperatureDifference = averageInsideTemperature - averageOutsideTemperature;

                    temperatureDifference = temperatureDifference > 0 ? temperatureDifference : -temperatureDifference;

                    tempDifferenceList.Add($"{day.Date.Date.ToShortDateString()} Temperatur Inomhus: {String.Format("{0:0.0}", averageInsideTemperature)}°" +
                        $"Temperatur Utomhus: {String.Format("{0:0.0}", averageOutsideTemperature)}° Temperaturskillnad: {String.Format("{0:0.0}", temperatureDifference)}°");
                }
            }
            return tempDifferenceList;
        }

        public static DateTime GetMeteorologicalAutumn()
        {
            using (var db = new WeatherDbContext())
            {
                var dateList = db.Temperatures
                    .Where(t => t.Location.ToLower() == "ute")
                    .GroupBy(t => t.DateAndTime.Date)
                    .Select(t => new TemperatureData { DateAndTime = t.Key, Temp = t.Average(a => a.Temp) })
                    .OrderBy(t => t.DateAndTime);

                int x = 0;

                foreach (var date in dateList)
                {
                    if (date.Temp < 10)
                    {
                        x++;
                        if (x == 5)
                        {
                            return date.DateAndTime.Subtract(TimeSpan.FromDays(x - 1));
                        }
                    }
                    else
                    {
                        x = 0;
                    }
                }
                return DateTime.Parse("9999 12 31"); // \[T]/
            }
        }
        public static DateTime GetMeteorologicalWinter()
        {
            using (var db = new WeatherDbContext())
            {
                var dateList = db.Temperatures
                    .Where(t => t.Location.ToLower() == "ute")
                    .GroupBy(t => t.DateAndTime.Date)
                    .Select(t => new TemperatureData { DateAndTime = t.Key, Temp = t.Average(a => a.Temp) })
                    .OrderBy(t => t.DateAndTime);

                int x = 0;

                foreach (var date in dateList)
                {
                    if ((int)date.Temp <= 0) // Casting to int required for result. (Otherwise meteorological winter does not occur.)
                    {
                        x++;
                        if (x == 5)
                        {
                            return date.DateAndTime.Subtract(TimeSpan.FromDays(x - 1));
                        }
                    }
                    else
                    {
                        x = 0;
                    }
                }
                return DateTime.Parse("9999 12 31"); //  (╯°□°）╯︵ ┻━┻ 
            }
        }
        public static IEnumerable<MoldData> GetMoldRisk(string location)
        {
            using (var db = new WeatherDbContext())
            {
                var q = db.Temperatures
                    .Where(t => t.Location.ToLower() == location)
                    .GroupBy(t => t.DateAndTime.Date)
                    .Select(t => new MoldData { DateAndTime = t.Key, Temp = t.Average(a => a.Temp), Humidity = (int)t.Average(h => h.Humidity) })
                    .OrderBy(t => t.DateAndTime);

                var moldRiskList = new List<MoldData>();

                foreach (var day in q)
                {
                    day.MoldRisk = Math.Round((((double)day.Humidity - 78) * (day.Temp / 15) / 0.22));

                    if (day.MoldRisk < 0)
                    {
                        day.MoldRisk = 0;
                        moldRiskList.Add(day);
                    }
                    else if (day.MoldRisk > 100)
                    {
                        day.MoldRisk = 100;
                        moldRiskList.Add(day);
                    }
                    else
                    {
                        moldRiskList.Add(day);
                    }
                }
                return moldRiskList
                    .OrderBy(m => m.MoldRisk)
                    .AsEnumerable();
            }
        }
        public static bool CheckDatabaseExists()
        {
            var connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TemperaturesDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            var databaseName = "TemperaturesDB";

            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand($"SELECT db_id('{databaseName}')", connection))
                {
                    connection.Open();
                    return (command.ExecuteScalar() != DBNull.Value);
                }
            }
        }
        public static DateTime MaxDate()
        {
            using (var db = new WeatherDbContext())
            {
                var maxDate = db.Temperatures
                         .Max(x => x.DateAndTime);
                return maxDate;
            }
        }
        public static DateTime MinDate()
        {
            using (var db = new WeatherDbContext())
            {
                var minDate = db.Temperatures
                         .Min(x => x.DateAndTime);
                return minDate;
            }
        }
        private static DateTime RemoveSeconds(DateTime date)
        {
            return date.Date
                .AddSeconds(-date.Date.Second);
        }
        public static List<Balcony> GetBalconyData()
        {
            List<Balcony> balconyOpen = new List<Balcony>();

            using (var db = new WeatherDbContext())
            {
                var temperatureQuery = db.Temperatures
                    .OrderBy(t => t.DateAndTime)
                    .Select(t => new { t.DateAndTime, t.Temp, t.Location })
                    .AsEnumerable()
                    .GroupBy(t => t.DateAndTime.Date)
                    .ToList();

                foreach (var day in temperatureQuery)
                {
                    List<TemperatureData> insideTempList = day.Where(i => i.Location == "inne")
                        .Select(i => new TemperatureData { DateAndTime = i.DateAndTime, Temp = i.Temp })
                        .ToList();
                    List<TemperatureData> outsideTempList = day.Where(o => o.Location == "ute")
                        .Select(o => new TemperatureData { DateAndTime = o.DateAndTime, Temp = o.Temp })
                        .ToList();

                    var index = 0;

                    var openTime = TimeSpan.Zero;
                    var openedAmount = 0;
                    for (int i = 0; i < insideTempList.Count; i++)
                    {
                        int counter = 0;
                        while (i + counter + 2 < insideTempList.Count)
                        {
                            bool timeCondition = insideTempList[i + counter + 1].DateAndTime - insideTempList[i + counter].DateAndTime < TimeSpan.Parse("00:10:00");
                            bool insideCondition = insideTempList[i + counter].Temp > insideTempList[i + counter + 1].Temp;

                            double? outsideTemp1 = 0;

                            outsideTemp1 = outsideTempList
                                .Where(o => RemoveSeconds(o.DateAndTime) == RemoveSeconds(insideTempList[i + counter].DateAndTime))
                                .Select(t => t.Temp)
                                .FirstOrDefault();

                            double? outsideTemp2 = 0;
                            outsideTemp2 = outsideTempList
                                .Where(o => RemoveSeconds(o.DateAndTime) == RemoveSeconds(insideTempList[i + counter + 1].DateAndTime))
                                .Select(t => t.Temp)
                                .FirstOrDefault();

                            bool outsideCondition = outsideTemp1 <= outsideTemp2;

                            if (timeCondition && outsideCondition && insideCondition)
                            {
                                index++;
                            }
                            else
                            {
                                break;
                            }
                            counter++;
                        }
                        if (index > 2)
                        {
                            openTime += (insideTempList[i + index].DateAndTime - insideTempList[i].DateAndTime);
                            openedAmount++;
                        }
                        i += counter;
                        index = 0;
                    }
                    balconyOpen.Add(new Balcony(insideTempList[0].DateAndTime.Date, openTime, openedAmount));
                }
                return balconyOpen
                    .OrderByDescending(b => b.DoorOpenTime)
                    .ToList();
            }
        }
    }
}
