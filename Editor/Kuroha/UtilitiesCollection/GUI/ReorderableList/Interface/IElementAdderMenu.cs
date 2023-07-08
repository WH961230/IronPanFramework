using UnityEngine;

namespace Kuroha.UtilitiesCollection
{
    public interface IElementAdderMenu
    {
        bool IsEmpty { get; }

        void DropDown(Rect position);
    }
}