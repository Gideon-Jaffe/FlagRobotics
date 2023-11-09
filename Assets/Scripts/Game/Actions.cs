using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IAction
{
    bool Execute(float deltaTime, ControllersLibrary controllers);

    List<IAction> PostActions();
}

public class GenericMoveAction : IAction
{
    protected readonly Player _player;

    private readonly int _amount = 1;

    protected Utilities.Direction _direction;

    private readonly List<Player> _players;

    protected bool _movementAdded = false;

    private IAction _subAction;

    public GenericMoveAction(Player player ,int amount, Utilities.Direction direction, List<Player> players) {
        _player = player;
        _amount = amount;
        _direction = direction;
        _players = players;
    }

    public virtual bool Execute(float deltaTime, ControllersLibrary controllers)
    {
        if (!_movementAdded) {
            Debug.Log("Player is facing: " + _player.characterFacing.Value.ToString());
            CreateSubAction();
            _player.AddToCurrentPoint(_direction.DirectionVector());
            _movementAdded = true;
        }
        
        var moveSpace = controllers.GetBoardTileMap().CellToWorld(Utilities.Vector2IntToVector3Int(_player.GetCurrentPoint(), 0));
        moveSpace.z = _player.transform.position.z;
        _player.transform.position = Vector3.MoveTowards(_player.transform.position, moveSpace, StaticVariables.SPEED * Time.deltaTime);
        return _subAction.Execute(deltaTime, controllers) && _player.transform.position == moveSpace;
    }

    private void CreateSubAction()
    {
        Player playerInWay = _players.Where(player => player.IsAlive && player.currentPoint == _player.GetCurrentPoint() + _direction.DirectionVector())
            .FirstOrDefault();
        if (playerInWay != null)
        {
            _subAction = new GenericMoveAction(playerInWay, 1, _direction, _players);
        } else {
            _subAction = new TrueAction();
        }
    }

    public List<IAction> PostActions()
    {
        List<IAction> postActions = new()
        {
            new CheckAliveAction(_player),
            
        };
        postActions.AddRange(_subAction.PostActions());
        if (_amount > 1)
        {
            postActions.Add(new GenericMoveAction(_player, _amount - 1, _direction, _players));
        }
        return postActions;
    }
}

public class MoveBasedOnCharacterFacingAction : GenericMoveAction
{
    public MoveBasedOnCharacterFacingAction(Player player, int amount, Utilities.Direction direction, List<Player> players)
        : base(player, amount, direction, players){}

    public override bool Execute(float deltaTime, ControllersLibrary controllers)
    {
        if (!_movementAdded) 
        {
            _direction = SetDirectionBasedOnPlayer();
        }
        return base.Execute(deltaTime, controllers);
    }

    private Utilities.Direction SetDirectionBasedOnPlayer()
    {
        return _direction switch
        {
            Utilities.Direction.Forward => _player.characterFacing.Value,
            Utilities.Direction.Back => _player.characterFacing.Value.Reverse(),
            Utilities.Direction.Right => (Utilities.Direction)(((int)_player.characterFacing.Value + 1)%4),
            Utilities.Direction.Left => (Utilities.Direction)(((int)_player.characterFacing.Value + 3)%4),
            _ => _player.characterFacing.Value,
        };
    }
}

public class TurnAction : IAction
{
    private Player _player;

    private readonly Utilities.Direction _turnDirection;

    public TurnAction(Player player, Utilities.Direction turnDirection) {
        _player = player;
        _turnDirection = turnDirection;
    }

    public List<IAction> PostActions()
    {
        return new List<IAction>();
    }

    public bool Execute(float deltaTime, ControllersLibrary controllers)
    {
        if (_turnDirection.DirectionVector() == Vector2Int.left) {
            _player.TurnLeft();
        } else if (_turnDirection.DirectionVector() == Vector2Int.right) {
            _player.TurnRight();
        }
    
        return true;
    }
}

public class CheckAliveAction : IAction
{
    private Player _player;

    public CheckAliveAction(Player player) {
        _player = player;
    }

    public bool Execute(float deltaTime, ControllersLibrary controllers)
    {
        Vector2Int currentPoint = _player.GetCurrentPoint();
        var pointV3 = new Vector3Int(currentPoint.x, currentPoint.y, 0);
        if (Utilities.CheckIfBlank(pointV3, controllers.GetBoardTileMap())) {
            Debug.Log("player died");
            _player.IsAlive = false;
        }

        return true;
    }

    public List<IAction> PostActions()
    {
        if (!_player.IsAlive)
        {
            return new()
            {
                new FallToDeathAction(_player)
            };
        }

        return new List<IAction>();
    }
}

public class FallToDeathAction : IAction
{
    private Player _player;

    public FallToDeathAction(Player player)
    {
        _player = player;
    }

    public bool Execute(float deltaTime, ControllersLibrary controllers)
    {
        _player.transform.position += Vector3.down * StaticVariables.SPEED * Time.deltaTime;
        if (_player.transform.position.y < -15) {
            return true;
        }

        return false; 
    }

    public List<IAction> PostActions()
    {
        return new List<IAction>();
    }
}

public class CaptureCheckpointsAction : IAction
{
    private readonly List<Player> _players;

    private bool _gameFinished;

    public CaptureCheckpointsAction(List<Player> players)
    {
        _players = players;
        _gameFinished = false;
    }

    public bool Execute(float deltaTime, ControllersLibrary controllers)
    {
        foreach (var player in _players)
        {
            bool wonGame = controllers.GetCheckpointController().CaptureCheckpoint(player);
            if (wonGame)
            {
                _gameFinished = true;
            }
        }

        return true;
    }

    public List<IAction> PostActions()
    {
        if (_gameFinished) 
        {
            return new() 
            {
                new GameOverAction()
            };
        }
        return new() {};
    }
}

public class GameOverAction : IAction
{
    public bool Execute(float deltaTime, ControllersLibrary controllers)
    {
        List<Player> victoriousPlayers = controllers.GetCheckpointController().VictoriousPlayers();
        controllers.GetGameOverManager().SetGameOverClientRpc(victoriousPlayers[0].username.Value.ToString());
        return true;
    }

    public List<IAction> PostActions()
    {
        return new List<IAction>();
    }
}

public class ConveyorAction : IAction
{
    private List<Player> _players;

    private List<IAction> _subActions;

    private List<IAction> _postActions;

    private int _currentMinConveyorSpeed = -1;

    public ConveyorAction(List<Player> players)
    {
        _players = players;
        _postActions = new List<IAction>();
    }

    public bool Execute(float deltaTime, ControllersLibrary controllers)
    {
        if (_currentMinConveyorSpeed == -1)
        {
            _currentMinConveyorSpeed = controllers.GetConveyorController().maxConveyorSpeed;
        }
        if (_subActions == null)
        {
            CreateSubActions(controllers);
        }

        bool finished = true;
        foreach (IAction action in _subActions)
        {
            if (!action.Execute(deltaTime, controllers))
            {
                finished = false;
            } else {
                _postActions.AddRange(action.PostActions());
            }
        }
        if (finished && _currentMinConveyorSpeed > 1)
        {
            _currentMinConveyorSpeed--;
            _postActions.Add(this);
        }
        return finished;
    }

    private void CreateSubActions(ControllersLibrary controllers) 
    {
        _subActions = new List<IAction>();
        Dictionary<Player, Utilities.Direction> movingPlayers = controllers.GetConveyorController().PlayersToMove(_players, _currentMinConveyorSpeed);
        foreach (var playerdirectionPair in movingPlayers)
        {
            _subActions.Add(new GenericMoveAction(playerdirectionPair.Key, 1, playerdirectionPair.Value, _players));
        }
        Debug.Log("Added " + movingPlayers.Count + " ConveyorMoves");
    }

    public List<IAction> PostActions()
    {
        return _postActions;
    }
}

public class CheckEndGameAction : IAction
{
    private List<Player> _players;

    public CheckEndGameAction(List<Player> players)
    {
        _players = players;
    }

    public bool Execute(float deltaTime, ControllersLibrary controllers)
    {
        List<Player> alivePlayers = _players.Where(player => player.IsAlive).ToList();
        if (alivePlayers.Count <= 1)
        {
            Player player = alivePlayers.FirstOrDefault();
            if (player == null)
            {
                controllers.GetGameOverManager().SetGameOverClientRpc("TIE");
            } else 
            {
                controllers.GetGameOverManager().SetGameOverClientRpc(player.username.Value.ToString());
            }
        }
        return true;
    }

    public List<IAction> PostActions()
    {
        return new();
    }
}

public class TrueAction : IAction
{
    public bool Execute(float deltaTime, ControllersLibrary controllers)
    {
        return true;
    }

    public List<IAction> PostActions()
    {
        return new();
    }
}

public class WaitForPlayersLocked : IAction
{
    private List<Player> _players;

    public bool Execute(float deltaTime, ControllersLibrary controllers)
    {
        return _players.TrueForAll(player => player.isLockedIn.Value);
    }

    public List<IAction> PostActions()
    {
        return new();
    }
}

public class SubmitPlayerActions : IAction
{
    private readonly List<Player> _players;
    private readonly Dictionary<Player, PlayerCards> _playerCards;
    private int _currentCardPlace = 0;
    private List<IAction> _postActions;

    public SubmitPlayerActions(List<Player> players, Dictionary<Player, PlayerCards> playerCards)
    {
        _players = players;
        _playerCards = playerCards;
    }

    public bool Execute(float deltaTime, ControllersLibrary controllers)
    {
        _postActions = new List<IAction>();
        if (_currentCardPlace >= 5) return true;
        Debug.Log("currant card place: " + _currentCardPlace);
        foreach (Player item in _players)
        {
            if (item.IsAlive)
            {
                int currentCard = _playerCards[item].ChosenCards[_currentCardPlace];
                CreateActionFromCardId(currentCard, item);
            }
        }
        _postActions.Add(new ConveyorAction(_players));
        _postActions.Add(new CheckEndGameAction(_players));
        _postActions.Add(new CaptureCheckpointsAction(_players));

        _currentCardPlace++;
        _postActions.Add(this);

        return true;
    }

    private void CreateActionFromCardId(int cardId, Player player)
    {
        Cards.CardInfo card = Cards.GetInstance().GetCardById(cardId);
        if (card.direction == Utilities.Direction.Left || card.direction == Utilities.Direction.Right) {
            _postActions.Add(new TurnAction(player, card.direction));
        } else {
            _postActions.Add(new MoveBasedOnCharacterFacingAction(player, 1, card.direction, _players));
            _postActions.Add(new CheckEndGameAction(_players));
        }
    }

    public List<IAction> PostActions()
    {
        return _postActions;
    }
}
