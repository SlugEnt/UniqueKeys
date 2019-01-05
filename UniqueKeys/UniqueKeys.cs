/*
 * Copyright 2018 Scott Herrmann

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

	https://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/



using System;
using System.Collections.Generic;
using System.Text;

namespace SlugEnt
{
	/// <summary>
	/// Class will generate a short unique key value that is guaranteed to be unique across a given instance of the class.
	/// Keys can be prefixed with a string upon each subsequent call.
	/// </summary>
	public class UniqueKeys
	{
		private int keyIncrementer = 0;
		private object keyIncrLock = new object();
		private string stGuid;

		public UniqueKeys() {
			DateTime d = DateTime.Now;
			stGuid = TimeGuid.ConvertTimeToChar(d);
		}


		/// <summary>
		/// Creates a small random key based upon the current time (H:M:S).
		/// </summary>
		/// <param name="prefix"></param>
		/// <returns></returns>
		public string GetKey(string prefix = "Key") {
			string val = prefix + stGuid + IncrementKey();
			return val;
		}


		public string RefreshKey(string prefix = "Key") {
			DateTime d = DateTime.Now;
			stGuid = TimeGuid.ConvertTimeToChar(d);
			return GetKey(prefix);
		}


		private string IncrementKey() {
			string val;
			lock (keyIncrLock) {
				keyIncrementer++;
				val = keyIncrementer.ToString();
			}
			return val;
		}
	}
}
