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
                var bagOJunk = items.ToDictionary(a=> a.MovieId, a=> a);
                bagOJunk.Add(movie!.Id, 
                    new OrderItem() { MovieId = movie!.Id, Quantity = Convert.ToInt32(q), MovieYear = movie.Year, MovieMetascore = movie.Metascore});
                order.Items.Add(bagOJunk.Last().Value);
            }
            catch
            {
                Rick.BadMath(order, movie!, q!);
            }

            calculator.DiscountCheck(items.ToArray());    

            calculator.CheckIt(order, order.Items.ToArray(), "W");
            
            calculator.EndItAll(order);

Label1:
            BigPapa.Normalize(order);
        }

        public static void Method1(Order order)
        {
            if(order!.CreatedDateTimeUtc.Date.Year - order.Customer.DateHired.Year >= 15)
            {
                order.Discounts.Add(new OrderDiscount { Type = DiscountType.LoyalEmployee, PercentDiscount = .25M});
            }
        }

        public static void Normalize(Order order)
        {
            var flag1 = false;
            var flag2 = false;
            if(order.Discounts.SingleOrDefault(a=>a.Type == DiscountType.SeniorCitizen) != null)
            {
                flag1 = true;
            }

            if(order.Discounts.Any(a=>(int)a.Type == 2))
            {
                flag2 = true;
            }

            if(flag1 && flag2)
            {
                order.Discounts.RemoveAll(a=> a.Type == default(DiscountType) + 2);
            }

            order.Shipping = order.Type == OrderType.Shipped? 5M: 0M;
            order.LineItemTotal = order.Items.Sum(a=>a.Quantity * a.Price);
            order.DiscountTotal = Math.Round(order.Discounts.Sum(a=> a.PercentDiscount) * order.LineItemTotal, 2);
        }
    }
}