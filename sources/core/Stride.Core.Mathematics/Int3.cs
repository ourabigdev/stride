// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
//
// -----------------------------------------------------------------------------
// Original code from SlimMath project. http://code.google.com/p/slimmath/
// Greetings to SlimDX Group. Original code published with the following license:
// -----------------------------------------------------------------------------
/*
* Copyright (c) 2007-2011 SlimDX Group
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Stride.Core.Mathematics;

/// <summary>
/// Represents a three dimensional mathematical vector.
/// </summary>
[DataContract("Int3")]
[DataStyle(DataStyle.Compact)]
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct Int3 : IEquatable<Int3>, ISpanFormattable
{
    /// <summary>
    /// The size of the <see cref="Int3"/> type, in bytes.
    /// </summary>
    public static readonly int SizeInBytes = Unsafe.SizeOf<Int3>();

    /// <summary>
    /// A <see cref="Int3"/> with all of its components set to zero.
    /// </summary>
    public static readonly Int3 Zero = new();

    /// <summary>
    /// The X unit <see cref="Int3"/> (1, 0, 0).
    /// </summary>
    public static readonly Int3 UnitX = new(1, 0, 0);

    /// <summary>
    /// The Y unit <see cref="Int3"/> (0, 1, 0).
    /// </summary>
    public static readonly Int3 UnitY = new(0, 1, 0);

    /// <summary>
    /// The Z unit <see cref="Int3"/> (0, 0, 1).
    /// </summary>
    public static readonly Int3 UnitZ = new(0, 0, 1);

    /// <summary>
    /// A <see cref="Int3"/> with all of its components set to one.
    /// </summary>
    public static readonly Int3 One = new(1, 1, 1);

    /// <summary>
    /// The X component of the vector.
    /// </summary>
    [DataMember(0)]
    public int X;

    /// <summary>
    /// The Y component of the vector.
    /// </summary>
    [DataMember(1)]
    public int Y;

    /// <summary>
    /// The Z component of the vector.
    /// </summary>
    [DataMember(2)]
    public int Z;

    /// <summary>
    /// Initializes a new instance of the <see cref="Int3"/> struct.
    /// </summary>
    /// <param name="value">The value that will be assigned to all components.</param>
    public Int3(int value)
    {
        X = value;
        Y = value;
        Z = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Int3"/> struct.
    /// </summary>
    /// <param name="x">Initial value for the X component of the vector.</param>
    /// <param name="y">Initial value for the Y component of the vector.</param>
    /// <param name="z">Initial value for the Z component of the vector.</param>
    public Int3(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Int3"/> struct.
    /// </summary>
    /// <param name="value">A vector containing the values with which to initialize the X and Y components.</param>
    /// <param name="z">Initial value for the Z component of the vector.</param>
    public Int3(Vector2 value, int z)
    {
        X = (int)value.X;
        Y = (int)value.Y;
        Z = z;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Int3"/> struct.
    /// </summary>
    /// <param name="values">The values to assign to the X, Y, and Z components of the vector. This must be an array with three elements.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="values"/> contains more or less than three elements.</exception>
    public Int3(int[] values)
    {
        ArgumentNullException.ThrowIfNull(values);
        if (values.Length != 3)
            throw new ArgumentOutOfRangeException(nameof(values), "There must be three and only three input values for Int3.");

        X = values[0];
        Y = values[1];
        Z = values[2];
    }

    /// <summary>
    /// Gets or sets the component at the specified index.
    /// </summary>
    /// <value>The value of the X, Y, or Z component, depending on the index.</value>
    /// <param name="index">The index of the component to access. Use 0 for the X component, 1 for the Y component, and 2 for the Z component.</param>
    /// <returns>The value of the component at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 2].</exception>
    public int this[int index]
    {
        readonly get
        {
            return index switch
            {
                0 => X,
                1 => Y,
                2 => Z,
                _ => throw new ArgumentOutOfRangeException(nameof(index), "Indices for Int3 run from 0 to 2, inclusive."),
            };
        }

        set
        {
            switch (index)
            {
                case 0: X = value; break;
                case 1: Y = value; break;
                case 2: Z = value; break;
                default: throw new ArgumentOutOfRangeException(nameof(index), "Indices for Int3 run from 0 to 2, inclusive.");
            }
        }
    }

    /// <summary>
    /// Casts from System.Numerics to Stride.Maths vectors
    /// </summary>
    /// <param name="v">Value to cast</param>
    public static explicit operator Int3(System.Numerics.Vector3 v) => new((int)v.X, (int)v.Y, (int)v.Z);

    /// <summary>
    /// Casts from Stride.Maths to System.Numerics vectors
    /// </summary>
    /// <param name="v">Value to cast</param>
    public static explicit operator System.Numerics.Vector3(Int3 v) => new(v.X, v.Y, v.Z);

    /// <summary>
    /// Calculates the length of the vector.
    /// </summary>
    /// <returns>The length of the vector.</returns>
    /// <remarks>
    /// <see cref="LengthSquared"/> may be preferred when only the relative length is needed
    /// and speed is of the essence.
    /// </remarks>
    public readonly int Length()
    {
        return (int)MathF.Sqrt((X * X) + (Y * Y) + (Z * Z));
    }

    /// <summary>
    /// Calculates the untruncated length of the vector.
    /// </summary>
    /// <returns>The length of the vector untruncated.</returns>
    public readonly float LengthUntruncated()
    {
        return (float)MathF.Sqrt((X * X) + (Y * Y) + (Z * Z));
    }

    /// <summary>
    /// Calculates the squared length of the vector.
    /// </summary>
    /// <returns>The squared length of the vector.</returns>
    /// <remarks>
    /// This method may be preferred to <see cref="Length"/> when only a relative length is needed
    /// and speed is of the essence.
    /// </remarks>
    public readonly int LengthSquared()
    {
        return (X * X) + (Y * Y) + (Z * Z);
    }

    /// <summary>
    /// Raises the exponent for each components.
    /// </summary>
    /// <param name="exponent">The exponent.</param>
    public void Pow(int exponent)
    {
        X = (int)Math.Pow(X, exponent);
        Y = (int)Math.Pow(Y, exponent);
        Z = (int)Math.Pow(Z, exponent);
    }

    /// <summary>
    /// Creates an array containing the elements of the vector.
    /// </summary>
    /// <returns>A three-element array containing the components of the vector.</returns>
    public readonly int[] ToArray()
    {
        return [X, Y, Z];
    }

    /// <summary>
    /// Adds two vectors.
    /// </summary>
    /// <param name="left">The first vector to add.</param>
    /// <param name="right">The second vector to add.</param>
    /// <param name="result">When the method completes, contains the sum of the two vectors.</param>
    public static void Add(ref readonly Int3 left, ref readonly Int3 right, out Int3 result)
    {
        result = new Int3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
    }

    /// <summary>
    /// Adds two vectors.
    /// </summary>
    /// <param name="left">The first vector to add.</param>
    /// <param name="right">The second vector to add.</param>
    /// <returns>The sum of the two vectors.</returns>
    public static Int3 Add(Int3 left, Int3 right)
    {
        return new Int3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
    }

    /// <summary>
    /// Subtracts two vectors.
    /// </summary>
    /// <param name="left">The first vector to subtract.</param>
    /// <param name="right">The second vector to subtract.</param>
    /// <param name="result">When the method completes, contains the difference of the two vectors.</param>
    public static void Subtract(ref readonly Int3 left, ref readonly Int3 right, out Int3 result)
    {
        result = new Int3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
    }

    /// <summary>
    /// Subtracts two vectors.
    /// </summary>
    /// <param name="left">The first vector to subtract.</param>
    /// <param name="right">The second vector to subtract.</param>
    /// <returns>The difference of the two vectors.</returns>
    public static Int3 Subtract(Int3 left, Int3 right)
    {
        return new Int3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
    }

    /// <summary>
    /// Scales a vector by the given value.
    /// </summary>
    /// <param name="value">The vector to scale.</param>
    /// <param name="scale">The amount by which to scale the vector.</param>
    /// <param name="result">When the method completes, contains the scaled vector.</param>
    public static void Multiply(ref readonly Int3 value, int scale, out Int3 result)
    {
        result = new Int3(value.X * scale, value.Y * scale, value.Z * scale);
    }

    /// <summary>
    /// Scales a vector by the given value.
    /// </summary>
    /// <param name="value">The vector to scale.</param>
    /// <param name="scale">The amount by which to scale the vector.</param>
    /// <returns>The scaled vector.</returns>
    public static Int3 Multiply(Int3 value, int scale)
    {
        return new Int3(value.X * scale, value.Y * scale, value.Z * scale);
    }

    /// <summary>
    /// Modulates a vector with another by performing component-wise multiplication.
    /// </summary>
    /// <param name="left">The first vector to modulate.</param>
    /// <param name="right">The second vector to modulate.</param>
    /// <param name="result">When the method completes, contains the modulated vector.</param>
    public static void Modulate(ref readonly Int3 left, ref readonly Int3 right, out Int3 result)
    {
        result = new Int3(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
    }

    /// <summary>
    /// Modulates a vector with another by performing component-wise multiplication.
    /// </summary>
    /// <param name="left">The first vector to modulate.</param>
    /// <param name="right">The second vector to modulate.</param>
    /// <returns>The modulated vector.</returns>
    public static Int3 Modulate(Int3 left, Int3 right)
    {
        return new Int3(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
    }

    /// <summary>
    /// Scales a vector by the given value.
    /// </summary>
    /// <param name="value">The vector to scale.</param>
    /// <param name="scale">The amount by which to scale the vector.</param>
    /// <param name="result">When the method completes, contains the scaled vector.</param>
    public static void Divide(ref readonly Int3 value, int scale, out Int3 result)
    {
        result = new Int3(value.X / scale, value.Y / scale, value.Z / scale);
    }

    /// <summary>
    /// Scales a vector by the given value.
    /// </summary>
    /// <param name="value">The vector to scale.</param>
    /// <param name="scale">The amount by which to scale the vector.</param>
    /// <returns>The scaled vector.</returns>
    public static Int3 Divide(Int3 value, int scale)
    {
        return new Int3(value.X / scale, value.Y / scale, value.Z / scale);
    }

    /// <summary>
    /// Reverses the direction of a given vector.
    /// </summary>
    /// <param name="value">The vector to negate.</param>
    /// <param name="result">When the method completes, contains a vector facing in the opposite direction.</param>
    public static void Negate(ref readonly Int3 value, out Int3 result)
    {
        result = new Int3(-value.X, -value.Y, -value.Z);
    }

    /// <summary>
    /// Reverses the direction of a given vector.
    /// </summary>
    /// <param name="value">The vector to negate.</param>
    /// <returns>A vector facing in the opposite direction.</returns>
    public static Int3 Negate(Int3 value)
    {
        return new Int3(-value.X, -value.Y, -value.Z);
    }

    /// <summary>
    /// Restricts a value to be within a specified range.
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <param name="result">When the method completes, contains the clamped value.</param>
    public static void Clamp(ref readonly Int3 value, ref readonly Int3 min, ref readonly Int3 max, out Int3 result)
    {
        int x = value.X;
        x = (x > max.X) ? max.X : x;
        x = (x < min.X) ? min.X : x;

        int y = value.Y;
        y = (y > max.Y) ? max.Y : y;
        y = (y < min.Y) ? min.Y : y;

        int z = value.Z;
        z = (z > max.Z) ? max.Z : z;
        z = (z < min.Z) ? min.Z : z;

        result = new Int3(x, y, z);
    }

    /// <summary>
    /// Restricts a value to be within a specified range.
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <returns>The clamped value.</returns>
    public static Int3 Clamp(Int3 value, Int3 min, Int3 max)
    {
        Clamp(ref value, ref min, ref max, out var result);
        return result;
    }

    /// <summary>
    /// Calculates the dot product of two vectors.
    /// </summary>
    /// <param name="left">First source vector.</param>
    /// <param name="right">Second source vector.</param>
    /// <param name="result">When the method completes, contains the dot product of the two vectors.</param>
    public static void Dot(ref readonly Int3 left, ref readonly Int3 right, out int result)
    {
        result = (left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z);
    }

    /// <summary>
    /// Calculates the dot product of two vectors.
    /// </summary>
    /// <param name="left">First source vector.</param>
    /// <param name="right">Second source vector.</param>
    /// <returns>The dot product of the two vectors.</returns>
    public static int Dot(Int3 left, Int3 right)
    {
        return (left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z);
    }

    /// <summary>
    /// Performs a linear interpolation between two vectors.
    /// </summary>
    /// <param name="start">Start vector.</param>
    /// <param name="end">End vector.</param>
    /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
    /// <param name="result">When the method completes, contains the linear interpolation of the two vectors.</param>
    /// <remarks>
    /// This method performs the linear interpolation based on the following formula.
    /// <c>start + (end - start) * amount</c>
    /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned.
    /// </remarks>
    public static void Lerp(ref readonly Int3 start, ref readonly Int3 end, float amount, out Int3 result)
    {
        result.X = (int)(start.X + ((end.X - start.X) * amount));
        result.Y = (int)(start.Y + ((end.Y - start.Y) * amount));
        result.Z = (int)(start.Z + ((end.Z - start.Z) * amount));
    }

    /// <summary>
    /// Performs a linear interpolation between two vectors.
    /// </summary>
    /// <param name="start">Start vector.</param>
    /// <param name="end">End vector.</param>
    /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
    /// <returns>The linear interpolation of the two vectors.</returns>
    /// <remarks>
    /// This method performs the linear interpolation based on the following formula.
    /// <c>start + (end - start) * amount</c>
    /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned.
    /// </remarks>
    public static Int3 Lerp(Int3 start, Int3 end, float amount)
    {
        Lerp(ref start, ref end, amount, out var result);
        return result;
    }

    /// <summary>
    /// Performs a cubic interpolation between two vectors.
    /// </summary>
    /// <param name="start">Start vector.</param>
    /// <param name="end">End vector.</param>
    /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
    /// <param name="result">When the method completes, contains the cubic interpolation of the two vectors.</param>
    public static void SmoothStep(ref readonly Int3 start, ref readonly Int3 end, float amount, out Int3 result)
    {
        amount = (amount > 1) ? 1 : ((amount < 0) ? 0 : amount);
        amount = amount * amount * (3 - (2 * amount));

        result.X = (int)(start.X + ((end.X - start.X) * amount));
        result.Y = (int)(start.Y + ((end.Y - start.Y) * amount));
        result.Z = (int)(start.Z + ((end.Z - start.Z) * amount));
    }

    /// <summary>
    /// Performs a cubic interpolation between two vectors.
    /// </summary>
    /// <param name="start">Start vector.</param>
    /// <param name="end">End vector.</param>
    /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
    /// <returns>The cubic interpolation of the two vectors.</returns>
    public static Int3 SmoothStep(Int3 start, Int3 end, float amount)
    {
        SmoothStep(ref start, ref end, amount, out var result);
        return result;
    }

    /// <summary>
    /// Returns a vector containing the smallest components of the specified vectors.
    /// </summary>
    /// <param name="left">The first source vector.</param>
    /// <param name="right">The second source vector.</param>
    /// <param name="result">When the method completes, contains an new vector composed of the largest components of the source vectors.</param>
    public static void Max(ref readonly Int3 left, ref readonly Int3 right, out Int3 result)
    {
        result.X = (left.X > right.X) ? left.X : right.X;
        result.Y = (left.Y > right.Y) ? left.Y : right.Y;
        result.Z = (left.Z > right.Z) ? left.Z : right.Z;
    }

    /// <summary>
    /// Returns a vector containing the largest components of the specified vectors.
    /// </summary>
    /// <param name="left">The first source vector.</param>
    /// <param name="right">The second source vector.</param>
    /// <returns>A vector containing the largest components of the source vectors.</returns>
    public static Int3 Max(Int3 left, Int3 right)
    {
        Max(ref left, ref right, out var result);
        return result;
    }

    /// <summary>
    /// Returns a vector containing the smallest components of the specified vectors.
    /// </summary>
    /// <param name="left">The first source vector.</param>
    /// <param name="right">The second source vector.</param>
    /// <param name="result">When the method completes, contains an new vector composed of the smallest components of the source vectors.</param>
    public static void Min(ref readonly Int3 left, ref readonly Int3 right, out Int3 result)
    {
        result.X = (left.X < right.X) ? left.X : right.X;
        result.Y = (left.Y < right.Y) ? left.Y : right.Y;
        result.Z = (left.Z < right.Z) ? left.Z : right.Z;
    }

    /// <summary>
    /// Returns a vector containing the smallest components of the specified vectors.
    /// </summary>
    /// <param name="left">The first source vector.</param>
    /// <param name="right">The second source vector.</param>
    /// <returns>A vector containing the smallest components of the source vectors.</returns>
    public static Int3 Min(Int3 left, Int3 right)
    {
        Min(ref left, ref right, out var result);
        return result;
    }

    /// <summary>
    /// Returns a vector containing the rounded values of the specified vector.
    /// </summary>
    /// <param name="value">The source vector.</param>
    /// <param name="result">When the method completes, contains an new vector composed of the rounded values of the specified vector.</param>
    /// <param name="rounding">The rounding strategy to use.</param>
    public static void Round(in Vector3 value, out Int3 result, MidpointRounding rounding = default)
    {
        result.X = (int)MathF.Round(value.X, rounding);
        result.Y = (int)MathF.Round(value.Y, rounding);
        result.Z = (int)MathF.Round(value.Z, rounding);
    }

    /// <summary>
    /// Returns a vector containing the rounded values of the specified vector.
    /// </summary>
    /// <param name="value">The source vector.</param>
    /// <param name="rounding">The rounding strategy to use.</param>
    /// <returns>A vector containing the rounded values of the source vector.</returns>
    public static Int3 Round(in Vector3 value, MidpointRounding rounding = default)
    {
        Round(in value, out var result, rounding);
        return result;
    }

    /// <summary>
    /// Adds two vectors.
    /// </summary>
    /// <param name="left">The first vector to add.</param>
    /// <param name="right">The second vector to add.</param>
    /// <returns>The sum of the two vectors.</returns>
    public static Int3 operator +(Int3 left, Int3 right)
    {
        return new Int3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
    }

    /// <summary>
    /// Assert a vector (return it unchanged).
    /// </summary>
    /// <param name="value">The vector to assert (unchange).</param>
    /// <returns>The asserted (unchanged) vector.</returns>
    public static Int3 operator +(Int3 value)
    {
        return value;
    }

    /// <summary>
    /// Subtracts two vectors.
    /// </summary>
    /// <param name="left">The first vector to subtract.</param>
    /// <param name="right">The second vector to subtract.</param>
    /// <returns>The difference of the two vectors.</returns>
    public static Int3 operator -(Int3 left, Int3 right)
    {
        return new Int3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
    }

    /// <summary>
    /// Reverses the direction of a given vector.
    /// </summary>
    /// <param name="value">The vector to negate.</param>
    /// <returns>A vector facing in the opposite direction.</returns>
    public static Int3 operator -(Int3 value)
    {
        return new Int3(-value.X, -value.Y, -value.Z);
    }

    /// <summary>
    /// Scales a vector by the given value.
    /// </summary>
    /// <param name="scale">The amount by which to scale the vector.</param>
    /// <param name="value">The vector to scale.</param>
    /// <returns>The scaled vector.</returns>
    public static Int3 operator *(float scale, Int3 value)
    {
        return new Int3((int)(value.X * scale), (int)(value.Y * scale), (int)(value.Z * scale));
    }

    /// <summary>
    /// Scales a vector by the given value.
    /// </summary>
    /// <param name="value">The vector to scale.</param>
    /// <param name="scale">The amount by which to scale the vector.</param>
    /// <returns>The scaled vector.</returns>
    public static Int3 operator *(Int3 value, float scale)
    {
        return new Int3((int)(value.X * scale), (int)(value.Y * scale), (int)(value.Z * scale));
    }

    /// <summary>
    /// Scales a vector by the given value.
    /// </summary>
    /// <param name="value">The vector to scale.</param>
    /// <param name="scale">The amount by which to scale the vector.</param>
    /// <returns>The scaled vector.</returns>
    public static Int3 operator /(Int3 value, float scale)
    {
        return new Int3((int)(value.X / scale), (int)(value.Y / scale), (int)(value.Z / scale));
    }

    /// <summary>
    /// Tests for equality between two objects.
    /// </summary>
    /// <param name="left">The first value to compare.</param>
    /// <param name="right">The second value to compare.</param>
    /// <returns><c>true</c> if <paramref name="left"/> has the same value as <paramref name="right"/>; otherwise, <c>false</c>.</returns>
    public static bool operator ==(Int3 left, Int3 right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Tests for inequality between two objects.
    /// </summary>
    /// <param name="left">The first value to compare.</param>
    /// <param name="right">The second value to compare.</param>
    /// <returns><c>true</c> if <paramref name="left"/> has a different value than <paramref name="right"/>; otherwise, <c>false</c>.</returns>
    public static bool operator !=(Int3 left, Int3 right)
    {
        return !left.Equals(right);
    }

    /// <summary>
    /// Performs an explicit conversion from <see cref="Int3"/> to <see cref="Vector2"/>.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator Vector2(Int3 value)
    {
        return new Vector2(value.X, value.Y);
    }

    /// <summary>
    /// Performs an explicit conversion from <see cref="Int3"/> to <see cref="Vector3"/>.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator Vector3(Int3 value)
    {
        return new Vector3(value.X, value.Y, value.Z);
    }

    /// <summary>
    /// Performs an explicit conversion from <see cref="Int3"/> to <see cref="Vector4"/>.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator Vector4(Int3 value)
    {
        return new Vector4(value.X, value.Y, value.Z, 0);
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
        var handler = new DefaultInterpolatedStringHandler(8, 3, formatProvider);
        handler.AppendLiteral("X:");
        handler.AppendFormatted(X, format);
        handler.AppendLiteral(" Y:");
        handler.AppendFormatted(Y, format);
        handler.AppendLiteral(" Z:");
        handler.AppendFormatted(Z, format);
        return handler.ToStringAndClear();
    }

    bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        var format1 = format.Length > 0 ? format.ToString() : null;
        var handler = new MemoryExtensions.TryWriteInterpolatedStringHandler(8, 3, destination, provider, out _);
        handler.AppendLiteral("X:");
        handler.AppendFormatted(X, format1);
        handler.AppendLiteral(" Y:");
        handler.AppendFormatted(Y, format1);
        handler.AppendLiteral(" Z:");
        handler.AppendFormatted(Z, format1);
        return destination.TryWrite(ref handler, out charsWritten);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
    /// </returns>
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }

    /// <summary>
    /// Determines whether the specified <see cref="Int3"/> is equal to this instance.
    /// </summary>
    /// <param name="other">The <see cref="Int3"/> to compare with this instance.</param>
    /// <returns>
    /// <c>true</c> if the specified <see cref="Int3"/> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public readonly bool Equals(Int3 other)
    {
        return MathF.Abs(other.X - X) < MathUtil.ZeroTolerance &&
            MathF.Abs(other.Y - Y) < MathUtil.ZeroTolerance &&
            MathF.Abs(other.Z - Z) < MathUtil.ZeroTolerance;
    }

    /// <summary>
    /// Determines whether the specified <see cref="object"/> is equal to this instance.
    /// </summary>
    /// <param name="value">The <see cref="object"/> to compare with this instance.</param>
    /// <returns>
    /// <c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public override readonly bool Equals(object? value)
    {
        return value is Int3 i && Equals(i);
    }

    /// <summary>
    /// Deconstructs the vector's components into named variables.
    /// </summary>
    /// <param name="x">The X component</param>
    /// <param name="y">The Y component</param>
    /// <param name="z">The Z component</param>
    public readonly void Deconstruct(out int x, out int y, out int z)
    {
        x = X;
        y = Y;
        z = Z;
    }

#if WPFInterop
    /// <summary>
    /// Performs an implicit conversion from <see cref="Stride.Core.Mathematics.Int3"/> to <see cref="System.Windows.Media.Media3D.Int3D"/>.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator System.Windows.Media.Media3D.Int3D(Int3 value)
    {
        return new System.Windows.Media.Media3D.Int3D(value.X, value.Y, value.Z);
    }

    /// <summary>
    /// Performs an explicit conversion from <see cref="System.Windows.Media.Media3D.Int3D"/> to <see cref="Stride.Core.Mathematics.Int3"/>.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator Int3(System.Windows.Media.Media3D.Int3D value)
    {
        return new Int3((float)value.X, (float)value.Y, (float)value.Z);
    }
#endif

#if XnaInterop
    /// <summary>
    /// Performs an implicit conversion from <see cref="Stride.Core.Mathematics.Int3"/> to <see cref="Microsoft.Xna.Framework.Int3"/>.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator Microsoft.Xna.Framework.Int3(Int3 value)
    {
        return new Microsoft.Xna.Framework.Int3(value.X, value.Y, value.Z);
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="Microsoft.Xna.Framework.Int3"/> to <see cref="Stride.Core.Mathematics.Int3"/>.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator Int3(Microsoft.Xna.Framework.Int3 value)
    {
        return new Int3(value.X, value.Y, value.Z);
    }
#endif
}
