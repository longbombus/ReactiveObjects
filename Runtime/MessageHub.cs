using System;
using System.Collections.Generic;

namespace ReactiveObjects
{
	/// <summary>
	/// Keyed messenger for a hierarchy of message types. Each concrete message type gets its own
	/// <see cref="Messenger{TKey,TMessage}"/> bucket, so listeners of one message type never receive
	/// messages of another. Implements <see cref="IServiceProvider"/> so containers can resolve
	/// typed <see cref="IMessageReceiver{TKey,TMessage}"/> / <see cref="IMessageSender{TKey,TMessage}"/>
	/// views from a single registered hub.
	/// </summary>
	public class MessageHub<TKey, TMessage>
		: IMessageSender<TKey, TMessage>
		, IServiceProvider
	{
		private readonly Dictionary<Type, IBucket> buckets = new();

		/// <summary> Returns the messenger dedicated to the specific message type. </summary>
		public Messenger<TKey, T> Of<T>()
			where T : TMessage
		{
			if (!buckets.TryGetValue(typeof(T), out var bucket))
				buckets.Add(typeof(T), bucket = new Bucket<T>());

			return (Bucket<T>)bucket;
		}

		/// <summary> Sends the message to listeners of its concrete type subscribed on the specified key. </summary>
		public void Send<T>(TKey key, T message)
			where T : TMessage
		{
			if (default(T) == null && message != null && message.GetType() != typeof(T))
				SendDynamic(key, message);
			else
				Of<T>().Send(key, message);
		}

		void IMessageSender<TKey, TMessage>.Send(TKey key, TMessage message)
			=> SendDynamic(key, message);

		/// <summary>
		/// Resolves <see cref="IMessageReceiver{TKey,TMessage}"/> or <see cref="IMessageSender{TKey,TMessage}"/>
		/// closed over this hub's key type and a message type derived from <typeparamref name="TMessage"/>.
		/// </summary>
		public object GetService(Type serviceType)
		{
			if (!serviceType.IsGenericType)
				return null;

			var definition = serviceType.GetGenericTypeDefinition();
			if (definition != typeof(IMessageReceiver<,>) && definition != typeof(IMessageSender<,>))
				return null;

			var arguments = serviceType.GetGenericArguments();
			if (arguments[0] != typeof(TKey) || !typeof(TMessage).IsAssignableFrom(arguments[1]))
				return null;

			return GetBucket(arguments[1]);
		}

		private void SendDynamic(TKey key, TMessage message)
		{
			// No bucket means nobody listens to that message type.
			if (message != null && buckets.TryGetValue(message.GetType(), out var bucket))
				bucket.Send(key, message);
		}

		private IBucket GetBucket(Type messageType)
		{
			if (!buckets.TryGetValue(messageType, out var bucket))
			{
				var bucketType = typeof(MessageHub<,>.Bucket<>).MakeGenericType(typeof(TKey), typeof(TMessage), messageType);
				buckets.Add(messageType, bucket = (IBucket)Activator.CreateInstance(bucketType));
			}

			return bucket;
		}

		private interface IBucket
		{
			void Send(TKey key, TMessage message);
		}

		private class Bucket<T> : Messenger<TKey, T>, IBucket
			where T : TMessage
		{
			void IBucket.Send(TKey key, TMessage message)
				=> Send(key, (T)message);
		}
	}
}
