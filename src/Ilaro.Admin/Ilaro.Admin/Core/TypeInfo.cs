using System;
using System.Linq;
using Ilaro.Admin.Models;

namespace Ilaro.Admin.Core
{
    public static class TypeInfo
    {
        private static readonly Type[] Real = new[]
        {
            typeof(sbyte), typeof(sbyte?),
            typeof(byte), typeof(byte?),
            typeof(short), typeof(short?),
            typeof(ushort), typeof(ushort?),
            typeof(int), typeof(int?),
            typeof(uint), typeof(uint?),
            typeof(long), typeof(long?),
            typeof(ulong), typeof(ulong?)
        };

        private static readonly Type[] FloatingPoint = new[]
        {
            typeof(float), typeof(float?),
            typeof(double), typeof(double?),
            typeof(decimal), typeof(decimal?)
        };

        private static readonly Type[] Bool = new[]
        {
            typeof(bool), typeof(bool?)
        };

        private static readonly Type[] AvailableForSearch = new[]
        {
            typeof (string),
            typeof (int),
            typeof (short),
            typeof (long),
            typeof (double),
            typeof (decimal),
            typeof (int?),
            typeof (short?),
            typeof (long?),
            typeof (double?),
            typeof (decimal?)
        };

        public static bool IsChangeEntity(Type type)
        {
            return typeof(IEntityChange).IsAssignableFrom(type);
        }

        public static bool IsReal(Type type)
        {
            return Real.Contains(type);
        }

        public static bool IsFloatingPoint(Type type)
        {
            return FloatingPoint.Contains(type);
        }

        public static bool IsNumber(Type type)
        {
            return IsReal(type) || IsFloatingPoint(type);
        }

        public static bool IsBool(Type type)
        {
            return Bool.Contains(type);
        }

        public static bool IsAvailableForSearch(Type type)
        {
            return AvailableForSearch.Contains(type);
        }
    }
}