using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
    [ProtoContract]
    [Config]
    public partial class ZoneConfigCategory : ConfigSingleton<ZoneConfigCategory>, IMerge
    {
        [ProtoIgnore]
        [BsonIgnore]
        private Dictionary<int, ZoneConfig> dict = new Dictionary<int, ZoneConfig>();
		
        [BsonElement]
        [ProtoMember(1)]
        private List<ZoneConfig> list = new List<ZoneConfig>();
		
        public void Merge(object o)
        {
            ZoneConfigCategory s = o as ZoneConfigCategory;
            this.list.AddRange(s.list);
        }
		
		[ProtoAfterDeserialization]        
        public void ProtoEndInit()
        {
            foreach (ZoneConfig config in list)
            {
                config.AfterEndInit();
                this.dict.Add(config.Id, config);
            }
            this.list.Clear();
            
            this.AfterEndInit();
        }
		
        public ZoneConfig Get(int id)
        {
            this.dict.TryGetValue(id, out ZoneConfig item);

            if (item == null)
            {
                throw new Exception($"配置找不到，配置表名: {nameof (ZoneConfig)}，配置id: {id}");
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, ZoneConfig> GetAll()
        {
            return this.dict;
        }

        public ZoneConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [ProtoContract]
	public partial class ZoneConfig: ProtoObject, IConfig
	{
		/// <summary>Id</summary>
		[ProtoMember(1)]
		public int Id { get; set; }
		/// <summary>数据库地址</summary>
		[ProtoMember(2)]
		public string DBConnection { get; set; }
		/// <summary>数据库名</summary>
		[ProtoMember(3)]
		public string DBName { get; set; }
		/// <summary>说明</summary>
		[ProtoMember(4)]
		public string ShowName { get; set; }

	}
}
