using System.Collections;
using System.Collections.Generic;

namespace CetchUp.Enumerators
{
    public class CetchModifierRepositoryValueEnumerator : IEnumerator
    {
        private CetchModifierRepository collection;
        private Queue<string> keyQueue;

        public object Current => collection[keyQueue.Peek()];

        public CetchModifierRepositoryValueEnumerator(CetchModifierRepository collection)
        {
            this.collection = collection;
        }

        public bool MoveNext()
        {
            if (keyQueue.Count == 0) { return false; }
            keyQueue.Dequeue();
            return true;
        }

        public void Reset()
        {
            keyQueue = new Queue<string>();
            foreach (string key in collection.Keys)
            {
                keyQueue.Enqueue(key);
            }
        }
    }
}