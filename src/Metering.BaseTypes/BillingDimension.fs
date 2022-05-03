﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Metering.BaseTypes

// https://docs.microsoft.com/en-us/azure/marketplace/azure-app-metered-billing#billing-dimensions

/// Defines a custom unit by which the ISV can emit usage events. 
/// Billing dimensions are also used to communicate to the customer about how they will be billed for using the software. 
type BillingDimensions =
    { Dimensions: Map<DimensionId, Quantity> }
          
    static member create (dimensions: Map<DimensionId, Quantity>) = 
        { Dimensions = dimensions}

    member this.currentMeterValues (now: MeteringDateTime) : CurrentMeterValues = 
        let toIncluded (quantity: Quantity) : MeterValue = 
            IncludedQuantity { Quantity = quantity; Created = now; LastUpdate = now }
            
        this.Dimensions
        |> Map.toSeq
        |> Seq.map(fun (dimensionId, quantity) -> (dimensionId, quantity |> toIncluded))
        |> Map.ofSeq
