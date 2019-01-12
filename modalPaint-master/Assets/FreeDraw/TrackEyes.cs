using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;

public class TrackEyes : MonoBehaviour {

    public GameObject hitPoint;

    // to change the color of the brush when it gets close to a new color
    public SpriteRenderer brushSprite;
    public RaycastHit hitZone;

    // to draw the lines
    public LineRenderer line;
    public List<Vector3> vectorDrawnLinesList;
    public float lineWidth = .1f;
    public int lineNumber = 0;
    public int lineOrder = 10;
    public float defaultBrushSize = 0.8f;
    public List<LineRenderer> lines = new List<LineRenderer>();
    public Camera camera;

    // colors and renderers
    private Color color = Color.black;
    private TrailRenderer meshRenderer;
    private bool isDrawing = false;

    // audio options
    UnityEngine.AudioSource[] songs;
    public int hitIntroSongCount = 0;

    public void setColor(Color color)
    {
        this.color = color;
        CreateLine();
    }

    public void setLineWidth(float lineWidth)
    {
        this.lineWidth = lineWidth;
        CreateLine();
    }

    public void setIsDrawing(bool isDrawing)
    {
        this.isDrawing = isDrawing;
    }
  
    void Start()
    {
        meshRenderer = hitPoint.GetComponent<TrailRenderer>();
        meshRenderer.enabled = false;
        CreateLine();
    }

    Vector3 avg = new Vector3(0,0,0);
    int counter = 0;

	void Update () {
        //meshRenderer.enabled = true;
        GazePoint gazePoint = TobiiAPI.GetGazePoint();
        
        // Use IsValid property instead to process old but valid data
        if (gazePoint.IsRecent())
        {
            
            Vector3 gazeOnScreen = new Vector3(Mathf.RoundToInt(gazePoint.Screen.x), Mathf.RoundToInt(gazePoint.Screen.y));
            counter++;
            avg = (gazeOnScreen + avg) / 2;

            if (counter == 7)
            {
                gazeOnScreen = (avg - new Vector3(Screen.width, Screen.height) / 2f) / 40f;
                brushSprite.transform.position = gazeOnScreen;
                avg = new Vector3(0, 0, 0);
                counter = 0;

                /*if (Physics.Raycast(gazeOnScreen, Vector3.forward, out hitZone))
                {
                    if (!gazePoint.IsValid)
                    {
                        hitPoint.transform.position = hitZone.point;
                    }
                    CreateLine();
                    ReconstructBrush(hitZone);
                }*/

                if (isDrawing)
                {
                    meshRenderer.enabled = true;
                    vectorDrawnLinesList.Add(gazeOnScreen);
                    ReconstructLine(this.line);
                }
            }
        }
    }

    public void CreateLine()
    {
        this.lines.Add(new GameObject().AddComponent<LineRenderer>());
        this.line = this.lines[this.lines.Count - 1];
        this.line.sortingOrder = lineOrder;
        this.line.material.color = this.color;
        this.lineNumber = 0;
        this.lineOrder++;
        this.vectorDrawnLinesList = new List<Vector3>();
        brushSprite.transform.localScale = new Vector3(defaultBrushSize + this.lineWidth, defaultBrushSize + this.lineWidth, defaultBrushSize + this.lineWidth);
    }

    void ReconstructLine(LineRenderer line)
    {
        // set the line width
        line.startWidth = this.lineWidth;
        line.endWidth = this.lineWidth;

        line.positionCount = vectorDrawnLinesList.Count;

        int i = lineNumber;
        while (i < vectorDrawnLinesList.Count)
        {
            line.SetPosition(i, vectorDrawnLinesList[i]);
            i++;
        }

        lineNumber = vectorDrawnLinesList.Count;
    }

    void ReconstructBrush(RaycastHit hit)
    {
        if (hit.collider.tag.Equals("GreenBucket"))
        {
            setColor(new Color(0, 255, 0));
            brushSprite.color = new Color(0, 255, 0);
        }
        else if (hit.collider.tag.Equals("RedBucket"))
        {
            setColor(new Color(255, 0, 0));
            brushSprite.color = new Color(255, 0, 0);
        }
        else if (hit.collider.tag.Equals("BlueBucket"))
        {
            setColor(new Color(0, 0, 238));
            brushSprite.color = new Color(0, 0, 238);
        }
        else if (hit.collider.tag.Equals("BlackBucket"))
        {
            setColor(new Color(0, 0, 0));
            brushSprite.color = new Color(0, 0, 0);
        }
        else if (hit.collider.tag.Equals("WhiteBucket"))
        {
            setColor(new Color(190, 186, 186));
        }
        else if (hit.collider.tag.Equals("SmallBrush"))
        {
            setLineWidth(0.1f);
        }
        else if (hit.collider.tag.Equals("MediumBrush"))
        {
            setLineWidth(0.5f);
        }
        else if (hit.collider.tag.Equals("LargeBrush"))
        {
            setLineWidth(0.9f);
        }
        // music options
        else if (hit.collider.tag.Equals("IntroSong"))
        {
            hitIntroSongCount++;
            songs = GetComponentsInChildren<UnityEngine.AudioSource>();
            // play on odds and stop on event counts
            // if (hitIntroSongCount % 2 == 1) {
            songs[1].Stop();
            songs[0].Play();

            //}
            //else {
            //  songs[0].Stop();
            //}   
        }
        else if (hit.collider.tag.Equals("SecondSong"))
        {
            songs = GetComponentsInChildren<UnityEngine.AudioSource>();
            songs[0].Stop();
            songs[1].Play();
        }

        if (this.meshRenderer)
        {
            this.meshRenderer.material.color = this.color;
        }
    }
}
