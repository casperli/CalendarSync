using System;
using System.Net.Http;
using Google.Apis.Calendar.v3.Data;

namespace CoreSync.Controllers
{
    public class CalendarEvent
    {
        public string Id { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public string DateTimeIdentifier { get; set; }

        public string Title { get; set; }

        public static string CreateDateTimeIdentifiier(DateTime start, DateTime end)
        {
            return $"{start:s}#{end:s}";
        }

        public bool IsEqualToGoogle(Event googleEvent)
        {
            return DateTimeIdentifier ==
                   CreateDateTimeIdentifiier(googleEvent.Start.DateTime.GetValueOrDefault(),
                       googleEvent.End.DateTime.GetValueOrDefault());
        }

        public CalendarEvent(string icalbuddystring)
        {
            try
            {
                var input = icalbuddystring.Split(new[] {"_#_"}, StringSplitOptions.RemoveEmptyEntries);
                Title = input[1];
                Id = input[2].Trim();

                Start = new DateTime(
                    int.Parse(input[0].Substring(0, 4)),
                    int.Parse(input[0].Substring(5, 2)),
                    int.Parse(input[0].Substring(8, 2))
                );

                // If All-Day-Event -> New start/end
                if (!input[0].Contains(" at "))
                {
                    End = new DateTime(
                        int.Parse(input[0].Substring(13, 4)),
                        int.Parse(input[0].Substring(18, 2)),
                        int.Parse(input[0].Substring(21, 2))
                    );

                    Start = Start.AddHours(7);
                    End = End.AddHours(18);
                }
                else
                {
                    End = Start;
                    Start = Start.AddHours(int.Parse(input[0].Substring(14, 2)));
                    Start = Start.AddMinutes(int.Parse(input[0].Substring(18, 2)));

                    End = End.AddHours(int.Parse(input[0].Substring(22, 2)));
                    End = End.AddMinutes(int.Parse(input[0].Substring(25, 2)));
                }


                DateTimeIdentifier = CreateDateTimeIdentifiier(Start, End);
            }
            catch (Exception e)
            {
                // Using exceptions for control flow is not recommended, but for this simple case sufficient
                throw new HttpRequestException($"Could not parse event: {icalbuddystring}", e);
            }
        }
    }
}