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
	private CSSearchWindow searchWindow;
	private CommandEditorWindow editorWindow;

	public CommandGraphView(CommandEditorWindow editorWindow)
	{
		this.editorWindow = editorWindow;

		AddSearchWindow();
		AddManipulators();
		AddGridBackground();

		AddStyles();
	}

	#region Overrided Methods
	public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
	{
		List<Port> compatiblePorts = new List<Port>();

		ports.ForEach(port =>
		{
			if(startPort == port)
			{
				return;
			}

			if(startPort.node == port.node)
			{
				return;
			}

			if(startPort.direction == port.direction)
			{
				return;
			}

			compatiblePorts.Add(port);
		});

		return compatiblePorts;
	}
	#endregion

	#region Mainpulators
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

		this.AddManipulator(CreateGroupContextMenu());
	}

	private IManipulator CreateGroupContextMenu()
	{
		ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
				menuEvent =>
					menuEvent.menu.AppendAction
						(
							"Add Group",
							actionEvent => AddElement(CreateGroup("DialogueGroup", GetLocalMousePosition(actionEvent.eventInfo.localMousePosition)))
						)
			);

		return contextualMenuManipulator;
	}

	private IManipulator CreateNodeContextualMenu(string actionTitle, CSCommandType commandType)
	{
		ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
				menuEvent => 
					menuEvent.menu.AppendAction
						(
							actionTitle, 
							actionEvent => AddElement(CreateNode(commandType, GetLocalMousePosition(actionEvent.eventInfo.localMousePosition)))
						)
			);

		return contextualMenuManipulator;
	}
	#endregion

	#region Elements Creation
	public Group CreateGroup(string title, Vector2 localMousePosition)
	{
		Group group = new Group()
		{
			title = title
		};

		group.SetPosition(new Rect(localMousePosition, Vector2.zero));

		return group;
	}

	public CSNode CreateNode(CSCommandType commandType, Vector2 position)
	{
		Type nodeType = Type.GetType($"CS{commandType}Node");

		CSNode node = (CSNode)Activator.CreateInstance(nodeType);

		node.Initialize(position);
		node.Draw();

		AddElement(node);

		return node;
	}
	#endregion

	#region Elements Addition
	private void AddSearchWindow()
	{
		if (searchWindow == null)
		{
			searchWindow = ScriptableObject.CreateInstance<CSSearchWindow>();

			searchWindow.Initialize(this);
		}

		nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
	}

	private void AddGridBackground()
	{
		GridBackground gridBackground = new GridBackground();

		gridBackground.StretchToParentSize();

		Insert(0, gridBackground);
	}

	private void AddStyles()
	{
		this.AddStyleSheets(
			"CommandSystem/CommandGraphViewStyles.uss",
			"CommandSystem/CSNodeStyles.uss"
		);
	}
	#endregion

	#region Utilites
	public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false)
	{
		Vector2 worldMousePosition = mousePosition;

		if(isSearchWindow)
		{
			worldMousePosition -= editorWindow.position.position;
		}

		Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);

		return localMousePosition;
	}
	#endregion
}
