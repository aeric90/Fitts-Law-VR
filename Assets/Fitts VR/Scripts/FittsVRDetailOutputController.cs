using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class FittsVRDetailOutputAddition
{
    public string Caption = "";
    public string Value  = "";

    public FittsVRDetailOutputAddition() { }
}

public class FittsVRDetailOutputController : MonoBehaviour
{
    public static FittsVRDetailOutputController instance;

    public List<FittsVRDetailOutputAddition> Additions = new List<FittsVRDetailOutputAddition>();

    private StreamWriter detailOutput;
    private int participantID;

    private void Awake()
    {
        instance = this;
    }

    public void OpenDetailOutput(int participantID)
    {
        this.participantID = participantID;

        detailOutput = new StreamWriter(Application.persistentDataPath + "/FittsVR-Detail-" + participantID + "-" + DateTime.Now.ToString("ddMMyy-MMss") + ".csv");

        string headerLine = "";

        headerLine += "Participant ID,";
        headerLine += "Condition ID,";
        headerLine += "Condition Name,";
        headerLine += "Amplitude,";
        headerLine += "Width,";
        headerLine += "Fitts ID,";
        headerLine += "Target Number,";
        headerLine += "Time,";
        headerLine += "Hit,";
        headerLine += "Target In Time,";
        headerLine += "Target Entry Count,";
        headerLine += "Selection X,";
        headerLine += "Selection Y,";
        headerLine += "Selection Z,";
        headerLine += "Target X,";
        headerLine += "Target Y,";
        headerLine += "Target Z,";
        headerLine += "Delta X,";
        headerLine += "Delta Y,";
        headerLine += "Delta Z,";
        headerLine += "Controller Position X,";
        headerLine += "Controller Position Y,";
        headerLine += "Controller Position Z,";
        headerLine += "Controller Rotation X,";
        headerLine += "Controller Rotation Y,";
        headerLine += "Controller Rotation Z,";
        headerLine += "Controller Rotation W";

        foreach(FittsVRDetailOutputAddition addition in Additions)
        {
            headerLine += ("," + addition.Caption);
        }

        detailOutput.WriteLine(headerLine);
    }

    public void DetailOutput()
    {
        string outputLine = "";
        float selectionTime = Time.time - FittsVRController.instance.lastTargetTime;

        Vector3 currentTargetVector = FittsVRTargetContainerController.instance.GetCurrentTarget().transform.localPosition;

        Vector3 selectionVector = FittsVRController.instance.fittsSelectionPoint.transform.position;

        Vector3 controllerVector = FittsVRController.instance.fittsControllerLocal.transform.position;
        Quaternion controllerRotation = FittsVRController.instance.fittsControllerLocal.transform.rotation;

        outputLine += FittsVRController.instance.participantID + ",";
        outputLine += FittsVRController.instance.conditionID + ",";
        outputLine += FittsVRController.instance.conditionText + ",";
        outputLine += FittsVRController.instance.currentCondition.amplitude + ",";
        outputLine += FittsVRController.instance.currentCondition.width + ",";
        outputLine += FittsVRController.instance.currentCondition.fittsID + ",";
        outputLine += FittsVRController.instance.currentTargetCount + ",";
        outputLine += selectionTime + ",";
        outputLine += FittsVRController.instance.targetIn + ",";
        outputLine += FittsVRController.instance.targetInTime + ",";
        outputLine += FittsVRController.instance.targetEntryCount + ",";
        outputLine += Math.Round(selectionVector.x, 5) + ",";
        outputLine += Math.Round(selectionVector.y, 5) + ",";
        outputLine += Math.Round(selectionVector.z, 5) + ",";
        outputLine += Math.Round(currentTargetVector.x, 5) + ",";
        outputLine += Math.Round(currentTargetVector.y, 5) + ",";
        outputLine += Math.Round(currentTargetVector.z, 5) + ",";
        outputLine += Math.Round(Math.Abs(currentTargetVector.x - selectionVector.x), 5) + ",";
        outputLine += Math.Round(Math.Abs(currentTargetVector.y - selectionVector.y), 5) + ",";
        outputLine += Math.Round(Math.Abs(currentTargetVector.z - selectionVector.z), 5) + ",";
        outputLine += Math.Round(controllerVector.x, 5) + ",";
        outputLine += Math.Round(controllerVector.y, 5) + ",";
        outputLine += Math.Round(controllerVector.z, 5) + ",";
        outputLine += Math.Round(controllerRotation.x, 5) + ",";
        outputLine += Math.Round(controllerRotation.y, 5) + ",";
        outputLine += Math.Round(controllerRotation.z, 5) + ",";
        outputLine += Math.Round(controllerRotation.w, 5);

        foreach (FittsVRDetailOutputAddition addition in Additions)
        {
            outputLine += ("," + addition.Value);
        }

        detailOutput.WriteLine(outputLine);
    }

    public void SetAddition(string caption, string value)
    {
        foreach (FittsVRDetailOutputAddition addition in Additions)
        {
           if(addition.Caption == caption) addition.Value = value;
        }
    }

    public void ResetAdditions()
    {
        foreach (FittsVRDetailOutputAddition addition in Additions)
        {
            addition.Value = "";
        }
    }

    public void DetailClose()
    {
        detailOutput.Close();
    }
}
