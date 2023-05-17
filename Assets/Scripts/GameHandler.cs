using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    [SerializeField] GameObject brickPrefab;
    [SerializeField] GameObject ballPrefab;
    [SerializeField] GameObject mainCamera;
    [SerializeField] Text progressDisplay;
    [SerializeField] Text ballsDisplay;
    [SerializeField] GameObject gameOverWindow;
    [SerializeField] Text gameOverDisplay;
    [SerializeField] float impulse;
    public bool isRectangular;

    private static Quaternion VERTICAL = Quaternion.Euler(0f, 90f, 0f);
    private static int RECT_WIDTH = 10;
    private static int RECT_HEIGHT = 10;
    private static int RECT_DEPTH = 5;
    private static int SQUARE_WIDTH = 6;
    private static int SQUARE_HEIGHT = 20;
    private static int SQUARE_DEPTH = 6;
    private static float PROGRESS_RATE = 90.0f;

    private bool isGameOn;
    private int generatedBricks;
    private int generatedBalls;
    private int width;
    private int height;
    private int depth;
    private float progressRate;
    private float progress;
    private float brickWidth;
    private float brickHeight;
    private float left;
    private float right;
    private float forward;
    private float back;
    private float rotationOffset;
    private float brickOffset;
    private float upOffset;
    private bool hasStarted;
    private HashSet<GameObject> downBricks;
    // Start is called before the first frame update
    void Start()
    {
        loadPrefs();
        gameOverWindow.SetActive(false);
        downBricks = new HashSet<GameObject>();
        isGameOn = false;
        hasStarted = false;
        delay(1f);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && isGameOn){
            if(!hasStarted){
                hasStarted = true;
                setKinematic(false);
            }
            createBall();
        }
        progress = (float) downBricks.Count / (float) generatedBricks;
        progress *= 100;
        if(isGameOn) progressDisplay.text = "Ladrillos derribados: " + ((int) progress) + "%";
        if(progress >= progressRate) gameOver();
    }

    public void OnTargetFound() {
        calculateTower();
        buildTower();
        isGameOn = true;
        delay(1f);
    }

    public void OnTargetLost() {
        GameObject.FindObjectsOfType<GameObject>()
            .Where(go => go.tag == "Brick" || go.tag == "Ball")
            .ToList()
            .ForEach(Destroy);
        generatedBalls = 0;
        downBricks.Clear();
        isGameOn = false;
    }

    private void gameOver() {
        isGameOn = false;
        progressDisplay.text = "Ladrillos derribados: " + ((int) progress) + "%";
        String legend = "Partida finalizada:";
        legend += "\nBolas lanzadas: " + generatedBalls;
        legend += "\nLadrillos derribados: " + downBricks.Count;
        legend += "\nLadrillos totales: " + generatedBricks;
        gameOverDisplay.text = legend;
        gameOverWindow.SetActive(true);
    }

    private void loadPrefs() {
        if (isRectangular) {
            width = PlayerPrefs.GetInt("RECT_WIDTH", RECT_WIDTH);
            height = PlayerPrefs.GetInt("RECT_HEIGHT", RECT_HEIGHT);
            depth = PlayerPrefs.GetInt("RECT_DEPTH", RECT_DEPTH);
        } else {
            width = PlayerPrefs.GetInt("SQUARE_WIDTH", SQUARE_WIDTH);
            height = PlayerPrefs.GetInt("SQUARE_HEIGHT", SQUARE_HEIGHT);
            depth = PlayerPrefs.GetInt("SQUARE_DEPTH", SQUARE_DEPTH);
        }
        progressRate = PlayerPrefs.GetFloat("PROGRESS_RATE", PROGRESS_RATE);
    }

    private void calculateTower()  {
        generatedBricks = 0;
        generatedBalls = 0;
        brickWidth = brickPrefab.transform.localScale.x;
        brickHeight = brickPrefab.transform.localScale.y;
        brickOffset = brickWidth / 2f;
        upOffset = brickHeight / 2f + transform.position.y;
        left = transform.position.x - width * brickOffset;
        right = left + width * brickWidth;
        forward = transform.position.z + depth * brickOffset;
        back = forward - depth * brickWidth;
        rotationOffset = brickWidth / 4f;
    }

    private void buildTower() {
        bool offset = false;
        Vector3 position = Vector3.zero;
        for(int level = 0; level < height; level++, offset ^= true) {
            position.y = level * brickHeight + upOffset;
            buildLevel(position, offset);
        }
    }

    private void buildLevel(Vector3 position, bool offset) {
        position.z = forward;
        for(int row = 0; row < width; row++) {
            position.x = left + row * brickWidth;
            position.x += offset ? brickOffset : 0;
            createBrick(position, Quaternion.identity);
        }
        position.z = back;
        for(int row = 0; row < width; row++) {
            position.x = left + row * brickWidth;
            position.x += offset ? 0 : brickOffset;
            createBrick(position, Quaternion.identity);
        }
        position.x = left - rotationOffset;
        for(int col = 0; col < depth; col++) {
            position.z = forward + rotationOffset - brickOffset - col * brickWidth ;
            position.z -= offset ? 0 : brickOffset;
            createBrick(position, VERTICAL);
        }
        position.x = right - rotationOffset;
        for(int col = 0; col < depth; col++) {
            position.z = forward  - rotationOffset - col * brickWidth;
            position.z -= offset ? brickOffset : 0;
            createBrick(position, VERTICAL);
        }
    }

    private void createBrick(Vector3 position, Quaternion rotation) {
        GameObject brick = Instantiate(brickPrefab) as GameObject;
        brick.transform.SetParent(transform.parent.parent);
        brick.transform.position = position;
        brick.transform.rotation = rotation;
        float random = UnityEngine.Random.Range(0.7f, 0.9f);
        if(isRectangular)
            brick.GetComponent<Renderer>().material.color = new Color(random + 0.1f, random - 0.2f, 0.5f);
        else
            brick.GetComponent<Renderer>().material.color = new Color(random, random, random);
        generatedBricks++;
    }

    private void createBall() {
        Vector3 position = mainCamera.transform.position + new Vector3(0, -5f, 0);
        GameObject ball = Instantiate(ballPrefab) as GameObject;
        ball.transform.SetParent(transform.parent.parent);
        ball.transform.position = position;
        ball.transform.rotation = Quaternion.identity;
        ball.GetComponent<Renderer>().material.color = new Color(0.1f, 0.1f, 0.1f);
        ball.GetComponent<Rigidbody>().AddForce(mainCamera.transform.forward * impulse, ForceMode.Impulse);
        ++generatedBalls;
        ballsDisplay.text = "Bolas lanzadas: " + generatedBalls;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.name == "Brick(Clone)") downBricks.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.transform.position.y < transform.position.y) Destroy(other.gameObject);
    }

    public void backToMenu() {
        SceneManager.LoadScene(0);
    }

    public IEnumerator delay(float seconds){
        yield return new WaitForSeconds(seconds);
    }

    public void setKinematic(bool k){
        GameObject.FindObjectsOfType<GameObject>()
            .Where(go => go.tag == "Brick")
            .ToList()
            .ForEach(go => go.GetComponent<Rigidbody>().isKinematic = k);
    }

}
