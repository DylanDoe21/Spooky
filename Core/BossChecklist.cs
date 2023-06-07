using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Content.Items.BossSummon;
using Spooky.Content.NPCs.Boss.BigBone;
using Spooky.Content.NPCs.Boss.Daffodil;
using Spooky.Content.NPCs.Boss.Moco;
using Spooky.Content.NPCs.Boss.Orroboro;
using Spooky.Content.NPCs.Boss.RotGourd;
using Spooky.Content.NPCs.Boss.SpookySpirit;
using Spooky.Content.NPCs.EggEvent;
using Spooky.Content.Tiles.MusicBox;

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
			//do not load any of this without boss checklist enabled
			if (!ModLoader.TryGetMod("BossChecklist", out Mod bossChecklistMod)) 
            {
				return;
			}

            if (bossChecklistMod.Version < new Version(1, 3, 1)) 
            {
				return;
			}

			/*
			Boss Checklist weights for all vanilla bosses, mini bosses, and events (copied directly from their source code)

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

			//Rot gourd
			var GourdPortrait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => 
            {
				Texture2D texture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/RotGourd/RotGourdBC").Value;
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
					["collectibles"] = ModContent.ItemType<RotGourdBox>(),
					["spawnInfo"] = Language.GetOrRegister("Mods.Spooky.NPCs.RotGourd.BossChecklistIntegration.SpawnInfo").Value,
					["despawnMessage"] = Language.GetOrRegister("Mods.Spooky.NPCs.RotGourd.BossChecklistIntegration.DespawnMessage").Value,
					["customPortrait"] = GourdPortrait,
				}
			);


			//Spooky Spirit
			var SpiritPortrait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => 
            {
				Texture2D texture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/SpookySpirit/SpookySpiritBC").Value;
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
					["collectibles"] = ModContent.ItemType<SpookySpiritBox>(),
					["spawnInfo"] = Language.GetOrRegister("Mods.Spooky.NPCs.SpookySpirit.BossChecklistIntegration.SpawnInfo").Value,
					["despawnMessage"] = Language.GetOrRegister("Mods.Spooky.NPCs.SpookySpirit.BossChecklistIntegration.DespawnMessage").Value,
					["customPortrait"] = SpiritPortrait,
				}
			);


			//Moco
			var MocoPortrait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => 
            {
				Texture2D texture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Moco/MocoBC").Value;
				Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
				spriteBatch.Draw(texture, centered, color);
			};

			bossChecklistMod.Call(
				"LogBoss", 
				Mod,
				nameof(Moco),
				4.01f,
				() => Flags.downedMoco,
				ModContent.NPCType<Moco>(),
				new Dictionary<string, object>() 
				{
					["spawnItems"] = ModContent.ItemType<CottonSwab>(),
					["collectibles"] = ModContent.ItemType<MocoBox>(),
					["spawnInfo"] = Language.GetOrRegister("Mods.Spooky.NPCs.Moco.BossChecklistIntegration.SpawnInfo").Value,
					["despawnMessage"] = Language.GetOrRegister("Mods.Spooky.NPCs.Moco.BossChecklistIntegration.DespawnMessage").Value,
					["customPortrait"] = MocoPortrait,
				}
			);


			//Daffodil
			var DaffodilPortrait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => 
            {
				Texture2D texture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Daffodil/DaffodilBC").Value;
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
					//["collectibles"] = ModContent.ItemType<MocoBox>(),
					["spawnInfo"] = Language.GetOrRegister("Mods.Spooky.NPCs.DaffodilEye.BossChecklistIntegration.SpawnInfo").Value,
					["despawnMessage"] = Language.GetOrRegister("Mods.Spooky.NPCs.DaffodilEye.BossChecklistIntegration.DespawnMessage").Value,
					["customPortrait"] = DaffodilPortrait,
				}
			);


			//Egg incursion
			List<int> EggEventEnemies = new List<int>()
			{
				ModContent.NPCType<Capillary>(), ModContent.NPCType<Crux>(), ModContent.NPCType<Distended>(), ModContent.NPCType<DistendedBrute>(), 
				ModContent.NPCType<Vesicator>(), ModContent.NPCType<Vigilante>(), ModContent.NPCType<Visitant>()
			};

			var EggEventPortrait = (SpriteBatch spriteBatch, Rectangle rect, Color color) =>
			{
				Texture2D texture = ModContent.Request<Texture2D>("Spooky/Content/Events/EggEventBC").Value;
				Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
				spriteBatch.Draw(texture, centered, color);
			};

			List<string> EggEventIcon = new List<string>()
			{
				"Spooky/Content/Events/EggEventIcon"
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
					["displayName"] = Language.GetOrRegister("Mods.Spooky.BossChecklistIntegration.EggEvent.EntryName").Value,
					["spawnItems"] = EggEventItems,
					["collectibles"] = ModContent.ItemType<EggEventBox>(),
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
				Texture2D texture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Orroboro/OrroboroBC").Value;
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
					["collectibles"] = ModContent.ItemType<SpookyHellBossBox>(),
					["spawnInfo"] = Language.GetOrRegister("Mods.Spooky.NPCs.OrroHead.BossChecklistIntegration.SpawnInfo").Value,
					["despawnMessage"] = Language.GetOrRegister("Mods.Spooky.NPCs.OrroHead.BossChecklistIntegration.DespawnMessage").Value,
					["customPortrait"] = OrroboroPortrait,
				}
			);


			//Big Bone
			var BigBonePortrait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => 
            {
				Texture2D texture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/BigBoneBC").Value;
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
					["spawnItems"] = ModContent.ItemType<Fertalizer>(),
					["collectibles"] = ModContent.ItemType<BigBoneBox>(),
					["spawnInfo"] = Language.GetOrRegister("Mods.Spooky.NPCs.BigBone.BossChecklistIntegration.SpawnInfo").Value,
					["despawnMessage"] = Language.GetOrRegister("Mods.Spooky.NPCs.BigBone.BossChecklistIntegration.DespawnMessage").Value,
					["customPortrait"] = BigBonePortrait,
				}
			);
		}
    }       
}