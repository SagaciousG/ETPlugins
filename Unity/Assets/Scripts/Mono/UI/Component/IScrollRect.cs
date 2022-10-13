using System;

namespace XGame
{
    public interface IScrollRect
    {
        void AddScrollListener(Action onScroll);
    }
}