namespace ReactiveObjects.Tests
{
	using System;
	using NUnit.Framework;

	public class ReactiveMapTests
	{
		[Test]
		public void IndexerGet_WhenKeyMissing_ReturnsDefault()
		{
			var map = new ReactiveMap<int, string>();
			Assert.AreEqual(default(string), map[123]);
		}

		[Test]
		public void Set_NewKey_InvokesChangeAndKeyValueListeners()
		{
			var map = new ReactiveMap<int, int>();
			var changeCalls = 0;
			var keyValueCalls = 0;
			(int key, int value) received = default;

			map.Listen(() => changeCalls++);
			map.Listen((k, v) => { keyValueCalls++; received = (k, v); });

			map.Set(7, 99);

			Assert.AreEqual(1, changeCalls);
			Assert.AreEqual(1, keyValueCalls);
			Assert.AreEqual((7, 99), received);
		}

		[Test]
		public void Set_SameValueForExistingKey_DoesNotInvokeListeners()
		{
			var map = new ReactiveMap<int, int>();
			map.Set(1, 10);

			var changeCalls = 0;
			var keyValueCalls = 0;
			map.Listen(() => changeCalls++);
			map.Listen((_, _) => keyValueCalls++);

			map.Set(1, 10);
			map[1] = 10;

			Assert.AreEqual(0, changeCalls);
			Assert.AreEqual(0, keyValueCalls);
		}

		[Test]
		public void SetSilent_ChangesValueWithoutNotifications()
		{
			var map = new ReactiveMap<int, int>();
			var changeCalls = 0;
			var keyValueCalls = 0;
			map.Listen(() => changeCalls++);
			map.Listen((_, _) => keyValueCalls++);

			map.SetSilent(5, 123);

			Assert.AreEqual(123, map[5]);
			Assert.AreEqual(0, changeCalls);
			Assert.AreEqual(0, keyValueCalls);
		}

		[Test]
		public void IndexerSet_DelegatesToSetAndNotifies()
		{
			var map = new ReactiveMap<int, int>();
			var calls = 0;
			map.Listen(() => calls++);

			map[2] = 3;

			Assert.AreEqual(1, calls);
			Assert.AreEqual(3, map[2]);
		}

		[Test]
		public void Listen_KeyedListeners_FireOnlyForThatKey()
		{
			var map = new ReactiveMap<int, int>();
			var keyedChangeCalls = 0;
			var keyedValueCalls = 0;
			int received = -1;

			map.Listen(10, () => keyedChangeCalls++);
			map.Listen(10, v => { keyedValueCalls++; received = v; });

			map.Set(9, 1);
			map.Set(10, 2);

			Assert.AreEqual(1, keyedChangeCalls);
			Assert.AreEqual(1, keyedValueCalls);
			Assert.AreEqual(2, received);
		}

		[Test]
		public void Forget_KeyedListeners_StopReceivingNotifications()
		{
			var map = new ReactiveMap<int, int>();
			var changeCalls = 0;
			var valueCalls = 0;
			Action changeListener = () => changeCalls++;
			Action<int> valueListener = _ => valueCalls++;

			map.Listen(1, changeListener);
			map.Listen(1, valueListener);

			map.Set(1, 10);
			map.Forget(1, changeListener);
			map.Forget(1, valueListener);
			map.Set(1, 11);

			Assert.AreEqual(1, changeCalls);
			Assert.AreEqual(1, valueCalls);
		}

		[Test]
		public void Remove_ExistingKey_InvokesListenersWithDefaultValue()
		{
			var map = new ReactiveMap<int, string>();
			map.Set(1, "A");

			var keyValueCalls = 0;
			(int key, string value) received = default;
			map.Listen((k, v) => { keyValueCalls++; received = (k, v); });

			var removed = map.Remove(1);

			Assert.IsTrue(removed);
			Assert.AreEqual(1, keyValueCalls);
			Assert.AreEqual((1, default(string)), received);
			Assert.AreEqual(default(string), map[1]);
		}

		[Test]
		public void Remove_MissingKey_DoesNotNotifyAndReturnsFalse()
		{
			var map = new ReactiveMap<int, int>();
			var calls = 0;
			map.Listen(() => calls++);

			var removed = map.Remove(123);

			Assert.IsFalse(removed);
			Assert.AreEqual(0, calls);
		}

		[Test]
		public void Clear_NotifiesForEachExistingKey()
		{
			var map = new ReactiveMap<int, int>();
			map.Set(1, 1);
			map.Set(2, 2);
			map.Set(3, 3);

			var keyValueCalls = 0;
			map.Listen((_, _) => keyValueCalls++);

			map.Clear();

			Assert.AreEqual(3, keyValueCalls);
			Assert.AreEqual(0, map.Count);
		}

		[Test]
		public void Add_NotifiesListeners()
		{
			var map = new ReactiveMap<int, int>();
			var calls = 0;
			map.Listen(() => calls++);

			map.Add(1, 10);

			Assert.AreEqual(1, calls);
			Assert.AreEqual(10, map[1]);
		}
	}
}