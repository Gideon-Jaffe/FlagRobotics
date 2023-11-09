using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.U2D.Animation;

public class Player : NetworkBehaviour
{   
    [SerializeField] private SpriteLibraryAsset CharacterSprites;

    public NetworkVariable<FixedString32Bytes> username = new("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public NetworkVariable<bool> isLockedIn = new(false);

    public NetworkVariable<Utilities.Direction> characterFacing = new(Utilities.Direction.Forward);

    public NetworkVariable<FixedString32Bytes> characterSprite = new();

    public bool IsAlive {get; set;} = true;

    public Vector2Int currentPoint = Vector2Int.zero;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        characterSprite.OnValueChanged += CharacterSpriteChanged;
        characterFacing.OnValueChanged += CharacterDirectionChanged;
    }

    public void CharacterDirectionChanged(Utilities.Direction previousValue, Utilities.Direction newValue)
    {
        GetComponent<SpriteRenderer>().sprite = CharacterSprites.GetSprite(characterSprite.Value.ToString(), newValue.ToString());
    }

    public void CharacterSpriteChanged(FixedString32Bytes previousValue, FixedString32Bytes newValue)
    {
        GetComponent<SpriteRenderer>().sprite = CharacterSprites.GetSprite(newValue.ToString(), characterFacing.Value.ToString());
    }

    public void SetCharacterSprites(String spriteList)
    {
        characterSprite.Value = spriteList;
    }

    public void TurnRight()
    {
        SetCharacterDirection((Utilities.Direction)(((int)characterFacing.Value + 1)%4));
    }

    public void TurnLeft()
    {
        SetCharacterDirection((Utilities.Direction)(((int)characterFacing.Value + 3)%4));
    }

    public void SetCharacterDirection(Utilities.Direction direction)
    {
        Debug.Log(characterSprite.Value.ToString());
        characterFacing.Value = direction;
        GetComponent<SpriteRenderer>().sprite = CharacterSprites.GetSprite(characterSprite.Value.ToString(), characterFacing.Value.ToString());
   }

    public void AddToCurrentPoint(Vector2Int movement)
    {
        currentPoint += movement;
    }

    public Vector2Int GetCurrentPoint()
    {
        return currentPoint;
    }

    public Vector2Int GetCharacterFacingVector() 
    {
        return characterFacing.Value.DirectionVector();
    }

    public PlayerSerializable ToSerializedPlayer() 
    {
        return new PlayerSerializable(username.Value.ToString(), isLockedIn.Value);
    }

    public void CopyFromSerializedPlayer(PlayerSerializable playerSerializable) 
    {
        username.Value = playerSerializable.username;
        isLockedIn.Value = playerSerializable.isLockedIn;
    }
}

public class PlayerSerializable : INetworkSerializable
{
    public string username;

    public bool isLockedIn;

    public PlayerSerializable() {
        username = "";
        isLockedIn = false;
    }

    public PlayerSerializable(string name, bool lockedIn) {
        username = name;
        isLockedIn = lockedIn;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref username);
        serializer.SerializeValue(ref isLockedIn);
    }
}
