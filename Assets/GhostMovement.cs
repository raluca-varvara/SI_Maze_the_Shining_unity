using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    private float moveSpeed = 1f;
    private Vector3 moveDirection;
    public string wallTag = "Wall";
    [SerializeField] public MazeRenderer mazeRenderer;
    public Menu menu;

    // private Rigidbody rb;
    private int height = 0;
    private int width = 0;
    private int yOffset = 0;
    private WallState[,] maze = null;

    private bool start = false;

    private Vector3Int currentCell;
    private Vector3Int targetCell;
    private bool isMoving = false;
    

    void Start()
    {
        // rb = GetComponent<Rigidbody>();
        moveDirection = Vector3.forward;
    
        height = mazeRenderer.GetHeight();
        width = mazeRenderer.GetWidth();

        GameObject canvasObject = GameObject.Find("Canvas");
        // Get the Menu script component from the Canvas GameObject
        menu = canvasObject.GetComponent<Menu>();
        
        StartCoroutine(WaitForMazes());

    }

    private System.Collections.IEnumerator WaitForMazes()
    {
        // Wait until the mazes are generated
        while (!mazeRenderer.AreMazesGenerated())
        {
            yield return null; // Wait for the next frame
        }
        Vector3 position = transform.position;
        if(position[2]>height/2)
        {
            maze = mazeRenderer.GetMaze2();
            Debug.Log(" Pos Maze " + maze[0,0] + " 01 " + maze[0,1] + " 10 "+ maze[1,0]);
            yOffset = height+1;
            currentCell = new Vector3Int(Mathf.RoundToInt(transform.position.x+ width/2) , 0, Mathf.RoundToInt(transform.position.z+height/2-yOffset));
            targetCell = currentCell;
            Debug.Log("Current_cell "+ currentCell);
            start = true;
        }
        else{
            maze = mazeRenderer.GetMaze1();
            Debug.Log(" Pos Maze " + maze[0,0] + " 01 " + maze[0,1] + " 10 "+ maze[1,0]);
            currentCell = new Vector3Int(Mathf.RoundToInt(transform.position.x+ width/2) , 0, Mathf.RoundToInt(transform.position.z+height/2-yOffset));
            targetCell = currentCell;
            Debug.Log("Current_cell "+ currentCell);
            start = true;
        }

    }


    void Update(){
        if(start && (menu.GetStartGame()==1)){
            if (isMoving)
            {
                // Move the character towards the target cell
                transform.position = Vector3.MoveTowards(transform.position, CellToPosition(targetCell), moveSpeed * Time.deltaTime);
                // Check if the character has reached the target cell
                if (Vector3.Distance(transform.position, CellToPosition(targetCell)) < 0.001f)
                {
                    // Stop moving
                    isMoving = false;
                    
                    // Set current cell to target cell
                    currentCell = new Vector3Int(Mathf.RoundToInt(transform.position.x+width/2), 0, Mathf.RoundToInt(transform.position.z+height/2-yOffset));
                    
                    // Find next target cell
                    FindNextTargetCell();
                    
                }
                Vector3 direction = (CellToPosition(targetCell) - transform.position).normalized;
                direction.y = 0; // Ensure y-component is zero

                if (direction != Vector3.zero)
                {
                    // Calculate rotation towards the direction
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    // Only adjust the y-component of the rotation
                    targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
                    // Smoothly rotate towards the target rotation
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
                }
            }
            else
            {
                // Start moving towards the target cell
                isMoving = true;
            }

        }
    }
    void FindNextTargetCell()
    {
        // Check for viable directions
        Vector3[] directions = GetViableDirections(currentCell);

        if (directions.Length > 0)
        {
            // Choose a random direction from viable directions
            int randomIndex = Random.Range(0, (int)directions.Length);
            targetCell = currentCell + new Vector3Int((int)directions[randomIndex].x, 0, (int)directions[randomIndex].z);

        }
        else
        {
            // No viable directions, stay in the current cell
            targetCell = currentCell;
        }
    }

    // Method to get viable directions from the current cell
    Vector3[] GetViableDirections(Vector3Int cell)
    {
        System.Collections.Generic.List<Vector3> viableDirections = new System.Collections.Generic.List<Vector3>();

        // Check each direction for viable paths
        if (!maze[cell.x, cell.z].HasFlag(WallState.LEFT) && !maze[cell.x - 1, cell.z].HasFlag(WallState.RIGHT))
            viableDirections.Add(Vector3.left);
        if (!maze[cell.x, cell.z].HasFlag(WallState.RIGHT) && !maze[cell.x + 1, cell.z].HasFlag(WallState.LEFT))
            viableDirections.Add(Vector3.right);
        if (!maze[cell.x, cell.z].HasFlag(WallState.UP) && !maze[cell.x, cell.z + 1].HasFlag(WallState.DOWN))
            viableDirections.Add(Vector3.forward);
        if (!maze[cell.x, cell.z].HasFlag(WallState.DOWN) && !maze[cell.x, cell.z - 1].HasFlag(WallState.UP))
            viableDirections.Add(Vector3.back);

        return viableDirections.ToArray();
    }
    Vector3 PositionToCell(Vector3 position){
        return new Vector3(Mathf.RoundToInt(position.x+ width/2) , 0, Mathf.RoundToInt(position.z+height/2-yOffset));
    }
    Vector3 CellToPosition(Vector3 cell){
        return new Vector3(Mathf.RoundToInt(cell.x - width/2) , 0.3f, Mathf.RoundToInt(cell.z - height/2 + yOffset));
    }
    
}
