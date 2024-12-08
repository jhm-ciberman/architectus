using System.Collections.Generic;
using System.Numerics;

namespace Architectus.Layouts;

/// <summary>
/// Represents a layout element that contains multiple children.
/// </summary>
public abstract class ContainerLayout : LayoutElement
{
    /// <summary>
    /// Gets the children of the container. Adding or removing children should be done through this property.
    /// </summary>
    public List<LayoutElement> Children { get; } = new List<LayoutElement>();

    /// <inheritdoc/>
    public override void UpdateWorldMatrix(Matrix3x2 parentMatrix)
    {
        base.UpdateWorldMatrix(parentMatrix);

        foreach (var child in this.Children)
        {
            child.UpdateWorldMatrix(this.WorldMatrix);
        }
    }

    /// <inheritdoc/>
    public override void Imprint(HouseLot house)
    {
        foreach (var child in this.Children)
        {
            child.Imprint(house);
        }
    }
}
