using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Items.SpiderCave.Misc;
using Spooky.Content.Projectiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave
{
    public class SporeBall : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.mana = 25;
			Item.DamageType = DamageClass.Magic;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 60;
			Item.useAnimation = 60;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 15);
            Item.UseSound = SoundID.Item8;
            Item.shoot = ModContent.ProjectileType<SporeBallProj>();
            Item.shootSpeed = 10f;
        }

        //spawn some dusts
        public override bool? UseItem(Player player)
		{
            Vector2 ShootSpeed = Main.MouseWorld - player.Center;
            ShootSpeed.Normalize();
            ShootSpeed *= 16f;

            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 20f;

            for (int numDusts = 0; numDusts < 12; numDusts++)
            {
                Dust dust = Dust.NewDustPerfect(new Vector2(player.Center.X + muzzleOffset.X, player.Center.Y + muzzleOffset.Y), Main.rand.NextBool() ? DustID.YellowTorch : DustID.BlueTorch,
                new Vector2(ShootSpeed.X + Main.rand.Next(-7, 8), ShootSpeed.Y + Main.rand.Next(-7, 8)) * 0.5f, default, default, 2f);
                dust.noGravity = true;
                dust.noLight = true;
                dust.velocity += player.velocity;
            }

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<MiteMandibles>(), 25)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}