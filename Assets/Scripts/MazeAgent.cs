using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MazeAgent : Agent
{
    [Header("Références")]
    public MazeGenerator mazeGen;
    public Transform agentBody;

    int posX, posY;
    float xOff, yOff;
    int stepCount;
    const int maxSteps = 500;

    // track visited cells
    HashSet<Vector2Int> visited;

    // Counters for inference-only success rate
    private static int _episodeCount = 0;
    private static int _successCount = 0;

    public override void Initialize()
    {
        // center offset calculation
        xOff = -mazeGen.width * 0.5f + 0.5f;
        yOff = -mazeGen.height * 0.5f + 0.5f;
    }

    public override void OnEpisodeBegin()
    {
        mazeGen.GenerateMaze();
        posX = 1;
        posY = 1;
        stepCount = 0;
        UpdatePosition();
        visited = new HashSet<Vector2Int> { new Vector2Int(posX, posY) };
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 1) relative pos
        sensor.AddObservation(posX / (float)mazeGen.width);
        sensor.AddObservation(posY / (float)mazeGen.height);
        // 2) vector to exit
        int exitX = mazeGen.width - 2;
        int exitY = mazeGen.height - 2;
        sensor.AddObservation((exitX - posX) / (float)mazeGen.width);
        sensor.AddObservation((exitY - posY) / (float)mazeGen.height);
        // 3) walls around
        var cell = mazeGen.grid[posX, posY];
        sensor.AddObservation(cell.WallTop ? 1f : 0f);
        sensor.AddObservation(cell.WallRight ? 1f : 0f);
        sensor.AddObservation(cell.WallBottom ? 1f : 0f);
        sensor.AddObservation(cell.WallLeft ? 1f : 0f);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        stepCount++;
        AddReward(-0.005f); // small time penalty

        int a = actions.DiscreteActions[0];
        Vector2Int dir = a switch
        {
            0 => Vector2Int.up,
            1 => Vector2Int.right,
            2 => Vector2Int.down,
            3 => Vector2Int.left,
            _ => Vector2Int.zero
        };

        int newX = posX + dir.x;
        int newY = posY + dir.y;

        // out of bounds check
        if (newX < 0 || newX >= mazeGen.width || newY < 0 || newY >= mazeGen.height)
        {
            AddReward(-0.5f);
            if (stepCount > maxSteps) EndEpisodeWithFailure();
            return;
        }

        // wall collision
        var currentCell = mazeGen.grid[posX, posY];
        bool blocked = (dir.x == 1 && currentCell.WallRight) ||
                       (dir.x == -1 && currentCell.WallLeft) ||
                       (dir.y == 1 && currentCell.WallTop) ||
                       (dir.y == -1 && currentCell.WallBottom);
        if (blocked)
        {
            AddReward(-0.5f);
            if (stepCount > maxSteps) EndEpisodeWithFailure();
            return;
        }

        // revisit penalty
        Vector2Int target = new Vector2Int(newX, newY);
        if (visited.Contains(target)) AddReward(-0.2f);
        else visited.Add(target);

        // distance shaping
        int exitX2 = mazeGen.width - 2;
        int exitY2 = mazeGen.height - 2;
        float oldDist = Vector2.Distance(new Vector2(posX, posY), new Vector2(exitX2, exitY2));

        // move agent
        posX = newX;
        posY = newY;
        UpdatePosition();
        AddReward(0.1f);

        float newDist = Vector2.Distance(new Vector2(posX, posY), new Vector2(exitX2, exitY2));
        AddReward((oldDist - newDist) * 0.1f);

        // success condition
        if (posX == exitX2 && posY == exitY2)
        {
            AddReward(20f);
            EndEpisodeWithSuccess();
            return;
        }

        // timeout condition
        if (stepCount > maxSteps)
        {
            EndEpisodeWithFailure();
        }
    }

    void EndEpisodeWithSuccess()
    {
        _successCount++;
        EndEpisodeCommon(1);
    }

    void EndEpisodeWithFailure()
    {
        EndEpisodeCommon(0);
    }

    void EndEpisodeCommon(int successFlag)
    {
        _episodeCount++;
        float rate = _successCount / (float)_episodeCount;
        Debug.Log($"Episode {_episodeCount} – Success: {successFlag} – Success rate = {rate:P2}");
        EndEpisode();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var act = actionsOut.DiscreteActions;
        if      (Input.GetKey(KeyCode.UpArrow))    act[0] = 0;
        else if (Input.GetKey(KeyCode.RightArrow)) act[0] = 1;
        else if (Input.GetKey(KeyCode.DownArrow))  act[0] = 2;
        else if (Input.GetKey(KeyCode.LeftArrow))  act[0] = 3;
        else return;
        RequestDecision();
    }

    void UpdatePosition()
    {
        agentBody.position = new Vector3(posX + xOff, posY + yOff, 0);
    }
}