using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Buffs.Debuff;

namespace Spooky.Core
{
	//special helmet drawing stuff
	public interface ISpecialHelmetDraw
	{
		string HeadTexture => string.Empty;

		string GlowTexture => string.Empty;

		Vector2 Offset => Vector2.Zero;

		Vector2 GlowOffset => Vector2.Zero;
	}

    //special drawing for helmets, such as helmets that are too long, too tall, or for glowmasks
    public class SpecialHelmetDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Head);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            Item headItem = drawPlayer.armor[0];

            if (drawPlayer.armor[10].type > ItemID.None)
            {
                headItem = drawPlayer.armor[10];
            }

			//handle textures for helmets that change with player direction
			//just checks if the texture path contains "_Flipped" which may not be the best solution but it works
            if (ModContent.GetModItem(headItem.type) is ISpecialHelmetDraw HelmetDrawer)
            {
				//if the helmet texture is a directional texture, then only draw the texture when the player is facing a certain direction
                if (HelmetDrawer.HeadTexture.Contains("_Flipped"))
                {
                    return (drawInfo.shadow == 0f || !drawInfo.drawPlayer.dead) && drawPlayer.direction == -1;
                }
            }

			//otherwise use normal visibility conditions
            return drawInfo.shadow == 0f || !drawInfo.drawPlayer.dead;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            Item headItem = drawPlayer.armor[0];

            if (drawPlayer.armor[10].type > ItemID.None)
            {
                headItem = drawPlayer.armor[10];
            }

            if (ModContent.GetModItem(headItem.type) is ISpecialHelmetDraw HelmetDrawer)
            {
                string equipSlotName = headItem.ModItem.Name;
                int equipSlot = EquipLoader.GetEquipSlot(Mod, equipSlotName, EquipType.Head);

                if (!drawInfo.drawPlayer.dead && equipSlot == drawPlayer.head)
                {
					//draw the actual texture if its defined
					if (HelmetDrawer.HeadTexture != string.Empty)
					{
						Texture2D Tex = ModContent.Request<Texture2D>(HelmetDrawer.HeadTexture).Value;

						Rectangle frame = Tex.Frame(1, 20, 0, drawPlayer.bodyFrame.Y / drawPlayer.bodyFrame.Height);
						Vector2 drawPos = drawInfo.Position - Main.screenPosition + new Vector2(drawPlayer.width / 2 - frame.Width / 2, drawPlayer.height - frame.Height + 4f) + drawPlayer.headPosition;
						drawPos = drawPos.Floor();
						Vector2 origin = drawInfo.headVect;
						float rotation = drawPlayer.headRotation;

						Vector2 GravityOffset = drawPlayer.gravDir == 1 ? HelmetDrawer.Offset : -HelmetDrawer.Offset - (HelmetDrawer.Offset.Y == 0 ? Vector2.Zero : new Vector2(0f, 4f));

						DrawData drawData = new DrawData(Tex, drawPos - GravityOffset + origin, frame, drawInfo.colorArmorHead, rotation, origin, 1f, drawInfo.playerEffect, 0)
						{
							shader = drawInfo.cHead
						};
						drawInfo.DrawDataCache.Add(drawData);
					}

					//draw the glowmask texture if a glowmask texture is defined
					if (HelmetDrawer.GlowTexture != string.Empty)
					{
						Texture2D Tex = ModContent.Request<Texture2D>(HelmetDrawer.GlowTexture).Value;

						Rectangle frame = Tex.Frame(1, 20, 0, drawPlayer.bodyFrame.Y / drawPlayer.bodyFrame.Height);
						Vector2 drawPos = drawInfo.Position - Main.screenPosition + new Vector2(drawPlayer.width / 2 - frame.Width / 2, drawPlayer.height - frame.Height + 4f) + drawPlayer.headPosition;
						drawPos = drawPos.Floor();
						Vector2 origin = drawInfo.headVect;
						float rotation = drawPlayer.headRotation;

						Vector2 GravityOffset = drawPlayer.gravDir == 1 ? HelmetDrawer.GlowOffset : -HelmetDrawer.GlowOffset - (HelmetDrawer.GlowOffset.Y == 0 ? Vector2.Zero : new Vector2(0f, 4f));

						DrawData drawData = new DrawData(Tex, drawPos - GravityOffset + origin, frame, Color.White, rotation, origin, 1f, drawInfo.playerEffect, 0)
						{
							shader = drawInfo.cHead
						};
						drawInfo.DrawDataCache.Add(drawData);
					}
				}
            }
        }
    }

	public class HeadUrchin : PlayerDrawLayer
	{
		public float addedStretch = 0f;
		public float stretchRecoil = 0f;

		public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.FinchNest);

		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
		{
			return drawInfo.drawPlayer.GetModPlayer<BloomBuffsPlayer>().SeaUrchin && !drawInfo.drawPlayer.dead;
		}

		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			if (drawInfo.drawPlayer.dead)
			{
				return;
			}

			Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Blooms/HeadUrchin").Value;

			float stretch = 0f;

			stretch = Math.Abs(stretch) - addedStretch;

			//limit how much it can stretch
			if (stretch > 0.5f)
			{
				stretch = 0.5f;
			}

			//limit how much it can squish
			if (stretch < -0.5f)
			{
				stretch = -0.5f;
			}

			Vector2 scaleStretch = new Vector2(1f + stretch, 1f - stretch);

			//stretch stuff
			if (stretchRecoil > 0)
			{
				stretchRecoil -= 0.02f;
			}
			else
			{
				stretchRecoil = 0;
			}

			addedStretch = -stretchRecoil;

			//copied from vanilla finch minion nest drawing
			Rectangle bodyFrame5 = new Rectangle(0, 0, tex.Width, tex.Height);
			bodyFrame5.Y = 0;
			Vector2 vector6 = new Vector2(0f, drawInfo.drawPlayer.gravDir == 1 ? -8f : 0f);
			Color color8 = drawInfo.colorArmorHead;
			if (drawInfo.drawPlayer.mount.Active && drawInfo.drawPlayer.mount.Type == 52)
			{
				Vector2 mountedCenter = drawInfo.drawPlayer.MountedCenter;
				color8 = drawInfo.drawPlayer.GetImmuneAlphaPure(Lighting.GetColorClamped((int)mountedCenter.X / 16, (int)mountedCenter.Y / 16, Color.White), drawInfo.shadow);
				vector6 = new Vector2(0f, -2f) * drawInfo.drawPlayer.Directions;
			}
			DrawData item = new DrawData(tex, vector6 + new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (drawInfo.drawPlayer.bodyFrame.Width / 2) + (drawInfo.drawPlayer.width / 2)), 
			(int)(drawInfo.Position.Y - Main.screenPosition.Y + drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.headPosition + drawInfo.headVect + Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height] * drawInfo.drawPlayer.gravDir, 
			bodyFrame5, color8, drawInfo.drawPlayer.headRotation, drawInfo.headVect, scaleStretch, drawInfo.playerEffect);
			drawInfo.DrawDataCache.Add(item);
		}
	}

    //cross charm drawing
    public class CrossCharmShield : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.WebbedDebuffBack);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return drawInfo.drawPlayer.GetModPlayer<SpookyPlayer>().CrossCharmShield && !drawInfo.drawPlayer.dead;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.dead)
            {
                return;
            }

            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/Items/Catacomb/CrossCharmDraw").Value;

            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) / 2f + 0.5f;

            Vector2 roundedPos = new Vector2(MathF.Round(drawInfo.drawPlayer.MountedCenter.X, MidpointRounding.ToNegativeInfinity),
            MathF.Round(drawInfo.drawPlayer.MountedCenter.Y - 45, MidpointRounding.ToNegativeInfinity));

            drawInfo.DrawDataCache.Add(new DrawData(tex, roundedPos - Main.screenPosition, null, Color.White, 0f, tex.Size() / 2, 0.8f + fade / 2f, SpriteEffects.None, 0));
        }
    }

    //monument mythos pyramid drawing
    public class MonumentMythosPyramidDraw : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.BeetleBuff);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return drawInfo.drawPlayer.GetModPlayer<SpookyPlayer>().MonumentMythosPyramid && !drawInfo.drawPlayer.HasBuff(ModContent.BuffType<MonumentMythosCooldown>());
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.dead)
            {
                return;
            }

            Texture2D tex1 = ModContent.Request<Texture2D>("Spooky/Content/Items/Cemetery/Contraband/MonumentMythosPyramidDraw").Value;
            Texture2D tex2 = ModContent.Request<Texture2D>("Spooky/Content/Items/Cemetery/Contraband/MonumentMythosPyramidDrawInside").Value;
            Color color = Lighting.GetColor((int)drawInfo.drawPlayer.MountedCenter.X / 16, (int)(drawInfo.drawPlayer.MountedCenter.Y / 16f));

            Vector2 roundedPos = new Vector2(MathF.Round(drawInfo.drawPlayer.MountedCenter.X, MidpointRounding.ToNegativeInfinity),
            MathF.Round(drawInfo.drawPlayer.MountedCenter.Y - 10, MidpointRounding.ToNegativeInfinity));

            drawInfo.DrawDataCache.Add(new DrawData(tex1, roundedPos - Main.screenPosition, null, color, 0, tex1.Size() / 2, 1.2f, SpriteEffects.None, 0));
            drawInfo.DrawDataCache.Add(new DrawData(tex2, roundedPos - Main.screenPosition, null, color * 0.8f, 0, tex2.Size() / 2, 1.2f, SpriteEffects.None, 0));
        }
    }

    //fossil protea drawing
    public class FossilProteaShellDrawBack : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.WebbedDebuffBack);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return drawInfo.drawPlayer.GetModPlayer<BloomBuffsPlayer>().FossilProtea;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.dead)
            {
                return;
            }

            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Blooms/FossilProteaShieldBack").Value;
            Color color = Lighting.GetColor((int)drawInfo.drawPlayer.MountedCenter.X / 16, (int)(drawInfo.drawPlayer.MountedCenter.Y / 16f));

            Vector2 roundedPos = new Vector2(MathF.Round(drawInfo.drawPlayer.MountedCenter.X, MidpointRounding.ToNegativeInfinity),
            MathF.Round(drawInfo.drawPlayer.MountedCenter.Y + 2, MidpointRounding.ToNegativeInfinity));

            drawInfo.DrawDataCache.Add(new DrawData(tex, roundedPos - Main.screenPosition, null, color, 0, tex.Size() / 2, 1f, SpriteEffects.None, 0));
        }
    }
    public class FossilProteaShellDrawFront : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.FinchNest);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return drawInfo.drawPlayer.GetModPlayer<BloomBuffsPlayer>().FossilProtea;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.dead)
            {
                return;
            }

			Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Blooms/FossilProteaShieldFront").Value;
			Color color = Lighting.GetColor((int)drawInfo.drawPlayer.MountedCenter.X / 16, (int)(drawInfo.drawPlayer.MountedCenter.Y / 16f));

            Vector2 roundedPos = new Vector2(MathF.Round(drawInfo.drawPlayer.MountedCenter.X, MidpointRounding.ToNegativeInfinity),
            MathF.Round(drawInfo.drawPlayer.MountedCenter.Y + 2, MidpointRounding.ToNegativeInfinity));

            drawInfo.DrawDataCache.Add(new DrawData(tex, roundedPos - Main.screenPosition, null, color, 0, tex.Size() / 2, 1f, SpriteEffects.None, 0));
        }
    }

    //dutchman pipe aura drawing
    public class DutchmanPipeRingDraw : PlayerDrawLayer
    {
		float DutchmanRingRotation;
		float DutchmanRingScale = 1f;

		public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Carpet);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return drawInfo.drawPlayer.GetModPlayer<BloomBuffsPlayer>().FossilDutchmanPipe;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.dead)
            {
                return;
            }

            Texture2D DutchmanPipeRingTex1 = ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Blooms/DutchmanPipeRing").Value;
            Texture2D DutchmanPipeRingTex2 = ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Blooms/DutchmanPipeRingInside").Value;
            Texture2D DutchmanPipeRingTex3 = ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Blooms/DutchmanPipeRingPattern").Value;

            float num = 1f;
            float num2 = 0.1f;
            float num3 = 0.9f;
            if (!Main.gamePaused)
            {
                DutchmanRingScale += 0.004f;
                DutchmanRingRotation += 0.01f;
            }

            if (DutchmanRingScale < 1f)
            {
                num = DutchmanRingScale;
            }
            else
            {
                DutchmanRingScale = 0.8f;
                num = DutchmanRingScale;
            }

            if (DutchmanRingRotation > (float)Math.PI * 2f)
            {
                DutchmanRingRotation -= (float)Math.PI * 2f;
            }

            if (DutchmanRingRotation < (float)Math.PI * 2f)
            {
                DutchmanRingRotation += (float)Math.PI * 2f;
            }

            for (int i = 0; i < 4; i++)
            {
                float num4 = num + num2 * (i / 2);
                if (num4 > 1f)
                {
                    num4 -= num2 * 2f;
                }

                float num5 = MathHelper.Lerp(0.8f, 0f, Math.Abs(num4 - num3) * 10f);

                for (int j = 0; j < 360; j += 90)
                {
                    Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 12f), Main.rand.NextFloat(1f, 12f)).RotatedBy(MathHelper.ToRadians(j));

                    drawInfo.DrawDataCache.Add(new DrawData(DutchmanPipeRingTex1, drawInfo.drawPlayer.MountedCenter - Main.screenPosition + circular, new Rectangle(0, 0, 348, 348), 
					Color.Red * 0.5f * num5, DutchmanRingRotation + (float)Math.PI / 3f * i, new Vector2(348 / 2, 348 / 2), num4 * 1.5f, SpriteEffects.None, 0f));
                }

                drawInfo.DrawDataCache.Add(new DrawData(DutchmanPipeRingTex2, drawInfo.drawPlayer.MountedCenter - Main.screenPosition, new Rectangle(0, 0, 348, 348), 
				Color.Red * 0.25f * num5, DutchmanRingRotation + (float)Math.PI / 3f * i, new Vector2(348 / 2, 348 / 2), num4 * 1.5f, SpriteEffects.None, 0f));

                drawInfo.DrawDataCache.Add(new DrawData(DutchmanPipeRingTex3, drawInfo.drawPlayer.MountedCenter - Main.screenPosition, new Rectangle(0, 0, 348, 348), 
				Color.White * num5, DutchmanRingRotation + (float)Math.PI / 3f * i, new Vector2(348 / 2, 348 / 2), num4 * 1.5f, SpriteEffects.None, 0f));
            }
        }
    }

    //biome compass drawing
    public class BiomeCompassDraw : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.WebbedDebuffBack);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return (drawInfo.drawPlayer.GetModPlayer<SpookyPlayer>().SpiderGrottoCompass || drawInfo.drawPlayer.GetModPlayer<SpookyPlayer>().EyeValleyCompass) && !drawInfo.drawPlayer.dead;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.dead)
            {
                return;
            }

            Texture2D GrottoCompassTex = ModContent.Request<Texture2D>("Spooky/Content/Items/SpiderCave/Misc/GrottoCompass").Value;
            Texture2D EyeValleyCompassTex = ModContent.Request<Texture2D>("Spooky/Content/Items/SpookyHell/Misc/EyeValleyCompass").Value;
            Texture2D ArrowTex = ModContent.Request<Texture2D>("Spooky/Content/Items/CompassArrow").Value;

            float pulse = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) / 2f + 0.5f;

            Vector2 roundedPos = new Vector2(MathF.Round(drawInfo.drawPlayer.MountedCenter.X, MidpointRounding.ToNegativeInfinity),
            MathF.Round(drawInfo.drawPlayer.MountedCenter.Y - 45, MidpointRounding.ToNegativeInfinity));

            if (drawInfo.drawPlayer.GetModPlayer<SpookyPlayer>().SpiderGrottoCompass)
            {
                Vector2 vector = new Vector2(roundedPos.X, roundedPos.Y);
                float RotateX = Flags.SpiderGrottoCenter.X - vector.X;
                float RotateY = Flags.SpiderGrottoCenter.Y - vector.Y;
                float rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

                drawInfo.DrawDataCache.Add(new DrawData(GrottoCompassTex, roundedPos - Main.screenPosition, null, Color.White, 0f, GrottoCompassTex.Size() / 2, 1f, SpriteEffects.None, 0));
                drawInfo.DrawDataCache.Add(new DrawData(ArrowTex, roundedPos - Main.screenPosition, null, Color.White, rotation, ArrowTex.Size() / 2, 0.8f + pulse / 2f, SpriteEffects.None, 0));
            }
            if (drawInfo.drawPlayer.GetModPlayer<SpookyPlayer>().EyeValleyCompass)
            {
                Vector2 vector = new Vector2(roundedPos.X, roundedPos.Y);
                float RotateX = Flags.EyeValleyCenter.X - vector.X;
                float RotateY = Flags.EyeValleyCenter.Y - vector.Y;
                float rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

                drawInfo.DrawDataCache.Add(new DrawData(EyeValleyCompassTex, roundedPos - Main.screenPosition, null, Color.White, 0f, EyeValleyCompassTex.Size() / 2, 1f, SpriteEffects.None, 0));
                drawInfo.DrawDataCache.Add(new DrawData(ArrowTex, roundedPos - Main.screenPosition, null, Color.White, rotation, ArrowTex.Size() / 2, 0.8f + pulse / 2f, SpriteEffects.None, 0));
            }
        }
    }
}