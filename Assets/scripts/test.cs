using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class test : MonoBehaviour {

	[SerializeField]
	TextAsset m_wordsFile;

	string[] m_wordArray;

	Dictionary<string, SegmentData> m_segmentDictionary;

	// Use this for initialization
	void Start () 
	{
		m_wordArray = RemoveSpecialCharacters(m_wordsFile.text).Split(' ');
//		foreach(string s in m_dictionary)
//		{
//			Debug.Log("word : "+s);
//		}

		for (int i=1; i<20; i++)
		{
			string[] slist = AllNLetterWords(i);
			Debug.Log("words with "+i+" letters : "+slist.Length);
		}

		string[] slist2 = AllNLetterWords(14);

		foreach(string s in slist2)
		{
			Debug.Log("words with "+15+" letters : "+s);
		}

		BuildSegmentDictionary();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public string RemoveSpecialCharacters(string str) 
	{
		StringBuilder sb = new StringBuilder();
		foreach (char c in str) {
			if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '\'') 
			{
				sb.Append(c);
			}
			if (char.IsWhiteSpace(c))
			{
				sb.Append(' ');
			}
		}
		return sb.ToString();
	}

	public string[] AllNLetterWords(int numLetters)
	{
		List<string> outList = new List<string>();
		foreach(string word in m_wordArray)
		{
			if (word.Length == numLetters)
			{
				outList.Add(word);
			}
		}
		string[] outArray = outList.ToArray();
		return outArray;	
	}

	class SegmentData
	{
		string m_segment;
		List<string> m_wordsContainingSegment;
		List<int> m_rankOfWords;

		public SegmentData(string segment)
		{
			m_segment = segment;
			m_wordsContainingSegment = new List<string>();
			m_rankOfWords = new List<int>();
		}

		public int WordsCount()
		{
			return m_wordsContainingSegment.Count;
		}

		public void AddWord(string word, int rank)
		{
			m_wordsContainingSegment.Add(word);
			m_rankOfWords.Add(rank);
		}

		public int WordRank(int wordNum)
		{
			return m_rankOfWords[wordNum];
		}

		public string Word(int wordNum)
		{
			return m_wordsContainingSegment[wordNum];
		}

		public string Segment()
		{
			return m_segment;
		}
	}

	public void BuildSegmentDictionary()
	{
		// go through all 3-letter sequences (AAA, AAB, AAC, etc.) and make a list of all words that contain that sequence.
		m_segmentDictionary = new Dictionary<string, SegmentData>();

		for(char a='a'; a<='z'; a++)
		{
			for(char b='a'; b<='z'; b++)
			{
				for(char c='a'; c<='z'; c++)
				{
					string segment = a.ToString() + b.ToString() + c.ToString();
					SegmentData segData = BuildSegmentData(segment);
					if (segData.WordsCount() > 0)
					{
						m_segmentDictionary.Add(segment,segData);
					}

					Debug.Log(segment+" : "+segData.WordsCount());
				}
			}
		}

		Debug.Log("Count : "+m_segmentDictionary.Count);

		SegmentData bestSeg = FindBestSegment();
		Debug.Log("BestSeg : "+bestSeg.Segment()+" : "+bestSeg.Word(0)+", "+bestSeg.Word(1)+", "+bestSeg.Word(2));
	}

//	List<string> GetListOfWordsContainingSegment(string segment)
//	{
//		List<string> outList = new List<string>();
//		foreach(string word in m_wordArray)
//		{
//			if (word.Contains(segment))
//			{
//				// add to list
//				outList.Add(word);
//			}
//		}
//		return outList;
//	}

	SegmentData BuildSegmentData(string segment)
	{
		SegmentData segData = new SegmentData(segment);
		for(int i=0; i<m_wordArray.Length; i++)
		{
			string word = m_wordArray[i];
			if (word.Contains(segment))
			{
				// add to list
				segData.AddWord(word,i);
			}
		}

		return segData;
	}

	SegmentData FindBestSegment()
	{
		const int NUMBER_OF_REQUIRED_WORDS = 3;
		const int MINIMUM_LENGTH_OF_REQUIRED_WORDS = 6;

		SegmentData bestSeg = null;
		int bestScore = int.MaxValue;

		foreach(KeyValuePair<string, SegmentData> item in m_segmentDictionary)
		{
			SegmentData segData = item.Value;
			if (segData.WordsCount() >= NUMBER_OF_REQUIRED_WORDS)
			{
				int score = segData.WordRank(0) + segData.WordRank(1) + segData.WordRank(2);
				if (score < bestScore)
				{
					bestScore = score;
					bestSeg = segData;
				}
			}
		}

		return bestSeg;
	}
}
