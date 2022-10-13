using ET;
using ProtoBuf;
using System.Collections.Generic;
namespace ET
{
	[Message(InnerMessage.G2C_AddOnlinePlayerAMessage)]
	[ProtoContract]
	public partial class G2C_AddOnlinePlayerAMessage: ProtoObject, IActorMessage
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public string PlayerAccount { get; set; }

		[ProtoMember(2)]
		public long GateActorId { get; set; }

	}

	[ResponseType(nameof(R2C_EnterZoneAResponse))]
	[Message(InnerMessage.R2C_EnterZoneARequest)]
	[ProtoContract]
	public partial class R2C_EnterZoneARequest: ProtoObject, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public int Zone { get; set; }

	}

	[Message(InnerMessage.R2C_EnterZoneAResponse)]
	[ProtoContract]
	public partial class R2C_EnterZoneAResponse: ProtoObject, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public long ZoneGateActorId { get; set; }

	}

	[Message(InnerMessage.R2C_IsOnlineAResponse)]
	[ProtoContract]
	public partial class R2C_IsOnlineAResponse: ProtoObject, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public int Online { get; set; }

		[ProtoMember(2)]
		public long UnitId { get; set; }

	}

	[ResponseType(nameof(R2C_IsOnlineAResponse))]
	[Message(InnerMessage.R2C_IsOnlineARequest)]
	[ProtoContract]
	public partial class R2C_IsOnlineARequest: ProtoObject, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public string Account { get; set; }

	}

	[Message(InnerMessage.R2C_RemoveOnlinePlayerAMessage)]
	[ProtoContract]
	public partial class R2C_RemoveOnlinePlayerAMessage: ProtoObject, IActorMessage
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public string PlayerAccount { get; set; }

	}

	[ResponseType(nameof(R2C_ServerZoneStateAResponse))]
	[Message(InnerMessage.R2C_ServerZoneStateARequest)]
	[ProtoContract]
	public partial class R2C_ServerZoneStateARequest: ProtoObject, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

	}

	[Message(InnerMessage.R2C_ServerZoneStateAResponse)]
	[ProtoContract]
	public partial class R2C_ServerZoneStateAResponse: ProtoObject, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public List<int> OnlineZones { get; set; }

		[ProtoMember(2)]
		public List<int> PlayerCount { get; set; }

	}

	[Message(InnerMessage.R2C_ZoneStateAMessage)]
	[ProtoContract]
	public partial class R2C_ZoneStateAMessage: ProtoObject, IActorMessage
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public int IsOnline { get; set; }

		[ProtoMember(2)]
		public int Zone { get; set; }

	}

	[ResponseType(nameof(R2G_GetLoginKeyAResponse))]
	[Message(InnerMessage.R2G_GetLoginKeyARequest)]
	[ProtoContract]
	public partial class R2G_GetLoginKeyARequest: ProtoObject, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public string Account { get; set; }

	}

	[Message(InnerMessage.R2G_GetLoginKeyAResponse)]
	[ProtoContract]
	public partial class R2G_GetLoginKeyAResponse: ProtoObject, IActorResponse
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

	[ResponseType(nameof(R2G_QueryZonePlayerAResponse))]
	[Message(InnerMessage.R2G_QueryZonePlayerARequest)]
	[ProtoContract]
	public partial class R2G_QueryZonePlayerARequest: ProtoObject, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public string Account { get; set; }

	}

	[Message(InnerMessage.R2G_QueryZonePlayerAResponse)]
	[ProtoContract]
	public partial class R2G_QueryZonePlayerAResponse: ProtoObject, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public List<ZonePlayer> ZonePlayers { get; set; }

	}

	[Message(InnerMessage.ZonePlayer)]
	[ProtoContract]
	public partial class ZonePlayer: ProtoObject
	{
		[ProtoMember(1)]
		public int Zone { get; set; }

		[ProtoMember(2)]
		public string RoleName { get; set; }

		[ProtoMember(3)]
		public int RoleLevel { get; set; }

	}

	public static partial class InnerMessage
	{
		 public const ushort G2C_AddOnlinePlayerAMessage = 20102;
		 public const ushort R2C_EnterZoneARequest = 20103;
		 public const ushort R2C_EnterZoneAResponse = 20104;
		 public const ushort R2C_IsOnlineAResponse = 20105;
		 public const ushort R2C_IsOnlineARequest = 20106;
		 public const ushort R2C_RemoveOnlinePlayerAMessage = 20107;
		 public const ushort R2C_ServerZoneStateARequest = 20108;
		 public const ushort R2C_ServerZoneStateAResponse = 20109;
		 public const ushort R2C_ZoneStateAMessage = 20110;
		 public const ushort R2G_GetLoginKeyARequest = 20111;
		 public const ushort R2G_GetLoginKeyAResponse = 20112;
		 public const ushort R2G_QueryZonePlayerARequest = 20113;
		 public const ushort R2G_QueryZonePlayerAResponse = 20114;
		 public const ushort ZonePlayer = 20115;
	}
}
