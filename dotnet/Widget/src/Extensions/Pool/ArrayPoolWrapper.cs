using System.Buffers;

namespace Extensions.Pool;

/// <summary>
/// Wrapper for ArrayPool that makes using ArrayPool concise and safe
/// </summary>
public struct ArrayPoolWrapper<T> : IDisposable
{
    private int _index = -1;
    private bool _disposed = false;
    private readonly int _capacity;

    private readonly T[] _pool;

    public ArrayPoolWrapper(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "The capacity must be greater than 0.");
        }

        this._capacity = capacity;
        _pool = ArrayPool<T>.Shared.Rent(capacity);
    }

    public readonly int Count => _index + 1;
    public readonly Span<T> Values => _pool.AsSpan()[..Count];


    public void Add(T info)
    {
        ThrowIfDisposed();

        _index++;
        if (_index >= _capacity)
        {
            _index--;

            throw new InvalidOperationException("The array pool has reached its capacity.");
        }

        _pool[_index] = info;
    }

    public void RemoveLastOne()
    {
        ThrowIfDisposed();

        if (Count <= 0)
        {
            throw new InvalidOperationException("The array pool is empty.");
        }

        _pool[_index] = default!;
        _index--;
    }

    public void Dispose()
    {
        ThrowIfDisposed();
        _disposed = true;

        ArrayPool<T>.Shared.Return(_pool);
    }


    private readonly void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(ArrayPoolWrapper<T>));
        }
    }
}
