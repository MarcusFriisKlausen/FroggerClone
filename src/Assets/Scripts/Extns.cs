using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class for smoothing something out over time, like moving from a to b (source: https://stackoverflow.com/questions/37223919/create-a-coroutine-to-fade-out-different-types-of-object)
public static class Extns
    {
    public static IEnumerator Tweeng( this float duration,
               System.Action<float> var, float aa, float zz )
        {
        float sT = Time.time;
        float eT = sT + duration;
        
        while (Time.time < eT)
            {
            float t = (Time.time-sT)/duration;
            var( Mathf.SmoothStep(aa, zz, t) );
            yield return null;
            }
        
        var(zz);
        }

    public static IEnumerator Tweeng( this float duration,
               System.Action<Vector3> var, Vector3 aa, Vector3 zz )
        {
        float sT = Time.time;
        float eT = sT + duration;
        
        while (Time.time < eT)
            {
            float t = (Time.time-sT)/duration;
            var( Vector3.Lerp(aa, zz, Mathf.SmoothStep(0f, 1f, t) ) );
            yield return null;
            }
        
        var(zz);
        }
}