using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using System.IO;
using System.Linq;
using System.Collections.Generic;

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

		//list of eel biome nodes
		public static List<Vector2> ZombieBiomePositions = new List<Vector2>();

		//bosses
		public static bool downedRotGourd = false;
        public static bool downedSpookySpirit = false;
        public static bool downedMoco = false;
        public static bool downedDaffodil = false;
        public static bool downedPandoraBox = false;
        public static bool downedEggEvent = false;
        public static bool downedOrroboro = false;
        public static bool downedBigBone = false;
        public static bool downedSpookFishron = false;

		//events
        public static bool downedMocoIdol1 = false;
        public static bool downedMocoIdol2 = false;
        public static bool downedMocoIdol3 = false;
        public static bool downedMocoIdol4 = false;
        public static bool downedMocoIdol5 = false;
        public static bool downedMocoIdol6 = false;
        public static bool MinibossBarrierOpen = false;
		public static bool RaveyardHappening = false;
		public static bool GuaranteedRaveyard = false;

		//misc stuff
        public static bool CatacombKey1 = false; 
        public static bool CatacombKey2 = false;
        public static bool CatacombKey3 = false;
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
		    //bosses
            downedRotGourd = false;
            downedSpookySpirit = false;
            downedMoco = false;
            downedDaffodil = false;
            downedOrroboro = false;
            downedBigBone = false;
            downedSpookFishron = false;

			//events
			downedPandoraBox = false;
			downedEggEvent = false;
			downedMocoIdol1 = false;
            downedMocoIdol2 = false;
            downedMocoIdol3 = false;
            downedMocoIdol4 = false;
            downedMocoIdol5 = false;
            downedMocoIdol6 = false;
            MinibossBarrierOpen = false;
			GuaranteedRaveyard = false;
			OldHunterAssembled = false;

			//misc stuff
            CatacombKey1 = false; 
            CatacombKey2 = false;
            CatacombKey3 = false;
            RaveyardHappening = false;
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
		    //important world positions
			tag[nameof(DaffodilPosition)] = DaffodilPosition;
			tag[nameof(PandoraPosition)] = PandoraPosition;
			tag[nameof(FlowerPotPosition)] = FlowerPotPosition;
			tag[nameof(SpiderWebPosition)] = SpiderWebPosition;
			tag[nameof(EggPosition)] = EggPosition;
			tag[nameof(MocoIdolPosition1)] = MocoIdolPosition1;
			tag[nameof(MocoIdolPosition2)] = MocoIdolPosition2;
			tag[nameof(MocoIdolPosition3)] = MocoIdolPosition3;
			tag[nameof(MocoIdolPosition4)] = MocoIdolPosition4;
			tag[nameof(MocoIdolPosition5)] = MocoIdolPosition5;
			tag[nameof(LeaderIdolPositon)] = LeaderIdolPositon;

			//biome positions for compasses
            tag[nameof(SpiderGrottoCenter)] = SpiderGrottoCenter;
            tag[nameof(EyeValleyCenter)] = EyeValleyCenter;

			//list of eel biome nodes
			tag["Spooky:ZombieBiomePositions"] = ZombieBiomePositions;

			//bosses
			tag[nameof(downedRotGourd)] = downedRotGourd;
			tag[nameof(downedSpookySpirit)] = downedSpookySpirit;
			tag[nameof(downedMoco)] = downedMoco;
			tag[nameof(downedDaffodil)] = downedDaffodil;
			tag[nameof(downedOrroboro)] = downedOrroboro;
			tag[nameof(downedBigBone)] = downedBigBone;
			tag[nameof(downedSpookFishron)] = downedSpookFishron;

			//events
			tag[nameof(downedPandoraBox)] = downedPandoraBox;
			tag[nameof(downedEggEvent)] = downedEggEvent;
			tag[nameof(downedMocoIdol1)] = downedMocoIdol1;
			tag[nameof(downedMocoIdol2)] = downedMocoIdol2;
			tag[nameof(downedMocoIdol3)] = downedMocoIdol3;
			tag[nameof(downedMocoIdol4)] = downedMocoIdol4;
			tag[nameof(downedMocoIdol5)] = downedMocoIdol5;
			tag[nameof(downedMocoIdol6)] = downedMocoIdol6;
			tag[nameof(MinibossBarrierOpen)] = MinibossBarrierOpen;
			tag[nameof(RaveyardHappening)] = RaveyardHappening;
			tag[nameof(GuaranteedRaveyard)] = GuaranteedRaveyard;

			//misc stuff
			tag[nameof(CatacombKey1)] = CatacombKey1;
			tag[nameof(CatacombKey2)] = CatacombKey2;
			tag[nameof(CatacombKey3)] = CatacombKey3;
			tag[nameof(OldHunterAssembled)] = OldHunterAssembled;
			tag[nameof(OldHunterHat)] = OldHunterHat;
			tag[nameof(OldHunterSkull)] = OldHunterSkull;
			tag[nameof(OldHunterTorso)] = OldHunterTorso;
			tag[nameof(OldHunterLegs)] = OldHunterLegs;
			tag[nameof(LittleEyeBounty1)] = LittleEyeBounty1;
			tag[nameof(LittleEyeBounty2)] = LittleEyeBounty2;
			tag[nameof(LittleEyeBounty3)] = LittleEyeBounty3;
			tag[nameof(LittleEyeBounty4)] = LittleEyeBounty4;
			tag[nameof(BountyInProgress)] = BountyInProgress;
			tag[nameof(encounteredMan)] = encounteredMan;
        }

        public override void LoadWorldData(TagCompound tag) 
        {
		    //important world positions
			DaffodilPosition = tag.Get<Vector2>(nameof(DaffodilPosition));
			PandoraPosition = tag.Get<Vector2>(nameof(PandoraPosition));
			FlowerPotPosition = tag.Get<Vector2>(nameof(FlowerPotPosition));
			SpiderWebPosition = tag.Get<Vector2>(nameof(SpiderWebPosition));
			EggPosition = tag.Get<Vector2>(nameof(EggPosition));
			MocoIdolPosition1 = tag.Get<Vector2>(nameof(MocoIdolPosition1));
			MocoIdolPosition2 = tag.Get<Vector2>(nameof(MocoIdolPosition2));
			MocoIdolPosition3 = tag.Get<Vector2>(nameof(MocoIdolPosition3));
			MocoIdolPosition4 = tag.Get<Vector2>(nameof(MocoIdolPosition4));
			MocoIdolPosition5 = tag.Get<Vector2>(nameof(MocoIdolPosition5));
			LeaderIdolPositon = tag.Get<Vector2>(nameof(LeaderIdolPositon));

			//world positions for compasses
			SpiderGrottoCenter = tag.Get<Vector2>(nameof(SpiderGrottoCenter));
			EyeValleyCenter = tag.Get<Vector2>(nameof(EyeValleyCenter));

			//eel biome positions
			if (tag.ContainsKey("Spooky:ZombieBiomePositions"))
			{
				ZombieBiomePositions = tag.Get<List<Vector2>>("Spooky:ZombieBiomePositions");
			}

			//bosses
			downedRotGourd = tag.GetBool(nameof(downedRotGourd));
			downedSpookySpirit = tag.GetBool(nameof(downedSpookySpirit));
			downedMoco = tag.GetBool(nameof(downedMoco));
			downedDaffodil = tag.GetBool(nameof(downedDaffodil));
			downedOrroboro = tag.GetBool(nameof(downedOrroboro));
			downedBigBone = tag.GetBool(nameof(downedBigBone));
			downedSpookFishron = tag.GetBool(nameof(downedSpookFishron));

			//events
			downedPandoraBox = tag.GetBool(nameof(downedPandoraBox));
			downedEggEvent = tag.GetBool(nameof(downedEggEvent));
			downedMocoIdol1 = tag.GetBool(nameof(downedMocoIdol1));
			downedMocoIdol2 = tag.GetBool(nameof(downedMocoIdol2));
			downedMocoIdol3 = tag.GetBool(nameof(downedMocoIdol3));
			downedMocoIdol4 = tag.GetBool(nameof(downedMocoIdol4));
			downedMocoIdol5 = tag.GetBool(nameof(downedMocoIdol5));
			downedMocoIdol6 = tag.GetBool(nameof(downedMocoIdol6));
			MinibossBarrierOpen = tag.GetBool(nameof(MinibossBarrierOpen));
			RaveyardHappening = tag.GetBool(nameof(RaveyardHappening));
			GuaranteedRaveyard = tag.GetBool(nameof(GuaranteedRaveyard));

			//misc stuff
			CatacombKey1 = tag.GetBool(nameof(CatacombKey1));
			CatacombKey2 = tag.GetBool(nameof(CatacombKey2));
			CatacombKey3 = tag.GetBool(nameof(CatacombKey3));
			OldHunterAssembled = tag.GetBool(nameof(OldHunterAssembled));
			OldHunterHat = tag.GetBool(nameof(OldHunterHat));
			OldHunterSkull = tag.GetBool(nameof(OldHunterSkull));
			OldHunterTorso = tag.GetBool(nameof(OldHunterTorso));
			OldHunterLegs = tag.GetBool(nameof(OldHunterLegs));
			LittleEyeBounty1 = tag.GetBool(nameof(LittleEyeBounty1));
			LittleEyeBounty2 = tag.GetBool(nameof(LittleEyeBounty2));
			LittleEyeBounty3 = tag.GetBool(nameof(LittleEyeBounty3));
			LittleEyeBounty4 = tag.GetBool(nameof(LittleEyeBounty4));
			BountyInProgress = tag.GetBool(nameof(BountyInProgress));
			encounteredMan = tag.GetBool(nameof(encounteredMan));
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

			//downed bosses
			writer.WriteFlags(downedRotGourd, downedSpookySpirit, downedMoco, downedDaffodil, downedOrroboro, downedBigBone, downedSpookFishron);

			//downed events
			writer.WriteFlags(downedPandoraBox, downedEggEvent);

			//downed moco temple stuff
			writer.WriteFlags(downedMocoIdol1, downedMocoIdol2, downedMocoIdol3, downedMocoIdol4, downedMocoIdol5, downedMocoIdol6, MinibossBarrierOpen);

			//misc stuff
			writer.WriteFlags(CatacombKey1, CatacombKey2, CatacombKey3, RaveyardHappening, GuaranteedRaveyard);

			//old hunter stuff
			writer.WriteFlags(OldHunterAssembled, OldHunterHat, OldHunterSkull, OldHunterTorso, OldHunterLegs);

			//little eye quest stuff
			writer.WriteFlags(LittleEyeBounty1, LittleEyeBounty2, LittleEyeBounty3, LittleEyeBounty4, BountyInProgress);

			//entity (more will probably be added here in the future)
			writer.WriteFlags(encounteredMan);
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

			//downed bosses
			reader.ReadFlags(out downedRotGourd, out downedSpookySpirit, out downedMoco, out downedDaffodil, out downedOrroboro, out downedBigBone, out downedSpookFishron);

			//downed events
			reader.ReadFlags(out downedPandoraBox, out downedEggEvent);

			//downed moco temple stuff
			reader.ReadFlags(out downedMocoIdol1, out downedMocoIdol2, out downedMocoIdol3, out downedMocoIdol4, out downedMocoIdol5, out downedMocoIdol6, out MinibossBarrierOpen);

			//misc stuff
			reader.ReadFlags(out CatacombKey1, out CatacombKey2, out CatacombKey3, out RaveyardHappening, out GuaranteedRaveyard);

			//old hunter stuff
			reader.ReadFlags(out OldHunterAssembled, out OldHunterHat, out OldHunterSkull, out OldHunterTorso, out OldHunterLegs);

			//little eye quest stuff
			reader.ReadFlags(out LittleEyeBounty1, out LittleEyeBounty2, out LittleEyeBounty3, out LittleEyeBounty4, out BountyInProgress);

			//entity (more will probably be added here in the future)
			reader.ReadFlags(out encounteredMan);
		}
    }
}
