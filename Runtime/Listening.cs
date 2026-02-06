using System;

#nullable enable

namespace ReactiveObjects
{
	public readonly struct Listening : IDisposable, IEquatable<Listening>
	{
		private readonly Action forget;

		private Listening(Action forget)
			=> this.forget = forget;

		public readonly void Dispose()
			=> forget();

		public bool Equals(Listening other)
			=> Equals(this.forget, other.forget);

		public override bool Equals(object? obj)
			=> obj is Listening other && Equals(other);

		public override int GetHashCode()
			=> forget != null ? forget.GetHashCode() : 0;

		public static Listening Create(IReactive reactive, Action listener)
		{
			reactive.Listen(listener);
			return new Listening(() => reactive.Forget(listener));
		}

		public static Listening Create<TValue>(IReactive<TValue> reactive, Action<TValue> listener)
		{
			reactive.Listen(listener);
			return new Listening(() => reactive.Forget(listener));
		}

		public static Listening Create<TKey, TValue>(IReactive<TKey, TValue> reactive, Action<TKey, TValue> listener)
		{
			reactive.Listen(listener);
			return new Listening(() => reactive.Forget(listener));
		}

		public static Listening Create<TKey, TValue>(IReactive<TKey, TValue> reactive, TKey key, Action listener)
		{
			reactive.Listen(key, listener);
			return new Listening(() => reactive.Forget(key, listener));
		}

		public static Listening Create<TKey, TValue>(IReactive<TKey, TValue> reactive, TKey key, Action<TValue> listener)
		{
			reactive.Listen(key, listener);
			return new Listening(() => reactive.Forget(key, listener));
		}
	}
}