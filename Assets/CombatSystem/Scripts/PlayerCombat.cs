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

	//	[Header("Player combat attributes")]
	//	[SerializeField] private float distanceThreshold;
	//	[SerializeField] private float detectionConeAngle;
	//	[SerializeField] private float detectionConeRadius;
	//	[SerializeField] private Transform detectionRayStartPoint;

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

	private CharacterController _characterController;
	private DamageRegister _damageRegister;
	private Transform enemyTarget;

	private void Start()
	{
		_damageRegister = GetComponent<DamageRegister>();
		_characterController = GetComponent<CharacterController>();

		_playerInput = InputProvider.GetPlayerInput();
		_block = _playerInput.actions["Block"];
		_basicAttack = _playerInput.actions["BasicAttack"];
		_heavyAttack = _playerInput.actions["HeavyAttack"];

	}

	private void Update()
	{
		if (_basicAttack.WasPressedThisFrame())
		{
			//EnemyDetection();
			LightAttack();
		}
		if (_heavyAttack.WasPerformedThisFrame())
		{
			//EnemyDetection();
			HeavyAttack();
		}
		if (isAttacking)
		{
			EndAttack();
		}
		Block();
	}

	//	private void EnemyDetection()
	//	{
	//		Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionConeRadius); // Overlap sphere to get all colliders within the radius

	//		foreach (var hitCollider in hitColliders)
	//		{
	//			if (hitCollider.CompareTag("Enemy"))
	//			{
	//				Vector3 toEnemy = hitCollider.transform.position - transform.position;
	//				float angleToEnemy = Vector3.Angle(detectionRayStartPoint.forward, toEnemy); // Calculate angle between forward and enemy direction

	//				if (angleToEnemy <= detectionConeAngle * 0.5f) // Check if the enemy is within the detection cone
	//				{
	//					enemyTarget = hitCollider.transform;
	//					_characterController.enabled = false;
	//					lerpStartPos = transform.position;

	//					lerpTarget = enemyTarget.position + ((transform.position - enemyTarget.position).normalized * distanceThreshold);

	//					lerpTimer = 0;
	//					lerpingToEnemy = true;

	//					break;
	//				}
	//			}
	//		}
	//	}

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
				_playerAnimator.SetTrigger("isAttacking");
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
