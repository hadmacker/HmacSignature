using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace HmacSignature
{
    public class InstanceSignatureBuilder
    {
        private readonly ISignatureCalculator _calculator;

        public InstanceSignatureBuilder(ISignatureCalculator calculator)
        {
            _calculator = calculator;
        }

        public SignatureCalculation Compute<T>(T input, string key)
        {
            var message = GetMessage(input, input.GetType());
            return _calculator.Calculate(message, key);
        }

        private readonly Dictionary<Type, Func<object, string>> _knownTypes = new Dictionary<Type, Func<object, string>>
        {
            {typeof(string), o => ((string)o ).Replace(" ", "") },
            {typeof(int), o => ((int)o).ToString(CultureInfo.InvariantCulture) },
            {typeof(short), o => ((short)o).ToString(CultureInfo.InvariantCulture) },
            {typeof(long), o => ((long)o).ToString(CultureInfo.InvariantCulture) },
            {typeof(decimal), o => ((decimal)o).ToString(CultureInfo.InvariantCulture) },
            {typeof(float), o => ((float)o).ToString(CultureInfo.InvariantCulture) },
            {typeof(double), o => ((double)o).ToString(CultureInfo.InvariantCulture) },
            {typeof(DateTime), o => ((DateTime)o).ToString("O").Replace(" ", "") },
            {typeof(bool), o => ((bool)o) ? "true" : "false" },
        };

        private string GetMessage<T>(T input, Type getType)
        {
            var stringBuilder = new StringBuilder();

            foreach (var propertyInfo in getType.GetProperties())
            {
                if (!propertyInfo.CanRead ||
                    !_knownTypes.TryGetValue(propertyInfo.PropertyType, out var formatter)) continue;

                SignatureAttribute attribute = null;
                if (Attribute.IsDefined(propertyInfo, typeof(SignatureAttribute)))
                    attribute = Attribute.GetCustomAttribute(propertyInfo, typeof(SignatureAttribute)) as SignatureAttribute;

                if(attribute == null || !attribute.Excluded)
                    stringBuilder.AppendFormat($"{propertyInfo.Name}{formatter.Invoke(propertyInfo.GetValue(input))}");
            }

            return stringBuilder.ToString();
        }

        public bool VerifyHex<T>(T input, string signatureAsHex, string key)
        {
            var computedSignature = Compute(input, key);
            return SignatureCalculation.TryHexDecode(signatureAsHex, out var signatureBytes)
                   && Compare(computedSignature.SignatureBytes, signatureBytes);
        }

        public bool VerifyBase64<T>(T input, string signatureAsBase64, string key)
        {
            var computedSignature = Compute(input, key);
            return SignatureCalculation.TryBase64Decode(signatureAsBase64, out var signatureBytes)
                   && Compare(computedSignature.SignatureBytes, signatureBytes);
        }

        private static bool Compare(byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length) 
                return false;
            return !a1
                .Where((t, i) => t != a2[i])
                .Any();
        }
    }
}
