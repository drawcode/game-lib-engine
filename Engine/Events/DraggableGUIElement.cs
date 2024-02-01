using System.Collections;
using Engine;
using UnityEngine;

namespace Engine.Events
{

    public class DraggableGUIElement : GameObjectBehavior
    {

        [System.Serializable]
        public class Border
        {

            public float minX, maxX, minY, maxY;
        }

        public Border border;
        private Vector3 lastMousePosition;

        private void OnMouseDown()
        {

            lastMousePosition = GetClampedMousePosition();
        }

        private Vector3 GetClampedMousePosition()
        {

            Vector3 mousePosition = Input.mousePosition;
            mousePosition.x = Mathf.Clamp(mousePosition.x, 0f, Screen.width);
            mousePosition.y = Mathf.Clamp(mousePosition.y, 0f, Screen.height);

            return mousePosition;
        }

        private void OnMouseDrag()
        {

            Vector3 delta = GetClampedMousePosition() - lastMousePosition;

            delta = Camera.main.ScreenToViewportPoint(delta);

            transform.position += delta;

            Vector3 position = transform.position;
            position.x = Mathf.Clamp(position.x, border.minX, border.maxX);
            position.y = Mathf.Clamp(position.y, border.minY, border.maxY);

            transform.position = position;

            lastMousePosition = GetClampedMousePosition();
        }
    }
}