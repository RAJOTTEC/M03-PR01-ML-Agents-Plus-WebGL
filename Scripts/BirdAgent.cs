using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class BirdAgent : Agent
{
    [SerializeField] private Level level;
    private Bird bird;
    private bool isJumpInputDown;

    private void Awake()
    {
        bird = GetComponent<Bird>();
    }

    private void Start()
    {
        bird.OnDied += Bird_OnDied;
        level.OnPipePassed += Level_OnPipePassed;
    }

    private void Level_OnPipePassed(object sender, System.EventArgs e)
    {
        AddReward(1f);
    }

    private void Bird_OnDied(object sender, System.EventArgs e)
    {
        //EndEpisode();
        gameObject.SetActive(false);
        //Loader.Load(Loader.Scene.GameScene);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isJumpInputDown = true;
        }
    }

    public override void OnEpisodeBegin()
    {
        bird.Reset();
        //level.Reset();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        float worldHeight = 100f;
        float birdNormalizedY = (bird.transform.position.y + (worldHeight / 2f)) / worldHeight;
        sensor.AddObservation(birdNormalizedY);

        float pipeSpawnXPosition = 100f;
        Level.PipeComplete pipeComplete = level.GetNextPipeComplete();
        if (pipeComplete != null && pipeComplete.pipeBottom != null && pipeComplete.pipeBottom.pipeBodyTransform != null)
        {
            sensor.AddObservation(pipeComplete.pipeBottom.GetXPosition() / pipeSpawnXPosition);
        } else
        {
            sensor.AddObservation(1f);
        }

        sensor.AddObservation(bird.GetVelocityY() / 200f);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (actions.DiscreteActions[0] == 1)
        {
            bird.Jump();
        }

        //AddReward(10f / MaxStep);

        //Debug.Log(GetCumulativeReward());
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = isJumpInputDown ? 1 : 0;

        isJumpInputDown = false;
    }
}
