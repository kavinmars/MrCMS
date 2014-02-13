using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities;

namespace MrCMS.DataAccess.CustomCollections
{
    public class MrCMSList<T> : IList<T> where T : SystemEntity
    {
        private readonly List<T> _backingList;

        public MrCMSList()
            : this(Enumerable.Empty<T>())
        {

        }
        public MrCMSList(IEnumerable<T> enumerable)
        {
            _backingList = new List<T>();
            foreach (var item in enumerable)
            {
                Add(item);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _backingList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            if (!item.IsDeleted)
                _backingList.Add(item);
        }

        public void Clear()
        {
            _backingList.Clear();
        }

        public bool Contains(T item)
        {
            return _backingList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _backingList.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return _backingList.Remove(item);
        }

        public int Count { get { return _backingList.Count; } }
        public bool IsReadOnly { get { return false; } }
        public int IndexOf(T item)
        {
            return _backingList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _backingList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _backingList.RemoveAt(index);
        }

        public T this[int index]
        {
            get { return _backingList[index]; }
            set { _backingList[index] = value; }
        }
    }
}