using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class PlayerMove : MonoBehaviour
{
    NavMeshAgent nav;
    NavMeshPath path;
    public static PlayerMove Ins { get; private set; }
    private void Awake()
    {

        Ins = this;
    }
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(0, 0, Time.deltaTime * 5);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(0, 0, Time.deltaTime * -5);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Time.deltaTime * -5, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Time.deltaTime * 5, 0,0);
        }
    }
    public void Move()
    {
        nav.SetPath(path);
    }
    /// <summary>
    /// 判断玩家能否移动到目标点   能抵达  返回true  否则为false
    /// </summary>
    /// <param name="target">目标点 </param>
    /// <returns></returns>
    public bool CalculatePath(Vector3 target)
    {
        nav.CalculatePath(target, path);
        Debug.Log(path.status);
        switch (path.status)
        {
            case NavMeshPathStatus.PathComplete:
                return true;
            case NavMeshPathStatus.PathPartial:
                return false;
            case NavMeshPathStatus.PathInvalid:
                return false;
        }
        return false;
    }
}
