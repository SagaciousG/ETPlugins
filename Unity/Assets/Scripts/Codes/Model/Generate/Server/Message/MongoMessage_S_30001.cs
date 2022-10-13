using ET;
using ProtoBuf;
using System.Collections.Generic;
namespace ET
{
	[Message(MongoMessage.DB_AccountExistAResponse)]
	[ProtoContract]
	public partial class DB_AccountExistAResponse: ProtoObject, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public string Message { get; set; }

		[ProtoMember(2)]
		public int Error { get; set; }

	}

	[Message(MongoMessage.DB_CreateUnitAResponse)]
	[ProtoContract]
	public partial class DB_CreateUnitAResponse: ProtoObject, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public Unit Unit { get; set; }

	}

	[Message(MongoMessage.DB_GetUnitAResponse)]
	[ProtoContract]
	public partial class DB_GetUnitAResponse: ProtoObject, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public Unit Unit { get; set; }

	}

	[Message(MongoMessage.DB_LoginAResponse)]
	[ProtoContract]
	public partial class DB_LoginAResponse: ProtoObject, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public string Message { get; set; }

		[ProtoMember(2)]
		public int Error { get; set; }

	}

	[Message(MongoMessage.DB_RegistAResponse)]
	[ProtoContract]
	public partial class DB_RegistAResponse: ProtoObject, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public string Message { get; set; }

		[ProtoMember(2)]
		public int Error { get; set; }

	}

	[Message(MongoMessage.DB_SaveUnitAMessage)]
	[ProtoContract]
	public partial class DB_SaveUnitAMessage: ProtoObject, IActorMessage
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public Unit Unit { get; set; }

	}

	[Message(MongoMessage.ObjectQueryAResponse)]
	[ProtoContract]
	public partial class ObjectQueryAResponse: ProtoObject, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public Entity entity { get; set; }

	}

	public static partial class MongoMessage
	{
		 public const ushort DB_AccountExistAResponse = 30002;
		 public const ushort DB_CreateUnitAResponse = 30003;
		 public const ushort DB_GetUnitAResponse = 30004;
		 public const ushort DB_LoginAResponse = 30005;
		 public const ushort DB_RegistAResponse = 30006;
		 public const ushort DB_SaveUnitAMessage = 30007;
		 public const ushort ObjectQueryAResponse = 30008;
	}
}
