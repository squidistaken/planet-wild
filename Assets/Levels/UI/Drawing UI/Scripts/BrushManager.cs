using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

// Credit: Marcus
public class BrushManager : MonoBehaviour
{
	BrushInitialize activeBrush;

	public GameObject brushPrefab;
	public Camera drawingCamera;
	private GameObject[] clonedBrushes;

	public Camera renderCamera;
	public RenderTexture RTexture;

	private int layerCount;

	private string selectedAnimal;

	private GameManager gameManager;

	void Update()
	{
		var Ray = drawingCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Input.GetMouseButtonDown(0))
		{
			if (Physics.Raycast(Ray, out hit))
			{
				GameObject newLine = Instantiate(brushPrefab, hit.transform);
				activeBrush = newLine.GetComponent<BrushInitialize>();

				layerCount++;
			}	
		}

		// or statement is for if the brushes leaves the plane
		if (Input.GetMouseButtonUp(0) || !Physics.Raycast(Ray, out hit))
		{
			activeBrush = null;
			
		}

		if (activeBrush != null)
		{
			Vector2 mousePos = drawingCamera.ScreenToWorldPoint(Input.mousePosition);
			activeBrush.UpdateLine(mousePos);
			activeBrush.lineRenderer.sortingOrder = layerCount;
		}
	}

	public void ClearLines()
	{
		clonedBrushes = GameObject.FindGameObjectsWithTag("Drawing");

		foreach (GameObject brush in clonedBrushes)
		{
			Destroy(brush);
			layerCount = 0;
		}
	}

	public void Save()
	{
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		selectedAnimal = gameManager.animalName;

		StartCoroutine(SaveDrawing(selectedAnimal));
	}

	private IEnumerator SaveDrawing(string fileName)
	{
		yield return new WaitForEndOfFrame(); // Waiting until end of frame to render

		RenderTexture.active = RTexture; // set Active texture to RTexture

		// convert rendering texture to texture2D
		var screenshotTexture = new Texture2D(RTexture.width, RTexture.height);
		Rect rect = new Rect(0, 0, RTexture.width, RTexture.height);

		screenshotTexture.ReadPixels(rect, 0, 0);
		screenshotTexture.Apply();

		//write data to file
		byte[] byteArray = screenshotTexture.EncodeToPNG();

		string filepath = Application.dataPath + "/MyDrawings";
		fileName = "/" + fileName + "Drawing.png";

		// Creating the folder if it doesn't already exist.
		if (!Directory.Exists(filepath))
		{
			Directory.CreateDirectory(Application.dataPath + "/MyDrawings");
			File.WriteAllBytes(filepath + fileName, byteArray);
			selectedAnimal = null;
		}
		else
		{
			File.WriteAllBytes(filepath + fileName, byteArray);
			selectedAnimal = null;
		}
	}

	public void UnloadDrawingUI()
	{
		// temp code - remove after testing session
		if (SceneManager.GetSceneByName("TutorialScene").isLoaded)
		{
			gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
			selectedAnimal = null;
			gameManager.UnloadScene("TutorialScene");
			gameManager.LoadScene("ForestScene", false);
			gameManager.UnloadDrawing();
		}
		else
		{
			gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
			selectedAnimal = null;
			gameManager.UnloadDrawing();
		}
	}
}