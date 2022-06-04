using System;
using System.Collections.Generic;
using System.Linq;
using Conditions;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    private Dictionary<Func<bool>, Action> ConditionalEvents = new Dictionary<Func<bool>, Action>();
    private Dictionary<Func<bool>, Action<object>> ConditionalEventsData = new Dictionary<Func<bool>, Action<object>>();
    private Dictionary<Enum, Action> EmittedEvents = new Dictionary<Enum, Action>();
    private Dictionary<Enum, Action<object>> EmittedEventsData = new Dictionary<Enum, Action<object>>();

    private void Awake()
    {
        Instance = this;
        var eventTypeList = GetAllEnumTypeList(nameof(EventTypes));
        
        foreach (var eventType in eventTypeList)
        {
            EmittedEvents.Add(eventType, delegate { });
            EmittedEventsData.Add(eventType, delegate(object o) {  });
        }

        foreach (var condition in Conditional.ArrayAll)
        {
            ConditionalEvents.Add(condition, () => {});
        }
    }
    private void Update()
    {
        CheckConditionDictionary();
    }
    
    /// <summary>
    /// Invoke subscribers of the emitted event type.
    /// </summary>
    /// <param name="eventType">Enum type defined in the EventType namespace.</param>
    /// <param name="data">Data to be invoked as a parameter, defaults to null.</param>
    public void EmitEvent(Enum eventType, object data = null)
    {
        EmittedEvents[eventType].Invoke();
        if (data != null)
        {
            EmittedEventsData[eventType].Invoke(data);
        }
    }
    /// <summary>
    /// Subscribe to an emitted event with an Action.
    /// </summary>
    /// <param name="eventType">Enum defined in the EventTypes namespace.</param>
    /// <param name="subscriber">The Action to be invoked.</param>
    public void Subscribe(Enum eventType, Action subscriber) => EmittedEvents[eventType] += subscriber;
    /// <summary>
    /// Subscribe to an emitted event using an Action with a single parameter.
    /// </summary>
    /// <param name="eventType">Enum defined in the EventTypes namespace.</param>
    /// <param name="subscriber">The Action to be invoked. In the event you need more parameters, Tuples are suggested.</param>
    public void Subscribe(Enum eventType, Action<object> subscriber) => EmittedEventsData[eventType] += subscriber;
    /// <summary>
    /// Subscribe to conditional event using an Action.
    /// </summary>
    /// <param name="condition">Condition to be subscribed to.</param>
    /// <param name="subscriber">Action to subscribe with.</param>
    public void Subscribe(Func<bool> condition, Action subscriber)
    {
        if (ConditionalEvents.ContainsKey(condition))
        {
            ConditionalEvents[condition] += subscriber;
        }
        else
        {
            ConditionalEvents.Add(condition, () => {});
            ConditionalEvents[condition] += subscriber;
        }
    }
    /// <summary>
    /// Subscribe to defined conditional event using an Action with a parameter.
    /// </summary>
    /// <param name="condition">The condition to be subscribed to.</param>
    /// <param name="subscriber">The Action to receive a parameter to do the subscribing.</param>
    public void Subscribe(Func<bool> condition, Action<object> subscriber)
    {
        if (ConditionalEventsData.ContainsKey(condition))
        {
            ConditionalEventsData[condition] += subscriber;
        }
        else
        {
            ConditionalEventsData.Add(condition, o => {} );
            ConditionalEventsData[condition] += subscriber;
        }
    }
    /// <summary>
    /// Unsubscribe an Action from an emitted EventType.
    /// </summary>
    /// <param name="eventType">Enum defined in the EventTypes namespace.</param>
    /// <param name="unSub">The Action to be unsubscribed.</param>
    public void UnSubscribe(Enum eventType, Action unSub)
    {
        if (EmittedEvents.ContainsKey(eventType))
        {
            EmittedEvents[eventType] -= unSub;
        }
    }
    /// <summary>
    /// Unsubscribe an Action with a parameter from an emitted EventType.
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="unsub">The Action to be unsubscribed. Be sure the signature matches that used in the Subscribe method.</param>
    public void UnSubScribe(Enum eventType, Action<object> unsub)
    {
        if (EmittedEventsData.ContainsKey(eventType))
        {
            EmittedEventsData[eventType] -= unsub;
        }
    } 
    /// <summary>
    /// An action triggered to evaluate any conditional events created in the ConditionDictionary.
    /// </summary>
    private void CheckConditionDictionary()
        {
            foreach (var condition in ConditionalEvents.Keys.Where(condition => condition.Invoke()))
            {
                ConditionalEvents[condition].Invoke();
            }
        }
    
    /// <summary>
    /// Collect all EventTypes iterated in a particular namespace.
    /// </summary>
    /// <param name="namespaceName">Use nameof(NAMESPACE) to avoid typos</param>
    /// <returns>List of all enums within the namespace.</returns>
    private static List<Enum> GetAllEnumTypeList(string namespaceName)
    {
        var list = new List<Enum>();
        var allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes());
        foreach (var type in allTypes)
        {
            if (type.Namespace == namespaceName && type.IsEnum)
            {
                foreach (Enum enumValue in Enum.GetValues(type))
                {
                    list.Add(enumValue);
                }
            }
        }

        return list;
    }
}