using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using System.IO;

namespace Spooky.Core
{
    public class Flags : ModSystem
    {
        public static Vector2 DaffodilPosition;
        public static Vector2 PandoraPosition;
        public static Vector2 FlowerPotPosition;
        public static Vector2 SpiderWebPosition;
        public static Vector2 EggPosition;

        public static bool downedRotGourd = false;
        public static bool downedSpookySpirit = false;
        public static bool downedMoco = false;
        public static bool downedDaffodil = false;
        public static bool downedPandoraBox = false;
        public static bool downedEggEvent = false;
        public static bool downedOrroboro = false;
        public static bool downedBigBone = false;

        public static bool SpookyBackgroundAlt = false;
        public static bool CatacombKey1 = false; 
        public static bool CatacombKey2 = false;
        public static bool CatacombKey3 = false;
        public static bool RaveyardHappening = false;

        public static bool OldHunterAssembled = false;
        public static bool OldHunterHat = false;
        public static bool OldHunterSkull = false;
        public static bool OldHunterTorso = false;
        public static bool OldHunterLegs = false;

        public static bool LittleEyeBounty1 = false; 
        public static bool LittleEyeBounty2 = false;
        public static bool LittleEyeBounty3 = false;
        public static bool LittleEyeBounty4 = false;
        public static bool BountyInProgress = false;
        public static bool DailyQuest = false;

        public static bool encounteredMan = false;
        public static bool encounteredBaby = false;
        public static bool encounteredHorse = false;
        public static bool encounteredFlesh = false;

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

            SpookyBackgroundAlt = false;
            CatacombKey1 = false; 
            CatacombKey2 = false;
            CatacombKey3 = false;
            RaveyardHappening = false;

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
            DailyQuest = false;

            encounteredMan = false;
            encounteredBaby = false;
            encounteredHorse = false;
            encounteredFlesh = false;
		}

        public override void SaveWorldData(TagCompound tag)
        {
            tag["DaffodilPosition"] = DaffodilPosition;
            tag["PandoraPosition"] = PandoraPosition;
            tag["FlowerPotPosition"] = FlowerPotPosition;
            tag["SpiderWebPosition"] = SpiderWebPosition;
            tag["EggPosition"] = EggPosition;

            if (downedRotGourd) tag["downedRotGourd"] = true;
            if (downedSpookySpirit) tag["downedSpookySpirit"] = true;
            if (downedMoco) tag["downedMoco"] = true;
            if (downedDaffodil) tag["downedDaffodil"] = true;
            if (downedPandoraBox) tag["downedPandoraBox"] = true;
            if (downedEggEvent) tag["downedEggEvent"] = true;
            if (downedOrroboro) tag["downedOrroboro"] = true;
            if (downedBigBone) tag["downedBigBone"] = true;

            if (SpookyBackgroundAlt) tag["SpookyBackgroundAlt"] = true;
            if (CatacombKey1) tag["CatacombKey1"] = true;
            if (CatacombKey2) tag["CatacombKey2"] = true;
            if (CatacombKey3) tag["CatacombKey3"] = true;
            if (RaveyardHappening) tag["RaveyardHappening"] = true;

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
            if (DailyQuest) tag["DailyQuest"] = true;

            if (encounteredMan) tag["encounteredMan"] = true;
            if (encounteredBaby) tag["encounteredBaby"] = true;
            if (encounteredHorse) tag["encounteredHorse"] = true;
            if (encounteredFlesh) tag["encounteredFlesh"] = true;
        }

        public override void LoadWorldData(TagCompound tag) 
        {
            DaffodilPosition = tag.Get<Vector2>("DaffodilPosition");
            PandoraPosition = tag.Get<Vector2>("PandoraPosition");
            FlowerPotPosition = tag.Get<Vector2>("FlowerPotPosition");
            SpiderWebPosition = tag.Get<Vector2>("SpiderWebPosition");
            EggPosition = tag.Get<Vector2>("EggPosition");

            downedRotGourd = tag.ContainsKey("downedRotGourd");
            downedSpookySpirit = tag.ContainsKey("downedSpookySpirit");
            downedMoco = tag.ContainsKey("downedMoco");
            downedDaffodil = tag.ContainsKey("downedDaffodil");
            downedPandoraBox = tag.ContainsKey("downedPandoraBox");
            downedEggEvent = tag.ContainsKey("downedEggEvent");
            downedOrroboro = tag.ContainsKey("downedOrroboro");
            downedBigBone = tag.ContainsKey("downedBigBone");

            SpookyBackgroundAlt = tag.ContainsKey("SpookyBackgroundAlt");
            CatacombKey1 = tag.ContainsKey("CatacombKey1");
            CatacombKey2 = tag.ContainsKey("CatacombKey2");
            CatacombKey3 = tag.ContainsKey("CatacombKey3");
            RaveyardHappening = tag.ContainsKey("RaveyardHappening");
            
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
            DailyQuest = tag.ContainsKey("DailyQuest");

            encounteredMan = tag.ContainsKey("encounteredMan");
            encounteredBaby = tag.ContainsKey("encounteredBaby");
            encounteredHorse = tag.ContainsKey("encounteredHorse");
            encounteredFlesh = tag.ContainsKey("encounteredFlesh");
		}

        public override void NetSend(BinaryWriter writer)
        {
            writer.WriteVector2(DaffodilPosition);
            writer.WriteVector2(PandoraPosition);
            writer.WriteVector2(FlowerPotPosition);
            writer.WriteVector2(SpiderWebPosition);
            writer.WriteVector2(EggPosition);

            var downedFlags = new BitsByte();
            downedFlags[0] = downedRotGourd;
            downedFlags[1] = downedSpookySpirit;
            downedFlags[2] = downedMoco;
            downedFlags[3] = downedDaffodil;
            downedFlags[4] = downedPandoraBox;
            downedFlags[5] = downedEggEvent;
            downedFlags[6] = downedOrroboro;
            downedFlags[7] = downedBigBone;
            writer.Write(downedFlags);

            var miscFlags = new BitsByte();
            miscFlags[0] = SpookyBackgroundAlt;
            miscFlags[1] = CatacombKey1;
            miscFlags[2] = CatacombKey2;
            miscFlags[3] = CatacombKey3;
            miscFlags[4] = RaveyardHappening;
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
            questFlags[5] = DailyQuest;
            writer.Write(questFlags);

            var encounterFlags = new BitsByte();
            encounterFlags[0] = encounteredMan;
            encounterFlags[1] = encounteredBaby;
            encounterFlags[2] = encounteredHorse;
            encounterFlags[3] = encounteredFlesh;
            writer.Write(encounterFlags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            DaffodilPosition = reader.ReadVector2();
            PandoraPosition = reader.ReadVector2();
            FlowerPotPosition = reader.ReadVector2();
            SpiderWebPosition = reader.ReadVector2();
            EggPosition = reader.ReadVector2();

            BitsByte downedFlags = reader.ReadByte();
            downedRotGourd = downedFlags[0];
            downedSpookySpirit = downedFlags[1];
            downedMoco = downedFlags[2];
            downedDaffodil = downedFlags[3];
            downedPandoraBox = downedFlags[4];
            downedEggEvent = downedFlags[5];
            downedOrroboro = downedFlags[6];
            downedBigBone = downedFlags[7];

            BitsByte miscFlags = reader.ReadByte();
            SpookyBackgroundAlt = miscFlags[0];
            CatacombKey1 = miscFlags[1];
            CatacombKey2 = miscFlags[2];
            CatacombKey3 = miscFlags[3];
            RaveyardHappening = miscFlags[4];

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
            DailyQuest = questFlags[5];

            BitsByte encounterFlags = reader.ReadByte();
            encounteredMan = encounterFlags[0];
            encounteredBaby = encounterFlags[1];
            encounteredHorse = encounterFlags[2];
            encounteredFlesh = encounterFlags[3];
        }
    }
}
