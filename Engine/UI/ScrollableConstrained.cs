using System.Collections;
using UnityEngine;

namespace Engine.UI {

    public class ScrollablConstrained : GameObjectBehavior {

        //boundary coordinates
        public float eastBoundary;

        public float westBoundary;
        public float northBoundary;
        public float southBoundary;
        public float scrollModifier;

        private Boundary boundary;

        private void Update() {
            HandleScroll();
        }

        //see if player scrolled the map
        private void HandleScroll() {

            //check for player moving finger across screen
            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved) {

                //get distance to move
                Vector2 positionDelta = GetPositionDeltaWithinBoundary(Input.GetTouch(0).deltaPosition);

                //do translation
                camera.transform.Translate(-(positionDelta.x), -(positionDelta.y), 0);
            }
        }

        //this will get a position delta within the confines of a boundary
        //if it is not within the boundary, then the corresponding x/y value
        //will be locked, so movement will be stopped in that direction
        private Vector2 GetPositionDeltaWithinBoundary(Vector2 inDelta) {

            //get distance to move
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            float x = touchDeltaPosition.x * scrollModifier;
            float y = touchDeltaPosition.y * scrollModifier;

            //get camera in pixls
            Rect r = camera.pixelRect;

            //create boundary object
            boundary = new Boundary(camera, eastBoundary, westBoundary, northBoundary, southBoundary);

            //get x/y coordinates within the boundary
            x = GetAxisDeltaWithinBoundary(x, r.xMin, r.xMax, boundary.xMin, boundary.xMax);
            y = GetAxisDeltaWithinBoundary(y, r.yMin, r.yMax, boundary.yMin, boundary.yMax);

            return new Vector2(x, y);
        }

        //this will check the boundary for a particular x or y coordinate
        private float GetAxisDeltaWithinBoundary(float axisDelta, float recMin, float recMax, float boundaryMinPixels, float boundaryMaxPixels) {

            //get where edge of camera will have moved if full translate is done
            float boundaryMinPixelsDestination = recMin - Mathf.Abs(axisDelta);
            float boundaryMaxPixelsDestination = recMax + Mathf.Abs(axisDelta);

            //check to see if you're within the border
            if ((boundaryMinPixelsDestination <= boundaryMinPixels && axisDelta > 0) ||
                (boundaryMaxPixelsDestination >= boundaryMaxPixels && axisDelta < 0)) {
                axisDelta = 0;
            }
            return axisDelta;
        }

        //Helper class to encapsulate min/max values for each edge of Boundary.
        private class Boundary {
            public float xMin;
            public float xMax;
            public float yMin;
            public float yMax;

            public Boundary(Camera camera, float east, float west, float north, float south) {
                xMax = camera.WorldToScreenPoint(new Vector3(0, 0, east)).x;
                xMin = camera.WorldToScreenPoint(new Vector3(0, 0, west)).x;
                yMax = camera.WorldToScreenPoint(new Vector3(north, 0, 0)).y;
                yMin = camera.WorldToScreenPoint(new Vector3(south, 0, 0)).y;
            }

            public override string ToString() {
                return "xMin: " + xMin + " xMax: " + xMax + " yMin: " + yMin + " yMax: " + yMax;
            }
        }
    }
}