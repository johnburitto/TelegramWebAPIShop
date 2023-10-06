using Core.Entities;

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
                    dictionary.TryAdd(el, list.Where(item => item == el).Count());
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return dictionary;
        }

        public static string ToOrderString(this List<Product> list)
        {
            var productString = "\n";
            var currentId = 0;
            var uniqueProductCounter = 0;

            list = list.OrderBy(product => product.Id).ToList();

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Id != currentId)
                {
                    currentId = list[i].Id;
                    uniqueProductCounter++;
                    productString += $"{uniqueProductCounter}. {list[i].Name} x{list.Where(product => product.Id == currentId).Count()}\n";
                }
            }

            return productString;
        }

        public static Order GetPreviousOrder(this List<Order> list, int id)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Id == id)
                {
                    try
                    {
                        return list[i - 1];
                    }
                    catch (Exception)
                    {
                        return list[i];
                    }
                }
            }

            return list[id];
        }

        public static Order GetNextOrder(this List<Order> list, int id)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Id == id)
                {
                    try
                    {
                        return list[i + 1];
                    }
                    catch (Exception)
                    {
                        return list[i];
                    }
                }
            }

            return list[id];
        }
    }
}
