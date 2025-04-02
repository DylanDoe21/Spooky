using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Content.Items.BossSummon;
using Spooky.Content.Items.Catacomb;
using Spooky.Content.Items.Costume;
using Spooky.Content.Items.Pets;
using Spooky.Content.Items.SpookyBiome.Misc;
using Spooky.Content.NPCs.Boss.BigBone;
using Spooky.Content.NPCs.Boss.Daffodil;
using Spooky.Content.NPCs.Boss.Moco;
using Spooky.Content.NPCs.Boss.Orroboro;
using Spooky.Content.NPCs.Boss.RotGourd;
using Spooky.Content.NPCs.Boss.SpookFishron;
using Spooky.Content.NPCs.Boss.SpookySpirit;
using Spooky.Content.NPCs.EggEvent;
using Spooky.Content.NPCs.PandoraBox;
using Spooky.Content.Tiles.MusicBox;
using Spooky.Content.Tiles.Relic;
using Spooky.Content.Tiles.Trophy;

namespace Spooky.Core
{
	public class BossChecklistCompatibility : ModSystem
	{
        public override void PostSetupContent() 
        {
			BossChecklistSetup();
        }

        private void BossChecklistSetup() 
        {
			//do not load any of this if boss checklist is not enabled
			if (!ModLoader.TryGetMod("BossChecklist", out Mod bossChecklistMod)) 
            {
				return;
			}

            if (bossChecklistMod.Version < new Version(1, 3, 1)) 
            {
				return;
			}

			/*
			Boss Checklist weights for all vanilla bosses, mini-bosses, and events (copied directly from their source code)

			KingSlime = 1f;
			EyeOfCthulhu = 2f;
			EvilBosses = 3f;
			QueenBee = 4f;
			Skeletron = 5f;
			DeerClops = 6f;
			WallOfFlesh = 7f;
			QueenSlime = 8f;
			TheTwins = 9f;
			TheDestroyer = 10f;
			SkeletronPrime = 11f;
			Plantera = 12f;
			Golem = 13f;
			Betsy = 14f;
			EmpressOfLight = 15f;
			DukeFishron = 16f;
			LunaticCultist = 17f;
			Moonlord = 18f;

			TorchGod = 1.5f;
			BloodMoon = 2.5f;
			GoblinArmy = 3.33f;
			OldOnesArmy = 3.66f;
			DarkMage = OldOnesArmy + 0.01f;
			Ogre = SkeletronPrime + 0.01f;
			FrostLegion = 7.33f;
			PirateInvasion = 7.66f;
			PirateShip = PirateInvasion + 0.01f;
			SolarEclipse = 11.5f;
			PumpkinMoon = 12.33f;
			MourningWood = PumpkinMoon + 0.01f;
			Pumpking = PumpkinMoon + 0.02f;
			FrostMoon = 12.66f;
			Everscream = FrostMoon + 0.01f;
			SantaNK1 = FrostMoon + 0.02f;
			IceQueen = FrostMoon + 0.03f;
			MartianMadness = 13.5f;
			MartianSaucer = MartianMadness + 0.01f;
			LunarEvent = LunaticCultist + 0.01f;
			*/

			//Rot Gourd
			var RotGourdPortrait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => 
            {
				Texture2D texture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/NPCDisplayTextures/RotGourdBestiary").Value;
				Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
				spriteBatch.Draw(texture, centered, color);
			};

			bossChecklistMod.Call(
				"LogBoss", 
				Mod,
				nameof(RotGourd),
				2.01f,
				() => Flags.downedRotGourd,
				ModContent.NPCType<RotGourd>(),
				new Dictionary<string, object>() 
				{
					["spawnItems"] = ModContent.ItemType<RottenSeed>(),
					["collectibles"] = new List<int>() { ModContent.ItemType<RotGourdTrophyItem>(), ModContent.ItemType<RotGourdMask>(), 
					ModContent.ItemType<RotGourdRelicItem>(), ModContent.ItemType<RottenGourd>(), ModContent.ItemType<RotGourdBox>() },
					["spawnInfo"] = Language.GetOrRegister("Mods.Spooky.NPCs.RotGourd.BossChecklistIntegration.SpawnInfo").Value,
					["despawnMessage"] = Language.GetOrRegister("Mods.Spooky.NPCs.RotGourd.BossChecklistIntegration.DespawnMessage").Value,
					["customPortrait"] = RotGourdPortrait,
				}
			);


			//Spooky Spirit
			var SpiritPortrait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => 
            {
				Texture2D texture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/NPCDisplayTextures/SpookySpiritBestiary").Value;
				Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
				spriteBatch.Draw(texture, centered, color);
			};

			bossChecklistMod.Call(
				"LogBoss", 
				Mod,
				nameof(SpookySpirit),
				3.01f,
				() => Flags.downedSpookySpirit,
				ModContent.NPCType<SpookySpirit>(),
				new Dictionary<string, object>() 
				{
					["spawnItems"] = ModContent.ItemType<EMFReader>(),
					["collectibles"] = new List<int>() { ModContent.ItemType<SpookySpiritTrophyItem>(), ModContent.ItemType<SpookySpiritMask>(),
					ModContent.ItemType<SpookySpiritRelicItem>(), ModContent.ItemType<SpiritLamp>(), ModContent.ItemType<SpookySpiritBox>() },
					["spawnInfo"] = Language.GetOrRegister("Mods.Spooky.NPCs.SpookySpirit.BossChecklistIntegration.SpawnInfo").Value,
					["despawnMessage"] = Language.GetOrRegister("Mods.Spooky.NPCs.SpookySpirit.BossChecklistIntegration.DespawnMessage").Value,
					["customPortrait"] = SpiritPortrait,
				}
			);


			//Moco
			var MocoPortrait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => 
            {
				Texture2D texture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/NPCDisplayTextures/MocoBossChecklist").Value;
				Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
				spriteBatch.Draw(texture, centered, color);
			};

			bossChecklistMod.Call(
				"LogBoss", 
				Mod,
				nameof(Moco),
				5.01f,
				() => Flags.downedMoco,
				ModContent.NPCType<Moco>(),
				new Dictionary<string, object>() 
				{
					["spawnItems"] = ModContent.ItemType<CottonSwab>(),
					["collectibles"] = new List<int>() { ModContent.ItemType<MocoTrophyItem>(), ModContent.ItemType<MocoMask>(), 
					ModContent.ItemType<MocoRelicItem>(), ModContent.ItemType<MocoTissue>(), ModContent.ItemType<MocoBox>() },
					["spawnInfo"] = Language.GetOrRegister("Mods.Spooky.NPCs.Moco.BossChecklistIntegration.SpawnInfo").Value,
					["despawnMessage"] = Language.GetOrRegister("Mods.Spooky.NPCs.Moco.BossChecklistIntegration.DespawnMessage").Value,
					["customPortrait"] = MocoPortrait,
				}
			);


			//Daffodil
			var DaffodilPortrait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => 
            {
				Texture2D texture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/NPCDisplayTextures/DaffodilBestiary").Value;
				Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
				spriteBatch.Draw(texture, centered, color);
			};

			bossChecklistMod.Call(
				"LogBoss", 
				Mod,
				nameof(DaffodilEye),
				8.01f,
				() => Flags.downedDaffodil,
				ModContent.NPCType<DaffodilEye>(),
				new Dictionary<string, object>() 
				{
					["spawnItems"] = ModContent.ItemType<Brick>(),
					["collectibles"] = new List<int>() { ModContent.ItemType<DaffodilTrophyItem>(), ModContent.ItemType<DaffodilMask>(), 
					ModContent.ItemType<DaffodilRelicItem>(), ModContent.ItemType<SmallDaffodil>(), ModContent.ItemType<DaffodilBox>() },
					["spawnInfo"] = Language.GetOrRegister("Mods.Spooky.NPCs.DaffodilEye.BossChecklistIntegration.SpawnInfo").Value,
					["despawnMessage"] = Language.GetOrRegister("Mods.Spooky.NPCs.DaffodilEye.BossChecklistIntegration.DespawnMessage").Value,
					["customPortrait"] = DaffodilPortrait,
				}
			);

			
			//Pandora's Box
			List<int> PandoraBoxEnemies = new List<int>()
			{
				ModContent.NPCType<Bobbert>(), 
				ModContent.NPCType<Stitch>(), 
				ModContent.NPCType<Sheldon>(),
				ModContent.NPCType<Chester>(),
			};

			var PandoraBoxPortrait = (SpriteBatch spriteBatch, Rectangle rect, Color color) =>
			{
				Texture2D texture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/NPCDisplayTextures/PandoraBoxBossChecklist").Value;
				Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
				spriteBatch.Draw(texture, centered, color);
			};

			List<string> PandoraBoxIcon = new List<string>()
			{
				"Spooky/Content/NPCs/NPCDisplayTextures/PandoraBoxIcon"
			};

			bossChecklistMod.Call(
				"LogEvent", 
				Mod,
				"PandoraBox",
				8.02f,
				() => Flags.downedPandoraBox,
				PandoraBoxEnemies,
				new Dictionary<string, object>()
				{
                    ["collectibles"] = new List<int>() { ModContent.ItemType<PandoraChalice>(), ModContent.ItemType<PandoraCross>(),
                    ModContent.ItemType<PandoraCuffs>(), ModContent.ItemType<PandoraRosary>(), ModContent.ItemType<PandoraBean>() },
                    ["displayName"] = Language.GetOrRegister("Mods.Spooky.BossChecklistIntegration.PandoraBox.EntryName").Value,
					["spawnInfo"] = Language.GetOrRegister("Mods.Spooky.BossChecklistIntegration.PandoraBox.SpawnInfo").Value,
					["overrideHeadTextures"] = PandoraBoxIcon,
					["customPortrait"] = PandoraBoxPortrait,
				}
			);


			//Egg Incursion
			List<int> EggEventEnemies = new List<int>()
			{
				ModContent.NPCType<Biojetter>(), 
				ModContent.NPCType<CoughLungs>(), 
				ModContent.NPCType<CruxBat>(), 
				ModContent.NPCType<EarWorm>(),
				ModContent.NPCType<ExplodingAppendix>(), 
				ModContent.NPCType<GooSlug>(), 
				ModContent.NPCType<HoppingHeart>(), 
				ModContent.NPCType<HoverBrain>(), 
				ModContent.NPCType<TongueBiter>()
			};

			var EggEventPortrait = (SpriteBatch spriteBatch, Rectangle rect, Color color) =>
			{
				Texture2D texture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/NPCDisplayTextures/EggEventBossChecklist").Value;
				Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
				spriteBatch.Draw(texture, centered, color);
			};

			List<string> EggEventIcon = new List<string>()
			{
				"Spooky/Content/Items/BossSummon/StrangeCyst"
			};

			List<int> EggEventItems = new List<int>()
			{
				ModContent.ItemType<Concoction>(), ModContent.ItemType<StrangeCyst>()
			};

			bossChecklistMod.Call(
				"LogEvent", 
				Mod,
				"EggEvent",
				11.01f,
				() => Flags.downedEggEvent,
				EggEventEnemies,
				new Dictionary<string, object>()
				{
					["spawnItems"] = EggEventItems,
					["collectibles"] = ModContent.ItemType<EggEventBox>(),
					["displayName"] = Language.GetOrRegister("Mods.Spooky.BossChecklistIntegration.EggEvent.EntryName").Value,
					["spawnInfo"] = Language.GetOrRegister("Mods.Spooky.BossChecklistIntegration.EggEvent.SpawnInfo").Value,
					["overrideHeadTextures"] = EggEventIcon,
					["customPortrait"] = EggEventPortrait,
				}
			);
			

			//Orro & Boro
			List<int> Orroboro = new List<int>() 
			{ 
				ModContent.NPCType<OrroHead>(), ModContent.NPCType<BoroHead>() 
			};

			var OrroboroPortrait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => 
            {
				Texture2D texture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/NPCDisplayTextures/OrroboroBossChecklist").Value;
				Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
				spriteBatch.Draw(texture, centered, color);
			};

			bossChecklistMod.Call(
				"LogBoss", 
				Mod,
				nameof(OrroHead),
				11.02f,
				() => Flags.downedOrroboro,
				Orroboro,
				new Dictionary<string, object>() 
				{
					["spawnItems"] = ModContent.ItemType<Concoction>(),
					["collectibles"] = new List<int>() { ModContent.ItemType<OrroTrophyItem>(), ModContent.ItemType<BoroTrophyItem>(), 
					ModContent.ItemType<OrroMask>(), ModContent.ItemType<BoroMask>(), ModContent.ItemType<OrroboroRelicItem>(),
					ModContent.ItemType<OrroboroRing>(), ModContent.ItemType<OrroboroBox>() },
					["spawnInfo"] = Language.GetOrRegister("Mods.Spooky.NPCs.OrroHead.BossChecklistIntegration.SpawnInfo").Value,
					["despawnMessage"] = Language.GetOrRegister("Mods.Spooky.NPCs.OrroHead.BossChecklistIntegration.DespawnMessage").Value,
					["customPortrait"] = OrroboroPortrait,
				}
			);


			//Spook Fishron
			var SpookFishronPortrait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => 
            {
				Texture2D texture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/NPCDisplayTextures/SpookFishronBossChecklist").Value;
				Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
				spriteBatch.Draw(texture, centered, color);
			};

			bossChecklistMod.Call(
				"LogBoss", 
				Mod,
				nameof(SpookFishron),
				14.01f,
				() => Flags.downedSpookFishron,
				ModContent.NPCType<SpookFishron>(),
				new Dictionary<string, object>() 
				{
					["availability"] = () => Flags.downedSpookFishron,
					["spawnItems"] = ModContent.ItemType<SinisterSnailItem>(),
					["collectibles"] = new List<int>() { ModContent.ItemType<SpookFishronTrophyItem>(), ModContent.ItemType<SpookFishronMask>(),
					ModContent.ItemType<SpookFishronRelicItem>(), ModContent.ItemType<SinisterShell>(), ModContent.ItemType<SpookFishronBox>() },
					["spawnInfo"] = Language.GetOrRegister("Mods.Spooky.NPCs.SpookFishron.BossChecklistIntegration.SpawnInfo"),
					["despawnMessage"] = Language.GetOrRegister("Mods.Spooky.NPCs.SpookFishron.BossChecklistIntegration.DespawnMessage"),
					["customPortrait"] = SpookFishronPortrait,
				}
			);


			//Big Bone
			var BigBonePortrait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => 
            {
				Texture2D texture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/NPCDisplayTextures/BigBoneBossChecklist").Value;
				Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
				spriteBatch.Draw(texture, centered, color);
			};

			bossChecklistMod.Call(
				"LogBoss", 
				Mod,
				nameof(BigBone),
				14.01f,
				() => Flags.downedBigBone,
				ModContent.NPCType<BigBone>(),
				new Dictionary<string, object>() 
				{
					["spawnItems"] = ModContent.ItemType<Fertilizer>(),
					["collectibles"] = new List<int>() { ModContent.ItemType<BigBoneTrophyItem>(), ModContent.ItemType<BigBoneMask>(), 
					ModContent.ItemType<BigBoneRelicItem>(), ModContent.ItemType<SkullSeed>(), ModContent.ItemType<BigBoneBox>() },
					["spawnInfo"] = Language.GetOrRegister("Mods.Spooky.NPCs.BigBone.BossChecklistIntegration.SpawnInfo").Value,
					["despawnMessage"] = Language.GetOrRegister("Mods.Spooky.NPCs.BigBone.BossChecklistIntegration.DespawnMessage").Value,
					["customPortrait"] = BigBonePortrait,
				}
			);
		}
    }       
}