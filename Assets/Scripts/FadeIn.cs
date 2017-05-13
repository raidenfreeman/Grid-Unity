using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FadeIn : MonoBehaviour
{
    [SerializeField]
    Text t;

    [SerializeField]
    Button b;
    void Start()
    {
        if (t != null && b != null)
        {
            DOTween.ToAlpha(() => t.color, x => t.color = x, 1, 1).SetEase(Ease.InQuint).SetDelay(1).OnComplete(() => b.interactable = true);
        }
    }
}
