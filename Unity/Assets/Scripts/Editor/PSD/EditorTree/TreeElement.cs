using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class TreeElement
{
	[SerializeField] 
	private int _ID;
	[SerializeField] 
	private string _name;
	[SerializeField]
	private int _depth;
	[NonSerialized]
	private TreeElement _parent;
	[NonSerialized]
	private List<TreeElement> _children;

	public int Depth
	{
		get => _depth;
		set => _depth = value;
	}

	public TreeElement Parent
	{
		get => _parent;
		set => _parent = value;
	}

	public List<TreeElement> Children
	{
		get => _children;
		set => _children = value;
	}

	public bool HasChildren => Children != null && Children.Count > 0;

	public virtual string Name
	{
		get => _name;
		set => _name = value;
	}

	public virtual int Id
	{
		get => _ID;
		set => _ID = value;
	}

	public TreeElement ()
	{
	}

	public TreeElement (string name, int depth, int id)
	{
		_name = name;
		_ID = id;
		_depth = depth;
	}
}



