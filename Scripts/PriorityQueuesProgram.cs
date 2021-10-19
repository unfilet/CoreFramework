using System;
using System.Linq;
using System.Collections.Generic;

// Demonstrate a Priority Queue implemented with a Binary Heap

namespace PriorityQueues
{
    // ===================================================================

    public class PriorityQueue<T,U>  where U : IComparable<U>
    {
        public struct Item : IComparable<Item>
        {
            public Item (T item, U priority)
            {
                this.value = item;
                this.priority = priority;
            }

            public T value;
            public U priority;

            public int CompareTo(Item b) {
                return -this.priority.CompareTo (b.priority);
            }
        }

        private List<Item> data;

        public PriorityQueue ()
        {
            this.data = new List<Item> ();
        }

        public void Enqueue (T item, U priority)
        {
            data.Add (new Item(item,priority));
            data.Sort((x, y) => x.CompareTo(y));
        }

        public T Dequeue ()
        {
            var frontItem = data.Last ();
            data.Remove (frontItem);
            return frontItem.value;
        }

        public T Peek ()
        {
            var frontItem = data.Last();
            return frontItem.value;
        }

        public int Count ()
        {
            return data.Count;
        }

        public override string ToString ()
        {
            string s = "";
            for (int i = 0; i < data.Count; ++i)
                s += data [i].ToString () + " ";
            s += "count = " + data.Count;
            return s;
        }

        public bool IsConsistent ()
        {
            // is the heap property true for all data?
            if (data.Count == 0)
                return true;
            int li = data.Count - 1; // last index
            for (int pi = 0; pi < data.Count; ++pi) { // each parent index
                int lci = 2 * pi + 1; // left child index
                int rci = 2 * pi + 2; // right child index

                if (lci <= li && data [pi].priority.CompareTo (data [lci].priority) > 0)
                    return false; // if lc exists and it's greater than parent then bad.
                if (rci <= li && data [pi].priority.CompareTo (data [rci].priority) > 0)
                    return false; // check the right child too.
            }
            return true; // passed all checks
        }
        // IsConsistent
    }
    // PriorityQueue

}
 // ns
