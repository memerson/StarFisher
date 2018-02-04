﻿namespace StarFisher.Console.Menu.Common
{
    public interface IParameter<T>
    {
        Argument<T> GetArgument();

        void PrintInvalidArgumentMessage();

        bool IsRequired { get; }
    }
}