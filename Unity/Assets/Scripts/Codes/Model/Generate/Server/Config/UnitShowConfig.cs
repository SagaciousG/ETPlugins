using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
    [ProtoContract]
    [Config]
    public partial class UnitShowConfigCategory : ConfigSingleton<UnitShowConfigCategory>, IMerge
    {
        [ProtoIgnore]
        [BsonIgnore]
        private Dictionary<int, UnitShowConfig> dict = new Dictionary<int, UnitShowConfig>();
		
        [BsonElement]
        [ProtoMember(1)]
        private List<UnitShowConfig> list = new List<UnitShowConfig>();
		
        public void Merge(object o)
        {
            UnitShowConfigCategory s = o as UnitShowConfigCategory;
            this.list.AddRange(s.list);
        }
		
		[ProtoAfterDeserialization]        
        public void ProtoEndInit()
        {
            foreach (UnitShowConfig config in list)
            {
                config.AfterEndInit();
                this.dict.Add(config.Id, config);
            }
            this.list.Clear();
            
            this.AfterEndInit();
        }
		
        public UnitShowConfig Get(int id)
        {
            this.dict.TryGetValue(id, out UnitShowConfig item);

            if (item == null)
            {
                throw new Exception($"配置找不到，配置表名: {nameof (UnitShowConfig)}，配置id: {id}");
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, UnitShowConfig> GetAll()
        {
            return this.dict;
        }

        public UnitShowConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [ProtoContract]
	public partial class UnitShowConfig: ProtoObject, IConfig
	{
		/// <summary>Id</summary>
		[ProtoMember(1)]
		public int Id { get; set; }
		/// <summary>模型</summary>
		[ProtoMember(2)]
		public string Model { get; set; }
		/// <summary>Name</summary>
		[ProtoMember(3)]
		public string Name { get; set; }
		/// <summary>Desc</summary>
		[ProtoMember(4)]
		public string Desc { get; set; }

	}
}
