using cakeslice;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GetOutlineReferences : MonoBehaviour
{
    public List<Outline> OutlineReferences;

    Outline[] oldOutlines;
#if UNITY_EDITOR
    void Update()
    {
        if (!Application.isEditor || Application.isPlaying)
        {
            return;
        }
        var outlines = this.GetComponentsInChildren<Outline>();
        if (oldOutlines != outlines)
        {
            OutlineReferences = new List<Outline>(outlines);
            oldOutlines = outlines;
        }
    }
#endif
}
