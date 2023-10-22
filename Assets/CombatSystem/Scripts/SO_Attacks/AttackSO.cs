using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Attacks/Light or Heavy Attack")]
public class AttackSO : ScriptableObject
{
	public AnimatorOverrideController _AOVController;
	public float damage;
}
