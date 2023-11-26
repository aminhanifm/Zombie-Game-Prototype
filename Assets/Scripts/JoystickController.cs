using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace ZGP.Game{
    public class JoystickController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [System.Serializable]
        public class Event : UnityEvent<Vector2> { }

        public RectTransform parentRect;
        public RectTransform handleRect;

        public float joystickRange = 50f;
        public float magnitudeMultiplier = 1f;
        public bool invertXOutputValue;
        public bool invertYOutputValue;

        public Event joystickOutputEvent;
        public UnityEvent joystickDownEvent = new UnityEvent();
        public UnityEvent joystickUpEvent = new UnityEvent();

        void Start()
        {
            SetupHandle();
        }

        private void SetupHandle()
        {
            if(handleRect)
            {
                UpdateHandleRectPosition(Vector2.zero);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
            joystickDownEvent.Invoke();
        }

        public void OnDrag(PointerEventData eventData)
        {

            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, eventData.pressEventCamera, out Vector2 position);
            
            position = ApplySizeDelta(position);
            
            Vector2 clampedPosition = ClampValuesToMagnitude(position);

            Vector2 outputPosition = ApplyInversionFilter(position);

            OutputPointerEventValue(outputPosition * magnitudeMultiplier);

            if(handleRect)
            {
                UpdateHandleRectPosition(clampedPosition * joystickRange);
            }
            
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OutputPointerEventValue(Vector2.zero);

            if(handleRect)
            {
                UpdateHandleRectPosition(Vector2.zero);
            }

            joystickUpEvent.Invoke();
        }

        private void OutputPointerEventValue(Vector2 pointerPosition)
        {
            joystickOutputEvent.Invoke(pointerPosition);
        }

        private void UpdateHandleRectPosition(Vector2 newPosition)
        {
            handleRect.anchoredPosition = newPosition;
        }

        Vector2 ApplySizeDelta(Vector2 position)
        {
            float x = (position.x/parentRect.sizeDelta.x) * 2.5f;
            float y = (position.y/parentRect.sizeDelta.y) * 2.5f;
            return new Vector2(x, y);
        }

        Vector2 ClampValuesToMagnitude(Vector2 position)
        {
            return Vector2.ClampMagnitude(position, 1);
        }

        Vector2 ApplyInversionFilter(Vector2 position)
        {
            if(invertXOutputValue)
            {
                position.x = InvertValue(position.x);
            }

            if(invertYOutputValue)
            {
                position.y = InvertValue(position.y);
            }

            return position;
        }

        float InvertValue(float value)
        {
            return -value;
        }
        
    }
}
