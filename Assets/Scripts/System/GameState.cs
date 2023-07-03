using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectState
{
   public bool isPlaying =false;
}

public class GameState : MonoBehaviour
{
    private static List<ObjectState> listObject = new List<ObjectState>(0);

   [SerializeField] private static bool m_isPlaying;

    public void MoveInput(InputAction.CallbackContext ctx)
    {
        if(ctx.started)
        {
            ChangeState();
        }
    }

        public static void ChangeState()
    {
        m_isPlaying = !m_isPlaying;
        for (int i = 0; i < listObject.Count; i++)
        {
            listObject[i].isPlaying = m_isPlaying;
        }
    }

    public static void AddObject(ObjectState state)
    {
        listObject.Add(state);
        state.isPlaying = m_isPlaying;
    }

    public static void RemoveObject(ObjectState state)
    {
        listObject.Remove(state);
    }
}
