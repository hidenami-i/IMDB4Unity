using MessagePack;

namespace IMDB4Unity.Tests
{
    [MessagePackObject()]
    public class MasterJobEntity
    {
        [Key(0)]
        public int Age { get; set; }

        [Key(1)]
        public string FirstName { get; set; }

        [Key(2)]
        public string LastName { get; set; }

        [IgnoreMember]
        public string FullName => FirstName + LastName;
    }
}