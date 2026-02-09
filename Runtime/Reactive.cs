using System;
using System.Collections.Generic;

namespace ReactiveObjects
{
	/// <summary>
	/// Base class for reactive objects that only have change listeners.
	/// You can use it as a base for your own reactive types that don't fit Reactive{T} or Reactive{TKey, TValue}.
	/// Don't forget to call Notify() method when the object changes to notify listeners.
	/// </summary>
	public abstract class Reactive : IReactive
	{
		private List<Action> changeListeners;

		public void Listen(Action listener)
			=> ListenersUtility.AddListener(ref changeListeners, listener);

		public void Forget(Action listener)
			=> ListenersUtility.RemoveListener(changeListeners, listener);

		protected void Notify()
			=> ListenersUtility.InvokeAllSafe(changeListeners);
	}
}