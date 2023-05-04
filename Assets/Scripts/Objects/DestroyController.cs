using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyController : MonoBehaviour
{
    //#����#  �ش� ��ũ��Ʈ�� ������Ʈ�� �ġڱ��ٸ� ����ϴ� ��ũ��Ʈ�Դϴ�. 


    [SerializeField]
    [Tooltip("0. No Destory \n1. DelayDestroy")]
    int setDestroy = 1;

    [SerializeField]
    [Tooltip("\"setDestroy\"�� \"1\"�ϰ�� �ı� �� �ð��� �����Ѵ�.")]
    float destroyDelayTime = 1;



    void Start()
    {
        switch(setDestroy)
        {
            case 0:
                break;

            case 1:
                StartCoroutine(DelayDestroy(destroyDelayTime));
                break;
        }
    }

    IEnumerator DelayDestroy (float delay)
    {
        yield return new WaitForSeconds(delay);

        Destroy(this.gameObject);
    }
}
