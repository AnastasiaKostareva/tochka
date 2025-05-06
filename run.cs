using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


class HotelCapacity
{
    static bool CheckCapacity(int maxCapacity, List<Guest> guests)
    {
        var dictWithDate = new Dictionary<DateTime, int>();
        foreach (var guest in guests)
        {
            var dateIn = guest.CheckIn.Split('-').Select(x => int.Parse(x)).ToList();
            var dateTimeIn = new DateTime(dateIn[0], dateIn[1], dateIn[2]);
            if (dictWithDate.ContainsKey(dateTimeIn))
                dictWithDate[dateTimeIn] += 1;
            else
                dictWithDate[dateTimeIn] = 1;

            var dateOut = guest.CheckOut.Split('-').Select(x => int.Parse(x)).ToList();
            var dateTimeOut = new DateTime(dateOut[0], dateOut[1], dateOut[2]);
            if (dictWithDate.ContainsKey(dateTimeOut))
                dictWithDate[dateTimeOut] -= 1;
            else
                dictWithDate[dateTimeOut] = -1;
        }

        var IEnumerableWithDate = dictWithDate.OrderBy(x => x.Key);
        var currSum = 0;
        foreach (var date in IEnumerableWithDate)
        {
            currSum += date.Value;
            if (currSum > maxCapacity)
                return false;
        }
        return true;
    }

    class Guest
    {
        public string Name { get; set; }
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
    }


    static void Main()
    {
        var maxCapacity = int.Parse(Console.ReadLine());
        var line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line))
            line = Console.ReadLine();

        var n = int.Parse(line);
        var guests = new List<Guest>();


        for (int i = 0; i < n; i++)
        {
            var data = Console.ReadLine();
            var guest = ParseGuest(data);
            guests.Add(guest);
        }

        var result = CheckCapacity(maxCapacity, guests);

        Console.WriteLine(result ? "True" : "False");
    }


    static Guest ParseGuest(string json)
    {
        var guest = new Guest();

        var nameMatch = Regex.Match(json, @"""name""\s*:\s*""([^""]*)""");
        var checkInMatch = Regex.Match(json, @"""check-in""\s*:\s*""([^""]*)""");
        var checkOutMatch = Regex.Match(json, @"""check-out""\s*:\s*""([^""]*)""");

        if (nameMatch.Success)
            guest.Name = nameMatch.Groups[1].Value;
        if (checkInMatch.Success)
            guest.CheckIn = checkInMatch.Groups[1].Value;
        if (checkOutMatch.Success)
            guest.CheckOut = checkOutMatch.Groups[1].Value;

        return guest;
    }
}
