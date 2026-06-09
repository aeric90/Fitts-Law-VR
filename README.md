WIP Documentation

Fitts VR Controller

The main component is the Fitts VR Controller. It should be placed where you want the Fitts' Law test to occur. It can be moved and rotated into position and the calculations will always be done on the plane of the targets. The child objects of the controller should not be moved. These are the current parameters:

Number of Targets: The number of targets that will be generated per ID. This should be an odd number.
Repetitions: How many times the Fitts VR Controller will go through the full set of IDs before completing the condition.
Amplitudes and Widths: Add your Amplitudes and widths (in m) here. The controller will build the IDs based on the combinations (3 A * 2 W = 6 IDS)
Selection Point Object: This should be the tracking point your system uses to determine the selection point's location. In this case, it's the end of the raycast, but it could be any tracked selection point.
Selection Controller Object: This should be the tracked controller object itself. The system will caputre it's position and rotation in the local space.

The 3 other parameters are public for development purposes and should not be modified. They will be hidden in the future.

The Fitts VR Controller includes some public methods that must be implemented in your experiment task for it to work correctly. They are accessed through the FittsVRController.instance public reference. These are all demonstrated in the Experiment Controller code file.

SetPID(int x) - Sets the current participant's ID. This is used to counterbalance the IDs presented to the participant and to generate the output.
SetConditionID(int conditionID, string conditionText) - Sets or resets the Fitts ID for a new condition and stores the condition ID number and name for the output.
FittsStart() - Starts the Fitts Law experiment and generates the targets.
FittsSelection() - Call this function when you are confirming a selection. In this case, it occurs whenever the user pulls the right trigger on the controller, but it can be customized using this function.
FittsEnd() - Ends the Fitts Law experiment and generates the summary output.

TargetObjectEnter(GameObject target)  - Call this function from an event which corresponds to when the tracked selection enters a Fitts Law target. This indicates the current selection, increments the number of entries, and totals up the time in contact with the target.
TargetObjectOut(GameObject target)  -  Call this function from an event which corresponds to when the tracked selection enters a Fitts Law target. This resets the current selection and stops totalling up the time in contact with the target.

There is also one important public parameter:

bool conditionComplete - becomes true when the Fitts VR controller has registered selections for all the targets in all the IDs. You can use this to determine when to move to the next experiment condition.

Output Controllers

These generate the detail and summary outputs. We're working on adding features to these. At the moment, they generate output to the system's default file storage location. In the case of Windows, this is generally C:\Users\<user name>\AppData\LocalLow\DefaultCompany\Fitts' Law VR\  , but this can vary.

Fitts VR Target Container

This object controls the target distribution. It is also currently where you link your target prefab object for replication. We will be moving this prefab to the main controller later.

Fitts VR Target Prefab

This is probably the most complicated part. You will need to design a target prefab that works for your current experimental design. In this case, we created a target that is an XR interaction Toolkit raycast interactable, then attached the Fitts VR Target Controller script.

This script handles changes to the target status and the materials associated with them. The main controller is the object which communicates these changes through the workflow and the TargetObjectIn and TargetObjectOut functions. You can see how those are implemented in the Fitts VR Sample Target Interaction Controller component.

The materials can be customized. They are used as follows (titles are placeholders):
Start - The first target in the ID. Indicates to the participant that they can rest since no time is being measured.
Default - The target is not active but has not been selected yet.
Active - This is the currently active target.
Inactive - This target has been selected.
Selected - This target is active  and the participant is currently in contact with it.

Target Object is the gameobject that contains the mesh renderer so the materials can be changed.

Start Target will be made private in the future.
