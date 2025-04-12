using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// This class provides functionality for triggering haptic feedback on XR controllers.
/// </summary>
public class CustomHaptic
{
    /// <summary>
    /// The intensity of the haptic feedback (0 to 1, where 1 is strongest).
    /// </summary>
    [Range(0f, 1f)]
    public float Intensity;

    /// <summary>
    /// The duration of the haptic feedback in seconds.
    /// </summary>
    public float Duration;

    /// <summary>
    /// Attempts to trigger haptic feedback based on the provided interaction arguments.
    /// If the interactor object is an XRBaseController, it triggers haptic feedback on that controller.
    /// </summary>
    /// <param name="args">The interaction arguments containing information about the interaction event.</param>
    public void TriggerHaptic(BaseInteractionEventArgs args)
    {
        // Check if the interacting object is an XRBaseController
        if (args.interactorObject is XRBaseController controller)
        {
            // If it is, trigger haptic feedback on that specific controller
            TriggerHaptic(controller);
        }
    }

    /// <summary>
    /// Triggers haptic feedback on a specified XRBaseController.
    /// If the intensity is greater than zero, it sends a haptic impulse with the defined intensity and duration to the controller.
    /// </summary>
    /// <param name="controller">The XRBaseController on which to trigger haptic feedback.</param>
    public void TriggerHaptic(XRBaseController controller)
    {
        if (Intensity > 0)
        {
            controller.SendHapticImpulse(Intensity, Duration);
        }
    }
}

