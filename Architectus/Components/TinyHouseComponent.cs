using Architectus.Layouts;
using Architectus.Support;
using LifeSim.Support.Numerics;

namespace Architectus.Components;

public class TinyHouseComponent : Component
{
    public override LayoutElement Expand(RectInt bounds, HouseContext context)
    {
        return new StackLayout
        {
            Orientation = context.Random.NextOrientation(),
            Children =
            {
                new RoomElement
                {
                    MinSize = new Vector2Int(3, 3),
                    Type = RoomType.LivingRoom,
                    GrowWeight = 1f,
                },
                new RoomElement
                {
                    MinSize = new Vector2Int(3, 3),
                    Type = RoomType.Bedroom,
                    GrowWeight = 5f,
                },
            },
        };
    }
}
