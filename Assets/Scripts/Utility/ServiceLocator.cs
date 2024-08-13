using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    private static readonly Dictionary<Type, MonoBehaviour> services = new ();
    
    public static void Register<T>(T service) where T : MonoBehaviour
    {
        services[typeof(T)] = service;
    }
    
    public static void Deregister<T>() where T : MonoBehaviour
    {
        services.Remove(typeof(T));
    }

    public static T GetService<T>() where T : MonoBehaviour
    {
        if(!services.ContainsKey(typeof(T)))
            throw new ApplicationException ("Service does not exist in ServiceLocator!");
        
        return (T)services[typeof(T)];
    }
}