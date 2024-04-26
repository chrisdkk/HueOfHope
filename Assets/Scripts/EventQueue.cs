using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class EventQueue
{
    private Queue<Action> events = new Queue<Action>();

    public void AddEvent(Action newEvent)
    {
        events.Enqueue(newEvent);
    }

    public Action GetNextEvent()
    {
        return events.Dequeue();
    }

    public bool HasEvents()
    {
        return events.Count > 0;
    }
}