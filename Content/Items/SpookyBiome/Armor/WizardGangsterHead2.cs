using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Tiles.SpookyBiome;
using System.Linq;

namespace Spooky.Content.Items.SpookyBiome.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class WizardGangsterHead2 : ModItem, IExtendedHelmet
	{
		public string ExtensionTexture => "Spooky/Content/Items/SpookyBiome/Armor/WizardGangsterHead2_Hat";
        public Vector2 ExtensionSpriteOffset(PlayerDrawSet drawInfo) => new Vector2(0, -8f);

		public override void SetStaticDefaults()
		{
			SpookyPlayer.AddGlowMask(Item.type, "Spooky/Content/Items/SpookyBiome/Armor/WizardGangsterHead_Glow");
		}

		public override void SetDefaults() 
		{
			Item.defense = 2;
			Item.width = 30;
			Item.height = 26;
			Item.rare = ItemRarityID.Blue;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<WizardGangsterBody>() && legs.type == ModContent.ItemType<WizardGangsterLegs>();
		}

		public override void ArmorSetShadows(Player player)
		{
			player.armorEffectDrawOutlines = true;
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.WizardGangsterArmor2");

			if (player.HasItem(ItemID.PlatinumCoin))
			{
				player.manaCost -= 0.15f;
			}
			else
			{
				float bonusPerGold = 0.015f;
				int numGoldCoins = player.CountItem(ItemID.GoldCoin, 10);
                player.manaCost -= bonusPerGold * numGoldCoins;
			}
        }

		public override void UpdateEquip(Player player) 
		{
			player.GetCritChance(DamageClass.Magic) += 3;
		}

		public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor)
		{
			glowMaskColor = Color.White;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.GoldBar, 8)
			.AddIngredient(ItemID.Silk, 8)
			.AddIngredient(ModContent.ItemType<SpookyGlowshroom>(), 20)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}

	public class WizardGangsterHead2Layer : HelmetGlowmaskVanityLayer
	{
		protected override int ID => 0;
		protected override EquipType Type => EquipType.Head;
		public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Head);
	}
}