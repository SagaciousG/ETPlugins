using ET;
using ProtoBuf;
using System.Collections.Generic;
namespace ET
{
	[ResponseType(nameof(Actor_TransferALResponse))]
	[Message(OuterMessage.Actor_TransferALRequest)]
	[ProtoContract]
	public partial class Actor_TransferALRequest: ProtoObject, IActorLocationRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public int MapIndex { get; set; }

	}

	[Message(OuterMessage.Actor_TransferALResponse)]
	[ProtoContract]
	public partial class Actor_TransferALResponse: ProtoObject, IActorLocationResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(BenchmarkResponse))]
	[Message(OuterMessage.BenchmarkRequest)]
	[ProtoContract]
	public partial class BenchmarkRequest: ProtoObject, IRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

	}

	[Message(OuterMessage.BenchmarkResponse)]
	[ProtoContract]
	public partial class BenchmarkResponse: ProtoObject, IResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(G2C_EnterMapResponse))]
	[Message(OuterMessage.C2G_EnterMapRequest)]
	[ProtoContract]
	public partial class C2G_EnterMapRequest: ProtoObject, IRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public long UnitId { get; set; }

	}

	[ResponseType(nameof(C2G_EnterZoneResponse))]
	[Message(OuterMessage.C2G_EnterZoneRequest)]
	[ProtoContract]
	public partial class C2G_EnterZoneRequest: ProtoObject, IRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public int Zone { get; set; }

	}

	[Message(OuterMessage.C2G_EnterZoneResponse)]
	[ProtoContract]
	public partial class C2G_EnterZoneResponse: ProtoObject, IResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(M2C_ReloadResponse))]
	[Message(OuterMessage.C2M_ReloadRequest)]
	[ProtoContract]
	public partial class C2M_ReloadRequest: ProtoObject, IRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public string Account { get; set; }

		[ProtoMember(2)]
		public string Password { get; set; }

	}

	[Message(OuterMessage.C2M_StopALMessage)]
	[ProtoContract]
	public partial class C2M_StopALMessage: ProtoObject, IActorLocationMessage
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

	}

	[ResponseType(nameof(M2C_TestALResponse))]
	[Message(OuterMessage.C2M_TestALRequest)]
	[ProtoContract]
	public partial class C2M_TestALRequest: ProtoObject, IActorLocationRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public string request { get; set; }

	}

	[ResponseType(nameof(M2C_TestRobotCaseALResponse))]
	[Message(OuterMessage.C2M_TestRobotCaseALRequest)]
	[ProtoContract]
	public partial class C2M_TestRobotCaseALRequest: ProtoObject, IActorLocationRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public int N { get; set; }

	}

	[ResponseType(nameof(M2C_TransferMapALResponse))]
	[Message(OuterMessage.C2M_TransferMapALRequest)]
	[ProtoContract]
	public partial class C2M_TransferMapALRequest: ProtoObject, IActorLocationRequest
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

	}

	[Message(OuterMessage.G2C_EnterMapResponse)]
	[ProtoContract]
	public partial class G2C_EnterMapResponse: ProtoObject, IResponse
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public int Error { get; set; }

		[ProtoMember(3)]
		public string Message { get; set; }

		[ProtoMember(4)]
		public long MyId { get; set; }

	}

	[Message(OuterMessage.G2C_KickOutAMessage)]
	[ProtoContract]
	public partial class G2C_KickOutAMessage: ProtoObject, IActorMessage
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

	}

	[Message(OuterMessage.G2C_TestMessage)]
	[ProtoContract]
	public partial class G2C_TestMessage: ProtoObject, IMessage
	{
	}

	[Message(OuterMessage.G2C_TestHotfixMessage)]
	[ProtoContract]
	public partial class G2C_TestHotfixMessage: ProtoObject, IMessage
	{
		[ProtoMember(1)]
		public string Info { get; set; }

	}

	[Message(OuterMessage.HttpGetRouterResponse)]
	[ProtoContract]
	public partial class HttpGetRouterResponse: ProtoObject
	{
		[ProtoMember(1)]
		public List<string> Realms { get; set; }

		[ProtoMember(2)]
		public List<string> Routers { get; set; }

	}

	[Message(OuterMessage.M2C_CreateMyUnitAMessage)]
	[ProtoContract]
	public partial class M2C_CreateMyUnitAMessage: ProtoObject, IActorMessage
	{
		[ProtoMember(1)]
		public UnitInfo Unit { get; set; }

	}

	[Message(OuterMessage.M2C_CreateUnitsAMessage)]
	[ProtoContract]
	public partial class M2C_CreateUnitsAMessage: ProtoObject, IActorMessage
	{
		[ProtoMember(2)]
		public List<UnitInfo> Units { get; set; }

	}

	[Message(OuterMessage.M2C_ReloadResponse)]
	[ProtoContract]
	public partial class M2C_ReloadResponse: ProtoObject, IResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[Message(OuterMessage.M2C_RemoveUnitsAMessage)]
	[ProtoContract]
	public partial class M2C_RemoveUnitsAMessage: ProtoObject, IActorMessage
	{
		[ProtoMember(2)]
		public List<long> Units { get; set; }

	}

	[Message(OuterMessage.M2C_StopAMessage)]
	[ProtoContract]
	public partial class M2C_StopAMessage: ProtoObject, IActorMessage
	{
		[ProtoMember(1)]
		public int Error { get; set; }

		[ProtoMember(2)]
		public long Id { get; set; }

		[ProtoMember(3)]
		public Unity.Mathematics.float3 Position { get; set; }

		[ProtoMember(4)]
		public Unity.Mathematics.quaternion Rotation { get; set; }

	}

	[Message(OuterMessage.M2C_TestALResponse)]
	[ProtoContract]
	public partial class M2C_TestALResponse: ProtoObject, IActorLocationResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public string response { get; set; }

	}

	[Message(OuterMessage.M2C_TestRobotCaseALResponse)]
	[ProtoContract]
	public partial class M2C_TestRobotCaseALResponse: ProtoObject, IActorLocationResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public int N { get; set; }

	}

	[Message(OuterMessage.M2C_TransferMapALResponse)]
	[ProtoContract]
	public partial class M2C_TransferMapALResponse: ProtoObject, IActorLocationResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[Message(OuterMessage.MoveInfo)]
	[ProtoContract]
	public partial class MoveInfo: ProtoObject
	{
		[ProtoMember(1)]
		public List<Unity.Mathematics.float3> Points { get; set; }

		[ProtoMember(4)]
		public float A { get; set; }

		[ProtoMember(5)]
		public float B { get; set; }

		[ProtoMember(6)]
		public float C { get; set; }

		[ProtoMember(7)]
		public float W { get; set; }

		[ProtoMember(8)]
		public int TurnSpeed { get; set; }

	}

	[Message(OuterMessage.PingResponse)]
	[ProtoContract]
	public partial class PingResponse: ProtoObject, IResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public long Time { get; set; }

	}

	[ResponseType(nameof(PingResponse))]
	[Message(OuterMessage.PingRequest)]
	[ProtoContract]
	public partial class PingRequest: ProtoObject, IRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

	}

	[Message(OuterMessage.RouterSync)]
	[ProtoContract]
	public partial class RouterSync: ProtoObject
	{
		[ProtoMember(1)]
		public uint ConnectId { get; set; }

		[ProtoMember(2)]
		public string Address { get; set; }

	}

	[Message(OuterMessage.UnitInfo)]
	[ProtoContract]
	public partial class UnitInfo: ProtoObject
	{
		[ProtoMember(1)]
		public long UnitId { get; set; }

		[ProtoMember(2)]
		public int ConfigId { get; set; }

		[ProtoMember(3)]
		public int Type { get; set; }

		[ProtoMember(4)]
		public Unity.Mathematics.float3 Position { get; set; }

		[MongoDB.Bson.Serialization.Attributes.BsonDictionaryOptions(MongoDB.Bson.Serialization.Options.DictionaryRepresentation.ArrayOfArrays)]
		[ProtoMember(10)]
		public Dictionary<int, long> KV { get; set; }
		[ProtoMember(12)]
		public MoveInfo MoveInfo { get; set; }

		[ProtoMember(13)]
		public Unity.Mathematics.float3 Forward { get; set; }

	}

	public static partial class OuterMessage
	{
		 public const ushort Actor_TransferALRequest = 10002;
		 public const ushort Actor_TransferALResponse = 10003;
		 public const ushort BenchmarkRequest = 10004;
		 public const ushort BenchmarkResponse = 10005;
		 public const ushort C2G_EnterMapRequest = 10006;
		 public const ushort C2G_EnterZoneRequest = 10007;
		 public const ushort C2G_EnterZoneResponse = 10008;
		 public const ushort C2M_ReloadRequest = 10009;
		 public const ushort C2M_StopALMessage = 10010;
		 public const ushort C2M_TestALRequest = 10011;
		 public const ushort C2M_TestRobotCaseALRequest = 10012;
		 public const ushort C2M_TransferMapALRequest = 10013;
		 public const ushort G2C_EnterMapResponse = 10014;
		 public const ushort G2C_KickOutAMessage = 10015;
		 public const ushort G2C_TestMessage = 10016;
		 public const ushort G2C_TestHotfixMessage = 10017;
		 public const ushort HttpGetRouterResponse = 10018;
		 public const ushort M2C_CreateMyUnitAMessage = 10019;
		 public const ushort M2C_CreateUnitsAMessage = 10020;
		 public const ushort M2C_ReloadResponse = 10021;
		 public const ushort M2C_RemoveUnitsAMessage = 10022;
		 public const ushort M2C_StopAMessage = 10023;
		 public const ushort M2C_TestALResponse = 10024;
		 public const ushort M2C_TestRobotCaseALResponse = 10025;
		 public const ushort M2C_TransferMapALResponse = 10026;
		 public const ushort MoveInfo = 10027;
		 public const ushort PingResponse = 10028;
		 public const ushort PingRequest = 10029;
		 public const ushort RouterSync = 10030;
		 public const ushort UnitInfo = 10031;
	}
}
