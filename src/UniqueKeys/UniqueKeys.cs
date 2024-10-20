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
using System.Threading;

namespace SlugEnt;

/// <summary>
///     Class will generate a short unique key value that is guaranteed to be unique across a given instance of the class.
///     Keys can be prefixed or suffixed with a string upon each subsequent call.  The class uses the TimeGuid object to
///     help build
///     a short but unique key.
/// </summary>
public class UniqueKeys
{
    private readonly bool _constantTimeGuid;
    private readonly bool _keyIdentifierFirst;

    private int _keyIncrementer;

    private          TimeGuid _timeGuid;
    private readonly object   keyIncrLock = new();



    /// <summary>
    ///     Creates an object that will generate UniqueKeys.  Keys are guaranteed to be unique across this single instance of
    ///     the object.  If you trully need
    ///     unique keys then use a full Guid.
    /// </summary>
    /// <param name="keySeparator">
    ///     The separator to be used to separate the custom identifier and the unique part of the key.
    ///     Set to empty string for no separator.
    /// </param>
    /// <param name="timeGuidSeparator">
    ///     The separator to be used to separate the numerically increasing number at the end of
    ///     the unique key.
    /// </param>
    /// <param name="keyIdentifierFirst">
    ///     Set to True to have the key start with the key identifier.  If false the key will end
    ///     with this identifier.
    /// </param>
    /// <param name="constantTimeGuid">
    ///     Set to True to have the same TimeGuid portion of the key.  The only unique part will be
    ///     the number increasing at the end.
    /// </param>
    public UniqueKeys(string keySeparator = ":",
                      string timeGuidSeparator = "_",
                      bool keyIdentifierFirst = true,
                      bool constantTimeGuid = false)
    {
        _keyIdentifierFirst      = keyIdentifierFirst;
        _constantTimeGuid        = constantTimeGuid;
        WhatIsKeySeparator       = keySeparator;
        WhatIsIncrementSeparator = timeGuidSeparator;


        // If user wants a constant time guid object then go compute it now.  
        if (_constantTimeGuid)
            _timeGuid = new TimeGuid(DateTime.Now);
    }



    /// <summary>
    ///     Returns the separator being used to separate the random part of the key and the key incrementer
    /// </summary>
    public string WhatIsIncrementSeparator { get; }



    /// <summary>
    ///     Returns the separator being used to separate the user provided key value from the class generated unique part of
    ///     the key.
    /// </summary>
    public string WhatIsKeySeparator { get; }



    /// <summary>
    ///     Formats the key into the correct format.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    private string FormatKey(string identifier)
    {
        string keyIncVal;

        // We do not display the key incrementer if it is presently zero.
        if (_keyIncrementer > 0)
            keyIncVal = WhatIsIncrementSeparator + _keyIncrementer;
        else
            keyIncVal = "";

        // Now determine the order of the keying.
        if (_keyIdentifierFirst)
            return identifier + WhatIsKeySeparator + _timeGuid.ToString + keyIncVal;

        return _timeGuid.ToString + keyIncVal + WhatIsKeySeparator + identifier;
    }



    /// <summary>
    ///     Generates a new unique key based upon the base objects settings.
    /// </summary>
    /// <param name="uniqueIdentifier">
    ///     If you wish to have an identifier as part of the key then specify that here.  This will either start or end the
    ///     key depending upon the setting keyIdentifierFirst in the constructor.
    /// </param>
    /// <returns></returns>
    public string GetKey(string uniqueIdentifier = "Key") => IncrementKey(uniqueIdentifier);



    /// <summary>
    ///     Generates the random part of the key.  Depending on the setting for _constantTimeGUID it may or may not generate a
    ///     new TimeGuid.
    ///     If the new TimeGuid is the same as the old (because they were both generated within the same physical second, then
    ///     it will increment
    ///     the numeric incrementer by 1.  If _constantTimeGUID is true then it will always increment the numeric part by 1.
    /// </summary>
    /// <returns></returns>
    private string IncrementKey(string identifier)
    {
        string val;
        lock (keyIncrLock)
        {
            // If the TimeGuid needs to change, then see what new TimeGuid is.  If same as existing then we increment the _keyIncrementer
            // otherwise we reset KeyIncrementer to zero.
            if (!_constantTimeGuid)
            {
                TimeGuid newGuid = new(DateTime.Now);
                if (newGuid == _timeGuid)
                {
                    _keyIncrementer++;
                    val = FormatKey(identifier);
                }
                else
                {
                    _keyIncrementer = 0;
                    _timeGuid       = newGuid;
                    val             = FormatKey(identifier);
                }
            }
            else
            {
                _keyIncrementer++;
                val = FormatKey(identifier);
            }
        } // End Lock

        return val;
    }



    /// <summary>
    ///     Generates a new TimeGuid value and resets the incrementer back to zero.  This is the only way that this class can
    ///     get a new TimeGuid value if its _constantTimeGUID flag is
    ///     set to True.  If the generated TimeGuid is the same as the previous it will sleep for up to 1 second and then try
    ///     again.  Therefore, this function should not be called
    ///     rapidly in sequence or else performance of calling app will suffer.
    /// </summary>
    /// <returns></returns>
    public string RefreshKey(string uniqueIdentifier = "Key")
    {
        bool keepTrying = true;

        while (keepTrying)
        {
            TimeGuid newGuid = new(DateTime.Now);

            if (newGuid == _timeGuid)
            {
                int millisec  = DateTimeOffset.Now.Millisecond;
                int sleepTime = 1000 - millisec;

                Thread.Sleep(sleepTime);
            }
            else
            {
                _timeGuid  = newGuid;
                keepTrying = false;
            }
        }

        _keyIncrementer = 0;
        return FormatKey(uniqueIdentifier);
    }
}