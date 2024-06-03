using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeRenderer : MonoBehaviour
{

    private int width = 20;
    private int height = 20;
    private float size = 1f;
    private int minDistanceBetweenArtifacts = 2;
    public int numberOfPowerUps = 4;
    public float spawnInterval = 10f;

    [SerializeField] private Transform wallMazePrefab = null;
    [SerializeField] private Transform wallHotelPrefab = null;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject[] artifactPrefabs; // Array to hold different artifact prefabs
    [SerializeField] private GameObject[] hotelArtifactPrefabs;
    [SerializeField] public GameObject[] powerupPrefab;
    public WallState[,] walls1;
    public WallState[,] walls2;

    private bool mazesGenerated = false;
   

    // Start is called before the first frame update
    void Start()
    {
        var maze = MazeGenerator.Generate(width, height);
        var maze1 = MazeGenerator.Generate(width, height);
        mazesGenerated = true;
        walls1=maze;
        walls2=maze1;
        Draw(maze, 0, (int)(height/2), width-1, wallMazePrefab);
        Draw(maze1, height+1, (int)(height/2), 0,wallHotelPrefab);
        int numArtifacts = artifactPrefabs.Length;
        GenerateArtifacts(numArtifacts,artifactPrefabs,0);
        int numArtifactsHotel = artifactPrefabs.Length;
        GenerateArtifacts(numArtifactsHotel,hotelArtifactPrefabs, height+1);

        numberOfPowerUps = numberOfPowerUps*powerupPrefab.Length;
        StartCoroutine(RespawnArtifacts(numberOfPowerUps, powerupPrefab, height+1));
    }

    private void Draw(WallState[,] maze, int yOffset, int xOut, int yOut, Transform wallPrefab)
    {

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                var cell = maze[i, j];
                var position = new Vector3(-width / 2 + i, 0, -height / 2 + j + yOffset);

                GameObject coin = Instantiate(coinPrefab, new Vector3(position.x,position.y+0.2f,position.z),Quaternion.identity);
                if(!(i==xOut && j==yOut) ){
                    if (cell.HasFlag(WallState.UP))
                    {
                        var topWall = Instantiate(wallPrefab, transform) as Transform;
                        topWall.position = position + new Vector3(0, 0, size / 2);
                        topWall.localScale = new Vector3(size, topWall.localScale.y, topWall.localScale.z);
                    }

                    if (cell.HasFlag(WallState.LEFT))
                    {
                        var leftWall = Instantiate(wallPrefab, transform) as Transform;
                        leftWall.position = position + new Vector3(-size / 2, 0, 0);
                        leftWall.localScale = new Vector3(size, leftWall.localScale.y, leftWall.localScale.z);
                        leftWall.eulerAngles = new Vector3(0, 90, 0);
                    }

                    if (i == width - 1)
                    {
                        if (cell.HasFlag(WallState.RIGHT))
                        {
                            var rightWall = Instantiate(wallPrefab, transform) as Transform;
                            rightWall.position = position + new Vector3(+size / 2, 0, 0);
                            rightWall.localScale = new Vector3(size, rightWall.localScale.y, rightWall.localScale.z);
                            rightWall.eulerAngles = new Vector3(0, 90, 0);
                        }
                    }

                    if (j == 0)
                    {
                        if (cell.HasFlag(WallState.DOWN))
                        {
                            var bottomWall = Instantiate(wallPrefab, transform) as Transform;
                            bottomWall.position = position + new Vector3(0, 0, -size / 2);
                            bottomWall.localScale = new Vector3(size, bottomWall.localScale.y, bottomWall.localScale.z);
                        }
                    }
                }
                
            }

        }
    }

    private void GenerateArtifacts(int numArtifacts, GameObject[] artifactPrefabs, int yOffset)
    {
        List<Vector3> artifactPositions = GenerateRandomPositions(numArtifacts, yOffset);
        for (int i = 0; i < numArtifacts; i++)
        {
            GameObject artifact = Instantiate(artifactPrefabs[i],  artifactPositions[i], Quaternion.identity);
        }
    }

    private List<Vector3> GenerateRandomPositions(int numPositions, int yOffset)
    {
        List<Vector3> positions = new List<Vector3>();
        
        while (positions.Count < numPositions)
        {
            Vector3 newPos = new Vector3((int)Random.Range(-width / 2f, width / 2f), 0.2f, (int)Random.Range(-height / 2f, height / 2f)+yOffset);
            
            bool validPosition = true;
            foreach (Vector3 existingPos in positions)
            {
                if (Vector3.Distance(newPos, existingPos) < minDistanceBetweenArtifacts)
                {
                    validPosition = false;
                    break;
                }
            }
            
            if (validPosition)
            {
                positions.Add(newPos);
            }
        }

        return positions;
    }
    private IEnumerator RespawnArtifacts(int numPowers, GameObject[] artifactPrefabs, int yOffset)
    {
        while (true)
        {
            // Remove existing artifacts
            List<GameObject> existingArtifacts = new List<GameObject>();
            existingArtifacts.AddRange(GameObject.FindGameObjectsWithTag("speed"));
            existingArtifacts.AddRange(GameObject.FindGameObjectsWithTag("ghost"));

            foreach (GameObject artifact in existingArtifacts)
            {
                Destroy(artifact);
            }

            // Generate new artifacts
            List<Vector3> artifactPositions = GenerateRandomPositions(numPowers, yOffset);
            List<Vector3> artifactPositionsMaze = GenerateRandomPositions(numPowers, 0);
            artifactPositions.AddRange(artifactPositionsMaze);

            for (int i = 0; i < 2*numPowers; i++)
            {
                Instantiate(artifactPrefabs[i % artifactPrefabs.Length], artifactPositions[i], Quaternion.identity);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }


    // Update is called once per frame
    void Update()
    {

    }
    public WallState[,] GetMaze1(){
        if (!mazesGenerated)
        {
            Debug.LogError("Mazes are not generated yet!");
            return null;
        }
        return walls1;
    }
    public WallState[,] GetMaze2(){
        if (!mazesGenerated)
        {
            Debug.LogError("Mazes are not generated yet!");
            return null;
        }
        return walls2;
    }
    public int GetHeight(){
        return height;
    }
    public int GetWidth(){
        return width;
    }
    public bool AreMazesGenerated()
    {
        return mazesGenerated;
    }
}
