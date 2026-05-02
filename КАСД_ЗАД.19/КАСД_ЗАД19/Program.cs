using System;
using System.Collections;
using System.Collections.Generic;

namespace MyTreeSetSolution
{
    // Делегат для компаратора
    public delegate int TreeMapComparator<T>(T x, T y);

    // Упрощенное красно-черное дерево
    public class RedBlackTree<T> where T : IComparable<T>
    {
        private enum NodeColor { Red, Black }

        private class RBNode
        {
            public T Key;
            public object Value;
            public RBNode Left;
            public RBNode Right;
            public RBNode Parent;
            public NodeColor Color;

            public RBNode(T key, object value)
            {
                Key = key;
                Value = value;
                Left = null;
                Right = null;
                Parent = null;
                Color = NodeColor.Red;
            }

            public bool IsLeftChild() => Parent != null && Parent.Left == this;
            public RBNode Sibling() => Parent == null ? null : (IsLeftChild() ? Parent.Right : Parent.Left);
        }

        private TreeMapComparator<T> comparator;
        private RBNode root;
        private int size;

        public RedBlackTree(TreeMapComparator<T> comp = null)
        {
            comparator = comp ?? ((x, y) => x.CompareTo(y));
            root = null;
            size = 0;
        }

        public int Size => size;
        public bool IsEmpty => size == 0;

        // Основные операции
        public bool ContainsKey(T key) => GetNode(key) != null;

        public void Put(T key, object value)
        {
            root = Insert(root, key, value);
            root.Color = NodeColor.Black;
            size++;
        }

        public bool Remove(T key)
        {
            if (!ContainsKey(key)) return false;

            // Упрощенное удаление - просто находим и удаляем (в реальности нужно балансировать)
            root = Delete(root, key);
            size--;
            return true;
        }

        public void Clear()
        {
            root = null;
            size = 0;
        }

        // Навигационные методы
        public T FirstKey()
        {
            if (root == null) throw new InvalidOperationException("Tree is empty");
            return GetMinNode(root).Key;
        }

        public T LastKey()
        {
            if (root == null) throw new InvalidOperationException("Tree is empty");
            return GetMaxNode(root).Key;
        }

        public T CeilingKey(T key)
        {
            var node = CeilingNode(root, key);
            return node != null ? node.Key : default(T);
        }

        public T FloorKey(T key)
        {
            var node = FloorNode(root, key);
            return node != null ? node.Key : default(T);
        }

        // Вспомогательные методы
        private RBNode GetNode(T key)
        {
            var current = root;
            while (current != null)
            {
                int cmp = comparator(key, current.Key);
                if (cmp == 0) return current;
                current = cmp < 0 ? current.Left : current.Right;
            }
            return null;
        }

        private RBNode Insert(RBNode node, T key, object value)
        {
            if (node == null) return new RBNode(key, value);

            int cmp = comparator(key, node.Key);
            if (cmp < 0)
                node.Left = Insert(node.Left, key, value);
            else if (cmp > 0)
                node.Right = Insert(node.Right, key, value);
            else
                node.Value = value;

            return Balance(node);
        }

        private RBNode Delete(RBNode node, T key)
        {
            if (node == null) return null;

            int cmp = comparator(key, node.Key);
            if (cmp < 0)
                node.Left = Delete(node.Left, key);
            else if (cmp > 0)
                node.Right = Delete(node.Right, key);
            else
            {
                if (node.Left == null) return node.Right;
                if (node.Right == null) return node.Left;

                var temp = node;
                node = GetMinNode(temp.Right);
                node.Right = DeleteMin(temp.Right);
                node.Left = temp.Left;
            }

            return Balance(node);
        }

        private RBNode DeleteMin(RBNode node)
        {
            if (node.Left == null) return node.Right;
            node.Left = DeleteMin(node.Left);
            return Balance(node);
        }

        // Упрощенная балансировка (в реальности полная балансировка КЧД сложнее)
        private RBNode Balance(RBNode node)
        {
            // Простая проверка и балансировка
            if (IsRed(node.Right) && !IsRed(node.Left)) node = RotateLeft(node);
            if (IsRed(node.Left) && IsRed(node.Left.Left)) node = RotateRight(node);
            if (IsRed(node.Left) && IsRed(node.Right)) FlipColors(node);

            return node;
        }

        private bool IsRed(RBNode node) => node != null && node.Color == NodeColor.Red;

        private RBNode RotateLeft(RBNode h)
        {
            var x = h.Right;
            h.Right = x.Left;
            x.Left = h;
            x.Color = h.Color;
            h.Color = NodeColor.Red;
            return x;
        }

        private RBNode RotateRight(RBNode h)
        {
            var x = h.Left;
            h.Left = x.Right;
            x.Right = h;
            x.Color = h.Color;
            h.Color = NodeColor.Red;
            return x;
        }

        private void FlipColors(RBNode h)
        {
            h.Color = NodeColor.Red;
            h.Left.Color = NodeColor.Black;
            h.Right.Color = NodeColor.Black;
        }

        private RBNode GetMinNode(RBNode node)
        {
            while (node.Left != null) node = node.Left;
            return node;
        }

        private RBNode GetMaxNode(RBNode node)
        {
            while (node.Right != null) node = node.Right;
            return node;
        }

        private RBNode CeilingNode(RBNode node, T key)
        {
            if (node == null) return null;

            int cmp = comparator(node.Key, key);
            if (cmp == 0) return node;
            if (cmp < 0) return CeilingNode(node.Right, key);

            var left = CeilingNode(node.Left, key);
            return left ?? node;
        }

        private RBNode FloorNode(RBNode node, T key)
        {
            if (node == null) return null;

            int cmp = comparator(node.Key, key);
            if (cmp == 0) return node;
            if (cmp > 0) return FloorNode(node.Left, key);

            var right = FloorNode(node.Right, key);
            return right ?? node;
        }

        // Получение всех ключей
        public List<T> GetAllKeys()
        {
            var keys = new List<T>();
            InOrderTraversal(root, keys);
            return keys;
        }

        private void InOrderTraversal(RBNode node, List<T> keys)
        {
            if (node == null) return;
            InOrderTraversal(node.Left, keys);
            keys.Add(node.Key);
            InOrderTraversal(node.Right, keys);
        }
    }

    // Упрощенный MyTreeSet
    public class MyTreeSet<T> : IEnumerable<T> where T : IComparable<T>
    {
        private RedBlackTree<T> tree;
        private static readonly object DummyValue = new object();

        // Конструкторы
        public MyTreeSet() => tree = new RedBlackTree<T>();
        public MyTreeSet(TreeMapComparator<T> comparator) => tree = new RedBlackTree<T>(comparator);
        public MyTreeSet(T[] array) : this() { if (array != null) AddAll(array); }

        // Основные методы
        public bool Add(T item)
        {
            if (Contains(item)) return false;
            tree.Put(item, DummyValue);
            return true;
        }

        public bool AddAll(T[] array)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            bool modified = false;
            foreach (var item in array) if (Add(item)) modified = true;
            return modified;
        }

        public void Clear() => tree.Clear();
        public bool Contains(T item) => tree.ContainsKey(item);

        public bool ContainsAll(T[] array)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            foreach (var item in array) if (!Contains(item)) return false;
            return true;
        }

        public bool IsEmpty() => tree.IsEmpty;
        public bool Remove(T item) => tree.Remove(item);

        public bool RemoveAll(T[] array)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            bool modified = false;
            foreach (var item in array) if (Remove(item)) modified = true;
            return modified;
        }

        public int Size() => tree.Size;

        // Преобразование
        public T[] ToArray() => tree.GetAllKeys().ToArray();

        // Граничные элементы
        public T First()
        {
            if (IsEmpty()) throw new InvalidOperationException("Set is empty");
            return tree.FirstKey();
        }

        public T Last()
        {
            if (IsEmpty()) throw new InvalidOperationException("Set is empty");
            return tree.LastKey();
        }

        // Навигационные методы
        public T Ceiling(T obj) => tree.CeilingKey(obj);
        public T Floor(T obj) => tree.FloorKey(obj);

        // Подмножества (упрощенные)
        public MyTreeSet<T> SubSet(T fromElement, T toElement)
        {
            var result = new MyTreeSet<T>();
            foreach (var item in this)
            {
                if (item.CompareTo(fromElement) >= 0 && item.CompareTo(toElement) < 0)
                    result.Add(item);
            }
            return result;
        }

        public MyTreeSet<T> HeadSet(T toElement)
        {
            var result = new MyTreeSet<T>();
            foreach (var item in this)
            {
                if (item.CompareTo(toElement) < 0)
                    result.Add(item);
            }
            return result;
        }

        public MyTreeSet<T> TailSet(T fromElement)
        {
            var result = new MyTreeSet<T>();
            foreach (var item in this)
            {
                if (item.CompareTo(fromElement) >= 0)
                    result.Add(item);
            }
            return result;
        }

        // Poll методы
        public T PollFirst()
        {
            if (IsEmpty()) return default(T);
            T first = First();
            Remove(first);
            return first;
        }

        public T PollLast()
        {
            if (IsEmpty()) return default(T);
            T last = Last();
            Remove(last);
            return last;
        }

        // Итераторы
        public IEnumerator<T> GetEnumerator() => tree.GetAllKeys().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString() => string.Join(", ", this);
    }

    // Пример использования
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Тестирование упрощенного MyTreeSet ===");

            // Создание и добавление
            var set = new MyTreeSet<int>();
            set.AddAll(new[] { 10, 5, 15, 3, 7, 12, 18 });

            Console.WriteLine($"Множество: {set}");
            Console.WriteLine($"Размер: {set.Size()}");
            Console.WriteLine($"Содержит 7: {set.Contains(7)}");
            Console.WriteLine($"Содержит 100: {set.Contains(100)}");

            // Граничные элементы
            Console.WriteLine($"\nПервый: {set.First()}");
            Console.WriteLine($"Последний: {set.Last()}");

            // Навигация
            Console.WriteLine($"\nCeiling(9): {set.Ceiling(9)}");
            Console.WriteLine($"Floor(9): {set.Floor(9)}");

            // Подмножества
            Console.WriteLine($"\nSubSet(5, 15): {set.SubSet(5, 15)}");
            Console.WriteLine($"HeadSet(<10): {set.HeadSet(10)}");
            Console.WriteLine($"TailSet(>=10): {set.TailSet(10)}");

            // Poll
            Console.WriteLine($"\nPollFirst: {set.PollFirst()}");
            Console.WriteLine($"PollLast: {set.PollLast()}");
            Console.WriteLine($"После poll: {set}");

            // Удаление
            set.Remove(10);
            Console.WriteLine($"После удаления 10: {set}");

            // Преобразование
            var array = set.ToArray();
            Console.WriteLine($"Массив: [{string.Join(", ", array)}]");

            // Очистка
            set.Clear();
            Console.WriteLine($"\nПосле Clear - IsEmpty: {set.IsEmpty()}, Size: {set.Size()}");

            // С компаратором
            Console.WriteLine("\n=== С пользовательским компаратором ===");
            TreeMapComparator<string> lengthComp = (x, y) => x.Length.CompareTo(y.Length);
            var stringSet = new MyTreeSet<string>(lengthComp);

            stringSet.AddAll(new[] { "apple", "banana", "cherry", "date" });
            Console.WriteLine($"Строки по длине: {stringSet}");

            Console.WriteLine("\n=== Тестирование завершено ===");
        }
    }
}
