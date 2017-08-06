using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTransfer.Library
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class IgnorePropertyAttribute : Attribute
    {
        public IgnorePropertyAttribute(bool ignore)
        {
            Value = ignore;
        }

        public bool Value { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ChangePropertyDataTypeAttribute : Attribute
    {
        public ChangePropertyDataTypeAttribute(Type type)
        {
            Value = type;
        }
        public Type Value { get; set; }
    }
}
