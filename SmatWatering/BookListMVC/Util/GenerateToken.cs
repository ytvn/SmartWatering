using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartWatering.Util
{
    public class GenerateToken
    {
        private static string allChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static Random random = new Random();
        public static string Generate()
        {
            return new string(
               Enumerable.Repeat(allChar, 12)
               .Select(token => token[random.Next(token.Length)]).ToArray());
        }
    }
}
