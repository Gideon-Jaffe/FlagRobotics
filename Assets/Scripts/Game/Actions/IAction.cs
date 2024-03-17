using System.Collections.Generic;

public interface IAction
{
    bool Execute(float deltaTime, ControllersLibrary controllers);

    List<IAction> PostActions();
}
