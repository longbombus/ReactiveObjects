using System;

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
		public static void ListenNow<TKey, TValue>(this IReadOnlyReactive<TKey, TValue> reactive, Action listener)
		{
			reactive.Listen(listener);
			listener.InvokeSafe();
		}

		/// <summary> Subscribes on any value change of any key and immediately invokes listener with current key-value pairs. </summary>
		public static void ListenNow<TKey, TValue>(this IReadOnlyReactive<TKey, TValue> reactive, Action<TKey, TValue> listener)
		{
			reactive.Listen(listener);
			foreach (var (key, value) in reactive)
				listener.InvokeSafe(key, value);
		}

		/// <summary> Subscribes on value change of specific key and immediately invokes listener. </summary>
		public static void ListenNow<TKey, TValue>(this IReadOnlyReactive<TKey, TValue> reactive, TKey key, Action listener)
		{
			reactive.Listen(key, listener);
			listener.InvokeSafe();
		}

		/// <summary> Subscribes on value change of specific key and immediately invokes listener with current value of that key. </summary>
		public static void ListenNow<TKey, TValue>(this IReadOnlyReactive<TKey, TValue> reactive, TKey key, Action<TValue> listener)
		{
			reactive.Listen(key, listener);
			listener.InvokeSafe(reactive[key]);
		}

		#endregion

		#region ListenOnce

		/// <summary> Subscribes on only single next change. </summary>
		public static Listening ListenOnce(this IReactive reactive, Action listener)
		{
			return Listening.Create(reactive, Wrapper);
			void Wrapper()
			{
				reactive.Forget(Wrapper);
				listener.InvokeSafe();
			}
		}

		/// <summary> Subscribes on only single next value change. </summary>
		public static Listening ListenOnce<TValue>(this IReadOnlyReactive<TValue> reactive, Action<TValue> listener)
		{
			return Listening.Create(reactive, Wrapper);
			void Wrapper(TValue value)
			{
				reactive.Forget(Wrapper);
				listener.InvokeSafe(value);
			}
		}

		/// <summary> Subscribes on only single next change of any key. </summary>
		public static Listening ListenOnce<TKey, TValue>(IReactive<TKey, TValue> reactive, Action listener)
		{
			return Listening.Create(reactive, Wrapper);
			void Wrapper(TKey key, TValue value)
			{
				reactive.Forget(Wrapper);
				listener.InvokeSafe();
			}
		}

		/// <summary> Subscribes on only single next value change of any key. </summary>
		public static Listening ListenOnce<TKey, TValue>(IReactive<TKey, TValue> reactive, Action<TKey, TValue> listener)
		{
			return Listening.Create(reactive, Wrapper);
			void Wrapper(TKey key, TValue value)
			{
				reactive.Forget(Wrapper);
				listener.InvokeSafe(key, value);
			}
		}

		/// <summary> Subscribes on only single next change of specific key. </summary>
		public static Listening ListenOnce<TKey, TValue>(this IReadOnlyReactive<TKey, TValue> reactive, TKey key, Action listener)
		{
			return Listening.Create(reactive, key, Wrapper);
			void Wrapper()
			{
				reactive.Forget(key, Wrapper);
				listener.InvokeSafe();
			}
		}

		/// <summary> Subscribes on only single next value change of specific key. </summary>
		public static Listening ListenOnce<TKey, TValue>(this IReadOnlyReactive<TKey, TValue> reactive, TKey key, Action<TValue> listener)
		{
			return Listening.Create(reactive, key, Wrapper);
			void Wrapper(TValue value)
			{
				reactive.Forget(key, Wrapper);
				listener.InvokeSafe(value);
			}
		}

		#endregion
	}
}