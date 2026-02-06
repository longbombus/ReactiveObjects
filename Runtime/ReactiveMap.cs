using System;
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
		private readonly EqualityComparer<TValue> comparer;

		public TValue this[TKey key]
		{
			get => map.TryGetValue(key, out var value) ? value : default;
			set => Set(key, value);
		}

		public void Set(TKey key, TValue value)
		{
			if (map.TryGetValue(key, out var oldValue) && comparer.Equals(oldValue, value))
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
	}
}