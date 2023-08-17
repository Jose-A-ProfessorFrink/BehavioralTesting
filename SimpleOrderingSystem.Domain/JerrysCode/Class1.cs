using System.ComponentModel;
using Jerry2;
using SimpleOrderingSystem.Domain.Models;

namespace Jerry
{
    public class BigPapa
    {
        public static Class2 calculator;
        public static decimal Magic1 = 5M;

        static BigPapa()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

            calculator = (Class2)assembly.CreateInstance("Jerry2.Class2")!;
        }

        public static void DoIt(Order order, Movie? movie, string? q, bool normalize)
        {
            if(normalize)
            {
                goto Label1;
            }
            var items = order.Items;

            try
            {
                var orderItem = default(OrderItem);
                for(int i = 0; i < order.Items.Count; i++)
                {
                    if(order.Items[i].MovieId == movie!.Id)
                    {
                        orderItem = order.Items[i];
                        orderItem.Quantity += Convert.ToInt32(q);
                    }
                }
  
                if(orderItem == null)
                {
                    items.Add(new OrderItem() { MovieId = movie!.Id, Quantity = Convert.ToInt32(q), MovieYear = movie.Year, MovieMetascore = movie.Metascore});
                }

                calculator.CheckIt(order, items.ToArray(), null);
            }
            catch
            {
                calculator.DiscountCheck(items.ToArray());    

                PriceItAll(order);

                calculator.CheckIt(order, order.Items.ToArray(), "W");
            }

Label1:
            BigPapa.Normalize(order);
        }

        private static void PriceItAll(Order order)
        {
            foreach(var item in order.Items)
            {
                if(int.TryParse(item.MovieYear, out var year) && decimal.TryParse(item.MovieMetascore, out var metascore))
                {
                    metascore = metascore/100M;
                }
                else
                {
                    item.Price = Magic1;
                    continue;
                }

                var basePrice = year switch
                {
                    int y when y <= 1945 => 2M,
                    int y when y <= 1970 => 6M,
                    int y when y <= 2000 => 12M,
                    int y when y <= 2010 => 15M,
                    _ => 20M
                };

                item.Price = Math.Round(basePrice * metascore, 2);             
            }
        }

        public static void Method1(Order order)
        {
            if(order!.CreatedDateTimeUtc.Date.Year - order.Customer.DateHired.Year >= 10)
            {
                order.Discounts.Add(new OrderDiscount { Type = DiscountType.LoyalEmployee, PercentDiscount = .25M});
            }
        }

        public static void Normalize(Order order)
        {
            var flag1 = false;
            var flag2 = false;
            if(order.Discounts.Select(a=>a.Type == DiscountType.SeniorCitizen) is not null)
            {
                flag1 = true;
            }

            if(order.Discounts.Any(a=>(int)a.Type == 2))
            {
                flag2 = true;
            }

            if(flag1 && flag2)
            {
                order.Discounts.RemoveAll(a=> a.Type == (int)default(DiscountType));
            }

            order.Shipping = order.Items.Count > 3? order.Items.Count> 5? 10: 7: 5;
            var a = order.Items.Sum(a=>a.Quantity * a.Price);
            var b = order.Discounts.Sum(a=> a.PercentDiscount);
            var c = Math.Round(a * (1-b), 2) + order.Shipping;
            order.TotalCost = c;
        }
    }
}