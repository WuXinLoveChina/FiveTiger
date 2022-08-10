using UnityEngine;
using System.Collections;
using Games.LogicObj;
using Games.GlobeDefine;
public class SGAutoFightBtn : UIControllerBase<SGAutoFightBtn>  {

    public GameObject m_BtnAutoBegin;
    public GameObject m_BtnAutoStop;

    public static SGAutoFightBtn Instance;

    void Awake() 
    {
        Instance = this;
    }
    void Start() 
    {
        ObjManager.Instance.MainPlayerOnLoad += UpdateAutoFightBtnState;
    }

    public void OnDestroy()
    {
        ObjManager.Instance.MainPlayerOnLoad -= UpdateAutoFightBtnState;
    }
    // 直接调用自动战斗
    void OnDoAutoFightClick()
    {
        Obj_MainPlayer mainPalyer = Singleton<ObjManager>.Instance.MainPlayer;
        if (null == mainPalyer)
        {
            return;
        }
		//pvp  和帮站不能挂机
//		if ((int)GameDefine_Globe.SCENE_DEFINE.SCENE_TIANXIAWUSHUANG == GameManager.gameManager.RunningScene ||
//						(int)GameDefine_Globe.SCENE_DEFINE.SCENE_RICHANGJUEDOU == GameManager.gameManager.RunningScene ||
//						(int)GameDefine_Globe.SCENE_DEFINE.SCENE_GUILDWAR == GameManager.gameManager.RunningScene)
//		{
//			return;
//		}
        mainPalyer.EnterAutoCombat();
        UpdateAutoFightBtnState();
    }

    void OnDoAutoStopFightClick()
    {
        Obj_MainPlayer mainPalyer = Singleton<ObjManager>.Instance.MainPlayer;
        if (null == mainPalyer)
        {
            return;
        }
        mainPalyer.LeveAutoCombat();
        UpdateAutoFightBtnState();
    }

    public void UpdateAutoFightBtnState()
    {
        Obj_MainPlayer mainPalyer = Singleton<ObjManager>.Instance.MainPlayer;
        m_BtnAutoStop.SetActive(mainPalyer.IsOpenAutoCombat);
        m_BtnAutoBegin.SetActive(!mainPalyer.IsOpenAutoCombat);
    }
}
