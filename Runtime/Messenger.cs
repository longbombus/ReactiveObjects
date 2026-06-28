using System;
using System.Collections.Generic;

namespace ReactiveObjects
{
	public class Messenger<TKey, TMessage>
		: IMessageReceiver<TKey, TMessage>
		, IMessageSender<TKey, TMessage>
	{
		private List<Action> changeListeners;
		private List<Action<TKey, TMessage>> keyValueListeners;
		private Dictionary<TKey, List<Action>> keyedChangeListeners;
		private Dictionary<TKey, List<Action<TMessage>>> keyedValueListeners;

		public void Send(TKey key, TMessage message)
		{
			ListenersUtility.InvokeAllSafe(changeListeners);
			ListenersUtility.InvokeAllSafe(keyValueListeners, key, message);
			ListenersUtility.InvokeAllSafe(keyedChangeListeners, key);
			ListenersUtility.InvokeAllSafe(keyedValueListeners, key, message);
		}

		public void Listen(Action listener)
			=> ListenersUtility.AddListener(ref changeListeners, listener);

		public void Forget(Action listener)
			=> ListenersUtility.RemoveListener(changeListeners, listener);

		public void Listen(Action<TKey, TMessage> listener)
			=> ListenersUtility.AddListener(ref keyValueListeners, listener);

		public void Forget(Action<TKey, TMessage> listener)
			=> ListenersUtility.RemoveListener(keyValueListeners, listener);

		public void Listen(TKey key, Action listener)
			=> ListenersUtility.AddListener(ref keyedChangeListeners, key, listener);

		public void Forget(TKey key, Action listener)
			=> ListenersUtility.RemoveListener(keyedChangeListeners, key, listener);

		public void Listen(TKey key, Action<TMessage> listener)
			=> ListenersUtility.AddListener(ref keyedValueListeners, key, listener);

		public void Forget(TKey key, Action<TMessage> listener)
			=> ListenersUtility.RemoveListener(keyedValueListeners, key, listener);
	}
}
