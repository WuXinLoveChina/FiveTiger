using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Module.Log;
using Games.GlobeDefine;
using GCGame.Table;
using Games.LogicObj;

public class WorldMapWindow : MonoBehaviour {

    public GameObject[] m_MapTexs;
    public int[] m_MapIDs;
    public Vector3[] m_RolePos;
    public GameObject m_SprRole;

    private List<GameObject> m_objTeamSprite = new List<GameObject>();
	// Use this for initialization
	void OnEnable () {

        for (int i = 0; i < m_objTeamSprite.Count; i++)
        {
            GameObject.Destroy(m_objTeamSprite[i]);
        }
        m_objTeamSprite.Clear();
        Obj_MainPlayer mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (null == mainPlayer)
        {
            return;
        }

        TeamMember[] curTeamMember = GameManager.gameManager.PlayerDataPool.TeamInfo.teamMember;
        bool isTeamMode = false;
        for (int i = 0; i < curTeamMember.Length; i++)
        {
            if (curTeamMember[i].IsValid())
            {
                isTeamMode = true;
                break;
            }
        }
        m_SprRole.SetActive(false);
        for (int i = 0; i < m_MapIDs.Length; i++)
        {
            m_MapTexs[i].SetActive(SceneData.ReachedScenes.Contains(m_MapIDs[i]));

            if (!isTeamMode)
            {
                if (m_MapIDs[i] == GameManager.gameManager.RunningScene)
                {
                    m_SprRole.transform.localPosition = m_RolePos[i];
                    m_SprRole.SetActive(true);
                }
            }
            else
            {
                int curMapMemberCount = 0;
                for (int curTeamMemberIndex = 0; curTeamMemberIndex < curTeamMember.Length; curTeamMemberIndex++)
                {
                    int curSceneClassID = curTeamMember[curTeamMemberIndex].SceneClassID;
                    if (curSceneClassID != m_MapIDs[i])
                    {
                        continue;
                    }

                    GameObject newRoleObj = GameObject.Instantiate(m_SprRole) as GameObject;
                    newRoleObj.transform.parent = m_SprRole.transform.parent;
                    newRoleObj.transform.localScale = Vector3.one;
                    Vector3 curMapPos = m_RolePos[i];
                    curMapPos.x += curMapMemberCount * 16;
                    newRoleObj.transform.localPosition = curMapPos;
                    newRoleObj.SetActive(true);
                    if (curTeamMember[curTeamMemberIndex].TeamJob == 0)
                    {
                        newRoleObj.GetComponent<UITexture>().color = Color.red;
                    }

                    m_objTeamSprite.Add(newRoleObj);
                    curMapMemberCount++;
                }
            }
            
        }
	}
    
    private int m_curShowSceneID = -1;
    void OnMapItemClick(GameObject button)
    {
        m_curShowSceneID = -1;
        for (int index = 0; index < m_MapTexs.Length; index++)
        {
            if (m_MapTexs[index].name == button.name)
            {
                m_curShowSceneID = m_MapIDs[index];
                break;
            }
        }
        if (m_curShowSceneID >= 0 && Singleton<ObjManager>.GetInstance().MainPlayer != null && m_curShowSceneID != GameManager.gameManager.RunningScene)
        {
            if (Singleton<ObjManager>.GetInstance().MainPlayer.IsInGuildBusiness())
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{3937}");
                return;
            }

            if (TeleportPoint.IsCanPK(m_curShowSceneID) && !TeleportPoint.IsIncPKValue(m_curShowSceneID))
            {
                MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{2672}"), "", EnterNonePKValueSceneOK);
            }
            else
            {
                EnterNonePKValueSceneOK();
            }
            
        }
        else
        {
            //MessageBoxLogic.OpenOKBox(2214, 1000);
        }
    }

    void EnterNonePKValueSceneOK()
    {
        Tab_SceneClass tabSceneClass = TableManager.GetSceneClassByID(m_curShowSceneID, 0);
        if (null == tabSceneClass)
        {
            return;
        }
        
        MessageBoxLogic.OpenOKCancelBox(StrDictionary.GetClientDictionaryString("#{1644}", tabSceneClass.Name), "", DoTeleport);
    }

    void DoTeleport()
    {
        if (GameManager.gameManager.PlayerDataPool.Money.GetMoney_Coin() < 1000)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{3036}");
            return;
        }

        Tab_SceneClass tabSceneClass = TableManager.GetSceneClassByID(m_curShowSceneID, 0);
        if (null == tabSceneClass)
        {
            return;
        }
		SkillProgressLogic.Instance ().PlaySkillProgress (SkillProgressLogic.ProgressModel.ORDERMODEL, 3.0f);
		Obj_MainPlayer main = Singleton<ObjManager>.Instance.MainPlayer;
		main.StopMove ();
		main.MoveTarget = null;
		main.MoveTo (main.gameObject.transform.position, main.gameObject, 0.0f);
		Invoke ("DoChange",1.0f);
		InvokeRepeating ("DoUpdate",0,0.1f);
        UIManager.CloseUI(UIInfo.WorldMapWindow);
    }

    void OnCloseClick()
    {
        UIManager.CloseUI(UIInfo.WorldMapWindow);
        UIManager.ShowUI(UIInfo.SceneMapRoot);
    }
	void DoChange()
	{
		SceneData.RequestChangeScene((int)CG_REQ_CHANGE_SCENE.CHANGETYPE.WORLDMAP, 0, m_curShowSceneID, 0);
	}
	void DoUpdate()
	{
		Obj_MainPlayer main = Singleton<ObjManager>.Instance.MainPlayer;
		if (main.CurObjAnimState != GameDefine_Globe.OBJ_ANIMSTATE.STATE_NORMOR)
		{
			CancelInvoke("DoChange");
			CancelInvoke("DoUpdate");
			SkillProgressLogic.Instance().CloseWindow();
		
		}
	}
}
