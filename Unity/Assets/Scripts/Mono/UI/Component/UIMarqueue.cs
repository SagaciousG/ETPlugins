using UnityEngine;

namespace XGame
{
    public class UIMarqueue : MonoBehaviour
    {
        public enum Direction
        {
            Horizontal,
            Vertical
        }
        
        [Tooltip("滚动速度")]
        [Range(0.1f, 5)]
        public float Speed = 1;
        [Tooltip("循环间隔")]
        public float Duration;

        public Direction Orientation;
        public RectTransform Content;
        public RectTransform Viewport;

        private float _playTime;
        private float _curTime;
        private float _distance;
        private bool _isPlay;
        
        private void Start()
        {
            Content.pivot = Vector2.up;
            Content.anchorMax = Vector2.up;
            Content.anchorMin = Vector2.up;
            Content.anchoredPosition = Vector2.zero;
            Play();
        }

        public void Play()
        {
            _playTime = Time.time;
            _distance = 0;
            _isPlay = true;
            Content.anchoredPosition = Vector2.zero;
        }
        
        private void Update()
        {
            if (!_isPlay) return;
            if (Time.time - _playTime < Duration)
                return;
            var offset = Content.rect.size - Viewport.rect.size;
            offset.x = -1 * Mathf.Clamp(offset.x, 0, int.MaxValue);
            offset.y = Mathf.Clamp(offset.y, 0, int.MaxValue);
            switch (Orientation)
            {
                case Direction.Horizontal:
                    offset.y = 0;
                    break;
                case Direction.Vertical:
                    offset.x = 0;
                    break;
            }

            var distance = Vector2.Distance(offset, Vector2.zero);
            if (distance <= 0)
            {
                Content.anchoredPosition = Vector2.zero;
                return;
            }
            var section = Speed * 20 * Time.deltaTime;
            _distance += section;
            var progress = _distance / distance;
            if (progress > 1)
            {
                if (Time.time - _curTime < Duration)
                    return;
                Play();
            }

            progress = Mathf.Clamp01(progress);
            Content.anchoredPosition = Vector2.LerpUnclamped(Vector2.zero, offset, progress);

            _curTime = Time.time;
        }
    }
}