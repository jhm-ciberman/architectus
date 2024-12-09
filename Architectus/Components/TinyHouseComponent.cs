using Architectus.Layouts;
using LifeSim.Support.Numerics;

namespace Architectus.Components;

public class TinyHouseComponent : Component
{
    public override LayoutElement Expand(RectInt bounds, HouseContext context)
    {
        return new StackLayout
        {
            Orientation = context.RandomOrientation(),
            Children =
            {
                new RoomElement { MinSize = new Vector2Int(3, 3), Type = RoomType.LivingRoom },
                new RoomElement { MinSize = new Vector2Int(3, 3), Type = RoomType.Bedroom },
            },
        };
    }
}
