using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public delegate void CollectedItem();
    public List<QuestBase> QuestTable;

    public void InitializeQuestTable(ApplicationModel applicationModelInstance_)
    {
        /* Create Main Quests */
        QuestKill _mainQuestOne = new QuestKill();
        _mainQuestOne.ConstructQuest();
        _mainQuestOne.Title = "Primeira Quest";
        _mainQuestOne.Description = "Primeira quest de todas as quests do jogo";
        _mainQuestOne.QuestActionType = QuestActionTypeEnum.KILL;
        _mainQuestOne.QuestType = QuestTypeEnum.MAIN;
        _mainQuestOne.Active = true;
        _mainQuestOne.TargetAmount = 3;
        _mainQuestOne.CurrentAmount = 0;
        //_mainQuestOne.TargetID = EnemyTable.GetMainQuestOneEnemyId;

        QuestBase _mainQuestTwo = new QuestBase();
        _mainQuestOne.QuestChild = _mainQuestTwo;
        _mainQuestTwo.QuestFather = _mainQuestOne;
        _mainQuestTwo.Title = "Segunda Quest";
        _mainQuestTwo.Description = "Segunda quest de todas as quests principais do Jogo";
        _mainQuestTwo.QuestType = QuestTypeEnum.MAIN;
        _mainQuestTwo.QuestActionType = QuestActionTypeEnum.DELIVER;

        QuestBase _mainQuestThre = new QuestBase();
        _mainQuestTwo.QuestChild = _mainQuestThre;
        _mainQuestThre.QuestFather = _mainQuestTwo;
    }

}

