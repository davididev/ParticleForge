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