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
    }
}
