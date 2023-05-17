using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameTest : MonoBehaviour
{
    [SerializeField] GameObject brickPrefab;
    [SerializeField] GameObject ballPrefab;
    [SerializeField] GameObject mainCamera;
    [SerializeField] float impulse;
    public bool isRectangular;

    private static Quaternion vertical = Quaternion.Euler(0f, 90f, 0f);
    private static int RECT_WIDTH = 10;
    private static int RECT_HEIGHT = 10;
    private static int RECT_DEPTH = 5;
    private static int SQUARE_WIDTH = 6;
    private static int SQUARE_HEIGHT = 20;
    private static int SQUARE_DEPTH = 6;

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
    private HashSet<GameObject> downBricks;
    // Start is called before the first frame update
    void Start()
    {
        loadPrefs();
        calculateTower();
        buildTower();
        isGameOn = true;
        //StartCoroutine(test());
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && isGameOn)
            createBall();
        CameraMovement();
        progress = (float) downBricks.Count / (float) generatedBricks;
        if(progress >= progressRate){
            Debug.Log("Game Over");
            isGameOn = false;
            //enable text on canvas 
        }
    }

    private IEnumerator test() {
        yield return new WaitForSeconds(5f);
        GameObject.FindObjectsOfType<GameObject>()
            .Where(go => go.name.StartsWith("Brick") || go.name.StartsWith("Ball"))
            .ToList()
            .ForEach(Destroy);
    }

    private void CameraMovement() {
        float speed = 10f;
        if(Input.GetKey(KeyCode.W))
            mainCamera.transform.Translate(Vector3.forward * speed * Time.deltaTime);
        if(Input.GetKey(KeyCode.A))
            mainCamera.transform.Translate(Vector3.left * speed * Time.deltaTime);
        if(Input.GetKey(KeyCode.S))
            mainCamera.transform.Translate(Vector3.back * speed * Time.deltaTime);
        if(Input.GetKey(KeyCode.D))
            mainCamera.transform.Translate(Vector3.right * speed * Time.deltaTime);
        if(Input.GetKey(KeyCode.Q))
            mainCamera.transform.Translate(Vector3.up * speed * Time.deltaTime);
        if(Input.GetKey(KeyCode.E))
            mainCamera.transform.Translate(Vector3.down * speed * Time.deltaTime);
        if(Input.GetKey(KeyCode.UpArrow))
            mainCamera.transform.Rotate(Vector3.left * speed * Time.deltaTime);
        if(Input.GetKey(KeyCode.DownArrow))
            mainCamera.transform.Rotate(Vector3.right * speed * Time.deltaTime);
        if(Input.GetKey(KeyCode.LeftArrow))
            mainCamera.transform.Rotate(Vector3.down * speed * Time.deltaTime, Space.World);
        if(Input.GetKey(KeyCode.RightArrow))
            mainCamera.transform.Rotate(Vector3.up * speed * Time.deltaTime, Space.World);
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
        progressRate = PlayerPrefs.GetFloat("PROGRESS_RATE", 0.9f);
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
        downBricks = new HashSet<GameObject>();
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
            createBrick(position, vertical);
        }
        position.x = right - rotationOffset;
        for(int col = 0; col < depth; col++) {
            position.z = forward  - rotationOffset - col * brickWidth;
            position.z -= offset ? brickOffset : 0;
            createBrick(position, vertical);
        }
    }

    private void createBrick(Vector3 position, Quaternion rotation) {
        GameObject brick = Instantiate(brickPrefab, position, rotation) as GameObject;
        float random = Random.Range(0.7f, 0.9f);
        if(isRectangular)
            brick.GetComponent<Renderer>().material.color = new Color(random + 0.1f, random - 0.2f, 0.5f);
        else
            brick.GetComponent<Renderer>().material.color = new Color(random, random, random);
        generatedBricks++;
    }

    private void createBall() {
        Vector3 position = mainCamera.transform.position + new Vector3(0, -1f, 0);
        GameObject ball = Instantiate(ballPrefab, position, Quaternion.identity) as GameObject;
        ball.GetComponent<Renderer>().material.color = new Color(0.1f, 0.1f, 0.1f);
        ball.GetComponent<Rigidbody>().AddForce(mainCamera.transform.forward * impulse, ForceMode.Impulse);
        ++generatedBalls;
        Debug.Log("Progress: " + downBricks.Count + " / " + generatedBricks);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.name == "Brick(Clone)") downBricks.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.transform.position.y < transform.position.y) Destroy(other.gameObject);
    }

}
