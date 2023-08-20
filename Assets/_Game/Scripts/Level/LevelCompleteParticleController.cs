using System;
using UnityEngine;

public class LevelCompleteParticleController : MonoBehaviour
{
	[SerializeField] private Transform confettiLeft;
	[SerializeField] private Transform confettiRight;

	private Camera _camera;
	private void Start()
	{
		_camera = Camera.main;
		var isCameraOrthographic = _camera.orthographic;
		
		SetupConfetti(isCameraOrthographic);
	}

	void SetupConfetti(bool orthoGraphic = false)
	{
		if (!orthoGraphic)
			return;

		var cameraSize = _camera.orthographicSize;
		var horizontalDistance = cameraSize / 2;
		var particleScale = Vector3.one * horizontalDistance;

		confettiLeft.localPosition = new Vector3(-horizontalDistance, -cameraSize, 2);
		confettiRight.localPosition = new Vector3(horizontalDistance, -cameraSize, 2);

		confettiLeft.localScale = particleScale;
		confettiRight.localScale = particleScale;
	}
}
