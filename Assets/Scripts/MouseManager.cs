using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{

    public GameObject highlightedObject;

    public GameObject selectedObject;

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
            if (SelectionEvent != null)
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
        selectedObject = null;
        var deselectedObject = e.clickedObject;
        //we could delegate or do the event handling in the object itself, let's keep it here for now
        //deselectedObject.transform.DOKill(true);
        deselectedObject.transform.DOBlendableLocalMoveBy(Vector3.down * 2, 0.5f);//TODO: This might force the object to certain X Y Z, if it's already moving from another source this could interfere

        var refs = deselectedObject.GetComponent<GetOutlineReferences>();
        if (refs != null)
        {
            var outlines = refs.OutlineReferences;
            foreach (var outline in outlines)
            {
                outline.color = 0;
            }
        }
    }

    private void MouseManager_SelectionEventHandler(object sender, ClickArgs e)
    {
        selectedObject = e.clickedObject;
        //we could delegate or do the event handling in the object itself, let's keep it here for now
        //e.clickedObject.transform.DOKill(true);
        e.clickedObject.transform.DOBlendableLocalMoveBy(Vector3.up * 2, 0.5f);//TODO: This might force the object to certain X Y Z, if it's already moving from another source this could interfere

        var refs = e.clickedObject.GetComponent<GetOutlineReferences>();
        if (refs != null)
        {
            var outlines = refs.OutlineReferences;
            foreach (var outline in outlines)
            {
                outline.color = 1;
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
            }
        }
        else
        {
            //if you did not hit a collider
            ClearHighlight();
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
