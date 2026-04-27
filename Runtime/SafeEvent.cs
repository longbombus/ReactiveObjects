using System;
using System.Collections.Generic;

namespace ReactiveObjects
{
	public class SafeEvent : IReactive
	{
		private List<Action> listeners;

		public void Listen(Action listener)
			=> ListenersUtility.AddListener(ref listeners, listener);

		public void Forget(Action listener)
			=> ListenersUtility.RemoveListener(listeners, listener);

		public void Invoke()
			=> ListenersUtility.InvokeAllSafe(listeners);

		public static SafeEvent operator+(SafeEvent safeEvent, Action listener)
		{
			safeEvent.Listen(listener);
			return safeEvent;
		}

		public static SafeEvent operator-(SafeEvent safeEvent, Action listener)
		{
			safeEvent.Forget(listener);
			return safeEvent;
		}
	}

	public class SafeEvent<T> : IReactive<T>
	{
		private List<Action> changeListeners;
		private List<Action<T>> valueListeners;

		public void Listen(Action listener)
			=> ListenersUtility.AddListener(ref changeListeners, listener);

		public void Forget(Action listener)
			=> ListenersUtility.RemoveListener(changeListeners, listener);

		public void Listen(Action<T> listener)
			=> ListenersUtility.AddListener(ref valueListeners, listener);

		public void Forget(Action<T> listener)
			=> ListenersUtility.RemoveListener(valueListeners, listener);

		public void Invoke(T arg)
		{
			ListenersUtility.InvokeAllSafe(changeListeners);
			ListenersUtility.InvokeAllSafe(valueListeners, arg);
		}

		public static SafeEvent<T> operator+(SafeEvent<T> safeEvent, Action listener)
		{
			safeEvent.Listen(listener);
			return safeEvent;
		}

		public static SafeEvent<T> operator-(SafeEvent<T> safeEvent, Action listener)
		{
			safeEvent.Forget(listener);
			return safeEvent;
		}

		public static SafeEvent<T> operator+(SafeEvent<T> safeEvent, Action<T> listener)
		{
			safeEvent.Listen(listener);
			return safeEvent;
		}

		public static SafeEvent<T> operator-(SafeEvent<T> safeEvent, Action<T> listener)
		{
			safeEvent.Forget(listener);
			return safeEvent;
		}
	}
}