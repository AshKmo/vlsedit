using System.Reflection;

// NOTE: THIS CODE IS ONLY USED TO GENERATE THE PROGRAM'S UML TEXT AND IS NOT USED IN NORMAL OPERATION OF THE PROGRAM

namespace VLSEdit
{
    public static class Meta
    {
        public static void Generate()
        {
            Type[] types = Assembly.GetExecutingAssembly().GetTypes().Where(t => String.Equals(t.Namespace, "VLSEdit", StringComparison.Ordinal)).ToArray();

            foreach (Type type in types)
            {
                if (type.Name == "Meta" || type.Name.StartsWith("<>") || !(type.IsClass || type.IsInterface)) continue;

                Console.WriteLine($"{type.Name}{Inheritance(type)}{ClassModifiers(type)}");
                Console.WriteLine();

                bool has = false;

                foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
                {
                    if (field.DeclaringType != type) continue;

                    Console.WriteLine($"{TypeIndicator(field)}{field.Name} : {GetFriendlyTypeName(field.FieldType)}{GetModifiers(field)}");

                    has = true;
                }

                if (has)
                {
                    Console.WriteLine();
                }

                has = false;

                foreach (PropertyInfo property in type.GetProperties())
                {
                    if (property.DeclaringType != type) continue;

                    Type? propertyType = property.GetMethod?.ReturnType ?? property.SetMethod?.GetParameters().First().ParameterType;

                    if (propertyType == null)
                    {
                        throw new Exception("property type is null");
                    }

                    Console.WriteLine($"{TypeIndicator(property)}{property.Name} : {GetFriendlyTypeName(propertyType)}{GetModifiers(property)}");

                    has = true;
                }

                if (has)
                {
                    Console.WriteLine();
                }

                has = false;

                foreach (ConstructorInfo constructor in type.GetConstructors())
                {
                    Console.WriteLine($"{TypeIndicator(constructor)}{type.Name}({String.Join(", ", constructor.GetParameters().ToList().Select(x => $"{GetFriendlyTypeName(x.ParameterType)} {x.Name}"))})");

                    has = true;
                }

                if (has)
                {
                    Console.WriteLine();
                }

                has = false;

                foreach (MethodInfo method in type.GetMethods())
                {
                    if (method.DeclaringType != type) continue;

                    if (method.Name.StartsWith("get_") || method.Name.StartsWith("set_") || method.DeclaringType != type) continue;

                    Console.WriteLine($"{TypeIndicator(method)}{method.Name}({String.Join(", ", method.GetParameters().ToList().Select(x => $"{GetFriendlyTypeName(x.ParameterType)} {x.Name}"))}){ReturnTypeName(method)}{GetModifiers(method)}");

                    has = true;
                }

                if (has)
                {
                    Console.WriteLine();
                }

                has = false;

                Console.WriteLine();
                Console.WriteLine();
            }
        }

        private static string Inheritance(Type type)
        {
            if (type.BaseType != null && type.BaseType != typeof(Object))
            {
                return $" : {GetFriendlyTypeName(type.BaseType)}";
            }

            return "";
        }

        private static string TypeIndicator(PropertyInfo info)
        {
            if (info.GetMethod != null)
            {
                return TypeIndicator(info.GetMethod);
            }

            if (info.SetMethod == null) return "";

            return TypeIndicator(info.SetMethod);
        }

        private static string TypeIndicator(MethodInfo info)
        {
            return info.IsPublic ? "+" : info.IsPrivate ? "-" : info.IsFamily ? "~" : "";
        }

        private static string TypeIndicator(ConstructorInfo info)
        {
            return info.IsPublic ? "+" : info.IsPrivate ? "-" : info.IsFamily ? "~" : "";
        }

        private static string TypeIndicator(FieldInfo info)
        {
            return info.IsPublic ? "+" : info.IsPrivate ? "-" : info.IsFamily ? "~" : "";
        }

        private static string GetFriendlyTypeName(Type type)
        {
            if (!type.IsGenericType)
                return type.Name;

            string typeName = type.Name;
            int backtickIndex = typeName.IndexOf('`');
            if (backtickIndex > 0)
                typeName = typeName[..backtickIndex]; // Remove the `1, `2 etc.

            string genericArgs = string.Join(", ",
                type.GetGenericArguments().Select(GetFriendlyTypeName));

            return $"{typeName}<{genericArgs}>";
        }

        private static string ReturnTypeName(MethodInfo method)
        {
            return method.ReturnType.Name == "Void" ? "" : " : " + GetFriendlyTypeName(method.ReturnType);
        }

        private static string ClassModifiers(Type type)
        {
            List<string> modifiers = new List<string>();

            if (type.IsInterface)
            {
                return " <<interface>>";
            }

            if (type.IsAbstract && !type.IsSealed)
            {
                modifiers.Add("abstract");
            }

            if (type.IsAbstract && type.IsSealed)
            {
                modifiers.Add("static");
            }

            return modifiers.Count == 0 ? "" : $" <<{String.Join(", ", modifiers)}>>";
        }

        private static string GetModifiers(MethodInfo method)
        {
            List<string> modifiers = new List<string>();

            if (method.IsStatic)
            {
                modifiers.Add("static");
            }

            if (method.IsAbstract)
            {
                modifiers.Add("abstract");
            }

            if (method.IsVirtual)
            {
                modifiers.Add("virtual");
            }

            return modifiers.Count == 0 ? "" : $" <<{String.Join(", ", modifiers)}>>";
        }

        private static string GetModifiers(PropertyInfo property)
        {
            List<string> modifiers = new List<string>();

            if ((property.GetMethod?.IsStatic ?? false) || (property.SetMethod?.IsStatic ?? false))
            {
                modifiers.Add("static");
            }

            if ((property.GetMethod?.IsAbstract ?? false) || (property.SetMethod?.IsAbstract ?? false))
            {
                modifiers.Add("abstract");
            }

            if ((property.GetMethod?.IsVirtual ?? false) || (property.SetMethod?.IsVirtual ?? false))
            {
                modifiers.Add("virtual");
            }

            if (property.SetMethod == null)
            {
                modifiers.Add("readonly");
            }

            modifiers.Add("property");

            return $" <<{String.Join(", ", modifiers)}>>";
        }

        private static string GetModifiers(FieldInfo field)
        {
            List<string> modifiers = new List<string>();

            if (field.IsStatic)
            {
                modifiers.Add("static");
            }

            if (field.IsInitOnly)
            {
                modifiers.Add("readonly");
            }

            return modifiers.Count == 0 ? "" : $" <<{String.Join(", ", modifiers)}>>";
        }
    }
}