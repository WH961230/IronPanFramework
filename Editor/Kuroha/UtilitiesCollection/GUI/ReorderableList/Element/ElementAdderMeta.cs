using System;
using System.Collections.Generic;
using System.Linq;

namespace Kuroha.UtilitiesCollection
{
    internal static class ElementAdderMeta
    {
        private static readonly Dictionary<Type, Dictionary<Type, List<Type>>> contextMap = new Dictionary<Type, Dictionary<Type, List<Type>>>();

        private static IEnumerable<Type> GetMenuCommandTypes<TContext>()
        {
            return
                from a in AppDomain.CurrentDomain.GetAssemblies()
                from t in a.GetTypes()
                where t.IsClass && !t.IsAbstract && t.IsDefined(typeof(ElementAdderMenuCommandAttribute), false)
                where typeof(IElementAdderMenuCommand<TContext>).IsAssignableFrom(t)
                select t;
        }

        private static Type[] GetMenuCommandTypes<TContext>(Type contractType)
        {
            if (contractType == null)
            {
                throw new ArgumentNullException(nameof(contractType));
            }

            List<Type> commandTypes;
            if (contextMap.TryGetValue(typeof(TContext), out var map))
            {
                if (map.TryGetValue(contractType, out commandTypes))
                {
                    return commandTypes.ToArray();
                }
            }
            else
            {
                map = new Dictionary<Type, List<Type>>();
                contextMap[typeof(TContext)] = map;
            }

            commandTypes = new List<Type>();

            foreach (var commandType in GetMenuCommandTypes<TContext>())
            {
                var attributes = (ElementAdderMenuCommandAttribute[]) Attribute.GetCustomAttributes(commandType, typeof(ElementAdderMenuCommandAttribute));
                if (attributes.All(a => a.ContractType != contractType))
                {
                    continue;
                }

                commandTypes.Add(commandType);
            }

            map[contractType] = commandTypes;
            return commandTypes.ToArray();
        }

        public static IElementAdderMenuCommand<TContext>[] GetMenuCommands<TContext>(Type contractType)
        {
            var commandTypes = GetMenuCommandTypes<TContext>(contractType);
            var commands = new IElementAdderMenuCommand<TContext>[commandTypes.Length];
            for (var i = 0; i < commandTypes.Length; ++i)
            {
                commands[i] = (IElementAdderMenuCommand<TContext>) Activator.CreateInstance(commandTypes[i]);
            }

            return commands;
        }

        private static readonly Dictionary<Type, Type[]> concreteElementTypes = new Dictionary<Type, Type[]>();

        private static IEnumerable<Type> GetConcreteElementTypesHelper(Type contractType)
        {
            if (contractType == null)
            {
                throw new ArgumentNullException(nameof(contractType));
            }

            if (!concreteElementTypes.TryGetValue(contractType, out var concreteTypes))
            {
                concreteTypes =
                    (from a in AppDomain.CurrentDomain.GetAssemblies()
                        from t in a.GetTypes()
                        where t.IsClass && !t.IsAbstract && contractType.IsAssignableFrom(t)
                        orderby t.Name
                        select t
                    ).ToArray();
                concreteElementTypes[contractType] = concreteTypes;
            }

            return concreteTypes;
        }

        public static Type[] GetConcreteElementTypes(Type contractType, Func<Type, bool>[] filters)
        {
            return
                (from t in GetConcreteElementTypesHelper(contractType)
                    where IsTypeIncluded(t, filters)
                    select t
                ).ToArray();
        }

        public static Type[] GetConcreteElementTypes(Type contractType)
        {
            return GetConcreteElementTypesHelper(contractType).ToArray();
        }

        private static bool IsTypeIncluded(Type concreteType, Func<Type, bool>[] filters)
        {
            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    if (!filter(concreteType))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
