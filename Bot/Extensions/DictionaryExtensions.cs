using Bot.HttpInfrastructure;
using Core.Entities;
using Newtonsoft.Json;

namespace Bot.Extensions
{
    public static class DictionaryExtensions
    {
        public static async Task<float?> TotalPriceAsync(this Dictionary<int, int> dictionary)
        {
            float? totalPrice = 0f;

            foreach (var el in dictionary)
            {
                var productResponse = await RequestClient.Client.GetAsync($"api/Product/{el.Key}");
                var product = JsonConvert.DeserializeObject<Product>(await productResponse.Content.ReadAsStringAsync());
                var discountPrice = product?.Price - product?.Price *
                    product?.Discounts?.Where(discount => discount.Status == DiscountStatus.Active).Select(discount => discount.NormalizedDiscount).Sum();

                totalPrice += el.Value * (discountPrice < product?.Price ? discountPrice : product?.Price);
            }

            return totalPrice;
        }

        public static int GetNextProduct(this Dictionary<int, int> dictionary, int currentId)
        {
            List<KeyValuePair<int, int>> enumDictionary = new(dictionary.AsEnumerable());

            for (int i = 0; i < enumDictionary.Count(); i++)
            {
                if (enumDictionary[i].Key == currentId)
                {
                    try
                    {
                        return enumDictionary[i + 1].Key;
                    }
                    catch(Exception)
                    {
                        return enumDictionary[i].Key;
                    }
                }
            }

            return currentId;
        }

        public static int GetPreviousProduct(this Dictionary<int, int> dictionary, int currentId)
        {
            List<KeyValuePair<int, int>> enumDictionary = new(dictionary.AsEnumerable());

            for (int i = 0; i < enumDictionary.Count(); i++)
            {
                if (enumDictionary[i].Key == currentId)
                {
                    try
                    {
                        return enumDictionary[i - 1].Key;
                    }
                    catch (Exception)
                    {
                        return enumDictionary[i].Key;
                    }
                }
            }

            return currentId;
        }
    }
}
