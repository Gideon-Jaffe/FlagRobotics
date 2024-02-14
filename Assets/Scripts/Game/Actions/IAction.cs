using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAction
{
    bool Execute(float deltaTime, ControllersLibrary controllers);

    List<IAction> PostActions();
}
