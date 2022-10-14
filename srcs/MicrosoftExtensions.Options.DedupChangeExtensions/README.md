# MicrosoftExtensions.Options.DedupChangeExtensions #

This project help silent the duplicated changing callbacks for `IOptionsMonitor`. See https://github.com/aspnet/Home/issues/2542 for more details.

## Basic Idea ##

Calculate the object hash & compare the hash value when changing callbacks happened.

The hash value is calculated by `BinaryFormatter` then `SHA1`.
