using Avalonia.DesignerSupport.Remote;
using DynamicData;
using NUnit.Framework;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Net.WebSockets;
using System.Windows.Input;


namespace LimitedSizeStack;

public class ListModel<TItem>
{
    public List<TItem> Items { get; }

    public int UndoLimit;

    private LimitedSizeStack<BaseCommand<TItem>> commandHistory;

    public ListModel(int undoLimit) : this(new List<TItem>(), undoLimit)
    {
    }

    public ListModel(List<TItem> items, int undoLimit)
    {
        Items = items;
        UndoLimit = undoLimit;
        commandHistory = new(undoLimit);

    }

    public void AddItem(TItem item)
    {
        var command = new AddCommand<TItem>(item, 0);

        command.Execute(Items);

        commandHistory.Push(command);

        command.Index++;
    }

    public void RemoveItem(int index)
    {
        var removedItem = Items[index];

        var command = new RemoveCommand<TItem>(removedItem, index);

        command.Execute(Items);

        commandHistory.Push(command);

    }

    public bool CanUndo()
    {
        return commandHistory.Count != 0;
    }

    public void Undo()
    {
        if (CanUndo())
        {
            var command = commandHistory.Pop();

            command.Undo(Items);
        }
    }
}

interface ICommand<TITem>
{
    void Execute(List<TITem> items);
    void Undo(List<TITem> items);
}

public abstract class BaseCommand<TItem> : ICommand<TItem>
{
    public int Index { get; set; }

    public BaseCommand(int index)
    {
        Index = index;
    }

    public abstract void Execute(List<TItem> items);

    public abstract void Undo(List<TItem> items);
}

public class AddCommand<TItem> : BaseCommand<TItem>
{
    private TItem item;

    public AddCommand(TItem item, int index) : base(index)
    {
        this.item = item;
    }

    public override void Execute(List<TItem> items)
    {
        items.Add(item);
    }

    public override void Undo(List<TItem> items)
    {
        items.Remove(item);
    }
}

public class RemoveCommand<TItem> : BaseCommand<TItem>
{
    private TItem removedItem;

    public RemoveCommand(TItem removedItem, int index) : base(index)
    {
        this.removedItem = removedItem;
    }

    public override void Execute(List<TItem> items)
    {
        if (Index >= 0 && Index < items.Count)
        {
            removedItem = items[Index];
            items.RemoveAt(Index);
        }
    }

    public override void Undo(List<TItem> items)
    {
        items.Insert(Index, removedItem);
    }
}

