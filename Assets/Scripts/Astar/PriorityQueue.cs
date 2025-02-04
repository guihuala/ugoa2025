using System.Collections;
using System.Collections.Generic;

public class PriorityQueue<T>
{
    private SortedList<float, Queue<T>> elements = new SortedList<float, Queue<T>>();

    public int Count { get; private set; } = 0;

    public void Enqueue(T item, float priority)
    {
        if (!elements.ContainsKey(priority))
            elements[priority] = new Queue<T>();

        elements[priority].Enqueue(item);
        Count++;
    }

    public T Dequeue()
    {
        if (elements.Count == 0) return default;

        var firstKey = elements.Keys[0];
        var queue = elements[firstKey];

        T bestItem = queue.Dequeue();
        if (queue.Count == 0) elements.Remove(firstKey);

        Count--;
        return bestItem;
    }

    public bool Contains(T item)
    {
        foreach (var queue in elements.Values)
        {
            if (queue.Contains(item))
                return true;
        }
        return false;
    }
}
