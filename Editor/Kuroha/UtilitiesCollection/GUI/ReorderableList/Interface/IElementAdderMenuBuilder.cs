using System;

namespace Kuroha.UtilitiesCollection
{
    public interface IElementAdderMenuBuilder<TContext>
    {
        void SetContractType(Type type);

        void SetElementAdder(IElementAdder<TContext> elementAdder);

        void SetTypeDisplayNameFormatter(Func<Type, string> formatter);

        void AddTypeFilter(Func<Type, bool> typeFilter);

        void AddCustomCommand(IElementAdderMenuCommand<TContext> command);

        IElementAdderMenu GetMenu();
    }
}