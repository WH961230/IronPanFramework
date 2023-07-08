using System;

namespace Kuroha.UtilitiesCollection
{
    public interface IElementAdder<out TContext>
    {
        TContext Object { get; }

        bool CanAddElement(Type type);

        object AddElement(Type type);
    }
}