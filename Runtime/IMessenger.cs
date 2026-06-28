namespace ReactiveObjects
{
	public interface IMessageReceiver<TKey, TMessage>
		: IReactiveKey<TKey, TMessage>
		, IReactivePair<TKey, TMessage>
	{
	}

	public interface IMessageSender<in Tkey, in TMessage>
	{
		/// <summary> Sends a message to all listeners subscribed on the specified key. </summary>
		/// <param name="key"> Key to send the message to. </param>
		/// <param name="message"> Message to deliver to the listeners. </param>
		void Send(Tkey key, TMessage message);
	}
}