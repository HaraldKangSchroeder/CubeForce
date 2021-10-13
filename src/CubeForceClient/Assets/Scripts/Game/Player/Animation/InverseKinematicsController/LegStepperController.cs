using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegStepperController : MonoBehaviour
{
    [SerializeField] LegStepper frontLeftLegStepper;
    [SerializeField] LegStepper frontRightLegStepper;
    [SerializeField] LegStepper backLeftLegStepper;
    [SerializeField] LegStepper backRightLegStepper;


    void Awake()
    {
        StartCoroutine(LegUpdateCoroutine());
    }

    // Only allow diagonal leg pairs to step together
    IEnumerator LegUpdateCoroutine()
    {
        // Run continuously
        while (true)
        {
            // Try moving one diagonal pair of legs
            do
            {
            frontLeftLegStepper.tryMove();
            backRightLegStepper.tryMove();
            // Wait a frame
            yield return null;
            
            // Stay in this loop while either leg is moving.
            // If only one leg in the pair is moving, the calls to tryMove() will let
            // the other leg move if it wants to.
            } while (backRightLegStepper.Moving || frontLeftLegStepper.Moving);

            // Do the same thing for the other diagonal pair
            do
            {
            frontRightLegStepper.tryMove();
            backLeftLegStepper.tryMove();
            yield return null;
            } while (backLeftLegStepper.Moving || frontRightLegStepper.Moving);
        }
    }
}
