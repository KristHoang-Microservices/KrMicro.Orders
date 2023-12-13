namespace KrMicro.Patterns.Prototype;

public interface IPrototype<out T>
{
    public T ShallowCopy();
    public T DeepCopy();
}