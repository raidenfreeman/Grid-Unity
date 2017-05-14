using System;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{

    public GameObject highlightedObject;

    public GameObject selectedObject;

    public GameObject SelectionSquare;

    public event EventHandler<ClickArgs> ClickEvent;
    public event EventHandler<ClickArgs> SelectionEvent;
    public event EventHandler<ClickArgs> DeselectionEvent;

    private List<GameObject> GameObjectsTweenQueue;

    public class ClickArgs : EventArgs
    {
        public readonly GameObject clickedObject;
        public ClickArgs(GameObject obj)
        {
            clickedObject = obj;
        }
    }

    private void Start()
    {
        SelectionEvent += MouseManager_SelectionEventHandler;
        DeselectionEvent += MouseManager_DeselectionEvent;
        ClickEvent += MouseManager_ClickEvent;
    }

    private void MouseManager_ClickEvent(object sender, ClickArgs e)
    {
        var clickedObject = e.clickedObject;

        //if you clicked an object that is not selected already
        if (selectedObject != clickedObject)
        {
            //clear previous selection
            if (DeselectionEvent != null && selectedObject != null)
            {
                DeselectionEvent.Invoke(this, new ClickArgs(selectedObject));
            }
            //select the object
            if (SelectionEvent != null && clickedObject != null)
            {
                SelectionEvent.Invoke(this, new ClickArgs(clickedObject));
            }
        }
        else
        {
            //if you clicked the selected object
            //deselect the object
            if (DeselectionEvent != null)
            {
                DeselectionEvent.Invoke(this, new ClickArgs(clickedObject));
            }
        }
    }

    private void MouseManager_DeselectionEvent(object sender, ClickArgs e)
    {
        if (selectedObject == null)
        {
            return;
        }

        if (SelectionSquare != null)
        {
            SelectionSquare.SetActive(false);
        }

        selectedObject = null;
        var deselectedObject = e.clickedObject;

        //we could delegate or do the event handling in the object itself, let's keep it here for now
        //deselectedObject.transform.DOBlendableLocalMoveBy(Vector3.down * 2, 0.5f);//TODO: This might force the object to certain X Y Z, if it's already moving from another source this could interfere

        var refs = deselectedObject.GetComponent<GetOutlineReferences>();
        if (refs != null)
        {
            var outlines = refs.OutlineReferences;
            foreach (var outline in outlines)
            {
                outline.color = 0;
                outline.enabled = false;
            }
        }
    }

    private void MouseManager_SelectionEventHandler(object sender, ClickArgs e)
    {
        selectedObject = e.clickedObject;

        Vector3 selectionCenter;
        if (SelectionSquare != null)
        {
            SelectionSquare.SetActive(true);
            selectionCenter = e.clickedObject.transform.position;
            var renderers = selectedObject.GetComponentsInChildren<Renderer>();
            if (renderers != null)
            {
                Bounds totalBounds = new Bounds();
                foreach (var renderer in renderers)
                {
                    if (totalBounds.extents == Vector3.zero)
                    {
                        totalBounds = renderer.bounds;
                    }
                    else
                    {
                        totalBounds.Encapsulate(renderer.bounds);
                    }
                }
                var x = totalBounds.size;
                x.y = 1;
                SelectionSquare.transform.localScale = x;
                var boundsCenter = totalBounds.center;
                boundsCenter.y = 0;
                selectionCenter = boundsCenter;
            }
            SelectionSquare.transform.position = selectionCenter;
        }
        //we could delegate or do the event handling in the object itself, let's keep it here for now
        //e.clickedObject.transform.DOBlendableLocalMoveBy(Vector3.up * 2, 0.5f);//TODO: This might force the object to certain X Y Z, if it's already moving from another source this could interfere

        var refs = e.clickedObject.GetComponent<GetOutlineReferences>();
        if (refs != null)
        {
            var outlines = refs.OutlineReferences;
            foreach (var outline in outlines)
            {
                outline.color = 1;
                outline.enabled = true;
            }
        }
    }

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

            var outlineRefs = hitObject.GetComponent<GetOutlineReferences>();
            if (outlineRefs != null && outlineRefs.IsClickable == true)//TODO: Change that with a way to know if the object is interactible
            {
                HighlightObject(hitObject);

                if (Input.GetMouseButtonDown(0))
                {
                    if (ClickEvent != null)
                    {
                        ClickEvent.Invoke(this, new ClickArgs(hitObject));
                    }
                }
            }
            else
            {
                //if you hit something, but it's not clickable
                ClearHighlight();
                if (Input.GetMouseButtonDown(0) && selectedObject != null)
                {
                    ClickEvent.Invoke(this, new ClickArgs(null));
                }
            }
        }
        else
        {
            //if you did not hit a collider
            ClearHighlight();
            if (Input.GetMouseButtonDown(0) && selectedObject != null)
            {
                DeselectionEvent.Invoke(this, new ClickArgs(selectedObject));
            }
        }
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
        if (highlightedObject != selectedObject)
        {
            //if it's not selected, disable outline
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
        }
        highlightedObject = null;
    }
}
