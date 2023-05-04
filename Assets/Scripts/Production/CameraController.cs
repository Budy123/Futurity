using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Tooltip("ī�޶� ������ ����Դϴ�.")]
    public Transform _target;
    [Tooltip("���� ��󿡰Լ� �󸶳� ������ ��ġ�� ī�޶� �ִ����� ��Ÿ���ϴ�.")]
    public Vector3 _offset;

	[Header("Caemra Shake")]
    [SerializeField] private bool isVibrate;
    [SerializeField] private float vibrationPower;
    [SerializeField] private float targetTime;
	[SerializeField] private float curvePower;
	[SerializeField] private AnimationCurve curve;
	private Vector3 initialPos;
    private float curTime;


	private void Start()
	{
		initialPos = _target.position + _offset;
		isVibrate = false;
	}

	private void FixedUpdate()
    {
        transform.position = _target.position + _offset;
    }

	private void Update()
	{
		if (isVibrate)
		{
			if (targetTime > curTime)
			{
				float curGraph = curve.Evaluate(curTime / targetTime);
				curTime += Time.deltaTime;
                transform.position = initialPos + Vector3.right * (curGraph - 0.5f) * curvePower + Random.insideUnitSphere * vibrationPower;
			}
            else
            {
                curTime = 0;
                transform.position = initialPos;
                isVibrate = false;
            }
		}
	}

	public void SetVibration(float time, float curvePower = 0.1f, float randomPower = 0.1f)
	{
		vibrationPower = randomPower;
		this.curvePower = curvePower;
		initialPos = transform.position;
		targetTime = time;
		isVibrate = true;
	}
}
