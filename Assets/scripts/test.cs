using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

public class test : MonoBehaviour {

	[SerializeField]
	TextAsset m_wordsFile;

	string[] m_wordArray;
	BestPuzzleFinder m_puzzleFinder;

	Dictionary<string, SegmentData> m_segmentDictionary;

	const int NUMBER_OF_REQUIRED_WORDS = 3;
	const int MINIMUM_LENGTH_OF_REQUIRED_WORDS = 7;
	const int MINIMUM_SCORE_REQUIRED = 2000;

	// Use this for initialization
	void Start () 
	{
		m_puzzleFinder = new BestPuzzleFinder();

		m_wordArray = RemoveSpecialCharacters(m_wordsFile.text).Split(' ');

//		foreach(string s in m_dictionary)
//		{
//			Debug.Log("word : "+s);
//		}

//		for (int i=1; i<20; i++)
//		{
//			string[] slist = AllNLetterWords(i);
//			Debug.Log("words with "+i+" letters : "+slist.Length);
//		}
//
//		string[] slist2 = AllNLetterWords(14);
//
//		foreach(string s in slist2)
//		{
//			Debug.Log("words with "+15+" letters : "+s);
//		}
//

		BuildSegmentDictionary();

		List<Puzzle> puzzles = FindBestPuzzles();
		Debug.Log("Best Puzzles : ");
		for (int i = 0; (i < 1000) && (i < puzzles.Count); i++)
		{
			Debug.Log (">"+(i+1)+" : " + puzzles [i].SegmentData ().Segment () + " " + puzzles [i].m_score + " : " + puzzles [i].Word (0) + ", " + puzzles [i].Word (1) + ", " + puzzles [i].Word (2));
		}
		Debug.Log("total puzzles : "+puzzles.Count);
		Debug.Log("total combos : "+BestPuzzleFinder.s_totalCount);
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

	class Puzzle
	{
		SegmentData m_segmentData;
		List<string> m_words;
		public int m_score;

		public Puzzle(SegmentData segment)
		{
			m_segmentData = segment;
			m_words = new List<string>();
		}

		public void AddWord(string word)
		{
			m_words.Add(word);
		}

		public string Word(int wordNum)
		{
			return m_words[wordNum];
		}

		public SegmentData SegmentData()
		{
			return m_segmentData;
		}
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

		Debug.Log("Segment dictionary count : "+m_segmentDictionary.Count);
	}


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

	List<Puzzle> FindBestPuzzles()
	{
		// goes through all segments and makes an ordered list of the 'best' puzzles 

		//Puzzle bestPuzzle = null;
		//SegmentData bestSeg = null;
		//int bestScore = int.MaxValue;
		List<Puzzle> masterPuzzleList;

		masterPuzzleList = new List<Puzzle> ();

		foreach(KeyValuePair<string, SegmentData> item in m_segmentDictionary)
		{
			SegmentData segData = item.Value;
			if (segData.WordsCount() >= NUMBER_OF_REQUIRED_WORDS)
			{
				// find the best puzzle for this particular segment.
				List<Puzzle> puzzles = m_puzzleFinder.FindBestPuzzleForSegment(segData);

				MergeListsOfPuzzles (puzzles, masterPuzzleList);

//				// if it is the best puzzle overall then record this.
//				if (puzzle != null)
//				{
//					int score = puzzle.m_score;
//					if (score < bestScore)
//					{
//						bestScore = score;
//						bestSeg = segData;
//						bestPuzzle = puzzle;
//					}
//				}
			}
		}

		// we have the best puzzles in the entire english language!
		return masterPuzzleList;
	}

	void MergeListsOfPuzzles(List<Puzzle> source, List<Puzzle> dest)
	{
		foreach (Puzzle p in source)
		{
			int i=0;
			for(i=0; i<dest.Count; i++)
			{
				if (dest[i].m_score > p.m_score)
				{
					break;
				}
			}
			dest.Insert(i, p);
		}
	}








	class BestPuzzleFinder
	{
		// class to find the 'best' puzzle of 3 words that contain a particular segment.
		// e.g. a segment such as 'hou' is contained in many english words. I might want to find the most commonly used 3 words that that segment is used in.

		//int m_bestScore = int.MaxValue;
		SegmentData m_segmentData;
		//Puzzle m_bestPuzzle;
		List<Puzzle> m_qualifyingPuzzleList;
		public static int s_totalCount = 0;

		void Combinations(int[] arr, int len, int startPosition, int[] result)
		{
			// recursive function to go through all the combinations of words
			if (len == 0)
			{
				FoundCombination(result);
				return;
			}       
			for (int i = startPosition; i <= arr.Count()-len; i++)
			{
				result[result.Count() - len] = arr[i];
				Combinations(arr, len-1, i+1, result);
			}
		}       

		public List<Puzzle> FindBestPuzzleForSegment(SegmentData segData)
		{
			m_qualifyingPuzzleList = new List<Puzzle>();
			m_segmentData = segData;
			//m_bestScore = int.MaxValue;
			int wordCount = segData.WordsCount();

			List<int> wordIndices = new List<int>();
			int[] allIndices;
			for(int i=0; i<wordCount; i++)
			{
				if (segData.Word(i).Length >= MINIMUM_LENGTH_OF_REQUIRED_WORDS)
				{
					wordIndices.Add(i);
				}
			}
			allIndices = wordIndices.ToArray();

			if (allIndices.Count () >= NUMBER_OF_REQUIRED_WORDS)
			{
				// recursive search
				Combinations (allIndices, NUMBER_OF_REQUIRED_WORDS, 0, new int[NUMBER_OF_REQUIRED_WORDS]);
			}

			// when we reach here, all the combinations have been explored, and the best puzzle will have been found.
			return m_qualifyingPuzzleList;

//			if (m_qualifyingPuzzleList.Count > 0)
//				return m_qualifyingPuzzleList [0];
//			else
//				return null;
		}

		void FoundCombination(int[] result)
		{
			// this will work out the score for a combination of words, and then decide whether it is the new best puzzle.

			int totalScore = 0;

			foreach(int i in result)
			{
				//Debug.Log ("word "+i+" = "+m_segmentData.Word(i));
				totalScore += m_segmentData.WordRank(i);
			}

			if (totalScore < MINIMUM_SCORE_REQUIRED)
			{
				Puzzle puzzle = new Puzzle(m_segmentData);
				foreach(int j in result)
				{
					puzzle.AddWord(m_segmentData.Word(j));
				}
				puzzle.m_score = totalScore;

				int i=0;
				for(i=0; i<m_qualifyingPuzzleList.Count; i++)
				{
					if (m_qualifyingPuzzleList[i].m_score > totalScore)
					{
						break;
					}
				}
				m_qualifyingPuzzleList.Insert(i, puzzle);
			}

//			if (totalScore < m_bestScore)
//			{
//				m_bestScore = totalScore;
//				m_bestPuzzle = new Puzzle(m_segmentData);
//				foreach(int j in result)
//				{
//					m_bestPuzzle.AddWord(m_segmentData.Word(j));
//				}
//				m_bestPuzzle.m_score = totalScore;
//			}

			s_totalCount++;
		}

	}
}
