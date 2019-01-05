using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NUnit.Framework;
using SlugEnt;


namespace SlugEnt.UnitTest
{
	[Parallelizable]
	public class TestUniqueKeys
	{
		// Validates that the default prefix for keys is Key
		[Test]
		public void DefaultKeyPrefix_IsCorrect() {
			UniqueKeys uniqueKey = new UniqueKeys();
			string key = uniqueKey.GetKey();
			string prefix = key.Substring(0, 3);
			Assert.AreEqual(3, prefix.Length, "A1: String prefix was expected to be 3 characters long.");
			Assert.AreEqual("Key", prefix, "A2: Default prefix is expected to be Key");
		}



		// Validates the key is generated as expected.
		[Test]
		public void KeyValueIsAsExpected_Success() {
			string defaultPrefix = "Key";

			// We need to simulate the Timeguid logic to derive at the initial key value.
			DateTime d = DateTime.Now;
			string stGuid = TimeGuid.ConvertTimeToChar(d);


			UniqueKeys uniqueKey = new UniqueKeys();
			string key = uniqueKey.GetKey();
			string expKey = defaultPrefix + stGuid + "1";
			Assert.AreEqual(expKey, key, "A1:  Expected initial key value was not correct. Note:  On Failures run a second time.  There is a very slight chance that due to a second rollover the values could be different.  Expected: " + expKey + "   Actual: " + key);
			TestContext.WriteLine("Unique Key Value:    {0}", key);
			TestContext.WriteLine("Expected Key Value:  {0}", expKey);
		}



		// Validates that the first key has a value of 1 and each subsequent call is increasing by 1.
		public void KeyNumberShouldIncrement_EachCall_Success() {
			string defaultPrefix = "Key";

			// We need to simulate the Timeguid logic to derive at the initial key value.
			DateTime d = DateTime.Now;
			string stGuid = TimeGuid.ConvertTimeToChar(d);


			UniqueKeys uniqueKey = new UniqueKeys();
			string key = uniqueKey.GetKey();
			string expKey = defaultPrefix + stGuid + "1";
			Assert.AreEqual(expKey, key, "A1:  Expected initial key value was not correct. Note:  On Failures run a second time.  There is a very slight chance that due to a second rollover the values could be different.  Expected: " + expKey + "   Actual: " + key);


			// Now generate another key:
			string key2 = uniqueKey.GetKey();
			string expKey2 = defaultPrefix + stGuid + "2";
			Assert.AreEqual(expKey2, key2, "A2:  Expected initial key value was not correct. Note:  On Failures run a second time.  There is a very slight chance that due to a second rollover the values could be different.  Expected: " + expKey + "   Actual: " + key);
		}


		// Validate that refresh key generates a new time value for the unique key.
		[Test]
		public void RefreshKeyWorks() {
			string defaultPrefix = "Key";

			// We need to simulate the Timeguid logic to derive at the initial key value.
			DateTime d = DateTime.Now;
			string stGuid = TimeGuid.ConvertTimeToChar(d);


			UniqueKeys uniqueKey = new UniqueKeys();
			string key = uniqueKey.GetKey();
			string expKey = defaultPrefix + stGuid + "1";
			Assert.AreEqual(expKey, key, "A1:  Expected initial key value was not correct. Note:  On Failures run a second time.  There is a very slight chance that due to a second rollover the values could be different.  Expected: " + expKey + "   Actual: " + key);


			// Now get a new key.  Note the TimeGuid portion should be updated and the increment should be updated.
			Thread.Sleep(2000);

			string key2 = uniqueKey.RefreshKey();
			DateTime d2 = DateTime.Now;
			string stGuid2 = TimeGuid.ConvertTimeToChar(d2);
			string expKey2 = defaultPrefix + stGuid2 + "2";
			Assert.AreEqual(expKey2, key2, "A2: Expected key after Refresh was not equal to the value returned by UniqueKeys.  Exp: " + expKey2 + "   Actual:  " + key2);
		}



		// Validate GetKey returns a key with the custom prefix.
		[Test]
		public void GetKeyWithUniquePrefix_Success() {
			string defaultPrefix = "unique";

			// We need to simulate the Timeguid logic to derive at the initial key value.
			DateTime d = DateTime.Now;
			string stGuid = TimeGuid.ConvertTimeToChar(d);


			UniqueKeys uniqueKey = new UniqueKeys();
			string key = uniqueKey.GetKey(defaultPrefix);
			string expKey = defaultPrefix + stGuid + "1";
			Assert.AreEqual(expKey, key, "A1:  Expected initial key value was not correct. Note:  On Failures run a second time.  There is a very slight chance that due to a second rollover the values could be different.  Expected: " + expKey + "   Actual: " + key);

			TestContext.WriteLine("Unique Key Value:    {0}", key);
			TestContext.WriteLine("Expected Key Value:  {0}", expKey);
		}


		// Validate RefreshKey returns a key with the custom prefix.
		[Test]
		public void RefreshKeyWithUniquePrefix_Success() {
			string defaultPrefix = "unique";

			// We need to simulate the Timeguid logic to derive at the initial key value.
			DateTime d = DateTime.Now;
			string stGuid = TimeGuid.ConvertTimeToChar(d);


			UniqueKeys uniqueKey = new UniqueKeys();
			string key = uniqueKey.GetKey(defaultPrefix);
			string expKey = defaultPrefix + stGuid + "1";
			Assert.AreEqual(expKey, key, "A1:  Expected initial key value was not correct. Note:  On Failures run a second time.  There is a very slight chance that due to a second rollover the values could be different.  Expected: " + expKey + "   Actual: " + key);


			// Now get a new key.  Note the TimeGuid portion should be updated and the increment should be updated.
			Thread.Sleep(2000);

			string key2 = uniqueKey.RefreshKey(defaultPrefix);
			DateTime d2 = DateTime.Now;
			string stGuid2 = TimeGuid.ConvertTimeToChar(d2);
			string expKey2 = defaultPrefix + stGuid2 + "2";
			Assert.AreEqual(expKey2, key2, "A2: Expected key after Refresh was not equal to the value returned by UniqueKeys.  Exp: " + expKey2 + "   Actual:  " + key2);


			TestContext.WriteLine("Unique Key Value:    {0}", key2);
			TestContext.WriteLine("Expected Key Value:  {0}", expKey2);
		}
	}
}
