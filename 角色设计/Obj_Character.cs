/********************************************************************************
 *	文件名：	Obj.cs
 *	全路径：	\Script\Obj\Obj_Character.cs
 *	创建人：	李嘉
 *	创建时间：2013-10-25
 *
 *	功能说明：游戏逻辑Obj_Character类
 *	修改记录：
 *	李嘉 2014-02-19 将原来的obj基类上移作为有动作行为的obj_character，下层添加基类obj
*********************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Games.GlobeDefine;
using Games.Events;
using Games.AI_Logic;
using Games.Animation_Modle;
using Games.SkillModle;
using GCGame.Table;
using GCGame;
namespace Games.LogicObj
{
    public partial class Obj_Character : Obj
    {
        #region 构造函数
        public Obj_Character()
        {
            m_ObjType = GameDefine_Globe.OBJ_TYPE.OBJ_CHARACTER;
            m_MoveOverEvent = new GameEvent();
            m_BaseAttr = new BaseAttr();
            m_CurObjAnimState = GameDefine_Globe.OBJ_ANIMSTATE.STATE_INVAILD;

            m_SkillCore = new SkillCore();
            m_MultiShowDamageInfo = new List<MultiShowDamageBoard>();

            m_BindParent = -1;
            m_BindChildren = new List<int>();
			
        }

        #endregion

#region BindData
        private int m_BindParent;
        //更新父节点的绑定状态
        public int BindParent
        {
            get { return m_BindParent; }
            set 
            {
                int oldId = m_BindParent;
                m_BindParent = value;
                OnBindParentChange(oldId, value);
            }
        }
        private List<int> m_BindChildren;
        public int GetBindChildIndex(int id)
        {
            for (int n = 0; n < m_BindChildren.Count; ++n)
            {
                if (m_BindChildren[n] == id)
                {
                    return n;
                }
            }
            return -1;
        }
        //用于在装载模型之后的一次刷新
        public void UpdateAllBind()
        {
            OnBindParentChange(-1, m_BindParent);
            for(int nIndex = 0; nIndex < GlobeVar.BIND_CHILDREN_MAX && nIndex < m_BindChildren.Count; ++nIndex)
            {
                OnBindChildChange(-1, m_BindChildren[nIndex]);
            }
        }
        //更新子节点绑定状态
        public void UpdateBindChildren(List<int> data)
        {
            for (int nIndex = 0; nIndex < GlobeVar.BIND_CHILDREN_MAX;++nIndex )
            {
                //只取能取到的范围，其他的给-1
                int newid = -1;
                if (nIndex < data.Count)
                {
                    newid = data[nIndex];
                }
                //在已有范围之内，更新数值
                if (nIndex < m_BindChildren.Count)
                {
                    int oldid = m_BindChildren[nIndex];
                    m_BindChildren[nIndex] = newid;
                    OnBindChildChange(oldid, newid);
                }
                //在已有范围之外，如果非-1说明有新增
                else if (newid >= 0)
                {
                    m_BindChildren.Add(newid);
                    OnBindChildChange(-1, newid);
                }
            }
        }        
        //绑定父节点的变化响应
        public virtual void OnBindParentChange(int oldID,int newID)
        {
            if (oldID == newID)
            {
                return;
            }
            //绑定(兼容已有旧父节点）
            if ( newID >= 0 ) 
            {
                //如果父节点已经刷出来了就绑定上，如果没刷出来，等父节点来绑定自己。
                Obj_Character objParent = Singleton<ObjManager>.GetInstance().FindObjCharacterInScene(newID);
                if (null != objParent)
                {
                    objParent.OnBindOpt(this);
                }
            }
            //解绑
            if (oldID >= 0 && newID < 0)
            {
                if (gameObject.transform.parent != null)
                {
                    OnUnBindOpt(gameObject.transform.parent.transform.position);
                }
            }
        }
        //绑定子节点的变化响应
        public virtual void OnBindChildChange(int oldID, int newID)
        {
            if (oldID == newID)
            {
                return;
            }
            //让子节点绑定（兼容子节点已有父节点的情况）
            if (newID >= 0)
            {
                //如果子节点已经刷出来了就帮上，如果没刷出来，等子节点自己刷出来之后绑定上
                Obj_Character objChild = Singleton<ObjManager>.GetInstance().FindObjCharacterInScene(newID);
                if (null != objChild)
                {
                    OnBindOpt(objChild);
                }
            }
            //解除子节点的绑定
            if (oldID >= 0 && newID < 0)
            {
                Obj_Character objChild = Singleton<ObjManager>.GetInstance().FindObjCharacterInScene(oldID);
                if (null != objChild)
                {
                    objChild.OnUnBindOpt(gameObject.transform.position);
                }
            }
        }
        //父节点：绑定子节点的操作内容
        public virtual void OnBindOpt(Obj_Character obj)
        {
            if (obj != null)
            {
                obj.gameObject.transform.parent = gameObject.transform;
                obj.gameObject.transform.localPosition = Vector3.zero;
                obj.NavAgent.enabled = false;
                obj.SetVisible(false);
            }
        }
        //子节点：与父节点解绑的操作内容
        public virtual void OnUnBindOpt(Vector3 parentPos)
        {
            SetVisible(true);
            gameObject.transform.parent = null;
            gameObject.transform.position = parentPos;
            NavAgent.enabled = true;
        }
        //初始化
        public void InitBindFromData(Obj_Init_Data data)
        {
            BindParent = data.m_BindParent;
            UpdateBindChildren(data.m_BindChildren);
        }
#endregion
                            #region Mono脚本接口
        #endregion

        #region 技能
        protected SkillCore m_SkillCore = null;
        public Games.SkillModle.SkillCore SkillCore
        {
            get { return m_SkillCore; }
        }

        protected int m_curUseSkillId = -1;
        public int CurUseSkillId
        {
            get { return m_curUseSkillId; }
            set { m_curUseSkillId = value; }
        }

        //初始化Obj逻辑数据
        public virtual bool Init(Obj_Init_Data initData)
        {
            if (null == m_ObjTransform)
            {
                m_ObjTransform = transform;
            }

            InitBindFromData(initData);        
            return true;
        }
        #endregion

        #region 重生
        //////////////////////////////////////////////////////////////////////////
        //NPC重生相关处理
        //////////////////////////////////////////////////////////////////////////
        //Obj创建时候如果是死亡状态时候调用
        public virtual bool OnCorpse()
        {
            BaseAttr.Die = true;
            //切换到死亡状态
            if (null != m_AnimLogic)
            {
                m_AnimLogic.Stop();
            }

            CurObjAnimState = GameDefine_Globe.OBJ_ANIMSTATE.STATE_CORPSE;
            return true;
        }
        //Obj死亡时候调用
        public virtual bool OnDie()
        {
            if (IsDie())
            {
                return false;
            }
			CurObjAnimState = GameDefine_Globe.OBJ_ANIMSTATE.STATE_DEATH;
			if (m_SkillCore != null) 
			{
								m_SkillCore.BreakCurSkill();
			}
			if (m_BaseAttr.RoleBaseID == 210203||m_BaseAttr.RoleBaseID == 210204||m_BaseAttr.RoleBaseID == 210205||m_BaseAttr.RoleBaseID == 210206||m_BaseAttr.RoleBaseID == 210207||
			    m_BaseAttr.RoleBaseID == 210303||m_BaseAttr.RoleBaseID == 210304||m_BaseAttr.RoleBaseID == 210305||m_BaseAttr.RoleBaseID == 210306||m_BaseAttr.RoleBaseID == 210307)
			{

				int index=m_BaseAttr.RoleBaseID%210203;
				if(index>4)
				{
					index=m_BaseAttr.RoleBaseID%210303;
				}
				string name="JuqingItem"+index;
				GameObject obj=ObjManager.GetInstance().FindOtherGameObj(name);
				if(obj)
				{
					obj.GetComponent<Obj_JuqingItem>().IsActive=true;
					obj.GetComponent<Obj_JuqingItem>().PlayEffect(301);
					Obj_JuqingItem  item=obj.GetComponent<Obj_JuqingItem>();
//					if(ObjManager.GetInstance().MainPlayer.gameObject.GetComponent<AI_JuQing>()==null)
//					{
//						ObjManager.GetInstance().MainPlayer.gameObject.AddComponent<AI_JuQing>();
//					}
					if(item!=null)
					{
						ObjManager.GetInstance().MainPlayer.SkillCore.BreakCurSkill();
					    ObjManager.GetInstance().MainPlayer.EnterJuqing(item);
					}


				}
			    //JuQingItemMgr.GetInstance().
			}
            BaseAttr.Die = true;
         
            StopMove();
           
            return true;
        }
        //Obj复活时调用
        public virtual bool OnRelife()
        {
            BaseAttr.Die = false;
            if (null != m_AnimLogic)
            {
                m_AnimLogic.Stop();
            }
            CurObjAnimState = GameDefine_Globe.OBJ_ANIMSTATE.STATE_NORMOR;
			GameObject obj=this.gameObject.transform.FindChild("Model").gameObject;
			if(obj)
				obj.SetActive(true);
            return true;
        }
        public bool IsDie()
        {
            return BaseAttr.Die;
        }
        private bool m_isAutoLife = false;      //是否自动重生
        public bool AutoLife
        {
            get { return m_isAutoLife; }
            set { m_isAutoLife = value; }
        }
        private short m_nAutoLifeTime;  //自动重生时间
        public short AutoLifeTime
        {
            get { return m_nAutoLifeTime; }
            set { m_nAutoLifeTime = value; }
        }
        #endregion


        #region UI及名字版
        //名字板相关
        protected GameObject m_HeadInfoBoard;        // 头顶信息板总节点
        public UnityEngine.GameObject HeadInfoBoard
        {
            get { return m_HeadInfoBoard; }

        }
        protected UILabel m_NameBoard;              // 名字板 所有obj共有

        // 名字板list中的索引
        protected int m_NameBoardIndex;
        public int NameBoardIndex
        {
            get { return m_NameBoardIndex; }
            set { m_NameBoardIndex = value; }
        }

        // 头顶信息板高度调整
        protected float m_DeltaHeight;
        public float DeltaHeight
        {
            get { return m_DeltaHeight; }
            set { m_DeltaHeight = value; }
        }

        public void ShowNameBoard()
        {
            if (null == m_NameBoard)
            {
                return;
            }

            if (PlayerPreferenceData.SystemNameBoard == 0)
            {
                CloseNameBoard();
            }
            else
            {
                if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER || 
                    ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER ||
                    ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_ZOMBIE_PLAYER)
                {
                    Obj_OtherPlayer player = this as Obj_OtherPlayer;
                    if (null != player && null != player.m_playerHeadInfo)
                    {
                        player.m_playerHeadInfo.ShowOriginalHeadInfo(true);
                        player.UpdateGBNameBoard();
                    }
                    SetNameBoardColor();
                    m_NameBoard.text = BaseAttr.RoleName;
                }
                else if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_NPC ||
                    ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_FELLOW)
                {
                    m_NameBoard.gameObject.SetActive(true);
                    SetNameBoardColor();
                    m_NameBoard.text = BaseAttr.RoleName;
                }
            }
            // 更新名字板高度的操作移到billboard脚本的update中执行
        }

        public void CloseNameBoard()
        {
            if (null == m_NameBoard)
            {
                return;
            }

            if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER ||
                ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER ||
                ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_ZOMBIE_PLAYER)
            {
                Obj_OtherPlayer player = this as Obj_OtherPlayer;
                if (null != player && null != player.m_playerHeadInfo)
                {
                    player.m_playerHeadInfo.ShowOriginalHeadInfo(false);
                }
            }
            else if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_NPC ||
                    ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_FELLOW)
            {
                m_NameBoard.gameObject.SetActive(false);
            }
        }

        public virtual void SetNameBoardColor()
        {
            if (m_NameBoard)
            {
                m_NameBoard.color = GetNameBoardColor();
            }
        }

        public virtual Color GetNameBoardColor()
        {
            return GCGame.Utils.GetColorByString("FFFFFF");
        }
        List<MultiShowDamageBoard> m_MultiShowDamageInfo;
        public List<MultiShowDamageBoard> MultiShowDamageInfo
        {
            get { return m_MultiShowDamageInfo; }
            set { m_MultiShowDamageInfo = value; }
        }
        public void UpdateShowMultiShowDamageBoard()
        {
			if (IsDie())
			{
				m_MultiShowDamageInfo.Clear();
				return;
			}
			if (m_MultiShowDamageInfo.Count >0)
			{
				List<MultiShowDamageBoard> _NeedRemoveInfo = new List<MultiShowDamageBoard>();
				MultiShowDamageBoard _TmpInfo =new MultiShowDamageBoard();
				for(int i=0;i<m_MultiShowDamageInfo.Count; i++)
				{
					if (m_MultiShowDamageInfo[i].ShowTimes >0)
					{
						float fPassTime = Time.time - m_MultiShowDamageInfo[i].LastShowTime;
						if (fPassTime -m_MultiShowDamageInfo[i].ShowInter >=0)
						{
							ShowDamageBoard(m_MultiShowDamageInfo[i].DamageType, m_MultiShowDamageInfo[i].ShowValue);
							_TmpInfo =m_MultiShowDamageInfo[i];
							_TmpInfo.ShowTimes =m_MultiShowDamageInfo[i].ShowTimes-1;
							_TmpInfo.LastShowTime =Time.time;
							m_MultiShowDamageInfo[i] =_TmpInfo;
						}
					}
					else
					{
						_NeedRemoveInfo.Add(m_MultiShowDamageInfo[i]);
					}
				}
				//移除掉失效的
				for (int i = 0; i < _NeedRemoveInfo.Count; ++i)
				{
					m_MultiShowDamageInfo.Remove(_NeedRemoveInfo[i]);
				}
			}
        }
        public virtual void ShowDamageBoard(GameDefine_Globe.DAMAGEBOARD_TYPE eType, int nValue = 0)
        {
			
			if (eType != GameDefine_Globe.DAMAGEBOARD_TYPE.SKILL_NAME &&
								eType != GameDefine_Globe.DAMAGEBOARD_TYPE.SKILL_NAME_NPC) {
								string strValue = "";
								if (eType == GameDefine_Globe.DAMAGEBOARD_TYPE.TARGET_ATTACK_MISS ||
										eType == GameDefine_Globe.DAMAGEBOARD_TYPE.PLAYER_ATTACK_MISS ||
										eType == GameDefine_Globe.DAMAGEBOARD_TYPE.PLAYER_ATTACK_MISS_PARTNER) {
										strValue = StrDictionary.GetClientDictionaryString ("#{2626}");
								} else if (eType == GameDefine_Globe.DAMAGEBOARD_TYPE.TARGET_ATTACK_IGNORE ||
										eType == GameDefine_Globe.DAMAGEBOARD_TYPE.PLAYER_ATTACK_IGNORE ||
										eType == GameDefine_Globe.DAMAGEBOARD_TYPE.PLAYER_ATTACK_IGNORE_PARTNER) {
										strValue = StrDictionary.GetClientDictionaryString ("#{2627}");
								} else {
										if (eType == GameDefine_Globe.DAMAGEBOARD_TYPE.PLAYER_ATTACK_CRITICAL ||
												eType == GameDefine_Globe.DAMAGEBOARD_TYPE.PLAYER_ATTACK_CRITICAL_PARTNER ||
												eType == GameDefine_Globe.DAMAGEBOARD_TYPE.TARGET_ATTACK_CRITICAL) {
												strValue = StrDictionary.GetClientDictionaryString ("#{2628}", nValue);
										} else if (eType == GameDefine_Globe.DAMAGEBOARD_TYPE.PLAYER_HP_DOWN ||
												eType == GameDefine_Globe.DAMAGEBOARD_TYPE.TARGET_HPDOWN_PARTNER ||
												eType == GameDefine_Globe.DAMAGEBOARD_TYPE.TARGET_HPDOWN_PLAYER ||
												eType == GameDefine_Globe.DAMAGEBOARD_TYPE.PLAYER_MP_DOWN ||
												eType == GameDefine_Globe.DAMAGEBOARD_TYPE.PLAYER_XP_DOWN) {
												strValue = nValue.ToString ();
										} else if (eType == GameDefine_Globe.DAMAGEBOARD_TYPE.PLAYER_HP_UP ||
												eType == GameDefine_Globe.DAMAGEBOARD_TYPE.PLAYER_MP_UP ||
												eType == GameDefine_Globe.DAMAGEBOARD_TYPE.PLAYER_XP_UP) {
												strValue = string.Format ("+{0}", nValue);
										}
								}
				
								if (null != GameManager.gameManager.ActiveScene.DamageBoardManager) {
										GameManager.gameManager.ActiveScene.DamageBoardManager.ShowDamageBoard ((int)eType, nValue.ToString (), m_ObjTransform.position, false);
								}
				
								//===收到伤害时进入战斗待机 ,如果在马上则不播放
								Obj_OtherPlayer player = this as Obj_OtherPlayer;
				                if(player!=null)
				                 {
								if (CurObjAnimState == GameDefine_Globe.OBJ_ANIMSTATE.STATE_NORMOR && player.MountID < 0) {
										AnimLogic.Play ((int)(CharacterDefine.CharacterAnimId.ActStand));
					                }
								}
						}
         
        }

        public virtual void ShowDamageBoard_SkillName(GameDefine_Globe.DAMAGEBOARD_TYPE eType, string strValue, bool isProfessionSkill = true)
        {
            if ((eType == GameDefine_Globe.DAMAGEBOARD_TYPE.SKILL_NAME || eType == GameDefine_Globe.DAMAGEBOARD_TYPE.SKILL_NAME_NPC) 
                && strValue != "")
            {
                if (null != GameManager.gameManager.ActiveScene.DamageBoardManager)
                {
                    GameManager.gameManager.ActiveScene.DamageBoardManager.ShowDamageBoard((int)eType, strValue, m_ObjTransform.position, isProfessionSkill);
                }
            }
        }

        //选择目标后更新UI信息
        public void UpdateTargetFrame(Obj_Character targetObj)
        {
            if (null == TargetFrameLogic.Instance() || null == GameManager.gameManager.ActiveScene)
            {
                return;
            }

            if (null == targetObj)
            {
                TargetFrameLogic.Instance().CancelTarget();
				//targetObj.CancelOutLine();
                GameManager.gameManager.ActiveScene.DeactiveSelectCircle();
                return;
            }

            //更新头像信息
            TargetFrameLogic.Instance().ChangeTarget(targetObj);
			//SetOutLine ();
            GameManager.gameManager.ActiveScene.ActiveSelectCircle(targetObj.gameObject,targetObj);
        }
        #endregion

        #region AI
        protected bool m_bCanMove = false;
        private int m_nReputation = 0;
        public int ReputationID
        {
            get { return m_nReputation; }
            set { m_nReputation = value; }
        }

        private AIController m_aiController = null;
        public AIController Controller
        {
            get { return m_aiController; }
            set { m_aiController = value; }
        }

        public void InitAI()
        {
            m_aiController = this.gameObject.GetComponent<AIController>();
        }
        //战斗相关
        public virtual void OnEnterCombat(Obj_Character Attacker)
        {
        }
        public virtual void OnLevelCombat(Obj_Character Attacker)
        {
        }

        public Vector3 DefaultPosition()
        {
            //单机点以后统一在场景中增加叫做OffLine节点，不用每一个场景写一个else if了
            GameObject offLinePoint = GameObject.Find("OffLine");
            if (null != offLinePoint)
            {
                return offLinePoint.transform.position;
            }

            return new Vector3(0.0f, 0.0f, 0.0f);
        }
        #endregion

        

        #region 纸娃娃 重载模型
        public void ReloadPlayerModelVisual(int nModelVisualID = GlobeVar.INVALID_ID, 
                                            int nProfession = GlobeVar.INVALID_ID)
        {
            if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER ||
                ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER)
            {
                Obj_OtherPlayer objPlayer = this as Obj_OtherPlayer;
                nModelVisualID = objPlayer.ModelVisualID;
            }

            Tab_ItemVisual tabItemVisual = TableManager.GetItemVisualByID(nModelVisualID, 0);
            if (tabItemVisual == null)
            {
                tabItemVisual = TableManager.GetItemVisualByID(GlobeVar.DEFAULT_VISUAL_ID, 0);
                if (tabItemVisual == null)
                {
                    return;
                }
            }

            int nCharModelID = GetCharModelID(tabItemVisual, nProfession);

            Tab_CharModel tabCharModel = TableManager.GetCharModelByID(nCharModelID, 0);
            if (tabCharModel == null)
            {
                return;
            }

            if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER ||
                ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER)
            {
                Obj_OtherPlayer objPlayer = this as Obj_OtherPlayer;

                if (false == Singleton<ObjManager>.GetInstance().ReloadModel(gameObject,
                    tabCharModel.ResPath,
                    Singleton<ObjManager>.GetInstance().AsycReloadModelOver,
                    tabCharModel.AnimPath,
                    objPlayer.MountObj))
                {
					if (gameObject) 
					{
						GameObject  Model=gameObject.transform.FindChild("Model").gameObject;
						if(objPlayer!=null&&objPlayer.MountID>0)
						{
							GameObject  rid=gameObject.transform.GetComponentInChildren<Obj_Mount>().gameObject;
							if(rid)
							{
								Model=rid.transform.FindChild("Model").gameObject;
							}
						}

						//缩放mainplayer中毒model，不能直接缩放mainplayer。这样会影响特效
					
						if(Model!=null)
							Model.transform.localScale=new Vector3(tabCharModel.Scale, tabCharModel.Scale, tabCharModel.Scale);
					}
                    return;
                }
            }
            else
            {
                if (false == Singleton<ObjManager>.GetInstance().ReloadModel(gameObject,
                    tabCharModel.ResPath,
                    Singleton<ObjManager>.GetInstance().AsycReloadModelOver,
                    tabCharModel.AnimPath,
                    null))
                {
					if (gameObject) 
					{
						GameObject  Model=gameObject.transform.FindChild("Model").gameObject;
						//缩放mainplayer中毒model，不能直接缩放mainplayer。这样会影响特效
						if(Model!=null)
							Model.transform.localScale=new Vector3(tabCharModel.Scale, tabCharModel.Scale, tabCharModel.Scale);
					}
                    return;
                }
            }

        }

        public void RealoadPlayerWeaponVisual(int nWeaponDataID = GlobeVar.INVALID_ID, 
                                              int nProfession = GlobeVar.INVALID_ID, 
                                              int nWeaponEffectGem = GlobeVar.INVALID_ID)
        {
            if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER ||
                ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER ||
                ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_ZOMBIE_PLAYER)
            {
                Obj_OtherPlayer objPlayer = this as Obj_OtherPlayer;
                nWeaponDataID = objPlayer.CurWeaponDataID;
            }

            bool defaultVisual = false;
            Tab_ItemVisual tabItemVisual = null;

            Tab_EquipAttr tabEquipAttr = TableManager.GetEquipAttrByID(nWeaponDataID, 0);
            if (tabEquipAttr == null)
            {
                defaultVisual = true;
            }
            else
            {
				tabItemVisual = TableManager.GetItemVisualByID(tabEquipAttr.ModelId, 0);
                if (tabItemVisual == null)
                {
                    defaultVisual = true;
                }
            }

            if (defaultVisual)
            {
				tabItemVisual = TableManager.GetItemVisualByID(nWeaponDataID, 0);//GlobeVar.DEFAULT_VISUAL_ID
                if (tabItemVisual == null)
                {
                    return;
                }
            }

            if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER ||
                ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER ||
                ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_ZOMBIE_PLAYER)
            {
                Obj_OtherPlayer objPlayer = this as Obj_OtherPlayer;
                nProfession = objPlayer.Profession;
            }
            int nWeaponModelID = GetWeaponModelID(tabItemVisual, nProfession);

            Tab_WeaponModel tabWeaponModel = TableManager.GetWeaponModelByID(nWeaponModelID, 0);
            if (tabWeaponModel == null)
            {
                return;
            }

            if (nProfession == (int)CharacterDefine.PROFESSION.TIANSHAN)
            {
                string resWeaponLeft = tabWeaponModel.ResPath + "_L";
                string resWeaponRight = tabWeaponModel.ResPath + "_R";

                List<object> param1 = new List<object>();
                if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER ||
                    ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER ||
                    ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_ZOMBIE_PLAYER)
                {
                    Obj_OtherPlayer objPlayer = this as Obj_OtherPlayer;

                    //param1.Add("Weapon_L");
					param1.Add("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 L Clavicle/Bip001 L UpperArm/Bip001 L Forearm/Bip001 L Hand/HH_weaponHandLf");
                    param1.Add(objPlayer.WeaponEffectGem);
                    param1.Add(objPlayer.MountObj);
                    param1.Add(nProfession);
                }
                else
                {
                    //param1.Add("Weapon_L");
					param1.Add("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 L Clavicle/Bip001 L UpperArm/Bip001 L Forearm/Bip001 L Hand/HH_weaponHandLf");
                    param1.Add(nWeaponEffectGem);
                    param1.Add(null);
                    param1.Add(nProfession);
                }
                if (false == Singleton<ObjManager>.GetInstance().ReloadWeapon(gameObject,
                    resWeaponLeft,
                    Singleton<ObjManager>.GetInstance().AsycReloadWeaponOver,
                    param1))
                {
                    return;
                }

                List<object> param2 = new List<object>();
                if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER ||
                    ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER ||
                    ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_ZOMBIE_PLAYER)
                {
                    Obj_OtherPlayer objPlayer = this as Obj_OtherPlayer;

                   // param2.Add("Weapon_R");
					param2.Add("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/HH_weaponHandRt");
                    param2.Add(objPlayer.WeaponEffectGem);
                    param2.Add(objPlayer.MountObj);
                    param2.Add(nProfession);
                }
                else
                {
                    //param2.Add("Weapon_R");
					param1.Add("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/HH_weaponHandRt");
                    param2.Add(nWeaponEffectGem);
                    param2.Add(null);
                    param2.Add(nProfession);
                }

                if (false == Singleton<ObjManager>.GetInstance().ReloadWeapon(gameObject,
                resWeaponRight,
                Singleton<ObjManager>.GetInstance().AsycReloadWeaponOver,
                param2))
                {
                    return;
                }
            }
           /* else if (nProfession == (int)CharacterDefine.PROFESSION.XIAOYAO)
            {
                List<object> param = new List<object>();
                if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER ||
                    ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER ||
                    ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_ZOMBIE_PLAYER)
                {
                    Obj_OtherPlayer objPlayer = this as Obj_OtherPlayer;

                    //param.Add("Weapon_L");
					param.Add("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/HH_weaponHandLf");
					param.Add(objPlayer.WeaponEffectGem);
                    param.Add(objPlayer.MountObj);
                    param.Add(nProfession);
                }
                else
                {
                   // param.Add("Weapon_L");
					param.Add("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/HH_weaponHandLf");
					param.Add(nWeaponEffectGem);
                    param.Add(null);
                    param.Add(nProfession);
                }
                if (false == Singleton<ObjManager>.GetInstance().ReloadWeapon(gameObject,
                tabWeaponModel.ResPath,
               Singleton<ObjManager>.GetInstance().AsycReloadWeaponOver,
               param))
               {
                   return;
               }
            }*/
            else
            {
                List<object> param = new List<object>();
                if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER ||
                    ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER ||
                    ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_ZOMBIE_PLAYER)
                {
                    Obj_OtherPlayer objPlayer = this as Obj_OtherPlayer;

                    param.Add("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/HH_weaponHandRt");
                    param.Add(objPlayer.WeaponEffectGem);
                    param.Add(objPlayer.MountObj);
                    param.Add(nProfession);
                }  
                else
                {
                    param.Add("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/HH_weaponHandRt");
                    param.Add(nWeaponEffectGem);
                    param.Add(null);
                    param.Add(nProfession);
                }

                if (false == Singleton<ObjManager>.GetInstance().ReloadWeapon(gameObject,
                tabWeaponModel.ResPath,
                Singleton<ObjManager>.GetInstance().AsycReloadWeaponOver,
                param))
                {
                    return;
                }
            }
            
        }

        public int GetCharModelID(Tab_ItemVisual tabItemVisual, int nProfession = GlobeVar.INVALID_ID)
        {
            if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER ||
                ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER ||
                ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_ZOMBIE_PLAYER)
            {
                Obj_OtherPlayer objPlayer = this as Obj_OtherPlayer;
                nProfession = objPlayer.Profession;
            }

            if (nProfession == (int)CharacterDefine.PROFESSION.SHAOLIN)
            {
                return tabItemVisual.CharModelIDShaoLin;
            }
            else if (nProfession == (int)CharacterDefine.PROFESSION.TIANSHAN)
            {
                return tabItemVisual.CharModelIDTianShan;
            }
            else if (nProfession == (int)CharacterDefine.PROFESSION.DALI)
            {
                return tabItemVisual.CharModelIDDaLi;
            }
            else if (nProfession == (int)CharacterDefine.PROFESSION.XIAOYAO)
            {
                return tabItemVisual.CharModelIDXiaoYao;
            }
            return GlobeVar.INVALID_ID;
        }

        public int GetWeaponModelID(Tab_ItemVisual tabItemVisual, int nProfession = GlobeVar.INVALID_ID)
        {
            if (ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_MAIN_PLAYER ||
                ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_OTHER_PLAYER ||
                ObjType == GameDefine_Globe.OBJ_TYPE.OBJ_ZOMBIE_PLAYER)
            {
                Obj_OtherPlayer objPlayer = this as Obj_OtherPlayer;
                nProfession = objPlayer.Profession;
            }

            if (nProfession == (int)CharacterDefine.PROFESSION.SHAOLIN)
            {
                return tabItemVisual.WeaponModelIDShaoLin;
            }
            else if (nProfession == (int)CharacterDefine.PROFESSION.TIANSHAN)
            {
                return tabItemVisual.WeaponModelIDTianShan;
            }
            else if (nProfession == (int)CharacterDefine.PROFESSION.DALI)
            {
                return tabItemVisual.WeaponModelIDDaLi;
            }
            else if (nProfession == (int)CharacterDefine.PROFESSION.XIAOYAO)
            {
                return tabItemVisual.WeaponModelIDXiaoYao;
            }
            return GlobeVar.INVALID_ID;
        }
        #endregion

#region 可见性
        protected bool m_bVisible = true;
        public virtual void SetVisible(bool bVisible)
        {
            m_bVisible = bVisible;

            for(int i=0; i<transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(m_bVisible);
            }
        }

        public bool IsVisibleChar() { return m_bVisible; }

        // 重载模型回调
        public virtual void OnReloadModle()
        {
            SetVisible(m_bVisible);
            //初始化特效挂点信息
            if (ObjEffectLogic != null)
            {
                ObjEffectLogic.InitEffectPointInfo();
            }
            //初始化 材质信息
          //  InitMaterialInfo();
         //   UpdateAllBind();
        }
        public override void OptAfterInitMaterialInfo()
        {
            base.OptAfterInitMaterialInfo();
            //是否隐身
            OptStealthLevChange();
        }
        // 返回隐藏权重值
        public int GetVisibleValue()
        {
            // 权重值越大，隐藏优先级越低，显示优先级越高
            if (Reputation.GetObjReputionType(this) == CharacterDefine.REPUTATION_TYPE.REPUTATION_HOSTILE)
            {
                return 100;
            }

            if (Reputation.GetObjReputionType(this) == CharacterDefine.REPUTATION_TYPE.REPUTATION_FRIEND)
            {
                return 50;
            }

            return 0;
        }
        
        // 和隐藏不一样，为模型是否可见
        private bool m_bModelInViewPort = true;
        public bool ModelInViewPort
        {
            get { return m_bModelInViewPort; }
            set { m_bModelInViewPort = value; }
        }
#endregion
#region 技能范围特效
        
        public void PlaySkillRangeEffect()
        {
            Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
            if (_mainPlayer == null)
            {
                return;
            }
            int nCurPressSkillId = _mainPlayer.CurPressSkillId;
            if (nCurPressSkillId ==-1)
            {
                return;
            }
            Tab_SkillEx _skillEx = TableManager.GetSkillExByID(nCurPressSkillId, 0);
            if (_skillEx != null)
            {
                int _rangeEffectType = _skillEx.RangeEffectType;
                if (_rangeEffectType != -1)
                {
                    switch (_rangeEffectType)
                    {
                        case (int) (SKILLRANGEEFFECTTYPE.RING):
                        {
                            PlayEffect((int)SKILLRANGEEFFECTID.RINGEFFECTID,OnLoadSkillRangeEffect);
                        }
                            break;
                        case (int) (SKILLRANGEEFFECTTYPE.CIRCLE):
                        {
                            PlayEffect((int)SKILLRANGEEFFECTID.CIRCLEEFFECTID,OnLoadSkillRangeEffect);
                        }
                            break;
                        case (int) (SKILLRANGEEFFECTTYPE.RECT):
                        {
                            PlayEffect((int)SKILLRANGEEFFECTID.RECTEFFECTID, OnLoadSkillRangeEffect);
                        }
                            break;
                        case (int) (SKILLRANGEEFFECTTYPE.ARROWS):
                        {
                            PlayEffect((int)SKILLRANGEEFFECTID.ARROWSEFFECTID, OnLoadSkillRangeEffect);
                        }
                            break;
					    case (int) (SKILLRANGEEFFECTTYPE.SICIRCLE):
					     {
						     PlayEffect((int)SKILLRANGEEFFECTID.SICIRCLEEFFECTID, OnLoadSkillRangeEffect);
					     }
						break;
                        default:
                            break;
                    }
                }
            }
        }
		public void PlaySkillRangeEffect(int skillid)
		{

			if (skillid ==-1)
			{
				return;
			}
			Tab_SkillEx _skillEx = TableManager.GetSkillExByID(skillid, 0);
			if (_skillEx != null)
			{
				int _rangeEffectType = _skillEx.RangeEffectType;
				if (_rangeEffectType != -1)
				{
					switch (_rangeEffectType)
					{
					case (int) (SKILLRANGEEFFECTTYPE.RING):
					{
						PlayEffect((int)SKILLRANGEEFFECTID.RINGEFFECTID,OnLoadSkillRangeEffect,skillid);
					}
						break;
					case (int) (SKILLRANGEEFFECTTYPE.CIRCLE):
					{
						PlayEffect((int)SKILLRANGEEFFECTID.CIRCLEEFFECTID,OnLoadSkillRangeEffect,skillid);
					}
						break;
					case (int) (SKILLRANGEEFFECTTYPE.RECT):
					{
						PlayEffect((int)SKILLRANGEEFFECTID.RECTEFFECTID, OnLoadSkillRangeEffect,skillid);
					}
						break;
					case (int) (SKILLRANGEEFFECTTYPE.ARROWS):
					{
						PlayEffect((int)SKILLRANGEEFFECTID.ARROWSEFFECTID, OnLoadSkillRangeEffect,skillid);
					}
						break;
					case (int) (SKILLRANGEEFFECTTYPE.SICIRCLE):
					{
						PlayEffect((int)SKILLRANGEEFFECTID.SICIRCLEEFFECTID, OnLoadSkillRangeEffect,skillid);
					}
						break;
					default:
						break;
					}
				}
			}
		}
		void OnLoadSkillRangeEffect(GameObject SkillRangeEffect,object param)
		{


			Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
			if (_mainPlayer == null)
			{
				return;
			}
			int nCurPressSkillId = -1;
			if (SkillRangeEffect ==null)
			{
				return;
			}
			if (param != null) 
			{
								
				nCurPressSkillId = (int)param;		
			} 
			else
			{
				nCurPressSkillId = _mainPlayer.CurPressSkillId;
			}
			if (nCurPressSkillId == -1)
			{
				StopEffect((int)SKILLRANGEEFFECTID.RINGEFFECTID);
				StopEffect((int)SKILLRANGEEFFECTID.CIRCLEEFFECTID);
				StopEffect((int)SKILLRANGEEFFECTID.RECTEFFECTID);
				StopEffect((int)SKILLRANGEEFFECTID.ARROWSEFFECTID);
				StopEffect((int)SKILLRANGEEFFECTID.SICIRCLEEFFECTID);
				return;
			}
			Tab_SkillEx _skillEx = TableManager.GetSkillExByID(nCurPressSkillId, 0);
            if (_skillEx != null)
            {
                int _rangeEffectType = _skillEx.RangeEffectType;
                if (_rangeEffectType != -1)
                {
                    float parentScaleX = gameObject.transform.localScale.x;
                    float parentScaleY = gameObject.transform.localScale.y;
                    float parentScaleZ = gameObject.transform.localScale.z;
                    if (parentScaleX == 0 || parentScaleY == 0 || parentScaleZ==0)
                    {
                        return;
                    }
                    //修正大小
                    switch (_rangeEffectType)
                    {
                        case (int)(SKILLRANGEEFFECTTYPE.RING):
                        {
//                            float newXScale = _skillEx.GetRangeEffectSizebyIndex(0)*2.25f/parentScaleX; //加上父节点的修正
//                            float newZScale = _skillEx.GetRangeEffectSizebyIndex(0)*2.25f / parentScaleZ; //加上父节点的修正

						float newXScale = 0.1f;//加上父节点的修正
						float newYScale =0.1f;//加上父节点的修正
						SkillRangeEffect.transform.localScale = new Vector3(newXScale,1, newYScale);
					}
						break;
                        case (int)(SKILLRANGEEFFECTTYPE.CIRCLE):
                        {
//                            float newXScale = _skillEx.GetRangeEffectSizebyIndex(0) * 3.0f / parentScaleX;//加上父节点的修正
//                            float newYScale = _skillEx.GetRangeEffectSizebyIndex(0) * 3.0f / parentScaleY;//加上父节点的修正
						float newXScale = 0.1f;//加上父节点的修正
						float newYScale =0.1f;//加上父节点的修正
						SkillRangeEffect.transform.localScale = new Vector3(newXScale,newYScale,1);
                        }
                            break;
					case (int)(SKILLRANGEEFFECTTYPE.SICIRCLE):
					{
						//                            float newXScale = _skillEx.GetRangeEffectSizebyIndex(0) * 3.0f / parentScaleX;//加上父节点的修正
						//                            float newYScale = _skillEx.GetRangeEffectSizebyIndex(0) * 3.0f / parentScaleY;//加上父节点的修正
						float newXScale = 0.1f;//加上父节点的修正
						float newYScale =0.1f;//加上父节点的修正
						SkillRangeEffect.transform.localScale = new Vector3(newXScale,newYScale,1);
					}
						break;
					case (int)(SKILLRANGEEFFECTTYPE.RECT):
					{
                            float newYScale = _skillEx.GetRangeEffectSizebyIndex(0) * 1.0f/parentScaleY; //长
					     	float newXScale =2.0f;// _skillEx.GetRangeEffectSizebyIndex(1) * 3.0f/parentScaleX; //宽
                            SkillRangeEffect.transform.localScale = new Vector3(newXScale, newYScale, 1);
                            Vector3 localPos = SkillRangeEffect.transform.localPosition;
                            localPos.z = newYScale/2.0f; //修正z 偏移
                            SkillRangeEffect.transform.localPosition = localPos;
                        }
                            break;
                        case (int)(SKILLRANGEEFFECTTYPE.ARROWS):
                        {
                            SkillRangeEffect.transform.localRotation = Quaternion.Euler(0, _skillEx.GetRangeEffectSizebyIndex(0),0);
                            Vector3 posVector3 = SkillRangeEffect.transform.localPosition;
                            if (_skillEx.GetRangeEffectSizebyIndex(0) ==0)
                            {
                                posVector3.x = 0.0f;
                                posVector3.z = -1.5f;
                            }
                            else if (_skillEx.GetRangeEffectSizebyIndex(0) == 90)
                            {
                                posVector3.x =-1.5f;
                                posVector3.z = 0.0f;
                            }
                            else if (_skillEx.GetRangeEffectSizebyIndex(0) == 180)
                            {
                                posVector3.x =0.0f;
                                posVector3.z =1.5f;
                            }
                            else if (_skillEx.GetRangeEffectSizebyIndex(0) == 270)
                            {
                                posVector3.x =1.5f;
                                posVector3.z =0.0f;
                            }
                            SkillRangeEffect.transform.localPosition = posVector3;
                        }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        public void StopSkillRangeEffect()
        {
            Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
            if (_mainPlayer == null)
            {
                return;
            }
            int nCurPressSkillId = _mainPlayer.CurPressSkillId;
            if (nCurPressSkillId ==-1)
            {
                return;
            }
            Tab_SkillEx _skillEx = TableManager.GetSkillExByID(nCurPressSkillId, 0);
            if (_skillEx != null)
            {
                int _rangeEffectType = _skillEx.RangeEffectType;
                if (_rangeEffectType != -1)
                {
                    switch (_rangeEffectType)
                    {
                        case (int)(SKILLRANGEEFFECTTYPE.RING):
                            {
                                StopEffect((int)SKILLRANGEEFFECTID.RINGEFFECTID);
                            }
                            break;
                        case (int)(SKILLRANGEEFFECTTYPE.CIRCLE):
                            {
                                StopEffect((int)SKILLRANGEEFFECTID.CIRCLEEFFECTID);
                            }
                            break;
                        case (int)(SKILLRANGEEFFECTTYPE.RECT):
                            {
                                StopEffect((int)SKILLRANGEEFFECTID.RECTEFFECTID);
                            }
                            break;
                        case (int)(SKILLRANGEEFFECTTYPE.ARROWS):
                            {
                                StopEffect((int)SKILLRANGEEFFECTID.ARROWSEFFECTID);
                            }
                            break;
					     case (int)(SKILLRANGEEFFECTTYPE.SICIRCLE):
					       {
						       StopEffect((int)SKILLRANGEEFFECTID.SICIRCLEEFFECTID);
					        }
						    break;
                        default:
                            break;
                    }
                }
            }
        }
		//boss加入技能环的功能
		public void StopSkillRangeEffect(int skillid)
		{

			int nCurPressSkillId = skillid;
			if (nCurPressSkillId ==-1)
			{
				return;
			}
			Tab_SkillEx _skillEx = TableManager.GetSkillExByID(nCurPressSkillId, 0);
			if (_skillEx != null)
			{
				int _rangeEffectType = _skillEx.RangeEffectType;
				if (_rangeEffectType != -1)
				{
					switch (_rangeEffectType)
					{
					case (int)(SKILLRANGEEFFECTTYPE.RING):
					{
						StopEffect((int)SKILLRANGEEFFECTID.RINGEFFECTID);
					}
						break;
					case (int)(SKILLRANGEEFFECTTYPE.CIRCLE):
					{
						StopEffect((int)SKILLRANGEEFFECTID.CIRCLEEFFECTID);
					}
						break;
					case (int)(SKILLRANGEEFFECTTYPE.RECT):
					{
						StopEffect((int)SKILLRANGEEFFECTID.RECTEFFECTID);
					}
						break;
					case (int)(SKILLRANGEEFFECTTYPE.ARROWS):
					{
						StopEffect((int)SKILLRANGEEFFECTID.ARROWSEFFECTID);
					}
						break;
					case (int)(SKILLRANGEEFFECTTYPE.SICIRCLE):
					{
						StopEffect((int)SKILLRANGEEFFECTID.SICIRCLEEFFECTID);
					}
						break;
					default:
						break;
					}
				}
			}
		}
		#endregion
		
	}
}
