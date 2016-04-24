using System;

public class QuestKill : QuestBase
{
    public int TargetAmount { get; set; }
    public int CurrentAmount { get; set; }
    public Guid[] TargetID { get; set; }

    public override void ConstructQuest()
    {
        base.ConstructQuest();

        // CODE HERE


        CallQuestActivate();
    }

    public override void UpdateQuest()
    {
        if (true)
            CallQuestUpdate();
/*        else
            CallQuestFinished();

        base.UpdateQuest();*/
    }

    public override void CancelQuest()
    {
        // CODE HERE

        base.CancelQuest();
    }
}

