using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterRing : MonoBehaviour 
{
	[SerializeField]
	GameObject m_letterPrefab;

	[SerializeField]
	float m_radiusOfRing;

	[SerializeField]
	char[] m_lettersToBeUsed;

	// Use this for initialization
	void Start () 
	{
		SetUpRing();
	}

	public void SetUpRing()
	{
		DeleteExistingLetters();

		int numLetters = m_lettersToBeUsed.Length;
		for(int i=0; i<numLetters; i++)
		{
			float angle = (Mathf.PI*2*i)/numLetters;
			Vector2 pos = new Vector2(m_radiusOfRing * Mathf.Sin(angle), m_radiusOfRing * Mathf.Cos(angle));
			GameObject letter = Instantiate(m_letterPrefab,transform);
			letter.transform.localPosition = pos;
			letter.GetComponent<Letter>().SetLetter(m_lettersToBeUsed[i]);
		}
	}

	void DeleteExistingLetters()
	{
		Letter[] letters = transform.GetComponentsInChildren<Letter>();
		foreach(Letter b in letters)
		{
			b.transform.SetParent(null);
			Destroy(b.gameObject);
		}
	}

	
	// Update is called once per frame
	void Update () {
		
	}
}
