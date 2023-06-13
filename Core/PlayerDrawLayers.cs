using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Content.Buffs.Debuff;

namespace Spooky.Core
{
    //helmet extension stuff
    public interface IExtendedHelmet
    {
        string ExtensionTexture { get; }

        Vector2 ExtensionSpriteOffset(PlayerDrawSet drawInfo);

        bool PreDrawExtension(PlayerDrawSet drawInfo) => true;

        string EquipSlotName(Player drawPlayer) => "";
    }

    //helmet extension for helmets that are bigger than the player sprite sheet size
    public class HelmetExtensionLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Head);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.shadow == 0f || !drawInfo.drawPlayer.dead;

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            Item headItem = drawPlayer.armor[0];

            if (drawPlayer.armor[10].type > ItemID.None)
                headItem = drawPlayer.armor[10];

            if (ModContent.GetModItem(headItem.type) is IExtendedHelmet extendedHatDrawer)
            {
                string equipSlotName = extendedHatDrawer.EquipSlotName(drawPlayer) != "" ? extendedHatDrawer.EquipSlotName(drawPlayer) : headItem.ModItem.Name;
                int equipSlot = EquipLoader.GetEquipSlot(Mod, equipSlotName, EquipType.Head);

                if (extendedHatDrawer.PreDrawExtension(drawInfo) && !drawInfo.drawPlayer.dead && equipSlot == drawPlayer.head)
                {
                    int dyeShader = drawPlayer.dye?[0].dye ?? 0;

                    Vector2 headDrawPosition = drawInfo.Position - Main.screenPosition;

                    headDrawPosition += new Vector2((drawPlayer.width - drawPlayer.bodyFrame.Width) / 2f, drawPlayer.height - drawPlayer.bodyFrame.Height + 4f);

                    headDrawPosition = new Vector2((int)headDrawPosition.X, (int)headDrawPosition.Y);

                    headDrawPosition += drawPlayer.headPosition + drawInfo.headVect;

                    headDrawPosition += extendedHatDrawer.ExtensionSpriteOffset(drawInfo);

                    Texture2D extraPieceTexture = ModContent.Request<Texture2D>(extendedHatDrawer.ExtensionTexture).Value;

                    Rectangle frame = extraPieceTexture.Frame(1, 20, 0, drawPlayer.bodyFrame.Y / drawPlayer.bodyFrame.Height);

                    DrawData pieceDrawData = new DrawData(extraPieceTexture, headDrawPosition, frame, drawInfo.colorArmorHead, drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0)
                    {
                        shader = dyeShader
                    };

                    drawInfo.DrawDataCache.Add(pieceDrawData);
                }
            }
        }
    }

    //vanity glowmask layer for my dev mask because it has a flickering effect, wasnt sure how to do this with the other armor glowmask layer
    public class DylanGlowmaskPlayer : ModPlayer
    {
        internal static readonly Dictionary<int, Texture2D> ItemGlowMask = new Dictionary<int, Texture2D>();

		internal new static void Unload() => ItemGlowMask.Clear();
		public static void AddGlowMask(int itemType, string texturePath) => ItemGlowMask[itemType] = ModContent.Request<Texture2D>(texturePath, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
    }

    public abstract class DylanGlowmaskVanityLayer : PlayerDrawLayer
	{
		protected abstract int ID { get; }
		protected abstract EquipType Type { get; }

		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			if (!drawInfo.drawPlayer.armor[ID].IsAir)
			{
				if (drawInfo.drawPlayer.armor[ID].type >= ItemID.Count && !Main.dayTime &&
				DylanGlowmaskPlayer.ItemGlowMask.TryGetValue(drawInfo.drawPlayer.armor[ID].type, out Texture2D textureLegs))
				{
					for (int i = 0; i < 2; i++)
					{
						DrawHeadGlowMask(Type, textureLegs, drawInfo);
					}
				}
			}
		}

		public static void DrawHeadGlowMask(EquipType type, Texture2D texture, PlayerDrawSet info)
		{
			float shakeX = Main.rand.Next(-1, 1);
			float shakeY = Main.rand.Next(-1, 1);

			Vector2 adjustedPosition = new Vector2((int)(info.Position.X - Main.screenPosition.X + shakeX) + 
			((info.drawPlayer.width - info.drawPlayer.bodyFrame.Width) / 2), (int)(info.Position.Y - Main.screenPosition.Y + shakeY) + 
			info.drawPlayer.height - info.drawPlayer.bodyFrame.Height + 4);

			DrawData drawData = new DrawData(texture, adjustedPosition + info.drawPlayer.headPosition + info.rotationOrigin, info.drawPlayer.bodyFrame, info.headGlowColor, info.drawPlayer.headRotation, info.rotationOrigin, 1f, info.playerEffect, 0)
			{

			};
			info.DrawDataCache.Add(drawData);
		}
	}

    //armor glowmask layer for regular glowmasks
    public abstract class HelmetGlowmaskVanityLayer : PlayerDrawLayer
	{
		protected abstract int ID { get; }
		protected abstract EquipType Type { get; }

		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			if (!drawInfo.drawPlayer.armor[ID].IsAir)
			{
				if (drawInfo.drawPlayer.armor[ID].type >= ItemID.Count &&
				SpookyPlayer.ItemGlowMask.TryGetValue(drawInfo.drawPlayer.armor[ID].type, out Texture2D textureLegs))
				{
					DrawHeadGlowMask(Type, textureLegs, drawInfo);
				}
			}
		}

		public static void DrawHeadGlowMask(EquipType type, Texture2D texture, PlayerDrawSet info)
		{
			Vector2 adjustedPosition = new Vector2((int)(info.Position.X - Main.screenPosition.X) + 
			((info.drawPlayer.width - info.drawPlayer.bodyFrame.Width) / 2), (int)(info.Position.Y - Main.screenPosition.Y) + 
			info.drawPlayer.height - info.drawPlayer.bodyFrame.Height + 4);

			DrawData drawData = new DrawData(texture, adjustedPosition + info.drawPlayer.headPosition + info.rotationOrigin, info.drawPlayer.bodyFrame, info.headGlowColor, info.drawPlayer.headRotation, info.rotationOrigin, 1f, info.playerEffect, 0)
			{

			};
			info.DrawDataCache.Add(drawData);
		}
	}

    //cross charm drawing
    public class CrossCharmShield : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.WebbedDebuffBack);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return drawInfo.drawPlayer.GetModPlayer<SpookyPlayer>().CrossCharmShield && !drawInfo.drawPlayer.HasBuff(ModContent.BuffType<CrossCooldown>());
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/Items/Catacomb/CrossCharmDraw").Value;

            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6.28318548f)) / 2f + 0.5f;

            Main.EntitySpriteDraw(tex, new Vector2(drawInfo.drawPlayer.MountedCenter.X, drawInfo.drawPlayer.MountedCenter.Y - 45) - Main.screenPosition, null, Color.White, 0f, tex.Size() / 2, 0.8f + fade / 2f, SpriteEffects.None, 0);
        }
    }

    //gore monger aura drawing
    public class GoreAura : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.WebbedDebuffBack);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return !drawInfo.drawPlayer.HasBuff<GoreAuraCooldown>() && drawInfo.drawPlayer.GetModPlayer<SpookyPlayer>().GoreArmorSet;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6.28318548f)) / 2f + 0.5f;

            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/Items/SpookyHell/Armor/GoreAuraEffect").Value;
            Color color = Color.Lerp(Color.Lerp(new Color(75, 5, 20, 10), new Color(255, 0, 50, 255), fade), new Color(75, 5, 20, 10), fade);

            Color realColor;

            if (!drawInfo.drawPlayer.armorEffectDrawOutlines && !drawInfo.drawPlayer.armorEffectDrawShadow)
            {
                realColor = color * 1.2f;
            }
            else
            {
                realColor = color * 0.25f;
            }

            Main.EntitySpriteDraw(tex, new Vector2(drawInfo.drawPlayer.MountedCenter.X - 1, drawInfo.drawPlayer.MountedCenter.Y) - Main.screenPosition, null, realColor, 0f, tex.Size() / 2, 0.8f + fade / 2f, SpriteEffects.None, 0);
        }
    }
}