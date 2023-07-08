using UnityEditor;
using UnityEngine;

namespace Kuroha.UtilitiesCollection
{
    internal sealed class GenericElementAdderMenu : IElementAdderMenu
    {
        private readonly GenericMenu innerMenu = new GenericMenu();

        public bool IsEmpty => innerMenu.GetItemCount() == 0;

        // public GenericElementAdderMenu() { }

        public void AddItem(GUIContent content, GenericMenu.MenuFunction handler)
        {
            innerMenu.AddItem(content, false, handler);
        }

        public void AddDisabledItem(GUIContent content)
        {
            innerMenu.AddDisabledItem(content);
        }

        public void AddSeparator(string path = "")
        {
            innerMenu.AddSeparator(path);
        }

        public void DropDown(Rect position)
        {
            innerMenu.DropDown(position);
        }
    }
}
