namespace TelepathyServerTest
{
	// This can't be an interface because users don't need to implement the
	// serialization functions, we'll code generate it for them when they omit it.
	public abstract class MessageBase
	{
		// De-serialize the contents of the reader into this message
		public virtual void Deserialize(NetworkReader reader) { }

		// Serialize the contents of this message into the writer
		public virtual void Serialize(NetworkWriter writer) { }
	}

	// ---------- General Typed Messages -------------------

	public class StringMessage : MessageBase
	{
		public string value;

		public StringMessage()
		{
		}

		public StringMessage(string v)
		{
			value = v;
		}

		public override void Deserialize(NetworkReader reader)
		{
			value = reader.ReadString();
		}

		public override void Serialize(NetworkWriter writer)
		{
			writer.Write(value);
		}
	}

	public class IntegerMessage : MessageBase
	{
		public int value;

		public IntegerMessage()
		{
		}

		public IntegerMessage(int v)
		{
			value = v;
		}

		public override void Deserialize(NetworkReader reader)
		{
			value = (int)reader.ReadPackedUInt32();
		}

		public override void Serialize(NetworkWriter writer)
		{
			writer.WritePackedUInt32((uint)value);
		}
	}

	public class DoubleMessage : MessageBase
	{
		public double value;

		public DoubleMessage()
		{
		}

		public DoubleMessage(double v)
		{
			value = v;
		}

		public override void Deserialize(NetworkReader reader)
		{
			value = reader.ReadDouble();
		}

		public override void Serialize(NetworkWriter writer)
		{
			writer.Write(value);
		}
	}

	public class EmptyMessage : MessageBase
	{
		public override void Deserialize(NetworkReader reader)
		{
		}

		public override void Serialize(NetworkWriter writer)
		{
		}
	}

	// ---------- Public System Messages -------------------

	public class ErrorMessage : MessageBase
	{
		public byte errorCode; // byte instead of int because NetworkServer uses byte anyway. saves bandwidth.

		public override void Deserialize(NetworkReader reader)
		{
			errorCode = reader.ReadByte();
		}

		public override void Serialize(NetworkWriter writer)
		{
			writer.Write(errorCode);
		}
	}

	public class ReadyMessage : EmptyMessage
	{
	}

	public class NotReadyMessage : EmptyMessage
	{
	}
}