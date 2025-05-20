using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartingShapeButton : MonoBehaviour
{
    [SerializeField]
    int MyShapeID = 0;

    [SerializeField]
    Sprite normalSprite, selectedSprite;

    [SerializeField]
    Image imageRef;


    public static int CurrentSelectedShape = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnPressButton()
    {
        CurrentSelectedShape = MyShapeID;
    }

    /// <summary>
    /// Messaged called by NewFileWindow
    /// </summary>
    void HighlightButton()
    {
        CurrentSelectedShape = MyShapeID;
    }

    // Update is called once per frame
    void Update()
    {
        bool isSelected = (CurrentSelectedShape == MyShapeID);
        if (isSelected)
            imageRef.sprite = selectedSprite; 
        else
            imageRef.sprite = normalSprite;

    }
}
