using System.Collections;
using System;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

public class DamageRegister : MonoBehaviour
{
	private float damageRecieved;

	public bool damageTaken { get; private set; }

	public bool playerBlock = false;

	public Action <float> OnDamageApply;
	public Action OnResetDamage;

	private void Awake()
	{
		damageTaken = false;
	}

	//Coroutine to reset hit reacgtion animation and damageTaken flag
	IEnumerator ResetDamageTaken()
	{	
		yield return new WaitForEndOfFrame();

		OnResetDamage?.Invoke();

		damageTaken = false;
	}

	//Function used to apply damage to entity this script is on
	public void ApplyDamage(float recievedDamage)
	{
		CalculateDamageRecieved(recievedDamage);
		damageTaken = true;

		//Invoking the declared Action
		OnDamageApply?.Invoke(recievedDamage);

		StartCoroutine(ResetDamageTaken());
	}

	private float CalculateDamageRecieved(float recievedDamage)
	{
		if(!playerBlock) 
		{
			damageRecieved = recievedDamage;
		}
		else if(playerBlock) 
		{
			damageRecieved = 0f;
		}
		return damageRecieved;
	}

}
