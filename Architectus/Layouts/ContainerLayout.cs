using System.Collections.Generic;

namespace Architectus;

public abstract class ContainerLayout : LayoutElement
{
    public List<LayoutElement> Children { get; } = new List<LayoutElement>();

    public override void Imprint(HouseLot house)
    {
        foreach (var child in this.Children)
        {
            child.Imprint(house);
        }
    }
}
