using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class FittsSelection
{
    public int participantID;
    public int conditionID;
    public string conditionText;
    public int targetNum;
    public float amplitude;
    public float width;
    public float time;
    public float selectionX;
    public float selectionY;
    public float selectionZ;
    public int hit;
    public float targetX;
    public float targetY;
    public float targetZ;
    public float deltaX;
    public float deltaY;
    public float deltaZ;
}

public class FittsVRSummaryOutputController : MonoBehaviour
{
    public static FittsVRSummaryOutputController instance;

    private StreamWriter summaryOutput;
    private int participantID;

    private List<FittsSelection> selections = new List<FittsSelection>();
    private List<float> tList = new List<float>();
    private List<float> dXlist = new List<float>();
    private int hitSum = 0;

    private void Awake()
    {
        instance = this;
    }

    public void OpenSummaryOutput(int participantID)
    {
        this.participantID = participantID;

        summaryOutput = new StreamWriter(Application.persistentDataPath + "/FittsVR-Summary-" + participantID + "-" + DateTime.Now.ToString("ddMMyy-MMss") +  ".csv");

        string headerLine = "";

        headerLine += "Participant ID,";
        headerLine += "Condition ID,";
        headerLine += "Condition Name,";
        headerLine += "Amplitude,";
        headerLine += "Width,";
        headerLine += "Fitts ID,";
        headerLine += "Mean Time,";
        headerLine += "Throughput,";
        headerLine += "Mean Delta X,";
        headerLine += "Std. Dev. X,";
        headerLine += "Width Effective,";
        headerLine += "Fitts ID Effective,";
        headerLine += "Throughput (We),";
        headerLine += "Hits,";
        headerLine += "Total Targets";

        summaryOutput.WriteLine(headerLine);
    }

    public void AddSelection(Vector3 selectionVector)
    {
        FittsSelection newSelection = new FittsSelection();
        Vector3 currentTargetVector = FittsVRTargetContainerController.instance.GetCurrentTarget().transform.localPosition;

        newSelection.participantID = FittsVRController.instance.participantID;
        newSelection.conditionID = FittsVRController.instance.conditionID;
        newSelection.conditionText = FittsVRController.instance.conditionText;
        newSelection.targetNum = FittsVRController.instance.currentTargetCount;
        newSelection.amplitude = FittsVRController.instance.currentCondition.amplitude;
        newSelection.width = FittsVRController.instance.currentCondition.width;
        newSelection.time = Time.time - FittsVRController.instance.lastTargetTime;
        newSelection.selectionX = (float)Math.Round(selectionVector.x, 5);
        newSelection.selectionY = (float)Math.Round(selectionVector.y, 5);
        newSelection.selectionZ = (float)Math.Round(selectionVector.z, 5);
        newSelection.targetX = (float)Math.Round(currentTargetVector.x, 5);
        newSelection.targetY = (float)Math.Round(currentTargetVector.y, 5);
        newSelection.targetZ = (float)Math.Round(currentTargetVector.z, 5);
        newSelection.deltaX = (float)Math.Round(Math.Abs(currentTargetVector.x - selectionVector.x), 5);
        newSelection.deltaY = (float)Math.Round(Math.Abs(currentTargetVector.y - selectionVector.y), 5);
        newSelection.deltaZ = (float)Math.Round(Math.Abs(currentTargetVector.z - selectionVector.z), 5);
        newSelection.hit = FittsVRController.instance.targetIn ? 1 : 0;

        // hit for aggregates

        selections.Add(newSelection);
    }

    public void SummaryOutput()
    {
        string outputLine = "";

        outputLine += FittsVRController.instance.participantID + ",";
        outputLine += FittsVRController.instance.conditionID + ",";
        outputLine += FittsVRController.instance.conditionText + ",";
        outputLine += FittsVRController.instance.currentCondition.amplitude + ",";
        outputLine += FittsVRController.instance.currentCondition.width + ",";
        outputLine += FittsVRController.instance.currentCondition.fittsID + ",";

        foreach (FittsSelection selection in selections)
        {
            tList.Add(selection.time);
            dXlist.Add(selection.deltaX);
            hitSum += selection.hit;
        }

        float mT = MeanFromList(tList);
        outputLine += mT + ",";

        float TP = FittsVRController.instance.currentCondition.fittsID / mT;
        outputLine += TP + ",";

        float mDx = MeanFromList(dXlist);
        outputLine += mDx + ",";

        float sDx = STDevFromList(dXlist, mDx);
        outputLine += sDx + ",";

        float We = sDx * 4.133f;
        outputLine += We + ",";

        float IDe = (float)Math.Log((FittsVRController.instance.currentCondition.amplitude / We) + 1, 2);
        outputLine += IDe + ",";

        float TPe = IDe / mT;
        outputLine += TPe + ",";

        outputLine += hitSum + ",";
        outputLine += FittsVRController.instance.numberOfTargets;

        summaryOutput.WriteLine(outputLine);

        tList.Clear();
        dXlist.Clear();
        hitSum = 0;

        selections.Clear();
    }

    public void SummaryClose()
    {
        summaryOutput.Close();
    }

    private float MeanFromList(List<float> input)
    {
        float totalValue = 0.0f;

        foreach (float value in input) totalValue += value;

        return totalValue / input.Count;
    }

    private float STDevFromList(List<float> input, float mean)
    {
        List<float> deviations = new List<float>();

        foreach (float value in input) deviations.Add((float)Math.Pow(value - mean, 2));

        return (float)Math.Sqrt(MeanFromList(deviations));
    }
}
