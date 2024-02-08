using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexUI : MonoBehaviour
{
    public static List<VertexUI> Selected = new List<VertexUI>();
    public static List<VertexUI> Hovered = new List<VertexUI>();
    public int VertexID = 0;
    public static Vector3 Midpoint = Vector3.zero;

    protected int selectID = 0;  //0 = not selected, 1 = hovered, 2 = selected
    public Sprite[] selectedImages;
    public SpriteRenderer rend;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// Should be called before Hovered is clear
    /// </summary>
    public static void ClearHovered()
    {
        for(int i = Hovered.Count-1; i >= 0; i--)
        {
            if (Hovered[i].selectID == 1)
            {
                Hovered[i].SetUnHovered();
                Hovered.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Should be called at the end of drag
    /// </summary>
    public static void SetAllHoveredAsSelected()
    {
        List<VertexUI>.Enumerator e1 = Hovered.GetEnumerator();
        while (e1.MoveNext())
        {
            e1.Current.SetHoveredToSelected();
        }



        CalculateMidpoint();

        ClearHovered();
    }

    public static void CalculateMidpoint()
    {
        Midpoint = Vector3.zero;
        int Count = Selected.Count;
        Debug.Log("Calculating midpoint: " + Count);
        List<VertexUI>.Enumerator e2 = Selected.GetEnumerator();
        while (e2.MoveNext())
        {
            Midpoint += e2.Current.transform.position;
            Debug.Log("Point #" + e2.Current.VertexID + " Pos: " + e2.Current.transform.position + " total: " + Midpoint);
        }
        Midpoint.z = 0f;
        Midpoint = Midpoint / Count;
    }

    /// <summary>
    /// Should be called when selected is cleared (right click)
    /// </summary>
    public static void ClearSelected()
    {
        for (int i = Selected.Count - 1; i >= 0; i--)
        {
            Selected[i].SetUnHovered();
        }
        for (int i = Hovered.Count - 1; i >= 0; i--)
        {
            Hovered[i].SetUnHovered();
        }
        Hovered.Clear();
        Selected.Clear();
    }

    /// <summary>
    /// Should be called when it overlaps 
    /// </summary>
    public void SetUnHovered()
    {
        selectID = 0;
    }

    /// <summary>
    /// Should be called when it overlaps 
    /// </summary>
    public void SetHovered()
    {
        selectID = 1;
        Hovered.Add(this);
    }

    /// <summary>
    /// Should be called from the static Hovered list when we want the hovered ones to be officially selected
    /// </summary>
    public void SetHoveredToSelected()
    {
        selectID = 2;
        Selected.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        rend.sprite = selectedImages[selectID];
    }
}
