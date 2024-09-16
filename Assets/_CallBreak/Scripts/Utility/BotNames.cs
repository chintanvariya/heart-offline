using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FGSOfflineCallBreak
{
    public static class BotNames
    {
        private static List<string> humanNames = new List<string> { "Alice", "Bob", "Charlie", "David", "Eva", "Frank", "Grace", "Hank", "Ivy", "Jack", "John", "Emma", "Michael", "Sophia", "Daniel", "Olivia", "David", "Ava", "Matthew", "Isabella", "Andrew", "Mia", "Christopher", "Emily", "Ethan", "Abigail", "Ryan", "Madison", "Nathan", "Grace", "Darmin", "Adarsh" };

        static BotNames()
        {
            ShuffleNames();
        }
        public static void ShuffleNames()
        {
            List<string> sorted = humanNames.OrderBy(a => Guid.NewGuid()).ToList();
            humanNames.Clear();
            humanNames.AddRange(sorted);
        }

        public static string RandomName()
        {
            string userName = humanNames[0];
            humanNames.RemoveAt(0);
            if (userName.Length > 10) userName = userName.Substring(0, 10);
            return userName;
        }

    }
}