using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Backgrounds.SpookyBiome;
using Spooky.Content.Gores.Misc;
using Spooky.Content.Tiles.SpookyBiome.Furniture;
using Spooky.Content.Tiles.Water;

namespace Spooky.Content.Biomes
{
    public class SpookyBiome : ModBiome
    {
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<SpookyForestBG>();

        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<SpookyUndergroundBackgroundStyle>();

        public override int Music
        {
            get
            {
                int music = Main.curMusic;

                if (!Main.bloodMoon && !Main.eclipse)
                {
                    //play normal theme
                    if (!Main.raining)
                    {
						//play town music if enough town npcs exist
						if (Main.LocalPlayer.townNPCs > 2f)
						{
							if (Main.dayTime)
							{
								music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyTownDay");
							}
							else
							{
								music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyTownNight");
							}
						}
						//play normal music
						else
						{
							if (Main.dayTime)
							{
								music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyBiomeDay");
							}
							else
							{
								music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyBiomeNight");
							}
						}
                    }
                    //play spooky biome rain song if it is raining
                    else if (Main.raining)
                    {
                        music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyBiomeRain");
                    }
                }
                else
                {
                    if (Main.bloodMoon)
                    {
                        music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyBloodmoon");
                    }

                    if (Main.eclipse)
                    {
                        music = MusicID.Eclipse;
                    }
                }
                
                return music;
            }
        }
       
        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

        public override ModWaterStyle WaterStyle => ModContent.GetInstance<SpookyWaterStyle>();

        public override int BiomeTorchItemType => ModContent.ItemType<SpookyBiomeTorchItem>();

        //bestiary stuff
        public override string BestiaryIcon => "Spooky/Content/Biomes/SpookyBiomeIcon";
        public override string MapBackground => BackgroundPath;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

        public override void SpecialVisuals(Player player, bool isActive)
        {
            //set isActive to also account for the underground spooky forest to use the tint in both the aboveground and underground biomes
            isActive = player.InModBiome<SpookyBiome>() || player.InModBiome<SpookyBiomeUg>();
            player.ManageSpecialBiomeVisuals("Spooky:SpookyForestTint", isActive, player.Center);
        }

        public override void OnInBiome(Player player)
        {
            //spawn falling leaves while in the spooky forest
            int[] Leaves = new int[] { ModContent.GoreType<LeafGreen>(), ModContent.GoreType<LeafOrange>(), ModContent.GoreType<LeafRed>() };

            Vector2 scaledPosition = Main.Camera.ScaledPosition;

            if (Main.rand.NextBool(40) && player.ZoneOverworldHeight)
            {
                float Scale = Main.rand.NextFloat(1f, 1.2f);
                int SpawnX = (int)Main.screenPosition.X - 100;
                int SpawnY = (int)Main.screenPosition.Y + Main.rand.Next(-100, Main.screenHeight);

                int dustPosXWorld = SpawnX / 16;
                int dustPosYWorld = SpawnY / 16;
                if (WorldGen.InWorld(dustPosXWorld, dustPosYWorld) && Main.tile[dustPosXWorld, dustPosYWorld] != null && !Main.tile[dustPosXWorld, dustPosYWorld].HasUnactuatedTile && Main.tile[dustPosXWorld, dustPosYWorld].WallType == 0)
                {
                    int LeafGore = Gore.NewGore(null, new Vector2(SpawnX, SpawnY), Vector2.Zero, Leaves[Main.rand.Next(3)], Scale);
                    Main.gore[LeafGore].rotation = 0f;
                    Main.gore[LeafGore].velocity.X = Main.rand.NextFloat(0.5f, 3.5f);
                    Main.gore[LeafGore].velocity.Y = Main.rand.NextFloat(0.5f, 1.2f);
                }
            }

            if (Main.rand.NextBool(40) && player.ZoneOverworldHeight)
            {
                float Scale = Main.rand.NextFloat(1f, 1.2f);
                int SpawnX = (int)Main.screenPosition.X + Main.screenWidth + 100;
                int SpawnY = (int)Main.screenPosition.Y + Main.rand.Next(-100, Main.screenHeight);

                int dustPosXWorld = SpawnX / 16;
                int dustPosYWorld = SpawnY / 16;
                if (WorldGen.InWorld(dustPosXWorld, dustPosYWorld) && Main.tile[dustPosXWorld, dustPosYWorld] != null && !Main.tile[dustPosXWorld, dustPosYWorld].HasUnactuatedTile && Main.tile[dustPosXWorld, dustPosYWorld].WallType == 0)
                {
                    int LeafGore = Gore.NewGore(null, new Vector2(SpawnX, SpawnY), Vector2.Zero, Leaves[Main.rand.Next(3)], Scale);
                    Main.gore[LeafGore].rotation = 0f;
                    Main.gore[LeafGore].velocity.X = Main.rand.NextFloat(-0.5f, -3.5f);
                    Main.gore[LeafGore].velocity.Y = Main.rand.NextFloat(0.5f, 1.2f);
                }
            }
        }

        //conditions to be in the biome
        public override bool IsBiomeActive(Player player)
        {
            bool BiomeCondition = ModContent.GetInstance<TileCount>().spookyTiles >= 1000;
            bool SurfaceCondition = player.ZoneOverworldHeight;

            return BiomeCondition && SurfaceCondition;
        }
    }
}