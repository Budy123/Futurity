using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public static class CSElementUtility
{
	public static Foldout CreateFoldout(string title, bool collapsed = false)
	{
		Foldout foldout = new Foldout()
		{
			text = title,
			value = !collapsed
		};

		return foldout;
	}

	public static Port CreatePort(this CSNode node, string portName = "", Orientation orientation = Orientation.Horizontal, Direction direction = Direction.Output, Port.Capacity capacity = Port.Capacity.Single)
	{
		Port port = node.InstantiatePort(orientation, direction, capacity, typeof(bool));

		port.portName = portName;

		return port;
	}

	public static TextField CreateTextField(string value = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
	{
		TextField textField = new TextField()
		{
			value = value
		};

		if (onValueChanged != null)
		{
			textField.RegisterValueChangedCallback(onValueChanged);
		}

		return textField;
	}

	public static TextField CreateTextArea(string value = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
	{
		TextField textArea = CreateTextField(value, onValueChanged);

		textArea.multiline = true;

		return textArea;
	}
}
