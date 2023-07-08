using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kuroha.UtilitiesCollection
{
    internal sealed class GenericElementAdderMenuBuilder<TContext> : IElementAdderMenuBuilder<TContext>
    {
        private static string NicifyTypeName(Type type)
        {
            return ObjectNames.NicifyVariableName(type.Name);
        }

        private Type contractType;
        private IElementAdder<TContext> elementAdder;
        private Func<Type, string> typeDisplayNameFormatter;
        private readonly List<Func<Type, bool>> typeFilters = new List<Func<Type, bool>>();
        private readonly List<IElementAdderMenuCommand<TContext>> customCommands = new List<IElementAdderMenuCommand<TContext>>();

        public GenericElementAdderMenuBuilder()
        {
            typeDisplayNameFormatter = NicifyTypeName;
        }

        public IElementAdderMenu GetMenu()
        {
            var menu = new GenericElementAdderMenu();

            AddCommandsToMenu(menu, customCommands);

            if (contractType != null)
            {
                AddCommandsToMenu(menu, ElementAdderMeta.GetMenuCommands<TContext>(contractType));
                AddConcreteTypesToMenu(menu, ElementAdderMeta.GetConcreteElementTypes(contractType, typeFilters.ToArray()));
            }

            return menu;
        }

        public void SetContractType(Type type)
        {
            contractType = type;
        }

        public void SetElementAdder(IElementAdder<TContext> adder)
        {
            elementAdder = adder;
        }

        public void SetTypeDisplayNameFormatter(Func<Type, string> formatter)
        {
            typeDisplayNameFormatter = formatter ?? NicifyTypeName;
        }

        public void AddTypeFilter(Func<Type, bool> typeFilter)
        {
            if (typeFilter == null)
            {
                throw new ArgumentNullException(nameof(typeFilter));
            }

            typeFilters.Add(typeFilter);
        }

        public void AddCustomCommand(IElementAdderMenuCommand<TContext> command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            customCommands.Add(command);
        }

        private void AddCommandsToMenu(GenericElementAdderMenu menu, in IList<IElementAdderMenuCommand<TContext>> commands)
        {
            if (commands.Count == 0)
            {
                return;
            }

            if (!menu.IsEmpty)
            {
                menu.AddSeparator();
            }

            foreach (var command in commands)
            {
                if (elementAdder != null && command.CanExecute(elementAdder))
                {
                    menu.AddItem(command.Content, () => command.Execute(elementAdder));
                }
                else
                {
                    menu.AddDisabledItem(command.Content);
                }
            }
        }

        private void AddConcreteTypesToMenu(GenericElementAdderMenu menu, in Type[] concreteTypes)
        {
            if (concreteTypes.Length == 0)
            {
                return;
            }

            if (!menu.IsEmpty)
            {
                menu.AddSeparator();
            }

            foreach (var concreteType in concreteTypes)
            {
                var content = new GUIContent(typeDisplayNameFormatter(concreteType));
                if (elementAdder != null && elementAdder.CanAddElement(concreteType))
                {
                    menu.AddItem(content, () =>
                    {
                        if (elementAdder.CanAddElement(concreteType))
                            elementAdder.AddElement(concreteType);
                    });
                }
                else
                {
                    menu.AddDisabledItem(content);
                }
            }
        }
    }
}
