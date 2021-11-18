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
            SearchFolderAndAddToCollection(path);
        }

        private void SearchFolderAndAddToCollection(string path, string folderName = "")
        {
            foreach (string fileName in Directory.GetFiles(path))
            {
                if (Regex.IsMatch(fileName, @".*\.cetch$"))
                {
                    string correctedName = folderName + Regex.Match(fileName, @"([A-Za-z]*)\.cetch$").Groups[1].Value;
                    Add(correctedName, new CetchModifier(fileName));
                }
            }
            foreach (string fileName in Directory.GetDirectories(path))
            {
                SearchFolderAndAddToCollection(fileName, folderName + Regex.Match(fileName, "[A-z]*$") + ".");
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