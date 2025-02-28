﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Metering.BaseTypes

open NodaTime

type BillingPeriodResult =
    /// The given date is before the customer subscribed to the offer.
    | DateBeforeSubscription 
    /// The date belongs to a previous BillingPeriod.
    | DateBelongsToPreviousBillingPeriod
    /// The date belongs to the current BillingPeriod.
    | SameBillingPeriod
    /// The date is n BillingPeriods ago
    | BillingPeriodsAgo of uint

type CloseBillingPeriod =
    | KeepOpen
    | Close

type BillingPeriod = // Each time the subscription is renewed, a new billing period start
    { Start: MeteringDateTime
      End: MeteringDateTime
      Index: uint }

    /// Compute the n'th BillingPeriod for a given subscription.
    static member createFromIndex (subscription : Subscription) (n: uint) : BillingPeriod =
        let period : (uint -> Period) = subscription.RenewalInterval.add
        let add (period: Period) (x: MeteringDateTime) : MeteringDateTime =
            let r = x.LocalDateTime + period
            MeteringDateTime(r, DateTimeZone.Utc, Offset.Zero)

        { Start = subscription.SubscriptionStart |> add (period (n))
          End = subscription.SubscriptionStart |> add (period (n+1u) - Period.FromDays(1) - Period.FromSeconds(1L))
          Index = n }

    /// Whether the given dateTime is in the given BillingPeriod
    static member isInBillingPeriod ({ Start = s; End = e }: BillingPeriod) (dateTime: MeteringDateTime) : bool =
        s.LocalDateTime <= dateTime.LocalDateTime && dateTime.LocalDateTime <= e.LocalDateTime

    static member previousBillingIntervalCanBeClosedNewEvent (previous: MeteringDateTime) (eventTime: MeteringDateTime) : CloseBillingPeriod =
        if previous.Hour <> eventTime.Hour || eventTime - previous >= Duration.FromHours(1.0)
        then Close
        else KeepOpen
