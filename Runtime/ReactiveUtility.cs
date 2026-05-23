using System;
using System.Collections.Generic;

namespace ReactiveObjects
{
	public static class ReactiveUtility
	{
		#region Now

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

		/// <inheritdoc cref="ListenNow(IReactive, Action)"/>
		public static Reaction ReactNow(this IReactive reactive, Action listener)
		{
			var result = Reaction.Create(reactive, listener);
			listener.InvokeSafe();
			return result;
		}

		/// <inheritdoc cref="ListenNow{TValue}(IReadOnlyReactive{TValue}, Action{TValue})"/>
		public static Reaction ReactNow<TValue>(this IReadOnlyReactive<TValue> reactive, Action<TValue> listener)
		{
			var result = Reaction.Create(reactive, listener);
			listener.InvokeSafe(reactive.Value);
			return result;
		}

		/// <inheritdoc cref="ListenNow{TKey, TValue}(IReadOnlyReactiveMap{TKey,TValue}, Action)"/>
		public static Reaction ReactNow<TCollection, TItem>(this TCollection reactive, Action<int, TItem> listener)
			where TCollection : IReadOnlyReactiveList<TItem>
		{
			var result = Reaction.Create(reactive, listener);
			for (int i = 0; i < reactive.Count; ++i)
				listener.InvokeSafe(i, reactive[i]);
			return result;
		}

		/// <inheritdoc cref="ListenNow{TKey, TValue}(IReadOnlyReactiveMap{TKey,TValue}, Action)"/>
		public static Reaction ReactNow<TCollection, TItem>(this TCollection reactive, Action<TItem, bool> listener)
			where TCollection : IReadOnlyReactiveSet<TItem>
		{
			var result = Reaction.Create(reactive, listener);
			foreach (var item in reactive)
				listener.InvokeSafe(item, true);
			return result;
		}

		/// <inheritdoc cref="ListenNow{TKey, TValue}(IReadOnlyReactiveMap{TKey,TValue}, Action{TKey, TValue})"/>
		public static Reaction ReactNow<TCollection, TKey, TValue>(this TCollection reactive, Action<TKey, TValue> listener)
			where TCollection : IReadOnlyReactiveMap<TKey, TValue>
		{
			var result = Reaction.Create(reactive, listener);
			foreach (var (key, value) in reactive)
				listener.InvokeSafe(key, value);
			return result;
		}

		/// <inheritdoc cref="ListenNow{TKey, TValue}(IReadOnlyReactiveMap{TKey,TValue}, TKey, Action)"/>
		public static Reaction ReactNow<TCollection, TKey, TValue>(this TCollection reactive, TKey key, Action listener)
			 where TCollection : IReadOnlyReactiveMap<TKey, TValue>
		{
			var result = Reaction.Create(reactive, key, listener);
			listener.InvokeSafe();
			return result;
		}

		/// <inheritdoc cref="ListenNow{TKey, TValue}(IReadOnlyReactiveMap{TKey,TValue}, TKey, Action{TValue})"/>
		public static Reaction ReactNow<TCollection, TKey, TValue>(this TCollection reactive, TKey key, Action<TValue> listener)
			where TCollection : IReadOnlyReactiveMap<TKey, TValue>
		{
			var result = Reaction.Create(reactive, key, listener);
			listener.InvokeSafe(reactive[key]);
			return result;
		}

		#endregion

		#region Once

		/// <summary> Subscribes on only single next change. </summary>
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