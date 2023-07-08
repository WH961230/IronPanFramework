using UnityEngine;

namespace Kuroha.UtilitiesCollection
{
    public interface IElementAdderMenuCommand<in TContext>
    {
        GUIContent Content { get; }

        bool CanExecute(IElementAdder<TContext> elementAdder);

        void Execute(IElementAdder<TContext> elementAdder);
    }
}