﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoreExploder : MonoBehaviour
{
	public AudioSource[] audioSource; // run, stop, spark, explode, glass

	public GlassBreak[] glassBreaker;
	public bool isReadyToOverPower;
	public bool isOverPowering;

	float currentExplodeRate = 0;
	public float maxExplodeRate = 3;
	bool isExploded;

	public ParticleSystem[] sparkParticle;
	public ParticleSystem[] explodeParticle;

	ParticleSystem[] spartkPs;
	ParticleSystem[] explodePs;

	public TextMeshProUGUI explanationText;
	public TextMeshProUGUI explodeRateText;

	TestShake testShake;

	IEnumerator volumeController;

	bool isPowerdOnFirst;

	void Awake() {
		spartkPs = sparkParticle;
		explodePs = explodeParticle;
		for (int i = 0; i < explodePs.Length; i++) explodePs[i].Stop();
		testShake = GetComponent<TestShake>();
	}

	void Update()
	{
		if (!isReadyToOverPower) return;

		if (Input.GetKey(KeyCode.F))
		{
			currentExplodeRate = Mathf.Clamp(currentExplodeRate +=Time.deltaTime * 1.7f, 0, maxExplodeRate);
			isOverPowering = true;
			isPowerdOnFirst = true;
			if (!audioSource[0].GetComponent<AudioSource>().isPlaying) audioSource[0].GetComponent<AudioSource>().Play();
		}

		if(Input.GetKeyUp(KeyCode.F))
		{
			if(!isExploded) currentExplodeRate = 0;
			isOverPowering = false;
			isTurnOff = true;
		}

		if (!isExploded && currentExplodeRate >= maxExplodeRate)
		{
			StartCoroutine(Explode());
			isExploded = true;
		}

		explodeRateText.text = currentExplodeRate.ToString("N2");

		EffectUpdate();
		SoundControl();
	}

	bool isTurnOff;

	void SoundControl()
	{
		if (!isTurnOff) return;

		if (!audioSource[1].GetComponent<AudioSource>().isPlaying) audioSource[1].GetComponent<AudioSource>().Play();

		audioSource[0].GetComponent<AudioSource>().volume -= Time.deltaTime * 0.18f;

		if (audioSource[1].GetComponent<AudioSource>().time > 1.5)
		{
			if(audioSource[0].GetComponent<AudioSource>().isPlaying) audioSource[0].GetComponent<AudioSource>().Stop();
			audioSource[1].GetComponent<AudioSource>().volume -= Time.deltaTime * 0.15f;
		}

		if (audioSource[1].GetComponent<AudioSource>().time > 1.95)
		{
			audioSource[1].GetComponent<AudioSource>().Stop();
		}

		if (!audioSource[1].GetComponent<AudioSource>().isPlaying)
		{
			audioSource[1].GetComponent<AudioSource>().Stop();
			audioSource[0].GetComponent<AudioSource>().volume = 0.2f;
			audioSource[1].GetComponent<AudioSource>().volume = 0.2f;
			isTurnOff = false;
		}
	}

	IEnumerator SoundOff()
	{
		while(audioSource[0].GetComponent<AudioSource>().volume <= 0f)
		{
			audioSource[0].GetComponent<AudioSource>().volume -= Time.deltaTime;
			yield return null;
		}
		audioSource[0].GetComponent<AudioSource>().Stop();
		audioSource[0].GetComponent<AudioSource>().volume = 0.2f;
		isReadyToOverPower = false;
	}

	IEnumerator Explode()
	{
		yield return new WaitForSeconds(1f);
		// light go down
		StartCoroutine(LightChanger());

		for (int i = 0; i < explodePs.Length; i++) explodePs[i].Play();
		for (int i = 0; i < spartkPs.Length; i++) spartkPs[i].Stop();
		for (int i = 0; i < glassBreaker.Length; i++) glassBreaker[i].Break();

		audioSource[0].GetComponent<AudioSource>().Stop();
		StartCoroutine(soundPlay());
		StartCoroutine(cameraShake());

		explodeRateText.text = "0";

		explanationText.color = new Color(explanationText.color.r, explanationText.color.g, explanationText.color.b, 0.2f);
		explodeRateText.color = new Color(explodeRateText.color.r, explodeRateText.color.g, explodeRateText.color.b, 0.2f);

		isReadyToOverPower = false;

		yield return null;
	}

	public Light pointLight;

	IEnumerator LightChanger()
	{
		float alpha = 0;

		while(alpha <= 1f)
		{
			pointLight.intensity = Random.Range(0.5f, 2f);
			alpha += Time.deltaTime;
			yield return null;
		}

		pointLight.intensity = 0.1f;

		yield return null;
	}

	IEnumerator soundPlay()
	{
		// explode sound play
		if (!audioSource[2].GetComponent<AudioSource>().isPlaying) audioSource[2].GetComponent<AudioSource>().Play();
		yield return new WaitForSeconds(.2f);
		if (!audioSource[3].GetComponent<AudioSource>().isPlaying) audioSource[3].GetComponent<AudioSource>().Play();
		yield return null;
	}

	IEnumerator cameraShake()
	{
		// cameraShake
		testShake.Shake();
		yield return new WaitForSeconds(.2f);
		testShake.Shake();
		yield return null;
	}

	void EffectUpdate()
	{
		if (isOverPowering)
		{
			for (int i = 0; i < spartkPs.Length; i++)
			{
				spartkPs[i].startLifetime = Mathf.Clamp(spartkPs[i].startLifetime += 0.05f, 0, 1.5f);
			}
		}

		else
		{
			for (int i = 0; i < spartkPs.Length; i++)
			{
				spartkPs[i].startLifetime = Mathf.Clamp(spartkPs[i].startLifetime -= 0.1f, 0, 1.5f);
			}
		}
	}
}