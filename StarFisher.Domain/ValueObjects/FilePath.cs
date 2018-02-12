using System;
using System.IO;
using StarFisher.Domain.Common;

namespace StarFisher.Domain.ValueObjects
{
    public class FilePath : ValueObject<FilePath>
    {
        private FilePath(string filePath)
        {
            Value = filePath;
        }

        public string Value { get; }

        public static FilePath Create(string filePath, bool shouldExist)
        {
            if(!IsValid(filePath, shouldExist))
                throw new ArgumentException(nameof(filePath));

            return new FilePath(filePath);
        }

        public static bool IsValid(string filePath, bool shouldExist)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return false;

            try
            {
                var fileInfo = new FileInfo(filePath);

                return !shouldExist || fileInfo.Exists;
            }
            catch
            {
                return false;
            }
        }

        protected override bool EqualsCore(FilePath other)
        {
            return string.Equals(Value, other.Value, StringComparison.InvariantCultureIgnoreCase);
        }

        protected override int GetHashCodeCore()
        {
            return Value.ToLower().GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
