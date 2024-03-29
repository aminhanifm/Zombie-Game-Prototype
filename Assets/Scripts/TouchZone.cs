using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace ZGP.Game{
    public class TouchZone : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [System.Serializable]
        public class Event : UnityEvent<Vector2> { }

        public RectTransform parentRect;
        public RectTransform handleRect;

        public bool clampToMagnitude;
        public float magnitudeMultiplier = 1f;
        public bool invertXOutputValue;
        public bool invertYOutputValue;
        public bool lockYValue;

        private Vector2 pointerDownPosition;
        private Vector2 currentPointerPosition;

        public Event touchZoneOutputEvent;

        void Start()
        {
            SetupHandle();
        }

        private void SetupHandle()
        {
            if(handleRect)
            {
                SetObjectActiveState(handleRect.gameObject, false); 
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {

            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, eventData.pressEventCamera, out pointerDownPosition);

            if(handleRect)
            {
                SetObjectActiveState(handleRect.gameObject, true);
                UpdateHandleRectPosition(pointerDownPosition);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {

            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, eventData.pressEventCamera, out currentPointerPosition);
            
            Vector2 positionDelta = GetDeltaBetweenPositions(pointerDownPosition, currentPointerPosition);

            Vector2 clampedPosition = ClampValuesToMagnitude(positionDelta);
            
            Vector2 outputPosition = ApplyInversionFilter(clampedPosition);

            OutputPointerEventValue(outputPosition * magnitudeMultiplier);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            pointerDownPosition = Vector2.zero;
            currentPointerPosition = Vector2.zero;

            OutputPointerEventValue(Vector2.zero);

            if(handleRect)
            {
                SetObjectActiveState(handleRect.gameObject, false);
                UpdateHandleRectPosition(Vector2.zero);
            }
        }

        void OutputPointerEventValue(Vector2 pointerPosition)
        {
            Vector2 modifiedPointerPos = lockYValue ? new Vector2(pointerPosition.x, 0) : pointerPosition;
            touchZoneOutputEvent.Invoke(modifiedPointerPos);
        }

        void UpdateHandleRectPosition(Vector2 newPosition)
        {
            handleRect.anchoredPosition = newPosition;
        }

        void SetObjectActiveState(GameObject targetObject, bool newState)
        {
            targetObject.SetActive(newState);
        }

        Vector2 GetDeltaBetweenPositions(Vector2 firstPosition, Vector2 secondPosition)
        {
            return secondPosition - firstPosition;
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
