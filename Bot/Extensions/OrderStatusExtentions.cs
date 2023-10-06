using Core.Entities;

namespace Bot.Extensions
{
    public static class OrderStatusExtentions
    {
        public static string ToEmoji(this OrderStatus status)
        {
            switch (status)
            {
                case OrderStatus.Canceled:
                    {
                        return "❌";
                    }
                case OrderStatus.Created:
                    {
                        return "⏳";
                    }
                case OrderStatus.Approved:
                    {
                        return "✅";
                    }
                case OrderStatus.Paid:
                    {
                        return "💸";
                    }
                case OrderStatus.Send:
                    {
                        return "🚚";
                    }
                default:
                    {
                        return string.Empty;
                    }
            }
        }
    }
}
