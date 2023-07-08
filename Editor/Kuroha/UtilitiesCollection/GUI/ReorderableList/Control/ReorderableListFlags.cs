using System;

namespace Kuroha.UtilitiesCollection
{
    [Flags]
    public enum ReorderableListFlags
    {
        DisableReordering = 0x0001,

        HideAddButton = 0x0002,

        HideRemoveButtons = 0x0004,

        DisableContextMenu = 0x0008,

        DisableDuplicateCommand = 0x0010,

        DisableAutoFocus = 0x0020,

        ShowIndices = 0x0040,

        DisableAutoScroll = 0x0080,

        ShowSizeField = 0x0100,
    }
}