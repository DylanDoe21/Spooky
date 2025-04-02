using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.NPCs.Boss.SpookFishron;
using System;
using Terraria.ID;

namespace Spooky.Content.Backgrounds
{
    public class FishronSky : CustomSky
    {
		private bool isActive = false;
		private float intensity = 0f;
		private int FishronNPC = -1;

		public override void Update(GameTime gameTime)
		{
			if (FishronNPC == -1)
			{
				FindActiveFishron();
				if (FishronNPC == -1)
				{
					isActive = false;
				}
			}

			if (isActive && intensity < 1f)
			{
				intensity += 0.01f;
			}
			else if (!isActive && intensity > 0f)
			{
				intensity -= 0.01f;
			}
		}

		public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
		{
			if (FindActiveFishron() && !Main.gameMenu)
			{
				//change the color to dark purple if spook fishron is in his expert mode exclusive phase
                bool ExpertPhase = (Main.npc[FishronNPC].ai[0] == -2 && Main.npc[FishronNPC].localAI[0] >= 120) || Main.npc[FishronNPC].ai[0] == 7;
				Color TintColor = ExpertPhase ? (Main.snowMoon ? new Color(25, 32, 46) : new Color(32, 13, 42)) * 0.95f : (Main.snowMoon ? Color.LightBlue : Color.OrangeRed) * 0.12f;

				if (maxDepth >= 0 && minDepth < 0)
				{
					float ClosebyIntensity = GetIntensity() * intensity;

					spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth * 2, Main.screenHeight * 2), TintColor * ClosebyIntensity);
				}
			}
		}

		public override void Activate(Vector2 position, params object[] args)
		{
			isActive = true;
		}

		public override void Deactivate(params object[] args)
		{
			isActive = false;
		}

		public override void Reset()
		{
			isActive = false;
		}

		public override bool IsActive()
		{
			return isActive || intensity > 0f;
		}

		private float GetIntensity()
		{
			if (FindActiveFishron())
			{
				float FishronIntensity = 0f;
				if (FishronNPC != -1)
				{
					FishronIntensity = Vector2.Distance(Main.player[Main.myPlayer].Center, Main.npc[FishronNPC].Center);
				}

				return (1f - Utils.SmoothStep(3000f, 6000f, FishronIntensity)) * intensity;
			}

			return 1f;
		}

		private bool FindActiveFishron()
		{
			if (FishronNPC >= 0 && Main.npc[FishronNPC].active && Main.npc[FishronNPC].type == ModContent.NPCType<SpookFishron>())
			{
				return true;
			}

			FishronNPC = NPC.FindFirstNPC(ModContent.NPCType<SpookFishron>());

			return FishronNPC != -1;
		}
	}

    public class FishronScreenShaderData : ScreenShaderData
    {
        private int FishronNPC;

        public FishronScreenShaderData(string passName) : base(passName)
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (FishronNPC == -1)
            {
				FindActiveFishron();
				
                if (FishronNPC == -1)
				{
                    Filters.Scene["Spooky:SpookFishron"].Deactivate();
				}
            }
        }

        public override void Apply()
        {
			FindActiveFishron();

            if (FishronNPC != -1)
            {
                UseTargetPosition(Main.npc[FishronNPC].Center);
            }

            base.Apply();
        }

		private void FindActiveFishron()
		{
			if (FishronNPC >= 0 && Main.npc[FishronNPC].active && Main.npc[FishronNPC].type == ModContent.NPCType<SpookFishron>())
			{
				return;
			}

			FishronNPC = NPC.FindFirstNPC(ModContent.NPCType<SpookFishron>());
		}
	}

	public class SpookFishronScene : ModSceneEffect
	{
		private int FishronNPC = -1;

		//public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookFishron");

		public override void Load()
		{
			On_Main.UpdateAudio_DecideOnNewMusic += FishronMusicOverride;
		}

		public override void Unload()
		{
			On_Main.UpdateAudio_DecideOnNewMusic -= FishronMusicOverride;
		}

		private void FishronMusicOverride(On_Main.orig_UpdateAudio_DecideOnNewMusic orig, Main self)
		{
			if (FindActiveFishron() && !Main.gameMenu)
			{
				Main.newMusic = Main.snowMoon ? MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookFishronXmas") : MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookFishron");
				return;
			}
			else
			{
				orig(self);
			}
		}

		public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

		public override bool IsSceneEffectActive(Player player) => FindActiveFishron() && !Main.gameMenu;

		public override void SpecialVisuals(Player player, bool isActive)
		{
			player.ManageSpecialBiomeVisuals("Spooky:SpookFishron", isActive);
		}

		private bool FindActiveFishron()
		{
			if (FishronNPC >= 0 && Main.npc[FishronNPC].active && Main.npc[FishronNPC].type == ModContent.NPCType<SpookFishron>())
			{
				return true;
			}

			FishronNPC = NPC.FindFirstNPC(ModContent.NPCType<SpookFishron>());

			return FishronNPC != -1;
		}
	}
}
