using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Architectus;

/// <summary>
/// Represents a 2D vector with integer components.
/// </summary>
public struct Vector2Int : IEquatable<Vector2Int>
{
    /// <summary>
    /// The X component of the vector.
    /// </summary>
    public int X;

    /// <summary>
    /// The Y component of the vector.
    /// </summary>
    public int Y;

    public static Vector2Int One => new Vector2Int(1, 1);
    public static Vector2Int Zero => new Vector2Int(0, 0);

    /// <summary>
    /// Initializes a new instance of the <see cref="Vector2Int"/> struct.
    /// </summary>
    /// <param name="x">The X component of the vector.</param>
    /// <param name="y">The Y component of the vector.</param>
    public Vector2Int(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Vector2Int"/> struct.
    /// </summary>
    /// <param name="x">The X component of the vector.</param>
    /// <param name="y">The Y component of the vector.</param>
    public Vector2Int(uint x, uint y)
    {
        this.X = (int)x;
        this.Y = (int)y;
    }

    /// <summary>
    /// Returns a vector whose elements are the maximum of each of the pairs of elements in two specified vectors.
    /// </summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The vector containing the maximum values from both vectors.</returns>
    public static Vector2Int Max(Vector2Int value1, Vector2Int value2)
    {
        return new Vector2Int(Math.Max(value1.X, value2.X), Math.Max(value1.Y, value2.Y));
    }

    /// <summary>
    /// Returns a vector whose elements are the minimum of each of the pairs of elements in two specified vectors.
    /// </summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The vector containing the minimum values from both vectors.</returns>
    public static Vector2Int Min(Vector2Int value1, Vector2Int value2)
    {
        return new Vector2Int(Math.Min(value1.X, value2.X), Math.Min(value1.Y, value2.Y));
    }

    /// <summary>
    /// Clamps the vector to the given minimum and maximum vectors.
    /// </summary>
    /// <param name="value">The vector to clamp.</param>
    /// <param name="min">The minimum vector.</param>
    /// <param name="max">The maximum vector.</param>
    /// <returns>The clamped vector.</returns>
    public static Vector2Int Clamp(Vector2Int value, Vector2Int min, Vector2Int max)
    {
        return new Vector2Int(
            Math.Min(max.X, Math.Max(min.X, value.X)),
            Math.Min(max.Y, Math.Max(min.Y, value.Y)));
    }

    /// <summary>
    /// Computes the manhattan distance between two vectors.
    /// </summary>
    /// <param name="a">The first vector.</param>
    /// <param name="b">The second vector.</param>
    /// <returns>The manhattan distance between the two vectors.</returns>
    public static float ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2Int operator +(Vector2Int a, Vector2Int b)
    {
        return new Vector2Int(a.X + b.X, a.Y + b.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2Int operator -(Vector2Int a, Vector2Int b)
    {
        return new Vector2Int(a.X - b.X, a.Y - b.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2Int operator *(Vector2Int a, Vector2Int b)
    {
        return new Vector2Int(a.X * b.X, a.Y * b.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2Int operator /(Vector2Int a, Vector2Int b)
    {
        return new Vector2Int(a.X / b.X, a.Y / b.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2Int operator *(Vector2Int a, int b)
    {
        return new Vector2Int(a.X * b, a.Y * b);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2Int operator /(Vector2Int a, int b)
    {
        return new Vector2Int(a.X / b, a.Y / b);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2Int operator *(Vector2Int a, uint b)
    {
        return new Vector2Int(a.X * (int)b, a.Y * (int)b);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2Int operator /(Vector2Int a, uint b)
    {
        return new Vector2Int(a.X / (int)b, a.Y / (int)b);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Vector2Int lhs, Vector2Int rhs)
    {
        return lhs.X == rhs.X && lhs.Y == rhs.Y;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Vector2Int lhs, Vector2Int rhs)
    {
        return !(lhs == rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Vector2(Vector2Int a)
    {
        return new Vector2(a.X, a.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals([AllowNull] object obj)
    {
        return obj is Vector2Int vector && this.Equals(vector);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Vector2Int other)
    {
        return this.X == other.X && this.Y == other.Y;
    }

    public override int GetHashCode()
    {
        return this.X.GetHashCode() ^ (this.Y.GetHashCode() << 16);
    }

    public override string? ToString()
    {
        return "(" + this.X + ", " + this.Y + ")";
    }
}