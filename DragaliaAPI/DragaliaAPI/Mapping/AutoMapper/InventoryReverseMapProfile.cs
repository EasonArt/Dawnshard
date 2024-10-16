﻿using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Mapping.AutoMapper;

public class InventoryReverseMapProfile : Profile
{
    public InventoryReverseMapProfile()
    {
        this.AddGlobalIgnore("ViewerId");
        this.AddGlobalIgnore("Owner");

        this.CreateMap<MaterialList, DbPlayerMaterial>();
        this.CreateMap<DragonGiftList, DbPlayerDragonGift>();
        this.CreateMap<EquipStampList, DbEquippedStamp>();
    }
}
