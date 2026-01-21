using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FittsVRTargetController : MonoBehaviour
{
    public Material targetStartMaterial;
    public Material targetBasicMaterial;
    public Material targetActiveMaterial;
    public Material targetInactiveMaterial;
    public Material targetSelectedMaterial;

    public GameObject targetObject;

    public bool startTarget = false;

    public void ChangeTargetStatus(FITTS_TARGET_STATUS newStatus)
    {
        switch (newStatus)
        {
            case FITTS_TARGET_STATUS.BASIC:
                targetObject.GetComponent<MeshRenderer>().material = targetBasicMaterial;
                targetObject.transform.localPosition = new Vector3(0.0f, 0.0f, 0.001f);
                break;
            case FITTS_TARGET_STATUS.INACTIVE:
                targetObject.GetComponent<MeshRenderer>().material = targetInactiveMaterial;
                targetObject.transform.localPosition = new Vector3(0.0f, 0.0f, 0.002f);
                break;
            case FITTS_TARGET_STATUS.ACTIVE:
                if (startTarget)
                {
                    targetObject.GetComponent<MeshRenderer>().material = targetStartMaterial;
                }
                else
                {
                    targetObject.GetComponent<MeshRenderer>().material = targetActiveMaterial;
                }
                targetObject.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                break;
            case FITTS_TARGET_STATUS.SELECTED:
                targetObject.GetComponent<MeshRenderer>().material = targetSelectedMaterial;
                targetObject.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                break;
        }
    }

    public void OnObjectEnter()
    {
        FittsVRController.instance.TargetObjectEnter(this.gameObject);
    }

    public void OnObjectExit()
    {
        FittsVRController.instance.TargetObjectOut(this.gameObject);
    }
}
