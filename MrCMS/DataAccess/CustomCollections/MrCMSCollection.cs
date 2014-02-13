using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MrCMS.Entities;

namespace MrCMS.DataAccess.CustomCollections
{
    public class MrCMSCollection<T> : ICollection<T> where T : SystemEntity
    {
        private readonly Collection<T> _backingCollection;

        public MrCMSCollection()
            : this(new Collection<T>())
        {
        }

        public MrCMSCollection(IEnumerable<T> enumerable)
        {
            _backingCollection = new Collection<T>();
            foreach (var item in enumerable)
                Add(item);
        }
        public IEnumerator<T> GetEnumerator()
        {
            return _backingCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            if (!item.IsDeleted)
                _backingCollection.Add(item);
        }

        public void Clear()
        {
            _backingCollection.Clear();
        }

        public bool Contains(T item)
        {
            return _backingCollection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _backingCollection.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            if (_backingCollection.Contains(item))
                return _backingCollection.Remove(item);
            return false;
        }

        public int Count { get { return _backingCollection.Count; } }
        public bool IsReadOnly { get { return false; } }
    }
}