﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRange : MonoBehaviour
{
	Projector projector;
	Zemmer zemmer;

	public float speed = 5f;
	public float viewRadius;
	public LayerMask playerMask;

	void Awake()
	{
		projector = GetComponent<Projector>();
		if(zemmer == null) zemmer = FindObjectOfType<Zemmer>();
		StartCoroutine(Init());
	}

	IEnumerator Init()
	{
		while (projector.orthographicSize < 7f)
		{
			projector.orthographicSize = Mathf.Clamp(projector.orthographicSize += speed * Time.deltaTime, 0f, 7f);
			yield return null;
		}
		StartCoroutine(ZemmerUpdate());
		yield return null;
	}

	IEnumerator ZemmerUpdate()
	{
		while (true)
		{
			if (!zemmer.activate)
			{
				projector.orthographicSize = Mathf.Clamp(projector.orthographicSize += speed * Time.deltaTime, 3f, 7f);
			}

			else
			{
				Collider[] playerInRadius = Physics.OverlapSphere(transform.position, viewRadius, playerMask);

				if (playerInRadius.Length != 0)
				{
					if (projector.orthographicSize > 3f)
						projector.orthographicSize -= speed * Time.deltaTime;
				}

				else if (playerInRadius.Length == 0)
				{
					if (projector.orthographicSize < 7f)
						projector.orthographicSize += speed * Time.deltaTime;
				}

				projector.orthographicSize = Mathf.Clamp(projector.orthographicSize, 3f, 7f);
			}

			yield return null;
		}
	}

	//private void OnDrawGizmosSelected()
	//{
	//	Gizmos.color = Color.red;
	//	Gizmos.DrawWireSphere(this.transform.position, viewRadius);
	//}
}