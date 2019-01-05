using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.SceneManagement;
using System.Linq;

public class SpeechInput : MonoBehaviour {

    KeywordRecognizer keywordRecognizer;
    Dictionary<string, System.Action> keywords;
    bool canSwitch;
    int nextScene;

    public GameObject confirmationText;
	// Use this for initialization
	void Start () {
        nextScene = -1;       

        keywords = new Dictionary<string, System.Action>();

        keywords.Add("eye", () =>
        {
            EyesCalled();
        });

        keywords.Add("gestures", () =>
        {
            GesturesCalled();
        });

        keywords.Add("menu", () =>
        {
            MenuCalled();
        });

        keywords.Add("yes", () =>
        {
            ChangeScene();
            confirmationText.SetActive(false);
        });

        keywords.Add("no", () =>
        {
            nextScene = -1;
            confirmationText.SetActive(false);
        });


        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizerOnPhraseRecognized;
        keywordRecognizer.Start();
	}

    void KeywordRecognizerOnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;

        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void EyesCalled()
    {
        print("You said eyes");
        nextScene = 2;
        confirmationText.SetActive(true);
    }

    void GesturesCalled()
    {
        print("you said gestures");
        nextScene = 1;
        confirmationText.SetActive(true);
    }

    void MenuCalled()
    {
        print("you said you want to go back to the menu");
        nextScene = 0;
        confirmationText.SetActive(true);
    }

    void ChangeScene()
    {
        if (nextScene != -1)
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}
