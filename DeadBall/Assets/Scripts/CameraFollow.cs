using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //Inpsired by the video
    //https://www.youtube.com/watch?v=XoKC3znQPcw

    public Transform target = null;

    public GameObject targetGameObject;
    public string currentTargetString;
    public string previousTargetString;

    public Transform spawnerTarget;

    public int reframeTime;
    public AnimationCurve reframeCurve;

    public float zDistance = 10f;

    private Vector3 targetOffset;
    private Vector3 currentOffset;

    private void Start()
    {
        FindTarget();
        targetOffset = transform.position - target.position;
        SetTarget(targetGameObject.transform);
    }
    // Update is called once per frame
    void Update()
    {
        FindTarget();
        if (!(currentTargetString.Equals(previousTargetString)))
        {
            currentTargetString = previousTargetString;
            SetTarget(targetGameObject.transform);
        }

        if (target != null)
        {
            //Debug.Log(currentOffset);
            //Debug.Log(target.position.x);
            transform.position = new Vector3(target.position.x + currentOffset.x, transform.position.y, -zDistance);
            //transform.position = target.position + currentOffset - new Vector3(0, 0, zDistance);
        }

        if (GlobalVariable.goToSpawner)
        {
            SetTarget(GlobalVariable.location.transform);
            GlobalVariable.goToSpawner = false;
        }
    }

    public void SetTarget(Transform t)
    {
        currentOffset = transform.position - t.position;
        target = t;
        StopAllCoroutines();
        StartCoroutine(ReframeCoroutine());
    }

    private void FindTarget()
    {
        if (GlobalVariable.playerBalls[0] != null)
        {
            float minTotalDistance = 0f;
            int indexOfMostCenteredBall = 0;
            for (int i = 0; i < GlobalVariable.playerBalls.Length; i++)
            {
                if (GlobalVariable.playerBalls[i] != null)
                {
                    float totalDistance = 0f;
                    for (int j = 0; j < GlobalVariable.playerBalls.Length; j++)
                    {
                        if (GlobalVariable.playerBalls[j] != null)
                        {
                            if (GlobalVariable.playerBalls[i] == GlobalVariable.playerBalls[j])
                            {
                                continue;
                            }

                            totalDistance += Vector3.Distance(GlobalVariable.playerBalls[i].transform.position, GlobalVariable.playerBalls[j].transform.position);
                        }
                    }
                    if (minTotalDistance == 0 || totalDistance < minTotalDistance)
                    {
                        minTotalDistance = totalDistance;
                        indexOfMostCenteredBall = i;
                    }
                }
            }
            GameObject t = GlobalVariable.playerBalls[indexOfMostCenteredBall];
            if (previousTargetString == null)
            {
                currentTargetString = t.name;
                previousTargetString = t.name;
                targetGameObject = t;
            }
            else
            {
                previousTargetString = t.name;
                targetGameObject = t;
            }
        }
    }

    IEnumerator ReframeCoroutine()
    {
        Vector3 fromOffset = currentOffset;
        float elapsed = 0f;
        float progress = 0f;

        while (progress < 0.999f)
        {
            currentOffset = Vector3.LerpUnclamped(fromOffset, targetOffset, reframeCurve.Evaluate(progress));
            yield return null;
            elapsed += Time.deltaTime;
            //elapsed += 0.01f;
            progress = elapsed / reframeTime;
        }
        currentOffset = targetOffset;
    }
}
