using System.Collections;
using System.Collections.Generic;

namespace CetchUp.Enumerators
{
    public class CetchModifierRepositoryItemEnumerator : IEnumerator<KeyValuePair<string, CetchModifier>>
    {
        public CetchModifierRepository collection;
        private Queue<KeyValuePair<string, CetchModifier>> queue;

        public KeyValuePair<string, CetchModifier> Current => new KeyValuePair<string, CetchModifier>();

        public CetchModifierRepositoryItemEnumerator(CetchModifierRepository collection)
        {
            this.collection = collection;
            Reset();
        }

        object IEnumerator.Current => queue.Peek();

        public void Dispose()
        {
            queue.Clear();
        }

        public bool MoveNext()
        {
            if (queue.Count == 0) { return false; }
            queue.Dequeue();
            return true;
        }

        public void Reset()
        {
            queue = new Queue<KeyValuePair<string, CetchModifier>>();
            foreach (string key in collection.Keys)
            {
                queue.Enqueue(new KeyValuePair<string, CetchModifier>(key, collection[key]));
            }
        }
    }
}