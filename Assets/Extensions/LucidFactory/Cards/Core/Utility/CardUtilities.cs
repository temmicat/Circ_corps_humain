using UnityEngine;

namespace LucidFactory.Cards.Utility
{
    public static class CardUtilities
    {
        public static Vector3 GetPosOfCard(Vector2 screenPos, Canvas canvas) =>
            GetPosOfCard(screenPos, canvas, canvas.worldCamera);

        public static Vector3 GetPosOfCard(Vector2 screenPos, Canvas canvas, Camera camera)
        {
            switch (canvas.renderMode)
            {
                case RenderMode.ScreenSpaceCamera:
                    if (canvas.TryGetComponent(out RectTransform rectTransform) &&
                        RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, screenPos, camera, out Vector3 worldPos))
                    {
                        return worldPos;
                    }
                    return camera.ScreenPointToRay(screenPos).GetPoint(canvas.planeDistance);

                case RenderMode.ScreenSpaceOverlay:
                    return screenPos;

                case RenderMode.WorldSpace:
                    Plane plane = GetCanvasPlane(canvas);
                    var ray = camera.ScreenPointToRay(screenPos);

                    if (plane.Raycast(ray, out float distance))
                        return ray.GetPoint(distance);

                    return screenPos;
            }

            return screenPos;
        }



        private static readonly Vector3[] Corners = new Vector3[4];
        private static Plane GetCanvasPlane(Canvas canvas)
        {
            if (canvas.TryGetComponent(out UnityEngine.RectTransform rectTransform))
            {
                rectTransform.GetWorldCorners(Corners);
                return new Plane(Corners[0], Corners[1], Corners[2]);
            }

            return default;
        }
    }
}