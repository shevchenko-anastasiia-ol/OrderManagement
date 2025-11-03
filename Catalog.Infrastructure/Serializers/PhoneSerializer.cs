using Catalog.Domain.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Catalog.Infrastructure.Serializers;

public class PhoneSerializer : SerializerBase<Phone>
{
    public override Phone Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var value = context.Reader.ReadString();
        return new Phone(value);
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Phone value)
    {
        context.Writer.WriteString(value.Value);
    }
}