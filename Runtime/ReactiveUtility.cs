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
	}
}