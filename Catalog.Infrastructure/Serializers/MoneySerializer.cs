using Catalog.Domain.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Catalog.Infrastructure.Serializers;

public class MoneySerializer : SerializerBase<Money>
{
    public override Money Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        context.Reader.ReadStartDocument();
        
        decimal amount = 0;
        string currency = "UAH";
        
        while (context.Reader.ReadBsonType() != BsonType.EndOfDocument)
        {
            var fieldName = context.Reader.ReadName();
            switch (fieldName)
            {
                case "amount":
                    amount = (decimal)context.Reader.ReadDecimal128();
                    break;
                case "currency":
                    currency = context.Reader.ReadString();
                    break;
                default:
                    context.Reader.SkipValue();
                    break;
            }
        }
        
        context.Reader.ReadEndDocument();
        return new Money(amount, currency);
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Money value)
    {
        context.Writer.WriteStartDocument();
        context.Writer.WriteName("amount");
        context.Writer.WriteDecimal128(value.Amount);
        context.Writer.WriteName("currency");
        context.Writer.WriteString(value.Currency);
        context.Writer.WriteEndDocument();
    }
}