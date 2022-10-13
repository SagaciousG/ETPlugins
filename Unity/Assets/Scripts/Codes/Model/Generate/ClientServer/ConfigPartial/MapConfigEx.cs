using System.Collections.Generic;

namespace ET
{
    public partial class MapConfigCategory
    {
        private MultiMap<int, MapConfig> _processMaps = new MultiMap<int, MapConfig>();
        // public override void AfterEndInit()
        // {
        //     foreach (MapConfig mapConfig in this.dict.Values)
        //     {
        //         this._processMaps.Add(mapConfig.Process, mapConfig);
        //     }
        // }

        // public List<MapConfig> GetByProcess(int process)
        // {
        //     if (!this._processMaps.TryGetValue(process, out var res))
        //     {
        //         foreach (MapConfig mapConfig in this.dict.Values)
        //         {
        //             mapConfig.InstanceId = new InstanceIdStruct(process, (uint) mapConfig.Id).ToLong();
        //             this._processMaps.Add(process, mapConfig);
        //         }
        //
        //         res = this._processMaps[process];
        //     }
        //     return res;
        // }
    }

    public partial class MapConfig
    {
        // public long InstanceId;
        // public override void AfterEndInit()
        // {
        //     InstanceIdStruct instanceIdStruct = new InstanceIdStruct(this.Process, (uint) this.Id);
        //     this.InstanceId = instanceIdStruct.ToLong();
        // }
    }
}