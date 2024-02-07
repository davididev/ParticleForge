using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexSelector : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public StartingShapeHolder startingShape;
    
    // Start is called before the first frame update
    void OnEnable()
    {
        spriteRenderer.enabled = false;
        //spriteRenderer.sprite.bounds.Contains()
    }

    Vector3 startingHighlightPosition;
    // Update is called once per frame
    void Update()
    {
        //Start select
        if (Input.GetMouseButtonDown(0))
        {
            spriteRenderer.enabled = true;
            Vector3 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            v.z = 0f;
            transform.position = v;
            startingHighlightPosition = Input.mousePosition;
        }
        //End select
        if (Input.GetMouseButtonUp(0))
        {
            spriteRenderer.enabled = false;
            VertexUI.SetAllHoveredAsSelected();
        }

        //Reset selection
        if(Input.GetMouseButtonDown(1))
        {
            VertexUI.ClearSelected();
        }

        //On drag
        if(spriteRenderer.enabled == true)
        {
            //Select the list
            Vector3 worldRel = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.ScreenToWorldPoint(startingHighlightPosition);
            worldRel.z = 0f;
            spriteRenderer.size = new Vector2(worldRel.x, worldRel.y);

            //Highlight the list
            VertexUI.ClearHovered();
            Vector3 halfSize = new Vector3(spriteRenderer.size.x / 2f, spriteRenderer.size.y / 2f, 1f);
            Bounds b = spriteRenderer.bounds;
            List<VertexUI>.Enumerator e1 = StartingShapeHolder.VertexUIList.GetEnumerator();
            while(e1.MoveNext())
            {
                Vector3 v1 = e1.Current.transform.position;
                v1.z = 0f;
                if (b.Contains(v1))
                {
                    e1.Current.SetHovered();
                }
            }
        }
            
            
    }
}
