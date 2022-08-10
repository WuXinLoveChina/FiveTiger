using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 点击地图，移动
/// </summary>
public class UIMap : MonoBehaviour, IPointerClickHandler
{
    //绿色箭头，代表方向
    //private Sprite directionSprite;
    private float sceneMapWidth = 100;
    private float sceneMapLength = 100;
    private float UIMapWidth;
    private float UIMapLength;
    public float ratio;//比例

    private RectTransform rectTransform;
    private PlayerMove player;

    private GameObject targetSprite;//目标  图片
    private GameObject jt;//代表玩家的箭头
    private Vector3 targetPos;
    private bool isRun = false;
    public List<GameObject> npcList;
    private List<GameObject> yellowPoint;

    private Button close;
    void Start()
    {
        yellowPoint = new List<GameObject>();
        rectTransform = GetComponent<RectTransform>();
        player = GameObject.Find("Player").GetComponent<PlayerMove>();
        targetSprite = transform.Find("Image").gameObject;
        jt = transform.Find("JT").gameObject;
        close = transform.Find("Close").GetComponent<Button>();
        close.onClick.AddListener(() =>
        {
            UIMapManager.Ins.ShowMini();
        });

        UIMapWidth = rectTransform.sizeDelta.x;
        UIMapLength = rectTransform.sizeDelta.y;
        ratio = UIMapLength / sceneMapLength;//UI / Scene
        jt.SetActive(true);
        CreateYellowPoint(npcList.Count);
    }
    private void Update()
    {
        if (isRun)
        {
            if (Vector3.Distance(player.transform.position, targetPos) < 2)
            {
                targetSprite.SetActive(false);
                isRun = false;
            }
        }
        if (jt != null)
        {
            jt.transform.localPosition = GetMapLocalPos();
            jt.transform.eulerAngles = GetFX();
            RefreshPointPos();
        }
    }
    /// <summary>
    /// 点击地图
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 screenPos = eventData.position; // UGUI的坐标和屏幕坐标是 1：1
        Vector3 localPos = transform.InverseTransformPoint(screenPos);
        targetPos = GetScenePos(localPos);
        
        if (!player.CalculatePath(targetPos))
        {
            return;
        }
        player.Move();
        targetSprite.transform.localPosition = localPos;
        isRun = true;
        targetSprite.SetActive(true);
        targetSprite.transform.localPosition = localPos;
    }
    private Vector3 GetScenePos(Vector3 v)
    {
        float x = v.x / ratio;
        float z = v.y / ratio;
        return new Vector3(x, 0, z);
    }
    private Vector3 GetMapLocalPos()
    {
        Vector3 v = player.transform.position;
        float x = v.x * ratio;
        float y = v.z * ratio;
        v = Vector3.zero;
        v.Set(x, y, 0);
        return v;
    }
    /// <summary>
    ///  获取箭头的方向
    ///  之所以要乘于-1，是因为，3D场景中，给物体绕Y轴旋转 大于0的度数，它是向右转
    ///  在UI中，给UI绕Z轴旋转 大于0的度数，它是向左转
    /// </summary>
    private Vector3 GetFX()
    {
        Vector3 v = player.transform.eulerAngles;
        float z = v.y*-1;
        v = Vector3.zero;
        v.Set(0, 0, z);
        return v;
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
    private void CreateYellowPoint(int count)
    {
        GameObject prefab = ResourcesManager.Ins.Load<GameObject>("Image");
        for (int i = 0; i < count; i++)
        {
            GameObject a = Instantiate(prefab, rectTransform);
            yellowPoint.Add(a);
        }
    }
    private void RefreshPointPos()
    {
        for (int i = 0; i < npcList.Count; i++)
        {
            GameObject npc = npcList[i];
            GameObject point = yellowPoint[i];
            point.transform.localPosition = GetMapPos(npc.transform.position);
        }
    }
    private Vector3 GetMapPos(Vector3 pos)
    {
        float x = pos.x * ratio;
        float y = pos.z * ratio;
        pos = Vector3.zero;
        pos.Set(x, y,0);
        return pos;
    }
}
