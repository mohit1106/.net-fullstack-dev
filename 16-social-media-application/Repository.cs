using System.Collections.Generic;
using System.Linq;

namespace MiniSocialMedia
{
    public class Repository<T> where T : class
    {
        private readonly List<T> _items = new();

        public void Add(T item) => _items.Add(item);

        public IReadOnlyList<T> GetAll() => _items.AsReadOnly();

        public T? Find(System.Predicate<T> match)
        {
            foreach (var it in _items)
                if (match(it)) return it;
            return null;
        }
    }
}
