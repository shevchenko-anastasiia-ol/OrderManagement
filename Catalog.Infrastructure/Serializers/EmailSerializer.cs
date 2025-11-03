using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Catalog.Domain.ValueObjects;

namespace Catalog.Infrastructure.Serializers;

public class EmailSerializer : SerializerBase<Email>
{
    public override Email Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var value = context.Reader.ReadString();
        return new Email(value);
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Email value)
    {
        context.Writer.WriteString(value.Value);
    }
}