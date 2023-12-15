using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerCombat : MonoBehaviour
{
	[Header("Attack Scriptable Objects - Attacks animations for light and heavy combo")]
	public List<AttackSO> lightAttack;
	public List<AttackSO> heavyAttack;

	[SerializeField] HitReactionSO normalHitReactionAnimOverride;
	[SerializeField] HitReactionSO blockHitReactionAnimOverride;

	[Header("Player references")]
	[SerializeField] private Animator _playerAnimator;
	[SerializeField] private WeaponHandler _weaponHandler;

	private float lastClickedTime;
	private float lastComboTime;

	private int comboCounter;

	private bool isAttacking = false;
	private bool isBlocking = false;

	private PlayerInput _playerInput;
	private InputAction _basicAttack;
	private InputAction _heavyAttack;
	private InputAction _block;

	private DamageRegister _damageRegister;

	private void Start()
	{
		_damageRegister = GetComponent<DamageRegister>();
		_playerInput = InputProvider.GetPlayerInput();
		_block = _playerInput.actions["Block"];
		_basicAttack = _playerInput.actions["BasicAttack"];
		_heavyAttack = _playerInput.actions["HeavyAttack"];

		CheckReferences();
	}

	private void Update()
	{
		if (!_playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerIntro"))
		{
			if (_basicAttack.WasPressedThisFrame())
			{
				LightAttack();
			}
			if (_heavyAttack.WasPerformedThisFrame())
			{
				HeavyAttack();
			}
		}
		if (isAttacking)
		{
			EndAttack();
		}

		Block();
	}

	private void CheckReferences()
	{
		// Checking if required references are assigned
		if (_playerAnimator == null)
		{
			Debug.LogError("Player Animator reference not assigned in PlayerCombat script!");
		}

		if (_weaponHandler == null)
		{
			Debug.LogError("Weapon Handler reference not assigned in PlayerCombat script!");
		}
	}

	//	//Light attack combo count and reset
	void LightAttack()
	{
		if (Time.time - lastClickedTime > 0.5f && comboCounter <= lightAttack.Count)
		{
			CancelInvoke("EndCombo");

			if (Time.time - lastComboTime >= 0.2f)
			{
				isAttacking = true;
				_playerAnimator.runtimeAnimatorController = lightAttack[comboCounter]._AOVController;
				_playerAnimator.Play("Attack", 0, 0);
				_weaponHandler.SetAttackDamage(lightAttack[comboCounter].damage);
				comboCounter++;
				lastClickedTime = Time.time;

				ComboCounterReset();
			}
		}
	}

	//	//Heavy attack combo count and reset
	void HeavyAttack()
	{
		if (Time.time - lastClickedTime > 0.5f && comboCounter <= heavyAttack.Count)
		{
			CancelInvoke("EndCombo");

			if (Time.time - lastComboTime >= 0.2f)
			{
				isAttacking = true;
				_playerAnimator.runtimeAnimatorController = heavyAttack[comboCounter]._AOVController;
				_playerAnimator.Play("Attack", 0, 0);
				_weaponHandler.SetAttackDamage(heavyAttack[comboCounter].damage);
				comboCounter++;
				lastClickedTime = Time.time;

				ComboCounterReset();
			}
		}
	}

	void ComboCounterReset()
	{
		if (comboCounter >= lightAttack.Count || comboCounter >= heavyAttack.Count)
		{
			comboCounter = 0;
		}
	}

	//	//Resetting runtime damage to 0;
	void ResetDamage()
	{
		_weaponHandler.SetWeaponDamageOnRuntime(0f);
	}

	void EndAttack()
	{
		Invoke("EndCombo", 1);
	}

	//Called through Invoke in EndAttack()
	void EndCombo()
	{
		isAttacking = false;
		comboCounter = 0;
		ResetDamage();
		lastComboTime = Time.time;
	}

	void Block()
	{
		if (_block.WasPressedThisFrame())
		{
			isBlocking = true;

			_playerAnimator.runtimeAnimatorController = blockHitReactionAnimOverride.hitReactionOverrideController;

			_playerAnimator.SetBool("isBlocking", true);
			_playerAnimator.SetTrigger("blockTrigger");
		}
		else if (_block.WasReleasedThisFrame())
		{
			isBlocking = false;

			_playerAnimator.runtimeAnimatorController = normalHitReactionAnimOverride.hitReactionOverrideController;

			_playerAnimator.ResetTrigger("blockTrigger");
			_playerAnimator.SetBool("isBlocking", false);
		}

		if (_damageRegister != null)
		{
			_damageRegister.playerBlock = isBlocking;
		}
	}

	public bool GetCurrentBlockState()
	{
		return isBlocking;
	}

	//isAttacking flag
	public bool GetCurrentAttackState()
	{
		return isAttacking;
	}
}
