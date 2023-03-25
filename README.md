# UniqueKeys

UniqueKeys is a C# class whose real usefulness may be limited.  I use it mainly to generate unique keys for unit test in which it 
is important that the keys have some meaning built into them, can be easily typed into other applications for debugging or other
purposes and who are guaranteed to be unique across many runs of the application for any given day.

The use of this class in a production situation would be very limiting.

## Overview
UniqueKeys builds string "keys" in one of several formats.  However, no matter how formatted, it really has 3 distinct parts:
1. Unique-Identifier:  
   This part of the key is just some text string that the caller passes to the GetKey method that helps to classify or identify this 
   key value amongst all the keys created.  For instance, if the key is related to a person object it might be Person or PER.  If 
   related to a car object it might be CAR or Car or Auto, it does not matter, it is simply a means for the caller to provide some
   clarity to what the key is representing or being used for.  The default value is "Key".

2. Unique-TimeValue:
   This part of the key is one of two parts that make up the random part of the key.  It consists of a [SlugEnt.TimeGuid](https://bitbucket.org/wshpersonal/timeguid/src/develop/) object.  
   This is simply a time representation of H:M:S but reduced to a 3 character value.  Example of this value would be:  L4A.

3. Unique-TimeValue-Incrementer:
   This part of the key is simply a numerically increasing value.  It may or may not be present depending on what type of key is 
   being created and/or the value of Unique-TimeValue.  If successive calls to GetKey occur within a single physical second then this 
   value will be incremented and appended as part of the key. If the caller set UniqueKeys to use a Constant-TimeGuid value then this
   value is incremented and appended on every call to Get-Key.

In addition to the above parts of the key there are 2 more parameters that can be set via the constructor.
1. Key-Separator:
   This is a the text string/character that is used to separate the 2 main key parts:  Unique-Identifier and the
   Unique-TimeValue/Incrementer.  It defaults to a ":".
2. TimeGuid-Separator:
   This is the text string/character that is used to separate the Unique-TimeValue from the Incrementer.  Defaults to "_".


The UniqueKeys class has a single constructor and a couple of methods.  It's quite simple in reality.
The constructor allows you to set whether the Unique-TimeValue or the Unique-Identifier is the first part of the generated key.
It also allows you to define the separators.

Finally, it allows you to set whether you have a Constant Unique-TimeValue or it will change as the time of key generation occurs.
This can be useful, depending upon what you are testing.  For instance, if you want to be able to uniquely identify all values created
during a given test run, then setting it to a Constant Unique-TimeValue will result in the Unique-TimeValue being defined once when the 
class is created and then never changed during the lifetime of this class (Except if RefreshKey is called).  If you also then define the key
format to put the Unique-TimeValue as the first part of the key then all generated keys will start with this same value.  


## Usage

```
#!CSharp
// Create a new instance of the class.  
UniqueKeys uniqueKey = new UniqueKeys();

// Generates a key like "Key:D4Y"
string key1 = uniqueKeys.GetKey ();

// Generates a key like "Car:Hxh"
string key1 = uniqueKeys.GetKey ("Car");

// Generates a key like "Car:Hyj" if generated about 30 seconds after key1.
string key2 = uniqueKeys.GetKey ("Car");



UniqueKeys uniqueKey = new UniqueKeys(":","_",false,true);
// Generates a key like "HyJ:Car"
string key1 = uniqueKeys.GetKey ("Car");

// Generates a key like "HyJ_1:Car"
string key2 = uniqueKeys.GetKey ("Car");

// Generates a key like "HyJ_2:Car"
string key3 = uniqueKeys.GetKey ("Car");

// Generates a key like "HyJ_3:Person"
string key4 = uniqueKeys.GetKey ("Person");


```


