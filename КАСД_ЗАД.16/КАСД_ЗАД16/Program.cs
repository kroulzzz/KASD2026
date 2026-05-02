using System;
using System.Collections;
using System.Collections.Generic;

public class MyLinkedList<T> : IEnumerable<T>
{
    private class Node
    {
        public T Data { get; set; }
        public Node Previous { get; set; }
        public Node Next { get; set; }

        public Node(T data)
        {
            Data = data;
            Previous = null;
            Next = null;
        }
    }

    private Node first;
    private Node last;
    private int size;

    public MyLinkedList()
    {
        first = null;
        last = null;
        size = 0;
    }

    public MyLinkedList(T[] a)
    {
        if (a == null)
            throw new ArgumentNullException(nameof(a));

        first = null;
        last = null;
        size = 0;

        foreach (var item in a)
        {
            AddLast(item);
        }
    }

    public void Add(T e) => AddLast(e);

    public void AddAll(T[] a)
    {
        if (a == null)
            throw new ArgumentNullException(nameof(a));

        foreach (var item in a)
        {
            AddLast(item);
        }
    }

    public void Clear()
    {
        first = null;
        last = null;
        size = 0;
    }

    public bool Contains(object o)
    {
        if (o == null)
        {
            var current = first;
            while (current != null)
            {
                if (current.Data == null)
                    return true;
                current = current.Next;
            }
            return false;
        }

        if (o is T item)
        {
            var current = first;
            while (current != null)
            {
                if (current.Data != null && current.Data.Equals(o))
                    return true;
                current = current.Next;
            }
        }

        return false;
    }

    public bool Contains(T item)
    {
        var current = first;
        while (current != null)
        {
            if (item == null)
            {
                if (current.Data == null)
                    return true;
            }
            else if (item.Equals(current.Data))
            {
                return true;
            }
            current = current.Next;
        }
        return false;
    }

    public bool ContainsAll(T[] a)
    {
        if (a == null)
            throw new ArgumentNullException(nameof(a));

        foreach (var item in a)
        {
            if (!Contains(item))
                return false;
        }
        return true;
    }

    public bool IsEmpty() => size == 0;

    public bool Remove(object o)
    {
        if (o == null)
        {
            return RemoveFirstOccurrence((T)(object)null);
        }

        if (o is T item)
        {
            return RemoveFirstOccurrence(item);
        }

        return false;
    }

    public bool RemoveAll(T[] a)
    {
        if (a == null)
            throw new ArgumentNullException(nameof(a));

        bool modified = false;
        foreach (var item in a)
        {
            while (RemoveFirstOccurrence(item))
            {
                modified = true;
            }
        }
        return modified;
    }

    public bool RetainAll(T[] a)
    {
        if (a == null)
            throw new ArgumentNullException(nameof(a));

        var set = new HashSet<T>();
        foreach (var item in a)
        {
            set.Add(item);
        }

        bool modified = false;
        var current = first;
        while (current != null)
        {
            var next = current.Next;
            if (!set.Contains(current.Data))
            {
                RemoveNode(current);
                modified = true;
            }
            current = next;
        }

        return modified;
    }

    public int Size() => size;

    public object[] ToArray()
    {
        object[] array = new object[size];
        var current = first;
        for (int i = 0; i < size; i++)
        {
            array[i] = current.Data;
            current = current.Next;
        }
        return array;
    }

    public T[] ToArray(T[] a)
    {
        if (a == null)
            return ToArrayTyped();

        if (a.Length < size)
        {
            return ToArrayTyped();
        }

        var current = first;
        for (int i = 0; i < size; i++)
        {
            a[i] = current.Data;
            current = current.Next;
        }

        if (a.Length > size)
        {
            a[size] = default;
        }

        return a;
    }

    private T[] ToArrayTyped()
    {
        T[] array = new T[size];
        var current = first;
        for (int i = 0; i < size; i++)
        {
            array[i] = current.Data;
            current = current.Next;
        }
        return array;
    }

    public void Add(int index, T e)
    {
        if (index < 0 || index > size)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (index == 0)
        {
            AddFirst(e);
        }
        else if (index == size)
        {
            AddLast(e);
        }
        else
        {
            var node = GetNodeAt(index);
            InsertBefore(node, e);
        }
    }

    public void AddAll(int index, T[] a)
    {
        if (index < 0 || index > size)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (a == null)
            throw new ArgumentNullException(nameof(a));

        if (a.Length == 0)
            return;

        if (index == size)
        {
            foreach (var item in a)
            {
                AddLast(item);
            }
        }
        else
        {
            var node = GetNodeAt(index);
            foreach (var item in a)
            {
                InsertBefore(node, item);
            }
        }
    }

    public T Get(int index)
    {
        if (index < 0 || index >= size)
            throw new ArgumentOutOfRangeException(nameof(index));

        return GetNodeAt(index).Data;
    }

    public int IndexOf(object o)
    {
        if (o == null)
        {
            var current = first;
            int index = 0;
            while (current != null)
            {
                if (current.Data == null)
                    return index;
                current = current.Next;
                index++;
            }
            return -1;
        }

        if (o is T item)
        {
            return IndexOf(item);
        }

        return -1;
    }

    public int IndexOf(T item)
    {
        var current = first;
        int index = 0;
        while (current != null)
        {
            if (item == null)
            {
                if (current.Data == null)
                    return index;
            }
            else if (item.Equals(current.Data))
            {
                return index;
            }
            current = current.Next;
            index++;
        }
        return -1;
    }

    public int LastIndexOf(object o)
    {
        if (o == null)
        {
            var current = last;
            int index = size - 1;
            while (current != null)
            {
                if (current.Data == null)
                    return index;
                current = current.Previous;
                index--;
            }
            return -1;
        }

        if (o is T item)
        {
            return LastIndexOf(item);
        }

        return -1;
    }

    public int LastIndexOf(T item)
    {
        var current = last;
        int index = size - 1;
        while (current != null)
        {
            if (item == null)
            {
                if (current.Data == null)
                    return index;
            }
            else if (item.Equals(current.Data))
            {
                return index;
            }
            current = current.Previous;
            index--;
        }
        return -1;
    }

    public T Remove(int index)
    {
        if (index < 0 || index >= size)
            throw new ArgumentOutOfRangeException(nameof(index));

        var node = GetNodeAt(index);
        RemoveNode(node);
        return node.Data;
    }

    public T Set(int index, T e)
    {
        if (index < 0 || index >= size)
            throw new ArgumentOutOfRangeException(nameof(index));

        var node = GetNodeAt(index);
        T oldValue = node.Data;
        node.Data = e;
        return oldValue;
    }

    public MyLinkedList<T> SubList(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || fromIndex > size)
            throw new ArgumentOutOfRangeException(nameof(fromIndex));
        if (toIndex < 0 || toIndex > size)
            throw new ArgumentOutOfRangeException(nameof(toIndex));
        if (fromIndex > toIndex)
            throw new ArgumentException("fromIndex > toIndex");

        var result = new MyLinkedList<T>();
        var current = GetNodeAt(fromIndex);
        for (int i = fromIndex; i < toIndex; i++)
        {
            result.AddLast(current.Data);
            current = current.Next;
        }
        return result;
    }

    public T Element()
    {
        if (first == null)
            throw new InvalidOperationException("List is empty");
        return first.Data;
    }

    public bool Offer(T obj)
    {
        try
        {
            AddLast(obj);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public T Peek()
    {
        if (first == null)
            return default;
        return first.Data;
    }

    public T Poll()
    {
        if (first == null)
            return default;

        T data = first.Data;
        RemoveFirst();
        return data;
    }

    public void AddFirst(T obj)
    {
        var newNode = new Node(obj);
        if (first == null)
        {
            first = last = newNode;
        }
        else
        {
            newNode.Next = first;
            first.Previous = newNode;
            first = newNode;
        }
        size++;
    }

    public void AddLast(T obj)
    {
        var newNode = new Node(obj);
        if (last == null)
        {
            first = last = newNode;
        }
        else
        {
            newNode.Previous = last;
            last.Next = newNode;
            last = newNode;
        }
        size++;
    }

    public T GetFirst()
    {
        if (first == null)
            throw new InvalidOperationException("List is empty");
        return first.Data;
    }

    public T GetLast()
    {
        if (last == null)
            throw new InvalidOperationException("List is empty");
        return last.Data;
    }

    public bool OfferFirst(T obj)
    {
        try
        {
            AddFirst(obj);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool OfferLast(T obj)
    {
        try
        {
            AddLast(obj);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public T Pop() => RemoveFirst();

    public void Push(T obj) => AddFirst(obj);

    public T PeekFirst()
    {
        if (first == null)
            return default;
        return first.Data;
    }

    public T PeekLast()
    {
        if (last == null)
            return default;
        return last.Data;
    }

    public T PollFirst() => Poll();

    public T PollLast()
    {
        if (last == null)
            return default;

        T data = last.Data;
        RemoveLast();
        return data;
    }

    public T RemoveLast()
    {
        if (last == null)
            throw new InvalidOperationException("List is empty");

        T data = last.Data;
        RemoveNode(last);
        return data;
    }

    public T RemoveFirst()
    {
        if (first == null)
            throw new InvalidOperationException("List is empty");

        T data = first.Data;
        RemoveNode(first);
        return data;
    }

    public bool RemoveLastOccurrence(object obj)
    {
        if (obj == null)
        {
            return RemoveLastOccurrence((T)(object)null);
        }

        if (obj is T item)
        {
            return RemoveLastOccurrence(item);
        }

        return false;
    }

    public bool RemoveLastOccurrence(T obj)
    {
        var current = last;
        while (current != null)
        {
            if (obj == null)
            {
                if (current.Data == null)
                {
                    RemoveNode(current);
                    return true;
                }
            }
            else if (obj.Equals(current.Data))
            {
                RemoveNode(current);
                return true;
            }
            current = current.Previous;
        }
        return false;
    }

    public bool RemoveFirstOccurrence(object obj)
    {
        if (obj == null)
        {
            return RemoveFirstOccurrence((T)(object)null);
        }

        if (obj is T item)
        {
            return RemoveFirstOccurrence(item);
        }

        return false;
    }

    public bool RemoveFirstOccurrence(T obj)
    {
        var current = first;
        while (current != null)
        {
            if (obj == null)
            {
                if (current.Data == null)
                {
                    RemoveNode(current);
                    return true;
                }
            }
            else if (obj.Equals(current.Data))
            {
                RemoveNode(current);
                return true;
            }
            current = current.Next;
        }
        return false;
    }

    private Node GetNodeAt(int index)
    {
        if (index < 0 || index >= size)
            throw new ArgumentOutOfRangeException(nameof(index));

        Node current;
        if (index < size / 2)
        {
            current = first;
            for (int i = 0; i < index; i++)
            {
                current = current.Next;
            }
        }
        else
        {
            current = last;
            for (int i = size - 1; i > index; i--)
            {
                current = current.Previous;
            }
        }
        return current;
    }

    private void RemoveNode(Node node)
    {
        if (node == null)
            return;

        if (node.Previous != null)
        {
            node.Previous.Next = node.Next;
        }
        else
        {
            first = node.Next;
        }

        if (node.Next != null)
        {
            node.Next.Previous = node.Previous;
        }
        else
        {
            last = node.Previous;
        }

        size--;
    }

    private void InsertBefore(Node node, T data)
    {
        var newNode = new Node(data);
        newNode.Previous = node.Previous;
        newNode.Next = node;

        if (node.Previous != null)
        {
            node.Previous.Next = newNode;
        }
        else
        {
            first = newNode;
        }

        node.Previous = newNode;
        size++;
    }

    public IEnumerator<T> GetEnumerator()
    {
        var current = first;
        while (current != null)
        {
            yield return current.Data;
            current = current.Next;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => size;
    public T First => GetFirst();
    public T Last => GetLast();
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Тестирование MyLinkedList ===");

        Console.WriteLine("\n1. Тест конструкторов:");
        var list1 = new MyLinkedList<int>();
        Console.WriteLine($"Пустой список: размер = {list1.Size()}, пуст? {list1.IsEmpty()}");

        int[] array = { 1, 2, 3, 4, 5 };
        var list2 = new MyLinkedList<int>(array);
        Console.WriteLine($"Список из массива: {string.Join(", ", list2)}");

        Console.WriteLine("\n2. Тест добавления элементов:");
        list1.Add(10);
        list1.Add(20);
        list1.Add(30);
        Console.WriteLine($"После Add: {string.Join(", ", list1)}");

        list1.AddFirst(5);
        list1.AddLast(40);
        Console.WriteLine($"После AddFirst/AddLast: {string.Join(", ", list1)}");

        Console.WriteLine("\n3. Тест вставки по индексу:");
        Console.WriteLine($"Текущий размер: {list1.Size()}");
        Console.WriteLine($"Текущее содержимое: {string.Join(", ", list1)}");

        int safeIndex = Math.Min(2, list1.Size() - 1);
        if (safeIndex >= 0)
        {
            list1.Add(safeIndex, 15);
            Console.WriteLine($"После Add({safeIndex}, 15): {string.Join(", ", list1)}");
        }

        Console.WriteLine("\n4. Тест получения элементов:");
        Console.WriteLine($"Первый: {list1.GetFirst()}, Последний: {list1.GetLast()}");
        if (list1.Size() >= 4)
        {
            Console.WriteLine($"Элемент по индексу 3: {list1.Get(3)}");
        }

        Console.WriteLine("\n5. Тест поиска:");
        Console.WriteLine($"Содержит 20? {list1.Contains(20)}");
        Console.WriteLine($"Содержит 100? {list1.Contains(100)}");
        Console.WriteLine($"Индекс 15: {list1.IndexOf(15)}");
        Console.WriteLine($"Индекс 20: {list1.IndexOf(20)}");

        Console.WriteLine("\n6. Тест удаления:");
        Console.WriteLine($"До удаления 15: {string.Join(", ", list1)}");
        if (list1.Contains(15))
        {
            bool removed = list1.Remove((object)15);
            Console.WriteLine($"Удаление 15: {removed}");
            Console.WriteLine($"После Remove(15): {string.Join(", ", list1)}");
        }
        else
        {
            Console.WriteLine("Элемент 15 не найден в списке");
        }

        Console.WriteLine("\n7. Тест RemoveFirst и RemoveLast:");
        Console.WriteLine($"До удалений: {string.Join(", ", list1)}");

        if (list1.Size() > 0)
        {
            try
            {
                int firstRemoved = list1.RemoveFirst();
                Console.WriteLine($"Удален первый элемент: {firstRemoved}");
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Список пуст, нельзя удалить первый элемент");
            }
        }

        if (list1.Size() > 0)
        {
            try
            {
                int lastRemoved = list1.RemoveLast();
                Console.WriteLine($"Удален последний элемент: {lastRemoved}");
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Список пуст, нельзя удалить последний элемент");
            }
        }

        Console.WriteLine($"После RemoveFirst/RemoveLast: {string.Join(", ", list1)}");

        Console.WriteLine("\n8. Тест AddAll:");
        int[] newArray = { 50, 60, 70 };
        list1.AddAll(newArray);
        Console.WriteLine($"После AddAll: {string.Join(", ", list1)}");

        Console.WriteLine("\n9. Тест ToArray:");
        if (list1.Size() > 0)
        {
            int[] arr = list1.ToArray(new int[list1.Size()]);
            Console.WriteLine($"Массив: {string.Join(", ", arr)}");
        }

        Console.WriteLine("\n10. Тест SubList:");
        if (list2.Size() >= 4)
        {
            var subList = list2.SubList(1, 4);
            Console.WriteLine($"SubList(1, 4): {string.Join(", ", subList)}");
        }
        else
        {
            Console.WriteLine("list2 слишком мал для создания SubList(1, 4)");
        }

        Console.WriteLine("\n11. Тест методов очереди:");
        var queueList = new MyLinkedList<int>();
        queueList.Offer(100);
        queueList.Offer(200);
        queueList.Offer(300);
        Console.WriteLine($"После Offer: {string.Join(", ", queueList)}");

        if (!queueList.IsEmpty())
        {
            Console.WriteLine($"Peek: {queueList.Peek()}");
            Console.WriteLine($"Poll: {queueList.Poll()}");
            Console.WriteLine($"После Poll: {string.Join(", ", queueList)}");
        }

        Console.WriteLine("\n12. Тест методов стека:");
        var stackList = new MyLinkedList<int>();
        stackList.Push(1);
        stackList.Push(2);
        stackList.Push(3);
        Console.WriteLine($"После Push: {string.Join(", ", stackList)}");

        if (!stackList.IsEmpty())
        {
            Console.WriteLine($"Pop: {stackList.Pop()}");
            Console.WriteLine($"После Pop: {string.Join(", ", stackList)}");
        }

        Console.WriteLine("\n13. Тест retainAll:");
        var retainList = new MyLinkedList<int>(new int[] { 1, 2, 3, 4, 5, 6, 7 });
        int[] retainArray = { 2, 4, 6 };
        Console.WriteLine($"До retainAll: {string.Join(", ", retainList)}");
        bool retained = retainList.RetainAll(retainArray);
        Console.WriteLine($"После retainAll {{2,4,6}} (изменен: {retained}): {string.Join(", ", retainList)}");

        Console.WriteLine("\n14. Тест со строками:");
        var stringList = new MyLinkedList<string>();
        stringList.Add("Hello");
        stringList.Add(null);
        stringList.Add("World");
        stringList.Add("!");
        Console.Write("Строковый список: ");
        foreach (var item in stringList)
        {
            Console.Write(item == null ? "[null] " : item + " ");
        }
        Console.WriteLine();
        Console.WriteLine($"Содержит null? {stringList.Contains(null)}");
        Console.WriteLine($"Индекс 'World': {stringList.IndexOf("World")}");

        Console.WriteLine("\n15. Тест Set и Get:");
        var testList = new MyLinkedList<int>(new int[] { 10, 20, 30, 40, 50 });
        Console.WriteLine($"До Set: {string.Join(", ", testList)}");

        if (testList.Size() > 2)
        {
            int oldValue = testList.Set(2, 35);
            Console.WriteLine($"Set(2, 35): старое значение = {oldValue}");
            Console.WriteLine($"После Set: {string.Join(", ", testList)}");
            Console.WriteLine($"Get(2): {testList.Get(2)}");
        }

        Console.WriteLine("\n16. Тест Remove по индексу:");
        if (testList.Size() > 1)
        {
            int removedByIndex = testList.Remove(1);
            Console.WriteLine($"Remove(1): удаленное значение = {removedByIndex}");
            Console.WriteLine($"После Remove по индексу: {string.Join(", ", testList)}");
        }

        Console.WriteLine("\n17. Тест AddAll с индексом:");
        int[] insertArray = { 25, 26, 27 };
        if (testList.Size() > 1)
        {
            testList.AddAll(1, insertArray);
            Console.WriteLine($"После AddAll(1, {{25,26,27}}): {string.Join(", ", testList)}");
        }

        Console.WriteLine("\n18. Тест методов вхождения:");
        var occurrenceList = new MyLinkedList<int>(new int[] { 1, 2, 3, 2, 4, 2, 5 });
        Console.WriteLine($"До удалений: {string.Join(", ", occurrenceList)}");

        bool firstRemovedOcc = occurrenceList.RemoveFirstOccurrence(2);
        Console.WriteLine($"RemoveFirstOccurrence(2): {firstRemovedOcc}");
        Console.WriteLine($"После первого удаления: {string.Join(", ", occurrenceList)}");

        bool lastRemovedOcc = occurrenceList.RemoveLastOccurrence(2);
        Console.WriteLine($"RemoveLastOccurrence(2): {lastRemovedOcc}");
        Console.WriteLine($"После последнего удаления: {string.Join(", ", occurrenceList)}");

        // 20. Тест очистки
        Console.WriteLine("\n19. Тест очистки:");
        Console.WriteLine($"До Clear: размер = {testList.Size()}");
        testList.Clear();
        Console.WriteLine($"После Clear: размер = {testList.Size()}, пуст? {testList.IsEmpty()}");

        Console.WriteLine("\n=== Тестирование завершено ===");
        Console.WriteLine("\nНажмите любую клавишу для выхода...");
        Console.ReadKey();
    }
}
