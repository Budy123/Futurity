using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using UnityEngine.XR;


public class PlayerController : UnitFSM<PlayerController>, IFSM
{
	public enum PlayerState : int
	{
		Idle,           // 대기
		Attack,         // 공격
			NormalAttack,	// 일반공격 
			ChargedAttack,	// 차지공격
		AttackDelay,    // 공격 후 딜레이
		Hit,            // 피격
		Move,           // 이동
		Dash,           // 대시
		Stun,           // 기절
		Death,          // 사망
	}

	public enum PlayerInput : int
	{
		None,
		NormalAttack,
		SpecialAttack,
		Dash
	}

	// reference
	public Player playerData;
	[HideInInspector] public Animator animator;
	[HideInInspector] public Rigidbody rigid;
	[HideInInspector] public TrailRenderer dashEffect;

	// move
	//public Vector3 moveInput;
	public Vector3 moveDir;

	// attack
	public PlayerInput curCombo;
	public AttackNode curNode;
	public Tree comboTree;
	public RadiusCapsuleCollider attackCollider;

	[HideInInspector] public bool isComboState = false;
	[HideInInspector] public bool isAttacking = false;

	//임시
	public GameObject glove;

	// sound 
	public FMODUnity.EventReference dash;
	public FMODUnity.EventReference hitMelee;
	public FMODUnity.EventReference hitRanged;

	private void Start()
	{
		animator = GetComponent<Animator>();
		rigid = GetComponent<Rigidbody>();
		dashEffect = GetComponent<TrailRenderer>();

		SetUp(PlayerState.Idle);
		unit = this;
		curNode = comboTree.top;
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		//if (IsCurrentState(PlayerState.Hit) || IsCurrentState(PlayerState.Stun))
		//	return;

		Vector3 input = context.ReadValue<Vector3>();
		if (input != null)
		{
			moveDir = new Vector3(input.x, 0f, input.y);

			if (!IsCurrentState(PlayerState.Move))
			{
				ChangeState(PlayerState.Move);
			}
		}
	}

	public void OnDash(InputAction.CallbackContext context)
	{
		/*if (IsCurrentState(PlayerState.Hit) || IsCurrentState(PlayerState.Stun))
			return;
*/
		if (context.performed)
		{
			if (!IsCurrentState(PlayerState.Dash))
			{
				curNode = comboTree.top;
				ChangeState(PlayerState.Dash);
			}
		}
	}

	public void OnNormalAttack(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			AttackNode node = FindInput(PlayerInput.NormalAttack);
			FDebug.Log("In");
			if (node != null && !IsCurrentState(PlayerState.Attack))
			{
				curNode = node;
				curCombo = node.command;
				ChangeState(PlayerState.Attack);
			}
		}
	}

	public void OnSpecialAttack(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			// 현재 노드가 top노드인지 체크
			// top노드라는 건, 콤보 입력 중이 아니라는 것.
			bool isInit = curNode == comboTree.top;

			AttackNode node = FindInput(PlayerInput.SpecialAttack);
			if (node != null )
			{
				curNode = node;
				curCombo = node.command;

				if (isInit) // 콤보 입력 중이 아니면 차지
				{
					ChangeState(PlayerState.ChargedAttack);
				}
				else        // 콤보 입력 중이면 일반
				{
					ChangeState(PlayerState.NormalAttack);
				}
			}
		}
	}

	public AttackNode FindInput(PlayerInput input)
	{
		AttackNode node = comboTree.FindNode(input, curNode);

		if (node == null)
		{
			node = comboTree.FindNode(input, comboTree.top);
		}

		return node;
	}
}