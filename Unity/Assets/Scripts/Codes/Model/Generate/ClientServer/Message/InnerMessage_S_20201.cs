using ET;
using ProtoBuf;
using System.Collections.Generic;
namespace ET
{
	[ResponseType(nameof(G2M_GetSessionKeyAResponse))]
	[Message(InnerMessage.G2M_GetSessionKeyARequest)]
	[ProtoContract]
	public partial class G2M_GetSessionKeyARequest: ProtoObject, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public long UnitId { get; set; }

	}

	[Message(InnerMessage.G2M_GetSessionKeyAResponse)]
	[ProtoContract]
	public partial class G2M_GetSessionKeyAResponse: ProtoObject, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public long Key { get; set; }

	}

	[Message(InnerMessage.G2M_RemoveUnitAMessage)]
	[ProtoContract]
	public partial class G2M_RemoveUnitAMessage: ProtoObject, IActorMessage
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public long UnitId { get; set; }

	}

	public static partial class InnerMessage
	{
		 public const ushort G2M_GetSessionKeyARequest = 20202;
		 public const ushort G2M_GetSessionKeyAResponse = 20203;
		 public const ushort G2M_RemoveUnitAMessage = 20204;
	}
}
