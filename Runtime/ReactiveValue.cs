using System;
using System.Collections.Generic;

namespace ReactiveObjects
{
	/// <summary> Implementation of listenable value. You can use it as a field or property type to make it listenable. </summary>
	/// <typeparam name="T"> Type of the changeable value. Notice that reference type internal changes will not trigger listeners. </typeparam>
	public class Reactive<T>
		: IReadOnlyReactive<T>
		, IReadWriteReactive<T>
	{
		private List<Action> changeListeners;
		private List<Action<T>> valueListeners;
		private readonly IEqualityComparer<T> comparer;
		private T value;

		public T Value
		{
			get => value;
			set => Set(value);
		}

		public Reactive() : this(default, null) { }
		public Reactive(IEqualityComparer<T> comparer) : this(default, comparer) { }

		public Reactive(T initialValue, IEqualityComparer<T> comparer = null)
		{
			this.value = initialValue;
			this.comparer = comparer ?? EqualityComparer<T>.Default;
		}

		/// <summary> Changes value and notifies listeners if the value is different from the current one. </summary>
		/// <param name="newValue"> New value to set. </param>
		public void Set(T newValue)
		{
			if (comparer.Equals(value, newValue))
				return;

			value = newValue;
			Notify(newValue);
		}

		/// <summary> Changes value without notifying listeners. </summary>
		/// <param name="newValue"> New value to set. </param>
		public void SetSilent(T newValue)
			=> value = newValue;

		public void Listen(Action listener)
			=> ListenersUtility.AddListener(ref changeListeners, listener);

		public void Forget(Action listener)
			=> ListenersUtility.RemoveListener(changeListeners, listener);

		public void Listen(Action<T> listener)
			=> ListenersUtility.AddListener(ref valueListeners, listener);

		public void Forget(Action<T> listener)
			=> ListenersUtility.RemoveListener(valueListeners, listener);

		private void Notify(T newValue)
		{
			ListenersUtility.InvokeAllSafe(changeListeners);
			ListenersUtility.InvokeAllSafe(valueListeners, newValue);
		}

		public static implicit operator T(Reactive<T> reactive)
			=> reactive.Value;

		public override string ToString()
			=> Value?.ToString() ?? "null";
	}
}