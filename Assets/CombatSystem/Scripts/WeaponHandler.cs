using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
	[SerializeField] private float baseWeaponDamage;
	[SerializeField] private float weaponCounterDamage;

	private float weaponDamageOnRuntime;
	private bool isDamagingEntity = false;
	
	private void OnTriggerEnter(Collider other)
	{
		if (!other.gameObject.CompareTag("Player") && this.GetComponent<BoxCollider>().enabled)
		{
			isDamagingEntity = true;
			DamageRegister damageRegister = other.gameObject.GetComponent<DamageRegister>();					
			if(damageRegister != null )
			{
				//Apply damage to enemy that was hit
				damageRegister.ApplyDamage(weaponDamageOnRuntime);
			}
		}
		else
		{
			isDamagingEntity = false;
		}
	}

	public void SetWeaponDamageOnRuntime(float damage)
	{
		weaponDamageOnRuntime = damage;
	}

	public void SetAttackDamage(float damage)
	{
		weaponDamageOnRuntime = damage + baseWeaponDamage;
	}

	public float GetWeaponDamage()
	{
		if(isDamagingEntity)
		{
			return weaponDamageOnRuntime;
		}
		else
		{
			return weaponDamageOnRuntime = 0;
		}
	}
}
