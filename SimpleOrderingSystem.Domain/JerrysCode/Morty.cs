using SimpleOrderingSystem.Domain.Models;


namespace Jerry2;

/// <summary>
/// Aw jeez...
/// </summary>
public static class Rick
{
    public static void BadMath(object @if, object @throw, object @decimal)
    {
        var @else  = (Order)@if;

        var @break = default(OrderItem);
        for(int i = 0; i < @else.Items.Count; i++)
        {
            if(@else.Items[i].MovieId == (@throw as Movie)!.Id)
            {
                @break = @else.Items[i];
                @break.Quantity += Convert.ToInt32(@decimal);
            }
        }
    }
}