namespace KrMicro.Patterns.Iterator;

public interface IAbstractCollection<out T>
{
    IAbstractIterator<T> CreateIterator();
}