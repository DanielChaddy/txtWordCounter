using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace txtWordCounter
{
    public class WordCounter
    {
        public string rootPath = @"C:\Users\DELL\Desktop\lexicon-data";
        public List<string> wordList = new List<string>();
        private Node rootNode = new Node(null, "n");
        private List<string> SpanishDictionary;
        private Dictionary<char, Node> SubDictionaries = new Dictionary<char, Node>();

        List<KeyValuePair<string, int>> DirectoryNamesAndNumbers = new List<KeyValuePair<string, int>>();

        private class Node
        {
            public Node Parent { get; set; }
            public Node Left { get; set; }
            public Node Right { get; set; }
            public string Entry { get; set; }

            public Node(Node parent, string entry)
            {
                Parent = parent;
                Entry = entry;
            }
        }

        public void RecursiveRun(string[] laVaina)
        {
            foreach (string path in laVaina)
            {
                if (File.Exists(path))
                {
                    // This path is a file
                    if (File.Exists("*.txt"))
                    {
                        ProcessFile(path);
                    }
                }
                else if (Directory.Exists(path))
                {
                    // This path is a directory
                    ProcessDirectory(path);
                }
                else
                {
                    Console.WriteLine("{0} is not a valid file or directory.", path);
                }

                string name = Path.GetFileName(path);
                DirectoryNamesAndNumbers.Add(new KeyValuePair<string, int>(name, wordList.Count()));
                

                wordList.Clear();
            }
        }

        public void ProcessDirectory(string targetDirectory)
        {
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName);
            
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory);
        }
        
        public void ProcessFile(string path)
        {
            string rawText = File.ReadAllText(path);
            string[] rawList = rawText.Split(' ');

            foreach (string word in rawList)
            {
                //Used for Regular Check
                if (WordIsValid(word))
                {
                    if (wordList.Count == 0)
                    {
                        wordList.Add(word);
                    }
                    else
                    {
                        if (!wordList.Contains(word))
                        {
                            wordList.Add(word);
                        }
                    }
                }

                //Used for RecursiveDictionary or RecursiveSubDictionaries
                /*if (IsValid(word))
                {
                    if (wordList.Count == 0)
                    {
                        wordList.Add(word);
                    }
                    else
                    {
                        if (!wordList.Contains(word))
                        {
                            wordList.Add(word);
                        }
                    }
                }*/
            }
        }

        private void LoadDictionary()
        {
            string dictionaryPath = @"C:\Users\DELL\Desktop\español.txt";
            List<string> rawDictionary = File.ReadAllLines(dictionaryPath, Encoding.GetEncoding("iso-8859-1")).ToList();
            //BuildTree(rawDictionary); Used for RecursiveDictionary
            SpanishDictionary = rawDictionary;
            //CreateSubDictionaries(rawDictionary);
        }

        private bool WordIsValid(string word)
        {
            foreach (var entry in SpanishDictionary)
            {
                if (word == entry)
                {
                    return true;
                }
            }
            return false;
        }

        #region RecursiveDictionary (Not used because of stack overflow exception)
        private bool IsValid(string word)
        {
            Node rootNode;
            foreach (var subDictionary in SubDictionaries)
            {
                if (subDictionary.Key == word[0])
                {
                    rootNode = subDictionary.Value;
                    return SearchDictionary(rootNode, word);
                }
            }
            return false;
        }
        private bool SearchDictionary(Node currentNode,string word)
        {
            if (String.Compare(word, currentNode.Entry) < 0)
            {
                if (currentNode.Left == null)
                {
                    return false;
                }
                else
                {
                    SearchDictionary(currentNode.Left, word);
                }
            }
            else if (String.Compare(word, currentNode.Entry) > 0)
            {
                if (currentNode.Right == null)
                {
                    return false;
                }
                else
                {
                    SearchDictionary(currentNode.Right, word);
                }
            }
            return true;
        }
        /*private void BuildTree(List<string> rawDictionary)
        {
            var currentNode = rootNode;
            foreach (var entry in rawDictionary)
            {
                SortEntry(currentNode, entry);
            }
        }*/
        private void SortEntry(Node currentNode, string entry)
        {
            if (String.Compare(entry, currentNode.Entry) < 0)
            {
                if (currentNode.Left == null)
                {
                    currentNode.Left = new Node(currentNode, entry);
                }
                else
                {
                    SortEntry(currentNode.Left, entry);
                }
            }
            else if(String.Compare(entry, currentNode.Entry) > 0)
            {
                if (currentNode.Right == null)
                {
                    currentNode.Right = new Node(currentNode, entry);
                }
                else
                {
                    SortEntry(currentNode.Right, entry);
                }
            }
        }
        private void CreateSubDictionaries(List<string> rawDictionary)
        {
            char[] alphabet = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
                'n', 'ñ', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

            foreach (var letter in alphabet)
            {
                SubDictionaries.Add(letter, new Node(null, letter + "nnnn"));
            }

            SortIntoSubDictionaries(rawDictionary);
        }
        private void SortIntoSubDictionaries(List<string> rawDictionary)
        {
            foreach (var word in rawDictionary)
            {
                foreach(var subDictionary in SubDictionaries)
                {
                    if(word[0] == subDictionary.Key)
                    {
                        SortEntry(subDictionary.Value, word);
                    }
                }
            }
        }
        #endregion

        static void Main(string[] args)
        {
            WordCounter wordCounter = new WordCounter();

            wordCounter.LoadDictionary();
            Console.WriteLine("Procesando archivos...");
            wordCounter.RecursiveRun(Directory.GetDirectories(wordCounter.rootPath));
            wordCounter.DirectoryNamesAndNumbers.Sort((x, y) => y.Value.CompareTo(x.Value));

            Console.Clear();
            Console.WriteLine("Resultados: ");
            Console.WriteLine("");

            foreach (var pair in wordCounter.DirectoryNamesAndNumbers)
            {
                Console.WriteLine("La carpeta '{0}' tiene {1} palabras distintas", pair.Key, pair.Value);
            }
           
            Console.ReadKey();
        }
    }
}
