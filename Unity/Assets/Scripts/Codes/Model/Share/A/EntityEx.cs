using System;

namespace ET
{
    public static class EntitySystem
    {
        public static T FindChild<T>(this Entity self, Func<T, bool> match) where T : Entity
        {
            foreach (var child in self.Children.Values)
            {
                if (child is T t)
                {
                    if (match.Invoke(t))
                        return t;
                };
            }

            return null;
        }
    }
}