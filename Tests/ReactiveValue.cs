using System;
using NUnit.Framework;

namespace ReactiveObjects.Tests
{
	public class ReactiveTests
	{
		[Test]
		public void DefaultConstructor_ValueIsDefault()
		{
			var reactive = new Reactive<int>();
			Assert.AreEqual(default(int), reactive.Value);
		}

		[Test]
		public void Constructor_WithInitialValue_SetsValue()
		{
			var reactive = new Reactive<int>(42);
			Assert.AreEqual(42, reactive.Value);
		}

		[Test]
		public void ValueSetter_WhenDifferent_InvokesChangeListenersOnce()
		{
			var reactive = new Reactive<int>(0);
			var calls = 0;
			reactive.Listen(() => calls++);

			reactive.Value = 1;

			Assert.AreEqual(1, calls);
		}

		[Test]
		public void ValueSetter_WhenDifferent_InvokesValueListenersWithNewValue()
		{
			var reactive = new Reactive<int>(0);
			int received = -1;
			var calls = 0;
			reactive.Listen(v => { received = v; calls++; });

			reactive.Value = 5;

			Assert.AreEqual(1, calls);
			Assert.AreEqual(5, received);
		}

		[Test]
		public void Set_WhenSame_DoesNotInvokeListeners()
		{
			var reactive = new Reactive<int>(10);
			var changeCalls = 0;
			var valueCalls = 0;
			reactive.Listen(() => changeCalls++);
			reactive.Listen(_ => valueCalls++);

			reactive.Set(10);
			reactive.Value = 10;

			Assert.AreEqual(0, changeCalls);
			Assert.AreEqual(0, valueCalls);
		}

		[Test]
		public void SetSilent_DoesNotInvokeListeners_ButChangesValue()
		{
			var reactive = new Reactive<int>(1);
			var changeCalls = 0;
			var valueCalls = 0;
			reactive.Listen(() => changeCalls++);
			reactive.Listen(_ => valueCalls++);

			reactive.SetSilent(2);

			Assert.AreEqual(2, reactive.Value);
			Assert.AreEqual(0, changeCalls);
			Assert.AreEqual(0, valueCalls);
		}

		[Test]
		public void Forget_ChangeListener_StopsReceivingNotifications()
		{
			var reactive = new Reactive<int>(0);
			var calls = 0;
			Action listener = () => calls++;

			reactive.Listen(listener);
			reactive.Value = 1;
			Assert.AreEqual(1, calls);

			reactive.Forget(listener);
			reactive.Value = 2;
			Assert.AreEqual(1, calls);
		}

		[Test]
		public void Forget_ValueListener_StopsReceivingNotifications()
		{
			var reactive = new Reactive<int>(0);
			var calls = 0;
			Action<int> listener = _ => calls++;

			reactive.Listen(listener);
			reactive.Value = 1;
			Assert.AreEqual(1, calls);

			reactive.Forget(listener);
			reactive.Value = 2;
			Assert.AreEqual(1, calls);
		}

		[Test]
		public void Set_WhenDifferent_InvokesBothChangeAndValueListeners()
		{
			var reactive = new Reactive<int>(0);
			var changeCalls = 0;
			var valueCalls = 0;
			reactive.Listen(() => changeCalls++);
			reactive.Listen(_ => valueCalls++);

			reactive.Set(123);

			Assert.AreEqual(1, changeCalls);
			Assert.AreEqual(1, valueCalls);
		}
	}
}