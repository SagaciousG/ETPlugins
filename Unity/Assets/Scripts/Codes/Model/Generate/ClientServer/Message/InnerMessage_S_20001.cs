using ET;
using ProtoBuf;
using System.Collections.Generic;
namespace ET
{
	[Message(InnerMessage.A2M_ReloadAResponse)]
	[ProtoContract]
	public partial class A2M_ReloadAResponse: ProtoObject, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(G2G_LockAResponse))]
	[Message(InnerMessage.G2G_LockARequest)]
	[ProtoContract]
	public partial class G2G_LockARequest: ProtoObject, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public long Id { get; set; }

		[ProtoMember(2)]
		public string Address { get; set; }

	}

	[Message(InnerMessage.G2G_LockAResponse)]
	[ProtoContract]
	public partial class G2G_LockAResponse: ProtoObject, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(G2G_LockReleaseAResponse))]
	[Message(InnerMessage.G2G_LockReleaseARequest)]
	[ProtoContract]
	public partial class G2G_LockReleaseARequest: ProtoObject, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public long Id { get; set; }

		[ProtoMember(2)]
		public string Address { get; set; }

	}

	[Message(InnerMessage.G2G_LockReleaseAResponse)]
	[ProtoContract]
	public partial class G2G_LockReleaseAResponse: ProtoObject, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[Message(InnerMessage.G2M_SessionDisconnectALMessage)]
	[ProtoContract]
	public partial class G2M_SessionDisconnectALMessage: ProtoObject, IActorLocationMessage
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

	}

	[Message(InnerMessage.G2R_KickOutPlayerAResponse)]
	[ProtoContract]
	public partial class G2R_KickOutPlayerAResponse: ProtoObject, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[Message(InnerMessage.ObjectAddAResponse)]
	[ProtoContract]
	public partial class ObjectAddAResponse: ProtoObject, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(ObjectAddAResponse))]
	[Message(InnerMessage.ObjectAddARequest)]
	[ProtoContract]
	public partial class ObjectAddARequest: ProtoObject, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public long Key { get; set; }

		[ProtoMember(2)]
		public long InstanceId { get; set; }

	}

	[ResponseType(nameof(ObjectGetAResponse))]
	[Message(InnerMessage.ObjectGetARequest)]
	[ProtoContract]
	public partial class ObjectGetARequest: ProtoObject, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public long Key { get; set; }

	}

	[Message(InnerMessage.ObjectGetAResponse)]
	[ProtoContract]
	public partial class ObjectGetAResponse: ProtoObject, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public long InstanceId { get; set; }

	}

	[ResponseType(nameof(ObjectLockAResponse))]
	[Message(InnerMessage.ObjectLockARequest)]
	[ProtoContract]
	public partial class ObjectLockARequest: ProtoObject, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public long Key { get; set; }

		[ProtoMember(2)]
		public long InstanceId { get; set; }

		[ProtoMember(3)]
		public int Time { get; set; }

	}

	[Message(InnerMessage.ObjectLockAResponse)]
	[ProtoContract]
	public partial class ObjectLockAResponse: ProtoObject, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(ObjectQueryAResponse))]
	[Message(InnerMessage.ObjectQueryARequest)]
	[ProtoContract]
	public partial class ObjectQueryARequest: ProtoObject, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public string CollectionName { get; set; }

		[ProtoMember(2)]
		public long InstanceId { get; set; }

	}

	[ResponseType(nameof(ObjectRemoveAResponse))]
	[Message(InnerMessage.ObjectRemoveARequest)]
	[ProtoContract]
	public partial class ObjectRemoveARequest: ProtoObject, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public long Key { get; set; }

	}

	[Message(InnerMessage.ObjectRemoveAResponse)]
	[ProtoContract]
	public partial class ObjectRemoveAResponse: ProtoObject, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[Message(InnerMessage.ObjectUnLockAResponse)]
	[ProtoContract]
	public partial class ObjectUnLockAResponse: ProtoObject, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(ObjectUnLockAResponse))]
	[Message(InnerMessage.ObjectUnLockARequest)]
	[ProtoContract]
	public partial class ObjectUnLockARequest: ProtoObject, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public long Key { get; set; }

		[ProtoMember(2)]
		public long OldInstanceId { get; set; }

		[ProtoMember(3)]
		public long InstanceId { get; set; }

	}

	[ResponseType(nameof(G2R_KickOutPlayerAResponse))]
	[Message(InnerMessage.R2G_KickOutPlayerARequest)]
	[ProtoContract]
	public partial class R2G_KickOutPlayerARequest: ProtoObject, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public string Account { get; set; }

	}

	public static partial class InnerMessage
	{
		 public const ushort A2M_ReloadAResponse = 20002;
		 public const ushort G2G_LockARequest = 20003;
		 public const ushort G2G_LockAResponse = 20004;
		 public const ushort G2G_LockReleaseARequest = 20005;
		 public const ushort G2G_LockReleaseAResponse = 20006;
		 public const ushort G2M_SessionDisconnectALMessage = 20007;
		 public const ushort G2R_KickOutPlayerAResponse = 20008;
		 public const ushort ObjectAddAResponse = 20009;
		 public const ushort ObjectAddARequest = 20010;
		 public const ushort ObjectGetARequest = 20011;
		 public const ushort ObjectGetAResponse = 20012;
		 public const ushort ObjectLockARequest = 20013;
		 public const ushort ObjectLockAResponse = 20014;
		 public const ushort ObjectQueryARequest = 20015;
		 public const ushort ObjectRemoveARequest = 20016;
		 public const ushort ObjectRemoveAResponse = 20017;
		 public const ushort ObjectUnLockAResponse = 20018;
		 public const ushort ObjectUnLockARequest = 20019;
		 public const ushort R2G_KickOutPlayerARequest = 20020;
	}
}
