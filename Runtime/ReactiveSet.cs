using System;
using System.Collections;
using System.Collections.Generic;


namespace ReactiveObjects
{
	public class ReactiveSet<TItem>
		: ISet<TItem>
		, IReadOnlyCollection<TItem>
		, IReactiveCollection<TItem>
	{
		private readonly HashSet<TItem> set;
		private readonly EqualityComparer<TItem> valueComparer;

		private List<Action> changeListeners;
		private List<Action<TItem, bool>> containListeners;
		private Dictionary<TItem, List<Action<bool>>> keyListeners;

		public int Count => set.Count;

		bool ICollection<TItem>.IsReadOnly => ((ICollection<TItem>)set).IsReadOnly;

		public void Listen(Action listener)
			=> ListenersUtility.AddListener(ref changeListeners, listener);

		public void Forget(Action listener)
			=> ListenersUtility.RemoveListener(changeListeners, listener);

		public void Listen(Action<TItem, bool> listener)
			=> ListenersUtility.AddListener(ref containListeners, listener);

		public void Forget(Action<TItem, bool> listener)
			=> ListenersUtility.RemoveListener(containListeners, listener);

		public void Listen(TItem key, Action<bool> listener)
			=> ListenersUtility.AddListener(ref keyListeners, key, listener);

		public void Forget(TItem key, Action<bool> listener)
			=> ListenersUtility.RemoveListener(keyListeners, key, listener);

		public IEnumerator<TItem> GetEnumerator() => set.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => set.GetEnumerator();

		void ICollection<TItem>.Add(TItem item)
			=> set.Add(item);

		public bool Add(TItem item)
		{
			if (!set.Add(item))
				return false;

			ListenersUtility.InvokeAllSafe(containListeners, item, true);
			ListenersUtility.InvokeAllSafe(changeListeners);
			return true;
		}

		public void ExceptWith(IEnumerable<TItem> other)
		{
			if (set.Count == 0)
				return;

			if (ReferenceEquals(other, set))
			{
				Clear();
				return;
			}

			int countWas = set.Count;
			foreach (var element in other)
				if (set.Remove(element))
					ListenersUtility.InvokeAllSafe(containListeners, element, false);

			if (countWas != set.Count)
				ListenersUtility.InvokeAllSafe(changeListeners);
		}

		public void IntersectWith(IEnumerable<TItem> other)
		{
			if (set.Count == 0)
				return;

			var otherSet = other as HashSet<TItem> ?? new HashSet<TItem>(other, valueComparer);

			foreach (var element in set)
				if (!otherSet.Contains(element))
					ListenersUtility.InvokeAllSafe(containListeners, element, false);

			var countWas = set.Count;
			set.IntersectWith(otherSet);
			if (countWas != set.Count)
				ListenersUtility.InvokeAllSafe(changeListeners);
		}

		public bool IsProperSubsetOf(IEnumerable<TItem> other)
			=> set.IsProperSubsetOf(other);

		public bool IsProperSupersetOf(IEnumerable<TItem> other)
			=> set.IsProperSupersetOf(other);

		public bool IsSubsetOf(IEnumerable<TItem> other)
			=> set.IsSubsetOf(other);

		public bool IsSupersetOf(IEnumerable<TItem> other)
			=> set.IsSupersetOf(other);

		public bool Overlaps(IEnumerable<TItem> other)
			=> set.Overlaps(other);

		public bool SetEquals(IEnumerable<TItem> other)
			=> set.SetEquals(other);

		public void SymmetricExceptWith(IEnumerable<TItem> other)
		{
			var countWas = set.Count;
			set.SymmetricExceptWith(other);

			foreach (var element in other)
				if (!Contains(element))
					ListenersUtility.InvokeAllSafe(containListeners, element, false);

			if (countWas != set.Count)
				ListenersUtility.InvokeAllSafe(changeListeners);
		}

		public void UnionWith(IEnumerable<TItem> other)
		{
			var countWas = set.Count;
			foreach (var element in other)
				if (set.Add(element))
					ListenersUtility.InvokeAllSafe(containListeners, element, true);

			if (countWas != set.Count)
				ListenersUtility.InvokeAllSafe(changeListeners);
		}

		public void Clear()
		{
			foreach (var element in set)
				ListenersUtility.InvokeAllSafe(containListeners, element, false);
			ListenersUtility.InvokeAllSafe(changeListeners);
			set.Clear();
		}

		public bool Contains(TItem item)
			=> set.Contains(item);

		public void CopyTo(TItem[] array, int arrayIndex)
			=> set.CopyTo(array, arrayIndex);

		public bool Remove(TItem item)
		{
			if (!set.Remove(item))
				return false;

			ListenersUtility.InvokeAllSafe(containListeners, item, false);
			ListenersUtility.InvokeAllSafe(changeListeners);
			return true;
		}
	}
}