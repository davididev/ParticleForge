using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public class KeyframeData<T>
{
    [SerializeField]
    public int FrameNum = 1;

    [SerializeField]
    public T State;

    public static void AddOrUpdate(int frame, T data, List<KeyframeData<T>> ListRef)
    {
        List<KeyframeData<T>>.Enumerator e1 = ListRef.GetEnumerator();
        while(e1.MoveNext())
        {
            if(e1.Current.FrameNum == frame)
            {
                e1.Current.State = data;
                return;
            }
        }
        //Didn't find an entry in the list, let's create a new one.
        ListRef.Add(new KeyframeData<T>(frame, data));
    }

    /// <summary>
    /// Used to draw the timeline- just lists where the keyframes are
    /// </summary>
    /// <param name="ListRef"></param>
    /// <returns></returns>
    public static int[] GetKeyframeIDS(List<KeyframeData<T>> ListRef)
    {
        int[] returnValue = new int[ListRef.Count];
        List<KeyframeData<T>>.Enumerator e1 = ListRef.GetEnumerator();
        int i = 0;
        while(e1.MoveNext())
        {
            returnValue[i] = e1.Current.FrameNum;
            i++;
        }

        return returnValue;

    }

    /// <summary>
    /// Calculates the percentage between points in keyframes
    /// </summary>
    /// <param name="frame">Frame number to calculate from</param>
    /// <param name="data1">(out) Keyframe to the left in the timeline (both are identical if we're at a keyframe or if there are none to the right)</param>
    /// <param name="data2">(out) Keyframe to the left in the timeline (both are identical if we're at a keyframe or if there are none to the right)</param>
    /// <param name="lerp">(out) percentage between data1 and data2</param>
    /// <param name="ListRef">The linked list we're working with</param>
    public static void GetLerpAmount(int frame, out T data1, out T data2, out float lerp, List<KeyframeData<T>> ListRef)
    {
        
        List<KeyframeData<T>>.Enumerator e1 = ListRef.GetEnumerator();
        int prevKeyframe = -1;
        lerp = 0f;
        data1 = ListRef[0].State;
        data2 = ListRef[0].State;
        if (ListRef.Count == 1)  //Default, only one keyframe
        {
            return;
        }

        while (e1.MoveNext())
        {
            if (e1.Current.FrameNum == frame)
            {
                lerp = 1f;
                data1 = e1.Current.State;
                data2 = e1.Current.State;
                return;

            }

            
            if (e1.Current.FrameNum >= frame)  //Future keyframe detected
            {
                int wholePerc = (e1.Current.FrameNum - frame) * 100 / (e1.Current.FrameNum - prevKeyframe);
                lerp = wholePerc / 100f;
                Debug.Log(Time.time + ": Getting lerp: " + frame + " vs prev: " + prevKeyframe + "; right: " + e1.Current.FrameNum + ": " + lerp);
                data2 = e1.Current.State;
                return;
            }
            else
            {
                //Assume is previous keyframe until we verify the next one
                prevKeyframe = e1.Current.FrameNum;
                data1 = e1.Current.State;
                data2 = e1.Current.State;
            }
                
            
        }
    }

    public KeyframeData(int frame, T data)
    {
        FrameNum = frame;
        State = data;
    }
    
}
public class FrameNumComparer<T> : IComparer<KeyframeData<T>>
{
    public int Compare(KeyframeData<T> x, KeyframeData<T> y)
    {
        // Ensure x and y are not null to avoid null reference exception
        if (x == null && y == null)
            return 0;
        if (x == null)
            return -1;
        if (y == null)
            return 1;

        // Compare the FrameNum property of x with y
        return x.FrameNum.CompareTo(y.FrameNum);
    }
}