// ReSharper disable UnusedMethodReturnValue.Local

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kuroha.UtilitiesCollection
{
    internal class ReorderableListControl
    {
        private readonly struct ListInfo
        {
            public readonly int controlID;
            public readonly Rect position;

            public ListInfo(int controlID, Rect position)
            {
                this.controlID = controlID;
                this.position = position;
            }
        }

        private readonly struct ItemInfo
        {
            public readonly int itemIndex;
            public readonly Rect itemPosition;

            public ItemInfo(int itemIndex, Rect itemPosition)
            {
                this.itemIndex = itemIndex;
                this.itemPosition = itemPosition;
            }
        }

        public delegate T TitleDrawer<T>(Rect position);

        public delegate T ItemDrawer<T>(Rect position, T item);

        public delegate void EmptyDrawer();

        public delegate void EmptyDrawerRect(Rect position);

        public static int CurrentListControlID => currentListStack.Peek().controlID;
        public static Rect CurrentListPosition => currentListStack.Peek().position;
        public static int CurrentItemIndex => currentItemStack.Peek().itemIndex;
        public static Rect CurrentItemTotalPosition => currentItemStack.Peek().itemPosition;

        private static float anchorMouseOffset;
        private static int autoFocusControlID;
        private static int anchorIndex = -1;
        private static int targetIndex = -1;
        private static int autoFocusIndex = -1;

        private static readonly int reorderableListControlHint = "_ReorderableListControl_".GetHashCode();
        private static readonly Stack<ListInfo> currentListStack;
        private static readonly Stack<ItemInfo> currentItemStack;
        private static readonly Color anchorBackgroundColor;
        private static readonly Color targetBackgroundColor;
        private static GUIStyle rightAlignedLabelStyle;

        static ReorderableListControl()
        {
            currentListStack = new Stack<ListInfo>();
            currentListStack.Push(default);

            currentItemStack = new Stack<ItemInfo>();
            currentItemStack.Push(new ItemInfo(-1, default));

            if (EditorGUIUtility.isProSkin)
            {
                anchorBackgroundColor = new Color(85f / 255f, 85f / 255f, 85f / 255f, 0.85f);
                targetBackgroundColor = new Color(0, 0, 0, 0.5f);
            }
            else
            {
                anchorBackgroundColor = new Color(225f / 255f, 225f / 255f, 225f / 255f, 0.85f);
                targetBackgroundColor = new Color(0, 0, 0, 0.5f);
            }
        }

        public ReorderableListControl()
        {
            ContainerStyle = ReorderableListStyles.Container;
            FooterButtonStyle = ReorderableListStyles.FooterButton;
            ItemButtonStyle = ReorderableListStyles.ItemButton;

            HorizontalLineColor = ReorderableListStyles.HorizontalLineColor;
        }

        private static int GetReorderableListControlID()
        {
            return GUIUtility.GetControlID(reorderableListControlHint, FocusType.Passive);
        }

        public static void DrawControlFromState(IReorderableListAdaptor adaptor, EmptyDrawer emptyDrawer, ReorderableListFlags flags)
        {
            var controlID = GetReorderableListControlID();

            if (GUIUtility.GetStateObject(typeof(ReorderableListControl), controlID) is ReorderableListControl control)
            {
                control.Flags = flags;
                control.Draw(controlID, adaptor, emptyDrawer);
            }
        }

        public static void DrawControlFromState(Rect position, IReorderableListAdaptor adaptor, EmptyDrawerRect emptyDrawer, ReorderableListFlags flags)
        {
            var controlID = GetReorderableListControlID();

            if (GUIUtility.GetStateObject(typeof(ReorderableListControl), controlID) is ReorderableListControl control)
            {
                control.Flags = flags;
                control.Draw(position, controlID, adaptor, emptyDrawer);
            }
        }

        #region Properties

        public ReorderableListFlags Flags { get; set; }
        private bool HasFooterControls => HasSizeField || HasAddButton || HasAddMenuButton;
        private bool HasSizeField => (Flags & ReorderableListFlags.ShowSizeField) != 0;
        private bool HasAddButton => (Flags & ReorderableListFlags.HideAddButton) == 0;
        private bool HasAddMenuButton { get; set; }
        private bool HasRemoveButtons => (Flags & ReorderableListFlags.HideRemoveButtons) == 0;

        public GUIStyle ContainerStyle { get; set; }
        public GUIStyle FooterButtonStyle { get; set; }
        public GUIStyle ItemButtonStyle { get; set; }

        private Color HorizontalLineColor { get; set; }
        private float VerticalSpacing { get; set; } = 10f;
        private bool HorizontalLineAtStart { get; set; } = false;
        private bool HorizontalLineAtEnd { get; set; } = false;

        #endregion

        #region Events

        private int addMenuClickedSubscriberCount;
        private event AddMenuClickedEventHandler AddMenuClickedEvent;

        public event AddMenuClickedEventHandler AddMenuClicked
        {
            add
            {
                if (value == null)
                {
                    return;
                }

                AddMenuClickedEvent += value;
                ++addMenuClickedSubscriberCount;
                HasAddMenuButton = addMenuClickedSubscriberCount != 0;
            }
            remove
            {
                if (value == null)
                {
                    return;
                }

                AddMenuClickedEvent -= value;
                --addMenuClickedSubscriberCount;
                HasAddMenuButton = addMenuClickedSubscriberCount != 0;
            }
        }

        public event ItemInsertedEventHandler ItemInserted;
        public event ItemRemovingEventHandler ItemRemoving;
        public event ItemMovingEventHandler ItemMoving;
        public event ItemMovedEventHandler ItemMoved;

        private void OnAddMenuClicked(AddMenuClickedEventArgs args)
        {
            AddMenuClickedEvent?.Invoke(this, args);
        }

        private void OnItemInserted(ItemInsertedEventArgs args)
        {
            ItemInserted?.Invoke(this, args);
        }

        private void OnItemRemoving(ItemRemovingEventArgs args)
        {
            ItemRemoving?.Invoke(this, args);
        }

        private void OnItemMoving(ItemMovingEventArgs args)
        {
            ItemMoving?.Invoke(this, args);
        }

        private void OnItemMoved(ItemMovedEventArgs args)
        {
            ItemMoved?.Invoke(this, args);
        }

        #endregion

        #region Control State

        private int controlID;
        private Rect visibleRect;
        private float indexLabelWidth;
        private bool tracking;
        private bool allowReordering;
        private bool allowDropInsertion;
        private int insertionIndex;
        private float insertionPosition;
        private int newSizeInput;

        private void PrepareState(int id, IReorderableListAdaptor adaptor)
        {
            controlID = id;
            visibleRect = UIDrawUtility.visibleRect();

            if ((Flags & ReorderableListFlags.ShowIndices) != 0)
            {
                indexLabelWidth = CountDigits(adaptor.Count) * 8 + 8;
            }
            else
            {
                indexLabelWidth = 0;
            }

            tracking = IsTrackingControl(id);

            allowReordering = (Flags & ReorderableListFlags.DisableReordering) == 0;

            allowDropInsertion = true;
        }

        private static int CountDigits(int number)
        {
            return Mathf.Max(2, Mathf.CeilToInt(Mathf.Log10(number)));
        }

        #endregion

        #region Event Handling

        private static Rect dragItemPosition;
        private static Rect removeButtonPosition;
        private static int dropTargetNestedCounter;
        private static int simulateMouseDragControlID;
        private static bool trackingCancelBlockContext;
        private static readonly Dictionary<int, float> containerHeightCache = new Dictionary<int, float>();

        private void AutoFocusItem(int id, int itemIndex)
        {
            if ((Flags & ReorderableListFlags.DisableAutoFocus) == 0)
            {
                autoFocusControlID = id;
                autoFocusIndex = itemIndex;
            }
        }

        private bool DoRemoveButton(Rect position, bool visible)
        {
            var iconNormal = ReorderableListResources.GetTexture(ReorderableListTexture.IconRemoveNormal);
            var iconActive = ReorderableListResources.GetTexture(ReorderableListTexture.IconRemoveActive);

            return UIDrawUtility.IconButton(position, visible, iconNormal, iconActive, ItemButtonStyle);
        }

        private static void BeginTrackingReorderDrag(int controlID, int itemIndex)
        {
            GUIUtility.hotControl = controlID;
            GUIUtility.keyboardControl = 0;
            anchorIndex = itemIndex;
            targetIndex = itemIndex;
            trackingCancelBlockContext = false;
        }

        private static void StopTrackingReorderDrag()
        {
            GUIUtility.hotControl = 0;
            anchorIndex = -1;
            targetIndex = -1;
        }

        private static bool IsTrackingControl(int controlID)
        {
            return !trackingCancelBlockContext && GUIUtility.hotControl == controlID;
        }

        private static bool ContainsRect(Rect a, Rect b)
        {
            return a.Contains(new Vector2(b.xMin, b.yMin)) && a.Contains(new Vector2(b.xMax, b.yMax));
        }

        private void AcceptReorderDrag(IReorderableListAdaptor adaptor)
        {
            try
            {
                // Reorder list as needed!
                targetIndex = Mathf.Clamp(targetIndex, 0, adaptor.Count + 1);
                if (targetIndex != anchorIndex && targetIndex != anchorIndex + 1)
                {
                    MoveItem(adaptor, anchorIndex, targetIndex);
                }
            }
            finally
            {
                StopTrackingReorderDrag();
            }
        }

        private void DrawListItem(Rect position, IReorderableListAdaptor adaptor, int itemIndex)
        {
            var isRepainting = Event.current.type == EventType.Repaint;
            var isVisible = (position.y < visibleRect.yMax && position.yMax > visibleRect.y);
            var isDraggable = allowReordering && adaptor.CanDrag(itemIndex);

            var itemContentPosition = position;
            itemContentPosition.x = position.x + 2;
            itemContentPosition.y += 1;
            itemContentPosition.width = position.width - 4;
            itemContentPosition.height = position.height - 4;

            // Make space for grab handle?
            if (isDraggable)
            {
                itemContentPosition.x += 20;
                itemContentPosition.width -= 20;
            }

            // Make space for element index.
            if (indexLabelWidth != 0)
            {
                itemContentPosition.width -= indexLabelWidth;

                if (isRepainting && isVisible)
                {
                    rightAlignedLabelStyle.Draw(new Rect(itemContentPosition.x, position.y, indexLabelWidth, position.height - 4), itemIndex + ":", false, false, false, false);
                }

                itemContentPosition.x += indexLabelWidth;
            }

            // Make space for remove button?
            if (HasRemoveButtons)
            {
                itemContentPosition.width -= 27;
            }

            try
            {
                currentItemStack.Push(new ItemInfo(itemIndex, position));
                EditorGUI.BeginChangeCheck();

                if (isRepainting && isVisible)
                {
                    // Draw background of list item.
                    var backgroundPosition = new Rect(position.x, position.y, position.width, position.height - 1);
                    adaptor.DrawItemBackground(backgroundPosition, itemIndex);

                    // Draw grab handle?
                    if (isDraggable)
                    {
                        var texturePosition = new Rect(position.x + 6, position.y + position.height / 2f - 3, 9, 5);
                        UIDrawUtility.DrawTexture(texturePosition, ReorderableListResources.GetTexture(ReorderableListTexture.GrabHandle));
                    }

                    // Draw horizontal line between list items.
                    if (!tracking || itemIndex != anchorIndex)
                    {
                        if (itemIndex != 0 || HorizontalLineAtStart)
                        {
                            var horizontalLinePosition = new Rect(position.x, position.y - 1, position.width, 1);
                            UIDrawUtility.Separator(horizontalLinePosition, HorizontalLineColor);
                        }
                    }
                }

                // Allow control to be automatically focused.
                if (autoFocusIndex == itemIndex)
                {
                    UnityEngine.GUI.SetNextControlName("AutoFocus_" + controlID + "_" + itemIndex);
                }

                // Present actual control.
                adaptor.DrawItem(itemContentPosition, itemIndex);

                if (EditorGUI.EndChangeCheck())
                {
                    ListUI.IndexOfChangedItem = itemIndex;
                }

                // Draw remove button?
                if (HasRemoveButtons && adaptor.CanRemove(itemIndex))
                {
                    removeButtonPosition = position;
                    removeButtonPosition.width = 27;
                    removeButtonPosition.x = itemContentPosition.xMax + 2;
                    removeButtonPosition.y -= 1;

                    if (DoRemoveButton(removeButtonPosition, isVisible))
                    {
                        RemoveItem(adaptor, itemIndex);
                    }
                }

                // Check for context click?
                if ((Flags & ReorderableListFlags.DisableContextMenu) == 0)
                {
                    if (Event.current.GetTypeForControl(controlID) == EventType.ContextClick && position.Contains(Event.current.mousePosition))
                    {
                        ShowContextMenu(itemIndex, adaptor);
                        Event.current.Use();
                    }
                }
            }
            finally
            {
                currentItemStack.Pop();
            }
        }

        private void DrawFloatingListItem(IReorderableListAdaptor adaptor, float targetSlotPosition)
        {
            if (Event.current.type == EventType.Repaint)
            {
                var restoreColor = UnityEngine.GUI.color;

                // Fill background of target area.
                var targetPosition = dragItemPosition;
                targetPosition.y = targetSlotPosition - 1;
                targetPosition.height = 1;

                UIDrawUtility.Separator(targetPosition, HorizontalLineColor);

                --targetPosition.x;
                ++targetPosition.y;
                targetPosition.width += 2;
                targetPosition.height = dragItemPosition.height - 1;

                UnityEngine.GUI.color = targetBackgroundColor;
                UIDrawUtility.DrawTexture(targetPosition, EditorGUIUtility.whiteTexture);

                // Fill background of item which is being dragged.
                --dragItemPosition.x;
                dragItemPosition.width += 2;
                --dragItemPosition.height;

                UnityEngine.GUI.color = anchorBackgroundColor;
                UIDrawUtility.DrawTexture(dragItemPosition, EditorGUIUtility.whiteTexture);

                ++dragItemPosition.x;
                dragItemPosition.width -= 2;
                ++dragItemPosition.height;

                // Draw horizontal splitter above and below.
                UnityEngine.GUI.color = new Color(0f, 0f, 0f, 0.6f);
                targetPosition.y = dragItemPosition.y - 1;
                targetPosition.height = 1;
                UIDrawUtility.DrawTexture(targetPosition, EditorGUIUtility.whiteTexture);

                targetPosition.y += dragItemPosition.height;
                UIDrawUtility.DrawTexture(targetPosition, EditorGUIUtility.whiteTexture);

                UnityEngine.GUI.color = restoreColor;
            }

            DrawListItem(dragItemPosition, adaptor, anchorIndex);
        }

        private void DrawListContainerAndItems(Rect position, IReorderableListAdaptor adaptor)
        {
            var initialDropTargetNestedCounterValue = dropTargetNestedCounter;

            var eventType = Event.current.GetTypeForControl(controlID);
            var mousePosition = Event.current.mousePosition;
            var newTargetIndex = targetIndex;
            var firstItemY = position.y + ContainerStyle.padding.top;
            var dragItemMaxY = position.yMax - ContainerStyle.padding.bottom - dragItemPosition.height + 1;
            var isMouseDragEvent = eventType == EventType.MouseDrag;

            if (simulateMouseDragControlID == controlID && eventType == EventType.Repaint)
            {
                simulateMouseDragControlID = 0;
                isMouseDragEvent = true;
            }

            if (isMouseDragEvent && tracking)
            {
                if (mousePosition.y < firstItemY)
                {
                    newTargetIndex = 0;
                }
                else if (mousePosition.y >= position.yMax)
                {
                    newTargetIndex = adaptor.Count;
                }

                dragItemPosition.y = Mathf.Clamp(mousePosition.y + anchorMouseOffset, firstItemY, dragItemMaxY);
            }

            switch (eventType)
            {
                case EventType.MouseDown:
                    if (tracking)
                    {
                        trackingCancelBlockContext = true;
                        Event.current.Use();
                    }

                    break;

                case EventType.MouseUp:
                    if (controlID == GUIUtility.hotControl)
                    {
                        if (!trackingCancelBlockContext && allowReordering)
                        {
                            AcceptReorderDrag(adaptor);
                        }
                        else
                        {
                            StopTrackingReorderDrag();
                        }

                        Event.current.Use();
                    }

                    break;

                case EventType.KeyDown:
                    if (tracking && Event.current.keyCode == KeyCode.Escape)
                    {
                        StopTrackingReorderDrag();
                        Event.current.Use();
                    }

                    break;

                case EventType.ExecuteCommand:
                    if (contextControlID == controlID)
                    {
                        var itemIndex = contextItemIndex;
                        try
                        {
                            DoCommand(contextCommandName, itemIndex, adaptor);
                            Event.current.Use();
                        }
                        finally
                        {
                            contextControlID = 0;
                            contextItemIndex = 0;
                        }
                    }

                    break;

                case EventType.Repaint:
                    ContainerStyle.Draw(position, GUIContent.none, false, false, false, false);
                    break;
            }

            ListUI.IndexOfChangedItem = -1;

            var itemPosition = new Rect(position.x + ContainerStyle.padding.left, firstItemY, position.width - ContainerStyle.padding.horizontal, 0);
            var targetSlotPosition = dragItemMaxY;

            insertionIndex = 0;
            insertionPosition = itemPosition.yMax;

            float lastMidPoint;
            var lastHeight = 0f;
            var count = adaptor.Count;

            for (var i = 0; i < count; ++i)
            {
                itemPosition.y = itemPosition.yMax;
                itemPosition.height = 0;

                lastMidPoint = itemPosition.y - lastHeight / 2f;

                if (tracking)
                {
                    if (i == targetIndex)
                    {
                        targetSlotPosition = itemPosition.y;
                        itemPosition.y += dragItemPosition.height;
                    }

                    if (i == anchorIndex)
                    {
                        continue;
                    }

                    itemPosition.height = adaptor.GetItemHeight(i) + 4;
                    lastHeight = itemPosition.height;
                }
                else
                {
                    itemPosition.height = adaptor.GetItemHeight(i) + 4;
                    lastHeight = itemPosition.height;

                    var midpoint = itemPosition.y + itemPosition.height / 2f;
                    if (mousePosition.y > lastMidPoint && mousePosition.y <= midpoint)
                    {
                        insertionIndex = i;
                        insertionPosition = itemPosition.y;
                    }
                }

                if (tracking && isMouseDragEvent)
                {
                    var midpoint = itemPosition.y + itemPosition.height / 2f;

                    if (targetIndex < i)
                    {
                        if (dragItemPosition.yMax > lastMidPoint && dragItemPosition.yMax < midpoint)
                        {
                            newTargetIndex = i;
                        }
                    }
                    else if (targetIndex > i)
                    {
                        if (dragItemPosition.y > lastMidPoint && dragItemPosition.y < midpoint)
                        {
                            newTargetIndex = i;
                        }
                    }
                }

                // Draw list item.
                DrawListItem(itemPosition, adaptor, i);

                // Did list count change
                if (adaptor.Count < count)
                {
                    // We assume that it was this item which was removed, so --i allows us to process the next item as usual.
                    count = adaptor.Count;
                    --i;
                    continue;
                }

                if (Event.current.type != EventType.Used)
                {
                    if (eventType == EventType.MouseDown)
                    {
                        if (UnityEngine.GUI.enabled && itemPosition.Contains(mousePosition))
                        {
                            GUIUtility.keyboardControl = 0;

                            if (allowReordering && adaptor.CanDrag(i) && Event.current.button == 0)
                            {
                                dragItemPosition = itemPosition;

                                BeginTrackingReorderDrag(controlID, i);
                                anchorMouseOffset = itemPosition.y - mousePosition.y;
                                targetIndex = i;

                                Event.current.Use();
                            }
                        }
                    }
                }
            }

            if (HorizontalLineAtEnd)
            {
                var horizontalLinePosition = new Rect(itemPosition.x, position.yMax - ContainerStyle.padding.vertical, itemPosition.width, 1);
                UIDrawUtility.Separator(horizontalLinePosition, HorizontalLineColor);
            }

            lastMidPoint = position.yMax - lastHeight / 2f;

            allowDropInsertion = false;

            if (IsTrackingControl(controlID))
            {
                if (isMouseDragEvent)
                {
                    if (dragItemPosition.yMax >= lastMidPoint)
                    {
                        newTargetIndex = count;
                    }

                    targetIndex = newTargetIndex;

                    if (eventType == EventType.MouseDrag)
                    {
                        Event.current.Use();
                    }
                }

                DrawFloatingListItem(adaptor, targetSlotPosition);
            }
            else
            {
                if (dropTargetNestedCounter == initialDropTargetNestedCounterValue)
                {
                    if (Event.current.mousePosition.y >= lastMidPoint)
                    {
                        insertionIndex = adaptor.Count;
                        insertionPosition = itemPosition.yMax;
                    }

                    allowDropInsertion = true;
                }
            }

            GUIUtility.GetControlID(FocusType.Keyboard);

            if (isMouseDragEvent && (Flags & ReorderableListFlags.DisableAutoScroll) == 0 && IsTrackingControl(controlID))
            {
                AutoScrollTowardsMouse();
            }
        }

        private void AutoScrollTowardsMouse()
        {
            const float TRIGGER_PADDING_IN_PIXELS = 8f;
            const float MAXIMUM_RANGE_IN_PIXELS = 4f;

            var visiblePosition = UIDrawUtility.visibleRect();
            var mousePosition = Event.current.mousePosition;
            var mouseRect = new Rect(mousePosition.x - TRIGGER_PADDING_IN_PIXELS, mousePosition.y - TRIGGER_PADDING_IN_PIXELS, TRIGGER_PADDING_IN_PIXELS * 2, TRIGGER_PADDING_IN_PIXELS * 2);

            if (!ContainsRect(visiblePosition, mouseRect))
            {
                mousePosition = mousePosition.y < visiblePosition.center.y
                    ? new Vector2(mouseRect.xMin, mouseRect.yMin)
                    : new Vector2(mouseRect.xMax, mouseRect.yMax);

                mousePosition.x = Mathf.Max(mousePosition.x - MAXIMUM_RANGE_IN_PIXELS, mouseRect.xMax);
                mousePosition.y = Mathf.Min(mousePosition.y + MAXIMUM_RANGE_IN_PIXELS, mouseRect.yMax);
                UnityEngine.GUI.ScrollTo(new Rect(mousePosition.x, mousePosition.y, 1, 1));

                simulateMouseDragControlID = controlID;

                var focusedWindow = EditorWindow.focusedWindow;
                if (focusedWindow != null)
                {
                    focusedWindow.Repaint();
                }
            }
        }

        private void HandleDropInsertion(Rect position, IReorderableListAdaptor adaptor)
        {
            if (!allowDropInsertion)
            {
                return;
            }

            // ReSharper disable once SuspiciousTypeConversion.Global
            if (adaptor is IReorderableListDropTarget target)
            {
                if (target.CanDropInsert(insertionIndex))
                {
                    ++dropTargetNestedCounter;

                    switch (Event.current.type)
                    {
                        case EventType.DragUpdated:
                            DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                            DragAndDrop.activeControlID = controlID;
                            target.ProcessDropInsertion(insertionIndex);
                            Event.current.Use();
                            break;

                        case EventType.DragPerform:
                            target.ProcessDropInsertion(insertionIndex);

                            DragAndDrop.AcceptDrag();
                            DragAndDrop.activeControlID = 0;
                            Event.current.Use();
                            break;

                        default:
                            target.ProcessDropInsertion(insertionIndex);
                            break;
                    }

                    if (DragAndDrop.activeControlID == controlID && Event.current.type == EventType.Repaint)
                    {
                        DrawDropIndicator(new Rect(position.x, insertionPosition - 2, position.width, 3));
                    }
                }
            }
        }

        private void DrawDropIndicator(Rect position)
        {
            UIDrawUtility.Separator(position);
        }

        private void CheckForAutoFocusControl()
        {
            if (Event.current.type == EventType.Used)
            {
                return;
            }

            // Automatically focus control!
            if (autoFocusControlID == controlID)
            {
                autoFocusControlID = 0;
                UIDrawUtility.focusTextInControl("AutoFocus_" + controlID + "_" + autoFocusIndex);
                autoFocusIndex = -1;
            }
        }

        private void DrawFooterControls(Rect position, IReorderableListAdaptor adaptor)
        {
            if (HasFooterControls)
            {
                var buttonPosition = new Rect(position.xMax - 30, position.yMax - 1, 30, FooterButtonStyle.fixedHeight);
                var menuButtonPosition = buttonPosition;
                var menuIconNormal = ReorderableListResources.GetTexture(ReorderableListTexture.IconAddMenuNormal);
                var menuIconActive = ReorderableListResources.GetTexture(ReorderableListTexture.IconAddMenuActive);

                if (HasSizeField)
                {
                    // Draw size field.
                    var sizeFieldPosition = new Rect(
                        position.x,
                        position.yMax + 1,
                        Mathf.Max(150f, position.width / 3f),
                        16f
                    );

                    DrawSizeFooterControl(sizeFieldPosition, adaptor);
                }

                if (HasAddButton)
                {
                    // Draw add menu drop-down button.
                    if (HasAddMenuButton)
                    {
                        menuButtonPosition.x = buttonPosition.xMax - 14;
                        menuButtonPosition.xMax = buttonPosition.xMax;
                        menuIconNormal = ReorderableListResources.GetTexture(ReorderableListTexture.IconMenuNormal);
                        menuIconActive = ReorderableListResources.GetTexture(ReorderableListTexture.IconMenuActive);
                        buttonPosition.width -= 5;
                        buttonPosition.x = menuButtonPosition.x - buttonPosition.width + 1;
                    }

                    // Draw add item button.
                    var iconNormal = ReorderableListResources.GetTexture(ReorderableListTexture.IconAddNormal);
                    var iconActive = ReorderableListResources.GetTexture(ReorderableListTexture.IconAddActive);

                    if (UIDrawUtility.IconButton(buttonPosition, true, iconNormal, iconActive, FooterButtonStyle))
                    {
                        // Append item to list.
                        GUIUtility.keyboardControl = 0;
                        AddItem(adaptor);
                    }
                }

                if (HasAddMenuButton)
                {
                    // Draw add menu drop-down button.
                    if (UIDrawUtility.IconButton(menuButtonPosition, true, menuIconNormal, menuIconActive, FooterButtonStyle))
                    {
                        GUIUtility.keyboardControl = 0;
                        var totalAddButtonPosition = buttonPosition;
                        totalAddButtonPosition.xMax = position.xMax;
                        OnAddMenuClicked(new AddMenuClickedEventArgs(adaptor, totalAddButtonPosition));

                        // This will be helpful in many circumstances; including by default!
                        GUIUtility.ExitGUI();
                    }
                }
            }
        }

        private void DrawSizeFooterControl(Rect position, IReorderableListAdaptor adaptor)
        {
            var restoreLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 60f;

            DrawSizeField(position, new GUIContent("Size"), adaptor);

            EditorGUIUtility.labelWidth = restoreLabelWidth;
        }

        private Rect GetListRectWithAutoLayout(IReorderableListAdaptor adaptor, float padding)
        {
            float totalHeight;

            // Calculate position of list field using layout engine.
            if (Event.current.type == EventType.Layout)
            {
                totalHeight = CalculateListHeight(adaptor);
                containerHeightCache[controlID] = totalHeight;
            }
            else
            {
                totalHeight = containerHeightCache.ContainsKey(controlID) ? containerHeightCache[controlID] : 0;
            }

            totalHeight += padding;

            return GUILayoutUtility.GetRect(GUIContent.none, ContainerStyle, GUILayout.Height(totalHeight));
        }

        private Rect DrawLayoutListField(IReorderableListAdaptor adaptor, float padding)
        {
            var position = GetListRectWithAutoLayout(adaptor, padding);

            // Make room for footer buttons?
            if (HasFooterControls)
            {
                position.height -= FooterButtonStyle.fixedHeight;
            }

            // Make room for vertical spacing below footer buttons.
            position.height -= VerticalSpacing;

            currentListStack.Push(new ListInfo(controlID, position));
            try
            {
                // Draw list as normal.
                adaptor.BeginGUI();
                DrawListContainerAndItems(position, adaptor);
                HandleDropInsertion(position, adaptor);
                adaptor.EndGUI();
            }
            finally
            {
                currentListStack.Pop();
            }

            CheckForAutoFocusControl();

            return position;
        }

        private Rect DrawLayoutEmptyList(IReorderableListAdaptor adaptor, EmptyDrawer emptyDrawer)
        {
            var position = EditorGUILayout.BeginVertical(ContainerStyle);
            {
                if (emptyDrawer != null)
                {
                    emptyDrawer();
                }
                else
                {
                    Debug.LogError("Unexpected call to 'DrawLayoutEmptyList'");
                }

                currentListStack.Push(new ListInfo(controlID, position));
                try
                {
                    adaptor.BeginGUI();
                    insertionIndex = 0;
                    insertionPosition = position.y + 2;
                    HandleDropInsertion(position, adaptor);
                    adaptor.EndGUI();
                }
                finally
                {
                    currentListStack.Pop();
                }
            }
            EditorGUILayout.EndVertical();

            // Allow room for footer buttons?
            if (HasFooterControls)
            {
                GUILayoutUtility.GetRect(0, FooterButtonStyle.fixedHeight - 1);
            }

            return position;
        }

        private void DrawEmptyListControl(Rect position, EmptyDrawerRect emptyDrawer)
        {
            if (Event.current.type == EventType.Repaint)
            {
                ContainerStyle.Draw(position, GUIContent.none, false, false, false, false);
            }

            // Take padding into consideration when drawing empty content.
            position = ContainerStyle.padding.Remove(position);

            emptyDrawer?.Invoke(position);
        }

        private void FixStyles()
        {
            if (ContainerStyle == null)
            {
                ContainerStyle = ReorderableListStyles.Container;
            }

            if (FooterButtonStyle == null)
            {
                FooterButtonStyle = ReorderableListStyles.FooterButton;
            }

            if (ItemButtonStyle == null)
            {
                ItemButtonStyle = ReorderableListStyles.ItemButton;
            }

            if (rightAlignedLabelStyle == null)
            {
                rightAlignedLabelStyle = new GUIStyle(UnityEngine.GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleRight, padding =
                    {
                        right = 4
                    }
                };
            }
        }

        private void Draw(int id, IReorderableListAdaptor adaptor, EmptyDrawer emptyDrawer)
        {
            FixStyles();
            PrepareState(id, adaptor);

            Rect position;
            if (adaptor.Count > 0)
            {
                position = DrawLayoutListField(adaptor, 0f);
            }
            else if (emptyDrawer == null)
            {
                position = DrawLayoutListField(adaptor, 5f);
            }
            else
            {
                position = DrawLayoutEmptyList(adaptor, emptyDrawer);
            }

            DrawFooterControls(position, adaptor);
        }

        private void Draw(Rect position, int id, IReorderableListAdaptor adaptor, EmptyDrawerRect emptyDrawer)
        {
            FixStyles();
            PrepareState(id, adaptor);

            // Allow for footer area.
            if (HasFooterControls)
            {
                position.height -= FooterButtonStyle.fixedHeight;
            }

            // Make room for vertical spacing below footer buttons.
            position.height -= VerticalSpacing;

            currentListStack.Push(new ListInfo(controlID, position));
            try
            {
                adaptor.BeginGUI();

                DrawListContainerAndItems(position, adaptor);
                HandleDropInsertion(position, adaptor);
                CheckForAutoFocusControl();

                if (adaptor.Count == 0)
                {
                    ListUI.IndexOfChangedItem = -1;
                    DrawEmptyListControl(position, emptyDrawer);
                }

                adaptor.EndGUI();
            }
            finally
            {
                currentListStack.Pop();
            }

            DrawFooterControls(position, adaptor);
        }

        private void DrawSizeField(Rect position, GUIContent label, IReorderableListAdaptor adaptor)
        {
            var sizeControlID = GUIUtility.GetControlID(FocusType.Passive);
            var sizeControlName = "ReorderableListControl.Size." + sizeControlID;
            UnityEngine.GUI.SetNextControlName(sizeControlName);

            if (UnityEngine.GUI.GetNameOfFocusedControl() == sizeControlName)
            {
                if (Event.current.rawType == EventType.KeyDown)
                {
                    if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
                    {
                        ResizeList(adaptor, newSizeInput);
                        Event.current.Use();
                    }
                }

                newSizeInput = EditorGUI.IntField(position, label, newSizeInput);
            }
            else
            {
                EditorGUI.IntField(position, label, adaptor.Count);
                newSizeInput = adaptor.Count;
            }
        }

        #endregion

        #region Context Menu

        private static int contextControlID;
        private static int contextItemIndex;
        private static string contextCommandName;
        private static readonly GUIContent commandMoveToTop = new GUIContent("Move to Top");
        private static readonly GUIContent commandMoveToBottom = new GUIContent("Move to Bottom");
        private static readonly GUIContent commandInsertAbove = new GUIContent("Insert Above");
        private static readonly GUIContent commandInsertBelow = new GUIContent("Insert Below");
        private static readonly GUIContent commandDuplicate = new GUIContent("Duplicate");
        private static readonly GUIContent commandRemove = new GUIContent("Remove");
        private static readonly GUIContent commandClearAll = new GUIContent("Clear All");
        private static readonly GenericMenu.MenuFunction2 defaultContextHandler = DefaultContextMenuHandler;

        private void ShowContextMenu(int itemIndex, IReorderableListAdaptor adaptor)
        {
            var menu = new GenericMenu();

            contextControlID = controlID;
            contextItemIndex = itemIndex;

            AddItemsToMenu(menu, itemIndex, adaptor);

            if (menu.GetItemCount() > 0)
            {
                menu.ShowAsContext();
            }
        }

        private void AddItemsToMenu(GenericMenu menu, int itemIndex, IReorderableListAdaptor adaptor)
        {
            if ((Flags & ReorderableListFlags.DisableReordering) == 0)
            {
                if (itemIndex > 0)
                {
                    menu.AddItem(commandMoveToTop, false, defaultContextHandler, commandMoveToTop);
                }
                else
                {
                    menu.AddDisabledItem(commandMoveToTop);
                }

                if (itemIndex + 1 < adaptor.Count)
                {
                    menu.AddItem(commandMoveToBottom, false, defaultContextHandler, commandMoveToBottom);
                }
                else
                {
                    menu.AddDisabledItem(commandMoveToBottom);
                }

                if (HasAddButton)
                {
                    menu.AddSeparator("");

                    menu.AddItem(commandInsertAbove, false, defaultContextHandler, commandInsertAbove);
                    menu.AddItem(commandInsertBelow, false, defaultContextHandler, commandInsertBelow);

                    if ((Flags & ReorderableListFlags.DisableDuplicateCommand) == 0)
                    {
                        menu.AddItem(commandDuplicate, false, defaultContextHandler, commandDuplicate);
                    }
                }
            }

            if (HasRemoveButtons)
            {
                if (menu.GetItemCount() > 0)
                {
                    menu.AddSeparator("");
                }

                menu.AddItem(commandRemove, false, defaultContextHandler, commandRemove);
                menu.AddSeparator("");
                menu.AddItem(commandClearAll, false, defaultContextHandler, commandClearAll);
            }
        }

        private bool HandleCommand(string commandName, int itemIndex, IReorderableListAdaptor adaptor)
        {
            switch (commandName)
            {
                case "Move to Top":
                    MoveItem(adaptor, itemIndex, 0);
                    return true;
                case "Move to Bottom":
                    MoveItem(adaptor, itemIndex, adaptor.Count);
                    return true;
                case "Insert Above":
                    InsertItem(adaptor, itemIndex);
                    return true;
                case "Insert Below":
                    InsertItem(adaptor, itemIndex + 1);
                    return true;
                case "Duplicate":
                    DuplicateItem(adaptor, itemIndex);
                    return true;
                case "Remove":
                    RemoveItem(adaptor, itemIndex);
                    return true;
                case "Clear All":
                    ClearAll(adaptor);
                    return true;
                default:
                    return false;
            }
        }

        private bool DoCommand(string commandName, int itemIndex, IReorderableListAdaptor adaptor)
        {
            if (!HandleCommand(commandName, itemIndex, adaptor))
            {
                Debug.LogWarning("未知的菜单命令");
                return false;
            }

            return true;
        }

        private static void DefaultContextMenuHandler(object userData)
        {
            if (userData is GUIContent commandContent)
            {
                if (string.IsNullOrEmpty(commandContent.text))
                {
                    return;
                }

                contextCommandName = commandContent.text;
                EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent("ReorderableListContextCommand"));
            }
        }

        #endregion

        #region Methods

        public float CalculateListHeight(IReorderableListAdaptor adaptor)
        {
            FixStyles();

            var totalHeight = ContainerStyle.padding.vertical - 1 + VerticalSpacing;

            // Take list items into consideration.
            var count = adaptor.Count;
            for (var i = 0; i < count; ++i)
            {
                totalHeight += adaptor.GetItemHeight(i);
            }

            // Add spacing between list items.
            totalHeight += 4 * count;

            // Add height of footer buttons.
            if (HasFooterControls)
            {
                totalHeight += FooterButtonStyle.fixedHeight;
            }

            return totalHeight;
        }

        public float CalculateListHeight(int itemCount, float itemHeight)
        {
            FixStyles();

            var totalHeight = ContainerStyle.padding.vertical - 1 + VerticalSpacing;

            // Take list items into consideration.
            totalHeight += (itemHeight + 4) * itemCount;

            // Add height of footer buttons.
            if (HasFooterControls)
            {
                totalHeight += FooterButtonStyle.fixedHeight;
            }

            return totalHeight;
        }

        private void MoveItem(IReorderableListAdaptor adaptor, int sourceIndex, int destIndex)
        {
            // Raise event before moving item so that the operation can be cancelled.
            var movingEventArgs = new ItemMovingEventArgs(adaptor, sourceIndex, destIndex);
            OnItemMoving(movingEventArgs);

            if (!movingEventArgs.Cancel)
            {
                adaptor.Move(sourceIndex, destIndex);

                // Item was actually moved!
                var newIndex = destIndex;
                if (newIndex > sourceIndex)
                {
                    --newIndex;
                }

                OnItemMoved(new ItemMovedEventArgs(adaptor, sourceIndex, newIndex));

                UnityEngine.GUI.changed = true;
            }

            ListUI.IndexOfChangedItem = -1;
        }

        private void AddItem(IReorderableListAdaptor adaptor)
        {
            adaptor.Add();
            AutoFocusItem(contextControlID, adaptor.Count - 1);

            UnityEngine.GUI.changed = true;
            ListUI.IndexOfChangedItem = -1;

            var args = new ItemInsertedEventArgs(adaptor, adaptor.Count - 1, false);
            OnItemInserted(args);
        }

        private void InsertItem(IReorderableListAdaptor adaptor, int itemIndex)
        {
            adaptor.Insert(itemIndex);
            AutoFocusItem(contextControlID, itemIndex);

            UnityEngine.GUI.changed = true;
            ListUI.IndexOfChangedItem = -1;

            var args = new ItemInsertedEventArgs(adaptor, itemIndex, false);
            OnItemInserted(args);
        }

        private void DuplicateItem(IReorderableListAdaptor adaptor, int itemIndex)
        {
            adaptor.Duplicate(itemIndex);
            AutoFocusItem(contextControlID, itemIndex + 1);

            UnityEngine.GUI.changed = true;
            ListUI.IndexOfChangedItem = -1;

            var args = new ItemInsertedEventArgs(adaptor, itemIndex + 1, true);
            OnItemInserted(args);
        }

        private bool RemoveItem(IReorderableListAdaptor adaptor, int itemIndex)
        {
            var args = new ItemRemovingEventArgs(adaptor, itemIndex);
            OnItemRemoving(args);
            if (args.Cancel)
            {
                return false;
            }

            adaptor.Remove(itemIndex);

            UnityEngine.GUI.changed = true;
            ListUI.IndexOfChangedItem = -1;

            return true;
        }

        private bool ClearAll(IReorderableListAdaptor adaptor)
        {
            if (adaptor.Count == 0)
            {
                return true;
            }

            var args = new ItemRemovingEventArgs(adaptor, 0);
            var count = adaptor.Count;
            for (var index = 0; index < count; ++index)
            {
                args.ItemIndex = index;
                OnItemRemoving(args);
                if (args.Cancel)
                {
                    return false;
                }
            }

            adaptor.Clear();

            UnityEngine.GUI.changed = true;
            ListUI.IndexOfChangedItem = -1;

            return true;
        }

        private bool ResizeList(IReorderableListAdaptor adaptor, int newCount)
        {
            if (newCount < 0)
            {
                return true;
            }

            var removeCount = Mathf.Max(0, adaptor.Count - newCount);
            var addCount = Mathf.Max(0, newCount - adaptor.Count);

            while (removeCount-- > 0)
            {
                if (!RemoveItem(adaptor, adaptor.Count - 1))
                {
                    return false;
                }
            }

            while (addCount-- > 0)
            {
                AddItem(adaptor);
            }

            return true;
        }

        #endregion
    }
}
