#region License

// Copyright (c) K2 Workflow (SourceCode Technology Holdings Inc.). All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace SourceCode.Clay
{
    /// <summary>
    /// Allows the conversion of enum values to integral values.
    /// </summary>
    public static class EnumConvert
    {
        private sealed class Checked { }

        private sealed class Unchecked { }

        private static class Converter<TEnum, TInteger, TChecked>
            where TEnum : struct, Enum
            where TInteger : unmanaged, IComparable, IConvertible, IFormattable
        {
#pragma warning disable CA1000 // Do not declare static members on generic types
            private static readonly bool s_isChecked = typeof(TChecked) == typeof(Checked);

            public static Func<TEnum, TInteger> ConvertTo { get; }

            public static Func<TInteger, TEnum> ConvertFrom { get; }
#pragma warning restore CA1000 // Do not declare static members on generic types

            static Converter()
            {
                Type @enum = typeof(TEnum);
                Type integer = typeof(TInteger);

                try
                {
                    ParameterExpression enumParam = Expression.Parameter(@enum, "value");
                    ParameterExpression integerParam = Expression.Parameter(integer, "value");
                    Type underlying = @enum.GetEnumUnderlyingType();

                    var enumConv = (Expression)Expression.Convert(enumParam, underlying);
                    if (underlying != integer) enumConv = Convert(enumConv, integer);

                    Expression integerConv = underlying == integer
                        ? integerParam
                        : Convert(integerParam, underlying);
                    integerConv = Expression.Convert(integerConv, @enum);

                    ConvertTo = Expression.Lambda<Func<TEnum, TInteger>>(enumConv, enumParam).Compile();
                    ConvertFrom = Expression.Lambda<Func<TInteger, TEnum>>(integerConv, integerParam).Compile();
                }
                catch (Exception e)
                {
#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
                    // Exceptions are not raised in this method.

                    ConvertTo = _ => throw e;
                    ConvertFrom = _ => throw e;
#pragma warning restore CA1065 // Do not raise exceptions in unexpected locations
                }
            }

            private static Expression Convert(in Expression value, in Type type) => s_isChecked
                ? Expression.ConvertChecked(value, type)
                : Expression.Convert(value, type);
        }

        private static class ValueCache<TEnum>
            where TEnum : struct, Enum
        {
            internal static readonly Func<int> Length = () =>
            {
                Type @enum = typeof(TEnum);

                ulong[] values = (ulong[])typeof(Enum)
                    .GetMethod("InternalGetValues", BindingFlags.Static | BindingFlags.NonPublic) // Also see InternalGetNames
                    .Invoke(null, new[] { typeof(TEnum) });

                return values.Length;
            };
        };

        /// <summary>
        /// Converts the specified <typeparamref name="TEnum"/> into
        /// a <see cref="ulong"/>.
        /// </summary>
        /// <remarks>
        /// This method will fail if <typeparamref name="TEnum"/>
        /// is not an enum.
        /// </remarks>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The enum value to convert.</param>
        /// <returns>The <see cref="ulong"/>.</returns>
        public static ulong ToUInt64<TEnum>(TEnum value)
            where TEnum : struct, Enum
            => Converter<TEnum, ulong, Unchecked>.ConvertTo(value);

        /// <summary>
        /// Converts the specified <see cref="ulong"/> into
        /// a <typeparamref name="TEnum"/>.
        /// </summary>
        /// <remarks>
        /// This method will fail if <typeparamref name="TEnum"/>
        /// is not an enum.
        /// </remarks>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The <see cref="ulong"/> to convert.</param>
        /// <returns>The <typeparamref name="TEnum"/>.</returns>
        public static TEnum ToEnum<TEnum>(ulong value)
            where TEnum : struct, Enum
            => Converter<TEnum, ulong, Unchecked>.ConvertFrom(value);

        /// <summary>
        /// Converts the specified <typeparamref name="TEnum"/> into
        /// a <see cref="long"/>.
        /// </summary>
        /// <remarks>
        /// This method will fail if <typeparamref name="TEnum"/>
        /// is not an enum.
        /// </remarks>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The enum value to convert.</param>
        /// <returns>The <see cref="long"/>.</returns>
        public static long ToInt64<TEnum>(TEnum value)
            where TEnum : struct, Enum
            => Converter<TEnum, long, Unchecked>.ConvertTo(value);

        /// <summary>
        /// Converts the specified <see cref="long"/> into
        /// a <typeparamref name="TEnum"/>.
        /// </summary>
        /// <remarks>
        /// This method will fail if <typeparamref name="TEnum"/>
        /// is not an enum.
        /// </remarks>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The <see cref="long"/> to convert.</param>
        /// <returns>The <typeparamref name="TEnum"/>.</returns>
        public static TEnum ToEnum<TEnum>(long value)
            where TEnum : struct, Enum
            => Converter<TEnum, long, Unchecked>.ConvertFrom(value);

        /// <summary>
        /// Converts the specified <typeparamref name="TEnum"/> into
        /// a <see cref="uint"/>.
        /// </summary>
        /// <remarks>
        /// This method will fail if <typeparamref name="TEnum"/>
        /// is not an enum.
        /// </remarks>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The enum value to convert.</param>
        /// <returns>The <see cref="uint"/>.</returns>
        public static uint ToUInt32<TEnum>(TEnum value)
            where TEnum : struct, Enum
            => Converter<TEnum, uint, Unchecked>.ConvertTo(value);

        /// <summary>
        /// Converts the specified <see cref="uint"/> into
        /// a <typeparamref name="TEnum"/>.
        /// </summary>
        /// <remarks>
        /// This method will fail if <typeparamref name="TEnum"/>
        /// is not an enum.
        /// </remarks>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The <see cref="uint"/> to convert.</param>
        /// <returns>The <typeparamref name="TEnum"/>.</returns>
        public static TEnum ToEnum<TEnum>(uint value)
            where TEnum : struct, Enum
            => Converter<TEnum, uint, Unchecked>.ConvertFrom(value);

        /// <summary>
        /// Converts the specified <typeparamref name="TEnum"/> into
        /// a <see cref="int"/>.
        /// </summary>
        /// <remarks>
        /// This method will fail if <typeparamref name="TEnum"/>
        /// is not an enum.
        /// </remarks>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The enum value to convert.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public static int ToInt32<TEnum>(TEnum value)
            where TEnum : struct, Enum
            => Converter<TEnum, int, Unchecked>.ConvertTo(value);

        /// <summary>
        /// Converts the specified <see cref="int"/> into
        /// a <typeparamref name="TEnum"/>.
        /// </summary>
        /// <remarks>
        /// This method will fail if <typeparamref name="TEnum"/>
        /// is not an enum.
        /// </remarks>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The <see cref="int"/> to convert.</param>
        /// <returns>The <typeparamref name="TEnum"/>.</returns>
        public static TEnum ToEnum<TEnum>(int value)
            where TEnum : struct, Enum
            => Converter<TEnum, int, Unchecked>.ConvertFrom(value);

        /// <summary>
        /// Converts the specified <typeparamref name="TEnum"/> into
        /// a <see cref="ushort"/>.
        /// </summary>
        /// <remarks>
        /// This method will fail if <typeparamref name="TEnum"/>
        /// is not an enum.
        /// </remarks>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The enum value to convert.</param>
        /// <returns>The <see cref="ushort"/>.</returns>
        public static ushort ToUInt16<TEnum>(TEnum value)
            where TEnum : struct, Enum
            => Converter<TEnum, ushort, Unchecked>.ConvertTo(value);

        /// <summary>
        /// Converts the specified <see cref="ushort"/> into
        /// a <typeparamref name="TEnum"/>.
        /// </summary>
        /// <remarks>
        /// This method will fail if <typeparamref name="TEnum"/>
        /// is not an enum.
        /// </remarks>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The <see cref="ushort"/> to convert.</param>
        /// <returns>The <typeparamref name="TEnum"/>.</returns>
        public static TEnum ToEnum<TEnum>(ushort value)
            where TEnum : struct, Enum
            => Converter<TEnum, ushort, Unchecked>.ConvertFrom(value);

        /// <summary>
        /// Converts the specified <typeparamref name="TEnum"/> into
        /// a <see cref="short"/>.
        /// </summary>
        /// <remarks>
        /// This method will fail if <typeparamref name="TEnum"/>
        /// is not an enum.
        /// </remarks>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The enum value to convert.</param>
        /// <returns>The <see cref="short"/>.</returns>
        public static short ToInt16<TEnum>(TEnum value)
            where TEnum : struct, Enum
            => Converter<TEnum, short, Unchecked>.ConvertTo(value);

        /// <summary>
        /// Converts the specified <see cref="short"/> into
        /// a <typeparamref name="TEnum"/>.
        /// </summary>
        /// <remarks>
        /// This method will fail if <typeparamref name="TEnum"/>
        /// is not an enum.
        /// </remarks>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The <see cref="short"/> to convert.</param>
        /// <returns>The <typeparamref name="TEnum"/>.</returns>
        public static TEnum ToEnum<TEnum>(short value)
            where TEnum : struct, Enum
            => Converter<TEnum, short, Unchecked>.ConvertFrom(value);

        /// <summary>
        /// Converts the specified <typeparamref name="TEnum"/> into
        /// a <see cref="byte"/>.
        /// </summary>
        /// <remarks>
        /// This method will fail if <typeparamref name="TEnum"/>
        /// is not an enum.
        /// </remarks>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The enum value to convert.</param>
        /// <returns>The <see cref="byte"/>.</returns>
        public static byte ToByte<TEnum>(TEnum value)
            where TEnum : struct, Enum
            => Converter<TEnum, byte, Unchecked>.ConvertTo(value);

        /// <summary>
        /// Converts the specified <see cref="byte"/> into
        /// a <typeparamref name="TEnum"/>.
        /// </summary>
        /// <remarks>
        /// This method will fail if <typeparamref name="TEnum"/>
        /// is not an enum.
        /// </remarks>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The <see cref="byte"/> to convert.</param>
        /// <returns>The <typeparamref name="TEnum"/>.</returns>
        public static TEnum ToEnum<TEnum>(byte value)
            where TEnum : struct, Enum
            => Converter<TEnum, byte, Unchecked>.ConvertFrom(value);

        /// <summary>
        /// Converts the specified <typeparamref name="TEnum"/> into
        /// a <see cref="sbyte"/>.
        /// </summary>
        /// <remarks>
        /// This method will fail if <typeparamref name="TEnum"/>
        /// is not an enum.
        /// </remarks>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The enum value to convert.</param>
        /// <returns>The <see cref="sbyte"/>.</returns>
        public static sbyte ToSByte<TEnum>(TEnum value)
            where TEnum : struct, Enum
            => Converter<TEnum, sbyte, Unchecked>.ConvertTo(value);

        /// <summary>
        /// Converts the specified <see cref="sbyte"/> into
        /// a <typeparamref name="TEnum"/>.
        /// </summary>
        /// <remarks>
        /// This method will fail if <typeparamref name="TEnum"/>
        /// is not an enum.
        /// </remarks>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The <see cref="sbyte"/> to convert.</param>
        /// <returns>The <typeparamref name="TEnum"/>.</returns>
        public static TEnum ToEnum<TEnum>(sbyte value)
            where TEnum : struct, Enum
            => Converter<TEnum, sbyte, Unchecked>.ConvertFrom(value);

        /// <summary>
        /// Converts the specified <typeparamref name="TEnum"/> into
        /// a <see cref="ulong"/>.
        /// </summary>
        /// <remarks>
        /// This method will fail if the enum value is less than
        /// <see cref="ulong.MinValue"/>, or if <typeparamref name="TEnum"/>
        /// is not an enum.
        /// </remarks>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The enum value to convert.</param>
        /// <returns>The <see cref="ulong"/>.</returns>
        public static ulong ToUInt64Checked<TEnum>(TEnum value)
            where TEnum : struct, Enum
            => Converter<TEnum, ulong, Checked>.ConvertTo(value);

        /// <summary>
        /// Converts the specified <see cref="ulong"/> into
        /// a <typeparamref name="TEnum"/>.
        /// </summary>
        /// <remarks>
        /// This method will fail if the <paramref name="value"/> is outside
        /// the range of valid values for the underlying type of
        /// <typeparamref name="TEnum"/>, or if <typeparamref name="TEnum"/>
        /// is not an enum.
        /// </remarks>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The <see cref="ulong"/> to convert.</param>
        /// <returns>The <typeparamref name="TEnum"/>.</returns>
        public static TEnum ToEnumChecked<TEnum>(ulong value)
            where TEnum : struct, Enum
            => Converter<TEnum, ulong, Checked>.ConvertFrom(value);

        /// <summary>
        /// Converts the specified <typeparamref name="TEnum"/> into
        /// a <see cref="long"/>.
        /// </summary>
        /// <remarks>
        /// This method will fail if the enum value is larger than
        /// <see cref="long.MaxValue"/>, or if <typeparamref name="TEnum"/> is
        /// not an enum.
        /// </remarks>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The enum value to convert.</param>
        /// <returns>The <see cref="long"/>.</returns>
        public static long ToInt64Checked<TEnum>(TEnum value)
            where TEnum : struct, Enum
            => Converter<TEnum, long, Checked>.ConvertTo(value);

        /// <summary>
        /// Converts the specified <see cref="long"/> into
        /// a <typeparamref name="TEnum"/>.
        /// </summary>
        /// <remarks>
        /// This method will fail if the <paramref name="value"/> is outside
        /// the range of valid values for the underlying type of
        /// <typeparamref name="TEnum"/>, or if <typeparamref name="TEnum"/>
        /// is not an enum.
        /// </remarks>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The <see cref="long"/> to convert.</param>
        /// <returns>The <typeparamref name="TEnum"/>.</returns>
        public static TEnum ToEnumChecked<TEnum>(long value)
            where TEnum : struct, Enum
            => Converter<TEnum, long, Checked>.ConvertFrom(value);

        /// <summary>
        /// Converts the specified <typeparamref name="TEnum"/> into
        /// a <see cref="uint"/>.
        /// </summary>
        /// <remarks>
        /// This method will fail if the enum value is less than
        /// <see cref="uint.MinValue"/> or larger than <see cref="uint.MaxValue"/>,
        /// or if <typeparamref name="TEnum"/> is not an enum.
        /// </remarks>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The enum value to convert.</param>
        /// <returns>The <see cref="uint"/>.</returns>
        public static uint ToUInt32Checked<TEnum>(TEnum value)
            where TEnum : struct, Enum
            => Converter<TEnum, uint, Checked>.ConvertTo(value);

        /// <summary>
        /// Converts the specified <see cref="uint"/> into
        /// a <typeparamref name="TEnum"/>.
        /// </summary>
        /// <remarks>
        /// This method will fail if the <paramref name="value"/> is outside
        /// the range of valid values for the underlying type of
        /// <typeparamref name="TEnum"/>, or if <typeparamref name="TEnum"/>
        /// is not an enum.
        /// </remarks>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The <see cref="uint"/> to convert.</param>
        /// <returns>The <typeparamref name="TEnum"/>.</returns>
        public static TEnum ToEnumChecked<TEnum>(uint value)
            where TEnum : struct, Enum
            => Converter<TEnum, uint, Checked>.ConvertFrom(value);

        /// <summary>
        /// Converts the specified <typeparamref name="TEnum"/> into
        /// a <see cref="int"/>.
        /// </summary>
        /// <remarks>
        /// This method will fail if the enum value is less than
        /// <see cref="int.MinValue"/> or larger than <see cref="int.MaxValue"/>,
        /// or if <typeparamref name="TEnum"/> is not an enum.
        /// </remarks>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The enum value to convert.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public static int ToInt32Checked<TEnum>(TEnum value)
            where TEnum : struct, Enum
            => Converter<TEnum, int, Checked>.ConvertTo(value);

        /// <summary>
        /// Converts the specified <see cref="int"/> into
        /// a <typeparamref name="TEnum"/>.
        /// </summary>
        /// <remarks>
        /// This method will fail if the <paramref name="value"/> is outside
        /// the range of valid values for the underlying type of
        /// <typeparamref name="TEnum"/>, or if <typeparamref name="TEnum"/>
        /// is not an enum.
        /// </remarks>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The <see cref="int"/> to convert.</param>
        /// <returns>The <typeparamref name="TEnum"/>.</returns>
        public static TEnum ToEnumChecked<TEnum>(int value)
            where TEnum : struct, Enum
            => Converter<TEnum, int, Checked>.ConvertFrom(value);

        /// <summary>
        /// Converts the specified <typeparamref name="TEnum"/> into
        /// a <see cref="ushort"/>.
        /// </summary>
        /// <remarks>
        /// This method will fail if the enum value is less than
        /// <see cref="ushort.MinValue"/> or larger than <see cref="ushort.MaxValue"/>,
        /// or if <typeparamref name="TEnum"/> is not an enum.
        /// </remarks>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The enum value to convert.</param>
        /// <returns>The <see cref="ushort"/>.</returns>
        public static ushort ToUInt16Checked<TEnum>(TEnum value)
            where TEnum : struct, Enum
            => Converter<TEnum, ushort, Checked>.ConvertTo(value);

        /// <summary>
        /// Converts the specified <see cref="ushort"/> into
        /// a <typeparamref name="TEnum"/>.
        /// </summary>
        /// <remarks>
        /// This method will fail if the <paramref name="value"/> is outside
        /// the range of valid values for the underlying type of
        /// <typeparamref name="TEnum"/>, or if <typeparamref name="TEnum"/>
        /// is not an enum.
        /// </remarks>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The <see cref="ushort"/> to convert.</param>
        /// <returns>The <typeparamref name="TEnum"/>.</returns>
        public static TEnum ConvertToEnumChecked<TEnum>(ushort value)
            where TEnum : struct, Enum
            => Converter<TEnum, ushort, Checked>.ConvertFrom(value);

        /// <summary>
        /// Converts the specified <typeparamref name="TEnum"/> into
        /// a <see cref="short"/>.
        /// </summary>
        /// <remarks>
        /// This method will fail if the enum value is less than
        /// <see cref="short.MinValue"/> or larger than <see cref="ushort.MaxValue"/>,
        /// or if <typeparamref name="TEnum"/> is not an enum.
        /// </remarks>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The enum value to convert.</param>
        /// <returns>The <see cref="short"/>.</returns>
        public static short ToInt16Checked<TEnum>(TEnum value)
            where TEnum : struct, Enum
            => Converter<TEnum, short, Checked>.ConvertTo(value);

        /// <summary>
        /// Converts the specified <see cref="short"/> into
        /// a <typeparamref name="TEnum"/>.
        /// </summary>
        /// <remarks>
        /// This method will fail if the <paramref name="value"/> is outside
        /// the range of valid values for the underlying type of
        /// <typeparamref name="TEnum"/>, or if <typeparamref name="TEnum"/>
        /// is not an enum.
        /// </remarks>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The <see cref="short"/> to convert.</param>
        /// <returns>The <typeparamref name="TEnum"/>.</returns>
        public static TEnum ToEnumChecked<TEnum>(short value)
            where TEnum : struct, Enum
            => Converter<TEnum, short, Checked>.ConvertFrom(value);

        /// <summary>
        /// Converts the specified <typeparamref name="TEnum"/> into
        /// a <see cref="byte"/>.
        /// </summary>
        /// <remarks>
        /// This method will fail if the enum value is less than
        /// <see cref="byte.MinValue"/> or larger than <see cref="byte.MaxValue"/>,
        /// or if <typeparamref name="TEnum"/> is not an enum.
        /// </remarks>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The enum value to convert.</param>
        /// <returns>The <see cref="byte"/>.</returns>
        public static byte ToByteChecked<TEnum>(TEnum value)
            where TEnum : struct, Enum
            => Converter<TEnum, byte, Checked>.ConvertTo(value);

        /// <summary>
        /// Converts the specified <see cref="byte"/> into
        /// a <typeparamref name="TEnum"/>.
        /// </summary>
        /// <remarks>
        /// This method will fail if the <paramref name="value"/> is outside
        /// the range of valid values for the underlying type of
        /// <typeparamref name="TEnum"/>, or if <typeparamref name="TEnum"/>
        /// is not an enum.
        /// </remarks>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The <see cref="byte"/> to convert.</param>
        /// <returns>The <typeparamref name="TEnum"/>.</returns>
        public static TEnum ToEnumChecked<TEnum>(byte value)
            where TEnum : struct, Enum
            => Converter<TEnum, byte, Checked>.ConvertFrom(value);

        /// <summary>
        /// Converts the specified <typeparamref name="TEnum"/> into
        /// a <see cref="sbyte"/>.
        /// </summary>
        /// <remarks>
        /// This method will fail if the enum value is less than
        /// <see cref="sbyte.MinValue"/> or larger than <see cref="sbyte.MaxValue"/>,
        /// or if <typeparamref name="TEnum"/> is not an enum.
        /// </remarks>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The enum value to convert.</param>
        /// <returns>The <see cref="sbyte"/>.</returns>
        public static sbyte ToSByteChecked<TEnum>(TEnum value)
            where TEnum : struct, Enum
            => Converter<TEnum, sbyte, Checked>.ConvertTo(value);

        /// <summary>
        /// Converts the specified <see cref="sbyte"/> into
        /// a <typeparamref name="TEnum"/>.
        /// </summary>
        /// <remarks>
        /// This method will fail if the <paramref name="value"/> is outside
        /// the range of valid values for the underlying type of
        /// <typeparamref name="TEnum"/>, or if <typeparamref name="TEnum"/>
        /// is not an enum.
        /// </remarks>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The <see cref="sbyte"/> to convert.</param>
        /// <returns>The <typeparamref name="TEnum"/>.</returns>
        public static TEnum ToEnumChecked<TEnum>(sbyte value)
            where TEnum : struct, Enum
            => Converter<TEnum, sbyte, Checked>.ConvertFrom(value);

        /// <summary>
        /// Gets the number of items in the specified <see cref="Enum"/>.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <returns>The member count.</returns>
        public static int Length<TEnum>()
            where TEnum : struct, Enum
            => ValueCache<TEnum>.Length();

        private static readonly Dictionary<RuntimeTypeHandle, StringDictionary> s_enumDesc = new Dictionary<RuntimeTypeHandle, StringDictionary>();

        public static string GetEnumDescription<TEnum>(this TEnum value)
            where TEnum : struct, Enum
        {
            Type typ = typeof(TEnum);
            if (!s_enumDesc.TryGetValue(typ.TypeHandle, out StringDictionary dict))
            {
                lock (((System.Collections.ICollection)s_enumDesc).SyncRoot)
                {
                    if (!s_enumDesc.TryGetValue(typ.TypeHandle, out dict))
                    {
                        dict = new StringDictionary();

                        foreach (string name in Enum.GetNames(typ))
                        {
                            // Try to get [Description] attribute
                            FieldInfo field = typ.GetField(name);
                            DescriptionAttribute attr = field.GetCustomAttribute<DescriptionAttribute>(false);

                            // Populate dictionary
                            dict[name] = attr == null ? name : attr.Description;
                        }

                        s_enumDesc[typ.TypeHandle] = dict;
                    }
                }
            }

            return dict[value.ToString()];
        }
    }
}
