namespace Silo.Application.Contracts;
public interface ISiloBaseValidator<T>
{
    void Validate(T instance);
}
