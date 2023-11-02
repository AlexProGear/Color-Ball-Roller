using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ColorRoad.UI
{
    public class InputMovementPanel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public event Action<Vector2> onInputChanged;
        private Vector2 lastInputPosition;
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            lastInputPosition = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 currentInputPosition = eventData.position;
            Vector2 delta = currentInputPosition - lastInputPosition;
            delta = new Vector2(delta.x / Screen.width, delta.y / Screen.height);
            onInputChanged?.Invoke(delta);
            lastInputPosition = currentInputPosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            onInputChanged?.Invoke(Vector2.zero);
        }
    }
}