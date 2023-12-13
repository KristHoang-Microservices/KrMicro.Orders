namespace KrMicro.Patterns.Iterator;

public interface IAbstractIterator<out T>
{
    bool IsDone { get; }
    T CurrentItem { get; }
    int Step { get; set; }
    T First();
    T Next();
}