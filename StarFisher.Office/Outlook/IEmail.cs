using System;

namespace StarFisher.Office.Outlook
{
    public interface IEmail : IDisposable
    {
        void Display();
    }
}