using System;
using System.Collections.Generic;
using System.Linq;
using LifeSim.Support.Numerics;

namespace Architectus.Layouts;

public enum Orientation
{
    /// <summary>
    /// The children are arranged in a row, from left to right.
    /// </summary>
    Row,

    /// <summary>
    /// The children are arranged in a column, from top to bottom.
    /// </summary>
    Column,

    /// <summary>
    /// The children are arranged in a row, from right to left.
    /// </summary>
    RowReverse,

    /// <summary>
    /// The children are arranged in a column, from bottom to top.
    /// </summary>
    ColumnReverse,
}

public class StackLayout : ContainerLayout
{
    /// <summary>
    /// Gets or sets the orientation of the layout.
    /// </summary>
    public Orientation Orientation { get; set; } = Orientation.Row;

    /// <summary>
    /// Gets or sets whether the last child should fill the remaining space. If false, the last child will be
    /// arranged with its desired size.
    /// </summary>
    public bool LastChildFill { get; set; } = true;
}
