using ET;
using ProtoBuf;
using System.Collections.Generic;
namespace ET
{
	[ResponseType(nameof(CreateRoleResponse))]
	[Message(OuterMessage.CreateRoleRequest)]
	[ProtoContract]
	public partial class CreateRoleRequest: ProtoObject, IRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public int RoleId { get; set; }

	}

	[Message(OuterMessage.CreateRoleResponse)]
	[ProtoContract]
	public partial class CreateRoleResponse: ProtoObject, IResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[Message(OuterMessage.EnterZoneResponse)]
	[ProtoContract]
	public partial class EnterZoneResponse: ProtoObject, IResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public string Address { get; set; }

		[ProtoMember(2)]
		public long GateKey { get; set; }

	}

	[ResponseType(nameof(EnterZoneResponse))]
	[Message(OuterMessage.EnterZoneRequest)]
	[ProtoContract]
	public partial class EnterZoneRequest: ProtoObject, IRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public int Zone { get; set; }

		[ProtoMember(2)]
		public string Account { get; set; }

	}

	[ResponseType(nameof(LoginResponse))]
	[Message(OuterMessage.LoginRequest)]
	[ProtoContract]
	public partial class LoginRequest: ProtoObject, IRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public string Account { get; set; }

		[ProtoMember(2)]
		public string Password { get; set; }

	}

	[Message(OuterMessage.LoginResponse)]
	[ProtoContract]
	public partial class LoginResponse: ProtoObject, IResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(LoginGateResponse))]
	[Message(OuterMessage.LoginGateRequest)]
	[ProtoContract]
	public partial class LoginGateRequest: ProtoObject, IRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public long Key { get; set; }

		[ProtoMember(2)]
		public long GateId { get; set; }

	}

	[Message(OuterMessage.LoginGateResponse)]
	[ProtoContract]
	public partial class LoginGateResponse: ProtoObject, IResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public long PlayerId { get; set; }

	}

	[ResponseType(nameof(RegistResponse))]
	[Message(OuterMessage.RegistRequest)]
	[ProtoContract]
	public partial class RegistRequest: ProtoObject, IRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public string Account { get; set; }

		[ProtoMember(2)]
		public string Password { get; set; }

	}

	[Message(OuterMessage.RegistResponse)]
	[ProtoContract]
	public partial class RegistResponse: ProtoObject, IResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(RoleListResponse))]
	[Message(OuterMessage.RoleListRequest)]
	[ProtoContract]
	public partial class RoleListRequest: ProtoObject, IRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

	}

	[Message(OuterMessage.RoleListResponse)]
	[ProtoContract]
	public partial class RoleListResponse: ProtoObject, IResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public List<SimpleUnit> Units { get; set; }

	}

	[ResponseType(nameof(SelectRoleResponse))]
	[Message(OuterMessage.SelectRoleRequest)]
	[ProtoContract]
	public partial class SelectRoleRequest: ProtoObject, IRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public long UnitId { get; set; }

	}

	[Message(OuterMessage.SelectRoleResponse)]
	[ProtoContract]
	public partial class SelectRoleResponse: ProtoObject, IResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[Message(OuterMessage.SimpleUnit)]
	[ProtoContract]
	public partial class SimpleUnit: ProtoObject
	{
		[ProtoMember(1)]
		public long UnitId { get; set; }

		[ProtoMember(2)]
		public int Level { get; set; }

		[ProtoMember(3)]
		public string Name { get; set; }

	}

	[Message(OuterMessage.ZoneInfo)]
	[ProtoContract]
	public partial class ZoneInfo: ProtoObject
	{
		[ProtoMember(1)]
		public int Zone { get; set; }

		[ProtoMember(2)]
		public int State { get; set; }

		[ProtoMember(3)]
		public string RoleName { get; set; }

		[ProtoMember(4)]
		public int RoleLevel { get; set; }

		[ProtoMember(5)]
		public int PlayerCount { get; set; }

	}

	[ResponseType(nameof(ZoneListResponse))]
	[Message(OuterMessage.ZoneListRequest)]
	[ProtoContract]
	public partial class ZoneListRequest: ProtoObject, IRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public string Account { get; set; }

	}

	[Message(OuterMessage.ZoneListResponse)]
	[ProtoContract]
	public partial class ZoneListResponse: ProtoObject, IResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public List<ZoneInfo> OnlineZones { get; set; }

		[ProtoMember(2)]
		public List<int> LatestEnterZones { get; set; }

	}

	public static partial class OuterMessage
	{
		 public const ushort CreateRoleRequest = 10102;
		 public const ushort CreateRoleResponse = 10103;
		 public const ushort EnterZoneResponse = 10104;
		 public const ushort EnterZoneRequest = 10105;
		 public const ushort LoginRequest = 10106;
		 public const ushort LoginResponse = 10107;
		 public const ushort LoginGateRequest = 10108;
		 public const ushort LoginGateResponse = 10109;
		 public const ushort RegistRequest = 10110;
		 public const ushort RegistResponse = 10111;
		 public const ushort RoleListRequest = 10112;
		 public const ushort RoleListResponse = 10113;
		 public const ushort SelectRoleRequest = 10114;
		 public const ushort SelectRoleResponse = 10115;
		 public const ushort SimpleUnit = 10116;
		 public const ushort ZoneInfo = 10117;
		 public const ushort ZoneListRequest = 10118;
		 public const ushort ZoneListResponse = 10119;
	}
}
