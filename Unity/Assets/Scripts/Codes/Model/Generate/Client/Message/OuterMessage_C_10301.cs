using ET;
using ProtoBuf;
using System.Collections.Generic;
namespace ET
{
	[Message(OuterMessage.FindPathALMessage)]
	[ProtoContract]
	public partial class FindPathALMessage: ProtoObject, IActorLocationMessage
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public Unity.Mathematics.float3 Position { get; set; }

	}

	[Message(OuterMessage.FindPathResultAMessage)]
	[ProtoContract]
	public partial class FindPathResultAMessage: ProtoObject, IActorMessage
	{
		[ProtoMember(1)]
		public Unity.Mathematics.float3 Position { get; set; }

		[ProtoMember(2)]
		public List<Unity.Mathematics.float3> Points { get; set; }

		[ProtoMember(3)]
		public long unitId { get; set; }

	}

	public static partial class OuterMessage
	{
		 public const ushort FindPathALMessage = 10302;
		 public const ushort FindPathResultAMessage = 10303;
	}
}
