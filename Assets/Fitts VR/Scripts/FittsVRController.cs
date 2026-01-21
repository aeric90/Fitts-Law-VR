using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class FittsCondition
{
    public float amplitude = 0.0f;
    public float width = 0.0f;
    [SerializeField] public float fittsID = 0.0f;

    public FittsCondition() { }

    public FittsCondition(float amplitude, float width)
    {
        this.amplitude = amplitude;
        this.width = width;

        this.fittsID = (float)Math.Log((amplitude / width) + 1, 2);
    }
}

[System.Serializable]
public class FittsTrial
{
    public int numOfTargets = 0;
    public List<FittsCondition> conditions = new List<FittsCondition>();

    public FittsTrial() { }

    public FittsTrial(int numOfTargets) { this.numOfTargets = numOfTargets; }
}

public class FittsVRController : MonoBehaviour
{
    public static FittsVRController instance;

    public int numberOfTargets;
    public int repetitions = 1;
    public List<float> amplitudes = new List<float>();
    public List<float> widths = new List<float>();

    private FittsTrial FittsTrials;

    private List<int> conditionSquareNew = new List<int>();
    public int participantID { get; private set; } = 0;
    public int conditionID { get; private set; } = -1;
    public string conditionText { get; private set; } = "";
    public int currentTrial { get; private set; } = 0;
    public FittsCondition currentCondition { get; private set; } = null;
    public int currentTargetCount { get; private set; } = 0;
    public bool targetIn { get; private set; }

    public float lastTargetTime { get; private set; } = 0.0f;
    public int targetEntryCount { get; private set; } = 0;
    public float targetInTime { get; private set; } = 0.0f;
    private float targetEntryTime = 0.0f;

    public GameObject selectionPointObject;
    public GameObject selectionControllerObject;

    public GameObject fittsSelectionPoint;
    public GameObject fittsControllerLocal;

    public bool fittsRunning = false;
    public bool conditionComplete { get; private set; } = false;

    void Awake()
    {
        instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        createConditions();
    }

    private void Update()
    {
        
    }

    public void SetPID(int PID)
    {
        participantID = PID;
        conditionSquareNew = LatinSquareGenerator(FittsTrials.conditions.Count, PID);
        FittsVRDetailOutputController.instance.OpenDetailOutput(PID);
        FittsVRSummaryOutputController.instance.OpenSummaryOutput(PID);
    }

    public void FittsStart()
    {
        currentTrial = 0;
        SetFittsCondition();
    }

    public void SetConditionID(int conditionID)
    {
        this.conditionID = conditionID;
        this.conditionText = "";
    }

    public void SetConditionID(int conditionID, string conditionText)
    {
        this.conditionID = conditionID;
        this.conditionText = conditionText;
    }

    private void createConditions()
    {
        if (amplitudes.Count > 0 && widths.Count > 0)
        {
            FittsTrials = new FittsTrial(numberOfTargets);

            for (int i = 0; i < amplitudes.Count; i++)
            {
                for (int j = 0; j < widths.Count; j++)
                {
                    for (int k = 0; k < repetitions; k++)
                    {
                        FittsTrials.conditions.Add(new FittsCondition(amplitudes[i], widths[j]));
                    }
                }
            }
        }
    }
    
    private void SetFittsCondition()
    {
        currentCondition = FittsTrials.conditions[conditionSquareNew[currentTrial % conditionSquareNew.Count]];

        FittsVRTargetContainerController.instance.SetupTargets(FittsTrials.numOfTargets, currentCondition.amplitude, currentCondition.width);

        currentTargetCount = 0;
        fittsRunning = true;
        conditionComplete = false;
    }

    private void NextFittsCondition()
    {
        FittsVRSummaryOutputController.instance.SummaryOutput();

        currentTrial++;

        if (currentTrial >= conditionSquareNew.Count)
        {
            EndFittsCondition();
        }
        else
        {
            SetFittsCondition();
        }
    }

    private void EndFittsCondition()
    {
        conditionComplete = true;
        FittsVRTargetContainerController.instance.DeleteTargets();
    }

    public void FittsEnd()
    {
        fittsRunning = false;
        FittsVRSummaryOutputController.instance.SummaryClose();
        FittsVRDetailOutputController.instance.DetailClose();
    }

    public void TargetObjectEnter(GameObject target)
    {
        if (FittsVRTargetContainerController.instance.GetCurrentTarget() == target)
        {
            targetIn = true;
            targetEntryCount++;
            targetEntryTime = Time.time;
            target.GetComponent<FittsVRTargetController>().ChangeTargetStatus(FITTS_TARGET_STATUS.SELECTED);
        }
    }

    public void TargetObjectOut(GameObject target)
    {
        if (FittsVRTargetContainerController.instance.GetCurrentTarget() == target)
        {
            targetIn = false;
            targetInTime += (Time.time - targetEntryTime);
            target.GetComponent<FittsVRTargetController>().ChangeTargetStatus(FITTS_TARGET_STATUS.ACTIVE);
        }
    }

    public void FittsSelection()
    {
        if (fittsRunning)
        {
            fittsSelectionPoint.transform.position = selectionPointObject.transform.position;
            fittsControllerLocal.transform.position = selectionControllerObject.transform.position;
            fittsControllerLocal.transform.rotation = selectionControllerObject.transform.rotation;

            if (currentTargetCount > 0)
            {
                FittsVRSummaryOutputController.instance.AddSelection(fittsSelectionPoint.transform.localPosition);
                FittsVRDetailOutputController.instance.DetailOutput();
            }

            lastTargetTime = Time.time;
            targetEntryCount = 0;
            currentTargetCount++;

            if (currentTargetCount > FittsVRTargetContainerController.instance.totalTargets)
            {
                NextFittsCondition();
            }
            else
            {
                FittsVRTargetContainerController.instance.SetNextActiveTarget(currentTargetCount);
            }
        }
    }

    private List<int> LatinSquareGenerator(int conditions, int participantID)
    {
        List<int> result = new List<int>();

        int j = 0;
        int h = 0;

        for (int i = 0; i < conditions; i++)
        {
            int val = 0;

            if (i < 2 || i % 2 != 0)
            {
                val = j++;
            }
            else
            {
                val = conditions - h - 1;
                h++;
            }

            int idx = (val + participantID) % conditions;
            result.Add(idx);
        }

        if (conditions % 2 != 0 && participantID % 2 != 0)
        {
            result.Reverse();
        }

        return result;
    }
}
