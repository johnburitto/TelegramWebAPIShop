using System.Collections.Generic;

namespace Bot.Estensions
{
    public static class ListExtensions
    {
        public static Dictionary<int, int> ToCartDictionary(this List<int> list)
        {
            var dictionary = new Dictionary<int, int>();

            list.Sort();

            foreach (var el in list)
            {
                try
                {
                    dictionary.TryAdd(el, list.CountOfElement(el));
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return dictionary;
        }

        public static int CountOfElement(this List<int> list, int element)
        {
            int number = 0;

            foreach (var el in list)
            {
                if (el == element)
                {
                    number++; 
                }
            }

            return number;
        }
    }
}
