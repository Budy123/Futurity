using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class CommandGraphView : GraphView
{
    public CommandGraphView()
	{
		AddManipulators();

		AddGridBackground();

		//CreateNode();

		AddStyles();
	}

	private void AddManipulators()
	{
		// ���� �ܾƿ� ���
		SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

		// ��ư �߰� ���
		this.AddManipulator(CreateNodeContextualMenu("Add Node (Normal Attack)", CSCommandType.NormalAttack));
		this.AddManipulator(CreateNodeContextualMenu("Add Node (Charged Attack)", CSCommandType.ChargedAttack));
		this.AddManipulator(CreateNodeContextualMenu("Add Node (Dash)", CSCommandType.Dash));

		// ���õ� ��� �巡�� �̵�
		// RectangleSelector���� ���߿� Add�Ǹ� ���� �������� ����
		this.AddManipulator(new SelectionDragger());

		// �׷��� ��� ���� ���
		this.AddManipulator(new RectangleSelector());

		// �巡�� ���
		this.AddManipulator(new ContentDragger());
	}

	private IManipulator CreateNodeContextualMenu(string actionTitle, CSCommandType commandType)
	{
		ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
				menuEvent => 
					menuEvent.menu.AppendAction
						(
							actionTitle, 
							actionEvent => AddElement(CreateNode(commandType, actionEvent.eventInfo.localMousePosition))
						)
			);

		return contextualMenuManipulator;
	}

	private CSNode CreateNode(CSCommandType commandType, Vector2 position)
	{
		Type nodeType = Type.GetType($"CS{commandType}Node");

		CSNode node = (CSNode)Activator.CreateInstance(nodeType);

		node.Initialize(position);
		node.Draw();

		AddElement(node);

		return node;
	}

	private void AddGridBackground()
	{
		GridBackground gridBackground = new GridBackground();

		gridBackground.StretchToParentSize();

		Insert(0, gridBackground);
	}

	private void AddStyles()
	{
		StyleSheet graphStyleSheet = (StyleSheet)EditorGUIUtility.Load("CommandSystem/CommandGraphViewStyles.uss");
		StyleSheet nodeStyleSheet = (StyleSheet)EditorGUIUtility.Load("CommandSystem/CSNodeStyles.uss");

		styleSheets.Add(graphStyleSheet);
		styleSheets.Add(nodeStyleSheet);
	}
	
}
