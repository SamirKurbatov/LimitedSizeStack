using System;
using System.Collections.Generic;

namespace LimitedSizeStack;

public class LimitedSizeStack<T>
{
    private readonly int undoLimit;

    public LinkedList<T> Items { get; private set; }

    public LimitedSizeStack(int undoLimit)
    {
        this.undoLimit = undoLimit;
        Items = new();
    }

    public void Push(T item)
    {
        if (undoLimit == 0)
        {
            return;
        }

        if (Items.Count == undoLimit)
        {
            Items.RemoveFirst();
            Count--;
        }

        Items.AddLast(item);
        Count++;

    }

    public T Pop()
    {
        var last = Items.Last.Value;
        Items.RemoveLast();
        Count--;
        return last;
    }

    public int Count { get; set; }
}

