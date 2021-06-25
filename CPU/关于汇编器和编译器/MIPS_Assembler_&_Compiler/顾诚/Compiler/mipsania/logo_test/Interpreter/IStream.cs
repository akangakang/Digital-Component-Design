
namespace Interpreter
{
    public interface IStream
    {
        char next();
        bool eof();
    }
}
