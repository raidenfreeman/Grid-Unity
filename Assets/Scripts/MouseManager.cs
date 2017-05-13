using UnityEngine;

public class MouseManager : MonoBehaviour
{

    public GameObject highlightedObject;

    //public event 

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo))
        {
            //Debug.Log("Mouse is over: " + hitInfo.collider.name);

            // The collider we hit may not be the "root" of the object
            // You can grab the most "root-est" gameobject using
            // transform.root, though if your objects are nested within
            // a larger parent GameObject (like "All Units") then this might
            // not work.  An alternative is to move up the transform.parent
            // hierarchy until you find something with a particular component.

            GameObject hitObject = hitInfo.transform.root.gameObject;

            HighlightObject(hitObject);
        }
        else
        {
            ClearHighlight();
        }

        //if (Input.GetMouseButtonDown(0))
        //{

        //}

    }

    void HighlightObject(GameObject obj)
    {
        if (highlightedObject != null)
        {
            if (obj == highlightedObject)
            {
                return;
            }
            ClearHighlight();
        }

        highlightedObject = obj;
        var refs = highlightedObject.GetComponent<GetOutlineReferences>();
        if (refs == null)
        {
            return;
        }
        var outlinesInChildren = refs.OutlineReferences;
        //var outlinesInChildren = selectedObject.GetComponentsInChildren<cakeslice.Outline>();
        foreach (var outline in outlinesInChildren)
        {
            outline.enabled = true;
        }
    }

    void ClearHighlight()
    {
        if (highlightedObject == null)
        {
            return;
        }
        var refs = highlightedObject.GetComponent<GetOutlineReferences>();
        if (refs == null)
        {
            return;
        }
        var outlinesInChildren = refs.OutlineReferences;
        //var outlinesInChildren = selectedObject.GetComponentsInChildren<cakeslice.Outline>();
        foreach (var outline in outlinesInChildren)
        {
            outline.enabled = false;
        }
        highlightedObject = null;
    }
}
