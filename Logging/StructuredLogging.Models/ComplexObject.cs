using System;
using System.Collections.Generic;

namespace StructuredLogging.Models
{
    public class ComplexObject
    {
        public string Id { get; set; }
        public DateTime Timestamp { get; set; }

        public ICollection<User> Users { get; set;}
    }
}
