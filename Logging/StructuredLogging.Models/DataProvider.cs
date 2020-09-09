using System;

namespace StructuredLogging.Models
{
    public static class DataProvider
    {
        public static User User => new User
        {
            Name = "Test User",
            Email = "test.user@example.org"
        };

        public static ComplexObject ComplexObject => new ComplexObject
        {
            Id = Guid.NewGuid().ToString("n"),
            Timestamp = DateTime.UtcNow,
            Users = new[]
            {
                new User
                {
                    Name = "Test User",
                    Email = "test.user@example.org"
                },
                new User
                {
                    Name = "Another User",
                    Email = "another.user@example.org"
                }
            }
        };
    }
}
