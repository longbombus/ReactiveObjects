using System;
using System.Collections.Generic;

namespace ReactiveObjects
{
	public static class ReactiveUtility
	{
		#region ListenNow

		/// <summary> Subscribes on change and immediately invokes listener. </summary>
		public static void ListenNow(this IReactive reactive, Action listener)
		{
			reactive.Listen(listener);
			listener.InvokeSafe();
		}

		/// <summary> Subscribes on value change and immediately invokes listener with current value. </summary>
		public static void ListenNow<TValue>(this IReadOnlyReactive<TValue> reactive, Action<TValue> listener)
		{
			reactive.Listen(listener);
			listener.InvokeSafe(reactive.Value);
		}

		/// <summary> Subscribes on any value change of any key and immediately invokes listener. </summary>
		public static void ListenNow<TKey, TValue>(this IReadOnlyReactiveMap<TKey, TValue> reactive, Action listener)
		{
			reactive.Listen(listener);
			listener.InvokeSafe();
		}

		/// <summary> Subscribes on any value change of any key and immediately invokes listener with current key-value pairs. </summary>
		public static void ListenNow<TKey, TValue>(this IReadOnlyReactiveMap<TKey, TValue> reactive, Action<TKey, TValue> listener)
		{
			reactive.Listen(listener);
			foreach (var (key, value) in reactive)
				listener.InvokeSafe(key, value);
		}

		/// <summary> Subscribes on value change of specific key and immediately invokes listener. </summary>
		public static void ListenNow<TKey, TValue>(this IReadOnlyReactiveMap<TKey, TValue> reactive, TKey key, Action listener)
		{
			reactive.Listen(key, listener);
			listener.InvokeSafe();
		}

		/// <summary> Subscribes on value change of specific key and immediately invokes listener with current value of that key. </summary>
		public static void ListenNow<TKey, TValue>(this IReadOnlyReactiveMap<TKey, TValue> reactive, TKey key, Action<TValue> listener)
		{
			reactive.Listen(key, listener);
			listener.InvokeSafe(reactive[key]);
		}

		#endregion

		#region React

		/// <inheritdoc cref="IReactive.Listen(Action)"/>
		/// <returns> Disposable subscruption handle. </returns>
		public static Reaction React(this IReactive reactive, Action listener)
			=> Reaction.Create(reactive, listener);

		/// <inheritdoc cref="IReactive{TValue}.Listen(Action{TValue})"/>
		/// <inheritdoc cref="React(IReactive, Action)"/>
		public static Reaction React<TValue>(this IReadOnlyReactive<TValue> reactive, Action<TValue> listener)
			=> Reaction.Create(reactive, listener);

		/// <inheritdoc cref="IReactivePair{TKey, TValue}.Listen(Action{TKey, TValue})"/>
		/// <inheritdoc cref="React(IReactive, Action)"/>
		public static Reaction React<TCollection, TItem>(this TCollection reactive, Action<int, TItem> listener)
			where TCollection : IReadOnlyReactiveList<TItem>
			=> Reaction.Create(reactive, listener);

		/// <inheritdoc cref="IReactivePair{TKey, TValue}.Listen(Action{TKey, TValue})"/>
		/// <inheritdoc cref="React(IReactive, Action)"/>
		public static Reaction React<TCollection, TItem>(this TCollection reactive, Action<TItem, bool> listener)
			where TCollection : IReadOnlyReactiveSet<TItem>
			=> Reaction.Create(reactive, listener);

		/// <inheritdoc cref="IReactivePair{TKey, TValue}.Listen(Action{TKey, TValue})"/>
		/// <inheritdoc cref="React(IReactive, Action)"/>
		public static Reaction React<TCollection, TKey, TValue>(this TCollection reactive, Action<TKey, TValue> listener)
			where TCollection : IReadOnlyReactiveMap<TKey, TValue>
			=> Reaction.Create(reactive, listener);

		/// <inheritdoc cref="IReactiveKey{TKey}.Listen(TKey, Action)"/>
		/// <inheritdoc cref="React(IReactive, Action)"/>
		public static Reaction React<TCollection, TKey, TValue>(this TCollection reactive, TKey key, Action listener)
			 where TCollection : IReadOnlyReactiveMap<TKey, TValue>
			=> Reaction.Create(reactive, key, listener);

		/// <inheritdoc cref="IReactiveKey{TKey, TValue}.Listen(TKey, Action{TValue})"/>
		/// <inheritdoc cref="React(IReactive, Action)"/>
		public static Reaction React<TCollection, TKey, TValue>(this TCollection reactive, TKey key, Action<TValue> listener)
			where TCollection : IReadOnlyReactiveMap<TKey, TValue>
			=> Reaction.Create(reactive, key, listener);

		#endregion

		#region ReactNow

		/// <inheritdoc cref="ListenNow(IReactive, Action)"/>
		/// <inheritdoc cref="React(IReactive, Action)"/>
		public static Reaction ReactNow(this IReactive reactive, Action listener)
		{
			var result = Reaction.Create(reactive, listener);
			listener.InvokeSafe();
			return result;
		}

		/// <inheritdoc cref="ListenNow{TValue}(IReadOnlyReactive{TValue}, Action{TValue})"/>
		/// <inheritdoc cref="React(IReactive, Action)"/>
		public static Reaction ReactNow<TValue>(this IReadOnlyReactive<TValue> reactive, Action<TValue> listener)
		{
			var result = Reaction.Create(reactive, listener);
			listener.InvokeSafe(reactive.Value);
			return result;
		}

		/// <inheritdoc cref="ListenNow{TKey, TValue}(IReadOnlyReactiveMap{TKey,TValue}, Action)"/>
		/// <inheritdoc cref="React(IReactive, Action)"/>
		public static Reaction ReactNow<TCollection, TItem>(this TCollection reactive, Action<int, TItem> listener)
			where TCollection : IReadOnlyReactiveList<TItem>
		{
			var result = Reaction.Create(reactive, listener);
			for (int i = 0; i < reactive.Count; ++i)
				listener.InvokeSafe(i, reactive[i]);
			return result;
		}

		/// <inheritdoc cref="ListenNow{TKey, TValue}(IReadOnlyReactiveMap{TKey,TValue}, Action)"/>
		/// <inheritdoc cref="React(IReactive, Action)"/>
		public static Reaction ReactNow<TCollection, TItem>(this TCollection reactive, Action<TItem, bool> listener)
			where TCollection : IReadOnlyReactiveSet<TItem>
		{
			var result = Reaction.Create(reactive, listener);
			foreach (var item in reactive)
				listener.InvokeSafe(item, true);
			return result;
		}

		/// <inheritdoc cref="ListenNow{TKey, TValue}(IReadOnlyReactiveMap{TKey,TValue}, Action{TKey, TValue})"/>
		/// <inheritdoc cref="React(IReactive, Action)"/>
		public static Reaction ReactNow<TCollection, TKey, TValue>(this TCollection reactive, Action<TKey, TValue> listener)
			where TCollection : IReadOnlyReactiveMap<TKey, TValue>
		{
			var result = Reaction.Create(reactive, listener);
			foreach (var (key, value) in reactive)
				listener.InvokeSafe(key, value);
			return result;
		}

		/// <inheritdoc cref="ListenNow{TKey, TValue}(IReadOnlyReactiveMap{TKey,TValue}, TKey, Action)"/>
		/// <inheritdoc cref="React(IReactive, Action)"/>
		public static Reaction ReactNow<TCollection, TKey, TValue>(this TCollection reactive, TKey key, Action listener)
			 where TCollection : IReadOnlyReactiveMap<TKey, TValue>
		{
			var result = Reaction.Create(reactive, key, listener);
			listener.InvokeSafe();
			return result;
		}

		/// <inheritdoc cref="ListenNow{TKey, TValue}(IReadOnlyReactiveMap{TKey,TValue}, TKey, Action{TValue})"/>
		/// <inheritdoc cref="React(IReactive, Action)"/>
		public static Reaction ReactNow<TCollection, TKey, TValue>(this TCollection reactive, TKey key, Action<TValue> listener)
			where TCollection : IReadOnlyReactiveMap<TKey, TValue>
		{
			var result = Reaction.Create(reactive, key, listener);
			listener.InvokeSafe(reactive[key]);
			return result;
		}

		#endregion

		#region ReactOnce

		/// <summary> Subscribes on only single next change. </summary>
		/// <inheritdoc cref="React(IReactive, Action)"/>
		public static Reaction ReactOnce(this IReactive reactive, Action listener)
		{
			return Reaction.Create(reactive, Wrapper);
			void Wrapper()
			{
				reactive.Forget(Wrapper);
				listener.InvokeSafe();
			}
		}

		/// <summary> Subscribes on only single next value change. </summary>
		/// <inheritdoc cref="React(IReactive, Action)"/>
		public static Reaction ReactOnce<TValue>(this IReactive<TValue> reactive, Action<TValue> listener)
		{
			return Reaction.Create(reactive, Wrapper);
			void Wrapper(TValue value)
			{
				reactive.Forget(Wrapper);
				listener.InvokeSafe(value);
			}
		}

		/// <summary> Subscribes on only single next change of specific key. </summary>
		/// <inheritdoc cref="React(IReactive, Action)"/>
		public static Reaction ReactOnce<TKey>(this IReactiveKey<TKey> reactive, TKey key, Action listener)
		{
			return Reaction.Create(reactive, key, Wrapper);
			void Wrapper()
			{
				reactive.Forget(key, Wrapper);
				listener.InvokeSafe();
			}
		}

		/// <summary> Subscribes on only single next value change of specific key. </summary>
		/// <inheritdoc cref="React(IReactive, Action)"/>
		public static Reaction ReactOnce<TKey, TValue>(this IReactiveKey<TKey, TValue> reactive, TKey key, Action<TValue> listener)
		{
			return Reaction.Create(reactive, key, Wrapper);
			void Wrapper(TValue value)
			{
				reactive.Forget(key, Wrapper);
				listener.InvokeSafe(value);
			}
		}

		/// <summary> Subscribes on only single next value change of any key. </summary>
		/// <inheritdoc cref="React(IReactive, Action)"/>
		public static Reaction ReactOnce<TKey, TValue>(IReactivePair<TKey, TValue> reactive, Action<TKey, TValue> listener)
		{
			return Reaction.Create(reactive, Wrapper);
			void Wrapper(TKey key, TValue value)
			{
				reactive.Forget(Wrapper);
				listener.InvokeSafe(key, value);
			}
		}

		#endregion

		#region Reactions Collections

		/// <summary> Forget all listenings and clears collection. </summary>
		public static void ForgetAll(this ICollection<Reaction> reactions)
		{
			foreach (var listening in reactions)
				listening.Forget();

			reactions.Clear();
		}

		 /// <summary> Forget all listenings and clears dictionary. </summary>
		public static void ForgetAll<TKey>(this IDictionary<TKey, Reaction> reactions)
		{
			foreach (var listening in reactions.Values)
				listening.Forget();

			reactions.Clear();
		}

		#endregion
	}
}