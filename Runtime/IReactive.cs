using System;
using System.Collections.Generic;

namespace ReactiveObjects
{
	public interface IReactive
	{
		void Listen(Action listener);
		void Forget(Action listener);
	}

	public interface IReactive<out TValue> : IReactive
	{
		void Listen(Action<TValue> listener);
		void Forget(Action<TValue> listener);
	}

	public interface IReactive<TKey, out TValue> : IReactive
	{
		void Listen(Action<TKey, TValue> listener);
		void Forget(Action<TKey, TValue> listener);

		void Listen(TKey key, Action listener);
		void Forget(TKey key, Action listener);

		void Listen(TKey key, Action<TValue> listener);
		void Forget(TKey key, Action<TValue> listener);
	}

	public interface IReadOnlyReactive<out TValue> : IReactive<TValue>
	{
		TValue Value { get; }
	}

	public interface IReadOnlyReactive<TKey, TValue> : IReactive<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
	{
	}

	public interface IReadWriteReactive<TValue> : IReactive<TValue>
	{
		TValue Value { get; set; }
	}

	public interface IReadWriteReactive<TKey, TValue> : IReactive<TKey, TValue>, IDictionary<TKey, TValue>
	{
	}
}