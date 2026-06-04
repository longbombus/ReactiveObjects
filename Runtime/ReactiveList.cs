using System;
using System.Collections;
using System.Collections.Generic;

namespace ReactiveObjects
{
	public class ReactiveList<TItem>
		: IReadOnlyReactiveList<TItem>
		, IList<TItem>
	{
		private readonly List<TItem> list;
		private readonly EqualityComparer<TItem> comparer;
		private List<Action> changeListeners;
		private List<Action<int, TItem>> indexListeners;

		public int Count => list.Count;

		bool ICollection<TItem>.IsReadOnly => ((ICollection<TItem>)list).IsReadOnly;

		public TItem this[int index]
		{
			get => list[index];
			set
			{
				var oldValue = list[index];
				if (comparer.Equals(oldValue, value))
					return;

				list[index] = value;

				ListenersUtility.InvokeAllSafe(indexListeners, ~index, oldValue);
				ListenersUtility.InvokeAllSafe(indexListeners, index, value);
				ListenersUtility.InvokeAllSafe(changeListeners);
			}
		}

		public ReactiveList() : this(0, null) { }
		public ReactiveList(EqualityComparer<TItem> itemsComparer) : this(0, itemsComparer) { }
		public ReactiveList(int capacity, EqualityComparer<TItem> itemsComparer = null)
		{
			list = new List<TItem>(capacity);
			comparer = itemsComparer ?? EqualityComparer<TItem>.Default;
		}

		public void Listen(Action listener)
			=> ListenersUtility.AddListener(ref changeListeners, listener);

		public void Forget(Action listener)
			=> ListenersUtility.RemoveListener(changeListeners, listener);

		public void Listen(Action<int, TItem> listener)
			=> ListenersUtility.AddListener(ref indexListeners, listener);

		public void Forget(Action<int, TItem> listener)
			=> ListenersUtility.RemoveListener(indexListeners, listener);

		public IEnumerator<TItem> GetEnumerator()
			=> list.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
			=> list.GetEnumerator();

		public bool Contains(TItem item)
			=> list.Contains(item);

		public int IndexOf(TItem item)
			=> list.IndexOf(item);

		public void Add(TItem item)
		{
			var itemIndex = list.Count;
			list.Add(item);
			ListenersUtility.InvokeAllSafe(indexListeners, itemIndex, item);
			ListenersUtility.InvokeAllSafe(changeListeners);
		}

		public void Add(TItem item, int count)
		{
			var wasCount = list.Count;
			for (int i = 0; i < count; ++i)
				list.Add(item);

			if (list.Count == wasCount)
				return;

			NotifyIndicesPositive(wasCount);
			ListenersUtility.InvokeAllSafe(changeListeners);
		}

		public void AddRange(IEnumerable<TItem> items)
		{
			var wasCount = list.Count;
			list.AddRange(items);
			NotifyIndicesPositive(wasCount);
			ListenersUtility.InvokeAllSafe(changeListeners);
		}

		public void Insert(int index, TItem item)
		{
			list.Insert(index, item);
			NotifyIndicesPositive(index);
			ListenersUtility.InvokeAllSafe(changeListeners);
		}

		public bool Remove(TItem item)
		{
			var itemIndex = list.IndexOf(item);
			if (itemIndex == -1)
				return false;

			RemoveAt(item, itemIndex);
			return true;
		}

		public void RemoveAt(int index)
			=> RemoveAt(list[index], index);

		private void RemoveAt(TItem item, int index)
		{
			list.RemoveAt(index);
			ListenersUtility.InvokeAllSafe(indexListeners, ~index, item);
			NotifyIndicesPositive(index);
			ListenersUtility.InvokeAllSafe(changeListeners);
		}

		public void Clear()
		{
			NotifyIndicesNegative(0);
			list.Clear();
			ListenersUtility.InvokeAllSafe(changeListeners);
		}

		public void CopyTo(TItem[] array, int arrayIndex)
			=> list.CopyTo(array, arrayIndex);

		private void NotifyIndicesPositive(int beginIndex)
		{
			for (int i = beginIndex; i < list.Count; ++i)
				ListenersUtility.InvokeAllSafe(indexListeners, i, list[i]);
		}

		private void NotifyIndicesNegative(int beginIndex)
		{
			for (int i = beginIndex; i < list.Count; ++i)
				ListenersUtility.InvokeAllSafe(indexListeners, ~i, list[i]);
		}
	}
}