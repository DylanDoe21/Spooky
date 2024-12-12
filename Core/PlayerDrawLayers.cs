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
            MathF.Round(drawInfo.drawPlayer.MountedCenter.Y - 45, MidpointRounding.AwayFromZero));

            drawInfo.DrawDataCache.Add(new DrawData(tex, roundedPos - Main.screenPosition, null, Color.White, 0f, tex.Size() / 2, 0.8f + fade / 2f, SpriteEffects.None, 0));
        }
    }

    //daffodil hairpin ring drawing
    public class DaffodilHairpinDraw : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.WebbedDebuffBack);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return drawInfo.drawPlayer.GetModPlayer<SpookyPlayer>().DaffodilHairpin;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.dead)
            {
                return;
            }

            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) / 2f + 0.5f;

            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/Items/BossBags/Accessory/DaffodilHairpinDraw").Value;
            Color color = Lighting.GetColor((int)drawInfo.drawPlayer.MountedCenter.X / 16, (int)(drawInfo.drawPlayer.MountedCenter.Y / 16f));

            Vector2 roundedPos = new Vector2(MathF.Round(drawInfo.drawPlayer.MountedCenter.X, MidpointRounding.ToNegativeInfinity),
            MathF.Round(drawInfo.drawPlayer.MountedCenter.Y, MidpointRounding.AwayFromZero));

            drawInfo.DrawDataCache.Add(new DrawData(tex, roundedPos - Main.screenPosition, null, color, 0, tex.Size() / 2, 0.8f + fade / 2f, SpriteEffects.None, 0));
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
            MathF.Round(drawInfo.drawPlayer.MountedCenter.Y - 10, MidpointRounding.AwayFromZero));

            drawInfo.DrawDataCache.Add(new DrawData(tex1, roundedPos - Main.screenPosition, null, color, 0, tex1.Size() / 2, 1.2f, SpriteEffects.None, 0));
            drawInfo.DrawDataCache.Add(new DrawData(tex2, roundedPos - Main.screenPosition, null, color * 0.8f, 0, tex2.Size() / 2, 1.2f, SpriteEffects.None, 0));
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
            MathF.Round(drawInfo.drawPlayer.MountedCenter.Y - 45, MidpointRounding.AwayFromZero));

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