namespace ET.Server
{
    [ChildOf(typeof(MapComponent))]
    public class Map : Entity, IAwake<int>
    {
        public int ConfigId { get; set; }
        public MapConfig MapConfig => MapConfigCategory.Instance.Get(this.ConfigId);
    }
}