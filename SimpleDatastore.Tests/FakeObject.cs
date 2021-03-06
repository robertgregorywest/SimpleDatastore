﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace SimpleDatastore.Tests
{
    public class FakeObject : PersistentObject<Guid>, IEquatable<FakeObject>, IComparable<FakeObject>
    {
        public const string IdentifierValue = "675b689d-db4e-43ed-94dd-591f73a0fc74";
        public const string NameValue = "FakeObject name";
        public const string RootCacheKey = "SimpleDatastore.Tests.FakeObject";
        public const string CacheKey = "SimpleDatastore.Tests.FakeObject.675b689d-db4e-43ed-94dd-591f73a0fc74";
        public const string IdentifierValue2 = "ab08bec7-835f-49ca-a285-6ba195576305";
        public const string NameValue2 = "Second FakeObject name";
        public const string NameValue2Updated = "Second FakeObject name updated";

        [DataMember(Name = "name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        public bool Equals(FakeObject other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((FakeObject)obj);
        }

        public override int GetHashCode()
        {
            return (Id, Name).GetHashCode();
        }

        public override string ToString() => Id.ToString();
        public int CompareTo(FakeObject other) => Id.CompareTo(other.Id);

        public static Guid InstanceIdentifier => new Guid(IdentifierValue);
        public static FakeObject Instance => new FakeObject() { Id = InstanceIdentifier, Name = NameValue };
        public static Guid SecondInstanceIdentifier => new Guid(IdentifierValue2);
        public static FakeObject SecondInstance => new FakeObject() { Id = SecondInstanceIdentifier, Name = NameValue2 };
        public static IList<FakeObject> Collection => new List<FakeObject> { SecondInstance, Instance };
    }
}
