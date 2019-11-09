// Copyright 2019 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Nuke.Common.Utilities.Collections;

namespace Nuke.Common.Execution
{
    public static class ReflectionService
    {
        public const BindingFlags All = Static | Instance | BindingFlags.FlattenHierarchy;

        public const BindingFlags Instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        public const BindingFlags Static = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        public static T GetValue<T>(this MemberInfo member, object obj = null, object[] args = null)
        {
            return (T) GetValue(member, obj, args);
        }

        public static object GetValue(this MemberInfo member, object obj = null, object[] args = null)
        {
            return member switch
            {
                FieldInfo fieldInfo => fieldInfo.GetValue(obj),
                PropertyInfo propertyInfo => propertyInfo.GetValue(obj),
                MethodInfo methodInfo => methodInfo.Invoke(obj, args),
                _ => throw new NotSupportedException()
            };
        }

        public static void SetValue(this MemberInfo member, object instance, object value)
        {
            // TODO: check if member is not (static && readonly)
            if (member is FieldInfo field)
            {
                field.SetValue(field.IsStatic ? null : instance, value);
            }
            else if (member is PropertyInfo property)
            {
                var backingField = member.DeclaringType.DescendantsAndSelf(x => x.GetTypeInfo().BaseType)
                    .SelectMany(x => x.GetFields(All))
                    .FirstOrDefault(x => x.Name.StartsWith($"<{member.Name}>"));

                if (backingField != null)
                {
                    backingField.SetValue(backingField.IsStatic ? null : instance, value);
                }
                else
                {
                    ControlFlow.Assert(property.SetMethod != null, $"Property '{member.Name}' is not settable.");
                    property.SetValue(property.GetMethod.IsStatic ? null : instance, value);
                }
            }
        }

        public static bool IsNullableType(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null ||
                   type == typeof(string) ||
                   type.IsClass ||
                   type.IsArray;
        }

        public static Type GetNullableType(this Type type)
        {
            return type.IsNullableType() ? type : typeof(Nullable<>).MakeGenericType(type);
        }

        public static bool HasCustomAttribute<T>(this MemberInfo member)
            where T : Attribute
        {
            return member.GetCustomAttribute<T>() != null;
        }

        public static MemberInfo GetMemberInfo(this LambdaExpression expression)
        {
            var memberExpression = !(expression.Body is UnaryExpression unaryExpression)
                ? (MemberExpression) expression.Body
                : (MemberExpression) unaryExpression.Operand;
            return memberExpression.Member;
        }

        public static Type GetMemberType(this MemberInfo member)
        {
            return member switch
            {
                FieldInfo fieldInfo => fieldInfo.FieldType,
                PropertyInfo propertyInfo => propertyInfo.PropertyType,
                MethodInfo methodInfo => methodInfo.ReturnType,
                _ => throw new NotSupportedException()
            };
        }

        public static T Convert<T>(string value)
        {
            return (T) Convert(value, typeof(T));
        }

        [CanBeNull]
        public static object Convert(object value, Type destinationType)
        {
            if (destinationType.IsInstanceOfType(value))
                return value;

            if (destinationType == typeof(string) && value == null)
                return null;

            try
            {
                var typeConverter = TypeDescriptor.GetConverter(destinationType);
                return typeConverter.ConvertFromInvariantString(value?.ToString());
            }
            catch
            {
                ControlFlow.Fail($"Value '{value}' could not be converted to '{GetPresentableName(destinationType)}'.");
                // ReSharper disable once HeuristicUnreachableCode
                return null;
            }
        }

        public static string GetPresentableName(Type type)
        {
            if (type.IsArray)
                return $"{type.GetElementType().NotNull().Name}[]";

            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
                return underlyingType.Name + "?";

            return type.Name;
        }

        public static object InvokeMember(
            this Type type,
            string memberName,
            object target,
            BindingFlags? bindingFlags = null,
            params object[] args)
        {
            return type.InvokeMember(
                memberName,
                (bindingFlags ?? (target != null ? Instance : Static)) | BindingFlags.InvokeMethod,
                binder: null,
                target,
                args);
        }

        public static T InvokeMember<T>(
            this Type type,
            string memberName,
            object target,
            BindingFlags? bindingFlags = null,
            params object[] args)
        {
            return (T) type.InvokeMember(memberName, target, bindingFlags, args);
        }
    }
}
