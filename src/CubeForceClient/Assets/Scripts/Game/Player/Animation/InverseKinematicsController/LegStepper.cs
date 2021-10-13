using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegStepper : MonoBehaviour
{
    // The position and rotation we want to stay in range of
    [SerializeField] Transform homeTransform;
    // Stay within this distance of home
    [SerializeField] float wantStepAtDistance;
    // How long a step takes to complete
    [SerializeField] float moveDuration;

    [SerializeField] float stepOvershootFraction;
    
    // Is the leg moving?
    public bool Moving;


    IEnumerator MoveToHome()
    {
        Moving = true;

        Vector3 startPoint = transform.position;
        Quaternion startRot = transform.rotation;

        Quaternion endRot = homeTransform.rotation;

        // Directional vector from the foot to the home position
        Vector3 towardHome = (homeTransform.position - transform.position);
        // Total distnace to overshoot by   
        float overshootDistance = wantStepAtDistance * stepOvershootFraction;
        Vector3 overshootVector = towardHome * overshootDistance;
        // Since we don't ground the point in this simplified implementation,
        // we restrict the overshoot vector to be level with the ground
        // by projecting it on the world XZ plane.
        overshootVector = Vector3.ProjectOnPlane(overshootVector, Vector3.up);

        // Apply the overshoot
        Vector3 endPoint = homeTransform.position + overshootVector;

        // We want to pass through the center point
        Vector3 centerPoint = (startPoint + endPoint) / 2;
        // But also lift off, so we move it up by half the step distance (arbitrarily)
        centerPoint += homeTransform.up * Vector3.Distance(startPoint, endPoint) / 2f;

        float timeElapsed = 0;
        do
        {
            timeElapsed += Time.deltaTime;
            float normalizedTime = timeElapsed / moveDuration;
            normalizedTime = Easing.Cubic.InOut(normalizedTime);
            // Quadratic bezier curve
            transform.position =
            Vector3.Lerp(
                Vector3.Lerp(startPoint, centerPoint, normalizedTime),
                Vector3.Lerp(centerPoint, endPoint, normalizedTime),
                normalizedTime
            );

            transform.rotation = Quaternion.Slerp(startRot, endRot, normalizedTime);

            yield return null;
        }
        while (timeElapsed < moveDuration);

        Moving = false;
    }

    public void tryMove()
    {
        // If we are already moving, don't start another move
        if (Moving) return;

        float distFromHome = Vector3.Distance(transform.position, homeTransform.position);

        // If we are too far off in position or rotation
        if (distFromHome > wantStepAtDistance)
        {
            // Start the step coroutine
            StartCoroutine(MoveToHome());
        }
    }
}


/* 
 * Most functions taken from Tween.js - Licensed under the MIT license
 * at https://github.com/sole/tween.js
 * Quadratic.Bezier by @fonserbc - Licensed under WTFPL license
 */
public delegate float EasingFunction(float k);

public class Easing
{
	public static float Linear (float k) {
		return k;
	}
	
	public class Quadratic
	{
		public static float In (float k) {
			return k*k;
		}
		
		public static float Out (float k) {
			return k*(2f - k);
		}
		
		public static float InOut (float k) {
			if ((k *= 2f) < 1f) return 0.5f*k*k;
			return -0.5f*((k -= 1f)*(k - 2f) - 1f);
		}

		/* 
		 * Quadratic.Bezier(k,0) behaves like Quadratic.In(k)
		 * Quadratic.Bezier(k,1) behaves like Quadratic.Out(k)
		 *
		 * If you want to learn more check Alan Wolfe's post about it http://www.demofox.org/bezquad1d.html
		 */
		public static float Bezier (float k, float c) {
			return c*2*k*(1 - k) + k*k;
		}
	};
	
	public class Cubic
	{		
		public static float In (float k) {
			return k*k*k;
		}
		
		public static float Out (float k) {
			return 1f + ((k -= 1f)*k*k);
		}
		
		public static float InOut (float k) {
			if ((k *= 2f) < 1f) return 0.5f*k*k*k;
			return 0.5f*((k -= 2f)*k*k + 2f);
		}
	};
	
	public class Quartic
	{		
		public static float In (float k) {
			return k*k*k*k;
		}
		
		public static float Out (float k) {
			return 1f - ((k -= 1f)*k*k*k);
		}
		
		public static float InOut (float k) {
			if ((k *= 2f) < 1f) return 0.5f*k*k*k*k;
			return -0.5f*((k -= 2f)*k*k*k - 2f);
		}		
	};
	
	public class Quintic
	{		
		public static float In (float k) {
			return k*k*k*k*k;
		}
		
		public static float Out (float k) {
			return 1f + ((k -= 1f)*k*k*k*k);
		}
		
		public static float InOut (float k) {
			if ((k *= 2f) < 1f) return 0.5f*k*k*k*k*k;
			return 0.5f*((k -= 2f)*k*k*k*k + 2f);
		}		
	};
	
	public class Sinusoidal
	{		
		public static float In (float k) {
			return 1f - Mathf.Cos(k*Mathf.PI/2f);
		}
		
		public static float Out (float k) {
			return Mathf.Sin(k*Mathf.PI/2f);
		}
		
		public static float InOut (float k) {
			return 0.5f*(1f - Mathf.Cos(Mathf.PI*k));
		}		
	};
	
	public class Exponential
	{		
		public static float In (float k) {
			return k == 0f? 0f : Mathf.Pow(1024f, k - 1f);
		}
		
		public static float Out (float k) {
			return k == 1f? 1f : 1f - Mathf.Pow(2f, -10f*k);
		}
		
		public static float InOut (float k) {
			if (k == 0f) return 0f;
			if (k == 1f) return 1f;
			if ((k *= 2f) < 1f) return 0.5f*Mathf.Pow(1024f, k - 1f);
			return 0.5f*(-Mathf.Pow(2f, -10f*(k - 1f)) + 2f);
		}		
	};
	
	public class Circular
	{		
		public static float In (float k) {
			return 1f - Mathf.Sqrt(1f - k*k);
		}
		
		public static float Out (float k) {
			return Mathf.Sqrt(1f - ((k -= 1f)*k));
		}
		
		public static float InOut (float k) {
			if ((k *= 2f) < 1f) return -0.5f*(Mathf.Sqrt(1f - k*k) - 1);
			return 0.5f*(Mathf.Sqrt(1f - (k -= 2f)*k) + 1f);
		}		
	};
	
	public class Elastic
	{
		public static float In (float k) {
			if (k == 0) return 0;
			if (k == 1) return 1;
			return -Mathf.Pow( 2f, 10f*(k -= 1f))*Mathf.Sin((k - 0.1f)*(2f*Mathf.PI)/0.4f);
		}
		
		public static float Out (float k) {
			if (k == 0) return 0;
			if (k == 1) return 1;
			return Mathf.Pow(2f, -10f*k)*Mathf.Sin((k - 0.1f)*(2f*Mathf.PI)/0.4f) + 1f;
		}
		
		public static float InOut (float k) {
			if ((k *= 2f) < 1f) return -0.5f*Mathf.Pow(2f, 10f*(k -= 1f))*Mathf.Sin((k - 0.1f)*(2f*Mathf.PI)/0.4f);
			return Mathf.Pow(2f, -10f*(k -= 1f))*Mathf.Sin((k - 0.1f)*(2f*Mathf.PI)/0.4f)*0.5f + 1f;
		}		
	};
	
	public class Back
	{
		static float s = 1.70158f;
		static float s2 = 2.5949095f;
		
		public static float In (float k) {
			return k*k*((s + 1f)*k - s);
		}
		
		public static float Out (float k) {
			return (k -= 1f)*k*((s + 1f)*k + s) + 1f;
		}
		
		public static float InOut (float k) {
			if ((k *= 2f) < 1f) return 0.5f*(k*k*((s2 + 1f)*k - s2));
			return 0.5f*((k -= 2f)*k*((s2 + 1f)*k + s2) + 2f);
		}		
	};
	
	public class Bounce
	{		
		public static float In (float k) {
			return 1f - Out(1f - k);
		}
		
		public static float Out (float k) {			
			if (k < (1f/2.75f)) {
				return 7.5625f*k*k;				
			}
			else if (k < (2f/2.75f)) {
				return 7.5625f*(k -= (1.5f/2.75f))*k + 0.75f;
			}
			else if (k < (2.5f/2.75f)) {
				return 7.5625f *(k -= (2.25f/2.75f))*k + 0.9375f;
			}
			else {
				return 7.5625f*(k -= (2.625f/2.75f))*k + 0.984375f;
			}
		}
		
		public static float InOut (float k) {
			if (k < 0.5f) return In(k*2f)*0.5f;
			return Out(k*2f - 1f)*0.5f + 0.5f;
		}		
	};
}