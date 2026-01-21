using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FITTS_TARGET_STATUS
{
    INACTIVE,
    BASIC,
    ACTIVE,
    SELECTED
}

public class FittsVRTargetContainerController : MonoBehaviour
{
    public static FittsVRTargetContainerController instance;

    private int currentTargetIndex = 0;
    public List<GameObject> targets = new List<GameObject>();

    public GameObject targetPrefab;

    public int totalTargets { get; private set; } = 11;
    private float amplitude = 1.0f;
    private float width = 0.4f;

    private void Awake()
    {
        instance = this;
    }

    public GameObject GetCurrentTarget()
    {
        return targets[currentTargetIndex].gameObject;
    }

    public void DeleteTargets()
    {
        foreach (GameObject target in targets)
        {
            DestroyImmediate(target);
        }
        targets.Clear();
    }

    public void SetupTargets(int totalTargets, float amplitude, float width)
    {
        this.totalTargets = totalTargets;
        this.amplitude = amplitude;
        this.width = width;

        ResetTargets();
    }

    public void ResetTargets()
    {
        DeleteTargets();

        for (float i = 0.0f; i < totalTargets; i++)
        {
            float x = (amplitude / 2.0f) * Mathf.Cos((Mathf.PI * 2) * (i / totalTargets));
            float y = (amplitude / 2.0f) * Mathf.Sin((Mathf.PI * 2) * (i / totalTargets));

            GameObject newTarget = Instantiate(targetPrefab, this.gameObject.transform, false);

            if (i == 0) newTarget.GetComponent<FittsVRTargetController>().startTarget = true;
            newTarget.GetComponent<FittsVRTargetController>().ChangeTargetStatus(FITTS_TARGET_STATUS.BASIC);

            newTarget.transform.localPosition = new Vector3(newTarget.transform.localPosition.x + x, newTarget.transform.localPosition.y + y, newTarget.transform.localPosition.z);
            newTarget.transform.localScale = new Vector3(width, width, width);
            targets.Add(newTarget);
        }

        currentTargetIndex = 0;
        SetNextActiveTarget(0);
    }

    public void SetNextActiveTarget(int targetCount)
    {
        if (targetCount > 0)
        {
            if (targetCount == 1)
            {
                targets[currentTargetIndex].gameObject.GetComponent<FittsVRTargetController>().ChangeTargetStatus(FITTS_TARGET_STATUS.BASIC);
                targets[currentTargetIndex].gameObject.GetComponent<FittsVRTargetController>().startTarget = false;
            }
            else
            {
                targets[currentTargetIndex].gameObject.GetComponent<FittsVRTargetController>().ChangeTargetStatus(FITTS_TARGET_STATUS.INACTIVE);
            }

            if (targetCount < totalTargets)
            {
                int halfWay = (totalTargets + 1) / 2;
                currentTargetIndex = (currentTargetIndex + halfWay) % totalTargets;
            }
            else
            {
                currentTargetIndex = 0;
            }
        }

        targets[currentTargetIndex].gameObject.GetComponent<FittsVRTargetController>().ChangeTargetStatus(FITTS_TARGET_STATUS.ACTIVE);
    }
}
