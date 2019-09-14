using SebController;
using UnityEngine;

public class QuickAndUglyFollowPlayer : MonoBehaviour
{
    private Transform Trans;
    private Transform targetTrans;

    private Vector3 smoothRef;

    [SerializeField]
    private float smoothTime;

    [SerializeField]
    private Vector3 offset = Vector3.up + Vector3.back;

    private void Awake()
    {
        Trans = transform;
        //targetTrans = FindObjectOfType<PlayerCharacter>().transform;
        if (!targetTrans)
        {
            targetTrans = FindObjectOfType<SebPlayer>().transform;
        }
    }

    private void Update()
    {
        Vector3 targetPosition = targetTrans.position + offset;

        Trans.position = Vector3.SmoothDamp(Trans.position, targetPosition, ref smoothRef, smoothTime);
    }
}
