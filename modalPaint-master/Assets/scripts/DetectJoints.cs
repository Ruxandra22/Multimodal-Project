using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;


public class DetectJoints : MonoBehaviour {

    public GameObject BodySourceManager, hitPoint;

    // to change the color of the brush when it gets close to a new color
    public SpriteRenderer brushSprite;
    public RaycastHit hitZone;

    // Kinect variables - detecting the hand and the gestures
    public JointType TrackedJoint;
    private BodySourceManager bodyManager;
    private Body[] bodies;
    public float multiplier = 20f;      // to make the range larger when the hand is detected 
    private Windows.Kinect.Joint rightHand;
    private bool handDetectedOpen = false;

    // to draw the lines
    public LineRenderer line;
    public List<Vector3> vectorDrawnLinesList;
    public float lineWidth = .1f;
    public int lineNumber = 0;
    public int lineOrder = 10;
    public List<LineRenderer> lines = new List<LineRenderer>();

    // colors and renderers
    private Color color = Color.black;
    private TrailRenderer meshRenderer;

    // audio options
    UnityEngine.AudioSource[] songs;
    public int hitIntroSongCount = 0;

    public void setColor(Color color)
    {
        this.color = color;
    }

    public void setLineWidth(float lineWidth)
    {
        this.lineWidth = lineWidth;
    }

    void Start() {


        meshRenderer = hitPoint.GetComponent<TrailRenderer>();
        meshRenderer.enabled = false;
        CreateLine();

        if (BodySourceManager == null) {
            Debug.Log("Assign Game Object with Body Source Manager!");
        }
        else {
            bodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        }
    }

    // Update is called once per frame
    void Update() {

        // in case the brush hovered the paint or the brush size
        if (Physics.Raycast(transform.position, Vector3.forward, out hitZone)) {
            if (!this.handDetectedOpen) {
                hitPoint.transform.position = hitZone.point;
            }
            ReconstructBrush(hitZone);
        }

        if (bodyManager == null) {
            return;
        }
        bodies = bodyManager.GetData();

        // happens in the first miliseconds of game initialization
        if (bodies == null) {
            return;
        }
        foreach (var body in bodies) {
            if (body == null) {
                // jump to the next element
                continue;
            }
            if (body.IsTracked) {
                // this code detects the body with Kinect
                var pos = body.Joints[TrackedJoint].Position;
                gameObject.transform.position = new Vector3(pos.X * multiplier, pos.Y * multiplier);

                // if the hand is not open at the beginning, a line should be drawn
                if (!this.handDetectedOpen) {
                    vectorDrawnLinesList.Add(new Vector3(pos.X * multiplier, pos.Y * multiplier));
                    ReconstructLine(this.line);
                }

                rightHand = body.Joints[JointType.HandRight];

                if(body.HandRightState == HandState.Open) {
                    this.handDetectedOpen = true;
                }
                else if(body.HandRightState == HandState.Closed) {
                    if (this.handDetectedOpen) {
                        CreateLine();
                    }
                    this.handDetectedOpen = false;
                }
            }
        }
    }

    void CreateLine() {

        this.lines.Add(new GameObject().AddComponent<LineRenderer>());
        this.line = this.lines[this.lines.Count - 1];
        this.line.sortingOrder = lineOrder;
        this.line.material.color = this.color;
        this.lineNumber = 0;
        this.lineOrder++;
        this.vectorDrawnLinesList = new List<Vector3>();
    }

    void ReconstructBrush(RaycastHit hit) {

        if (hit.collider.tag.Equals("GreenBucket")) {
            this.color = new Color(0, 255, 0);
            brushSprite.color = new Color(0, 255, 0);
        }
        else if (hit.collider.tag.Equals("RedBucket")) {
            this.color = new Color(255, 0, 0);
            brushSprite.color = new Color(255, 0, 0);
        }
        else if (hit.collider.tag.Equals("BlueBucket")) {
            this.color = new Color(0, 0, 238);
            brushSprite.color = new Color(0, 0, 238);
        }
        else if (hit.collider.tag.Equals("BlackBucket")) {
            this.color = new Color(0, 0, 0);
            brushSprite.color = new Color(0, 0, 0);
        }
        else if (hit.collider.tag.Equals("WhiteBucket")) {
            this.color = new Color(190, 186, 186);
        }
        else if (hit.collider.tag.Equals("SmallBrush")) {
            this.lineWidth = 0.1f;
        }
        else if (hit.collider.tag.Equals("MediumBrush")) {
            this.lineWidth = 0.5f;
        }
        else if (hit.collider.tag.Equals("LargeBrush")) {
            this.lineWidth = 0.9f;
        }
        // music options
        else if (hit.collider.tag.Equals("IntroSong")) {
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
        else if(hit.collider.tag.Equals("SecondSong")) {
            songs = GetComponentsInChildren<UnityEngine.AudioSource>();
            songs[0].Stop();
            songs[1].Play();
        }

        if (this.meshRenderer) {
            this.meshRenderer.material.color = this.color;
        }
    }
    
    void ReconstructLine(LineRenderer line) {
        // set the line width
        line.startWidth = this.lineWidth;
        line.endWidth = this.lineWidth;

        line.positionCount = vectorDrawnLinesList.Count;

        int i = lineNumber;
        while(i < vectorDrawnLinesList.Count) {
            line.SetPosition(i, vectorDrawnLinesList[i]);
            i++;
        }

        lineNumber = vectorDrawnLinesList.Count;
    }
}
