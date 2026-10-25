using System.Collections.Generic;
using Nitrox.Model.Subnautica.DataStructures.GameLogic;
using Nitrox.Server.Subnautica.Models.GameLogic.Unlockables;
using Nitrox.Server.Subnautica.Services;

namespace Nitrox.Server.Subnautica.Models.GameLogic;

internal sealed class GameData
{
    public PdaStateData PDAState { get; set; }

    public StoryGoalData StoryGoals { get; set; }

    public StoryTimingData StoryTiming { get; set; }

    public List<ResourceTrackerEntry> ResourceTrackerEntries { get; set; } = [];

    public static GameData From(PdaManager pdaManager, StoryGoalData storyGoals, StoryScheduler storyScheduler, StoryManager storyManager, TimeService timeService, ResourceTrackerManager resourceTrackerManager)
    {
        return new GameData
        {
            PDAState = pdaManager.GetPdaStateCopy(),
            StoryGoals = StoryGoalData.From(storyGoals, storyScheduler),
            StoryTiming = StoryTimingData.From(storyManager, timeService),
            ResourceTrackerEntries = resourceTrackerManager.GetEntries()
        };
    }
}
