using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using System.IO;

namespace Spooky.Core
{
    public class Flags : ModSystem
    {
        //positions for npc spawning
        public static Vector2 DaffodilPosition = Vector2.Zero;
        public static Vector2 PandoraPosition = Vector2.Zero;
        public static Vector2 FlowerPotPosition = Vector2.Zero;
        public static Vector2 SpiderWebPosition = Vector2.Zero;
        public static Vector2 EggPosition = Vector2.Zero;
        public static Vector2 MocoIdolPosition1 = Vector2.Zero;
        public static Vector2 MocoIdolPosition2 = Vector2.Zero;
        public static Vector2 MocoIdolPosition3 = Vector2.Zero;
        public static Vector2 MocoIdolPosition4 = Vector2.Zero;
        public static Vector2 MocoIdolPosition5 = Vector2.Zero;
        public static Vector2 LeaderIdolPositon = Vector2.Zero;

        //compass positions
        public static Vector2 SpiderGrottoCenter = Vector2.Zero;
        public static Vector2 EyeValleyCenter = Vector2.Zero;

        public static bool downedRotGourd = false;
        public static bool downedSpookySpirit = false;
        public static bool downedMoco = false;
        public static bool downedDaffodil = false;
        public static bool downedPandoraBox = false;
        public static bool downedEggEvent = false;
        public static bool downedOrroboro = false;
        public static bool downedBigBone = false;
        public static bool downedSpookFishron = false;

        public static bool downedMocoIdol1 = false;
        public static bool downedMocoIdol2 = false;
        public static bool downedMocoIdol3 = false;
        public static bool downedMocoIdol4 = false;
        public static bool downedMocoIdol5 = false;
        public static bool downedMocoIdol6 = false;
        public static bool MinibossBarrierOpen = false;

        public static bool SpookyBackgroundAlt = false;
        public static bool CatacombKey1 = false; 
        public static bool CatacombKey2 = false;
        public static bool CatacombKey3 = false;
        public static bool RaveyardHappening = false;
        public static bool GuaranteedRaveyard = false;

        public static bool OldHunterAssembled = false;
        public static bool OldHunterHat = false;
        public static bool OldHunterSkull = false;
        public static bool OldHunterTorso = false;
        public static bool OldHunterLegs = false;
        public static bool KillWeb = false;

        public static bool LittleEyeBounty1 = false; 
        public static bool LittleEyeBounty2 = false;
        public static bool LittleEyeBounty3 = false;
        public static bool LittleEyeBounty4 = false;
        public static bool BountyInProgress = false;

        public static bool encounteredMan = false;

        public override void ClearWorld()
        {
            downedRotGourd = false;
            downedSpookySpirit = false;
            downedMoco = false;
            downedDaffodil = false;
            downedPandoraBox = false;
            downedEggEvent = false;
            downedOrroboro = false;
            downedBigBone = false;
            downedSpookFishron = false;

            downedMocoIdol1 = false;
            downedMocoIdol2 = false;
            downedMocoIdol3 = false;
            downedMocoIdol4 = false;
            downedMocoIdol5 = false;
            downedMocoIdol6 = false;
            MinibossBarrierOpen = false;

            SpookyBackgroundAlt = false;
            CatacombKey1 = false; 
            CatacombKey2 = false;
            CatacombKey3 = false;
            RaveyardHappening = false;
            GuaranteedRaveyard = false;

            OldHunterAssembled = false;
            OldHunterHat = false;
            OldHunterSkull = false;
            OldHunterTorso = false;
            OldHunterLegs = false;

            LittleEyeBounty1 = false; 
            LittleEyeBounty2 = false;
            LittleEyeBounty3 = false;
            LittleEyeBounty4 = false;
            BountyInProgress = false;

            encounteredMan = false;
		}

        public override void SaveWorldData(TagCompound tag)
        {
            tag["DaffodilPosition"] = DaffodilPosition;
            tag["PandoraPosition"] = PandoraPosition;
            tag["FlowerPotPosition"] = FlowerPotPosition;
            tag["SpiderWebPosition"] = SpiderWebPosition;
            tag["EggPosition"] = EggPosition;
            tag["MocoIdolPosition1"] = MocoIdolPosition1;
            tag["MocoIdolPosition2"] = MocoIdolPosition2;
            tag["MocoIdolPosition3"] = MocoIdolPosition3;
            tag["MocoIdolPosition4"] = MocoIdolPosition4;
            tag["MocoIdolPosition5"] = MocoIdolPosition5;
            tag["LeaderIdolPositon"] = LeaderIdolPositon;

            tag["SpiderGrottoCenter"] = SpiderGrottoCenter;
            tag["EyeValleyCenter"] = EyeValleyCenter;

            if (downedRotGourd) tag["downedRotGourd"] = true;
            if (downedSpookySpirit) tag["downedSpookySpirit"] = true;
            if (downedMoco) tag["downedMoco"] = true;
            if (downedDaffodil) tag["downedDaffodil"] = true;
            if (downedPandoraBox) tag["downedPandoraBox"] = true;
            if (downedEggEvent) tag["downedEggEvent"] = true;
            if (downedOrroboro) tag["downedOrroboro"] = true;
            if (downedBigBone) tag["downedBigBone"] = true;
            if (downedSpookFishron) tag["downedSpookFishron"] = true;

            if (downedMocoIdol1) tag["downedMocoIdol1"] = true;
            if (downedMocoIdol2) tag["downedMocoIdol2"] = true;
            if (downedMocoIdol3) tag["downedMocoIdol3"] = true;
            if (downedMocoIdol4) tag["downedMocoIdol4"] = true;
            if (downedMocoIdol5) tag["downedMocoIdol5"] = true;
            if (downedMocoIdol6) tag["downedMocoIdol6"] = true;
            if (MinibossBarrierOpen) tag["MinibossBarrierOpen"] = true;

            if (SpookyBackgroundAlt) tag["SpookyBackgroundAlt"] = true;
            if (CatacombKey1) tag["CatacombKey1"] = true;
            if (CatacombKey2) tag["CatacombKey2"] = true;
            if (CatacombKey3) tag["CatacombKey3"] = true;
            if (RaveyardHappening) tag["RaveyardHappening"] = true;
            if (GuaranteedRaveyard) tag["GuaranteedRaveyard"] = true;

            if (OldHunterAssembled) tag["OldHunterAssembled"] = true;
            if (OldHunterHat) tag["OldHunterHat"] = true;
            if (OldHunterSkull) tag["OldHunterSkull"] = true;
            if (OldHunterTorso) tag["OldHunterTorso"] = true;
            if (OldHunterLegs) tag["OldHunterLegs"] = true;

            if (LittleEyeBounty1) tag["LittleEyeBounty1"] = true;
            if (LittleEyeBounty2) tag["LittleEyeBounty2"] = true;
            if (LittleEyeBounty3) tag["LittleEyeBounty3"] = true;
            if (LittleEyeBounty4) tag["LittleEyeBounty4"] = true;
            if (BountyInProgress) tag["BountyInProgress"] = true;

            if (encounteredMan) tag["encounteredMan"] = true;
        }

        public override void LoadWorldData(TagCompound tag) 
        {
            DaffodilPosition = tag.Get<Vector2>("DaffodilPosition");
            PandoraPosition = tag.Get<Vector2>("PandoraPosition");
            FlowerPotPosition = tag.Get<Vector2>("FlowerPotPosition");
            SpiderWebPosition = tag.Get<Vector2>("SpiderWebPosition");
            EggPosition = tag.Get<Vector2>("EggPosition");
            MocoIdolPosition1 = tag.Get<Vector2>("MocoIdolPosition1");
            MocoIdolPosition2 = tag.Get<Vector2>("MocoIdolPosition2");
            MocoIdolPosition3 = tag.Get<Vector2>("MocoIdolPosition3");
            MocoIdolPosition4 = tag.Get<Vector2>("MocoIdolPosition4");
            MocoIdolPosition5 = tag.Get<Vector2>("MocoIdolPosition5");
            LeaderIdolPositon = tag.Get<Vector2>("LeaderIdolPositon");

            SpiderGrottoCenter = tag.Get<Vector2>("SpiderGrottoCenter");
            EyeValleyCenter =  tag.Get<Vector2>("EyeValleyCenter");

            downedRotGourd = tag.ContainsKey("downedRotGourd");
            downedSpookySpirit = tag.ContainsKey("downedSpookySpirit");
            downedMoco = tag.ContainsKey("downedMoco");
            downedDaffodil = tag.ContainsKey("downedDaffodil");
            downedPandoraBox = tag.ContainsKey("downedPandoraBox");
            downedEggEvent = tag.ContainsKey("downedEggEvent");
            downedOrroboro = tag.ContainsKey("downedOrroboro");
            downedBigBone = tag.ContainsKey("downedBigBone");
            downedSpookFishron = tag.ContainsKey("downedSpookFishron");

            downedMocoIdol1 = tag.ContainsKey("downedMocoIdol1");
            downedMocoIdol2 = tag.ContainsKey("downedMocoIdol2");
            downedMocoIdol3 = tag.ContainsKey("downedMocoIdol3");
            downedMocoIdol4 = tag.ContainsKey("downedMocoIdol4");
            downedMocoIdol5 = tag.ContainsKey("downedMocoIdol5");
            downedMocoIdol6 = tag.ContainsKey("downedMocoIdol6");
            MinibossBarrierOpen = tag.ContainsKey("MinibossBarrierOpen");

            SpookyBackgroundAlt = tag.ContainsKey("SpookyBackgroundAlt");
            CatacombKey1 = tag.ContainsKey("CatacombKey1");
            CatacombKey2 = tag.ContainsKey("CatacombKey2");
            CatacombKey3 = tag.ContainsKey("CatacombKey3");
            RaveyardHappening = tag.ContainsKey("RaveyardHappening");
            GuaranteedRaveyard = tag.ContainsKey("GuaranteedRaveyard");
            
            OldHunterAssembled = tag.ContainsKey("OldHunterAssembled");
            OldHunterHat = tag.ContainsKey("OldHunterHat");
            OldHunterSkull = tag.ContainsKey("OldHunterSkull");
            OldHunterTorso = tag.ContainsKey("OldHunterTorso");
            OldHunterLegs = tag.ContainsKey("OldHunterLegs");

            LittleEyeBounty1 = tag.ContainsKey("LittleEyeBounty1");
            LittleEyeBounty2 = tag.ContainsKey("LittleEyeBounty2");
            LittleEyeBounty3 = tag.ContainsKey("LittleEyeBounty3");
            LittleEyeBounty4 = tag.ContainsKey("LittleEyeBounty4");
            BountyInProgress = tag.ContainsKey("BountyInProgress");

            encounteredMan = tag.ContainsKey("encounteredMan");
		}

        public override void NetSend(BinaryWriter writer)
        {
            writer.WriteVector2(DaffodilPosition);
            writer.WriteVector2(PandoraPosition);
            writer.WriteVector2(FlowerPotPosition);
            writer.WriteVector2(SpiderWebPosition);
            writer.WriteVector2(EggPosition);
            writer.WriteVector2(MocoIdolPosition1);
            writer.WriteVector2(MocoIdolPosition2);
            writer.WriteVector2(MocoIdolPosition3);
            writer.WriteVector2(MocoIdolPosition4);
            writer.WriteVector2(MocoIdolPosition5);
            writer.WriteVector2(LeaderIdolPositon);

            writer.WriteVector2(SpiderGrottoCenter);
            writer.WriteVector2(EyeValleyCenter);

            var downedFlags = new BitsByte();
            downedFlags[0] = downedRotGourd;
            downedFlags[1] = downedSpookySpirit;
            downedFlags[2] = downedMoco;
            downedFlags[3] = downedDaffodil;
            downedFlags[4] = downedOrroboro;
            downedFlags[5] = downedBigBone;
            downedFlags[6] = downedSpookFishron;
            writer.Write(downedFlags);

            var downedEventFlags = new BitsByte();
            downedEventFlags[0] = downedPandoraBox;
            downedEventFlags[1] = downedEggEvent;
            writer.Write(downedEventFlags);

            var noseDungeonFlags = new BitsByte();
            noseDungeonFlags[0] = downedMocoIdol1;
            noseDungeonFlags[1] = downedMocoIdol2;
            noseDungeonFlags[2] = downedMocoIdol3;
            noseDungeonFlags[3] = downedMocoIdol4;
            noseDungeonFlags[4] = downedMocoIdol5;
            noseDungeonFlags[5] = downedMocoIdol6;
            noseDungeonFlags[6] = MinibossBarrierOpen;
            writer.Write(noseDungeonFlags);

            var miscFlags = new BitsByte();
            miscFlags[0] = SpookyBackgroundAlt;
            miscFlags[1] = CatacombKey1;
            miscFlags[2] = CatacombKey2;
            miscFlags[3] = CatacombKey3;
            miscFlags[4] = RaveyardHappening;
            miscFlags[5] = GuaranteedRaveyard;
            writer.Write(miscFlags);

            var oldHunterFlags = new BitsByte();
            oldHunterFlags[0] = OldHunterAssembled;
            oldHunterFlags[1] = OldHunterHat;
            oldHunterFlags[2] = OldHunterSkull;
            oldHunterFlags[3] = OldHunterTorso;
            oldHunterFlags[4] = OldHunterLegs;
            writer.Write(oldHunterFlags);

            var questFlags = new BitsByte();
            questFlags[0] = LittleEyeBounty1;
            questFlags[1] = LittleEyeBounty2;
            questFlags[2] = LittleEyeBounty3;
            questFlags[3] = LittleEyeBounty4;
            questFlags[4] = BountyInProgress;
            writer.Write(questFlags);

            var encounterFlags = new BitsByte();
            encounterFlags[0] = encounteredMan;
            writer.Write(encounterFlags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            DaffodilPosition = reader.ReadVector2();
            PandoraPosition = reader.ReadVector2();
            FlowerPotPosition = reader.ReadVector2();
            SpiderWebPosition = reader.ReadVector2();
            EggPosition = reader.ReadVector2();
            MocoIdolPosition1 = reader.ReadVector2();
            MocoIdolPosition2 = reader.ReadVector2();
            MocoIdolPosition3 = reader.ReadVector2();
            MocoIdolPosition4 = reader.ReadVector2();
            MocoIdolPosition5 = reader.ReadVector2();
            LeaderIdolPositon = reader.ReadVector2();

            SpiderGrottoCenter = reader.ReadVector2();
            EyeValleyCenter = reader.ReadVector2();

            BitsByte downedFlags = reader.ReadByte();
            downedRotGourd = downedFlags[0];
            downedSpookySpirit = downedFlags[1];
            downedMoco = downedFlags[2];
            downedDaffodil = downedFlags[3];
            downedOrroboro = downedFlags[4];
            downedBigBone = downedFlags[5];
            downedSpookFishron = downedFlags[6];

            BitsByte downedEventFlags = reader.ReadByte();
            downedPandoraBox = downedEventFlags[0];
            downedEggEvent = downedEventFlags[1];

            BitsByte noseDungeonFlags = reader.ReadByte();
            downedMocoIdol1 = noseDungeonFlags[0];
            downedMocoIdol2 = noseDungeonFlags[1];
            downedMocoIdol3 = noseDungeonFlags[2];
            downedMocoIdol4 = noseDungeonFlags[3];
            downedMocoIdol5 = noseDungeonFlags[4];
            downedMocoIdol6 = noseDungeonFlags[5];
            MinibossBarrierOpen = noseDungeonFlags[6];

            BitsByte miscFlags = reader.ReadByte();
            SpookyBackgroundAlt = miscFlags[0];
            CatacombKey1 = miscFlags[1];
            CatacombKey2 = miscFlags[2];
            CatacombKey3 = miscFlags[3];
            RaveyardHappening = miscFlags[4];
            GuaranteedRaveyard = miscFlags[5];

            BitsByte oldHunterFlags = reader.ReadByte();
            OldHunterAssembled = oldHunterFlags[0];
            OldHunterHat = oldHunterFlags[1];
            OldHunterSkull = oldHunterFlags[2];
            OldHunterTorso = oldHunterFlags[3];
            OldHunterLegs = oldHunterFlags[4];

            BitsByte questFlags = reader.ReadByte();
            LittleEyeBounty1 = questFlags[0];
            LittleEyeBounty2 = questFlags[1];
            LittleEyeBounty3 = questFlags[2];
            LittleEyeBounty4 = questFlags[3];
            BountyInProgress = questFlags[4];

            BitsByte encounterFlags = reader.ReadByte();
            encounteredMan = encounterFlags[0];
        }
    }
}
