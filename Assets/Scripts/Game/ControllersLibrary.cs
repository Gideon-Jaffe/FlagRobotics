using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ControllersLibrary : MonoBehaviour
{
    [SerializeField] private Tilemap boardTileMap;

    [SerializeField] private CheckpointController checkpointController;

    [SerializeField] private GameOverManager gameOverManager;

    [SerializeField] private ConveyorController conveyorController;

    [SerializeField] private CardController cardController;

    [SerializeField] private RoundController roundController;

    [SerializeField] private MapController mapController;

    [SerializeField] private PrefabLibrary prefabLibrary;

    public Tilemap GetBoardTileMap()
    {
        return boardTileMap;
    }

    public CheckpointController GetCheckpointController()
    {
        return checkpointController;
    }

    public GameOverManager GetGameOverManager()
    {
        return gameOverManager;
    }

    public ConveyorController GetConveyorController()
    {
        return conveyorController;
    }

    public CardController GetCardController()
    {
        return cardController;
    }

    public RoundController GetRoundController()
    {
        return roundController;
    }

    public MapController GetMapController()
    {
        return mapController;
    }

    public PrefabLibrary GetPrefabLibrary()
    {
        return prefabLibrary;
    }
}
