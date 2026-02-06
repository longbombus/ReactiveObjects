using System;

namespace ReactiveObjects
{
	public static class ReactiveUtility
	{
		#region ListenNow

		public static void ListenNow(this IReactive reactive, Action listener)
		{
			reactive.Listen(listener);
			listener.InvokeSafe();
		}

		public static void ListenNow<TValue>(this IReadOnlyReactive<TValue> reactive, Action<TValue> listener)
		{
			reactive.Listen(listener);
			listener.InvokeSafe(reactive.Value);
		}

		public static void ListenNow<TKey, TValue>(this IReadOnlyReactive<TKey, TValue> reactive, Action<TKey, TValue> listener)
		{
			reactive.Listen(listener);
			foreach (var (key, value) in reactive)
				listener.InvokeSafe(key, value);
		}

		public static void ListenNow<TKey, TValue>(this IReadOnlyReactive<TKey, TValue> reactive, TKey key, Action listener)
		{
			reactive.Listen(key, listener);
			listener.InvokeSafe();
		}

		public static void ListenNow<TKey, TValue>(this IReadOnlyReactive<TKey, TValue> reactive, TKey key, Action<TValue> listener)
		{
			reactive.Listen(key, listener);
			listener.InvokeSafe(reactive[key]);
		}

		#endregion

		#region ListenOnce

		public static Listening ListenOnce(this IReactive reactive, Action listener)
		{
			return Listening.Create(reactive, Wrapper);
			void Wrapper()
			{
				reactive.Forget(Wrapper);
				listener.InvokeSafe();
			}
		}

		public static Listening ListenOnce<TValue>(this IReadOnlyReactive<TValue> reactive, Action<TValue> listener)
		{
			return Listening.Create(reactive, Wrapper);
			void Wrapper(TValue value)
			{
				reactive.Forget(Wrapper);
				listener.InvokeSafe(value);
			}
		}

		public static Listening ListenOnce<TKey, TValue>(IReactive<TKey, TValue> reactive, Action<TKey, TValue> listener)
		{
			return Listening.Create(reactive, Wrapper);
			void Wrapper(TKey key, TValue value)
			{
				reactive.Forget(Wrapper);
				listener.InvokeSafe(key, value);
			}
		}

		public static Listening ListenOnce<TKey, TValue>(this IReadOnlyReactive<TKey, TValue> reactive, TKey key, Action listener)
		{
			return Listening.Create(reactive, key, Wrapper);
			void Wrapper()
			{
				reactive.Forget(key, Wrapper);
				listener.InvokeSafe();
			}
		}

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