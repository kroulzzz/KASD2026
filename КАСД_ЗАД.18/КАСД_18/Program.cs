using System;
using System.Collections.Generic;

namespace MyTreeMapSolution
{
    public delegate int TreeMapComparator<T>(T x, T y);

    public class MyTreeMap<K, V> where K : IComparable<K>
    {
        private class Node
        {
            public K Key;
            public V Value;
            public Node Left;
            public Node Right;

            public Node(K key, V value)
            {
                Key = key;
                Value = value;
                Left = null;
                Right = null;
            }
        }

        public class Entry
        {
            public K Key { get; }
            public V Value { get; }

            public Entry(K key, V value)
            {
                Key = key;
                Value = value;
            }

            public override string ToString()
            {
                return $"[{Key} = {Value}]";
            }
        }

        private TreeMapComparator<K> comparator;
        private Node root;
        private int size;

        public MyTreeMap()
        {
            comparator = (x, y) => x.CompareTo(y);
            root = null;
            size = 0;
        }

        public MyTreeMap(TreeMapComparator<K> comp)
        {
            comparator = comp ?? ((x, y) => x.CompareTo(y));
            root = null;
            size = 0;
        }

        public void Clear()
        {
            root = null;
            size = 0;
        }

        public bool ContainsKey(K key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return GetNode(key) != null;
        }

        public bool ContainsValue(V value)
        {
            return ContainsValueRecursive(root, value);
        }

        private bool ContainsValueRecursive(Node node, V value)
        {
            if (node == null)
                return false;

            if (EqualityComparer<V>.Default.Equals(node.Value, value))
                return true;

            return ContainsValueRecursive(node.Left, value) ||
                   ContainsValueRecursive(node.Right, value);
        }

        public HashSet<Entry> EntrySet()
        {
            var entries = new HashSet<Entry>();
            InOrderTraversal(root, node => entries.Add(new Entry(node.Key, node.Value)));
            return entries;
        }

        public V Get(K key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var node = GetNode(key);
            return node != null ? node.Value : default;
        }

        public bool IsEmpty()
        {
            return size == 0;
        }

        public HashSet<K> KeySet()
        {
            var keys = new HashSet<K>();
            InOrderTraversal(root, node => keys.Add(node.Key));
            return keys;
        }

        public V Put(K key, V value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            V oldValue = default;
            bool found = false;

            root = PutRecursive(root, key, value, ref oldValue, ref found);

            if (!found)
                size++;

            return oldValue;
        }

        private Node PutRecursive(Node node, K key, V value, ref V oldValue, ref bool found)
        {
            if (node == null)
            {
                found = false;
                return new Node(key, value);
            }

            int cmp = comparator(key, node.Key);

            if (cmp < 0)
            {
                node.Left = PutRecursive(node.Left, key, value, ref oldValue, ref found);
            }
            else if (cmp > 0)
            {
                node.Right = PutRecursive(node.Right, key, value, ref oldValue, ref found);
            }
            else
            {
                oldValue = node.Value;
                node.Value = value;
                found = true;
            }

            return node;
        }

        public V Remove(K key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            V oldValue = default;
            bool removed = false;
            root = RemoveRecursive(root, key, ref oldValue, ref removed);

            if (removed)
                size--;

            return oldValue;
        }

        private Node RemoveRecursive(Node node, K key, ref V oldValue, ref bool removed)
        {
            if (node == null)
                return null;

            int cmp = comparator(key, node.Key);

            if (cmp < 0)
            {
                node.Left = RemoveRecursive(node.Left, key, ref oldValue, ref removed);
            }
            else if (cmp > 0)
            {
                node.Right = RemoveRecursive(node.Right, key, ref oldValue, ref removed);
            }
            else
            {
                oldValue = node.Value;
                removed = true;

                if (node.Left == null)
                    return node.Right;
                else if (node.Right == null)
                    return node.Left;

                Node minRight = GetMinNode(node.Right);
                node.Key = minRight.Key;
                node.Value = minRight.Value;
                node.Right = RemoveRecursive(node.Right, minRight.Key, ref oldValue, ref removed);
                removed = false;
            }

            return node;
        }

        private K MinValue(Node node)
        {
            K minValue = node.Key;
            while (node.Left != null)
            {
                minValue = node.Left.Key;
                node = node.Left;
            }
            return minValue;
        }

        public int Size()
        {
            return size;
        }

        public K FirstKey()
        {
            if (root == null)
                throw new InvalidOperationException("Tree is empty");

            return GetMinNode(root).Key;
        }

        public K LastKey()
        {
            if (root == null)
                throw new InvalidOperationException("Tree is empty");

            return GetMaxNode(root).Key;
        }

        public MyTreeMap<K, V> HeadMap(K end)
        {
            var result = new MyTreeMap<K, V>(comparator);
            BuildHeadMap(root, end, result);
            return result;
        }

        private void BuildHeadMap(Node node, K end, MyTreeMap<K, V> result)
        {
            if (node == null)
                return;

            int cmp = comparator(node.Key, end);

            if (cmp < 0)
            {
                result.Put(node.Key, node.Value);
                BuildHeadMap(node.Right, end, result);
            }

            BuildHeadMap(node.Left, end, result);
        }

        public MyTreeMap<K, V> SubMap(K start, K end)
        {
            if (comparator(start, end) >= 0)
                throw new ArgumentException("Start key must be less than end key");

            var result = new MyTreeMap<K, V>(comparator);
            BuildSubMap(root, start, end, result);
            return result;
        }

        private void BuildSubMap(Node node, K start, K end, MyTreeMap<K, V> result)
        {
            if (node == null)
                return;

            int cmpStart = comparator(node.Key, start);
            int cmpEnd = comparator(node.Key, end);

            if (cmpStart >= 0 && cmpEnd < 0)
            {
                result.Put(node.Key, node.Value);
            }

            if (cmpStart > 0)
            {
                BuildSubMap(node.Left, start, end, result);
            }

            if (cmpEnd < 0)
            {
                BuildSubMap(node.Right, start, end, result);
            }
        }

        public MyTreeMap<K, V> TailMap(K start)
        {
            var result = new MyTreeMap<K, V>(comparator);
            BuildTailMap(root, start, result);
            return result;
        }

        private void BuildTailMap(Node node, K start, MyTreeMap<K, V> result)
        {
            if (node == null)
                return;

            int cmp = comparator(node.Key, start);

            if (cmp > 0)
            {
                result.Put(node.Key, node.Value);
                BuildTailMap(node.Left, start, result);
            }

            BuildTailMap(node.Right, start, result);
        }

        public Entry LowerEntry(K key)
        {
            Node node = LowerNode(root, key);
            return node != null ? new Entry(node.Key, node.Value) : null;
        }

        public Entry FloorEntry(K key)
        {
            Node node = FloorNode(root, key);
            return node != null ? new Entry(node.Key, node.Value) : null;
        }

        public Entry HigherEntry(K key)
        {
            Node node = HigherNode(root, key);
            return node != null ? new Entry(node.Key, node.Value) : null;
        }

        public Entry CeilingEntry(K key)
        {
            Node node = CeilingNode(root, key);
            return node != null ? new Entry(node.Key, node.Value) : null;
        }

        public K LowerKey(K key)
        {
            Node node = LowerNode(root, key);
            return node != null ? node.Key : default;
        }

        public K FloorKey(K key)
        {
            Node node = FloorNode(root, key);
            return node != null ? node.Key : default;
        }

        public K HigherKey(K key)
        {
            Node node = HigherNode(root, key);
            return node != null ? node.Key : default;
        }

        public K CeilingKey(K key)
        {
            Node node = CeilingNode(root, key);
            return node != null ? node.Key : default;
        }

        public Entry PollFirstEntry()
        {
            if (root == null)
                return null;

            var first = GetMinNode(root);
            Remove(first.Key);
            return new Entry(first.Key, first.Value);
        }

        public Entry PollLastEntry()
        {
            if (root == null)
                return null;

            var last = GetMaxNode(root);
            Remove(last.Key);
            return new Entry(last.Key, last.Value);
        }

        public Entry FirstEntry()
        {
            if (root == null)
                return null;

            var first = GetMinNode(root);
            return new Entry(first.Key, first.Value);
        }

        public Entry LastEntry()
        {
            if (root == null)
                return null;

            var last = GetMaxNode(root);
            return new Entry(last.Key, last.Value);
        }

        private Node GetNode(K key)
        {
            Node current = root;
            while (current != null)
            {
                int cmp = comparator(key, current.Key);
                if (cmp == 0)
                    return current;
                else if (cmp < 0)
                    current = current.Left;
                else
                    current = current.Right;
            }
            return null;
        }

        private void InOrderTraversal(Node node, Action<Node> action)
        {
            if (node == null)
                return;

            InOrderTraversal(node.Left, action);
            action(node);
            InOrderTraversal(node.Right, action);
        }

        private Node GetMinNode(Node node)
        {
            if (node == null)
                return null;

            while (node.Left != null)
            {
                node = node.Left;
            }
            return node;
        }

        private Node GetMaxNode(Node node)
        {
            if (node == null)
                return null;

            while (node.Right != null)
            {
                node = node.Right;
            }
            return node;
        }

        private Node LowerNode(Node node, K key)
        {
            Node result = null;
            while (node != null)
            {
                int cmp = comparator(node.Key, key);
                if (cmp < 0)
                {
                    result = node;
                    node = node.Right;
                }
                else
                {
                    node = node.Left;
                }
            }
            return result;
        }

        private Node FloorNode(Node node, K key)
        {
            Node result = null;
            while (node != null)
            {
                int cmp = comparator(node.Key, key);
                if (cmp == 0)
                    return node;
                else if (cmp < 0)
                {
                    result = node;
                    node = node.Right;
                }
                else
                {
                    node = node.Left;
                }
            }
            return result;
        }

        private Node HigherNode(Node node, K key)
        {
            Node result = null;
            while (node != null)
            {
                int cmp = comparator(node.Key, key);
                if (cmp > 0)
                {
                    result = node;
                    node = node.Left;
                }
                else
                {
                    node = node.Right;
                }
            }
            return result;
        }

        private Node CeilingNode(Node node, K key)
        {
            Node result = null;
            while (node != null)
            {
                int cmp = comparator(node.Key, key);
                if (cmp == 0)
                    return node;
                else if (cmp > 0)
                {
                    result = node;
                    node = node.Left;
                }
                else
                {
                    node = node.Right;
                }
            }
            return result;
        }

        public void PrintInOrder()
        {
            Console.Write("In-order: ");
            InOrderTraversal(root, node => Console.Write($"[{node.Key}:{node.Value}] "));
            Console.WriteLine();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Тестирование MyTreeMap ===");

            var treeMap = new MyTreeMap<int, string>();

            Console.WriteLine("\n1. Добавление элементов:");
            treeMap.Put(5, "Five");
            treeMap.Put(3, "Three");
            treeMap.Put(7, "Seven");
            treeMap.Put(2, "Two");
            treeMap.Put(4, "Four");
            treeMap.Put(6, "Six");
            treeMap.Put(8, "Eight");

            treeMap.PrintInOrder();
            Console.WriteLine($"Размер: {treeMap.Size()}");

            Console.WriteLine("\n2. Поиск элементов:");
            Console.WriteLine($"Get(3): {treeMap.Get(3)}");
            Console.WriteLine($"Get(10): {treeMap.Get(10)}");
            Console.WriteLine($"ContainsKey(4): {treeMap.ContainsKey(4)}");
            Console.WriteLine($"ContainsValue(\"Six\"): {treeMap.ContainsValue("Six")}");

            Console.WriteLine("\n3. Граничные элементы:");
            Console.WriteLine($"FirstKey: {treeMap.FirstKey()}");
            Console.WriteLine($"LastKey: {treeMap.LastKey()}");
            Console.WriteLine($"FirstEntry: {treeMap.FirstEntry()}");
            Console.WriteLine($"LastEntry: {treeMap.LastEntry()}");

            Console.WriteLine("\n4. Навигация:");
            Console.WriteLine($"LowerEntry(5): {treeMap.LowerEntry(5)}");
            Console.WriteLine($"FloorEntry(5): {treeMap.FloorEntry(5)}");
            Console.WriteLine($"HigherEntry(5): {treeMap.HigherEntry(5)}");
            Console.WriteLine($"CeilingEntry(5): {treeMap.CeilingEntry(5)}");

            Console.WriteLine("\n5. Подотображения:");
            var headMap = treeMap.HeadMap(5);
            Console.Write("HeadMap(<5): ");
            headMap.PrintInOrder();

            var subMap = treeMap.SubMap(3, 7);
            Console.Write("SubMap(3-7): ");
            subMap.PrintInOrder();

            var tailMap = treeMap.TailMap(5);
            Console.Write("TailMap(>5): ");
            tailMap.PrintInOrder();

            Console.WriteLine("\n6. Удаление элементов:");
            Console.WriteLine($"Remove(3): {treeMap.Remove(3)}");
            Console.WriteLine($"Remove(10): {treeMap.Remove(10)}");
            treeMap.PrintInOrder();
            Console.WriteLine($"Размер после удаления: {treeMap.Size()}");

            Console.WriteLine("\n7. Poll методы:");
            Console.WriteLine($"PollFirstEntry: {treeMap.PollFirstEntry()}");
            Console.WriteLine($"PollLastEntry: {treeMap.PollLastEntry()}");
            treeMap.PrintInOrder();

            Console.WriteLine("\n8. Очистка:");
            treeMap.Clear();
            Console.WriteLine($"IsEmpty после Clear: {treeMap.IsEmpty()}");
            Console.WriteLine($"Размер: {treeMap.Size()}");

            Console.WriteLine("\n9. Тест с пользовательским компаратором:");
            TreeMapComparator<string> reverseComparator = (x, y) => y.CompareTo(x);
            var reverseTree = new MyTreeMap<string, int>(reverseComparator);

            reverseTree.Put("apple", 1);
            reverseTree.Put("banana", 2);
            reverseTree.Put("cherry", 3);
            reverseTree.Put("date", 4);

            Console.Write("Дерево с обратным порядком строк: ");
            reverseTree.PrintInOrder();

            Console.WriteLine("\n=== Тестирование завершено ===");
        }
    }
}
