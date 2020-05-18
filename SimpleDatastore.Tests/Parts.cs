using System;
using Example.Domain;

namespace SimpleDatastore.Tests
{
    public static class Parts
    {
        public static Part SomeWidgetA => new Part { Id = new Guid("47ee8994-7a72-463f-ad8f-1a4b0f61ae16"), Name = "Some widget part A"};
        public static Part SomeWidgetB => new Part { Id = new Guid("90397722-a7e2-4615-951b-742662630fcf"), Name = "Some widget part B"};
        public static Part AnotherWidgetA => new Part { Id = new Guid("dd9bec1b-73ed-4ba7-bfde-2de6b2cc8ae0"), Name = "Another widget part A"};
    }
}