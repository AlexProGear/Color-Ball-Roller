using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ColorRoad.UI
{
    public class TutorialScreen : MonoBehaviour
    {
        [SerializeField] private RectTransform view;
        private void Awake()
        {
            if (PlayerPrefs.GetInt("tutorial_finished", 0) == 1)
            {
                Destroy(gameObject);
                return;
            }

            view.DOScale(transform.localScale, 1f).From(Vector3.zero).SetEase(Ease.OutBack).OnComplete(() =>
            {
                GetComponent<Button>().onClick.AddListener(OnButtonClicked);
            });
        }

        private void OnButtonClicked()
        {
            PlayerPrefs.SetInt("tutorial_finished", 1);
            Destroy(gameObject);
        }
    }
}