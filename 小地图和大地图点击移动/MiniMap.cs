using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class MiniMap : MonoBehaviour,IPointerClickHandler
{
    public List<GameObject> NPCList;

    private int count = 20;//每隔20帧  刷新一次
    public void OnPointerClick(PointerEventData eventData)
    {
        UIMapManager.Ins.ShowMap();
    }
    private float ratio;
    private RectTransform MapRect;
    private GameObject jt;
    private Transform maskParent;
    private Vector3 mapOffset;
    private Vector3 v;
    private Vector3 rot;
    private List<GameObject> yellowPoint;
    // Start is called before the first frame update
    void Start()
    {
        MapRect = transform.Find("Mask").Find("MapTexture").GetComponent<RectTransform>();
        maskParent = MapRect.parent;
        ratio = MapRect.sizeDelta.x / 100;
        jt = transform.Find("JT").gameObject;
        yellowPoint = new List<GameObject>();
        CreateYellowPoint(NPCList.Count);
    }
   
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
    private void Update()
    {
        UpdateMap();
        if (jt!=null)
        {
            rot.Set(0, 0, PlayerMove.Ins.transform.eulerAngles.y * -1);
            jt.transform.eulerAngles = rot;
        }
    }
    public void UpdateMap()
    {
        count -= 1;
        if (count>0)
        {
            return;
        }
        count = 20;
        Transform player = PlayerMove.Ins.transform;
        Vector3 mapTexturePos = GetMapPos(player.position);
        MapRect.localPosition =mapTexturePos*-1;
        RefreshPointPos();
    }
    private Vector3 GetMapPos(Vector3 pos)
    {
        float x = pos.x * ratio;
        float y = pos.z * ratio;
        v.Set(x, y,0);
        return v;
    }
    private void CreateYellowPoint(int count)
    {
        GameObject prefab= ResourcesManager.Ins.Load<GameObject>("Image");
        for (int i = 0; i < count; i++)
        {
            GameObject a = Instantiate(prefab, MapRect);
            yellowPoint.Add(a);
        }
    }
    private void RefreshPointPos()
    {
        for (int i = 0; i < NPCList.Count; i++)
        {
            GameObject npc = NPCList[i];
            GameObject point = yellowPoint[i];
            point.transform.localPosition = GetMapPos(npc.transform.position);
        }
    }
}
