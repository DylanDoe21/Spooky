using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class WizardGangsterHead : ModItem, IExtendedHelmet
	{
		public string ExtensionTexture => "Spooky/Content/Items/SpookyBiome/Armor/WizardGangsterHead_Hat";
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
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.WizardGangsterArmor");
			player.manaCost -= 0.10f;
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetCritChance(DamageClass.Magic) += 2;
		}

		public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor)
		{
			glowMaskColor = Color.White;
		}

		/*
		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<SpookyWoodItem>(), 20)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
		*/
	}

	public class WizardGangsterHeadLayer : HelmetGlowmaskVanityLayer
	{
		protected override int ID => 10;
		protected override EquipType Type => EquipType.Head;
		public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Head);
	}
}