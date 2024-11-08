using System;
using UnityEngine;

namespace LucidFactory.UI
{
    public static class DraggableUtility
    {
        public static bool IsRectOver(this IDraggable draggable, RectTransform rectTransform) =>
            IsRectOver(draggable, rectTransform, draggable.Canvas);

        public static bool IsRectOver(this IDraggable draggable, RectTransform rectTransform, Canvas rectTransformCanvas)
        {
            while (!rectTransformCanvas.isRootCanvas)
                rectTransformCanvas = rectTransformCanvas.rootCanvas;

            var draggableCanvas = draggable.Canvas;
            while (!draggableCanvas.isRootCanvas)
                draggableCanvas = draggableCanvas.rootCanvas;

            Rect rect1 = GetScreenRectForRect(rectTransform, rectTransformCanvas);
            Rect rect2 = GetScreenRectForRect(draggable.RectTransform, draggableCanvas);

            // Debug.Log($"{rect1} => {rect2} = {rect1.Overlaps(rect2, true)}");
            return rect1.Overlaps(rect2);
        }

        private const int CORNER_COUNT = 4;

        private static readonly Vector3[] Corners = new Vector3[CORNER_COUNT];
        private static readonly Vector2[] LocalPoints = new Vector2[CORNER_COUNT];

        private static Rect GetScreenRectForRect(RectTransform rectTransform, Canvas canvas)
        {
            return canvas.renderMode switch
            {
                RenderMode.ScreenSpaceOverlay => GetScreenRectFromOverlayCanvas(rectTransform, canvas),
                RenderMode.ScreenSpaceCamera => GetScreenRectFromCameraCanvas(rectTransform, canvas),
                RenderMode.WorldSpace => GetScreenRectFromWorldCanvas(rectTransform, canvas),
                _ => rectTransform.rect,
            };
        }
        private static Rect GetScreenRectFromOverlayCanvas(RectTransform rectTransform, Canvas canvas)
        {
            rectTransform.GetWorldCorners(Corners);
            Vector2 size = rectTransform.rect.size * rectTransform.lossyScale;

            Rect rect = new Rect(Corners[0], size);

#if UNITY_EDITOR
            Vector2 a = new Vector2(rect.xMin, rect.yMin);
            Vector2 b = new Vector2(rect.xMin, rect.yMax);
            Vector2 c = new Vector2(rect.xMax, rect.yMax);
            Vector2 d = new Vector2(rect.xMax, rect.yMin);

            Debug.DrawLine(a ,b);
            Debug.DrawLine(b,c);
            Debug.DrawLine(c ,d);
            Debug.DrawLine(d ,a);
#endif

            return rect;// new Vector2(width, height));;
        }

        private static Rect GetScreenRectFromCameraCanvas(RectTransform rectTransform, Canvas canvas)
        {
            rectTransform.GetWorldCorners(Corners);

            Camera camera = canvas.worldCamera == null ? Camera.main : canvas.worldCamera;

            for (int i = 0; i < CORNER_COUNT; i++)
                LocalPoints[i] = RectTransformUtility.WorldToScreenPoint(camera, Corners[i]);


            float height = Mathf.Abs(LocalPoints[1].y - LocalPoints[0].y);
            float width = Mathf.Abs(LocalPoints[1].x - LocalPoints[2].x);
            Vector2 size = new(width, height);

            Rect rect = new Rect(LocalPoints[0], size);
#if UNITY_EDITOR
            Vector2 a = new Vector2(rect.xMin, rect.yMin);
            Vector2 b = new Vector2(rect.xMin, rect.yMax);
            Vector2 c = new Vector2(rect.xMax, rect.yMax);
            Vector2 d = new Vector2(rect.xMax, rect.yMin);

            Debug.DrawLine(a ,b);
            Debug.DrawLine(b,c);
            Debug.DrawLine(c ,d);
            Debug.DrawLine(d ,a);
#endif
            return rect;
        }
        private static Rect GetScreenRectFromWorldCanvas(RectTransform rectTransform, Canvas canvas)
        {
            rectTransform.GetWorldCorners(Corners);

            Camera camera = canvas.worldCamera == null ? Camera.main : canvas.worldCamera;

            for (int i = 0; i < CORNER_COUNT; i++)
                LocalPoints[i] = RectTransformUtility.WorldToScreenPoint(camera, Corners[i]);


            float height = Mathf.Abs(LocalPoints[1].y - LocalPoints[0].y);
            float width = Mathf.Abs(LocalPoints[1].x - LocalPoints[2].x);

            Rect rect = new Rect(LocalPoints[0], new(width, height));
            return rect;
        }


    }
}