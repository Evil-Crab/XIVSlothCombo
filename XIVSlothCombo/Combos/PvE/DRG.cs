using Dalamud.Game.ClientState.JobGauge.Types;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.Core;

namespace XIVSlothCombo.Combos.PvE
{
    internal static class DRG
    {
        public const byte ClassID = 4;
        public const byte JobID = 22;

        public const uint
            TrueNorth = 7546,
            PiercingTalon = 90,
            LanceCharge = 85,
            DragonSight = 7398,
            BattleLitany = 3557,
            Jump = 92,
            LifeSurge = 83,
            HighJump = 16478,
            MirageDive = 7399,
            BloodOfTheDragon = 3553,
            Stardiver = 16480,
            CoerthanTorment = 16477,
            DoomSpike = 86,
            SonicThrust = 7397,
            ChaosThrust = 88,
            RaidenThrust = 16479,
            TrueThrust = 75,
            Disembowel = 87,
            FangAndClaw = 3554,
            WheelingThrust = 3556,
            FullThrust = 84,
            VorpalThrust = 78,
            WyrmwindThrust = 25773,
            DraconianFury = 25770,
            ChaoticSpring = 25772,
            DragonfireDive = 96,
            SpineshatterDive = 95,
            Geirskogul = 3555,
            Nastrond = 7400,
            HeavensThrust = 25771;

        public static class Buffs
        {
            public const ushort
                TrueNorth = 1250,
                LanceCharge = 1864,
                RightEye = 1910,
                BattleLitany = 786,
                SharperFangAndClaw = 802,
                EnhancedWheelingThrust = 803,
                DiveReady = 1243,
                RaidenThrustReady = 1863,
                PowerSurge = 2720,
                LifeSurge = 116,
                DraconianFire = 1863;
        }

        public static class Debuffs
        {
            public const ushort
                ChaosThrust = 118,
                ChaoticSpring = 2719;
        }

        public static class Config
        {
            public const string
                DRG_ST_DiveOptions = "DRG_ST_DiveOptions",
                DRG_AOE_DiveOptions = "DRG_AOE_DiveOptions";
        }

        internal class DRG_STCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_STCombo;
            internal static bool inOpener = false;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                var gauge = GetJobGauge<DRGGauge>();
                bool openerReady = IsOffCooldown(LanceCharge) && IsOffCooldown(BattleLitany);
                var DiveOptions = PluginConfiguration.GetCustomIntValue(Config.DRG_ST_DiveOptions);

                if (actionID is FullThrust)
                {
                    // Piercing Talon Uptime Option
                    if (IsEnabled(CustomComboPreset.DRG_ST_RangedUptime) && LevelChecked(PiercingTalon) && !InMeleeRange() && HasBattleTarget())
                        return PiercingTalon;

                    // Lvl88+ Opener
                    if (!InCombat() && IsEnabled(CustomComboPreset.DRG_ST_Opener) && level >= 88)
                    {
                        inOpener = false;

                        if (HasEffect(Buffs.TrueNorth) && openerReady)
                            inOpener = true;
                        if (inOpener)
                            return OriginalHook(TrueThrust);
                    }

                    if (InCombat())
                    {
                        if (inOpener)
                        {
                            if (IsOnCooldown(BattleLitany) && !HasEffect(Buffs.LanceCharge))
                                inOpener = false;
                            //oGCDs
                            if (CanWeave(actionID))
                            {
                                if (WasLastWeaponskill(Disembowel))
                                {
                                    if (IsOffCooldown(LanceCharge))
                                        return LanceCharge;
                                    if (IsOffCooldown(DragonSight))
                                        return DragonSight;
                                }

                                if (WasLastWeaponskill(ChaoticSpring) && IsOffCooldown(BattleLitany))
                                    return BattleLitany;
                                if (WasLastWeaponskill(WheelingThrust))
                                {
                                    if (IsOffCooldown(Geirskogul))
                                        return Geirskogul;
                                    if (GetRemainingCharges(LifeSurge) > 0 && !HasEffect(Buffs.LifeSurge))
                                        return LifeSurge;
                                }

                                if (WasLastWeaponskill(FangAndClaw))
                                {
                                    if (IsOffCooldown(OriginalHook(Jump)))
                                        return OriginalHook(Jump);
                                    if (GetRemainingCharges(SpineshatterDive) > 0 && !HasEffect(Buffs.DiveReady))
                                        return SpineshatterDive;
                                }

                                if (WasLastWeaponskill(RaidenThrust) && IsOffCooldown(DragonfireDive))
                                    return DragonfireDive;
                                if (WasLastWeaponskill(VorpalThrust))
                                {
                                    if (GetRemainingCharges(LifeSurge) > 0 && !HasEffect(Buffs.LifeSurge))
                                        return LifeSurge;
                                    if (HasEffect(Buffs.DiveReady))
                                        return OriginalHook(Jump);
                                }

                                if (WasLastWeaponskill(HeavensThrust) && GetRemainingCharges(SpineshatterDive) > 0 && !WasLastAction(SpineshatterDive))
                                    return SpineshatterDive;
                            }

                            //GCDs
                            if (HasEffect(Buffs.SharperFangAndClaw))
                                return FangAndClaw;
                            if (HasEffect(Buffs.EnhancedWheelingThrust))
                                return WheelingThrust;

                            if (comboTime > 0)
                            {
                                if (lastComboMove is TrueThrust or RaidenThrust && LevelChecked(Disembowel) && GetBuffRemainingTime(Buffs.PowerSurge) < 10)
                                    return Disembowel;
                                if (lastComboMove is Disembowel && LevelChecked(ChaosThrust))
                                    return OriginalHook(ChaosThrust);
                                if (lastComboMove is TrueThrust or RaidenThrust && LevelChecked(VorpalThrust))
                                    return VorpalThrust;
                                if (lastComboMove is VorpalThrust && LevelChecked(FullThrust))
                                    return OriginalHook(FullThrust);
                            }

                            return OriginalHook(TrueThrust);
                        }

                        if (!inOpener)
                        {
                            if (CanWeave(actionID, 0.5))
                            {
                                if (HasEffect(Buffs.PowerSurge))
                                {
                                    //Wyrmwind Thrust Feature
                                    if (IsEnabled(CustomComboPreset.DRG_ST_Wyrmwind) && gauge.FirstmindsFocusCount is 2)
                                        return WyrmwindThrust;

                                    //Lance Charge Feature
                                    if (IsEnabled(CustomComboPreset.DRG_ST_Lance) && LevelChecked(LanceCharge) && IsOffCooldown(LanceCharge))
                                        return LanceCharge;

                                    //Dragon Sight Feature
                                    if (IsEnabled(CustomComboPreset.DRG_ST_DragonSight) && LevelChecked(DragonSight) && IsOffCooldown(DragonSight))
                                        return DragonSight;

                                    //Battle Litany Feature
                                    if (IsEnabled(CustomComboPreset.DRG_ST_Litany) && LevelChecked(BattleLitany) && IsOffCooldown(BattleLitany) && CanWeave(actionID, 1.3))
                                        return BattleLitany;

                                    //Geirskogul and Nastrond Feature
                                    if (IsEnabled(CustomComboPreset.DRG_ST_GeirskogulNastrond) && LevelChecked(Geirskogul) && ((gauge.IsLOTDActive && IsOffCooldown(Nastrond)) || IsOffCooldown(Geirskogul)))
                                        return OriginalHook(Geirskogul);

                                    //(High) Jump Feature + Mirage Feature
                                    if ((IsEnabled(CustomComboPreset.DRG_ST_HighJump) && LevelChecked(Jump) && IsOffCooldown(OriginalHook(Jump))) ||
                                        (IsEnabled(CustomComboPreset.DRG_ST_Mirage) && LevelChecked(MirageDive) && HasEffect(Buffs.DiveReady)))
                                        return OriginalHook(Jump);

                                    //Life Surge Feature
                                    if (IsEnabled(CustomComboPreset.DRG_ST_LifeSurge) && !HasEffect(Buffs.LifeSurge) && GetRemainingCharges(LifeSurge) > 0 &&
                                        (((HasEffect(Buffs.RightEye) || HasEffect(Buffs.LanceCharge)) && lastComboMove is VorpalThrust) ||
                                        (HasEffect(Buffs.BattleLitany) && ((HasEffect(Buffs.EnhancedWheelingThrust) && WasLastWeaponskill(FangAndClaw)) || HasEffect(Buffs.SharperFangAndClaw) && WasLastWeaponskill(WheelingThrust)))))
                                        return LifeSurge;

                                    //Dives Feature
                                    if (IsEnabled(CustomComboPreset.DRG_ST_Dives))
                                    {
                                        if (DiveOptions is 0 or 1 || //Dives on cooldown
                                           (DiveOptions is 2 && gauge.IsLOTDActive && HasEffect(Buffs.BattleLitany)) || //Dives under Litany and Life of the Dragon
                                           (DiveOptions is 3 && HasEffect(Buffs.BattleLitany)) || //Dives under Litany
                                           (DiveOptions is 4 && HasEffect(Buffs.LanceCharge))) //Dives under Lance Charge Feature
                                        {
                                            if (LevelChecked(DragonfireDive) && IsOffCooldown(DragonfireDive))
                                                return DragonfireDive;
                                            if (LevelChecked(SpineshatterDive) && GetRemainingCharges(SpineshatterDive) > 0)
                                                return SpineshatterDive;
                                        }
 
                                        if (DiveOptions is 0 or 1 or 2 or 3 or 4 && gauge.IsLOTDActive && LevelChecked(Stardiver) && IsOffCooldown(Stardiver) && CanWeave(actionID, 1.3))
                                            return Stardiver;
                                    }
                                }
                            }

                            //1-2-3 Combo
                            if (HasEffect(Buffs.SharperFangAndClaw))
                                return FangAndClaw;
                            if (HasEffect(Buffs.EnhancedWheelingThrust))
                                return WheelingThrust;
                            if (comboTime > 0)
                            {
                                if (lastComboMove is TrueThrust or RaidenThrust && LevelChecked(Disembowel) && GetBuffRemainingTime(Buffs.PowerSurge) < 10)
                                    return Disembowel;
                                if (lastComboMove is Disembowel && LevelChecked(ChaosThrust))
                                    return OriginalHook(ChaosThrust);
                                if (lastComboMove is TrueThrust or RaidenThrust && LevelChecked(VorpalThrust))
                                    return VorpalThrust;
                                if (lastComboMove is VorpalThrust && LevelChecked(FullThrust))
                                    return OriginalHook(FullThrust);
                            }
                        }
                    }

                    return OriginalHook(TrueThrust);
                }

                return actionID;
            }
        }

        internal class DRG_AoECombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRG_AoECombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is CoerthanTorment)
                {
                    var gauge = GetJobGauge<DRGGauge>();
                    var DiveOptions = PluginConfiguration.GetCustomIntValue(Config.DRG_AOE_DiveOptions);

                    // Piercing Talon Uptime Option
                    if (IsEnabled(CustomComboPreset.DRG_AoE_RangedUptime) && LevelChecked(PiercingTalon) && !InMeleeRange() && HasBattleTarget())
                        return PiercingTalon;

                    if (CanWeave(actionID, 0.5))
                    {
                        if (HasEffect(Buffs.PowerSurge))
                        {
                            //Buffs AoE Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Buffs))
                            {
                                if (LevelChecked(LanceCharge) && IsOffCooldown(LanceCharge))
                                    return LanceCharge;
                                if (LevelChecked(BattleLitany) && IsOffCooldown(BattleLitany))
                                    return BattleLitany;

                                //Dragon Sight AoE Feature
                                if (IsEnabled(CustomComboPreset.DRG_AoE_DragonSight) && LevelChecked(DragonSight) && IsOffCooldown(DragonSight))
                                    return DragonSight;
                            }

                            //Geirskogul and Nastrond AoE Feature
                            if ((IsEnabled(CustomComboPreset.DRG_AoE_GeirskogulNastrond) && LevelChecked(Geirskogul) && IsOffCooldown(Geirskogul)) ||
                                (IsEnabled(CustomComboPreset.DRG_AoE_GeirskogulNastrond) && gauge.IsLOTDActive && IsOffCooldown(Nastrond)))
                                return OriginalHook(Geirskogul);

                            //(High) Jump AoE Feature + Mirage Dive Feature
                            if ((IsEnabled(CustomComboPreset.DRG_AoE_HighJump) && LevelChecked(Jump) && IsOffCooldown(OriginalHook(Jump)) && CanWeave(actionID, 1)) ||
                                (IsEnabled(CustomComboPreset.DRG_AoE_Mirage) && LevelChecked(MirageDive) && HasEffect(Buffs.DiveReady)))
                                return OriginalHook(Jump);

                            //Life Surge AoE Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_LifeSurge) &&
                                !HasEffect(Buffs.LifeSurge) && GetRemainingCharges(LifeSurge) > 0 && (HasEffect(Buffs.LanceCharge) || HasEffect(Buffs.RightEye)) &&
                                ((lastComboMove is CoerthanTorment && LevelChecked(CoerthanTorment)) ||
                                (lastComboMove is SonicThrust && LevelChecked(SonicThrust) && !LevelChecked(CoerthanTorment)) ||
                                (lastComboMove is DoomSpike && !LevelChecked(SonicThrust))))
                                return LifeSurge;

                            //Wyrmwind Thrust AoE Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_WyrmwindFeature) && gauge.FirstmindsFocusCount is 2)
                                return WyrmwindThrust;

                            //Dives AoE Feature
                            if (IsEnabled(CustomComboPreset.DRG_AoE_Dives))
                            {
                                if (DiveOptions is 0 or 1 || //Dives on cooldown
                                   (DiveOptions is 2 && gauge.IsLOTDActive && HasEffect(Buffs.BattleLitany)) || //Dives under Litany and Life of the Dragon
                                   (DiveOptions is 3 && HasEffect(Buffs.BattleLitany)) || //Dives under Litany
                                   (DiveOptions is 4 && HasEffect(Buffs.LanceCharge))) //Dives under Lance Charge Feature
                                {
                                    if (LevelChecked(DragonfireDive) && IsOffCooldown(DragonfireDive))
                                        return DragonfireDive;
                                    if (LevelChecked(SpineshatterDive) && GetRemainingCharges(SpineshatterDive) > 0)
                                        return SpineshatterDive;
                                }

                                if (DiveOptions is 0 or 1 or 2 or 3 or 4 && gauge.IsLOTDActive && LevelChecked(Stardiver) && IsOffCooldown(Stardiver) && CanWeave(actionID, 1.3))
                                    return Stardiver;
                            }
                        }
                    }

                    if (comboTime > 0)
                    {
                        if (lastComboMove is DoomSpike or DraconianFury && LevelChecked(SonicThrust))
                            return SonicThrust;
                        if (lastComboMove is SonicThrust && LevelChecked(CoerthanTorment))
                            return CoerthanTorment;
                    }

                    return OriginalHook(DoomSpike);
                }

                return actionID;
            }
        }
    }
}
