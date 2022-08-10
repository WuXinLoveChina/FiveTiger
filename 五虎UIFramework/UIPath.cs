/********************************************************************
	created:	2014/02/17
	created:	17:2:2014   9:56
	filename: 	UIPath.cs
	author:		王迪
	
	purpose:	记录UI存储位置信息，调用统一接口，方便加载位置发生变化时统一更改
*********************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class UIPathData
{
    public enum UIType
    {
        TYPE_ITEM,          // 只是用资源路径
        TYPE_BASE,          // 常驻场景的UI，Close不销毁 一级UI
        TYPE_POP,           // 弹出式UI，互斥，当前只能有一个弹出界面 二级弹出 在一级之上 阻止移动 无法操作后面UI
        TYPE_STORY,         // 故事界面，故事界面出现，所有UI消失，故事界面关闭，其他界面恢复
        TYPE_TIP,           // 三级弹出 在二级之上 不互斥 阻止移动 无法操作后面UI
        TYPE_MENUPOP,       // TYPE_POP的一个分支 由主菜单MenuBar打开的二级UI 主要用于动态加载特殊屏蔽区域 其他和POPUI完全一致
        TYPE_MESSAGE,       // 消息提示UI 在三级之上 一般是最高层级 不互斥 不阻止移动 可操作后面UI
        TYPE_DEATH,         // 死亡UI 目前只有复活界面 用于添加复活特殊规则
    };

    public static Dictionary<string, UIPathData> m_DicUIInfo = new Dictionary<string, UIPathData>();
    public static Dictionary<string, UIPathData> m_DicUIName = new Dictionary<string, UIPathData>();

    // group捆绑打包名称，会将同一功能的UI打包在一起
    // isMainAsset
    // bDestroyOnUnload 只对PopUI起作用
    public UIPathData(string _path, UIType _uiType, string groupName = null, bool bMainAsset = false, bool bDestroyOnUnload = true)
    {
        path = _path;
        uiType = _uiType;
        int lastPathPos = _path.LastIndexOf('/');
        if (lastPathPos > 0)
        {
            name = path.Substring(lastPathPos + 1);
        }
        else
        {
            name = path;
        }

        uiGroupName = groupName;
        isMainAsset = bMainAsset;

        isDestroyOnUnload = bDestroyOnUnload;

#if UNITY_ANDROID

        // < 768M UI不进行缓存
        if (SystemInfo.systemMemorySize < 768)
        {
            isDestroyOnUnload = true;
        }

#endif

        m_DicUIInfo.Add(_path, this);
        m_DicUIName.Add(name, this);
    }

    public string path;
    public string name;
    public UIType uiType;
    public string uiGroupName;
    public bool isMainAsset;            // 是否是主资源，如果主资源UI被关闭
    public bool isDestroyOnUnload;
}
public class UIInfo
{
    // item
    public static UIPathData SysShopPageItem = new UIPathData("Prefab/UI/SGSysShopPageItem", UIPathData.UIType.TYPE_ITEM, "SystemShop");
    public static UIPathData SysShopPage = new UIPathData("Prefab/UI/SGSysShopPage", UIPathData.UIType.TYPE_ITEM, "SystemShop");
    public static UIPathData MailListItem = new UIPathData("Prefab/UI/SGMailListItem", UIPathData.UIType.TYPE_ITEM, "Relation");
    public static UIPathData RelationNameListItem = new UIPathData("Prefab/UI/SGRelationNameListItem", UIPathData.UIType.TYPE_ITEM, "Relation");
    public static UIPathData AwardListItem = new UIPathData("Prefab/UI/AwardActivityItem", UIPathData.UIType.TYPE_ITEM);
    //public static UIPathData PVPOpListItem = new UIPathData("Prefab/UI/PVPOpListItem", UIPathData.UIType.TYPE_ITEM);
	public static UIPathData ChannelListItem = new UIPathData("Prefab/UI/ChannelListItem", UIPathData.UIType.TYPE_ITEM);
	public static UIPathData PVPSkillListItem = new UIPathData("Prefab/UI/PVPSkillListItem", UIPathData.UIType.TYPE_ITEM);
	public static UIPathData PVPRecordListItem = new UIPathData("Prefab/UI/PVPRecordListItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData AutoDrugItem = new UIPathData("Prefab/UI/SGAutoDrugItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData HuaShanMemberListItem = new UIPathData("Prefab/UI/SGHuaShanMemberListItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData AchieveItem = new UIPathData("Prefab/UI/AchieveItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData NoticeItem = new UIPathData("Prefab/UI/NoticeItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData BackPackItem = new UIPathData("Prefab/UI/BackPackItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData EquipStrengthenItem = new UIPathData("Prefab/UI/SGEquipStrengthenItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData PlayerListItem = new UIPathData("Prefab/UI/SGPlayerListItem", UIPathData.UIType.TYPE_ITEM);
	public static UIPathData PlayerFindItem = new UIPathData("Prefab/UI/SGPlayerFindItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData SceneMapTransmitPoint = new UIPathData("Prefab/UI/SGSceneMapTransmitPoint", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData PopMenuItem = new UIPathData("Prefab/UI/SGPopMenuItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData MissionItem = new UIPathData("Prefab/UI/SGMissionItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData MissionLogItem = new UIPathData("Prefab/UI/SGMissionLogItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData PartnerSkillItem = new UIPathData("Prefab/UI/SGPartnerSkillItem", UIPathData.UIType.TYPE_ITEM, "Partner");
    public static UIPathData PartnerFrameItem = new UIPathData("Prefab/UI/SGPartnerFrameItem", UIPathData.UIType.TYPE_ITEM, "Partner");
    public static UIPathData MountItem = new UIPathData("Prefab/UI/MountItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData TitleInvestitiveItem = new UIPathData("Prefab/UI/TitleInvestitiveItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData FastReplyItem = new UIPathData("Prefab/UI/SGFastReplyItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData LastSpeakerItem = new UIPathData("Prefab/UI/SGLastSpeakerItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData FashionItem = new UIPathData("Prefab/UI/FashionItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData ConsignSaleBagItem = new UIPathData("Prefab/UI/SGConsignSaleBagItem", UIPathData.UIType.TYPE_ITEM, "ConsignSale");
    public static UIPathData ConsignSaleBuyItem = new UIPathData("Prefab/UI/SGConsignSaleBuyItem", UIPathData.UIType.TYPE_ITEM, "ConsignSale");
    public static UIPathData ConsignSaleMySaleItem = new UIPathData("Prefab/UI/SGConsignSaleMySaleItem", UIPathData.UIType.TYPE_ITEM, "ConsignSale");
    public static UIPathData ConsignSaleQualityItem = new UIPathData("Prefab/UI/SGConsignSaleQualityItem", UIPathData.UIType.TYPE_ITEM, "ConsignSale");
    public static UIPathData ConsignSaleClassItem = new UIPathData("Prefab/UI/SGConsignSaleClassItem", UIPathData.UIType.TYPE_ITEM, "ConsignSale");
    public static UIPathData ConsignSaleSubClassItem = new UIPathData("Prefab/UI/SGConsignSaleSubClassItem", UIPathData.UIType.TYPE_ITEM, "ConsignSale");
    public static UIPathData TeamPlatformItem = new UIPathData("Prefab/UI/TeamPlatformItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData SkillRootButtonItem = new UIPathData("Prefab/UI/SkillRootButtonItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData SkillRootBarItem = new UIPathData("Prefab/UI/SGSkillRootBarItem", UIPathData.UIType.TYPE_ITEM);
	public static UIPathData SystemSkillBarItem = new UIPathData("Prefab/UI/SGSystemSkillBarItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData GuildListItem = new UIPathData("Prefab/UI/SGGuildListItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData GuildMemberListItem = new UIPathData("Prefab/UI/SGGuildMemberListItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData YuanBaoShopItem = new UIPathData("Prefab/UI/YBGood", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData DropItemHeadInfo = new UIPathData("Prefab/UI/SGDropItemHeadInfo", UIPathData.UIType.TYPE_ITEM, "DropItemHeadInfo");
    public static UIPathData PlayerHeadInfo = new UIPathData("Prefab/UI/SGPlayerHeadInfo", UIPathData.UIType.TYPE_ITEM, "PlayerHeadInfo");
    public static UIPathData FellowHeadInfo = new UIPathData("Prefab/UI/FellowHeadInfo", UIPathData.UIType.TYPE_ITEM, "FellowHeadInfo");
    public static UIPathData NPCHeadInfo = new UIPathData("Prefab/UI/SGNPCHeadInfo", UIPathData.UIType.TYPE_ITEM, "NPCHeadInfo");
    public static UIPathData MercenaryListItem = new UIPathData("Prefab/UI/MercenaryListItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData LivingSkillFormulaItem = new UIPathData("Prefab/UI/LiveSkillFormulaItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData LivingSkillStuffItem = new UIPathData("Prefab/UI/LiveSkillStuffItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData MasterMemberItem = new UIPathData("Prefab/UI/SGMasterMemberItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData MasterReserveMemberItem = new UIPathData("Prefab/UI/SGMasterCheckItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData MasterPreviewItem = new UIPathData("Prefab/UI/SGMasterItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData WorldBossListItem = new UIPathData("Prefab/UI/SGWorldBossListItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData DamageBoard = new UIPathData("Prefab/UI/SGDamageBoard", UIPathData.UIType.TYPE_ITEM, "DamageBoard");
    public static UIPathData BackCamera = new UIPathData("Prefab/UI/BackCamera", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData PvPRankListItem = new UIPathData("Prefab/UI/PvPRankListItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData GuildShopItem = new UIPathData("Prefab/UI/SGGuildGood", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData PowerPushListItem = new UIPathData("Prefab/UI/PowerPushItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData PowerPushReliveListItem = new UIPathData("Prefab/UI/SGPowerPushReliveItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData SwordsManItem = new UIPathData("Prefab/UI/SwordsManItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData SwordsManShopItem = new UIPathData("Prefab/UI/SGSwordsManShopItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData CangKuItem = new UIPathData("Prefab/UI/CangKuItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData CangKuBackItem = new UIPathData("Prefab/UI/CangKuBackItem", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData MasterShopItem = new UIPathData("Prefab/UI/SGMasterGood", UIPathData.UIType.TYPE_ITEM);
    public static UIPathData GuildMakeItem = new UIPathData("Prefab/UI/SGGuildMakeItem", UIPathData.UIType.TYPE_ITEM);
    

    //baseui
    public static UIPathData TargetFrameRoot = new UIPathData("Prefab/UI/SGTargetFrameRoot", UIPathData.UIType.TYPE_BASE, "MainBaseUI");
    public static UIPathData PlayerFrameRoot = new UIPathData("Prefab/UI/SGPlayerFrameRoot", UIPathData.UIType.TYPE_BASE, "MainBaseUI");
    public static UIPathData MenuBarRoot = new UIPathData("Prefab/UI/SGMenuBarRoot", UIPathData.UIType.TYPE_BASE, "MainBaseUI");
    public static UIPathData FunctionButtonRoot = new UIPathData("Prefab/UI/SGFunctionButtonRoot", UIPathData.UIType.TYPE_BASE, "MainBaseUI");
    public static UIPathData MissionDialogAndLeftTabsRoot = new UIPathData("Prefab/UI/SGMissionDialogAndLeftTabsRoot", UIPathData.UIType.TYPE_BASE, "MainBaseUI");
    public static UIPathData ChatFrameRoot = new UIPathData("Prefab/UI/SGChatFrameRoot", UIPathData.UIType.TYPE_BASE, "MainBaseUI");
    public static UIPathData ExpRoot = new UIPathData("Prefab/UI/SGExpRoot", UIPathData.UIType.TYPE_BASE, "MainBaseUI");
    public static UIPathData JoyStickRoot = new UIPathData("Prefab/UI/SGJoyStickRoot", UIPathData.UIType.TYPE_BASE, "MainBaseUI");
    public static UIPathData SkillBarRoot = new UIPathData("Prefab/UI/SGSkillBarRoot", UIPathData.UIType.TYPE_BASE, "MainBaseUI");
    public static UIPathData PlayerHitsRoot = new UIPathData("Prefab/UI/SGPlayerHitsRoot", UIPathData.UIType.TYPE_BASE, "MainBaseUI");
    public static UIPathData SkillProgress = new UIPathData("Prefab/UI/SGSkillProgress", UIPathData.UIType.TYPE_BASE, "MainBaseUI");
    public static UIPathData CollectItemSlider = new UIPathData("Prefab/UI/SGCollectItemSliderRoot", UIPathData.UIType.TYPE_BASE, "MainBaseUI");
    public static UIPathData RechargeBar = new UIPathData("Prefab/UI/SGRechargeBar", UIPathData.UIType.TYPE_BASE, "MainBaseUI");
    public static UIPathData PkNotice = new UIPathData("Prefab/UI/PKNoticeRoot", UIPathData.UIType.TYPE_BASE, "MainBaseUI");

    public static UIPathData MessageBox = new UIPathData("Prefab/UI/SGMessageBoxRoot", UIPathData.UIType.TYPE_MESSAGE);
    // popwindow
    public static UIPathData SysShop = new UIPathData("Prefab/UI/SGSysShopController", UIPathData.UIType.TYPE_POP, "SystemShop", true);
	public static UIPathData Notice = new UIPathData("Prefab/UI/SGNotice", UIPathData.UIType.TYPE_POP);
    public static UIPathData SceneMapRoot = new UIPathData("Prefab/UI/SGSceneMapRoot", UIPathData.UIType.TYPE_POP);
    public static UIPathData WorldMapWindow = new UIPathData("Prefab/UI/SGWorldMapWindow", UIPathData.UIType.TYPE_POP, null, false, false);
    //public static UIPathData AutoFightRoot = new UIPathData("Prefab/UI/AutoFightRoot", UIPathData.UIType.TYPE_POP);
    public static UIPathData MissionInfoController = new UIPathData("Prefab/UI/SGMissionInfoRoot", UIPathData.UIType.TYPE_POP, null, false, false);
	public static UIPathData MarryRoot = new UIPathData("Prefab/UI/MarryDialogueRoot", UIPathData.UIType.TYPE_POP);
    public static UIPathData OptionDialogRoot = new UIPathData("Prefab/UI/SGOptionDialogRoot", UIPathData.UIType.TYPE_POP);
    public static UIPathData ChatInfoRoot = new UIPathData("Prefab/UI/SGChatInfoRoot", UIPathData.UIType.TYPE_POP, null, false, false);
    public static UIPathData ServerChoose = new UIPathData("Prefab/UI/SGServerChooseController", UIPathData.UIType.TYPE_POP);
    public static UIPathData RoleChoose = new UIPathData("Prefab/UI/SGRoleChoose", UIPathData.UIType.TYPE_POP);
    public static UIPathData RoleCreate = new UIPathData("Prefab/UI/SGRoleCreate", UIPathData.UIType.TYPE_POP); 
    public static UIPathData ChannelChange = new UIPathData("Prefab/UI/SGChangeChannel", UIPathData.UIType.TYPE_POP);
    public static UIPathData Activity = new UIPathData("Prefab/UI/SGActivityController", UIPathData.UIType.TYPE_POP, null, false, false);
    public static UIPathData PKSetInfo = new UIPathData("Prefab/UI/SGPKSetRoot", UIPathData.UIType.TYPE_POP,null,false,false);
    
	public static UIPathData AwardRoot = new UIPathData("Prefab/UI/RewardRoot", UIPathData.UIType.TYPE_POP, null, false, false);
	//===
	public static UIPathData RewardRoot = new UIPathData("Prefab/UI/SGRewardRoot", UIPathData.UIType.TYPE_POP, null, false, false);
    
	public static UIPathData MoneyTreeRoot = new UIPathData("Prefab/UI/MoneyTreeRoot", UIPathData.UIType.TYPE_POP, null, false, false);
    public static UIPathData DailyDrawRoot = new UIPathData("Prefab/UI/DailyLuckyDrawRoot", UIPathData.UIType.TYPE_POP,null,false,false);
    public static UIPathData HelpController = new UIPathData("Prefab/UI/HelpController", UIPathData.UIType.TYPE_POP);
    public static UIPathData YuanBaoShop = new UIPathData("Prefab/UI/SGYBShop", UIPathData.UIType.TYPE_POP, null, false, false);
    public static UIPathData RankRoot = new UIPathData("Prefab/UI/SGRankRoot", UIPathData.UIType.TYPE_POP);
    public static UIPathData CreateGuild = new UIPathData("Prefab/UI/SGGuildCreate", UIPathData.UIType.TYPE_POP);
    public static UIPathData ZiPaiRoot = new UIPathData("Prefab/UI/ZiPaiRoot", UIPathData.UIType.TYPE_POP);
    public static UIPathData CheckPowerRoot = new UIPathData("Prefab/UI/SGCheckPowerRoot", UIPathData.UIType.TYPE_POP); 
    public static UIPathData VictoryScoreRoot = new UIPathData("Prefab/UI/SGVictoryScoreRoot", UIPathData.UIType.TYPE_POP);
	public static UIPathData SNSRoot = new UIPathData("Prefab/UI/SNS", UIPathData.UIType.TYPE_POP);
//	public static UIPathData GongGaoRoot = new UIPathData("Prefab/UI/SGNotice", UIPathData.UIType.TYPE_POP);
	public static UIPathData SNSShareCodeRoot = new UIPathData("Prefab/UI/SNSBonusCode", UIPathData.UIType.TYPE_POP);
	public static UIPathData SNSShareRoot = new UIPathData("Prefab/UI/SNSShare", UIPathData.UIType.TYPE_POP);
    public static UIPathData CDkeyRoot = new UIPathData("Prefab/UI/jihuoma", UIPathData.UIType.TYPE_POP);
    public static UIPathData Recharge = new UIPathData("Prefab/UI/ReChargeCollect", UIPathData.UIType.TYPE_POP);
    public static UIPathData ChangeName = new UIPathData("Prefab/UI/SGChangeName", UIPathData.UIType.TYPE_POP);
    public static UIPathData ChargeActivity = new UIPathData("Prefab/UI/ChargeActivity", UIPathData.UIType.TYPE_POP);
    public static UIPathData CangKu = new UIPathData("Prefab/UI/CangKu", UIPathData.UIType.TYPE_POP);
    public static UIPathData BlackMarket = new UIPathData("Prefab/UI/BlackMarket", UIPathData.UIType.TYPE_POP);
	public static UIPathData VipRoot = new UIPathData("Prefab/UI/SGVipRoot", UIPathData.UIType.TYPE_POP);
	public static UIPathData PackageRoot  = new UIPathData("Prefab/UI/SGPackageRoot", UIPathData.UIType.TYPE_POP);
    public static UIPathData ShareRoot = new UIPathData("Prefab/UI/ShareRoot", UIPathData.UIType.TYPE_POP);
	public static UIPathData DivinationRoot = new UIPathData("Prefab/UI/SGDivinationRoot", UIPathData.UIType.TYPE_POP,null,false,false);

    // STORY
    public static UIPathData StoryDialogRoot = new UIPathData("Prefab/UI/SGStoryDialogRoot", UIPathData.UIType.TYPE_STORY);
    public static UIPathData ShiCiRoot = new UIPathData("Prefab/UI/ShiCiRoot", UIPathData.UIType.TYPE_STORY);
    public static UIPathData JianPuRoot = new UIPathData("Prefab/UI/JianPuRoot", UIPathData.UIType.TYPE_STORY);

    // TIPBOX
    public static UIPathData ItemTooltipsRoot = new UIPathData("Prefab/UI/SGItemTooltipsRoot", UIPathData.UIType.TYPE_TIP, "TooltipsGroup");
    public static UIPathData EquipTooltipsRoot = new UIPathData("Prefab/UI/SGEquipTooltipsRoot", UIPathData.UIType.TYPE_TIP, "TooltipsGroup");
    public static UIPathData NumChoose = new UIPathData("Prefab/UI/SGNumChooseController", UIPathData.UIType.TYPE_TIP);
    public static UIPathData QueueWindow = new UIPathData("Prefab/UI/QueueWindow", UIPathData.UIType.TYPE_TIP);
	public static UIPathData RelationNamePopWindow = new UIPathData("Prefab/UI/SGRelationNamePopWindow", UIPathData.UIType.TYPE_TIP, "Relation");
    public static UIPathData AutoDrug = new UIPathData("Prefab/UI/SGAutoDrug", UIPathData.UIType.TYPE_TIP);
    public static UIPathData SwordsManTooltipsRoot = new UIPathData("Prefab/UI/SGSwordsManToolTips", UIPathData.UIType.TYPE_TIP, "TooltipsGroup");
    public static UIPathData MoneyTipRoot = new UIPathData("Prefab/UI/SGMoneyTipRoot", UIPathData.UIType.TYPE_TIP, "TooltipsGroup");
        
    //MenuPop
    public static UIPathData Belle = new UIPathData("Prefab/UI/BelleController", UIPathData.UIType.TYPE_MENUPOP);
    public static UIPathData EquipStren = new UIPathData("Prefab/UI/SGEquipStrengthenRoot", UIPathData.UIType.TYPE_MENUPOP, null, false, false);
    public static UIPathData BackPackRoot = new UIPathData("Prefab/UI/SGBackPackRoot", UIPathData.UIType.TYPE_MENUPOP, null, false, false);
    public static UIPathData RelationRoot = new UIPathData("Prefab/UI/SGRelationRoot", UIPathData.UIType.TYPE_MENUPOP, "Relation", true);
    public static UIPathData MissionLogRoot = new UIPathData("Prefab/UI/SGMissionLogRoot", UIPathData.UIType.TYPE_MENUPOP);
    public static UIPathData PartnerAndMountRoot = new UIPathData("Prefab/UI/SGPartnerAndMountRoot", UIPathData.UIType.TYPE_MENUPOP, "Partner", true, true);
    public static UIPathData RoleView = new UIPathData("Prefab/UI/SGRoleView", UIPathData.UIType.TYPE_MENUPOP,null,false,false);
    public static UIPathData SkillInfo = new UIPathData("Prefab/UI/SGSkillRoot", UIPathData.UIType.TYPE_MENUPOP);
    public static UIPathData SystemAndAutoFight = new UIPathData("Prefab/UI/SGSystemRoot", UIPathData.UIType.TYPE_MENUPOP);
    public static UIPathData ConsignSaleRoot = new UIPathData("Prefab/UI/SGConsignSaleRoot", UIPathData.UIType.TYPE_MENUPOP, "ConsignSale", true);
    public static UIPathData MasterAndGuildRoot = new UIPathData("Prefab/UI/SGMasterAndGuildRoot", UIPathData.UIType.TYPE_MENUPOP, "MasterAndGuildRoot", true);
    public static UIPathData Restaurant = new UIPathData("Prefab/UI/SGRestaurant", UIPathData.UIType.TYPE_MENUPOP);
    public static UIPathData LivingSkill = new UIPathData("Prefab/UI/SGLiveskill", UIPathData.UIType.TYPE_MENUPOP);
    public static UIPathData OtherRoleView = new UIPathData("Prefab/UI/SGOtherRoleView", UIPathData.UIType.TYPE_MENUPOP, null, false, false);
    public static UIPathData SwordsManLevelUpRoot = new UIPathData("Prefab/UI/SGSwordsManLevelupController", UIPathData.UIType.TYPE_MENUPOP, null, false, false);
    public static UIPathData SwordsManShopRoot = new UIPathData("Prefab/UI/SGSwordsManShop", UIPathData.UIType.TYPE_MENUPOP, null, false, false);
    public static UIPathData SwordsManRoot = new UIPathData("Prefab/UI/SGSwordsManController", UIPathData.UIType.TYPE_MENUPOP, null, false, true);
    public static UIPathData Biography = new UIPathData("Prefab/UI/SGBiographyController", UIPathData.UIType.TYPE_MENUPOP);

    //MessageUI
    public static UIPathData CentreNotice = new UIPathData("Prefab/UI/SGCentreNotice", UIPathData.UIType.TYPE_MESSAGE);
    public static UIPathData RollNotice = new UIPathData("Prefab/UI/RollNotice", UIPathData.UIType.TYPE_MESSAGE);
    public static UIPathData PopMenuRoot = new UIPathData("Prefab/UI/SGPopMenuRoot", UIPathData.UIType.TYPE_MESSAGE);
    public static UIPathData PowerRemindRoot = new UIPathData("Prefab/UI/PowerRemindRoot", UIPathData.UIType.TYPE_MESSAGE);
    public static UIPathData EquipRemindRoot = new UIPathData("Prefab/UI/SGEquipRemindRoot", UIPathData.UIType.TYPE_MESSAGE);
	public static UIPathData MountRemindRoot = new UIPathData("Prefab/UI/SGMountRemindRoot", UIPathData.UIType.TYPE_MESSAGE);
    public static UIPathData UseItemRemindRoot = new UIPathData("Prefab/UI/UseItemRemindRoot", UIPathData.UIType.TYPE_MESSAGE);
    public static UIPathData ItemRemindRoot = new UIPathData("Prefab/UI/ItemRemindRoot", UIPathData.UIType.TYPE_MESSAGE);
    public static UIPathData CountDown = new UIPathData("Prefab/UI/CountDown", UIPathData.UIType.TYPE_MESSAGE);
    public static UIPathData ChallengeRewardRoot = new UIPathData("Prefab/UI/ChallengeRewardRoot", UIPathData.UIType.TYPE_MESSAGE);
    public static UIPathData LevelupTip = new UIPathData("Prefab/UI/SGLevelUpTip", UIPathData.UIType.TYPE_MESSAGE);
    public static UIPathData NewPlayerGuidRoot = new UIPathData("Prefab/UI/SGNewPlayerGuidRoot", UIPathData.UIType.TYPE_MESSAGE);
	public static UIPathData NewItemGetRoot = new UIPathData("Prefab/UI/SGNewItemGetRoot", UIPathData.UIType.TYPE_MESSAGE);
	public static UIPathData BonusItemGetRoot = new UIPathData("Prefab/UI/SGBonusItemGetRoot", UIPathData.UIType.TYPE_MENUPOP,null,false,false);
    public static UIPathData ShareTargetChooseRoot = new UIPathData("Prefab/UI/SGShareTargetChooseRoot", UIPathData.UIType.TYPE_MESSAGE);
    public static UIPathData UpdateLoadingBar = new UIPathData("Prefab/UI/UpdateLoadingBar", UIPathData.UIType.TYPE_MESSAGE);
    public static UIPathData MercenaryWindowRoot = new UIPathData("Prefab/UI/MercenaryWindowRoot", UIPathData.UIType.TYPE_MESSAGE);
	public static UIPathData WorldBossWindowRoot = new UIPathData("Prefab/UI/SGWorldBossRoot", UIPathData.UIType.TYPE_POP);//UIPathData.UIType.TYPE_POP
	//public static UIPathData WorldBossWindowRoot = new UIPathData("Prefab/UI/SGWorldBossRoot", UIPathData.UIType.TYPE_MESSAGE);
    public static UIPathData GuilWarRetWindow = new UIPathData("Prefab/UI/SGGuildWarRetInfoRoot", UIPathData.UIType.TYPE_MESSAGE);
    public static UIPathData GuilWarPushMessage = new UIPathData("Prefab/UI/SGGuildWarInfoBt", UIPathData.UIType.TYPE_MESSAGE);
   

    //DeathUI
    public static UIPathData Relive = new UIPathData("Prefab/UI/SGRelive", UIPathData.UIType.TYPE_DEATH);

    public static UIPathData DyingBlood = new UIPathData("Prefab/UI/SGDyingBlood", UIPathData.UIType.TYPE_BASE);
    public static UIPathData UIEventMask = new UIPathData("Prefab/UI/SGUIEventMask", UIPathData.UIType.TYPE_BASE);
    public static UIPathData SkipStory = new UIPathData("Prefab/UI/SGSkipStory",UIPathData.UIType.TYPE_STORY);
    public static UIPathData Black = new UIPathData("Prefab/UI/SGBlack",UIPathData.UIType.TYPE_STORY);
    public static UIPathData XinShouAnim = new UIPathData("Prefab/UI/SGXinShouAnim",UIPathData.UIType.TYPE_STORY);
	public static UIPathData OneMonthAgo = new UIPathData("Prefab/UI/SGOneMonthAgo", UIPathData.UIType.TYPE_POP);
    public static UIPathData AutoMedicine = new UIPathData("Prefab/UI/SGAutoMedicine", UIPathData.UIType.TYPE_BASE);

    public static UIPathData AutoFightBtn = new UIPathData("Prefab/UI/SGAutoFightBtn", UIPathData.UIType.TYPE_BASE);
}
  