using System;
using UnityEngine;
using XGame;

namespace ET.Client
{
    [FriendOf(typeof(OperaComponent))]
    public static class OperaComponentSystem
    {
        [ObjectSystem]
        public class OperaComponentAwakeSystem : AwakeSystem<OperaComponent>
        {
            protected override void Awake(OperaComponent self)
            {
                self.listenerId = InputComponent.Instance.AddListener(OnInput, self); 
            }

            private void OnInput(InputData obj, object arg)
            {
                var self = (OperaComponent)arg;
                switch (obj.eventType)
                {
                    case InputEventType.Click:
                        var hit = RayUtil.RayCast(Camera.main, obj.position, LayerMask.GetMask("Map"));
                        FindPathALMessage c2MPathfindingResult = new FindPathALMessage();
                        c2MPathfindingResult.Position = hit.HitInfo.point;
                        SessionHelper.Send(self.ClientScene(), c2MPathfindingResult, SessionType.Map);
                        break;
                }
            }
        }
        
        public class OperaComponentDestroySystem : DestroySystem<OperaComponent>
        {
            protected override void Destroy(OperaComponent self)
            {
                InputComponent.Instance.RemoveListener(self.listenerId);
            }
        }
        
    }
}