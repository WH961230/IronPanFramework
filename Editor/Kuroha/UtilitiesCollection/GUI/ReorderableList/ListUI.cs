// ReSharper disable UnusedAutoPropertyAccessor.Global

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kuroha.UtilitiesCollection
{
    internal static class ListUI
    {
        private const float DEFAULT_ITEM_HEIGHT = 18;
        private static readonly GUIContent titleGUIContent = new GUIContent();
        private static ReorderableListControl DefaultListControl { get; }

        public static int IndexOfChangedItem { get; set; }
        public static int CurrentItemIndex => ReorderableListControl.CurrentItemIndex;
        public static int CurrentListControlID => ReorderableListControl.CurrentListControlID;
        public static Rect CurrentListPosition => ReorderableListControl.CurrentListPosition;
        public static Rect CurrentItemTotalPosition => ReorderableListControl.CurrentItemTotalPosition;

        static ListUI()
        {
            DefaultListControl = new ReorderableListControl
            {
                ContainerStyle = new GUIStyle(ReorderableListStyles.Container),
                FooterButtonStyle = new GUIStyle(ReorderableListStyles.FooterButton),
                ItemButtonStyle = new GUIStyle(ReorderableListStyles.ItemButton)
            };

            IndexOfChangedItem = -1;
        }

        public static T DefaultItemDrawer<T>(Rect position, T item)
        {
            UnityEngine.GUI.Label(position, "方法 Item Drawer 未实现");
            return item;
        }

        public static void Title(string title, bool defaultDrawer = true, Action<Rect, GUIStyle> titleDrawer = null)
        {
            var position = GUILayoutUtility.GetRect(titleGUIContent, ReorderableListStyles.Title);

            titleGUIContent.text = title;

            if (Event.current.type == EventType.Repaint)
            {
                if (defaultDrawer)
                {
                    ReorderableListStyles.Title.Draw(position, titleGUIContent, false, false, false, false);
                }
                else
                {
                    titleDrawer?.Invoke(position, ReorderableListStyles.Title);
                }
            }

            GUILayout.Space(-1);
        }

        public static void List<T>(IList<T> list, ReorderableListControl.ItemDrawer<T> itemDrawer, ReorderableListControl.EmptyDrawer emptyDrawer, float itemHeight = DEFAULT_ITEM_HEIGHT, ReorderableListFlags flags = 0)
        {
            var adaptor = new GenericList<T>(list, itemHeight, itemDrawer);
            ReorderableListControl.DrawControlFromState(adaptor, emptyDrawer, flags);
        }

        public static void List<T>(Rect position, IList<T> list, ReorderableListControl.ItemDrawer<T> drawItem, ReorderableListControl.EmptyDrawerRect emptyDrawer, float itemHeight = DEFAULT_ITEM_HEIGHT, ReorderableListFlags flags = 0)
        {
            var adaptor = new GenericList<T>(list, itemHeight, drawItem);
            ReorderableListControl.DrawControlFromState(position, adaptor, emptyDrawer, flags);
        }

        public static float CalculateListFieldHeight(int itemCount, float itemHeight = DEFAULT_ITEM_HEIGHT, ReorderableListFlags flags = 0)
        {
            // We need to push/pop flags so that nested controls are properly calculated.
            var restoreFlags = DefaultListControl.Flags;
            try
            {
                DefaultListControl.Flags = flags;
                return DefaultListControl.CalculateListHeight(itemCount, itemHeight);
            }
            finally
            {
                DefaultListControl.Flags = restoreFlags;
            }
        }

        public static void List(SerializedProperty arrayProperty, float fixedItemHeight = 0, ReorderableListControl.EmptyDrawer emptyDrawer = null, ReorderableListFlags flags = 0)
        {
            var adaptor = new MonoBehaviourList(arrayProperty, fixedItemHeight);
            ReorderableListControl.DrawControlFromState(adaptor, emptyDrawer, flags);
        }

        public static void List(Rect position, SerializedProperty arrayProperty, float fixedItemHeight = 0, ReorderableListControl.EmptyDrawerRect emptyDrawer = null, ReorderableListFlags flags = 0)
        {
            var adaptor = new MonoBehaviourList(arrayProperty, fixedItemHeight);
            ReorderableListControl.DrawControlFromState(position, adaptor, emptyDrawer, flags);
        }

        public static float CalculateListFieldHeight(SerializedProperty arrayProperty, ReorderableListFlags flags = 0)
        {
            var restoreFlags = DefaultListControl.Flags;
            try
            {
                DefaultListControl.Flags = flags;
                return DefaultListControl.CalculateListHeight(new MonoBehaviourList(arrayProperty));
            }
            finally
            {
                DefaultListControl.Flags = restoreFlags;
            }
        }

        public static void List(IReorderableListAdaptor adaptor, ReorderableListControl.EmptyDrawer emptyDrawer, ReorderableListFlags flags = 0)
        {
            ReorderableListControl.DrawControlFromState(adaptor, emptyDrawer, flags);
        }

        public static void List(Rect position, IReorderableListAdaptor adaptor, ReorderableListControl.EmptyDrawerRect emptyDrawer, ReorderableListFlags flags = 0)
        {
            ReorderableListControl.DrawControlFromState(position, adaptor, emptyDrawer, flags);
        }

        public static float CalculateListFieldHeight(IReorderableListAdaptor adaptor, ReorderableListFlags flags = 0)
        {
            var restoreFlags = DefaultListControl.Flags;
            try
            {
                DefaultListControl.Flags = flags;
                return DefaultListControl.CalculateListHeight(adaptor);
            }
            finally
            {
                DefaultListControl.Flags = restoreFlags;
            }
        }
    }
}
