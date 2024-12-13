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
            Orientation = context.Random.NextOrientationLargeSide(bounds.Size),
            Children =
            {
                new RoomElement
                {
                    Type = RoomType.LivingRoom,
                    GrowWeight = context.Random.NextSingle(1f, 3f),
                },
                new StackLayout
                {
                    Orientation = context.Random.NextOrientationSmallSide(bounds.Size),
                    GrowWeight = context.Random.NextSingle(1f, 3f),
                    Children =
                    {

                        new RoomElement
                        {
                            Type = RoomType.Bedroom,
                            GrowWeight = context.Random.NextSingle(1f, 6f),
                        },
                        new RoomElement
                        {
                            Type = RoomType.Kitchen,
                            GrowWeight = context.Random.NextSingle(1f, 6f),
                        },
                    },
                }
            },
        };
    }
}
