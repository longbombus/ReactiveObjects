using System;
using System.Collections.Generic;

namespace ReactiveObjects
{
	/// <summary> Implementation of listenable nullable value. </summary>
	/// <typeparam name="T"> Value-type of the changeable value. </typeparam>
	public class ReactiveNullable<T>
		: IReadOnlyReactive<T>
		, IReadWriteReactive<T>
		, IReadOnlyReactive<T?>
		, IReadWriteReactive<T?>
		where T : struct
	{
		private List<Action> changeListeners;
		private List<Action<T>> valueListeners;
		private List<Action<T?>> nullableValueListeners;
		private readonly IEqualityComparer<T> comparer;
		private T value;
		private bool hasValue;

		/// <returns> Default value if not set. </returns>
		public T Value => value;

		/*
		It would be wonderful to have such declaration, but it is not supported by C#:
		public T Value { get => value; }
		public T? Value { set => Set(value); }
		*/

		T IReadOnlyReactive<T>.Value => value;

		T IReadWriteReactive<T>.Value
		{
			get => value;
			set => Set(value);
		}

		T? IReadOnlyReactive<T?>.Value => hasValue ? value : null;

		T? IReadWriteReactive<T?>.Value
		{
			get => hasValue ? value : null;
			set => Set(value);
		}

		/// <summary> Whether the value is set. </summary>
		public bool HasValue => hasValue;

		public ReactiveNullable() : this(null, null) { }
		public ReactiveNullable(IEqualityComparer<T> comparer) : this(null, comparer) { }

		public ReactiveNullable(T? initialValue, IEqualityComparer<T> comparer = null)
		{
			this.value = initialValue ?? default;
			this.hasValue = initialValue.HasValue;
			this.comparer = comparer ?? EqualityComparer<T>.Default;
		}

		/// <summary> Changes value and notifies listeners if the value is different from the current one. </summary>
		/// <param name="newValue"> New value to set. </param>
		public void Set(T newValue)
		{
			if (hasValue && comparer.Equals(value, newValue))
				return;

			value = newValue;
			hasValue = true;
			Notify(value);
		}

		/// <summary> Nullifies value and notifies listeners if the value was not null. </summary>
		public void SetNull()
		{
			if (!hasValue)
				return;

			value = default;
			hasValue = false;
			NotifyNull();
		}

		/// <inheritdoc cref="ReactiveNullable{T}.Set(T)"/>
		public void Set(T? newValue)
		{
			if (newValue.HasValue)
				Set(newValue.Value);
			else
				SetNull();
		}

		/// <summary> Changes value without notifying listeners. </summary>
		/// <param name="newValue"> New value to set. </param>
		public void SetSilent(T newValue)
			=> value = newValue;

		/// <summary> Changes value without notifying listeners. </summary>
		/// <param name="newValue"> New value to set. </param>
		public void SetSilent(T? newValue)
		{
			value = newValue ?? default;
			hasValue = newValue.HasValue;
		}

		public void Listen(Action listener)
			=> ListenersUtility.AddListener(ref changeListeners, listener);

		public void Forget(Action listener)
			=> ListenersUtility.RemoveListener(changeListeners, listener);

		/// <inheritdoc cref="IReactive{TValue}.Listen(Action{TValue})"/>
		/// <remarks> Skips null value </remarks>
		public void Listen(Action<T> listener)
			=> ListenersUtility.AddListener(ref valueListeners, listener);

		public void Forget(Action<T> listener)
			=> ListenersUtility.RemoveListener(valueListeners, listener);

		public void Listen(Action<T?> listener)
			=> ListenersUtility.AddListener(ref nullableValueListeners, listener);

		public void Forget(Action<T?> listener)
			=> ListenersUtility.RemoveListener(nullableValueListeners, listener);

		private void Notify(T newValue)
		{
			ListenersUtility.InvokeAllSafe(changeListeners);
			ListenersUtility.InvokeAllSafe(valueListeners, newValue);
			ListenersUtility.InvokeAllSafe(nullableValueListeners, newValue);
		}

		private void NotifyNull()
		{
			ListenersUtility.InvokeAllSafe(changeListeners);
			ListenersUtility.InvokeAllSafe(nullableValueListeners, null);
		}

		public static implicit operator T?(ReactiveNullable<T> reactive)
			=> reactive.HasValue ? reactive.Value : null;

		public override string ToString()
			=> HasValue ? Value.ToString() : "null";
	}
}