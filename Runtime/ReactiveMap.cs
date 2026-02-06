using System;
using System.Collections;
using System.Collections.Generic;

namespace ReactiveObjects
{
	public class Reactive<TKey, TValue>
		: IReadOnlyReactive<TKey, TValue>
		, IReadWriteReactive<TKey, TValue>
	{
		private List<Action> changeListeners;
		private List<Action<TKey, TValue>> keyValueListeners;
		private Dictionary<TKey, List<Action>> keyedChangeListeners;
		private Dictionary<TKey, List<Action<TValue>>> keyedValueListeners;

		private readonly Dictionary<TKey, TValue> map;
		private readonly EqualityComparer<TValue> valueComparer;

		public bool IsReadOnly => false;
		public int Count => map.Count;

		public IReadOnlyCollection<TKey> Keys => map.Keys;
		IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => map.Keys;
		IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => map.Values;

		public IReadOnlyCollection<TValue> Values => map.Values;
		ICollection<TKey> IDictionary<TKey, TValue>.Keys => map.Keys;
		ICollection<TValue> IDictionary<TKey, TValue>.Values => map.Values;

		public Reactive() : this(0, null, null) { }
		public Reactive(IEqualityComparer<TKey> keyComparer) : this(0, keyComparer, null) { }
		public Reactive(EqualityComparer<TValue> valueComparer) : this(0, null, valueComparer) { }
		public Reactive(IEqualityComparer<TKey> keyComparer, EqualityComparer<TValue> valueComparer) : this(0, keyComparer, valueComparer) { }

		public Reactive(int capacity, IEqualityComparer<TKey> keyComparer = null, EqualityComparer<TValue> valueComparer = null)
		{
			map = new Dictionary<TKey, TValue>(capacity, keyComparer);
			this.valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;
		}

		public TValue this[TKey key]
		{
			get => map.TryGetValue(key, out var value) ? value : default;
			set => Set(key, value);
		}

		public void Set(TKey key, TValue value)
		{
			if (map.TryGetValue(key, out var oldValue) && valueComparer.Equals(oldValue, value))
				return;

			map[key] = value;
			Notify(key, value);
		}

		public void Listen(Action listener)
			=> ListenersUtility.AddListener(ref changeListeners, listener);

		public void Forget(Action listener)
			=> ListenersUtility.RemoveListener(changeListeners, listener);

		public void Listen(Action<TKey, TValue> listener)
			=> ListenersUtility.AddListener(ref keyValueListeners, listener);

		public void Forget(Action<TKey, TValue> listener)
			=> ListenersUtility.RemoveListener(keyValueListeners, listener);

		public void Listen(TKey key, Action listener)
			=> ListenersUtility.AddListener(ref keyedChangeListeners, key, listener);

		public void Forget(TKey key, Action listener)
			=> ListenersUtility.RemoveListener(keyedChangeListeners, key, listener);

		public void Listen(TKey key, Action<TValue> listener)
			=> ListenersUtility.AddListener(ref keyedValueListeners, key, listener);

		public void Forget(TKey key, Action<TValue> listener)
			=> ListenersUtility.RemoveListener(keyedValueListeners, key, listener);

		private void Notify(TKey key, TValue value)
		{
			ListenersUtility.InvokeAllSafe(changeListeners);
			ListenersUtility.InvokeAllSafe(keyValueListeners, key, value);
			ListenersUtility.InvokeAllSafe(keyedChangeListeners, key);
			ListenersUtility.InvokeAllSafe(keyedValueListeners, key, value);
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
			=> map.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
			=> map.GetEnumerator();

		public bool TryGetValue(TKey key, out TValue value)
			=> map.TryGetValue(key, out value);

		public bool ContainsKey(TKey key)
			=> map.ContainsKey(key);

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
			=> ((ICollection<KeyValuePair<TKey, TValue>>)map).Contains(item);

		public void Add(TKey key, TValue value)
		{
			map.Add(key, value);
			Notify(key, value);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
			=> Add(item.Key, item.Value);

		public bool Remove(TKey key)
		{
			if (!map.Remove(key))
				return false;

			Notify(key, default);
			return true;
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
			=> Remove(item.Key);

		public void Clear()
		{
			foreach (var key in map.Keys)
				Notify(key, default);

			map.Clear();
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
			=> ((ICollection<KeyValuePair<TKey, TValue>>)map).CopyTo(array, arrayIndex);
	}
}