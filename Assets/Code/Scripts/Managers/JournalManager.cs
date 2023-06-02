using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.IO;

public class JournalManager : MonoBehaviour
{
	private GameManager gameManager;

	public void OpenDrawings()
	{
		string filepath = Application.dataPath + "/MyDrawings";

		if (!Directory.Exists(filepath))
		{
			Directory.CreateDirectory(filepath);
		}
		Process.Start(filepath);
	}

	public void LoadOption(string option)
	{
		GameManager.LoadUI(option, "JournalScene");
	}

	public void ResumeGame()
	{
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		gameManager.UnloadUI("JournalScene");
	}

	public void CloseGame()
	{
		#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
	}
}
