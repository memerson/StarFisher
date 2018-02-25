using System;
using System.Runtime.Serialization;

namespace StarFisher.Domain.Faults
{
    [Serializable]
    public class InvalidFilePathException : ApplicationException
    {
        public InvalidFilePathException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
        }

        private InvalidFilePathException(string message)
            : base(message)
        {
        }

        public static InvalidFilePathException WorkingDirectoryFilePathInvalid(string fileName)
        {
            fileName = fileName ?? @"UNKNOWN FILE NAME";
            var message =
                $@"Could not create a valid file path for the file {fileName} in the current working directory.";
            return new InvalidFilePathException(message);
        }
    }
}