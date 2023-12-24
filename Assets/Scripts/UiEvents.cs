using System;

using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Provides quick and simple access to Unity Event System
/// that allows to subscribe listeners to specified UI objects.
/// </summary>
public static class UiEvents
{
    /// <summary>
    /// Adds the start drag listener.
    /// </summary>
    /// <param name="target">Target GameObject</param>
    /// <param name="handler">Handler</param>
    public static void AddStartDragListener(GameObject target, Action handler)
    {
        AddEventListener(target, handler, EventTriggerType.BeginDrag);
    }

    /// <summary>
    /// Adds the stop drag listener.
    /// </summary>
    /// <param name="target">Target GameObject</param>
    /// <param name="handler">Handler</param>
    public static void AddStopDragListener(GameObject target, Action handler)
    {
        AddEventListener(target, handler, EventTriggerType.EndDrag);
    }

    /// <summary>
    /// Adds the mouse over listener.
    /// </summary>
    /// <param name="target">Target GameObject</param>
    /// <param name="handler">Handler</param>
    public static void AddMouseOverListener(GameObject target, Action handler)
    {
        AddEventListener(target, handler, EventTriggerType.PointerEnter);
    }

    /// <summary>
    /// Adds the mouse out listener.
    /// </summary>
    /// <param name="target">Target GameObject</param>
    /// <param name="handler">Handler</param>
    public static void AddMouseOutListener(GameObject target, Action handler)
    {
        AddEventListener(target, handler, EventTriggerType.PointerExit);
    }

    /// <summary>
    /// Adds the mouse press listener.
    /// </summary>
    /// <param name="target">Target GameObject</param>
    /// <param name="handler">Handler</param>
    public static void AddMousePressListener(GameObject target, Action handler)
    {
        AddEventListener(target, handler, EventTriggerType.PointerDown);

        for (int i = 0; i < target.transform.childCount; i++)
        {
            AddMousePressListener(target.transform.GetChild(i).gameObject, handler);
        }
    }

    /// <summary>
    /// Adds the mouse release listener.
    /// </summary>
    /// <param name="target">Target GameObject</param>
    /// <param name="handler">Handler</param>
    public static void AddMouseReleaseListener(GameObject target, Action handler)
    {
        AddEventListener(target, handler, EventTriggerType.PointerUp);

        for (int i = 0; i < target.transform.childCount; i++)
        {
            AddMouseReleaseListener(target.transform.GetChild(i).gameObject, handler);
        }
    }

    /// <summary>
    /// Adds the mouse click listener.
    /// </summary>
    /// <param name="target">Target GameObject</param>
    /// <param name="handler">Handler</param>
    public static void AddClickListener(GameObject target, Action handler)
    {
        AddEventListener(target, handler, EventTriggerType.PointerUp);

        for (int i = 0; i < target.transform.childCount; i++)
        {
            AddClickListener(target.transform.GetChild(i).gameObject, handler);
        }
    }

    // Adds listener to GameObject's EventTrigger
    private static void AddEventListener(GameObject target, Action handler, EventTriggerType eventID)
    {
        var trigger = target.GetComponent<EventTrigger>();

        if (trigger == null)
        {
            trigger = target.AddComponent<EventTrigger>();
        }
        
        var entry = new EventTrigger.Entry();

        entry.eventID = eventID;
        entry.callback.AddListener((eventData) => { handler(); });

        trigger.triggers.Add(entry);
    }
}
