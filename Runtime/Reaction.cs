using System;

#nullable enable

namespace ReactiveObjects
{
	/// <summary> Struct that preserves listening ownership. Disposing it will stop listening. </summary>
	public readonly struct Reaction : IDisposable, IEquatable<Reaction>
	{
		private readonly Action forget;

		private Reaction(Action forget)
			=> this.forget = forget;

		public void Forget()
			=> forget();

		void IDisposable.Dispose()
			=> forget();

		public bool Equals(Reaction other)
			=> Equals(this.forget, other.forget);

		public override bool Equals(object? obj)
			=> obj is Reaction other && Equals(other);

		public override int GetHashCode()
			=> forget != null ? forget.GetHashCode() : 0;

		public static Reaction Create(IReactive reactive, Action listener)
		{
			reactive.Listen(listener);
			return new Reaction(() => reactive.Forget(listener));
		}

		public static Reaction Create<TValue>(IReactive<TValue> reactive, Action<TValue> listener)
		{
			reactive.Listen(listener);
			return new Reaction(() => reactive.Forget(listener));
		}

		public static Reaction Create<TKey, TValue>(IReactivePair<TKey, TValue> reactive, Action<TKey, TValue> listener)
		{
			reactive.Listen(listener);
			return new Reaction(() => reactive.Forget(listener));
		}

		public static Reaction Create<TKey>(IReactiveKey<TKey> reactive, TKey key, Action listener)
		{
			reactive.Listen(key, listener);
			return new Reaction(() => reactive.Forget(key, listener));
		}

		public static Reaction Create<TKey, TValue>(IReactiveKey<TKey, TValue> reactive, TKey key, Action<TValue> listener)
		{
			reactive.Listen(key, listener);
			return new Reaction(() => reactive.Forget(key, listener));
		}
	}
}