﻿using MessagePack;

namespace DragaliaAPI.Photon.Plugin.Plugins.GameLogic.Events
{
    [MessagePackObject]
    public class ClearQuestRequest : EventBase<ClearQuestRequest>
    {
        [Key(1)]
        public required byte[] RecordMultiRequest { get; set; }
    }
}
