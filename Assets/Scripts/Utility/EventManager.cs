/*
 * Inspired by Jeffrey Richter's EventSet from CLR via C#, but differs in a couple significant ways.
 * EventManager is intended to be used as a centralized hub for messages that can be accessed from anywhere.
 * I restricted the delegate type to Action<T> only, thus losing some versatility but avoiding DynamicInvoke.
 * To declare an event with no arguments (well, kind of) use an empty struct like "Void".
 * Raise the event like so: EventManager.Raise("EventKey", new Void());
 * ^if you're coming from a C++ background - no, this does not allocate memory on the heap.
 * ^look into value types vs. reference types in C#.
 *
 * I recommend changing EventKey to something more scalable than an enum if:
 * - you're working with multiple other programmers
 * - you have more than ~20-30 events.
 *
 * Don't use this on large projects. You have been warned.
 *
 * Sergei Grigorev, 2023
 */

using System;
using System.Collections.Generic;

public struct Void {}

public static class EventManager
{
    private static Dictionary<EventKey, Delegate> data = new();

    public static void AddListener<T>(EventKey key, Action<T> listener)
    {
        data.TryGetValue(key, out Delegate d);
        data[key] = Delegate.Combine(d, listener);
    }

    public static void RemoveListener<T>(EventKey key, Action<T> listener)
    {
        if (data.TryGetValue(key, out Delegate d))
        {
            d = Delegate.Remove(d, listener);
            if (d != null) data[key] = d;
            else data.Remove(key);
        }
    }
    
    public static void Raise<T>(EventKey key, T parameter)
    {
        data.TryGetValue(key, out Delegate d);

        if (d != null)
        {
            Action<T> action = (Action<T>)d;
            action.Invoke(parameter);
        }
    }
}