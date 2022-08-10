using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using Games.SkillModle;
using GCGame.Table;
using UnityEngine;
using System.Collections;
using Games.LogicObj;
using Games.GlobeDefine;
using Module.Log;
public enum SKILLBAR
{
    MAXSKILLBARNUM =6,
}
public struct SkillBarInfo
{
    public void CleanUp()
    {
        buttonInfo = null;
        CDPicInfo = null;
        IconInfo = null;
        CDEffect = null;
        SkillIndex = -1;
        IsPress = false;
    }

    public GameObject buttonInfo;
    public UISprite CDPicInfo;
    public UISprite IconInfo;
    public UISpriteAnimation CDEffect;
    public int SkillIndex;
    public bool IsPress;
}
public class SkillBarLogic : MonoBehaviour
{
    public GameObject m_FirstChild;

    public UISprite m_Skill1CDPic;
    public UISprite m_Skill2CDPic;
    public UISprite m_Skill3CDPic;
    public UISprite m_Skill4CDPic;
    public UISprite m_Skill5CDPic;
    public UISprite m_Skill6CDPic;
    public UISprite m_SkillXPCPPic;

    public UISprite m_Skill1IconPic;
    public UISprite m_Skill2IconPic;
    public UISprite m_Skill3IconPic;
    public UISprite m_Skill4IconPic;
    public UISprite m_Skill5IconPic;
    public UISprite m_Skill6IconPic;
    public UISprite m_SkillXPIconPic;

    public GameObject m_Skill1Bt;
    public GameObject m_Skill2Bt;
    public GameObject m_Skill3Bt;
    public GameObject m_Skill4Bt;
    public GameObject m_Skill5Bt;
    public GameObject m_Skill6Bt;
    public GameObject m_SkillAttackBt;
    public GameObject m_SkillXPBt;

    public UISpriteAnimation m_Skill1CDEffect;
    public UISpriteAnimation m_Skill2CDEffect;
    public UISpriteAnimation m_Skill3CDEffect;
    public UISpriteAnimation m_Skill4CDEffect;
    public UISpriteAnimation m_Skill5CDEffect;
    public UISpriteAnimation m_Skill6CDEffect;
    
    public UISprite m_SkillXPEnergySprite;
    public GameObject m_SkillXPEnergyEffectRotation;  

    private static SkillBarLogic m_Instance = null;
    public static SkillBarLogic Instance()
    {
        return m_Instance;
    }

    private SkillBarInfo[] m_MySkillBarInfo;
    public SkillBarInfo[] MySkillBarInfo
    {
        get { return m_MySkillBarInfo; }
        set { m_MySkillBarInfo = value; }
    }
    private bool m_bFirstUpdate = false;
    private bool m_bSetSkillBarSuccess = false;
    // 新手指引
    private int m_NewPlayerGuide_Step = 0;
    public int NewPlayerGuide_Step
    {
        get { return m_NewPlayerGuide_Step; }
        set { m_NewPlayerGuide_Step = value; }
    }


    //特殊效果ID
    private int m_NewSkillEffectID  = 60;         //新技能学会特效
    private int m_XPSkillEffectID   = 64;         //XP技能学会特效
    //private int m_SkillCDZeroEffectID = 86;         //技能CD清零特效
   
    //public TweenAlpha m_SkillXPTween;
    //public List<TweenAlpha> m_FoldTween;
    void Awake()
    {
       m_Instance = this;
    }
    // Use this for initialization
    void Start()
    {
//#if UNITY_ANDROID		
//        m_FirstChild.SetActive(false);
//#else
//		m_FirstChild.SetActive(true);
//#endif
        m_MySkillBarInfo=new SkillBarInfo[(int)SKILLBAR.MAXSKILLBARNUM];
        for (int i = 0; i < (int)SKILLBAR.MAXSKILLBARNUM; i++)
        {
            m_MySkillBarInfo[i] =new SkillBarInfo();
            m_MySkillBarInfo[i].CleanUp();
        }
        m_SkillXPEnergySprite.fillAmount = 0;
        m_SkillXPEnergyEffectRotation.transform.localRotation = Quaternion.AngleAxis(0, Vector3.forward);
        m_SkillXPEnergyEffectRotation.SetActive(false);
        m_SkillXPIconPic.spriteName = "";
        //由于创建UI的时候玩家没有选中任何目标，所以攻击按钮暂为未选取状态
      //  UpdateAttackButtonState(CharacterDefine.SELECT_TARGET_TYPE.SELECT_TARGET_NONE);

        InitMySkillBarInfo(0, m_Skill1Bt, m_Skill1CDPic, m_Skill1IconPic, -1, m_Skill1CDEffect);
        InitMySkillBarInfo(1, m_Skill2Bt, m_Skill2CDPic, m_Skill2IconPic, -1, m_Skill2CDEffect);
        InitMySkillBarInfo(2, m_Skill3Bt, m_Skill3CDPic, m_Skill3IconPic, -1, m_Skill3CDEffect);
        InitMySkillBarInfo(3, m_Skill4Bt, m_Skill4CDPic, m_Skill4IconPic, -1, m_Skill4CDEffect);
        InitMySkillBarInfo(4, m_Skill5Bt, m_Skill5CDPic, m_Skill5IconPic, -1, m_Skill5CDEffect);
        InitMySkillBarInfo(5, m_Skill6Bt, m_Skill6CDPic, m_Skill6IconPic, -1, m_Skill6CDEffect);
        if (m_SkillXPBt.activeInHierarchy)
        {
            EffectLogic effectLogic = m_SkillXPBt.GetComponent<EffectLogic>();
            if (null == effectLogic)
            {
                effectLogic = m_SkillXPBt.AddComponent<EffectLogic>();
                effectLogic.InitEffect(m_SkillXPBt);
            }
            if (null != effectLogic)
            {
                effectLogic.PlayEffect(m_XPSkillEffectID);
            }
        }
        //更新下 技能按钮信息
        UpdateSkillBarInfo();

        if (GameManager.gameManager.PlayerDataPool.ForthSkillFlag == true)
        {
            NewPlayerGuide(4);
        }
    }

    void InitMySkillBarInfo(int nIndex, GameObject _button, UISprite _CDPic, UISprite _IconPic,int _SkillIndex, UISpriteAnimation _CDEffect)
    {
        if (nIndex>=0 && nIndex<(int)SKILLBAR.MAXSKILLBARNUM)
        {
            m_MySkillBarInfo[nIndex].buttonInfo=_button;
            m_MySkillBarInfo[nIndex].CDPicInfo=_CDPic;
            m_MySkillBarInfo[nIndex].IconInfo=_IconPic;
            m_MySkillBarInfo[nIndex].SkillIndex=_SkillIndex;
            m_MySkillBarInfo[nIndex].CDPicInfo.gameObject.SetActive(false);
            m_MySkillBarInfo[nIndex].IconInfo.spriteName = "";
            m_MySkillBarInfo[nIndex].CDEffect = _CDEffect;
            m_MySkillBarInfo[nIndex].CDEffect.gameObject.SetActive(false);
        }
    }
    public  void UpdateSkillBarInfo()
    {
        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer == null)
        {
            return;
        }
        m_bFirstUpdate = true;
        //设置XP图标
        if (m_SkillXPIconPic.spriteName=="")
        {
            for (int _skillIndex = 0; _skillIndex < _mainPlayer.OwnSkillInfo.Length; _skillIndex++)
            {
                Tab_SkillEx _skillEx = TableManager.GetSkillExByID(_mainPlayer.OwnSkillInfo[_skillIndex].SkillId, 0);
                if (_skillEx != null)
                {
                    Tab_SkillBase _skillBase = TableManager.GetSkillBaseByID(_skillEx.BaseId, 0);
                    if (_skillBase != null &&(_skillBase.SkillClass&(int)SKILLCLASS.XP)!=0)
                    {
                        //设置XP图标
                        m_SkillXPIconPic.spriteName = _skillBase.Icon;
                        m_SkillXPIconPic.MakePixelPerfect();
                        XPNewPlayGuid();
                        break;
                    }
                }
            } 
        }
        //读取技能栏配置
        m_bSetSkillBarSuccess = false;
        Dictionary<string, SkillBarInfo[]> _skillBarSetMap= UserConfigData.GetSkillBarSetInfo();
        if (_skillBarSetMap != null && _skillBarSetMap.ContainsKey(_mainPlayer.GUID.ToString()))
        {
            SkillBarInfo[] _skillBarSetInfo = _skillBarSetMap[_mainPlayer.GUID.ToString()];
            for (int _skillBarIndex = 0; _skillBarIndex < m_MySkillBarInfo.Length; _skillBarIndex++)
            {
                int nSkillIndex = _skillBarSetInfo[_skillBarIndex].SkillIndex;
                if (nSkillIndex >= 0 && nSkillIndex < _mainPlayer.OwnSkillInfo.Length)
                {
                    if (_mainPlayer.OwnSkillInfo[nSkillIndex].SkillId != -1)
                    {
                        SetSkillBarInfo(_skillBarIndex, nSkillIndex);
                    }
                    else // 保存的技能不存在 清掉
                    {
                        _skillBarSetInfo[_skillBarIndex].SkillIndex = -1;
                        // 保存配置
                        UserConfigData.AddSkillBarSetInfo(_mainPlayer.GUID.ToString(), m_MySkillBarInfo);
                    }
                }               
            }
        }
        if (m_bSetSkillBarSuccess == false) //配置读取失败了 给一个默认的配置
        {
            for (int _skillIndex = 0; _skillIndex < _mainPlayer.OwnSkillInfo.Length; _skillIndex++)
            {
                int _skillID = _mainPlayer.OwnSkillInfo[_skillIndex].SkillId;
                if (_skillID > 0)
                {
                    Tab_SkillEx _skillExinfo = TableManager.GetSkillExByID(_skillID, 0);
                    if (_skillExinfo != null)
                    {
                        Tab_SkillBase _skillBase = TableManager.GetSkillBaseByID(_skillExinfo.BaseId, 0);
                        if (_skillBase != null )
                        {
                            if ((_skillBase.SkillClass&(int)SKILLCLASS.AUTOREPEAT)==0 &&
                                (_skillBase.SkillClass&(int)SKILLCLASS.XP)==0)
                            {
                                for (int _skillBarIndex = 0; _skillBarIndex < (int)SKILLBAR.MAXSKILLBARNUM; _skillBarIndex++)
                                {
                                    //找到空位了
                                    if (m_MySkillBarInfo[_skillBarIndex].SkillIndex == -1)
                                    {
                                        SetSkillBarInfo(_skillBarIndex, _skillIndex);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //保存配置
            UserConfigData.AddSkillBarSetInfo(_mainPlayer.GUID.ToString(), m_MySkillBarInfo);
        }

        if (Turntable.Instance() != null)
        {
            Turntable.Instance().UpdateSkillBarShow();
        }
    }
    public void SetSkillBarInfo(int _skillBarIndex, int _skillIndex)
    {
        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer == null)
        {
            return;
        }
        if (_skillBarIndex >= 0 && _skillBarIndex < (int)SKILLBAR.MAXSKILLBARNUM)
        {
            if (_skillIndex >= 0 && _skillIndex<_mainPlayer.OwnSkillInfo.Length)
            {
                Tab_SkillEx _skillEx = TableManager.GetSkillExByID(_mainPlayer.OwnSkillInfo[_skillIndex].SkillId, 0);
                if (_skillEx != null)
                {
                    Tab_SkillBase _skillBase = TableManager.GetSkillBaseByID(_skillEx.BaseId, 0);
                    if (_skillBase != null)
                    {
                        m_MySkillBarInfo[_skillBarIndex].SkillIndex = _skillIndex;
                        m_MySkillBarInfo[_skillBarIndex].CDPicInfo.fillAmount = 0;
                        m_MySkillBarInfo[_skillBarIndex].CDPicInfo.gameObject.SetActive(false);
                        m_MySkillBarInfo[_skillBarIndex].IconInfo.spriteName = _skillBase.Icon;
                        m_MySkillBarInfo[_skillBarIndex].IconInfo.MakePixelPerfect();
                        m_bSetSkillBarSuccess = true;
                    }
                }
            }
            else
            {
                m_MySkillBarInfo[_skillBarIndex].SkillIndex = -1;
                m_MySkillBarInfo[_skillBarIndex].CDPicInfo.fillAmount = 0;
                m_MySkillBarInfo[_skillBarIndex].CDPicInfo.gameObject.SetActive(false);
                m_MySkillBarInfo[_skillBarIndex].IconInfo.spriteName ="";
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer == null)
        {
            return;
        }
        if (m_bFirstUpdate ==false)
        {
            UpdateSkillBarInfo();
        }
        for (int _skillBarIndex = 0; _skillBarIndex < m_MySkillBarInfo.Length;_skillBarIndex++)
        {
            if (m_MySkillBarInfo[_skillBarIndex].IsPress)
            {
                if (m_MySkillBarInfo[_skillBarIndex].buttonInfo.activeInHierarchy == false)
                {
                    ReleaseSkill(m_MySkillBarInfo[_skillBarIndex].buttonInfo);
                }
                else if (Input.GetMouseButton((int)ProcessInput.MOUSE_BUTTON.MOUSE_BUTTON_LEFT) ==false) //鼠标没按下 释放
                {
                    ReleaseSkill(m_MySkillBarInfo[_skillBarIndex].buttonInfo);
                }
            }
            //播放冷却动画
            int nSkillIndex = m_MySkillBarInfo[_skillBarIndex].SkillIndex;
            if (nSkillIndex >= 0 && nSkillIndex <_mainPlayer.OwnSkillInfo.Length)
            {
                int _skillId = _mainPlayer.OwnSkillInfo[nSkillIndex].SkillId;
                int _CDTime = _mainPlayer.OwnSkillInfo[nSkillIndex].CDTime;
                //先走公共CD
                if (_skillId >0 && _mainPlayer.SkillPublicTime >0)
                {
                    if (m_MySkillBarInfo[_skillBarIndex].CDPicInfo.fillAmount == 0)
                    {
                        m_MySkillBarInfo[_skillBarIndex].CDPicInfo.fillAmount = 1;
                        m_MySkillBarInfo[_skillBarIndex].CDPicInfo.gameObject.SetActive(true);
                    }
                    int _publicCDID =(int)SKILLDEFINE.PUBLICCDID;
                    Tab_CoolDownTime _CDTimeInfo = TableManager.GetCoolDownTimeByID(_publicCDID, 0);
                    if (_CDTimeInfo != null)
                    {
                        int _totalCDTime = _CDTimeInfo.CDTime;
                        if (_totalCDTime > 0)
                        {
                            m_MySkillBarInfo[_skillBarIndex].CDPicInfo.fillAmount = (_mainPlayer.SkillPublicTime * 1.0f) / _totalCDTime;
                        }
                    }
                }
                else if (_skillId > 0&&_CDTime > 0)
                {
                    if (m_MySkillBarInfo[_skillBarIndex].CDPicInfo.fillAmount == 0)
                    {
                        m_MySkillBarInfo[_skillBarIndex].CDPicInfo.fillAmount = 1;
                        m_MySkillBarInfo[_skillBarIndex].CDPicInfo.gameObject.SetActive(true);
                    }
                    //技能的总CD时间
                    Tab_SkillEx _skillEx = TableManager.GetSkillExByID(_skillId, 0);
                    if (_skillEx !=null)
                    {
                        Tab_CoolDownTime _CDTimeInfo = TableManager.GetCoolDownTimeByID(_skillEx.CDTimeId, 0);
                        if (_CDTimeInfo!=null)
                        {
                            int _totalCDTime = _CDTimeInfo.CDTime;
                            if (_totalCDTime >0)
                            {
                                m_MySkillBarInfo[_skillBarIndex].CDPicInfo.fillAmount = (_CDTime * 1.0f)/_totalCDTime; 
                            }
                        }
                    }
                    
                }
                else if (m_MySkillBarInfo[_skillBarIndex].CDPicInfo.gameObject.activeInHierarchy)
                {
                    PlayCDZeroEffect(m_MySkillBarInfo[_skillBarIndex].CDEffect);
                    m_MySkillBarInfo[_skillBarIndex].CDPicInfo.fillAmount = 0;
                    m_MySkillBarInfo[_skillBarIndex].CDPicInfo.gameObject.SetActive(false);
                }
            }
        }
    }

    // xp能量槽挪到玩家头像计算
    public void ChangeXPEnergy(int nValue,int maxXP)
    {
        // 增加怒气 改变精灵的fill amount属性 注意范围是0.3~0.95 按比例映射过去
        if (maxXP ==0)
        {
            return;
        }
        m_SkillXPEnergyEffectRotation.SetActive(true);

        float nFillAmount = (float)nValue / (float)maxXP * 0.65f + 0.3f;
        m_SkillXPEnergySprite.fillAmount = nFillAmount;
        
        // 更新怒气特效位置 绕Z轴旋转m_SkillXPEnergyEffectRotation 范围:10~-200
        float nAngel = 10 - (float)nValue / (float)maxXP * 210;
        m_SkillXPEnergyEffectRotation.transform.localRotation = Quaternion.AngleAxis(nAngel, Vector3.forward);

		int nlevel = GameManager.gameManager.PlayerDataPool.MainPlayerBaseAttr.Level;
		if (nValue >= maxXP&&nlevel>=13)
        {
            m_SkillXPCPPic.alpha = 0;
            PlayXPActiveEffect(true);
        }
        else
        {
            if (m_SkillXPCPPic.alpha == 0)
            {
                m_SkillXPCPPic.alpha = 1;
                PlayXPActiveEffect(false);
            }
        }
    }
    void OnEnable()
    {
        if (m_SkillXPBt.activeInHierarchy)
        {
            EffectLogic effectLogic = m_SkillXPBt.GetComponent<EffectLogic>();
            if (null == effectLogic)
            {
                effectLogic = m_SkillXPBt.AddComponent<EffectLogic>();
                effectLogic.InitEffect(m_SkillXPBt);
            }
            if (null != effectLogic)
            {
                effectLogic.PlayEffect(m_XPSkillEffectID);
            }
        }

        if (GameManager.gameManager.PlayerDataPool.ForthSkillFlag == true)
        {
            NewPlayerGuide(4);
        }
    }
    void OnDisable()
    {
        if (m_MySkillBarInfo !=null)
        {
            for (int _skillBarIndex = 0; _skillBarIndex < m_MySkillBarInfo.Length; _skillBarIndex++)
            {
                if (m_MySkillBarInfo[_skillBarIndex].IsPress)
                {
                    ReleaseSkill(m_MySkillBarInfo[_skillBarIndex].buttonInfo);
                }
            }
        }
    }
    void OnDestroy()
    {
        m_Instance = null;
    }

    public void PlayNewSkillEffect(GameObject button)
    {
        if (button.activeInHierarchy ==false)
        {
            return;
        }
        EffectLogic effectLogic = button.GetComponent<EffectLogic>();
        if (null == effectLogic)
        {
            effectLogic = button.AddComponent<EffectLogic>();
            effectLogic.InitEffect(button);
        }
        if (null != effectLogic)
        {
            effectLogic.PlayEffect(m_NewSkillEffectID);
        }
    }

    public void PlayXPActiveEffect(bool bShow)
    {
        EffectLogic effectLogic = m_SkillXPBt.GetComponent<EffectLogic>();
        if (null == effectLogic)
        {
            effectLogic = m_SkillXPBt.AddComponent<EffectLogic>();
            effectLogic.InitEffect(m_SkillXPBt);
        }
        if (null != effectLogic)
        {
            Tab_SceneClass _sceneClassInfo = TableManager.GetSceneClassByID(GameManager.gameManager.RunningScene, 0);
            if (bShow && _sceneClassInfo != null && _sceneClassInfo.IsCanUseXp ==1)
            {
                m_SkillXPBt.transform.parent.gameObject.SetActive(true);
                //m_SkillXPTween.Play();
                if (null != effectLogic && m_SkillXPBt.activeInHierarchy)
                {
                    effectLogic.PlayEffect(m_XPSkillEffectID);
                }
            }
            else
            {
                effectLogic.StopEffect(m_XPSkillEffectID);
                //m_SkillXPTween.Reset();
                m_SkillXPBt.transform.parent.gameObject.SetActive(false);
            }
        }
    }

    void PlayCDZeroEffect(UISpriteAnimation CDEffect)
    {
        CDEffect.gameObject.SetActive(true);
        CDEffect.Reset();
    }

    public void UseSkill(GameObject button)
    {
        // 正在转动 此时不响应技能
        if (Turntable.Instance() != null && Turntable.Instance().Turning)
        {
            return;
        }
        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer == null)
        {
            LogModule.DebugLog("_mainPlayer is Null");
            return;
        }
        if (button.name == "AttackButton")
        {
            _mainPlayer.LastClickAttackBtTime = Time.time;
            _mainPlayer.UseSkillOpt(_mainPlayer.OwnSkillInfo[0].SkillId, button);
        }
        else if (button.name == "SkillXPButton" )
        {
            _mainPlayer.UseSkillOpt(_mainPlayer.OwnSkillInfo[1].SkillId, button);
        }
        else 
        {
            for (int i = 0; i < m_MySkillBarInfo.Length; i++)
            {
                if (m_MySkillBarInfo[i].buttonInfo.name ==button.name)
                {
                    int _skillIndex = m_MySkillBarInfo[i].SkillIndex;
                    if (_skillIndex>=0 && _skillIndex<_mainPlayer.OwnSkillInfo.Length)
                    {
                        _mainPlayer.UseSkillOpt(_mainPlayer.OwnSkillInfo[_skillIndex].SkillId, button);
                    }
                    break;
                }
            }
        }

        // 新手指引 暂时写这儿
		if (button.name == "SkillXPButton" && NewPlayerGuidLogic.Instance() != null && NewPlayerGuidLogic.Instance().CurShowType == "SkillXP")
		{
			PlayerPreferenceData.XPNewPlayerGuideFlag = true;
			m_NewPlayerGuide_Step = 0;
			NewPlayerGuidLogic.CloseWindow();
		}
		else if (button.name == "AttackButton" && NewPlayerGuidLogic.Instance() != null && NewPlayerGuidLogic.Instance().CurShowType == "Attack")
		{
			NewPlayerGuidLogic.CloseWindow();
		}
		else if (button.name == "Skill2Button" && NewPlayerGuidLogic.Instance() != null && NewPlayerGuidLogic.Instance().CurShowType == "Skill")
		{
			m_NewPlayerGuide_Step = 0;
			NewPlayerGuidLogic.CloseWindow();
		}
    }
   
    public void PressSkill(GameObject button)
    {
        //先松开
        if (m_MySkillBarInfo != null)
        {
            for (int _skillBarIndex = 0; _skillBarIndex < m_MySkillBarInfo.Length; _skillBarIndex++)
            {
                if (m_MySkillBarInfo[_skillBarIndex].IsPress)
                {
                    ReleaseSkill(m_MySkillBarInfo[_skillBarIndex].buttonInfo);
                }
            }

        }  
        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer == null)
        {
            LogModule.DebugLog("_mainPlayer is Null");
            return;
        }
        for (int i = 0; i < m_MySkillBarInfo.Length; i++)
        {
            if (m_MySkillBarInfo[i].buttonInfo.name == button.name)
            {
                m_MySkillBarInfo[i].IsPress = true;
                int _skillIndex = m_MySkillBarInfo[i].SkillIndex;
                if (_skillIndex >= 0 && _skillIndex < _mainPlayer.OwnSkillInfo.Length)
                {
                    //播放范围特效
                    _mainPlayer.CurPressSkillId = _mainPlayer.OwnSkillInfo[_skillIndex].SkillId;
                    Tab_SkillEx _skillEx = TableManager.GetSkillExByID(_mainPlayer.CurPressSkillId, 0);
                    if (_skillEx != null)
                    {
                        int _rangeEffectType = _skillEx.RangeEffectType;
                        if (_rangeEffectType != -1) 
                        {
                            //目标播放
                            if (_skillEx.RangeEffectTarType ==(int)SKILLRANGEEFFECTTAR.SELECTTARGET)
                            {
                                if (_mainPlayer.SelectTarget)
                                {
                                    _mainPlayer.SelectTarget.PlaySkillRangeEffect();
                                }
                            }
                            else //玩家自己播放
                            {
                                _mainPlayer.PlaySkillRangeEffect();
                            }
                        }
                        break;
                    }
                }
            }
        }
    }
    public void ReleaseSkill(GameObject button)
    {
        Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (_mainPlayer == null)
        {
            LogModule.DebugLog("_mainPlayer is Null");
            return;
        }
        //停止范围特效 
        if (_mainPlayer.CurPressSkillId !=-1)
        {
            for (int i = 0; i < m_MySkillBarInfo.Length; i++)
            {
                if (m_MySkillBarInfo[i].buttonInfo.name == button.name)
                {
                    m_MySkillBarInfo[i].IsPress = false;
                    int _skillIndex = m_MySkillBarInfo[i].SkillIndex;
                    if (_skillIndex >= 0 && _skillIndex < _mainPlayer.OwnSkillInfo.Length)
                    {
                        Tab_SkillEx _skillEx = TableManager.GetSkillExByID(_mainPlayer.CurPressSkillId, 0);
                        if (_skillEx != null)
                        {
                            int _rangeEffectType = _skillEx.RangeEffectType;
                            if (_rangeEffectType != -1) 
                            {
                                //目标
                                if (_skillEx.RangeEffectTarType == (int)SKILLRANGEEFFECTTAR.SELECTTARGET)
                                {
                                    if (_mainPlayer.SelectTarget)
                                    {
                                        _mainPlayer.SelectTarget.StopSkillRangeEffect();
                                    }
                                }
                                else //玩家自己
                                {
                                    _mainPlayer.StopSkillRangeEffect();
                                }
                            }
                            break;
                        }
                    }
                }
            }
            _mainPlayer.CurPressSkillId = -1;
        }
        //使用技能
     //   UseSkill(button);
    }
    void UseItem(GameObject button)
    {
    }

    // 新手教学
    public void NewPlayerGuide(int nIndex)
    {
        m_NewPlayerGuide_Step = nIndex;
        switch(nIndex)
        {
            case 1:
                if (m_SkillXPBt && m_SkillXPBt.activeInHierarchy)
                {
                    NewPlayerGuidLogic.OpenWindow(m_SkillXPBt, 110, 110, "", "left", 0, true);
                }
                break;
            case 2:
                if (m_SkillAttackBt && m_SkillAttackBt.activeInHierarchy)
                {
                    //NewPlayerGuidLogic.OpenWindow(m_SkillAttackBt, 134, 134, "点击施放普通攻击", "left", 0, true);
                    NewPlayerGuidLogic.OpenWindow(m_SkillAttackBt, 134, 134, StrDictionary.GetClientDictionaryString("#{2874}"), "left", 0, true);
                }
                break;
            case 3:
                if (m_Skill1Bt && m_Skill1Bt.activeInHierarchy)
                {
                    NewPlayerGuidLogic.OpenWindow(m_Skill2Bt, 110, 110, StrDictionary.GetClientDictionaryString("#{2875}"), "left", 0, true);
                    //NewPlayerGuidLogic.OpenWindow(m_Skill1Bt, 110, 110, "点击施放技能", "left", 0, true);
                }
                break;
            case 4: 
                if (m_SkillAttackBt && m_SkillAttackBt.activeInHierarchy)
                {
                    //NewPlayerGuidLogic.OpenWindow(m_SkillAttackBt, 134, 134, "点击施放普通攻击", "left", 0, true);
                    NewPlayerGuidLogic.OpenWindow(m_SkillAttackBt, 200, 200, "", "left", 3);
                }
                break;
            default:
                break;
        }
    }

    //切换目标
    public void SwitchTarget()
    {
        if (null != Singleton<ObjManager>.GetInstance().MainPlayer)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SwitchTarget();
        }
    }

    public void PlayTween(bool nDirection)
    {
        //foreach (TweenAlpha tween in m_FoldTween)
        //{
        //    tween.Play(nDirection);
        //}
        //foreach (BoxCollider box in gameObject.GetComponentsInChildren<BoxCollider>())
        //{
        //    box.enabled = !nDirection;
        //}
        gameObject.SetActive(!nDirection);
    }

    void XPNewPlayGuid()
    {
        if (PlayerPreferenceData.XPNewPlayerGuideFlag == false)
        {
             NewPlayerGuide(1);
        }
    }
}
