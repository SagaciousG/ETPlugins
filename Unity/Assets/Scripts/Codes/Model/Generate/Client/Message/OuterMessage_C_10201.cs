using ET;
using ProtoBuf;
using System.Collections.Generic;
namespace ET
{
	[ResponseType(nameof(GetMapSessionKeyResponse))]
	[Message(OuterMessage.GetMapSessionKeyRequest)]
	[ProtoContract]
	public partial class GetMapSessionKeyRequest: ProtoObject, IRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public long UnitId { get; set; }

	}

	[Message(OuterMessage.GetMapSessionKeyResponse)]
	[ProtoContract]
	public partial class GetMapSessionKeyResponse: ProtoObject, IResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public long MapKey { get; set; }

		[ProtoMember(2)]
		public string Address { get; set; }

	}

	[ResponseType(nameof(LoginInMapSessionResponse))]
	[Message(OuterMessage.LoginInMapSessionRequest)]
	[ProtoContract]
	public partial class LoginInMapSessionRequest: ProtoObject, IRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public long Key { get; set; }

		[ProtoMember(2)]
		public long UnitId { get; set; }

	}

	[Message(OuterMessage.LoginInMapSessionResponse)]
	[ProtoContract]
	public partial class LoginInMapSessionResponse: ProtoObject, IResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[Message(OuterMessage.StartSceneChangeAMessage)]
	[ProtoContract]
	public partial class StartSceneChangeAMessage: ProtoObject, IActorMessage
	{
		[ProtoMember(1)]
		public int MapId { get; set; }

		[ProtoMember(2)]
		public long MapActorId { get; set; }

	}

	public static partial class OuterMessage
	{
		 public const ushort GetMapSessionKeyRequest = 10202;
		 public const ushort GetMapSessionKeyResponse = 10203;
		 public const ushort LoginInMapSessionRequest = 10204;
		 public const ushort LoginInMapSessionResponse = 10205;
		 public const ushort StartSceneChangeAMessage = 10206;
	}
}
