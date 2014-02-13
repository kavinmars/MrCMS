using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities;

namespace MrCMS.DataAccess.CustomCollections
{
    public class MrCMSSet<T> : ISet<T> where T : SystemEntity
    {
        private readonly HashSet<T> _backingSet;

        public MrCMSSet()
            : this(Enumerable.Empty<T>())
        {

        }
        public MrCMSSet(IEnumerable<T> enumerable)
        {
            _backingSet = new HashSet<T>();
            foreach (var item in enumerable)
                Add(item);
        }
        public IEnumerator<T> GetEnumerator()
        {
            return _backingSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void UnionWith(IEnumerable<T> other)
        {
            _backingSet.UnionWith(other);
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            _backingSet.IntersectWith(other);
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            _backingSet.ExceptWith(other);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            _backingSet.SymmetricExceptWith(other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return _backingSet.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return _backingSet.IsSupersetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return _backingSet.IsProperSupersetOf(other);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return _backingSet.IsProperSubsetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return _backingSet.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return _backingSet.SetEquals(other);
        }

        public bool Add(T item)
        {
            return !item.IsDeleted && _backingSet.Add(item);
        }

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        public void Clear()
        {
            _backingSet.Clear();
        }

        public bool Contains(T item)
        {
            return _backingSet.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _backingSet.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return _backingSet.Remove(item);
        }

        public int Count { get { return _backingSet.Count; } }
        public bool IsReadOnly { get { return false; } }
    }
}