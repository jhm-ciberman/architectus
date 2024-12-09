using Architectus.Layouts;
using LifeSim.Support.Numerics;

namespace Architectus.Components;

public abstract class Component
{
    public abstract LayoutElement Expand(RectInt bounds, HouseContext context);
}
