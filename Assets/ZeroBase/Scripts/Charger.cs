﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charger : MonoBehaviour
{
	Animator anim;
	Rigidbody rgbd;

	public Transform target;
	public float runSpeed;
	public float turnSpeed;
	public float lookTurnSpeed = 40f;
	float runLerpSpeed;

	void Awake()
	{
		anim = GetComponent<Animator>();
		rgbd = GetComponent<Rigidbody>();
		StartCoroutine(FindAndScream(target.transform.position));
	}

	void Update()
	{
		AnimatorStateInfo animStateInfo = anim.GetCurrentAnimatorStateInfo(0);

		if (animStateInfo.IsName("Run"))
		{
			Vector3 dirToLookTarget = (target.transform.position - this.transform.position).normalized;
			float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

			if (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
			{
				float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, lookTurnSpeed * Time.deltaTime);
				transform.eulerAngles = Vector3.up * angle;
			}

			runSpeed = Mathf.Lerp(0, 15f, runLerpSpeed);
			runLerpSpeed += Time.deltaTime;
			transform.Translate(Vector3.forward * runSpeed * Time.deltaTime);
		}
	}

	bool fallbackTrigger;

	void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag("Wall"))
		{
			if (fallbackTrigger == false) {
				StartCoroutine(Fallback());
				fallbackTrigger = true;
			}
		}
	}

	IEnumerator Fallback()
	{
		Debug.Log("I Hit Wall");
		anim.SetTrigger("HitWall");
		runSpeed = 0f;
		runLerpSpeed = 0;

		float tempSpeed = 4f;
		float lerpSpeed = 1.4f;
		float lerpTime = 0;

		while (tempSpeed > 0f)
		{
			transform.Translate(Vector3.back * tempSpeed * Time.deltaTime);
			tempSpeed = Mathf.Lerp(8f, 0, lerpTime);
			lerpTime += lerpSpeed * Time.deltaTime;
			yield return null;
		}

		rgbd.constraints = RigidbodyConstraints.FreezePosition;

		yield return new WaitForSeconds(1f);

		rgbd.constraints = RigidbodyConstraints.None;
		rgbd.constraints = RigidbodyConstraints.FreezePositionY;

		anim.SetTrigger("Turn");
		StopAllCoroutines();
		StartCoroutine(TurnToFace(target.position));
	}

	public IEnumerator FindAndScream(Vector3 lookTarget)
	{
		Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
		float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

		while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
		{
			float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
			transform.eulerAngles = Vector3.up * angle;
			yield return null;
		}

		anim.SetTrigger("Scream");
		yield return null;
	}

	public IEnumerator TurnToFace(Vector3 lookTarget)
	{
		fallbackTrigger = false;

		Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
		float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

		while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
		{
			float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
			transform.eulerAngles = Vector3.up * angle;
			yield return null;
		}

		anim.SetBool("Run", true);
		yield return null;
	}
}