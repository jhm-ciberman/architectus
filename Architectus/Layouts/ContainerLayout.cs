using System.Collections.Generic;
using System.Numerics;

namespace Architectus.Layouts;

public abstract class ContainerLayout : LayoutElement
{
    public List<LayoutElement> Children { get; } = new List<LayoutElement>();

    public override void UpdateWorldTransform(LayoutElement parent)
    {
        base.UpdateWorldTransform(parent);

        foreach (var child in this.Children)
        {
            child.UpdateWorldTransform(this);
        }
    }

    public override void Imprint(HouseLot house)
    {
        foreach (var child in this.Children)
        {
            child.Imprint(house);
        }
    }
}
