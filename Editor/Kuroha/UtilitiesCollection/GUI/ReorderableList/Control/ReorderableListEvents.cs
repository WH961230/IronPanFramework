// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

using System;
using System.ComponentModel;
using UnityEngine;

namespace Kuroha.UtilitiesCollection
{
    public delegate void AddMenuClickedEventHandler(object sender, AddMenuClickedEventArgs args);

    public delegate void ItemInsertedEventHandler(object sender, ItemInsertedEventArgs args);

    public delegate void ItemRemovingEventHandler(object sender, ItemRemovingEventArgs args);

    public delegate void ItemMovingEventHandler(object sender, ItemMovingEventArgs args);

    public delegate void ItemMovedEventHandler(object sender, ItemMovedEventArgs args);

    public sealed class AddMenuClickedEventArgs : EventArgs
    {
        public IReorderableListAdaptor Adaptor { get; set; }

        public Rect ButtonPosition { get; set; }

        public AddMenuClickedEventArgs(IReorderableListAdaptor adaptor, Rect buttonPosition)
        {
            Adaptor = adaptor;
            ButtonPosition = buttonPosition;
        }
    }

    public sealed class ItemInsertedEventArgs : EventArgs
    {
        public IReorderableListAdaptor Adaptor { get; set; }

        public int ItemIndex { get; set; }

        public bool WasDuplicated { get; set; }

        public ItemInsertedEventArgs(IReorderableListAdaptor adaptor, int itemIndex, bool wasDuplicated)
        {
            Adaptor = adaptor;
            ItemIndex = itemIndex;
            WasDuplicated = wasDuplicated;
        }
    }

    public sealed class ItemRemovingEventArgs : CancelEventArgs
    {
        public IReorderableListAdaptor Adaptor { get; set; }

        public int ItemIndex { get; set; }

        public ItemRemovingEventArgs(IReorderableListAdaptor adaptor, int itemIndex)
        {
            Adaptor = adaptor;
            ItemIndex = itemIndex;
        }
    }

    public sealed class ItemMovingEventArgs : CancelEventArgs
    {
        public IReorderableListAdaptor Adaptor { get; set; }

        public int ItemIndex { get; set; }

        public int DestinationItemIndex { get; set; }

        public int NewItemIndex
        {
            get
            {
                var result = DestinationItemIndex;
                if (result > ItemIndex)
                {
                    --result;
                }

                return result;
            }
        }

        public ItemMovingEventArgs(IReorderableListAdaptor adaptor, int itemIndex, int destinationItemIndex)
        {
            Adaptor = adaptor;
            ItemIndex = itemIndex;
            DestinationItemIndex = destinationItemIndex;
        }
    }

    public sealed class ItemMovedEventArgs : EventArgs
    {
        public IReorderableListAdaptor Adaptor { get; set; }

        public int OldItemIndex { get; set; }

        public int NewItemIndex { get; set; }

        public ItemMovedEventArgs(IReorderableListAdaptor adaptor, int oldItemIndex, int newItemIndex)
        {
            Adaptor = adaptor;
            OldItemIndex = oldItemIndex;
            NewItemIndex = newItemIndex;
        }
    }
}
