﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class test : MonoBehaviour {

	[SerializeField]
	TextAsset m_wordsFile;

	string[] m_wordArray;

	Dictionary<string, List<string>> m_segmentDictionary;

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
	}

	public void BuildSegmentDictionary()
	{
		// go through all 3-letter sequences (AAA, AAB, AAC, etc.) and make a list of all words that contain that sequence.
		m_segmentDictionary = new Dictionary<string, List<string>>();

		for(char a='a'; a<='z'; a++)
		{
			for(char b='a'; b<='z'; b++)
			{
				for(char c='a'; c<='z'; c++)
				{
					string segment = a.ToString() + b.ToString() + c.ToString();
					List<string> words = GetListOfWordsContainingSegment(segment);
					if (words.Count > 0)
					{
						m_segmentDictionary.Add(segment,words);
					}

					Debug.Log(segment+" : "+words.Count);
				}
			}
		}

		Debug.Log("Count : "+m_segmentDictionary.Count);
	}

	List<string> GetListOfWordsContainingSegment(string segment)
	{
		List<string> outList = new List<string>();
		foreach(string word in m_wordArray)
		{
			if (word.Contains(segment))
			{
				// add to list
				outList.Add(word);
			}
		}
		return outList;
	}
}
