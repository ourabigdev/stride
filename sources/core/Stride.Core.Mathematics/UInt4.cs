// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
//
// Copyright (c) 2010-2011 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Stride.Core.Mathematics;

/// <summary>
///   Represents a four dimensional mathematical vector.
/// </summary>
[DataContract]
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct UInt4 : IEquatable<UInt4>, ISpanFormattable
{
    /// <summary>
    ///   The size of the <see cref = "UInt4" /> type, in bytes.
    /// </summary>
    public static readonly int SizeInBytes = Unsafe.SizeOf<UInt4>();

    /// <summary>
    ///   A <see cref = "UInt4" /> with all of its components set to zero.
    /// </summary>
    public static readonly UInt4 Zero = new();

    /// <summary>
    ///   The X unit <see cref = "UInt4" /> (1, 0, 0, 0).
    /// </summary>
    public static readonly UInt4 UnitX = new(1, 0, 0, 0);

    /// <summary>
    ///   The Y unit <see cref = "UInt4" /> (0, 1, 0, 0).
    /// </summary>
    public static readonly UInt4 UnitY = new(0, 1, 0, 0);

    /// <summary>
    ///   The Z unit <see cref = "UInt4" /> (0, 0, 1, 0).
    /// </summary>
    public static readonly UInt4 UnitZ = new(0, 0, 1, 0);

    /// <summary>
    ///   The W unit <see cref = "UInt4" /> (0, 0, 0, 1).
    /// </summary>
    public static readonly UInt4 UnitW = new(0, 0, 0, 1);

    /// <summary>
    ///   A <see cref = "UInt4" /> with all of its components set to one.
    /// </summary>
    public static readonly UInt4 One = new(1, 1, 1, 1);

    /// <summary>
    ///   The X component of the vector.
    /// </summary>
    public uint X;

    /// <summary>
    ///   The Y component of the vector.
    /// </summary>
    public uint Y;

    /// <summary>
    ///   The Z component of the vector.
    /// </summary>
    public uint Z;

    /// <summary>
    ///   The W component of the vector.
    /// </summary>
    public uint W;

    /// <summary>
    ///   Initializes a new instance of the <see cref = "UInt4" /> struct.
    /// </summary>
    /// <param name = "value">The value that will be assigned to all components.</param>
    public UInt4(uint value)
    {
        X = value;
        Y = value;
        Z = value;
        W = value;
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref = "UInt4" /> struct.
    /// </summary>
    /// <param name = "x">Initial value for the X component of the vector.</param>
    /// <param name = "y">Initial value for the Y component of the vector.</param>
    /// <param name = "z">Initial value for the Z component of the vector.</param>
    /// <param name = "w">Initial value for the W component of the vector.</param>
    public UInt4(uint x, uint y, uint z, uint w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref = "UInt4" /> struct.
    /// </summary>
    /// <param name = "values">The values to assign to the X, Y, Z, and W components of the vector. This must be an array with four elements.</param>
    /// <exception cref = "ArgumentNullException">Thrown when <paramref name = "values" /> is <c>null</c>.</exception>
    /// <exception cref = "ArgumentOutOfRangeException">Thrown when <paramref name = "values" /> contains more or less than four elements.</exception>
    public UInt4(uint[] values)
    {
        if (values == null)
            throw new ArgumentNullException(nameof(values));
        if (values.Length != 4)
            throw new ArgumentOutOfRangeException(nameof(values), "There must be four and only four input values for UInt4.");

        X = values[0];
        Y = values[1];
        Z = values[2];
        W = values[3];
    }

    /// <summary>
    ///   Gets or sets the component at the specified index.
    /// </summary>
    /// <value>The value of the X, Y, Z, or W component, depending on the index.</value>
    /// <param name = "index">The index of the component to access. Use 0 for the X component, 1 for the Y component, 2 for the Z component, and 3 for the W component.</param>
    /// <returns>The value of the component at the specified index.</returns>
    /// <exception cref = "System.ArgumentOutOfRangeException">Thrown when the <paramref name = "index" /> is out of the range [0, 3].</exception>
    public uint this[uint index]
    {
        get
        {
            switch (index)
            {
                case 0:
                    return X;
                case 1:
                    return Y;
                case 2:
                    return Z;
                case 3:
                    return W;
            }

            throw new ArgumentOutOfRangeException(nameof(index), "Indices for UInt4 run from 0 to 3, inclusive.");
        }

        set
        {
            switch (index)
            {
                case 0:
                    X = value;
                    break;
                case 1:
                    Y = value;
                    break;
                case 2:
                    Z = value;
                    break;
                case 3:
                    W = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index), "Indices for UInt4 run from 0 to 3, inclusive.");
            }
        }
    }

    /// <summary>
    /// Casts from System.Numerics to Stride.Maths vectors
    /// </summary>
    /// <param name="v">Value to cast</param>
    public static explicit operator UInt4(System.Numerics.Vector4 v) => new((uint)v.X, (uint)v.Y, (uint)v.Z, (uint)v.W);

    /// <summary>
    /// Casts from Stride.Maths to System.Numerics vectors
    /// </summary>
    /// <param name="v">Value to cast</param>
    public static explicit operator System.Numerics.Vector4(UInt4 v) => new(v.X, v.Y, v.Z, v.W);


    /// <summary>
    ///   Creates an array containing the elements of the vector.
    /// </summary>
    /// <returns>A four-element array containing the components of the vector.</returns>
    public uint[] ToArray()
    {
        return [X, Y, Z, W];
    }

    /// <summary>
    ///   Adds two vectors.
    /// </summary>
    /// <param name = "left">The first vector to add.</param>
    /// <param name = "right">The second vector to add.</param>
    /// <param name = "result">When the method completes, contains the sum of the two vectors.</param>
    public static void Add(ref readonly UInt4 left, ref readonly UInt4 right, out UInt4 result)
    {
        result = new UInt4(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);
    }

    /// <summary>
    ///   Adds two vectors.
    /// </summary>
    /// <param name = "left">The first vector to add.</param>
    /// <param name = "right">The second vector to add.</param>
    /// <returns>The sum of the two vectors.</returns>
    public static UInt4 Add(UInt4 left, UInt4 right)
    {
        return new UInt4(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);
    }

    /// <summary>
    ///   Subtracts two vectors.
    /// </summary>
    /// <param name = "left">The first vector to subtract.</param>
    /// <param name = "right">The second vector to subtract.</param>
    /// <param name = "result">When the method completes, contains the difference of the two vectors.</param>
    public static void Subtract(ref readonly UInt4 left, ref readonly UInt4 right, out UInt4 result)
    {
        result = new UInt4(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W);
    }

    /// <summary>
    ///   Subtracts two vectors.
    /// </summary>
    /// <param name = "left">The first vector to subtract.</param>
    /// <param name = "right">The second vector to subtract.</param>
    /// <returns>The difference of the two vectors.</returns>
    public static UInt4 Subtract(UInt4 left, UInt4 right)
    {
        return new UInt4(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W);
    }

    /// <summary>
    ///   Scales a vector by the given value.
    /// </summary>
    /// <param name = "value">The vector to scale.</param>
    /// <param name = "scale">The amount by which to scale the vector.</param>
    /// <param name = "result">When the method completes, contains the scaled vector.</param>
    public static void Multiply(ref readonly UInt4 value, uint scale, out UInt4 result)
    {
        result = new UInt4(value.X * scale, value.Y * scale, value.Z * scale, value.W * scale);
    }

    /// <summary>
    ///   Scales a vector by the given value.
    /// </summary>
    /// <param name = "value">The vector to scale.</param>
    /// <param name = "scale">The amount by which to scale the vector.</param>
    /// <returns>The scaled vector.</returns>
    public static UInt4 Multiply(UInt4 value, uint scale)
    {
        return new UInt4(value.X * scale, value.Y * scale, value.Z * scale, value.W * scale);
    }

    /// <summary>
    ///   Modulates a vector with another by performing component-wise multiplication.
    /// </summary>
    /// <param name = "left">The first vector to modulate.</param>
    /// <param name = "right">The second vector to modulate.</param>
    /// <param name = "result">When the method completes, contains the modulated vector.</param>
    public static void Modulate(ref readonly UInt4 left, ref readonly UInt4 right, out UInt4 result)
    {
        result = new UInt4(left.X * right.X, left.Y * right.Y, left.Z * right.Z, left.W * right.W);
    }

    /// <summary>
    ///   Modulates a vector with another by performing component-wise multiplication.
    /// </summary>
    /// <param name = "left">The first vector to modulate.</param>
    /// <param name = "right">The second vector to modulate.</param>
    /// <returns>The modulated vector.</returns>
    public static UInt4 Modulate(UInt4 left, UInt4 right)
    {
        return new UInt4(left.X * right.X, left.Y * right.Y, left.Z * right.Z, left.W * right.W);
    }

    /// <summary>
    ///   Scales a vector by the given value.
    /// </summary>
    /// <param name = "value">The vector to scale.</param>
    /// <param name = "scale">The amount by which to scale the vector.</param>
    /// <param name = "result">When the method completes, contains the scaled vector.</param>
    public static void Divide(ref readonly UInt4 value, uint scale, out UInt4 result)
    {
        result = new UInt4(value.X / scale, value.Y / scale, value.Z / scale, value.W / scale);
    }

    /// <summary>
    ///   Scales a vector by the given value.
    /// </summary>
    /// <param name = "value">The vector to scale.</param>
    /// <param name = "scale">The amount by which to scale the vector.</param>
    /// <returns>The scaled vector.</returns>
    public static UInt4 Divide(UInt4 value, uint scale)
    {
        return new UInt4(value.X / scale, value.Y / scale, value.Z / scale, value.W / scale);
    }

    /// <summary>
    ///   Restricts a value to be within a specified range.
    /// </summary>
    /// <param name = "value">The value to clamp.</param>
    /// <param name = "min">The minimum value.</param>
    /// <param name = "max">The maximum value.</param>
    /// <param name = "result">When the method completes, contains the clamped value.</param>
    public static void Clamp(ref readonly UInt4 value, ref readonly UInt4 min, ref readonly UInt4 max, out UInt4 result)
    {
        uint x = value.X;
        x = (x > max.X) ? max.X : x;
        x = (x < min.X) ? min.X : x;

        uint y = value.Y;
        y = (y > max.Y) ? max.Y : y;
        y = (y < min.Y) ? min.Y : y;

        uint z = value.Z;
        z = (z > max.Z) ? max.Z : z;
        z = (z < min.Z) ? min.Z : z;

        uint w = value.W;
        w = (w > max.W) ? max.W : w;
        w = (w < min.W) ? min.W : w;

        result = new UInt4(x, y, z, w);
    }

    /// <summary>
    ///   Restricts a value to be within a specified range.
    /// </summary>
    /// <param name = "value">The value to clamp.</param>
    /// <param name = "min">The minimum value.</param>
    /// <param name = "max">The maximum value.</param>
    /// <returns>The clamped value.</returns>
    public static UInt4 Clamp(UInt4 value, UInt4 min, UInt4 max)
    {
        Clamp(ref value, ref min, ref max, out var result);
        return result;
    }

    /// <summary>
    ///   Returns a vector containing the smallest components of the specified vectors.
    /// </summary>
    /// <param name = "left">The first source vector.</param>
    /// <param name = "right">The second source vector.</param>
    /// <param name = "result">When the method completes, contains an new vector composed of the largest components of the source vectors.</param>
    public static void Max(ref readonly UInt4 left, ref readonly UInt4 right, out UInt4 result)
    {
        result.X = (left.X > right.X) ? left.X : right.X;
        result.Y = (left.Y > right.Y) ? left.Y : right.Y;
        result.Z = (left.Z > right.Z) ? left.Z : right.Z;
        result.W = (left.W > right.W) ? left.W : right.W;
    }

    /// <summary>
    ///   Returns a vector containing the largest components of the specified vectors.
    /// </summary>
    /// <param name = "left">The first source vector.</param>
    /// <param name = "right">The second source vector.</param>
    /// <returns>A vector containing the largest components of the source vectors.</returns>
    public static UInt4 Max(UInt4 left, UInt4 right)
    {
        Max(ref left, ref right, out var result);
        return result;
    }

    /// <summary>
    ///   Returns a vector containing the smallest components of the specified vectors.
    /// </summary>
    /// <param name = "left">The first source vector.</param>
    /// <param name = "right">The second source vector.</param>
    /// <param name = "result">When the method completes, contains an new vector composed of the smallest components of the source vectors.</param>
    public static void Min(ref readonly UInt4 left, ref readonly UInt4 right, out UInt4 result)
    {
        result.X = (left.X < right.X) ? left.X : right.X;
        result.Y = (left.Y < right.Y) ? left.Y : right.Y;
        result.Z = (left.Z < right.Z) ? left.Z : right.Z;
        result.W = (left.W < right.W) ? left.W : right.W;
    }

    /// <summary>
    ///   Returns a vector containing the smallest components of the specified vectors.
    /// </summary>
    /// <param name = "left">The first source vector.</param>
    /// <param name = "right">The second source vector.</param>
    /// <returns>A vector containing the smallest components of the source vectors.</returns>
    public static UInt4 Min(UInt4 left, UInt4 right)
    {
        Min(ref left, ref right, out var result);
        return result;
    }

    /// <summary>
    ///   Adds two vectors.
    /// </summary>
    /// <param name = "left">The first vector to add.</param>
    /// <param name = "right">The second vector to add.</param>
    /// <returns>The sum of the two vectors.</returns>
    public static UInt4 operator +(UInt4 left, UInt4 right)
    {
        return new UInt4(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);
    }

    /// <summary>
    ///   Assert a vector (return it unchanged).
    /// </summary>
    /// <param name = "value">The vector to assert (unchange).</param>
    /// <returns>The asserted (unchanged) vector.</returns>
    public static UInt4 operator +(UInt4 value)
    {
        return value;
    }

    /// <summary>
    ///   Subtracts two vectors.
    /// </summary>
    /// <param name = "left">The first vector to subtract.</param>
    /// <param name = "right">The second vector to subtract.</param>
    /// <returns>The difference of the two vectors.</returns>
    public static UInt4 operator -(UInt4 left, UInt4 right)
    {
        return new UInt4(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W);
    }

    /// <summary>
    ///   Scales a vector by the given value.
    /// </summary>
    /// <param name = "value">The vector to scale.</param>
    /// <param name = "scale">The amount by which to scale the vector.</param>
    /// <returns>The scaled vector.</returns>
    public static UInt4 operator *(uint scale, UInt4 value)
    {
        return new UInt4(value.X * scale, value.Y * scale, value.Z * scale, value.W * scale);
    }

    /// <summary>
    ///   Scales a vector by the given value.
    /// </summary>
    /// <param name = "value">The vector to scale.</param>
    /// <param name = "scale">The amount by which to scale the vector.</param>
    /// <returns>The scaled vector.</returns>
    public static UInt4 operator *(UInt4 value, uint scale)
    {
        return new UInt4(value.X * scale, value.Y * scale, value.Z * scale, value.W * scale);
    }

    /// <summary>
    ///   Scales a vector by the given value.
    /// </summary>
    /// <param name = "value">The vector to scale.</param>
    /// <param name = "scale">The amount by which to scale the vector.</param>
    /// <returns>The scaled vector.</returns>
    public static UInt4 operator /(UInt4 value, uint scale)
    {
        return new UInt4(value.X / scale, value.Y / scale, value.Z / scale, value.W / scale);
    }

    /// <summary>
    ///   Tests for equality between two objects.
    /// </summary>
    /// <param name = "left">The first value to compare.</param>
    /// <param name = "right">The second value to compare.</param>
    /// <returns><c>true</c> if <paramref name = "left" /> has the same value as <paramref name = "right" />; otherwise, <c>false</c>.</returns>
    public static bool operator ==(UInt4 left, UInt4 right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///   Tests for inequality between two objects.
    /// </summary>
    /// <param name = "left">The first value to compare.</param>
    /// <param name = "right">The second value to compare.</param>
    /// <returns><c>true</c> if <paramref name = "left" /> has a different value than <paramref name = "right" />; otherwise, <c>false</c>.</returns>
    public static bool operator !=(UInt4 left, UInt4 right)
    {
        return !left.Equals(right);
    }

    /// <summary>
    ///   Performs an explicit conversion from <see cref = "UInt4" /> to <see cref = "Vector2" />.
    /// </summary>
    /// <param name = "value">The value.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator Vector2(UInt4 value)
    {
        return new Vector2(value.X, value.Y);
    }

    /// <summary>
    ///   Performs an explicit conversion from <see cref = "UInt4" /> to <see cref = "Vector3" />.
    /// </summary>
    /// <param name = "value">The value.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator Vector3(UInt4 value)
    {
        return new Vector3(value.X, value.Y, value.Z);
    }

    /// <summary>
    ///   Performs an explicit conversion from <see cref = "UInt4" /> to <see cref = "Vector4" />.
    /// </summary>
    /// <param name = "value">The value.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator Vector4(UInt4 value)
    {
        return new Vector4(value.X, value.Y, value.Z, value.W);
    }

    /// <summary>
    /// Returns a <see cref="string"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="string"/> that represents this instance.
    /// </returns>
    public override readonly string ToString() => $"{this}";

    /// <summary>
    /// Returns a <see cref="string"/> that represents this instance.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <returns>
    /// A <see cref="string"/> that represents this instance.
    /// </returns>
    public readonly string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format, IFormatProvider? formatProvider)
    {
        var handler = new DefaultInterpolatedStringHandler(11, 4, formatProvider);
        handler.AppendLiteral("X:");
        handler.AppendFormatted(X, format);
        handler.AppendLiteral(" Y:");
        handler.AppendFormatted(Y, format);
        handler.AppendLiteral(" Z:");
        handler.AppendFormatted(Z, format);
        handler.AppendLiteral(" W:");
        handler.AppendFormatted(W, format);
        return handler.ToStringAndClear();
    }

    bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        var format1 = format.Length > 0 ? format.ToString() : null;
        var handler = new MemoryExtensions.TryWriteInterpolatedStringHandler(11, 4, destination, provider, out _);
        handler.AppendLiteral("X:");
        handler.AppendFormatted(X, format1);
        handler.AppendLiteral(" Y:");
        handler.AppendFormatted(Y, format1);
        handler.AppendLiteral(" Z:");
        handler.AppendFormatted(Z, format1);
        handler.AppendLiteral(" W:");
        handler.AppendFormatted(W, format1);
        return destination.TryWrite(ref handler, out charsWritten);
    }

    /// <summary>
    ///   Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    ///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
    /// </returns>
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z, W);
    }

    /// <summary>
    ///   Determines whether the specified <see cref = "UInt4" /> is equal to this instance.
    /// </summary>
    /// <param name = "other">The <see cref = "UInt4" /> to compare with this instance.</param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref = "UInt4" /> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public readonly bool Equals(UInt4 other)
    {
        return other.X == X && other.Y == Y && other.Z == Z && other.W == W;
    }

    /// <summary>
    ///   Determines whether the specified <see cref = "object" /> is equal to this instance.
    /// </summary>
    /// <param name = "value">The <see cref = "object" /> to compare with this instance.</param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref = "object" /> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public override readonly bool Equals(object? value)
    {
        return value is UInt4 u && Equals(u);
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="uint"/> array to <see cref="Stride.Core.Mathematics.UInt4"/>.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator UInt4(uint[] input)
    {
        return new UInt4(input);
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="Stride.Core.Mathematics.UInt4"/> to <see cref="int"/> array.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator uint[](UInt4 input)
    {
        return input.ToArray();
    }

    /// <summary>
    /// Deconstructs the vector's components into named variables.
    /// </summary>
    /// <param name="x">The X component</param>
    /// <param name="y">The Y component</param>
    /// <param name="z">The Z component</param>
    /// <param name="w">The W component</param>
    public readonly void Deconstruct(out uint x, out uint y, out uint z, out uint w)
    {
        x = X;
        y = Y;
        z = Z;
        w = W;
    }
}
