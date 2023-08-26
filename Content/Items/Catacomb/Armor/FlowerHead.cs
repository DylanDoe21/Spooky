using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Items.Catacomb.Misc;
using Spooky.Content.Projectiles.Catacomb;

namespace Spooky.Content.Items.Catacomb.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class FlowerHead : ModItem, IExtendedHelmet
	{
		public string ExtensionTexture => "Spooky/Content/Items/Catacomb/Armor/FlowerHead_RealHead";
        public Vector2 ExtensionSpriteOffset(PlayerDrawSet drawInfo) => new Vector2(0, -8f);

		public override void SetDefaults() 
		{
			Item.defense = 8;
			Item.width = 38;
			Item.height = 36;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 1);
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<FlowerBody>() && legs.type == ModContent.ItemType<FlowerLegs>();
		}

        public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.FlowerArmor");
            player.GetModPlayer<SpookyPlayer>().FlowerArmorSet = true;
            player.lifeRegen += 5;
		}

		public override void UpdateEquip(Player player)
        {
			player.manaCost -= 0.15f;
			player.maxMinions += 2;
			player.maxTurrets += 1;
        }

        public override void Load()
        {
            On_Player.KeyDoubleTap += ActivateSetBonus;
        }

        private void ActivateSetBonus(On_Player.orig_KeyDoubleTap orig, Player player, int keyDir)
        {
            if (keyDir == 0 && player.GetModPlayer<SpookyPlayer>().FlowerArmorSet && !player.HasBuff(ModContent.BuffType<FlowerArmorCooldown>()))
            {
				SoundEngine.PlaySound(SoundID.DD2_BookStaffCast, player.Center);

				for (int numProjectiles = 0; numProjectiles < 12; numProjectiles++)
                {
                    Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center.X + Main.rand.Next(-30, 30), 
					player.Center.Y + Main.rand.Next(-30, 30), 0, 0, ModContent.ProjectileType<FlowerArmorPollen>(), 55, 2f, Main.myPlayer);
                }

				player.AddBuff(ModContent.BuffType<FlowerArmorCooldown>(), 1800);
            }

            orig(player, keyDir);
        }

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<PlantChunk>(), 20)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}