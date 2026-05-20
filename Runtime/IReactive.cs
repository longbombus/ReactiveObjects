using System;
using System.Collections.Generic;

namespace ReactiveObjects
{
	/// <summary> Interface of something listenable. </summary>
	public interface IReactive
	{
		/// <summary> Subscribes on any change. </summary>
		/// <param name="listener"> Reaction on change. </param>
		void Listen(Action listener);

		/// <summary> Unsubscribes from change. </summary>
		/// <param name="listener"> Reaction on change that you used to subscribe. </param>
		void Forget(Action listener);
	}

	/// <summary> Interface of listenable value. </summary>
	/// <typeparam name="TValue"> Type of the changeable value. Notice that reference type internal changes will not trigger listeners. </typeparam>
	public interface IReactive<out TValue> : IReactive
	{
		/// <summary> Subscribes on value change. </summary>
		/// <param name="listener"> Reaction on value change. </param>
		void Listen(Action<TValue> listener);

		/// <summary> Unsubscribes from value change. </summary>
		/// <param name="listener"> Reaction on value change that you used to subscribe. </param>
		void Forget(Action<TValue> listener);
	}

	/// <summary> Interface of listenable value that can be only read. </summary>
	public interface IReadOnlyReactive<out TValue> : IReactive<TValue>
	{
		/// <summary> Current value which changes you can subscribe on. </summary>
		TValue Value { get; }
	}

	/// <summary> Interface of listenable value that can be read and written. </summary>
	public interface IReadWriteReactive<TValue> : IReactive<TValue>
	{
		/// <inheritdoc cref="IReadOnlyReactive{TValue}.Value" />
		TValue Value { get; set; }
	}

	/// <summary> Interface of listenable key collection. </summary>
	public interface IReactiveKey<in TKey> : IReactive
	{
		/// <summary> Subscribes on change of specific key. </summary>
		/// <param name="key"> Key of change to react on. </param>
		/// <param name="listener"> Reaction on change of specific key. </param>
		void Listen(TKey key, Action listener);

		/// <summary> Unsubscribes from change of specific key. </summary>
		/// <param name="key"> Key of change to stop reacting on. </param>
		/// <param name="listener"> Reaction on change of specific key that you used to subscribe. </param>
		void Forget(TKey key, Action listener);
	}

	/// <summary> Interface of listenable key-value collection. </summary>
	public interface IReactiveKey<in TKey, out TValue> : IReactive
	{
		/// <summary> Subscribes on value change of specific key. </summary>
		/// <param name="key"> Key of change to react on. </param>
		/// <param name="listener"> Reaction on value change of specific key. </param>
		void Listen(TKey key, Action<TValue> listener);

		/// <summary> Unsubscribes from value change of specific key. </summary>
		/// <param name="key"> Key of change to stop reacting on. </param>
		/// <param name="listener"> Reaction on value change of specific key that you used to subscribe. </param>
		void Forget(TKey key, Action<TValue> listener);
	}

	/// <summary> Interface of listenable key-value collection. </summary>
	public interface IReactivePair<out TKey, out TValue> : IReactive
	{
		/// <summary> Subscribes on any value change of any key. </summary>
		/// <param name="listener"> Reaction on value change of some key. </param>
		void Listen(Action<TKey, TValue> listener);

		/// <summary> Unsubscribes from any value change of any key. </summary>
		/// <param name="listener"> Reaction on value change of some key that you used to subscribe. </param>
		void Forget(Action<TKey, TValue> listener);
	}

	public interface IReadOnlyReactiveList<out TItem>
		: IReactivePair<int, TItem>
		, IReadOnlyList<TItem>
	{
	}

	public interface IReadOnlyReactiveSet<TItem>
		: IReactivePair<TItem, bool>
		, IReactiveKey<TItem, bool>
		, IReadOnlyCollection<TItem>
	{
		bool Contains(TItem item);
	}

	/// <summary> Interface of listenable key-value collection that can be only read. </summary>
	public interface IReadOnlyReactiveMap<TKey, TValue>
		: IReactivePair<TKey, TValue>
		, IReactiveKey<TKey, TValue>
		, IReactiveKey<TKey>
		, IReadOnlyDictionary<TKey, TValue>
	{
	}
}