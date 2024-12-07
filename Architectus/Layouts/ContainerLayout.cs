using System.Collections.Generic;
using System.Numerics;

namespace Architectus.Layouts;

public abstract class ContainerLayout : LayoutElement
{
    public List<LayoutElement> Children { get; } = new List<LayoutElement>();

    public override void UpdateWorldMatrix(Matrix3x2 parentMatrix)
    {
        base.UpdateWorldMatrix(parentMatrix);

        foreach (var child in this.Children)
        {
            child.UpdateWorldMatrix(this.WorldMatrix);
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
