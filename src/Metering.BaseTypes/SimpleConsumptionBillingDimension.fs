﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Metering.BaseTypes

/// The 'simple consumption' represents the most simple Azure Marketplace metering model. 
/// There is a dimension with included quantities, and once the included quantities are consumed, the overage starts counting.
type SimpleConsumptionBillingDimension = 
    { /// Application-internal name of the meter / billing dimension. 
      MeterName: ApplicationInternalMeterName

      /// The dimension as Marketplace knows it.
      Dimension: DimensionId

      /// The dimensions configured
      IncludedQuantity: Quantity }

type WaterfallRange =
    { DimensionId: DimensionId 
      LowerIncluding: Quantity
      UpperExcluding: Quantity }

type WaterfallEntry =
    | Included of Quantity
    | Below of WaterfallRange
    | Overage of DimensionId

module WaterfallEntry =
    let included (quantity: uint) =
        quantity |> Quantity.create |> Included

    let below dimension (lowerIncluding:uint) (upperExcluding: uint) =
        { DimensionId = dimension |> DimensionId.create
          LowerIncluding = lowerIncluding |> Quantity.create 
          UpperExcluding = upperExcluding |> Quantity.create } 
        |> Below
    let overage dimension =
        dimension |> DimensionId.create |> Overage

type WaterfallBilling = 
    { /// Application-internal name of the meter / billing dimension. 
      MeterName: ApplicationInternalMeterName
      
      WaterfallDimensions: WaterfallEntry list

      IncludedQuantity: Quantity }
