using DG.Tweening;
using UnityEngine;

public class StartScript : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        var a = this.GetComponent<RectTransform>();
        a.DOAnchorPosX(0, 2, true).SetEase(Ease.OutBounce);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
