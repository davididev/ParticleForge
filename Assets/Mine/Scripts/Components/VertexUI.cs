using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexUI : MonoBehaviour
{
    public static List<VertexUI> Selected;
    public static List<VertexUI> Hovered;
    public int VertexID = 0;

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
    public void ClearHovered()
    {
        if(selectID == 1)  //Is hovered but not selected
            selectID = 0;
    }

    /// <summary>
    /// Should be called when selected is cleared. 
    /// </summary>
    public void ClearSelected()
    {
        selectID = 0;
    }

    /// <summary>
    /// Should be called when it overlaps 
    /// </summary>
    public void SetHovered()
    {
        selectID = 1;
    }

    /// <summary>
    /// Should be called from the static Hovered list when we want the hovered ones to be officially selected
    /// </summary>
    public void SetHoveredToSelected()
    {
        selectID = 2;
    }

    // Update is called once per frame
    void Update()
    {
        rend.sprite = selectedImages[selectID];
    }
}
