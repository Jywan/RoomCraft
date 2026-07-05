using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoomCraft.UI
{
    /// <summary>
    /// 원형 휠에서 클릭, 드래그한 좌표를 극좌표로 변환해서 전달한다.
    /// hue = 중심 기준 각도, saturation = 중심 기준 거리(0~1)
    /// </summary>
    public class ColorWheelDragHandler : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        public Action<float, float> OnDragPolar;
        
        private RectTransform rect;

        private void Awake()
        {
             rect = GetComponent<RectTransform>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            HandleDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            HandleDrag(eventData);
        }

        private void HandleDrag(PointerEventData eventData)
        {
            Vector2 localPoint;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, eventData.position,
                    eventData.pressEventCamera, out localPoint)) return;

            float radius = rect.rect.width / 2;
            float dist = localPoint.magnitude / radius;
            float angle = Mathf.Atan2(localPoint.y, localPoint.x);
            float hue = angle / (Mathf.PI * 2f);
            if (hue < 0f) hue += 1f;
            
            OnDragPolar?.Invoke(hue, Mathf.Clamp01(dist));
        }
    }
}