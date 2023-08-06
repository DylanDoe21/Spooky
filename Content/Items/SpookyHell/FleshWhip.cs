using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell
{
    public class FleshWhip : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 20;
			Item.DamageType = DamageClass.SummonMeleeSpeed;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.width = 42;
            Item.height = 42;
			Item.useTime = 45;
			Item.useAnimation = 45;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 3);
			Item.UseSound = SoundID.Item152;
			Item.shoot = ModContent.ProjectileType<FleshWhipProj>();
			Item.shootSpeed = 2f;
        }

		public override bool MeleePrefix() 
		{
			return true;
		}
    }
}