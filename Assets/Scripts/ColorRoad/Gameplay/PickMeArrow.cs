using System;
using DG.Tweening;
using UnityEngine;

namespace ColorRoad.Gameplay
{
    public class PickMeArrow : MonoBehaviour
    {
        private Tweener _tweener;

        private void Start()
        {
            _tweener = transform.DOMoveY(0.75f, 0.5f)
                .SetRelative(true)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void OnDestroy()
        {
            _tweener.Kill();
        }

        private void Update()
        {
            Vector3 target = Camera.main.transform.position;
            target.y = transform.position.y;
            transform.LookAt(target);
        }
    }
}