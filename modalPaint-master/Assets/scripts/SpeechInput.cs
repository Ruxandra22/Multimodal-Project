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
    public SpriteRenderer brushSprite;
    public DetectJoints detectJoints;

    // sizes for the brush
    public float smallBrush = 0.1f;
    public float mediumBrush = 0.5f;
    public float largeBrush = 0.9f;

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

        // Change colors
        keywords.Add("blue", () =>
        {
            brushSprite.color = new Color(0, 0, 238);
            detectJoints.setColor(new Color(0, 0, 238));
        });
        keywords.Add("red", () =>
        {
            brushSprite.color = new Color(255, 0, 0);
            detectJoints.setColor(new Color(255, 0, 0));
        });
        keywords.Add("green", () =>
        {
            brushSprite.color = new Color(0, 255, 0);
            detectJoints.setColor(new Color(0, 255, 0));
        });
        keywords.Add("black", () =>
        {
            brushSprite.color = new Color(0, 0, 0);
            detectJoints.setColor(new Color(0, 0, 0));
        });
        keywords.Add("white", () =>
        {
            brushSprite.color = new Color(190, 186, 186);
            detectJoints.setColor(new Color(190, 186, 186));
        });

        // Change the brush size
        keywords.Add("small brush", () =>
        {
            detectJoints.setLineWidth(smallBrush);
        });
        keywords.Add("medium brush", () =>
        {
            detectJoints.setLineWidth(mediumBrush);
        });
        keywords.Add("large brush", () =>
        {
            detectJoints.setLineWidth(largeBrush);
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
