// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
//
// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
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

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using Stride.Core.Mathematics;

namespace Stride.Core.TypeConverters;

/// <summary>
/// Defines a type converter for <see cref="Color"/>.
/// </summary>
public class ColorConverter : BaseConverter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ColorConverter"/> class.
    /// </summary>
    public ColorConverter()
    {
        var type = typeof(Color);
        Properties = new PropertyDescriptorCollection(
        [
            new FieldPropertyDescriptor(type.GetField(nameof(Color.R))!),
            new FieldPropertyDescriptor(type.GetField(nameof(Color.G))!),
            new FieldPropertyDescriptor(type.GetField(nameof(Color.B))!),
            new FieldPropertyDescriptor(type.GetField(nameof(Color.A))!),
        ]);
    }

    /// <inheritdoc/>
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        return destinationType == typeof(Color3) || destinationType == typeof(Color4) || base.CanConvertTo(context, destinationType);
    }

    /// <inheritdoc/>
    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(destinationType);
#else
        if (destinationType is null) throw new ArgumentNullException(nameof(destinationType));
#endif

        if (value is Color color2)
        {
            if (destinationType == typeof(string))
            {
                return color2.ToString();
            }
            if (destinationType == typeof(Color3))
            {
                return color2.ToColor3();
            }
            if (destinationType == typeof(Color4))
            {
                return color2.ToColor4();
            }
            if (destinationType == typeof(InstanceDescriptor))
            {
                var constructor = typeof(Color).GetConstructor(MathUtil.Array(typeof(byte), 4));
                if (constructor != null)
                    return new InstanceDescriptor(constructor, color2.ToArray());
            }
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }

    /// <inheritdoc/>
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(Color3) || sourceType == typeof(Color4) || base.CanConvertFrom(context, sourceType);
    }

    /// <inheritdoc/>
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        switch (value)
        {
            case Color3 color3:
                return (Color)color3;
            case Color4 color4:
                return (Color)color4;
            case string str:
                {
                    var colorValue = ColorExtensions.StringToRgba(str);
                    return Color.FromRgba(colorValue);
                }

            default:
                return base.ConvertFrom(context, culture, value);
        }
    }

    /// <inheritdoc/>
    public override object CreateInstance(ITypeDescriptorContext? context, IDictionary propertyValues)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(propertyValues);
#else
        if (propertyValues is null) throw new ArgumentNullException(nameof(propertyValues));
#endif
        return new Color(
            (byte)propertyValues[nameof(Color.R)]!,
            (byte)propertyValues[nameof(Color.G)]!,
            (byte)propertyValues[nameof(Color.B)]!,
            (byte)propertyValues[nameof(Color.A)]!);
    }
}
