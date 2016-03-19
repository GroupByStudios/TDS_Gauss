using System;
using System.Collections;

public enum QuestTypeEnum
{
    MAIN = 0,
    SIDE = 1
}

public enum QuestActionTypeEnum
{
    KILL = 0,
    DELIVER = 1,
    GATHER = 2,
    ESCORT = 3,
    PUZZLE = 4
}

/// <summary>
/// Interface que deve ser implementada nos itens que serao utilizados como recompensas de Quest
/// </summary>
public interface QuestRewardItem
{
    
}

/// <summary>
/// Interface que deve ser implementada nos itens que serao utilizados como coletavies de quest
/// </summary>
public interface QuestCollectableItem
{

}

/// <summary>
/// Interface que deve ser implementada nos tipos de monstros que serao marcados como killable nas quests
/// </summary>
public interface QuestKillable
{
}


/// <summary>
/// Classe base de todas as quests
/// </summary>
public class QuestBase
{
    public delegate void QuestActivate(QuestBase questActivated_);
    public delegate void QuestUpdate(QuestBase questUpdated_);
    public delegate void QuestFinished(QuestBase questFinished_);
    public delegate void QuestCancel(QuestBase questCancelled_);
    public event QuestActivate OnQuestActivate;
    public event QuestUpdate OnQuestUpdate;
    public event QuestFinished OnQuestFinished;
    public event QuestCancel OnQuestCancel;

    public int ID { get; set; }
    public Guid QuestID { get; set; }
    public QuestBase QuestFather { get; set; }
    public QuestBase QuestChild { get; set; }
    public QuestTypeEnum QuestType { get; set; }
    public QuestActionTypeEnum QuestActionType { get; set; }
    public QuestRewardItem[] QuestReward { get; set; }
    public float QuestCurrencyReward { get; set; }
    public bool Active { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    /// <summary>
    /// Método responsável por construir a base da Quest. SE for feito override deve ser chamado primeiro
    /// </summary>
    public virtual void ConstructQuest()
    {
        QuestID = Guid.NewGuid();
    }

    /// <summary>
    /// Método responsável por atualizar as informacoes da Quest se for feito override deve ser o ultimo chamado
    /// </summary>
    public virtual void UpdateQuest()
    {
    }

    /// <summary>
    /// Methodo responsavel por cancelar a quest deve ser o ultimo chamado se for feito o override
    /// </summary>
    /// <returns><c>true</c> if this instance cancel quest; otherwise, <c>false</c>.</returns>
    public virtual void CancelQuest()
    {
        Active = false;
        CallQuestCancel();
    }

    protected void CallUpdateQuest()
    {
        if (OnQuestUpdate != null)
            OnQuestUpdate(this);
    }

    protected void CallQuestActivate()
    {
        if (OnQuestActivate != null)
            OnQuestActivate(this);
    }

    protected void CallQuestUpdate()
    {
        if (OnQuestUpdate != null)
            OnQuestUpdate(this);
    }

    protected void CallQuestFinished()
    {
        if (OnQuestFinished != null)
            OnQuestFinished(this);
    }

    protected void CallQuestCancel()
    {
        if (OnQuestCancel != null)
            OnQuestCancel(this);
    }
}




