using ColorRoad.Systems;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ColorRoad.UI
{
    public class CoinsIndicatorPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text countText;

        [Inject] private PlayerAccount playerAccount;

        private void Awake()
        {
            playerAccount.Coins.Subscribe(OnCoinsCountChanged).AddTo(this);
        }

        private void OnCoinsCountChanged(int value)
        {
            countText.text = value.ToString();
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }
    }
}