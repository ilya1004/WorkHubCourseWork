using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace ChatService.Infrastructure.Configurations;

public static class MessageConfiguration
{
    public static void Configure()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(Message)))
        {
            BsonClassMap.RegisterClassMap<Message>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);

                // cm.MapIdMember(c => c.Id)
                //     .SetIdGenerator(GuidGenerator.Instance)
                //     .SetSerializer(new StringSerializer(BsonType.ObjectId));

                cm.MapMember(m => m.CreatedAt)
                    .SetDefaultValue(() => DateTime.UtcNow);
                    // .SetSerializer(new DateTimeSerializer(BsonType.DateTime));

                cm.MapMember(m => m.FileId).SetIgnoreIfNull(true);
                cm.MapMember(m => m.Text).SetIgnoreIfNull(true);
            });
        }
    }
}