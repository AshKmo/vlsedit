using System.Reflection;

// NOTE: THIS CODE IS ONLY USED TO GENERATE THE PROGRAM'S UML TEXT AND IS NOT USED IN NORMAL OPERATION OF THE PROGRAM

namespace VLSEdit
{
    public static class Meta
    {
        public static void Generate()
        {
            BindingFlags allFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

            Type[] types = Assembly.GetExecutingAssembly().GetTypes().Where(t => String.Equals(t.Namespace, "VLSEdit", StringComparison.Ordinal)).ToArray();

            foreach (Type type in types)
            {
                if (type.Name == "Meta" || type.Name.StartsWith("<>") || !(type.IsClass || type.IsInterface)) continue;

                Console.WriteLine($"{type.Name}{Inheritance(type)}{ClassModifiers(type)}");
                Console.WriteLine();

                bool has = false;

                foreach (FieldInfo field in type.GetFields(allFlags))
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

                foreach (PropertyInfo property in type.GetProperties(allFlags))
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

                foreach (ConstructorInfo constructor in type.GetConstructors(allFlags))
                {
                    if (type.IsAbstract) break;

                    Console.WriteLine($"{TypeIndicator(constructor)}{type.Name}({String.Join(", ", constructor.GetParameters().ToList().Select(x => $"{GetFriendlyTypeName(x.ParameterType)} {x.Name}"))})");

                    has = true;
                }

                if (has)
                {
                    Console.WriteLine();
                }

                has = false;

                foreach (MethodInfo method in type.GetMethods(allFlags))
                {
                    if (method.DeclaringType != type) continue;

                    if (method.Name.StartsWith("get_") || method.Name.StartsWith("set_") || method.DeclaringType != type) continue;

                    Console.WriteLine($"{TypeIndicator(method)}{method.Name}({String.Join(", ", method.GetParameters().ToList().Select(x => $"{GetFriendlyTypeName(x.ParameterType)} {x.Name}"))}){ReturnTypeName(method)}{GetModifiers(method, type)}");

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
            List<string> modifiers = new List<string>();

            if (type.BaseType != null && type.BaseType != typeof(Object))
            {
                modifiers.Add(GetFriendlyTypeName(type.BaseType));
            }

            foreach (Type inter in type.GetInterfaces())
            {
                if (type.BaseType?.GetInterfaces().Contains(inter) ?? false) continue;
                modifiers.Add(GetFriendlyTypeName(inter));
            }

            if (modifiers.Count == 0)
            {
                return "";
            }

            return $" : {String.Join(", ", modifiers)}";
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

        private static string GetModifiers(MethodInfo method, Type type)
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

            if (IsActuallyVirtual(method))
            {
                modifiers.Add("virtual");
            }

            return modifiers.Count == 0 ? "" : $" <<{String.Join(", ", modifiers)}>>";
        }

        private static bool IsActuallyVirtual(MethodInfo method)
        {
            return method.IsVirtual && !method.IsAbstract && method.Equals(method.GetBaseDefinition()) && !OverridesAbstractBaseMethod(method) && !ImplementsInterfaceMethod(method);
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

            if (property.GetMethod != null && IsActuallyVirtual(property.GetMethod) || property.SetMethod != null && IsActuallyVirtual(property.SetMethod))
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

        public static bool ImplementsInterfaceMethod(MethodInfo method)
        {
            Type? type = method.DeclaringType;
            if (type == null) return false;
            foreach (var iface in type.GetInterfaces())
            {
                var map = type.GetInterfaceMap(iface);
                if (Array.Exists(map.TargetMethods, m => m == method))
                    return true;
            }
            return false;
        }

        public static bool OverridesAbstractBaseMethod(MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            if (!method.IsVirtual || method.IsAbstract)
                return false;

            var current = method;
            var baseType = method.DeclaringType?.BaseType;

            while (baseType != null)
            {
                var candidates = baseType.GetMethods(
                    BindingFlags.Public | BindingFlags.NonPublic |
                    BindingFlags.Instance | BindingFlags.DeclaredOnly);

                foreach (var candidate in candidates)
                {
                    if (!candidate.IsVirtual)
                        continue;

                    // Compare signature (name + parameters)
                    if (candidate.Name == current.Name &&
                        ParametersMatch(candidate, current))
                    {
                        // Found the overridden base method
                        return candidate.IsAbstract;
                    }
                }

                baseType = baseType.BaseType;
            }

            return false;
        }

        private static bool ParametersMatch(MethodInfo a, MethodInfo b)
        {
            var pa = a.GetParameters();
            var pb = b.GetParameters();
            if (pa.Length != pb.Length) return false;

            for (int i = 0; i < pa.Length; i++)
                if (pa[i].ParameterType != pb[i].ParameterType)
                    return false;

            return true;
        }
    }
}