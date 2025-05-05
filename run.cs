using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;


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
        int maxCapacity = int.Parse(Console.ReadLine());
        Console.ReadLine();
        int n = int.Parse(Console.ReadLine());


        List<Guest> guests = new List<Guest>();


        for (int i = 0; i < n; i++)
        {
            string line = Console.ReadLine();
            Guest guest = ParseGuest(line);
            guests.Add(guest);
        }


        bool result = CheckCapacity(maxCapacity, guests);


        Console.WriteLine(result ? "True" : "False");
    }


    static Guest ParseGuest(string json)
    {
        var guest = new Guest();

        // Извлекаем имя
        Match nameMatch = Regex.Match(json, "\"name\"\\s*:\\s*\"([^\"]+)\"");
        if (nameMatch.Success)
            guest.Name = nameMatch.Groups[1].Value;

        // Извлекаем дату заезда (исправлен check-in)
        Match checkInMatch = Regex.Match(json, "\"check-in\"\\s*:\\s*\"([^\"]+)\"");
        if (checkInMatch.Success)
            guest.CheckIn = checkInMatch.Groups[1].Value;

        // Извлекаем дату выезда (исправлен check-out)
        Match checkOutMatch = Regex.Match(json, "\"check-out\"\\s*:\\s*\"([^\"]+)\"");
        if (checkOutMatch.Success)
            guest.CheckOut = checkOutMatch.Groups[1].Value;

        return guest;
    }
}
