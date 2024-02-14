using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class WaitForPlayersLocked : IAction
{
    private List<Player> _players;
    private List<IAction> _postActions;

    public WaitForPlayersLocked(List<Player> players) {
        _players = players;
        _postActions = new();
    }

    public bool Execute(float deltaTime, ControllersLibrary controllers)
    {
        if (_players.TrueForAll(player => player.isLockedIn.Value))
        {
            _postActions.Add(
                new SubmitPlayerActions(
                    _players,
                    controllers.GetRoundController().GetPlayerCards()));
            _players.ForEach(player => player.isLockedIn.Value = false);
                    return true;
        }
        return false;
    }

    public List<IAction> PostActions()
    {
        return _postActions;
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
                PlayerCards playerCards = _playerCards[item];
                int currentCard = playerCards.ChosenCards[_currentCardPlace];
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
