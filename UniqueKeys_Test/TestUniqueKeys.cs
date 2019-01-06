using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NUnit.Framework;
using SlugEnt;


namespace SlugEnt.UnitTest
{
	internal class TestBeaker {
		private string _keyName;
		private string _keyseparator;
		private string _tgseparator;


	}
	[Parallelizable]
	public class TestUniqueKeys
	{

		// Helper method - tries to keep all code within 1 physical second.
		internal void KeepWithinSingleSecond () {
			int millisec = DateTimeOffset.Now.Millisecond;
			int sleepInt = 0;

			if (millisec > 800) {
				sleepInt = 1000 - millisec;
				Thread.Sleep(sleepInt);
			}
		}




		// Validates that the default identifier for keys is Key
		[Test]
		public void DefaultKeyPrefix_IsCorrect() {
			UniqueKeys uniqueKey = new UniqueKeys();
			string key = uniqueKey.GetKey();
			string prefix = key.Substring(0, 3);
			Assert.AreEqual(3, prefix.Length, "A1: String prefix was expected to be 3 characters long.");
			Assert.AreEqual("Key", prefix, "A2: Default prefix is expected to be Key");
		}



		// Validate that default constructor creates a key in expected format:
		// Default is:  <KeyIdentifier>:<TimeGuid>_numerically increasing number
		[Test]
		public void DefaultConstructor_CreatesExpectedKey () {
			string key = "ABC";

			// For this test we want to make sure we do not cross a second boundary.  So any time above 700ms we will wait out until the next second hits.
			int millisec = DateTimeOffset.Now.Millisecond;
			int sleepInt = 0;

			if ( millisec > 800 ) {
				sleepInt = 1000 - millisec;
				Thread.Sleep(sleepInt);
			}

			// We will test twice to determine if the TimeGuid is being set correctly.
			UniqueKeys uniqueKeys = new UniqueKeys();
			string key1 = uniqueKeys.GetKey(key);

			// Now sleep so we can make sure the next call to uniqueKeys will generate a new timeGuid.
			millisec = DateTimeOffset.Now.Millisecond;
			sleepInt = 1000 - millisec + 10;
			Thread.Sleep(sleepInt);

			// Create another unique key.  The TimeGuid's should be different.
			string key2 = uniqueKeys.GetKey(key);

			Assert.AreNotEqual(key1,key2,"A10:  Expected the keys to be different.");
			Assert.True(key2.StartsWith(key + ":"),"A20:  Expected the generated key to start with ABC:");


			// Now get the TimeGuid parts of the keys
			string[] timeGuidPart1 = key1.Split(":");
			Assert.AreEqual(2, timeGuidPart1.Length, "A30:  Expected the generated key to contain 2 parts.");


			string[] timeGuidPart2 = key2.Split(":");
			Assert.AreEqual(2,timeGuidPart2.Length,"A30:  Expected the generated key to contain 2 parts.");

			Assert.AreNotEqual(timeGuidPart1[1],timeGuidPart2[1],"A40:  Expected the TimeGuid part of the keys to be different.");
			Assert.AreEqual(timeGuidPart1[0],timeGuidPart2[0],"A50:  Expected the Key part of the keys to be the same.");
		}





		// Validates the key is generated as expected.
		[Test]
		public void KeyValueIsAsExpected_Success() {
			string defaultPrefix = "Key";

			KeepWithinSingleSecond();


			// We need to simulate the Timeguid logic to derive at the initial key value.
			DateTime d = DateTime.Now;
			string stGuid = TimeGuid.ConvertTimeToChar(d);


			UniqueKeys uniqueKey = new UniqueKeys();
			string key = uniqueKey.GetKey();
			string expKey = defaultPrefix + uniqueKey.WhatIsKeySeparator + stGuid;
			Assert.AreEqual(expKey, key, "A1:  Expected initial key value was not correct. Note:  On Failures run a second time.  There is a very slight chance that due to a second rollover the values could be different.  Expected: " + expKey + "   Actual: " + key);
			TestContext.WriteLine("Unique Key Value:    {0}", key);
			TestContext.WriteLine("Expected Key Value:  {0}", expKey);
		}



		// Validates the key field separator can be set
		[Test]
		public void KeyValueSeparator_CanBeSet_Success() {
			string defaultPrefix = "ABC";
			string sep = "^";

			KeepWithinSingleSecond();

			// We need to simulate the Timeguid logic to derive at the initial key value.
			DateTime d = DateTime.Now;
			string stGuid = TimeGuid.ConvertTimeToChar(d);


			// Set separator to be carat.
			UniqueKeys uniqueKey = new UniqueKeys(sep);
			string key = uniqueKey.GetKey(defaultPrefix);
			string expKey = defaultPrefix + sep + stGuid;

			Assert.AreEqual(expKey, key, "A10:  Expected initial key value was not correct. Note:  On Failures run a second time.  There is a very slight chance that due to a second rollover the values could be different.  Expected: " + expKey + "   Actual: " + key);
			TestContext.WriteLine("Unique Key Value:    {0}", key);
			TestContext.WriteLine("Expected Key Value:  {0}", expKey);
		}



		// Validates that key incrementer increments for same TimeGuid value.  Since a TimeGuid only has the ability to go to a second, if 2 sequential
		// requests for a key happen within a physical second, we need to make sure the numerical incrementer is increased.
		[Test]
		public void KeyIncrementer_Increments_Success() {
			string defaultPrefix = "ABC";

			KeepWithinSingleSecond();

			// We need to simulate the Timeguid logic to derive at the initial key value.
			DateTime d = DateTime.Now;
			string stGuid = TimeGuid.ConvertTimeToChar(d);


			UniqueKeys uniqueKey = new UniqueKeys();
			string key = uniqueKey.GetKey(defaultPrefix);
			string expKey = defaultPrefix + uniqueKey.WhatIsKeySeparator + stGuid;

			Assert.AreEqual(expKey, key, "A10:  Expected initial key value was not correct. Note:  On Failures run a second time.  There is a very slight chance that due to a second rollover the values could be different.  Expected: " + expKey + "   Actual: " + key);


			// Now create 2 more keys.  They should have incrementers attched to them.
			string key2 = uniqueKey.GetKey(defaultPrefix);
			string key3 = uniqueKey.GetKey(defaultPrefix);

			Assert.AreNotEqual(key,key2,"A20:  Expected Key1 and Key2 to be different.");
			Assert.AreNotEqual(key2, key3, "A30:  Expected Key2 and Key3 to be different.");

			Assert.IsTrue(key2.EndsWith(uniqueKey.WhatIsIncrementSeparator + "1"),"A40:  Key2 should have ended with a 1.");
			Assert.IsTrue(key3.EndsWith(uniqueKey.WhatIsIncrementSeparator + "2"), "A50:  Key3 should have ended with a 2.");
		}



		[Test]
		public void StaticTimeGuid_StaysSameThruLifetimeOfUniqueKeyObject () {
			string defaultPrefix = "ABC";

			KeepWithinSingleSecond();

			// We need to simulate the Timeguid logic to derive at the initial key value.
			DateTime d = DateTime.Now;
			string stGuid = TimeGuid.ConvertTimeToChar(d);


			//We want a constant TimeGuid
			UniqueKeys uniqueKey = new UniqueKeys(constantTimeGuid: true);
			string key = uniqueKey.GetKey(defaultPrefix);
			string expKey = defaultPrefix + uniqueKey.WhatIsKeySeparator + stGuid + uniqueKey.WhatIsIncrementSeparator + "1";

			Assert.AreEqual(expKey, key, "A10:  Expected initial key value was not correct. Note:  On Failures run a second time.  There is a very slight chance that due to a second rollover the values could be different.  Expected: " + expKey + "   Actual: " + key);

			// Now lets create 3 more keys with a second between them.  TimeGuid should be same and just the incrementer should be increasing.
			string key2 = uniqueKey.GetKey(defaultPrefix);
			Thread.Sleep(1000);
			string key3 = uniqueKey.GetKey(defaultPrefix);
			Thread.Sleep(1500);
			string key4 = uniqueKey.GetKey(defaultPrefix);
			Thread.Sleep(2000);

			Assert.IsTrue(key2.EndsWith(uniqueKey.WhatIsIncrementSeparator + "2"), "A20:  Key2 should have ended with a 2.");
			Assert.IsTrue(key3.EndsWith(uniqueKey.WhatIsIncrementSeparator + "3"), "A21:  Key3 should have ended with a 3.");
			Assert.IsTrue(key4.EndsWith(uniqueKey.WhatIsIncrementSeparator + "4"), "A22:  Key4 should have ended with a 4.");

			Assert.AreEqual(defaultPrefix+uniqueKey.WhatIsKeySeparator + stGuid + uniqueKey.WhatIsIncrementSeparator + "2",key2,"A50:  key2 is not expected value.");
			Assert.AreEqual(defaultPrefix + uniqueKey.WhatIsKeySeparator + stGuid + uniqueKey.WhatIsIncrementSeparator + "3", key3, "A51:  key3 is not expected value.");
			Assert.AreEqual(defaultPrefix + uniqueKey.WhatIsKeySeparator + stGuid + uniqueKey.WhatIsIncrementSeparator + "4", key4, "A52:  key4 is not expected value.");
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

			KeepWithinSingleSecond();


			// We need to simulate the Timeguid logic to derive at the initial key value.
			DateTime d = DateTime.Now;
			string stGuid = TimeGuid.ConvertTimeToChar(d);


			UniqueKeys uniqueKey = new UniqueKeys(constantTimeGuid:true);
			string key = uniqueKey.GetKey(defaultPrefix);
			string expKey = defaultPrefix + uniqueKey.WhatIsKeySeparator + stGuid + uniqueKey.WhatIsIncrementSeparator + "1";
			Assert.AreEqual(expKey, key, "A10:  Expected initial key value was not correct.   Expected: " + expKey + "   Actual: " + key);

			string key2 = uniqueKey.GetKey(defaultPrefix);

			// Now refresh the key.
			string key3 = uniqueKey.RefreshKey(defaultPrefix);

			// Because key3 requested RefreshKey, we should get a new TimeGuid as it would have made the thread sleep for at least 1 second.
			string newGuid = TimeGuid.ConvertTimeToChar(DateTime.Now);


			Assert.IsTrue(key2.EndsWith(uniqueKey.WhatIsIncrementSeparator + "2"), "A20:  Key2 should have ended with a 2.");
			Assert.IsTrue(key3.EndsWith(newGuid), "A21:  Key3 should have ended with a 1 as it was called with the RefreshKey.");

			string expKey3 = defaultPrefix + uniqueKey.WhatIsKeySeparator + newGuid;

			Assert.AreEqual(expKey3, key3, "A30: Expected key after Refresh was not equal to the value returned by UniqueKeys.  Exp: " + expKey3 + "   Actual:  " + key3);
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
			string expKey = defaultPrefix + uniqueKey.WhatIsKeySeparator +  stGuid;
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
			string expKey = defaultPrefix + uniqueKey.WhatIsKeySeparator + stGuid;
			Assert.AreEqual(expKey, key, "A1:  Expected initial key value was not correct. Note:  On Failures run a second time.  There is a very slight chance that due to a second rollover the values could be different.  Expected: " + expKey + "   Actual: " + key);


			// Now get a new key.  Note the TimeGuid portion should be updated and the increment should be updated.
			Thread.Sleep(1000);
			string key2 = uniqueKey.RefreshKey(defaultPrefix);
			string stGuid2 = TimeGuid.ConvertTimeToChar(DateTime.Now);
			string expKey2 = defaultPrefix + uniqueKey.WhatIsKeySeparator + stGuid2;
			Assert.AreEqual(expKey2, key2, "A2: Expected key after Refresh was not equal to the value returned by UniqueKeys.  Exp: " + expKey2 + "   Actual:  " + key2);


			TestContext.WriteLine("Unique Key Value:    {0}", key2);
			TestContext.WriteLine("Expected Key Value:  {0}", expKey2);
		}
	}
}
