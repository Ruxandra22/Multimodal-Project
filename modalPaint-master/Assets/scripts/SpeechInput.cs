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
    public TrackEyes trackEyes;

    // sizes for the brush
    public float smallBrush = 0.1f;
    public float mediumBrush = 0.5f;
    public float largeBrush = 0.9f;

    public GameObject confirmationText;
    public GameObject loadingImage;
	// Use this for initialization
	void Start () {
        nextScene = -1;       

        keywords = new Dictionary<string, System.Action>();

        keywords.Add("eye tracking", () =>
        {
            EyesCalled();
        });

        keywords.Add("gesture tracking", () =>
        {
            GesturesCalled();
        });

        keywords.Add("menu screen", () =>
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
        keywords.Add("blue brush", () =>
        {
            brushSprite.color = new Color(0, 0, 238);
            if (detectJoints != null)
            {
                detectJoints.setColor(new Color(0, 0, 238));
            }
            if (trackEyes != null)
            {
                trackEyes.setColor(new Color(0, 0, 238));
            }
        });
        keywords.Add("red brush", () =>
        {
            brushSprite.color = new Color(255, 0, 0);
            if (detectJoints != null)
            {
                detectJoints.setColor(new Color(255, 0, 0));
            }
            if (trackEyes != null)
            {
                trackEyes.setColor(new Color(255, 0, 0));
            }
        });
        keywords.Add("green brush", () =>
        {
            brushSprite.color = new Color(0, 255, 0);
            if (detectJoints != null)
            {
                detectJoints.setColor(new Color(0, 255, 0));
            }
            if (trackEyes != null)
            {
                trackEyes.setColor(new Color(0, 255, 0));
            }
        });
        keywords.Add("black brush", () =>
        {
            brushSprite.color = new Color(0, 0, 0);
            if (detectJoints != null)
            {
                detectJoints.setColor(new Color(0, 0, 0));
            }
            if (trackEyes != null)
            {
                trackEyes.setColor(new Color(0, 0, 0));
            }
        });
        keywords.Add("white brush", () =>
        {
            brushSprite.color = new Color(190, 186, 186);
            if (detectJoints != null)
            {
                detectJoints.setColor(new Color(190, 186, 186));
            }
            if (trackEyes != null)
            {
                trackEyes.setColor(new Color(190, 186, 186));
            }
        });

        // Change the brush size
        keywords.Add("small brush", () =>
        {
            if (detectJoints != null)
            {
                detectJoints.setLineWidth(smallBrush);
            }
            if (trackEyes != null)
            {
                trackEyes.setLineWidth(smallBrush);
            }
        });
        keywords.Add("medium brush", () =>
        {
            if (detectJoints != null)
            {
                detectJoints.setLineWidth(mediumBrush);
            }
            if (trackEyes != null)
            {
                trackEyes.setLineWidth(mediumBrush);
            }
        });
        keywords.Add("large brush", () =>
        {
            if (detectJoints != null)
            {
                detectJoints.setLineWidth(largeBrush);
            }
            if (trackEyes != null)
            {
                trackEyes.setLineWidth(largeBrush);
            }
        });

        // stop/start painting
        keywords.Add("start painting", () =>
        {
            if (trackEyes != null)
            {
                trackEyes.setIsDrawing(true);
            }
        });
        keywords.Add("start brushing", () =>
        {
            if (trackEyes != null)
            {
                trackEyes.setIsDrawing(true);
            }
        });
        keywords.Add("start drawing", () =>
        {
            if (trackEyes != null)
            {
                trackEyes.setIsDrawing(true);
            }
        });
        keywords.Add("start", () =>
        {
            if (trackEyes != null)
            {
                trackEyes.setIsDrawing(true);
            }
        });
        keywords.Add("stop painting", () =>
        {
            if (trackEyes != null)
            {
                trackEyes.setIsDrawing(false);
            }
        });
        keywords.Add("stop brushing", () =>
        {
            if (trackEyes != null)
            {
                trackEyes.setIsDrawing(false);
            }
        });
        keywords.Add("stop drawing", () =>
        {
            if (trackEyes != null)
            {
                trackEyes.setIsDrawing(false);
            }
        });
        keywords.Add("stop", () =>
        {
            if (trackEyes != null)
            {
                trackEyes.setIsDrawing(false);
            }
        });
        keywords.Add("oh my god", () =>
        {
            if (trackEyes != null)
            {
                trackEyes.setIsDrawing(false);
            }
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
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            ChangeScene();
            return;
        }
        confirmationText.SetActive(true);
    }

    void GesturesCalled()
    {
        print("you said gestures");
        nextScene = 1;
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            ChangeScene();
            return;
        }
        confirmationText.SetActive(true);
    }

    void MenuCalled()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) return;
        print("you said you want to go back to the menu");
        nextScene = 0;
        confirmationText.SetActive(true);
    }

    void ChangeScene()
    {
        if (SceneManager.GetActiveScene().buildIndex == nextScene) return;
        if (nextScene != -1)
        {
            loadingImage.SetActive(true);
            SceneManager.LoadScene(nextScene);
        }
    }
}
