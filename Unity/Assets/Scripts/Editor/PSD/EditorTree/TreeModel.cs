using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;


// The TreeModel is a utility class working on a list of serializable TreeElements where the order and the depth of each TreeElement define
// the tree structure. Note that the TreeModel itself is not serializable (in Unity we are currently limited to serializing lists/arrays) but the 
// input list is.
// The tree representation (parent and children references) are then build internally using TreeElementUtility.ListToTree (using depth 
// values of the elements). 
// The first element of the input list is required to have depth == -1 (the hiddenroot) and the rest to have
// depth >= 0 (otherwise an exception will be thrown)

public class TreeModel<T> where T : TreeElement
{
	private IList<T> _data;
	private T _root;

	public T Root {
		get => _root;
		set => _root = value;
	}
	public event Action ModelChanged;
	public int NumberOfDataElements => _data.Count;
	public T[] Data => _data.ToArray();

	public TreeModel()
	{
		
	}
	
	public TreeModel (IList<T> data)
	{
		SetData (data);
	}

	public T Find (int id)
	{
		return _data.FirstOrDefault (element => element.Id == id);
	}

	public void SetData (IList<T> data)
	{
		Init (data);
	}

	void Init (IList<T> data)
	{
		if (data == null)
			throw new ArgumentNullException("data", "Input data is null. Ensure input is a non-null list.");

		_data = data;
		if (_data.Count > 0)
			_root = TreeElementUtility.ListToTree(data);
	}
	
	public IList<int> GetAncestors (int id)
	{
		var parents = new List<int>();
		TreeElement T = Find(id);
		if (T != null)
		{
			while (T.Parent != null)
			{
				parents.Add(T.Parent.Id);
				T = T.Parent;
			}
		}
		return parents;
	}

	public IList<int> GetDescendantsThatHaveChildren (int id)
	{
		T searchFromThis = Find(id);
		if (searchFromThis != null)
		{
			return GetParentsBelowStackBased(searchFromThis);
		}
		return new List<int>();
	}

	IList<int> GetParentsBelowStackBased(TreeElement searchFromThis)
	{
		Stack<TreeElement> stack = new Stack<TreeElement>();
		stack.Push(searchFromThis);

		var parentsBelow = new List<int>();
		while (stack.Count > 0)
		{
			TreeElement current = stack.Pop();
			if (current.HasChildren)
			{
				parentsBelow.Add(current.Id);
				foreach (var T in current.Children)
				{
					stack.Push(T);
				}
			}
		}

		return parentsBelow;
	}

	public void RemoveElements (IList<int> elementIDs)
	{
		IList<T> elements = _data.Where (element => elementIDs.Contains (element.Id)).ToArray ();
		RemoveElements (elements);
	}

	public void RemoveElements (IList<T> elements)
	{
		foreach (var element in elements)
			if (element == _root)
				throw new ArgumentException("It is not allowed to remove the root element");
	
		var commonAncestors = TreeElementUtility.FindCommonAncestorsWithinList (elements);

		foreach (var element in commonAncestors)
		{
			element.Parent.Children.Remove (element);
			element.Parent = null;
		}

		TreeElementUtility.TreeToList(_root, _data);

		Changed();
	}

	public void AddElements (IList<T> elements, TreeElement parent, int insertPosition)
	{
		if (elements == null)
			throw new ArgumentNullException("elements", "elements is null");
		if (elements.Count == 0)
			throw new ArgumentNullException("elements", "elements Count is 0: nothing to add");
		if (parent == null)
			throw new ArgumentNullException("parent", "parent is null");

		if (parent.Children == null)
			parent.Children = new List<TreeElement>();

		parent.Children.InsertRange(insertPosition, elements.Cast<TreeElement> ());
		foreach (var element in elements)
		{
			element.Parent = parent;
			element.Depth = parent.Depth + 1;
			TreeElementUtility.UpdateDepthValues(element);
		}

		TreeElementUtility.TreeToList(_root, _data);

		Changed();
	}

	public void AddRoot (T root)
	{
		if (root == null)
			throw new ArgumentNullException("root", "root is null");

		if (_data == null)
			throw new InvalidOperationException("Internal Error: data list is null");

		if (_data.Count != 0)
			throw new InvalidOperationException("AddRoot is only allowed on empty data list");

		root.Depth = -1;
		_data.Add (root);
	}

	public void AddElement (T element, TreeElement parent, int insertPosition)
	{
		if (element == null)
			throw new ArgumentNullException("element", "element is null");
		if (parent == null)
			throw new ArgumentNullException("parent", "parent is null");
	
		if (parent.Children == null)
			parent.Children = new List<TreeElement> ();

		parent.Children.Insert (insertPosition, element);
		element.Parent = parent;

		TreeElementUtility.UpdateDepthValues(parent);
		TreeElementUtility.TreeToList(_root, _data);

		Changed ();
	}

	public void MoveElements(TreeElement parentElement, int insertionIndex, List<TreeElement> elements)
	{
		if (insertionIndex < 0)
			throw new ArgumentException("Invalid input: insertionIndex is -1, client needs to decide what index elements should be reparented at");

		// Invalid reparenting input
		if (parentElement == null)
			return;

		// We are moving items so we adjust the insertion index to accomodate that any items above the insertion index is removed before inserting
		if (insertionIndex > 0)
			insertionIndex -= parentElement.Children.GetRange(0, insertionIndex).Count(elements.Contains);

		// Remove draggedItems from their parents
		foreach (var draggedItem in elements)
		{
			draggedItem.Parent.Children.Remove(draggedItem);	// remove from old parent
			draggedItem.Parent = parentElement;					// set new parent
		} 

		if (parentElement.Children == null)
			parentElement.Children = new List<TreeElement>();

		// Insert dragged items under new parent
		parentElement.Children.InsertRange(insertionIndex, elements);

		TreeElementUtility.UpdateDepthValues (Root);
		TreeElementUtility.TreeToList (_root, _data);

		Changed ();
	}

	void Changed ()
	{
		if (ModelChanged != null)
			ModelChanged ();
	}
}

