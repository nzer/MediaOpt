using System.Security.Cryptography.X509Certificates;

namespace MediaOpt
{
    public interface IConverter
    {
        void Convert(string source, string target);
    }
}