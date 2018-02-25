using System;
using System.IO;
using StarFisher.Domain.Common;
using StarFisher.Domain.Faults;

namespace StarFisher.Domain.ValueObjects
{
    public class DirectoryPath : ValueObject<DirectoryPath>
    {
        private DirectoryPath(string directoryPath)
        {
            Value = directoryPath;
        }

        public string Value { get; }

        public static DirectoryPath Create(string directoryPath)
        {
            if (!IsValid(directoryPath))
                throw new ArgumentException(nameof(directoryPath));

            return new DirectoryPath(directoryPath);
        }

        public static bool IsValid(string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
                return false;

            try
            {
                var directoryInfo = new DirectoryInfo(directoryPath);

                if (directoryInfo.Exists)
                    return true;

                directoryInfo.Create();
                directoryInfo.Delete();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CreateIfDoesNotExist()
        {
            Directory.CreateDirectory(Value);
        }

        public FilePath GetFilePathForFileInDirectory(string fileName, bool createDirectory, bool fileShouldExist)
        {
            if(!TryGetFilePathForFileInDirectory(fileName, createDirectory, fileShouldExist, out FilePath filePath))
                throw InvalidFilePathException.WorkingDirectoryFilePathInvalid(fileName);

            return filePath;
        }

        public bool TryGetFilePathForFileInDirectory(string fileName, bool createDirectory, bool fileShouldExist,
            out FilePath filePath)
        {
            filePath = null;

            if (createDirectory)
                CreateIfDoesNotExist();

            var path = Path.Combine(Value, fileName);

            if (!FilePath.IsValid(path, fileShouldExist))
                return false;

            filePath = FilePath.Create(path, fileShouldExist);

            return true;
        }

        protected override bool EqualsCore(DirectoryPath other)
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