using System.Reflection.Emit;
using Jerry;
using SimpleOrderingSystem.Domain.Models;

namespace Jerry2
{
    public class Class2
    {
        public BigPapa Handle = default!;
        private const string Toplevel = "20";
        private static List<Tuple<int,decimal>> Magic1 = new List<Tuple<int, decimal>>
        {
            new(1945, 2M),
            new(1970, 6M),
            new(2000, 12M),
            new(2010, 15M),
            new(2020, 20M),
        };

        public void CheckIt(Order? order, OrderItem[] orderItems, string? CodeSection)
        {
            if(CodeSection == "Q")
            {
                goto Label;
            }
            if(CodeSection == "W")
            {
                goto Label2;
            }

Label:
            int total = 0;
            for(int i = 0; i < orderItems.Count(); i++)
            {
                total = orderItems[i].Quantity + total;
            }
            if(total > Convert.ToUInt16(Toplevel))
            {
                throw new SimpleOrderingSystemException(SimpleOrderingSystemErrorType.InvalidRequest,
                    "Unable to add items because that would exceed the maximum movies for a single order. An order can have up to 20 items.");
            }

            if(CodeSection is not null) return;

            foreach(OrderItem item in orderItems)
            {
                BigPapa.calculator.PriceItem(item);
            }
            
Label2:
            decimal a = Decimal.Zero;
            for(int i = 0; i<orderItems.Length;i++)
            {
                a = (orderItems[i].Price * orderItems[i].Quantity) + a;
            }

            Stack<object> stack = new Stack<object>();
            if(a > BigPapa.Magic1 * 20)
            {
                stack.Push(new OrderDiscount { Type = DiscountType.LargeOrder, PercentDiscount = .1M});
            }

            if(order!.CreatedDateTimeUtc.Date.Year - order.Customer.DateOfBirth.Year >= 65)
            {
                stack.Push(new OrderDiscount { Type = DiscountType.SeniorCitizen, PercentDiscount = .15M});
            }

            order.Discounts.Clear();
            BigPapa.Method1(order);

            while(stack.Count > 0)
            {
                order.Discounts.Add((stack.Pop() as OrderDiscount)!);
            }

            if(CodeSection is not null) goto Exit;
            
            BigPapa.DoIt(order, null, string.Empty, true);
Exit:
            return;
        }

        public void DiscountCheck(OrderItem[] orderItems)
        {
            this.CheckIt(null, orderItems, "Q");
        }

        public void PriceItem(OrderItem item)
        {

            try
            { 
                  var a = Int32.Parse(item.MovieYear!);
                  var b = Decimal.Parse(item.MovieMetascore!)/100M;

                  for(int i = 0; i < Magic1.Count; i++)
                  {
                    if(Magic1[i].Item1 >= a)
                    {
                        item.Price = Math.Round(Magic1[i].Item2 * b, 2);
                        return;
                    }
                  }
            }
            catch
            {
                item.Price = BigPapa.Magic1;
            }
        }
    }
}