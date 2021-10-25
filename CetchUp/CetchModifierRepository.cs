using System.Collections;
using System.Collections.Generic;
using CetchUp.Enumerators;
using System.IO;
using System.Text.RegularExpressions;

namespace CetchUp
{
    public class CetchModifierRepository : IDictionary<string, CetchModifier>
    {
        private Dictionary<string, CetchModifier> innerCollection = new Dictionary<string, CetchModifier>();
        public CetchModifier this[string key] { get => innerCollection[key]; set => innerCollection[key] = value; }
        public ICollection<string> Keys => innerCollection.Keys;
        public ICollection<CetchModifier> Values => innerCollection.Values;
        public int Count => innerCollection.Count;
        public bool IsReadOnly => false;

        public void AddDirectory(string path)
        {
            Regex regex = new Regex(@"^.*\.cetch$");
            SearchFolderAndAddToCollection(path, regex);
        }

        private void SearchFolderAndAddToCollection(string path, Regex regex)
        {
            foreach (string fileName in Directory.GetFiles(path))
            {
                if (regex.IsMatch(fileName))
                {
                    string correctedName = Regex.Replace(Regex.Replace(fileName, ".cetch$", ""), @"^.*\/", "");
                    Add(correctedName, new CetchModifier(fileName));
                }
            }
            foreach (string fileName in Directory.GetDirectories(path))
            {
                SearchFolderAndAddToCollection(path + fileName, regex);
            }
        }

        public void Add(string key, CetchModifier value) => innerCollection.Add(key, value);

        public void Add(KeyValuePair<string, CetchModifier> item) => innerCollection.Add(item.Key, item.Value);

        public void Clear() => innerCollection.Clear();

        public bool Contains(KeyValuePair<string, CetchModifier> item)
        {
            if (!innerCollection.ContainsKey(item.Key)) { return false; }
            return innerCollection[item.Key] == item.Value;
        }

        public bool ContainsKey(string key)
        {
            return innerCollection.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, CetchModifier>[] array, int arrayIndex)
        {
            int i = arrayIndex;
            foreach (KeyValuePair<string, CetchModifier> item in array)
            {
                array[i + arrayIndex] = item;
                i++;
            }
        }

        public IEnumerator<KeyValuePair<string, CetchModifier>> GetEnumerator() => new CetchModifierRepositoryItemEnumerator(this);

        public bool Remove(string key) => innerCollection.Remove(key);

        public bool Remove(KeyValuePair<string, CetchModifier> item) => innerCollection.Remove(item.Key);

        public bool TryGetValue(string key, out CetchModifier value)
        {
            if (!innerCollection.ContainsKey(key))
            {
                value = null;
                return false;
            }
            value = innerCollection[key];
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator() => new CetchModifierRepositoryValueEnumerator(this);
    }
}