﻿using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Dragons;

[Route("dragon")]
public class DragonController : DragaliaControllerBase
{
    private readonly IDragonService dragonService;

    public DragonController(IDragonService dragonService)
    {
        this.dragonService = dragonService;
    }

    [Route("buildup")]
    [HttpPost]
    public async Task<DragaliaResult> Buildup(
        [FromBody] DragonBuildupRequest request,
        CancellationToken cancellationToken
    )
    {
        return this.Ok(await this.dragonService.DoBuildup(request, cancellationToken));
    }

    [Route("reset_plus_count")]
    [HttpPost]
    public async Task<DragaliaResult> DragonResetPlusCount(
        [FromBody] DragonResetPlusCountRequest request,
        CancellationToken cancellationToken
    )
    {
        return this.Ok(await this.dragonService.DoDragonResetPlusCount(request, cancellationToken));
    }

    [Route("limit_break")]
    [HttpPost]
    public async Task<DragaliaResult> DragonLimitBreak(
        [FromBody] DragonLimitBreakRequest request,
        CancellationToken cancellationToken
    )
    {
        return this.Ok(await this.dragonService.DoDragonLimitBreak(request, cancellationToken));
    }

    [Route("get_contact_data")]
    [HttpPost]
    public async Task<DragaliaResult> DragonGetContactData()
    {
        return this.Ok(await this.dragonService.DoDragonGetContactData());
    }

    [Route("buy_gift_to_send_multiple")]
    [HttpPost]
    public async Task<DragaliaResult> DragonBuyGiftToSendMultiple(
        [FromBody] DragonBuyGiftToSendMultipleRequest request,
        CancellationToken cancellationToken
    )
    {
        return this.Ok(
            await this.dragonService.DoDragonBuyGiftToSendMultiple(request, cancellationToken)
        );
    }

    [Route("buy_gift_to_send")]
    [HttpPost]
    public async Task<DragaliaResult> DragonBuyGiftToSend(
        [FromBody] DragonBuyGiftToSendRequest request,
        CancellationToken cancellationToken
    )
    {
        DragonBuyGiftToSendMultipleResponse resultData =
            await this.dragonService.DoDragonBuyGiftToSendMultiple(
                new DragonBuyGiftToSendMultipleRequest()
                {
                    DragonId = request.DragonId,
                    DragonGiftIdList = new List<DragonGifts>() { request.DragonGiftId },
                },
                cancellationToken
            );
        return this.Ok(
            new DragonBuyGiftToSendResponse()
            {
                DragonContactFreeGiftCount = resultData.DragonContactFreeGiftCount,
                EntityResult = resultData.EntityResult,
                IsFavorite = resultData.DragonGiftRewardList.First().IsFavorite,
                ReturnGiftList = resultData.DragonGiftRewardList.First().ReturnGiftList,
                RewardReliabilityList = resultData
                    .DragonGiftRewardList.First()
                    .RewardReliabilityList,
                ShopGiftList = resultData.ShopGiftList,
                UpdateDataList = resultData.UpdateDataList,
            }
        );
    }

    [Route("send_gift_multiple")]
    [HttpPost]
    public async Task<DragaliaResult> DragonSentGiftMultiple(
        [FromBody] DragonSendGiftMultipleRequest request,
        CancellationToken cancellationToken
    )
    {
        return this.Ok(
            await this.dragonService.DoDragonSendGiftMultiple(request, cancellationToken)
        );
    }

    [Route("send_gift")]
    [HttpPost]
    public async Task<DragaliaResult> DragonSendGift(
        [FromBody] DragonSendGiftRequest request,
        CancellationToken cancellationToken
    )
    {
        DragonSendGiftMultipleResponse resultData =
            await this.dragonService.DoDragonSendGiftMultiple(
                new DragonSendGiftMultipleRequest()
                {
                    DragonId = request.DragonId,
                    DragonGiftId = request.DragonGiftId,
                    Quantity = 1,
                },
                cancellationToken
            );
        return this.Ok(
            new DragonSendGiftResponse()
            {
                IsFavorite = resultData.IsFavorite,
                ReturnGiftList = resultData.ReturnGiftList,
                RewardReliabilityList = resultData.RewardReliabilityList,
                UpdateDataList = resultData.UpdateDataList,
            }
        );
    }

    [Route("set_lock")]
    [HttpPost]
    public async Task<DragaliaResult> DragonSetLock(
        [FromBody] DragonSetLockRequest request,
        CancellationToken cancellationToken
    )
    {
        return this.Ok(await this.dragonService.DoDragonSetLock(request, cancellationToken));
    }

    [Route("sell")]
    [HttpPost]
    public async Task<DragaliaResult> DragonSell(
        [FromBody] DragonSellRequest request,
        CancellationToken cancellationToken
    )
    {
        return this.Ok(await this.dragonService.DoDragonSell(request, cancellationToken));
    }
}
