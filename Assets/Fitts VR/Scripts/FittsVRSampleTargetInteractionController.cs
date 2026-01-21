using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

public class FittsVRSampleTargetInteractionController : MonoBehaviour
{
    public void onHoverEnter()
    {
        FittsVRController.instance.TargetObjectEnter(this.gameObject);
    }

    public void onHoverExit()
    {
        FittsVRController.instance.TargetObjectOut(this.gameObject);

    }
}
