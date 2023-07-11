using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectState
{
    public bool isPlaying = true;
}

public class GameState : MonoBehaviour
{
    private static Enemies.EnemyManager m_enemyManager;
    private static List<ObjectState> listObject = new List<ObjectState>(0);

    [SerializeField] private static bool m_isPlaying =true;

    public void Start()
    {
        m_enemyManager = GetComponent<Enemies.EnemyManager>();
      
    }

    public void MoveInput(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
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
        m_enemyManager.ChangePauseState(m_isPlaying);
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
