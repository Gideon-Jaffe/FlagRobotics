using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionController : MonoBehaviour
{
    private LinkedList<IAction> actions;
    private IAction lastAction;

    [SerializeField] private ControllersLibrary controllersLibrary;

    // Start is called before the first frame update
    void Start()
    {
        actions = new LinkedList<IAction>();
    }

    public void AddAction(IAction action) {
        actions.AddLast(action);
    }

    // Update is called once per frame
    void Update()
    {
        IAction currentAction = GetFirstElement();
        if (currentAction != null) {
            if (lastAction != currentAction)
            {
                Debug.Log("Executing Action: " + currentAction.GetType().Name);
                lastAction = currentAction;
            }
            if (currentAction.Execute(Time.deltaTime, controllersLibrary)) {
                actions.RemoveFirst();
                List<IAction> postActions = currentAction.PostActions();
                postActions.Reverse();
                foreach (var action in postActions)
                {
                    actions.AddFirst(action);
                }
                Debug.Log("Finished Action: " + currentAction.GetType().Name);
            }
        }
    }

    private IAction GetFirstElement() {
        LinkedListNode<IAction> firstNode = actions.First;
        if (firstNode == null) {
            return null;
        }

        return firstNode.Value;
    }
}
