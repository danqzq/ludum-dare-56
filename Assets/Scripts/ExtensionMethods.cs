using System;
using System.Collections;
using UnityEngine;

public static class ExtensionMethods
{
    public static void DoAfter(this MonoBehaviour monoBehaviour, float seconds, Action action)
    {
        monoBehaviour.StartCoroutine(DoAfterCoroutine(seconds, action));
    }
    
    private static IEnumerator DoAfterCoroutine(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action();
    }
    
    public static void DoAfterUntil(this MonoBehaviour monoBehaviour, Func<bool> condition, Action action)
    {
        monoBehaviour.StartCoroutine(DoAfterUntilCoroutine(condition, action));
    }
    
    private static IEnumerator DoAfterUntilCoroutine(Func<bool> condition, Action action)
    {
        yield return new WaitUntil(condition);
        action();
    }
    
    public static Vector3 Horizontal(this Vector3 vector3)
    {
        return new Vector3(vector3.x, 0f, vector3.z);
    }
}