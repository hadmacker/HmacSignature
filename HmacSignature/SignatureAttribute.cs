using System;

namespace HmacSignature
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SignatureAttribute : Attribute
    {
        public bool Excluded { get; set; }
    }
}