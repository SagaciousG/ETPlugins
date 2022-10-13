using System;

namespace ET
{
    public abstract class Object
    {
        public override string ToString()
        {
            try
            {
                return JsonHelper.ToJson(this);
            }
            catch
            {
                return $"{this.GetType().Name}";
            }
        }
    }
}