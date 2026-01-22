using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;

public class ExperimentController : MonoBehaviour
{
    public InputActionAsset inputActions;
    private InputActionMap rightActionMap;
    private InputAction rightActivateAction;

    public NearFarInteractor rightRayInteractor;
    public GameObject selectionPoint;

    public GameObject experimentUI;
    public GameObject conditionUI;
    public TextMeshProUGUI conditionUIHeader;
    public GameObject endUI;

    public int currentExpCondition = 0;
    public int totalExpConditions = 2;

    private bool fittsEntry = false;

    void Start()
    {
        rightActionMap = inputActions.FindActionMap("XRI Right Interaction");
        rightActivateAction = rightActionMap.FindAction("Activate");
    }

    private void Update()
    {
        if (fittsEntry)
        {
            if (!FittsVRController.instance.conditionComplete)
            {


                if (rightActivateAction.WasPerformedThisFrame())
                {
                    FittsVRDetailOutputController.instance.SetAddition("New Field", "Val " + Random.Range(0, 100));

                    rightRayInteractor.TryGetCurveEndPoint(out Vector3 endPoint, true, false);
                    selectionPoint.transform.position = endPoint;
                    FittsVRController.instance.FittsSelection();
                }
            }
            else
            {
                fittsEntry = false;
                nextExpCondition();
            }
        }
    }

    public void StartExperiment()
    {
        experimentUI.SetActive(false);
        FittsVRController.instance.SetPID(0); // Set the participant ID for the Fitts conditions for balancing purposes.
        setupExpCondition();        
    }

    public void StartCondition()
    {
        conditionUI.SetActive(false);
        FittsVRController.instance.SetConditionID(currentExpCondition, "CONDITION " + currentExpCondition); // Set the condition information for the Fitts output.
        FittsVRController.instance.FittsStart(); // Start the Fitts Law process.
        fittsEntry = true;
    }

    private void nextExpCondition()
    {
        currentExpCondition++;

        if (currentExpCondition < totalExpConditions)
        {
            setupExpCondition();
        } else
        {
            endExp();
        }
    }

    private void setupExpCondition()
    {
        conditionUI.SetActive(true);
        conditionUIHeader.text = "CONDITION " + currentExpCondition;
    }

    private void endExp()
    {
        endUI.SetActive(true);
        FittsVRController.instance.FittsEnd();
    }
}
