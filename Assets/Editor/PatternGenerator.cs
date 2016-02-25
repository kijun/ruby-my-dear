using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public static class PatternGenerator {

    const float cameraWidth = 5.63f;
    const float cameraHeight = 10f;
    const float gapBetweenPatterns = cameraWidth*2;
    const string patternDirPath = "Assets/Patterns";
    const string patternBackgroundPath = "Assets/Prefabs/PatternBackground.prefab";
    const string playerPrefabPath = "Assets/Prefabs/Player.prefab";

	// Add menu named "My Window" to the Window menu
	[MenuItem ("GameObject/Create New Pattern %&n")]
	public static void CreatePattern () {
        // count the number of patterns in folder
        var dir = new DirectoryInfo(patternDirPath);
        FileInfo[] info = dir.GetFiles("*.prefab");
        foreach (FileInfo f in info) {
            Debug.Log(f);
        }

        int patNum = info.Length + 1;
        var go = new GameObject("Pattern"+ patNum);
        go.transform.position = new Vector3(CenterX(patNum), 0, 0);

        var background = InstantiateFromPath(patternBackgroundPath);
        Debug.Log(AssetDatabase.LoadAssetAtPath<GameObject>(patternBackgroundPath));
        Debug.Log(background);
        background.transform.localScale = new Vector2(cameraWidth, cameraHeight*2);
        background.transform.SetParent(go.transform);
        background.transform.localPosition = new Vector3(0, cameraHeight, 100); // clip

        /*
        var bounds = new Bounds(
                       new Vector2(cameraWidth/2, cameraHeight),
                       new Vector2(cameraWidth, cameraHeight*2));
        border.bounds = bounds;
        */

        /*
		// Get existing open window or if none, make a new one:
		DialogueSystemWindow window = (DialogueSystemWindow)EditorWindow.GetWindow(
                typeof (DialogueSystemWindow));
        window.Initialize();
		window.Show();
        */
	}

    [MenuItem ("Assets/Play Pattern %&p")]
    public static void PlayPattern() {
        EditorApplication.isPlaying = true;
        var pattern = Selection.activeGameObject;
        var startPos = pattern.transform.position;
        Camera.main.transform.position = new Vector3(startPos.x, cameraHeight/2, -10);
        var player = GameObject.FindWithTag("Player");
        player.transform.position = startPos.IncrY(0.5f);
    }

    public static float CenterX(int patNum) {
        return (cameraWidth + gapBetweenPatterns) * patNum;
    }

    public static GameObject InstantiateFromPath(string path) {
        return (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(path));
    }
}

public class PatternController : MonoBehaviour {

    void Start() {
    }
}
