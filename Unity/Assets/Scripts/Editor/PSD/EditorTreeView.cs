using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.TreeViewExamples;
using UnityEngine;
using Object = System.Object;

namespace XGame
{
    public class EditorTreeView<T> : TreeView where T : TreeElement
    {
        public event Action<TreeViewItemRow<T>> OnDrawRowCallback;
        public event Func<TreeViewItem, List<TreeViewItem>, bool> OnDropVerify;
        public event Action<int> OnContextClickedItem;
        public event Action<int> OnSingleClickedItem;
        public event Action<int> OnDoubleClickItem;
        public event Action<IList<TreeViewItem>>  BeforeDroppingDraggedItems;
        public event Action<T, List<TreeViewItem>>  OnMoveElementsResult; 
        
        public TreeModel<T> Data => _treeModel;

        public bool Inited => _inited;

        private readonly List<TreeViewItem> _rows = new List<TreeViewItem>(100);
        private TreeModel<T> _treeModel;
        private bool _inited;
        
        public EditorTreeView(TreeViewState state) : base(state)
        {
	        showAlternatingRowBackgrounds = true;
	        rowHeight = 30;
	        customFoldoutYOffset = -5;
	        showBorder = true;
        }
        
        public EditorTreeView(TreeViewState state, TreeModel<T> treeModel) : base(state)
        {
            showAlternatingRowBackgrounds = true;
            rowHeight = 30;
            customFoldoutYOffset = -5;
            showBorder = true;
            _treeModel = treeModel;
            _inited = true;
            Reload();
        }

        public TreeViewItem<T>[] GetViewCells()
        {
	        GetFirstAndLastVisibleRows(out var first, out var last);
	        var res = new TreeViewItem<T>[last - first + 1];
	        for (int i = 0; i < res.Length; i++)
	        {
		        res[i] = (TreeViewItem<T>)_rows[first + i];
	        }

	        return res;
        }

        public new void Reload()
        {
	        if (_inited)
	        {
				base.Reload();
	        }
        }
        
        public void Build(TreeModel<T> model)
        {
	        _treeModel = model;
	        _inited = true;
	        Reload();
        }

        protected override TreeViewItem BuildRoot()
        {
	        if (!_inited)
		        return null;
	        int depthForHiddenRoot = -1;
	        return new TreeViewItem<T>(_treeModel.Root.Id, depthForHiddenRoot, _treeModel.Root.Name, _treeModel.Root);
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
	        if (_treeModel.Root == null)
	        {
		        Debug.LogError ("tree model root is null. did you call SetData()?");
	        }

	        _rows.Clear ();
	
	        if (_treeModel.Root.HasChildren)
		        AddChildrenRecursive(_treeModel.Root, 0, _rows);
	        // We still need to setup the child parent information for the rows since this 
	        // information is used by the TreeView internal logic (navigation, dragging etc)
	        SetupParentsAndChildrenFromDepths (root, _rows);

	        return _rows;
        }
        
        private void AddChildrenRecursive (T parent, int depth, IList<TreeViewItem> newRows)
        {
	        foreach (T child in parent.Children)
	        {
		        var item = new TreeViewItem<T>(child.Id, depth, child.Name, child);
		        newRows.Add(item);

		        if (child.HasChildren)
		        {
			        if (IsExpanded(child.Id))
			        {
				        AddChildrenRecursive (child, depth + 1, newRows);
			        }
			        else
			        {
				        item.children = CreateChildListForCollapsedParent();
			        }
		        }
	        }
        }

        public override void OnGUI(Rect rect)
        {
	        if (_inited)
				base.OnGUI(rect);
        }

        protected override IList<int> GetAncestors (int id)
        {
	        return _treeModel.GetAncestors(id);
        }

        protected override IList<int> GetDescendantsThatHaveChildren (int id)
        {
	        return _treeModel.GetDescendantsThatHaveChildren(id);
        }
        
        protected override void RowGUI(RowGUIArgs args)
        {
            OnDrawRowCallback?.Invoke(new TreeViewItemRow<T>()
            {
                item = (TreeViewItem<T>) args.item,
                label = args.label,
                rowRect = args.rowRect,
                row = args.row,
                selected = args.selected,
                focused = args.focused,
                isRenaming = args.isRenaming,
                GetCellRect = args.GetCellRect,
                GetColumn = args.GetColumn,
                GetNumVisibleColumns = args.GetNumVisibleColumns,
                GetContentIndent = GetContentIndent
            });
        }

        protected override void ContextClickedItem(int id)
        {
            OnContextClickedItem?.Invoke(id);
        }

        protected override void SingleClickedItem(int id)
        {
            OnSingleClickedItem?.Invoke(id);
        }

        protected override void DoubleClickedItem(int id)
        {
	        OnDoubleClickItem?.Invoke(id);
        }

        const string k_GenericDragID = "GenericDragColumnDragging";

		protected override bool CanStartDrag (CanStartDragArgs args)
		{
			return true;
		}

		protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
		{
			if (hasSearch)
				return;

			DragAndDrop.PrepareStartDrag();
			var draggedRows = GetRows().Where(item => args.draggedItemIDs.Contains(item.id)).ToList();
			DragAndDrop.SetGenericData(k_GenericDragID, draggedRows);
			DragAndDrop.objectReferences = new UnityEngine.Object[] { }; // this IS required for dragging to work
			string title = draggedRows.Count == 1 ? draggedRows[0].displayName : "< Multiple >";
			DragAndDrop.StartDrag (title);
		}

		protected override DragAndDropVisualMode HandleDragAndDrop (DragAndDropArgs args)
		{
			// Check if we can handle the current drag data (could be dragged in from other areas/windows in the editor)
			var draggedRows = DragAndDrop.GetGenericData(k_GenericDragID) as List<TreeViewItem>;
			if (draggedRows == null)
				return DragAndDropVisualMode.None;

			// Parent item is null when dragging outside any tree view items.
			switch (args.dragAndDropPosition)
			{
				case DragAndDropPosition.UponItem:
				case DragAndDropPosition.BetweenItems:
					{
						bool validDrag = ValidDrag(args.parentItem, draggedRows);
						if (args.performDrop && validDrag)
						{
							T parentData = ((TreeViewItem<T>)args.parentItem).Data;
							OnDropDraggedElementsAtIndex(draggedRows, parentData, args.insertAtIndex == -1 ? 0 : args.insertAtIndex);
							Reload();
						}
						return validDrag ? DragAndDropVisualMode.Move : DragAndDropVisualMode.None;
					}

				case DragAndDropPosition.OutsideItems:
				{
					var valid = OnDropVerify?.Invoke(args.parentItem, draggedRows) ?? true;
					if (args.performDrop && valid)
					{
						OnDropDraggedElementsAtIndex(draggedRows, _treeModel.Root, _treeModel.Root.Children.Count);
						Reload();
					}

					return valid ? DragAndDropVisualMode.Move : DragAndDropVisualMode.None;
				}
				default:
					Debug.LogError("Unhandled enum " + args.dragAndDropPosition);
					return DragAndDropVisualMode.None;
			}
		}

		public virtual void OnDropDraggedElementsAtIndex (List<TreeViewItem> draggedRows, T parent, int insertIndex)
		{
			if (BeforeDroppingDraggedItems != null)
				BeforeDroppingDraggedItems(draggedRows);

			var draggedElements = new List<TreeElement> ();
			foreach (var x in draggedRows)
				draggedElements.Add (((TreeViewItem<T>) x).Data);
		
			var selectedIDs = draggedElements.Select (x => x.Id).ToArray();
			_treeModel.MoveElements (parent, insertIndex, draggedElements);
			SetSelection(selectedIDs, TreeViewSelectionOptions.RevealAndFrame);
			OnMoveElementsResult?.Invoke(parent, draggedRows);
		}


		bool ValidDrag(TreeViewItem parent, List<TreeViewItem> draggedItems)
		{
			TreeViewItem currentParent = parent;
			while (currentParent != null)
			{
				if (draggedItems.Contains(currentParent))
					return false;
				currentParent = currentParent.parent;
			}

			return OnDropVerify?.Invoke(parent, draggedItems) ?? true;
		}
    }

    public class TreeViewItem<T> : TreeViewItem where T : TreeElement
    {
        public TreeViewItem(int id, int depth, string displayName, T data) : base(id, depth, displayName)
        {
            Data = data;
        }
        public T Data;
    }

    public class TreeViewMultiColumnHeader : MultiColumnHeader
    {
        public TreeViewMultiColumnHeader(MultiColumnHeaderState state) : base(state)
        {
        }

        protected override void ColumnHeaderGUI(MultiColumnHeaderState.Column column, Rect headerRect, int columnIndex)
        {
            base.ColumnHeaderGUI(column, headerRect, columnIndex);
        }
    }
    
    public struct TreeViewItemRow<T> where T : TreeElement
    {
        public TreeViewItem<T> item;
        public string label;
        public Rect rowRect;
        public int row;
        public bool selected;
        public bool focused;
        public bool isRenaming;
        public Func<int, Rect> GetCellRect;
        public Func<int> GetNumVisibleColumns;
        public Func<int, int> GetColumn;
        public Func<TreeViewItem, float> GetContentIndent;
    }
}